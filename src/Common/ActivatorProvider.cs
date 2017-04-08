/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2017 Zongsoft Corporation <http://www.zongsoft.com>
 *
 * This file is part of Zongsoft.CoreLibrary.
 *
 * Zongsoft.CoreLibrary is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 *
 * Zongsoft.CoreLibrary is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
 * Lesser General Public License for more details.
 *
 * The above copyright notice and this permission notice shall be
 * included in all copies or substantial portions of the Software.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with Zongsoft.CoreLibrary; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Zongsoft.Common
{
	public class ActivatorProvider
	{
		#region 单例字段
		public static readonly ActivatorProvider Default = new ActivatorProvider();
		#endregion

		#region 成员字段
		private ICollection<IActivator> _activators;
		#endregion

		#region 构造函数
		public ActivatorProvider()
		{
			_activators = new HashSet<IActivator>(new IActivator[] { DictionaryActivator.Instance });
		}

		public ActivatorProvider(IEnumerable<IActivator> activators)
		{
			if(activators == null)
				throw new ArgumentNullException(nameof(activators));

			_activators = new HashSet<IActivator>(activators);

			if(!_activators.Contains(DictionaryActivator.Instance))
				_activators.Add(DictionaryActivator.Instance);
		}
		#endregion

		#region 公共属性
		public virtual ICollection<IActivator> Activators
		{
			get
			{
				return _activators;
			}
		}
		#endregion

		#region 公共方法
		public object CreateInstance(Type type, object source)
		{
			if(type == null)
				throw new ArgumentNullException(nameof(type));

			var activators = this.Activators;

			if(activators != null)
			{
				foreach(var activator in activators)
				{
					if(activator != null && activator.CanCreate(type, source))
						return activator.Create(type, source);
				}
			}

			return System.Activator.CreateInstance(type);
		}
		#endregion

		#region 嵌套子类
		private class DictionaryActivator : IActivator
		{
			#region 单例字段
			public static readonly DictionaryActivator Instance = new DictionaryActivator();
			#endregion

			#region 私有构造
			private DictionaryActivator()
			{
			}
			#endregion

			#region 公共方法
			public bool CanCreate(Type type, object source)
			{
				return type != null && source != null && source.GetType().IsDictionary();
			}

			public object Create(Type type, object source)
			{
				if(type == null)
					throw new ArgumentNullException(nameof(type));

				if(source == null || !source.GetType().IsDictionary())
					return null;

				var constructors = type.GetConstructors(BindingFlags.Public).OrderByDescending(p => p.GetParameters().Length);

				foreach(var constructor in constructors)
				{
					object[] values;

					if(this.TryGetParameters(constructor, source, out values))
						return constructor.Invoke(values);
				}

				return null;
			}
			#endregion

			#region 私有方法
			private bool TryGetParameters(ConstructorInfo constructor, object source, out object[] values)
			{
				var parameters = constructor.GetParameters();
				values = new object[parameters.Length];

				for(int i = 0; i < parameters.Length; i++)
				{
					var found = false;

					foreach(var entry in (IEnumerable)source)
					{
						//获取当前字典条目的键
						var key = entry.GetType().GetProperty("Key", BindingFlags.Public | BindingFlags.Instance).GetValue(entry)?.ToString();

						//判断当前参数名是否与字典条目的键相同
						found = string.Equals(key, parameters[i].Name, StringComparison.OrdinalIgnoreCase);

						if(found)
							values[i] = entry.GetType().GetProperty("Value", BindingFlags.Public | BindingFlags.Instance).GetValue(entry);
					}

					if(!found)
						return false;
				}

				return true;
			}
			#endregion
		}
		#endregion
	}
}

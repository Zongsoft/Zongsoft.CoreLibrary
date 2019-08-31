/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2010-2018 Zongsoft Corporation <http://www.zongsoft.com>
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

namespace Zongsoft.Data
{
	/// <summary>
	/// 提供数据访问名映射功能的类。
	/// </summary>
	public class DataAccessNaming : IDataAccessNaming, ICollection<KeyValuePair<Type, string>>
	{
		#region 成员字段
		private readonly IDictionary<Type, string> _mapping;
		#endregion

		#region 构造函数
		public DataAccessNaming()
		{
			_mapping = new Dictionary<Type, string>();
		}
		#endregion

		#region 公共属性
		public int Count
		{
			get
			{
				return _mapping.Count;
			}
		}
		#endregion

		#region 公共方法
		public void Map(Type type, string name = null)
		{
			if(type == null)
				throw new ArgumentNullException(nameof(type));

			_mapping[type] = string.IsNullOrEmpty(name) ? GetName(type) : name;
		}

		public void Map<T>(string name = null)
		{
			this.Map(typeof(T), name);
		}

		public string Get(Type type)
		{
			if(type == null)
				throw new ArgumentNullException(nameof(type));

			if(_mapping.TryGetValue(type, out var name))
				return name;

			//将非空的名字注册到映射表中
			return _mapping[type] = GetName(type);
		}

		public string Get<T>()
		{
			return this.Get(typeof(T));
		}
		#endregion

		#region 静态方法
		internal static string GetName(Type type)
		{
			//如果该类型应用了数据访问注解，则返回注解中声明的名字
			if(TryGetNameFromAttribute(type, out var name))
				return name;

			name = type.Name;

			if(type.IsGenericType)
			{
				//如果类型的声明类型为空，很可能它是一个匿名类（匿名类必定是一个泛型类）
				if(type.DeclaringType == null)
				{
					foreach(var attribute in type.CustomAttributes)
					{
						//如果指定的类型是一个匿名类，则返回空（因为匿名类名没有意义）
						if(attribute.AttributeType == typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute))
							return null;
					}
				}

				//去掉泛型类名中关于泛型参数数量的后缀部分
				name = type.Name.Substring(0, type.Name.IndexOf('`'));
			}

			//处理接口命名的规范
			if(type.IsInterface && name.Length > 1 && name[0] == 'I' && char.IsUpper(name[1]))
				name = name.Substring(1);
			//处理抽象类命名的规范
			else if(type.IsAbstract && name.Length > 4 && name.EndsWith("Base"))
				name = name.Substring(0, name.Length - 4);

			return name;
		}

		private static bool TryGetNameFromAttribute(Type type, out string name)
		{
			name = null;

			foreach(var attribute in type.CustomAttributes)
			{
				if(attribute.AttributeType == typeof(ModelAttribute))
				{
					switch(attribute.ConstructorArguments.Count)
					{
						case 1:
							if(attribute.ConstructorArguments[0].ArgumentType == typeof(string))
							{
								name = (string)attribute.ConstructorArguments[0].Value;
								return true;
							}

							break;
					}
				}
			}

			return false;
		}
		#endregion

		#region 集合成员
		bool ICollection<KeyValuePair<Type, string>>.IsReadOnly
		{
			get
			{
				return false;
			}
		}

		void ICollection<KeyValuePair<Type, string>>.Add(KeyValuePair<Type, string> item)
		{
			this.Map(item.Key, item.Value);
		}

		void ICollection<KeyValuePair<Type, string>>.Clear()
		{
			_mapping.Clear();
		}

		bool ICollection<KeyValuePair<Type, string>>.Contains(KeyValuePair<Type, string> item)
		{
			return _mapping.TryGetValue(item.Key, out var name) &&
			       string.Equals(name, item.Value, StringComparison.OrdinalIgnoreCase);
		}

		void ICollection<KeyValuePair<Type, string>>.CopyTo(KeyValuePair<Type, string>[] array, int arrayIndex)
		{
			_mapping.CopyTo(array, arrayIndex);
		}

		bool ICollection<KeyValuePair<Type, string>>.Remove(KeyValuePair<Type, string> item)
		{
			return _mapping.Remove(item);
		}

		public IEnumerator<KeyValuePair<Type, string>> GetEnumerator()
		{
			return _mapping.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _mapping.GetEnumerator();
		}
		#endregion
	}
}

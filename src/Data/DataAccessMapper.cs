/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2010-2017 Zongsoft Corporation <http://www.zongsoft.com>
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
using System.Collections.Concurrent;

namespace Zongsoft.Data
{
	/// <summary>
	/// 提供数据访问名的映射功能。
	/// </summary>
	public class DataAccessMapper
	{
		#region 成员字段
		private ConcurrentDictionary<Type, string> _mapping;
		#endregion

		#region 构造函数
		public DataAccessMapper()
		{
			_mapping = new ConcurrentDictionary<Type, string>();
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

			if(string.IsNullOrWhiteSpace(name))
			{
				var attribute = (DataAccessAttribute)Attribute.GetCustomAttribute(type, typeof(DataAccessAttribute), false);

				if(attribute == null || string.IsNullOrWhiteSpace(attribute.Name))
					name = type.Name;
				else
					name = attribute.Name;
			}

			_mapping[type] = name.Trim();
		}

		public void Map<T>(string name = null)
		{
			this.Map(typeof(T), name);
		}

		public string Get(Type type)
		{
			if(type == null)
				throw new ArgumentNullException(nameof(type));

			string result;

			if(_mapping.TryGetValue(type, out result))
				return result;

			var attribute = (DataAccessAttribute)Attribute.GetCustomAttribute(type, typeof(DataAccessAttribute), false);

			if(attribute != null && !string.IsNullOrWhiteSpace(attribute.Name))
			{
				_mapping[type] = attribute.Name;
				return attribute.Name;
			}

			return null;
		}

		public string Get<T>()
		{
			return this.Get(typeof(T));
		}
		#endregion
	}
}

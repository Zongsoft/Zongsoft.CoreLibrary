/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2016 Zongsoft Corporation <http://www.zongsoft.com>
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

namespace Zongsoft.Collections
{
	public struct KeyValuePair
	{
		#region 成员字段
		private string _key;
		private object _value;
		#endregion

		#region 构造函数
		public KeyValuePair(string key, object value)
		{
			if(key == null)
				throw new ArgumentNullException("key");

			_key = key;
			_value = value;
		}
		#endregion

		#region 公共属性
		public string Key
		{
			get
			{
				return _key;
			}
		}

		public object Value
		{
			get
			{
				return _value;
			}
		}
		#endregion

		#region 重写方法
		public override string ToString()
		{
			return _key + "=" + _value;
		}
		#endregion

		#region 静态方法
		public static KeyValuePair[] CreatePairs(string[] keys, params object[] values)
		{
			if(keys == null)
				throw new ArgumentNullException("keys");

			var result = new KeyValuePair[keys.Length];

			for(int i = 0; i < result.Length; i++)
			{
				result[i] = new KeyValuePair(keys[0], (values != null && i < values.Length ? values[i] : null));
			}

			return result;
		}

		public static KeyValuePair[] CreatePairs(object[] values, params string[] keys)
		{
			if(keys == null)
				throw new ArgumentNullException("keys");

			var result = new KeyValuePair[keys.Length];

			for(int i = 0; i < result.Length; i++)
			{
				result[i] = new KeyValuePair(keys[0], (values != null && i < values.Length ? values[i] : null));
			}

			return result;
		}
		#endregion
	}
}

/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2003-2014 Zongsoft Corporation <http://www.zongsoft.com>
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
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace Zongsoft.Runtime.Caching
{
	public class MemoryDictionaryCache : IDictionaryCache
	{
		#region 成员字段
		private readonly ConcurrentDictionary<string, object> _cache = new ConcurrentDictionary<string, object>();
		#endregion

		#region 公共属性
		public long Count
		{
			get
			{
				return _cache.Count;
			}
		}
		#endregion

		#region 公共方法
		public object GetValue(string key)
		{
			if(string.IsNullOrWhiteSpace(key))
				throw new ArgumentNullException("key");

			object result;

			if(_cache.TryGetValue(key.ToLowerInvariant(), out result))
				return result;

			return null;
		}

		public void SetValue(string key, object value)
		{
			if(string.IsNullOrWhiteSpace(key))
				throw new ArgumentNullException("key");

			if(value != null)
				_cache[key.ToLowerInvariant()] = value;
		}

		public void Remove(string key)
		{
			if(key != null)
				((IDictionary<string, object>)_cache).Remove(key);
		}

		public void Clear()
		{
			_cache.Clear();
		}
		#endregion
	}
}

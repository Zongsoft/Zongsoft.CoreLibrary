/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2014 Zongsoft Corporation <http://www.zongsoft.com>
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
using System.Threading;

namespace Zongsoft.Common
{
	public class ObjectCache<T> : MarshalByRefObject, IDisposable, IEnumerable<T> where T : class
	{
		#region 私有字段
		private int _limit;
		private ConcurrentDictionary<string, ObjectCacheItem> _cache;
		private string[] _keys;
		#endregion

		#region 构造函数
		public ObjectCache(int limit = 31)
		{
			_limit = Math.Max(10, limit);
			_keys = new string[_limit];
			_cache = new ConcurrentDictionary<string, ObjectCacheItem>();
		}
		#endregion

		#region 公共属性
		public int Count
		{
			get
			{
				var cache = _cache;

				if(cache != null)
					return cache.Count;

				return 0;
			}
		}

		public int MaximumLimit
		{
			get
			{
				return _limit;
			}
		}
		#endregion

		#region 公共方法
		public T Get(string name, Func<string, T> valueFactory)
		{
			if(string.IsNullOrWhiteSpace(name))
				throw new ArgumentNullException("name");

			var cache = _cache;

			if(cache == null)
				throw new ObjectDisposedException(this.GetType().Name);

			var keys = _keys;

			var item = cache.GetOrAdd(name, key =>
			{
				if(cache.Count >= _limit)
				{
					var removedKey = keys[keys.Length - 1];

					if(removedKey != null)
						((IDictionary<string, ObjectCacheItem>)cache).Remove(removedKey);
				}

				var value = valueFactory(key);
				return new ObjectCacheItem(value);
			});

			var index = item.Index < 0 ? keys.Length - 1 : item.Index;

			for(int i = index; i > 0; i--)
			{
				keys[i] = keys[i - 1];
			}

			item.Index = 0;
			keys[0] = name;

			return item.Value as T;
		}

		public T Remove(string name)
		{
			if(string.IsNullOrWhiteSpace(name))
				throw new ArgumentNullException("name");

			var cache = _cache;

			if(cache == null)
				throw new ObjectDisposedException(this.GetType().Name);

			var keys = _keys;
			ObjectCacheItem item;

			if(cache.TryRemove(name, out item))
			{
				if(item.Index >= 0 && keys != null && keys.Length > 0)
				{
					for(int i = item.Index; i < keys.Length - 1; i++)
					{
						keys[i] = keys[i + 1];
					}

					keys[keys.Length - 1] = null;
				}

				return item.Value;
			}

			return null;
		}

		public void Clear()
		{
			var cache = _cache;

			if(cache == null)
				throw new ObjectDisposedException(this.GetType().Name);

			this.Clear(cache);
		}

		private void Clear(IDictionary<string, ObjectCacheItem> cache)
		{
			if(cache == null)
				return;

			cache.Clear();

			var keys = _keys;

			if(keys != null)
				Array.Clear(keys, 0, keys.Length);
		}
		#endregion

		#region 释放资源
		protected virtual void Dispose(bool disposing)
		{
			var cache = System.Threading.Interlocked.Exchange(ref _cache, null);
			this.Clear(cache);
			_keys = null;
		}

		public void Dispose()
		{
			this.Dispose(true);
		}
		#endregion

		#region 枚举遍历
		public IEnumerator<T> GetEnumerator()
		{
			var cache = _cache;

			if(cache == null)
				throw new ObjectDisposedException(this.GetType().Name);

			var keys = _keys;

			if(keys == null)
				yield break;

			foreach(var key in keys)
				yield return cache[key].Value;
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}
		#endregion

		#region 嵌套子类
		private class ObjectCacheItem
		{
			public ObjectCacheItem(T value)
			{
				this.Index = -1;
				this.Value = value;
			}

			public int Index;
			public T Value;
		}
		#endregion
	}
}

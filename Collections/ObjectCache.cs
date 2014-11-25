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

namespace Zongsoft.Collections
{
	/// <summary>
	/// 提供了对象缓存的功能。
	/// </summary>
	/// <typeparam name="T">缓存的对象类型。</typeparam>
	public class ObjectCache<T> : MarshalByRefObject, IDisposable, IEnumerable<T> where T : class
	{
		#region 事件定义
		public event EventHandler<CollectionRemovedEventArgs> Removed;
		#endregion

		#region 私有字段
		private int _limit;
		private ConcurrentDictionary<string, ObjectCacheItem> _cache;
		private string[] _keys;
		#endregion

		#region 构造函数
		/// <summary>
		/// 创建一个对象缓存容器，默认限制数为31。
		/// </summary>
		public ObjectCache() : this(null, 31)
		{
		}

		/// <summary>
		/// 创建一个对象缓存容器。
		/// </summary>
		/// <param name="limit">指定的最大缓存数，如果为零则表示不做限制。</param>
		public ObjectCache(int limit) : this(null, limit)
		{
		}

		/// <summary>
		/// 创建一个对象缓存容器。
		/// </summary>
		/// <param name="comparer">对键进行比较时要使用的<see cref="System.Collections.Generic.IEqualityComparer<System.String>"/>比较器。</param>
		/// <param name="limit">指定的最大缓存数，如果为零则表示不做限制。</param>
		public ObjectCache(IEqualityComparer<string> comparer, int limit = 31)
		{
			if(limit > 0)
			{
				_limit = Math.Max(10, limit);
				_keys = new string[_limit];
			}

			if(comparer == null)
				_cache = new ConcurrentDictionary<string, ObjectCacheItem>();
			else
				_cache = new ConcurrentDictionary<string, ObjectCacheItem>(comparer);
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取缓存项目的个数。
		/// </summary>
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

		/// <summary>
		/// 获取缓存容器的最大活动条目的限制数，超过该数量的将自动按顺序覆盖最不活跃的缓存项。
		/// </summary>
		/// <remarks>
		///		<para>如果最大限制数为零，则表示不做限制。那么该缓存容器将退化成一个线程安全的字典。</para>
		/// </remarks>
		public int MaximumLimit
		{
			get
			{
				return _limit;
			}
		}
		#endregion

		#region 公共方法
		/// <summary>
		/// 获取指定名称的缓存项。
		/// </summary>
		/// <param name="name">指定要获取的缓存项名称。</param>
		/// <param name="valueFactory">用于为指定缓存键生成缓存项的函数，如果为空(null)则不会为不存在的键创建缓存项。</param>
		/// <returns>返回获取到的缓存项，如果返回值为空(null)并且<paramref name="valueFactory"/>参数为空，则表示没有获取到对应的</returns>
		public T Get(string name, Func<string, T> valueFactory = null)
		{
			if(string.IsNullOrWhiteSpace(name))
				throw new ArgumentNullException("name");

			var cache = _cache;

			if(cache == null)
				throw new ObjectDisposedException(this.GetType().Name);

			if(valueFactory == null)
			{
				ObjectCacheItem result;

				if(cache.TryGetValue(name, out result) && result != null)
					return result.Value;

				return null;
			}

			var keys = _keys;

			var item = cache.GetOrAdd(name, key =>
			{
				if(_limit > 0 && cache.Count >= _limit)
				{
					var removedKey = keys[keys.Length - 1];

					if(removedKey != null)
					{
						ObjectCacheItem removedItem;

						//如果删除成功则激发“Removed”事件
						if(_cache.TryRemove(removedKey, out removedItem))
							this.OnRemoved(new CollectionRemovedEventArgs(CollectionRemovedReason.Overflow, removedItem.Value));
					}
				}

				var value = valueFactory(key);
				return new ObjectCacheItem(value);
			});

			if(_limit > 0)
			{
				var index = item.Index < 0 ? Math.Min(cache.Count, keys.Length - 1) : item.Index;

				for(int i = index; i > 0; i--)
				{
					keys[i] = keys[i - 1];

					if(keys[i] != null)
						cache[keys[i]].Index = i;
				}

				item.Index = 0;
				keys[0] = name;
			}

			return item.Value as T;
		}

		/// <summary>
		/// 删除指定名称的缓存项。
		/// </summary>
		/// <param name="name">指定要删除的缓存项名称。</param>
		/// <returns>返回删除后的缓存项对象，如果删除失败则返回空(null)。</returns>
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

						if(keys[i] != null)
							cache[keys[i]].Index = i;
					}

					keys[keys.Length - 1] = null;
				}

				//激发“Removed”事件
				this.OnRemoved(new CollectionRemovedEventArgs(CollectionRemovedReason.Remove, item.Value));

				return item.Value;
			}

			return null;
		}

		/// <summary>
		/// 清空缓存容器中的所有内容。
		/// </summary>
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

		#region 激发事件
		protected virtual void OnRemoved(CollectionRemovedEventArgs args)
		{
			var removed = this.Removed;

			if(removed != null)
				removed(this, args);
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

﻿/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2013-2016 Zongsoft Corporation <http://www.zongsoft.com>
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

namespace Zongsoft.Runtime.Caching
{
	public class MemoryCache : ICache, Zongsoft.Common.ISequence
	{
		#region 单例字段
		public static readonly MemoryCache Default = new MemoryCache("Zongsoft.Runtime.Caching.MemoryCache");
		#endregion

		#region 事件声明
		public event EventHandler<CacheChangedEventArgs> Changed;
		#endregion

		#region 成员字段
		private System.Runtime.Caching.MemoryCache _innerCache;
		#endregion

		#region 构造函数
		public MemoryCache(string name)
		{
			if(string.IsNullOrWhiteSpace(name))
				throw new ArgumentNullException("name");

			_innerCache = new System.Runtime.Caching.MemoryCache(name);
		}
		#endregion

		#region 公共属性
		public string Name
		{
			get
			{
				return _innerCache.Name;
			}
		}

		public long Count
		{
			get
			{
				return _innerCache.GetCount();
			}
		}

		public bool IsDisposed
		{
			get
			{
				return _innerCache == null;
			}
		}
		#endregion

		#region 公共方法
		public bool Exists(string key)
		{
			return _innerCache.Contains(key);
		}

		public TimeSpan? GetExpiry(string key)
		{
			throw new NotSupportedException();
		}

		public void SetExpiry(string key, TimeSpan duration)
		{
			throw new NotSupportedException();
		}

		public T GetValue<T>(string key)
		{
			return Zongsoft.Common.Convert.ConvertValue<T>(this.GetValue(key));
		}

		public object GetValue(string key)
		{
			return _innerCache.Get(key);
		}

		public object GetValue(string key, Func<string, CacheEntry> valueCreator)
		{
			if(valueCreator == null)
				return _innerCache.Get(key);

			var entry = valueCreator(key);

			return _innerCache.AddOrGetExisting(key, entry.Value, new System.Runtime.Caching.CacheItemPolicy()
			{
				SlidingExpiration = entry.Expiry ?? System.Runtime.Caching.ObjectCache.NoSlidingExpiration,
				//UpdateCallback = this.OnUpdateCallback,
				RemovedCallback = this.OnRemovedCallback,
			});
		}

		public bool SetValue(string key, object value)
		{
			return this.SetValue(key, value, TimeSpan.Zero);
		}

		public bool SetValue(string key, object value, TimeSpan duration, bool requiredNotExists = false)
		{
			if(requiredNotExists)
			{
				var exists = _innerCache.Contains(key);

				if(exists)
					return false;
			}

			if(duration == TimeSpan.Zero)
				_innerCache.Set(key, value, new System.Runtime.Caching.CacheItemPolicy()
				{
					AbsoluteExpiration = System.Runtime.Caching.ObjectCache.InfiniteAbsoluteExpiration,
					//UpdateCallback = this.OnUpdateCallback,
					RemovedCallback = this.OnRemovedCallback,
				});
			else
				_innerCache.Set(key, value, new System.Runtime.Caching.CacheItemPolicy()
				{
					SlidingExpiration = duration,
					//UpdateCallback = this.OnUpdateCallback,
					RemovedCallback = this.OnRemovedCallback,
				});

			return true;
		}

		public bool SetValue(string key, object value, DateTime expires, bool requiredNotExists = false)
		{
			if(requiredNotExists)
			{
				var exists = _innerCache.Contains(key);

				if(exists)
					return false;
			}

			if(expires < DateTime.Now)
				_innerCache.Set(key, value, new System.Runtime.Caching.CacheItemPolicy()
				{
					AbsoluteExpiration = System.Runtime.Caching.ObjectCache.InfiniteAbsoluteExpiration,
					//UpdateCallback = this.OnUpdateCallback,
					RemovedCallback = this.OnRemovedCallback,
				});
			else
				_innerCache.Set(key, value, new System.Runtime.Caching.CacheItemPolicy()
				{
					AbsoluteExpiration = expires.ToUniversalTime(),
					//UpdateCallback = this.OnUpdateCallback,
					RemovedCallback = this.OnRemovedCallback,
				});

			return true;
		}

		public bool Rename(string key, string newKey)
		{
			if(string.IsNullOrWhiteSpace(newKey))
				throw new ArgumentNullException("newKey");

			var orignalValue = _innerCache.Remove(key);

			if(orignalValue != null)
				_innerCache.Add(newKey, orignalValue, System.Runtime.Caching.ObjectCache.InfiniteAbsoluteExpiration);

			return orignalValue != null;
		}

		public bool Remove(string key)
		{
			return _innerCache.Remove(key) != null;
		}

		public void Clear()
		{
			_innerCache.Trim(100);

			//System.Runtime.Caching.MemoryCache 没有Clear方法，它的Trim()也不一定会回收缓存项
			throw new NotSupportedException();
		}
		#endregion

		#region 缓存事件
		private void OnUpdateCallback(System.Runtime.Caching.CacheEntryUpdateArguments args)
		{
			this.OnChanged(new CacheChangedEventArgs(CacheChangedReason.Updated, args.Key, null, args.UpdatedCacheItem.Key, args.UpdatedCacheItem.Value));
		}

		private void OnRemovedCallback(System.Runtime.Caching.CacheEntryRemovedArguments args)
		{
			this.OnChanged(new CacheChangedEventArgs(this.ConvertReason(args.RemovedReason), args.CacheItem.Key, args.CacheItem.Value));
		}
		#endregion

		#region 激发事件
		protected virtual void OnChanged(CacheChangedEventArgs args)
		{
			var changed = this.Changed;

			if(changed != null)
				changed(this, args);
		}
		#endregion

		#region 私有方法
		private CacheChangedReason ConvertReason(System.Runtime.Caching.CacheEntryRemovedReason reason)
		{
			switch(reason)
			{
				case System.Runtime.Caching.CacheEntryRemovedReason.Expired:
					return CacheChangedReason.Expired;
				case System.Runtime.Caching.CacheEntryRemovedReason.Removed:
					return CacheChangedReason.Removed;
				case System.Runtime.Caching.CacheEntryRemovedReason.ChangeMonitorChanged:
					return CacheChangedReason.Dependented;
				default:
					return CacheChangedReason.None;
			}
		}
		#endregion

		#region 处置方法
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			var cache = System.Threading.Interlocked.Exchange(ref _innerCache, null);

			if(cache != null)
				cache.Dispose();
		}
		#endregion

		#region 序列号器
		public long Increment(string key, int interval = 1, int seed = 0)
		{
			var value = this.GetValue(key);

			if(value == null)
			{
				this.SetValue(key, seed);
				return this.GetValue<long>(key);
			}

			long number;

			if(!Zongsoft.Common.Convert.TryConvertValue<long>(value, out number))
				throw new InvalidOperationException();

			number += interval;

			this.SetValue(key, number);

			return number;
		}

		public long Decrement(string key, int interval = 1, int seed = 0)
		{
			return this.Increment(key, -interval);
		}

		void Zongsoft.Common.ISequence.Reset(string key, int value)
		{
			this.SetValue(key, value);
		}
		#endregion
	}
}

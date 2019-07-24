/*
 *   _____                                ______
 *  /_   /  ____  ____  ____  _________  / __/ /_
 *    / /  / __ \/ __ \/ __ \/ ___/ __ \/ /_/ __/
 *   / /__/ /_/ / / / / /_/ /\_ \/ /_/ / __/ /_
 *  /____/\____/_/ /_/\__  /____/\____/_/  \__/
 *                   /____/
 *
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@qq.com>
 *
 * Copyright (C) 2016-2019 Zongsoft Corporation <http://www.zongsoft.com>
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
using System.Collections.Concurrent;
using System.Threading;

namespace Zongsoft.Runtime.Caching
{
	public abstract class StorageBase<T>
	{
		#region 成员字段
		private ConcurrentBag<T> _cache;
		private int _limit;
		private TimeSpan _duration;
		private Timer _timer;
		#endregion

		#region 构造函数
		protected StorageBase() : this(TimeSpan.Zero)
		{
		}

		protected StorageBase(TimeSpan duration, int limit = 256)
		{
			_duration = duration == TimeSpan.Zero ? TimeSpan.FromSeconds(60) : duration;
			_limit = Math.Max(10, limit);
			_cache = new ConcurrentBag<T>();
			_timer = new Timer(this.OnTick, null, _duration, _duration);
		}
		#endregion

		#region 公共方法
		public void Append(T data)
		{
			this.OnAppend(data);

			if(_cache.Count >= _limit)
				this.Flush();
		}

		public void Flush()
		{
			if(_cache == null || _cache.IsEmpty)
				return;

			this.OnPersist(new StorageEnumerable(_cache));
		}
		#endregion

		#region 抽象方法
		protected virtual void OnAppend(T data)
		{
			_cache.Add(data);
		}

		protected abstract void OnPersist(IEnumerable<T> items);
		#endregion

		#region 定时回调
		private void OnTick(object state)
		{
			this.Flush();
		}
		#endregion

		#region 嵌套子类
		private class StorageEnumerable : IEnumerable<T>
		{
			private ConcurrentBag<T> _cache;

			public StorageEnumerable(ConcurrentBag<T> cache)
			{
				_cache = cache;
			}

			public IEnumerator<T> GetEnumerator()
			{
				return new Iterator(_cache);
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return new Iterator(_cache);
			}

			private class Iterator : IEnumerator<T>
			{
				private T _current;
				private ConcurrentBag<T> _cache;

				public Iterator(ConcurrentBag<T> cache)
				{
					_cache = cache;
				}

				public T Current
				{
					get => _current;
				}

				object IEnumerator.Current
				{
					get => _current;
				}

				public bool MoveNext()
				{
					return _cache.TryTake(out _current);
				}

				public void Reset()
				{
					throw new NotImplementedException();
				}

				public void Dispose()
				{
				}
			}
		}
		#endregion
	}
}

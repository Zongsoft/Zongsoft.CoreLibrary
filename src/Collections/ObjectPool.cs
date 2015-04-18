/*
 * Authors:
 *   邓祥云(X.Z. Deng) <627825056@qq.com>
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com> Last modified date: 2015-04-18
 *
 * Copyright (C) 2008-2012 Zongsoft Corporation <http://www.zongsoft.com>
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
	/// 提供了一个线程安全的通用对象池的相关功能。
	/// </summary>
	public class ObjectPool<T> : MarshalByRefObject, IDisposable where T : class
	{
		#region 私有变量
		private ConcurrentBag<T> _idles;
		private Func<T> _creator;
		private Action<T> _remover;
		private int _maximumLimit;
		private SemaphoreSlim _semaphore;
		#endregion

		#region 构造函数
		protected ObjectPool() : this(0)
		{
		}

		protected ObjectPool(int maximumLimit)
		{
			_maximumLimit = Math.Max(maximumLimit, 0);
			_idles = new ConcurrentBag<T>();

			if(_maximumLimit > 0)
				_semaphore = new SemaphoreSlim(_maximumLimit, _maximumLimit);
		}

		/// <summary>
		/// 创建一个新的对象管理池。
		/// </summary>
		/// <param name="creator">对象的创建方法。</param>
		/// <param name="remover">对象移除时的回调，该参数值可以为空(null)。</param>
		public ObjectPool(Func<T> creator, Action<T> remover) : this(creator, remover, 0)
		{
		}

		/// <summary>
		/// 创建一个新的对象管理池。
		/// </summary>
		/// <param name="creator">对象的创建方法。</param>
		/// <param name="remover">对象移除时的回调，该参数值可以为空(null)。</param>
		/// <param name="maximumLimit">对象池的最大容量，如果小于一则表示不控制池的大小。</param>
		public ObjectPool(Func<T> creator, Action<T> remover, int maximumLimit)
		{
			if(creator == null)
				throw new ArgumentNullException("creator");

			_creator = creator;
			_remover = remover;
			_maximumLimit = Math.Max(maximumLimit, 0);
			_idles = new ConcurrentBag<T>();

			if(_maximumLimit > 0)
				_semaphore = new SemaphoreSlim(_maximumLimit, _maximumLimit);
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取对象池的可用空闲元素数量，负数表示未限定。
		/// </summary>
		public int Count
		{
			get
			{
				var idles = _idles;

				if(idles == null)
					throw new ObjectDisposedException(null);

				if(_semaphore == null || _maximumLimit < 1)
					return -1;

				return _maximumLimit - idles.Count;
			}
		}

		/// <summary>
		/// 获取对象池的最大容量，零表示不限制。
		/// </summary>
		public int MaximumLimit
		{
			get
			{
				return _maximumLimit;
			}
		}
		#endregion

		#region 公共方法
		/// <summary>
		/// 从对象池中获取一个可用对象。
		/// </summary>
		public T GetObject()
		{
			var idles = _idles;

			if(idles == null)
				throw new ObjectDisposedException(null);

			T item;

			if(_semaphore != null)
				_semaphore.Wait();

			if(!idles.TryTake(out item))
				item = this.OnCreate();

			//如果获取或者创建的新项为空，则释放一个信号量并返回空值
			if(item == null)
			{
				if(_semaphore != null)
					_semaphore.Release();

				return null;
			}

			//回调取出方法
			this.OnTakeout(item);

			return item;
		}

		/// <summary>
		/// 将一个对象释放到池中。
		/// </summary>
		public void Release(T item)
		{
			var idles = _idles;

			if(idles == null)
				throw new ObjectDisposedException(null);

			if(item == null)
				throw new ArgumentNullException("item");

			//回调放入方法
			this.OnTakein(item);

			idles.Add(item);

			if(_semaphore != null)
				_semaphore.Release();
		}

		/// <summary>
		/// 清空对象池，该方法会依次调用池中空闲对象的<see cref="OnRemove"/>方法。
		/// </summary>
		public void Clear()
		{
			var idles = _idles;

			if(idles == null)
				throw new ObjectDisposedException(null);

			T item;

			while(idles.TryTake(out item))
			{
				this.OnRemove(item);
			}
		}
		#endregion

		#region 虚拟方法
		protected virtual T OnCreate()
		{
			if(_creator == null)
				return Activator.CreateInstance<T>();

			return _creator();
		}

		/// <summary>
		/// 表示将一个对象放入当前缓存池时该方法被调用。
		/// </summary>
		/// <param name="value">被放入的那个缓存项对象。</param>
		protected virtual void OnTakein(T value)
		{
		}

		/// <summary>
		/// 表示当从当前缓存池中取出一个缓存项时该方法被调用。
		/// </summary>
		/// <param name="value">被取出的那个缓存项对象。</param>
		protected virtual void OnTakeout(T value)
		{
		}

		/// <summary>
		/// 表示删除当前缓存池中的一个缓存项时该方法被调用。
		/// </summary>
		/// <param name="value">被删除的那个缓存项对象。</param>
		protected virtual void OnRemove(T value)
		{
			if(_remover != null)
				_remover(value);

			IDisposable disposable = value as IDisposable;

			if(disposable != null)
				disposable.Dispose();
		}
		#endregion

		#region 释放资源
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if(!disposing)
				return;

			Clear();

			var idles = System.Threading.Interlocked.Exchange(ref _idles, null);

			if(idles != null)
			{
				_idles = null;
				_creator = null;
				_remover = null;

				if(_semaphore != null)
				{
					_semaphore.Dispose();
					_semaphore = null;
				}
			}
		}
		#endregion
	}
}
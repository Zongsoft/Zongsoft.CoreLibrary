/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2015 Zongsoft Corporation <http://www.zongsoft.com>
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

namespace Zongsoft.Common
{
	public class ObjectReference<T> : MarshalByRefObject, IDisposableObject, IObjectReference<T> where T : class
	{
		#region 事件定义
		public event EventHandler Disposed;
		#endregion

		#region 常量定义
		private const int NORMAL_STATE = 0;
		private const int INVALID_STATE = 1;
		private const int DISPOSED_STATE = 2;
		#endregion

		#region 同步变量
		private readonly object _syncRoot;
		#endregion

		#region 成员字段
		private T _target;
		private int _state;
		private Func<T> _targetFactory;
		#endregion

		#region 构造函数
		public ObjectReference(Func<T> targetFactory)
		{
			if(targetFactory == null)
				throw new ArgumentNullException("targetFactory");

			_targetFactory = targetFactory;
			_syncRoot = new Object();
		}
		#endregion

		#region 公共属性
		public bool IsDisposed
		{
			get
			{
				return _state == DISPOSED_STATE;
			}
		}

		public virtual bool HasTarget
		{
			get
			{
				if(_state == NORMAL_STATE)
					return _target != null;

				return false;
			}
		}

		public virtual T Target
		{
			get
			{
				if(_state == DISPOSED_STATE)
					throw new ObjectDisposedException(this.GetType().FullName);

				if(_state == INVALID_STATE || _target == null)
				{
					lock(_syncRoot)
					{
						if(_state == INVALID_STATE || _target == null)
						{
							var target = _targetFactory();

							if(target != null)
							{
								if(target is IDisposableObject)
									((IDisposableObject)target).Disposed += DisposableObject_Disposed;

								_target = target;
								_state = NORMAL_STATE;
							}
						}
					}
				}

				return _target;
			}
		}
		#endregion

		#region 公共方法
		public void Invalidate()
		{
			if(_state == DISPOSED_STATE)
				throw new ObjectDisposedException(this.GetType().FullName);

			var original = System.Threading.Interlocked.CompareExchange(ref _state, INVALID_STATE, NORMAL_STATE);

			if(original != NORMAL_STATE)
				return;

			var disposable = this.Target as IDisposable;

			//处置目标对象
			if(disposable != null)
				disposable.Dispose();

			//设置目标对象引用为空
			_target = null;
		}
		#endregion

		#region 激发事件
		protected virtual void OnDisposed(EventArgs args)
		{
			var disposed = this.Disposed;

			if(disposed != null)
				disposed(this, args);
		}
		#endregion

		#region 私有方法
		private void DisposableObject_Disposed(object sender, EventArgs e)
		{
			_target = null;

			var target = sender as IDisposableObject;

			if(target != null)
				target.Disposed -= DisposableObject_Disposed;
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
			var original = System.Threading.Interlocked.Exchange(ref _state, DISPOSED_STATE);

			if(original == DISPOSED_STATE)
				return;

			_state = DISPOSED_STATE;

			var disposable = _target as IDisposable;

			if(disposable != null)
				disposable.Dispose();

			_target = null;

			this.OnDisposed(EventArgs.Empty);
		}
		#endregion
	}
}

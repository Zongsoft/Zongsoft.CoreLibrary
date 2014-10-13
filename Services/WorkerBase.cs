/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2010-2013 Zongsoft Corporation <http://www.zongsoft.com>
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
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;

namespace Zongsoft.Services
{
	public abstract class WorkerBase : MarshalByRefObject, IWorker, IDisposable
	{
		#region 事件声明
		public event EventHandler<WorkerStateChangedEventArgs> StateChanged;
		#endregion

		#region 成员变量
		private string _name;
		private bool _disabled;
		private bool _canPauseAndContinue;
		private WorkerState _state;
		private int _isDisposed;
		#endregion

		#region 构造函数
		protected WorkerBase()
		{
			_name = this.GetType().Name;
			_disabled = false;
			_canPauseAndContinue = false;
			_state = WorkerState.Stopped;
		}

		protected WorkerBase(string name)
		{
			if(string.IsNullOrWhiteSpace(name))
				throw new ArgumentNullException("name");

			_name = name.Trim();
			_disabled = false;
			_canPauseAndContinue = false;
			_state = WorkerState.Stopped;
		}
		#endregion

		#region 公共属性
		public string Name
		{
			get
			{
				return _name;
			}
			protected set
			{
				if(string.IsNullOrWhiteSpace(value))
					throw new ArgumentNullException();

				_name = value;
			}
		}

		/// <summary>
		/// 获取或设置是否禁用当前工作器。
		/// </summary>
		/// <remarks>如果当前工作器</remarks>
		public bool Disabled
		{
			get
			{
				return _disabled;
			}
			set
			{
				if(_disabled == value)
					return;

				_disabled = value;

				if(value)
					this.Stop();
			}
		}

		/// <summary>
		/// 获取或设置工作器工作器是否可以暂停和继续。
		/// </summary>
		public bool CanPauseAndContinue
		{
			get
			{
				return _canPauseAndContinue;
			}
			protected set
			{
				if(_state != WorkerState.Stopped)
					throw new InvalidOperationException();

				_canPauseAndContinue = value;
			}
		}

		/// <summary>
		/// 获取工作器的状态。
		/// </summary>
		public WorkerState State
		{
			get
			{
				return _state;
			}
		}

		/// <summary>
		/// 获取一个值表示工作器是否已经被处置了。
		/// </summary>
		public bool IsDisposed
		{
			get
			{
				return _isDisposed != 0;
			}
		}
		#endregion

		#region 公共方法
		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.Synchronized)]
		public void Start(params string[] args)
		{
			if(this.IsDisposed)
				throw new ObjectDisposedException(_name);

			if(_disabled || _state != WorkerState.Stopped)
				return;

			//更新当前状态为“整体启动中”
			_state = WorkerState.Starting;

			try
			{
				//调用启动抽象方法，以执行实际的启动操作
				this.OnStart(args);

				//更新当前状态为“运行中”
				_state = WorkerState.Running;

				//激发“StateChanged”事件
				this.OnStateChanged(new WorkerStateChangedEventArgs("Start", WorkerState.Running));
			}
			catch(Exception ex)
			{
				_state = WorkerState.Stopped;

				//激发“StateChanged”事件
				this.OnStateChanged(new WorkerStateChangedEventArgs("Start", WorkerState.Stopped, ex));

				throw;
			}
		}

		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.Synchronized)]
		public void Stop(params string[] args)
		{
			if(this.IsDisposed)
				throw new ObjectDisposedException(_name);

			if(_disabled || _state == WorkerState.Stopping || _state == WorkerState.Stopped)
				return;

			//保存原来的状态
			var originalState = _state;

			//更新当前状态为“正在停止中”
			_state = WorkerState.Stopping;

			try
			{
				//调用停止抽象方法，以执行实际的停止操作
				this.OnStop(args);

				//更新当前状态为已停止
				_state = WorkerState.Stopped;

				//激发“StateChanged”事件
				this.OnStateChanged(new WorkerStateChangedEventArgs("Stop", WorkerState.Stopped));
			}
			catch(Exception ex)
			{
				//还原状态
				_state = originalState;

				//激发“StateChanged”事件
				this.OnStateChanged(new WorkerStateChangedEventArgs("Stop", originalState, ex));

				throw;
			}
		}

		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.Synchronized)]
		public void Pause()
		{
			if(this.IsDisposed)
				throw new ObjectDisposedException(_name);

			if(_disabled || (!_canPauseAndContinue))
				return;

			if(_state != WorkerState.Running)
				return;

			//保存原来的状态
			var originalState = _state;

			//更新当前状态为“正在暂停中”
			_state = WorkerState.Pausing;

			try
			{
				//执行暂停操作
				this.OnPause();

				//更新当前状态为“已经暂停”
				_state = WorkerState.Paused;

				//激发“StateChanged”事件
				this.OnStateChanged(new WorkerStateChangedEventArgs("Pause", WorkerState.Paused));
			}
			catch(Exception ex)
			{
				//还原状态
				_state = originalState;

				//激发“StateChanged”事件
				this.OnStateChanged(new WorkerStateChangedEventArgs("Pause", originalState, ex));

				throw;
			}
		}

		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.Synchronized)]
		public void Resume()
		{
			if(this.IsDisposed)
				throw new ObjectDisposedException(_name);

			if(_disabled || (!_canPauseAndContinue))
				return;

			if(_state != WorkerState.Paused)
				return;

			//保存原来的状态
			var originalState = _state;

			//更新当前状态为“正在恢复中”
			_state = WorkerState.Resuming;

			try
			{
				//执行恢复操作
				this.OnResume();

				_state = WorkerState.Running;

				//激发“StateChanged”事件
				this.OnStateChanged(new WorkerStateChangedEventArgs("Resume", WorkerState.Running));
			}
			catch(Exception ex)
			{
				//还原状态
				_state = originalState;

				//激发“StateChanged”事件
				this.OnStateChanged(new WorkerStateChangedEventArgs("Resume", originalState, ex));

				throw;
			}
		}
		#endregion

		#region 抽象方法
		protected abstract void OnStart(string[] args);
		protected abstract void OnStop(string[] args);

		protected virtual void OnPause()
		{
		}

		protected virtual void OnResume()
		{
		}
		#endregion

		#region 事件激发
		protected virtual void OnStateChanged(WorkerStateChangedEventArgs args)
		{
			var stateChanged = this.StateChanged;

			if(stateChanged != null)
				stateChanged(this, args);
		}
		#endregion

		#region 释放资源
		protected virtual void Dispose(bool disposing)
		{
			this.Stop();
		}

		void IDisposable.Dispose()
		{
			var isDisposed = System.Threading.Interlocked.CompareExchange(ref _isDisposed, 1, 0);

			if(isDisposed == 0)
				this.Dispose(true);
		}
		#endregion
	}
}

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
		public event EventHandler Started;
		public event CancelEventHandler Starting;
		public event EventHandler Stopped;
		public event CancelEventHandler Stopping;
		#endregion

		#region 成员变量
		private string _name;
		private bool _disabled;
		private WorkerState _state;
		#endregion

		#region 构造函数
		protected WorkerBase()
		{
			_name = this.GetType().Name;
			_disabled = false;
			_state = WorkerState.Stopped;
		}

		protected WorkerBase(string name)
		{
			if(string.IsNullOrWhiteSpace(name))
				throw new ArgumentNullException("name");

			_name = name.Trim();
			_disabled = false;
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

		public WorkerState State
		{
			get
			{
				return _state;
			}
		}
		#endregion

		#region 公共方法
		public void Start()
		{
			this.Start(new string[0]);
		}

		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.Synchronized)]
		public virtual void Start(string[] args)
		{
			if(_disabled || _state != WorkerState.Stopped)
				return;

			try
			{
				_state = WorkerState.Starting;

				//激发“Starting”事件，如果该事件处理程序返回真表示取消后续的启动操作
				if(this.OnStarting())
				{
					_state = WorkerState.Stopped;
					return;
				}
			}
			catch
			{
				_state = WorkerState.Stopped;
				throw;
			}

			try
			{
				//调用启动抽象方法，以执行实际的启动操作
				this.OnStart(args);

				//更新当前状态为“运行中”
				_state = WorkerState.Running;

				//激发“Started”事件
				this.OnStarted(EventArgs.Empty);
			}
			catch
			{
				_state = WorkerState.Stopped;
				throw;
			}
		}

		public void Stop()
		{
			this.Stop(new string[0]);
		}

		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.Synchronized)]
		public virtual void Stop(string[] args)
		{
			var state = _state;

			try
			{
				_state = WorkerState.Stopping;

				//激发“Stopping”事件，如果该事件处理程序返回真表示取消后续的停止操作
				if(this.OnStopping())
				{
					_state = state;
					return;
				}
			}
			catch
			{
				_state = state;
				throw;
			}

			try
			{
				//调用停止抽象方法，以执行实际的停止操作
				this.OnStop(args);

				//更新当前状态为已停止
				_state = WorkerState.Stopped;

				//激发“Stopped”事件
				this.OnStopped(EventArgs.Empty);
			}
			catch
			{
				_state = state;
				throw;
			}
		}
		#endregion

		#region 抽象方法
		protected abstract void OnStart(string[] args);
		protected abstract void OnStop(string[] args);
		#endregion

		#region 事件激发
		protected virtual void OnStarted(EventArgs args)
		{
			if(this.Started != null)
				this.Started(this, args);
		}

		protected bool OnStarting()
		{
			CancelEventArgs args = new CancelEventArgs();

			this.OnStarting(args);

			return args.Cancel;
		}

		protected virtual void OnStarting(CancelEventArgs args)
		{
			if(this.Starting != null)
				this.Starting(this, args);
		}

		protected virtual void OnStopped(EventArgs args)
		{
			if(this.Stopped != null)
				this.Stopped(this, args);
		}

		protected bool OnStopping()
		{
			CancelEventArgs args = new CancelEventArgs();

			this.OnStopping(args);

			return args.Cancel;
		}

		protected virtual void OnStopping(CancelEventArgs args)
		{
			if(this.Stopping != null)
				this.Stopping(this, args);
		}
		#endregion

		#region 释放资源
		protected virtual void Dispose(bool disposing)
		{
			this.Stop();
		}

		void IDisposable.Dispose()
		{
			this.Dispose(true);
		}
		#endregion
	}
}

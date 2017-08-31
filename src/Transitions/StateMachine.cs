/*
 * Authors:
 *   钟峰(Popeye Zhong) <9555843@qq.com>
 *
 * Copyright (C) 2017 Zongsoft Corporation <http://www.zongsoft.com>
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

namespace Zongsoft.Transitions
{
	public class StateMachine : IDisposable
	{
		#region 事件声明
		public event EventHandler<StateStepEventArgs> Stepping;
		public event EventHandler<StateStepEventArgs> Stepped;
		public event EventHandler<StateStopEventArgs> Stopping;
		public event EventHandler<StateStopEventArgs> Stopped;
		#endregion

		#region 成员字段
		private StateMachineOptions _options;
		#endregion

		#region 私有变量
		private int _isDisposed = 0;
		private int _isAborted = 0;
		private Stack<StateContextBase> _stack;
		#endregion

		#region 构造函数
		public StateMachine()
		{
			_options = new StateMachineOptions();
		}

		public StateMachine(StateMachineOptions options)
		{
			_options = options ?? new StateMachineOptions();
		}
		#endregion

		#region 公共属性
		public StateMachineOptions Options
		{
			get
			{
				return _options;
			}
			set
			{
				if(value == null)
					throw new ArgumentNullException();

				_options = value;
			}
		}
		#endregion

		#region 公共方法
		public void Abort()
		{
			//确认是否当前对象是否已被释放
			this.EnsureDisposed();

			//设置终止标记
			_isAborted = 1;

			var transaction = Zongsoft.Transactions.Transaction.Current;

			if(transaction != null)
				transaction.Rollback();
		}

		public StateMachine Run<TState>(TState destination, IDictionary<string, object> parameters = null) where TState : State
		{
			return this.Run(destination, _options, parameters);
		}

		public StateMachine Run<TState>(TState destination, StateMachineOptions options, IDictionary<string, object> parameters = null) where TState : State
		{
			//确认是否当前对象是否已被释放
			this.EnsureDisposed();

			//如果当前状态机已经被终止则返回
			if(_isAborted != 0)
				return this;

			//参数有效性检测
			if(destination == null)
				throw new ArgumentNullException(nameof(destination));

			//根据参数获取上下文对象
			var context = this.GetContext(destination, options ?? _options, parameters);

			//上下文获取失败则返回
			if(context == null)
				return this;

			//激发“Stepping”事件
			this.OnStepping(new StateStepEventArgs(context));

			//遍历状态图中的转换器，依次进行状态转换处理
			foreach(var transfer in destination.Diagram.Transfers)
			{
				if(transfer.CanTransfer(context))
				{
					//调用状态转换器进行状态转换
					transfer.Transfer(context);

					//将转换成功的状态压入状态栈
					_stack.Push(context);

					//遍历状态图中的所有触发器
					foreach(var trigger in destination.Diagram.Triggers)
					{
						//如果触发器可用，则激发触发器
						if(trigger.Enabled)
							trigger.OnTrigger(context);
					}
				}
			}

			//激发“Stepped”事件
			this.OnStepped(new StateStepEventArgs(context));

			//始终返回当前状态机
			return this;
		}
		#endregion

		#region 激发事件
		protected void OnStop()
		{
			var stack = _stack;

			if(stack == null || stack.Count == 0)
				return;

			var reason = _isAborted != 0 ? StateStopReason.Abortion : StateStopReason.Normal;

			//激发“Stopping”事件
			this.OnStopping(new StateStopEventArgs(reason));

			foreach(var frame in stack)
			{
				IDictionary<string, object> parameters = null;

				if(frame.HasParameters)
					parameters = frame.Parameters;

				frame.InnerDestination.Diagram.SetState(frame.InnerDestination, parameters);

				if(frame.OnStopInner != null)
				{
					frame.OnStopInner.DynamicInvoke(frame, reason);
				}
			}

			if(reason == StateStopReason.Normal)
			{
				var transaction = Zongsoft.Transactions.Transaction.Current;

				if(transaction != null)
					transaction.Commit();
			}

			//激发“Stopped”事件
			this.OnStopped(new StateStopEventArgs(reason));
		}

		protected virtual void OnStepping(StateStepEventArgs args)
		{
			var e = this.Stepping;

			if(e != null)
				e.Invoke(this, args);
		}

		protected virtual void OnStepped(StateStepEventArgs args)
		{
			var e = this.Stepped;

			if(e != null)
				e.Invoke(this, args);
		}

		protected virtual void OnStopping(StateStopEventArgs args)
		{
			var e = this.Stopping;

			if(e != null)
				e.Invoke(this, args);
		}

		protected virtual void OnStopped(StateStopEventArgs args)
		{
			var e = this.Stopped;

			if(e != null)
				e.Invoke(this, args);
		}
		#endregion

		#region 私有方法
		private State GetState(State state, out bool isTransfered)
		{
			isTransfered = false;

			if(_stack != null)
			{
				foreach(var frame in _stack)
				{
					isTransfered = frame.InnerDestination.GetType() == state.GetType() && frame.InnerDestination.Match(state);

					if(isTransfered)
						return frame.InnerDestination;
				}
			}

			return state.Diagram.GetState(state);
		}

		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.Synchronized)]
		private StateContextBase GetContext(State destination, StateMachineOptions options, IDictionary<string, object> parameters)
		{
			var isFirstStep = _stack == null;

			if(isFirstStep)
				_stack = new Stack<StateContextBase>();

			//获取指定状态实例的当前状态
			var origin = this.GetState(destination, out var isTransfered);

			//如果指定状态实例已经被处理过，则不能再次转换
			if(isTransfered)
				return null;

			//如果流程图未定义当前的流转向量，则退出或抛出异常
			if(!destination.Diagram.CanVectoring(origin, destination))
			{
				//如果是首次操作则抛出异常
				if(isFirstStep)
					throw new InvalidOperationException("Not supported state transfer.");

				//退出方法
				return null;
			}

			var contextType = typeof(StateContext<>).MakeGenericType(destination.GetType());
			return (StateContextBase)Activator.CreateInstance(contextType, new object[] { this, isFirstStep, origin, destination, options, parameters });
		}
		#endregion

		#region 处置方法
		private void EnsureDisposed()
		{
			if(_isDisposed != 0)
				throw new ObjectDisposedException(this.GetType().Name);
		}

		protected virtual void Dispose(bool disposing)
		{
			if(System.Threading.Interlocked.CompareExchange(ref _isDisposed, 1, 0) == 0)
				return;

			if(disposing)
			{
				//激发完成事件
				this.OnStop();

				if(_stack != null)
					_stack.Clear();
			}

			_stack = null;
		}

		void IDisposable.Dispose()
		{
			this.Dispose(true);
		}
		#endregion
	}
}

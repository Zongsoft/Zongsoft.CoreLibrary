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
		}

		public StateMachine Run<TState>(TState destination, IDictionary<string, object> parameters = null) where TState : State
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
			var context = this.GetContext(destination, parameters);

			//上下文获取失败则返回
			if(context == null)
				return this;

			//激发“Stepping”事件
			this.OnStepping(new StateStepEventArgs(context));

			//如果状态转换失败则取消后续动作
			if(!destination.Diagram.Transfer(context))
				return this;

			//将转换成功的上下文压入执行栈
			_stack.Push(context);

			//遍历状态图中的所有触发器
			foreach(var trigger in destination.Diagram.Triggers)
			{
				//如果触发器可用，则激发触发器
				if(trigger.Enabled)
					trigger.OnTrigger(context);
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

			Transactions.Transaction transaction = null;

			try
			{
				if(reason == StateStopReason.Normal && _options.AffiliateTransactionEnabled)
				{
					transaction = Transactions.Transaction.Current;

					if(transaction == null)
						transaction = new Transactions.Transaction();
				}

				//将执行栈转换为数组
				var frames = stack.ToArray();

				for(var index = frames.Length - 1; index >= 0; index--)
				{
					var frame = frames[index];
					IDictionary<string, object> parameters = null;

					if(frame.HasParameters)
						parameters = frame.Parameters;

					//如果上下文绑定了回调委托则调用该回调方法
					if(frame.StoppingThunk != null)
					{
						var result = frame.StoppingThunk.DynamicInvoke(frame, reason);

						if(result != null && result.GetType() == typeof(bool) && (bool)result)
							continue;
					}

					//将目标状态持久化
					frame.InnerDestination.Diagram.SetState(frame.InnerDestination, parameters);

					//如果上下文绑定了回调委托则调用该回调方法
					if(frame.StoppedThunk != null)
						frame.StoppedThunk.DynamicInvoke(frame, reason);
				}

				if(reason == StateStopReason.Normal && _options.AffiliateTransactionEnabled)
				{
					if(transaction != null)
						transaction.Commit();
				}
			}
			finally
			{
				if(transaction != null)
					transaction.Dispose();
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
		private StateContextBase GetContext(State destination, IDictionary<string, object> parameters)
		{
			var isFirstStep = _stack == null;

			if(isFirstStep)
				_stack = new Stack<StateContextBase>();

			//获取指定状态实例的当前状态
			var origin = this.GetState(destination, out var isTransfered);

			if(origin == null)
				throw new InvalidOperationException(string.Format("The '{0}' state is not existed.", destination));

			//如果指定状态实例已经被处理过或当前状态等于目标状态，则不能再次转换
			if(isTransfered || origin.Equals(destination))
				return null;

			//如果流程图定义了当前的流转向量则返回新建的上下文对象
			if(destination.Diagram.CanVectoring(origin, destination))
				return this.CreateContext(isFirstStep, origin, destination, parameters);

			//通知状态图失败
			destination.Diagram.Failed(this.CreateContext(isFirstStep, origin, destination, parameters));

			//返回空对象
			return null;
		}

		private StateContextBase CreateContext(bool isFirstStep, State origin, State destination, IDictionary<string, object> parameters)
		{
			var contextType = typeof(StateContext<>).MakeGenericType(destination.GetType());
			return (StateContextBase)Activator.CreateInstance(contextType, new object[] { this, isFirstStep, origin, destination, parameters });
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
			if(System.Threading.Interlocked.CompareExchange(ref _isDisposed, 1, 0) != 0)
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

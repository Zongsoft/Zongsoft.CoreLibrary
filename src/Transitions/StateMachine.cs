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
	public class StateMachine
	{
		#region 成员字段
		private IStateDiagramProvider _diagrams;
		#endregion

		#region 私有变量
		private readonly Stack<object> _stack;
		#endregion

		#region 构造函数
		public StateMachine()
		{
			_stack = new Stack<object>();
		}
		#endregion

		#region 公共属性
		public IStateDiagramProvider Diagrams
		{
			get
			{
				if(_diagrams == null)
				{
					lock(_stack)
					{
						if(_diagrams == null)
							_diagrams = this.CreateDiagrams();
					}
				}

				return _diagrams;
			}
		}
		#endregion

		#region 公共方法
		public StateMachine Run<T>(State<T> destination, IDictionary<string, object> parameters = null) where T : struct
		{
			if(destination == null)
				throw new ArgumentNullException(nameof(destination));

			//获取指定状态实例的当前状态
			var current = this.GetState(destination, out var isTransfered);

			//如果指定状态实例已经被处理过，则不能再次转换
			if(isTransfered)
				return this;

			//获取指定状态实例所属的状态图
			var diagram = this.GetDiagram<T>();

			//设置目标状态实例所属的状态图
			destination.Diagram = diagram;

			//如果流程图未定义当前的流转向量，则退出或抛出异常
			if(!diagram.CanVectoring(current.Value, destination.Value))
			{
				//如果流转栈为空，则表示首次操作即抛出异常
				if(_stack == null || _stack.Count == 0)
					throw new InvalidOperationException("Not supported state transfer.");

				//退出方法
				return this;
			}

			var context = new StateContext<T>(this, destination, parameters);

			//遍历状态图中的转换器，依次进行状态转换处理
			foreach(var transfer in diagram.Transfers)
			{
				//调用状态转换器进行状态转换
				if(transfer.CanTransfer(context) && transfer.Transfer(context))
				{
					//将转换成功的状态压入状态栈
					_stack.Push(destination);

					//遍历状态图中的所有触发器
					foreach(var trigger in diagram.Triggers)
					{
						//如果触发器可用，则激发触发器
						if(trigger.Enabled)
							trigger.OnTrigger(context);
					}
				}
			}

			//始终返回当前状态机
			return this;
		}
		#endregion

		#region 虚拟方法
		protected virtual IStateDiagramProvider CreateDiagrams()
		{
			return StateDiagramProvider.Default;
		}
		#endregion

		#region 私有方法
		private StateDiagramBase<T> GetDiagram<T>() where T : struct
		{
			var diagrams = this.Diagrams;

			if(diagrams == null)
				throw new InvalidOperationException("Missing the state didgram provider.");

			var diagram = diagrams.GetDiagram<T>();

			if(diagram == null)
				throw new InvalidOperationException(string.Format("Not found the state diagram with '{0}' type.", typeof(T).FullName));

			return diagram;
		}

		private State<T> GetState<T>(State<T> state, out bool isTransfered) where T : struct
		{
			isTransfered = false;

			if(_stack != null)
			{
				foreach(var frame in _stack)
				{
					isTransfered = frame.GetType() == typeof(State<T>) && ((State<T>)frame).Match(state);

					if(isTransfered)
						return (State<T>)frame;
				}
			}

			return this.GetDiagram<T>().GetState(state);
		}
		#endregion
	}
}

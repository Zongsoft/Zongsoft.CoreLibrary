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
	public class StateContext<TState> : StateContextBase where TState : State
	{
		#region 构造函数
		public StateContext(StateMachine machine, bool isFirst, TState origin, TState destination, IDictionary<string, object> parameters = null) : base(machine, isFirst, origin, destination, parameters)
		{
		}
		#endregion

		#region 公共属性
		public TState Origin
		{
			get
			{
				return (TState)base.InnerOrigin;
			}
		}

		public TState Destination
		{
			get
			{
				return (TState)base.InnerDestination;
			}
		}
		#endregion

		#region 公共方法
		public TDiagram GetDiagram<TDiagram>() where TDiagram : StateDiagramBase<TState>
		{
			return this.Destination.Diagram as TDiagram;
		}

		public void OnStop(Action<StateContext<TState>, StateStopReason> thunk)
		{
			this.OnStopInner = thunk;
		}
		#endregion
	}
}

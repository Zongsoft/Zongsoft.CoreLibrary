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
	public abstract class StateDiagramBase<TState> : IStateDiagram where TState : State
	{
		#region 成员字段
		private ICollection<IStateTrigger> _triggers;
		private ICollection<IStateTransfer> _transfers;
		#endregion

		#region 构造函数
		protected StateDiagramBase()
		{
		}
		#endregion

		#region 公共属性
		public ICollection<IStateTrigger> Triggers
		{
			get
			{
				if(_triggers == null)
					System.Threading.Interlocked.CompareExchange(ref _triggers, new List<IStateTrigger>(), null);

				return _triggers;
			}
		}

		public ICollection<IStateTransfer> Transfers
		{
			get
			{
				if(_transfers == null)
					System.Threading.Interlocked.CompareExchange(ref _transfers, new List<IStateTransfer>(), null);

				return _transfers;
			}
		}
		#endregion

		#region 显式实现
		bool IStateDiagram.CanVectoring(State origin, State destination)
		{
			return this.CanVectoring(origin as TState, destination as TState);
		}

		State IStateDiagram.GetState(State state)
		{
			return this.GetState(state as TState);
		}

		bool IStateDiagram.SetState(State state, IDictionary<string, object> parameters)
		{
			return this.SetState(state as TState, parameters);
		}
		#endregion

		#region 抽象方法
		protected abstract bool CanVectoring(TState origin, TState destination);

		protected abstract TState GetState(TState state);
		protected abstract bool SetState(TState state, IDictionary<string, object> parameters);
		#endregion
	}
}

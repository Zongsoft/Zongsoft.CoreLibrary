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
		private readonly object _syncRoot;

		private int _initialized;
		private Stack<object> _stack;
		#endregion

		#region 构造函数
		public StateMachine()
		{
			_syncRoot = new object();
		}
		#endregion

		#region 公共属性
		public IStateDiagramProvider Diagrams
		{
			get
			{
				if(_diagrams == null)
				{
					lock(_syncRoot)
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
		public StateMachine Run<TKey, TValue>(State<TKey, TValue> destination, StateMachineOptions options = null) where TKey : struct where TValue : struct
		{
			//确认运行参数设置
			if(options == null)
				options = StateMachineOptions.Default;

			//首先初始化运行环境
			this.Initialize();

			//获取指定状态实例的当前状态
			var initialState = this.GetState<TKey, TValue>(destination.Key);

			//始终返回当前状态机
			return this;
		}
		#endregion

		#region 虚拟方法
		protected virtual IStateDiagramProvider CreateDiagrams()
		{
			return StateDiagramProvider.Default;
		}

		protected virtual State<TKey, TValue> GetState<TKey, TValue>(TKey key) where TKey : struct where TValue : struct
		{
			if(_stack != null)
			{
			}

			return this.GetDiagram<TValue>().GetState(key);
		}
		#endregion

		#region 私有方法
		private bool Initialize()
		{
			//如果已经初始化过，则返回失败
			if(System.Threading.Interlocked.CompareExchange(ref _initialized, 1, 0) != 0)
				return false;

			_stack = new Stack<object>();

			//返回初始化成功
			return true;
		}

		private StateDiagramBase<T> GetDiagram<T>() where T : struct
		{
			var diagrams = this.Diagrams;

			if(diagrams == null)
				throw new InvalidOperationException("Missing the state didgram provider.");

			var diagram = diagrams.GetDiagram<T>();

			if(diagram == null)
				throw new InvalidOperationException(string.Format("Not found the state diagram of '{0}' type.", typeof(T).FullName));

			return diagram;
		}
		#endregion
	}
}

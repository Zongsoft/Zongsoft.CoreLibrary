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
	public abstract class StateContextBase
	{
		#region 成员字段
		private StateMachine _machine;
		private bool _isFirstStep;
		private State _origin;
		private State _destination;
		private IDictionary<string, object> _parameters;
		private Delegate _stoppedThunk;
		private Delegate _stoppingThunk;
		#endregion

		#region 构造函数
		protected StateContextBase(StateMachine machine, bool isFirst, State origin, State destination, IDictionary<string, object> parameters = null)
		{
			if(machine == null)
				throw new ArgumentNullException(nameof(machine));
			if(origin == null)
				throw new ArgumentNullException(nameof(origin));
			if(destination == null)
				throw new ArgumentNullException(nameof(destination));

			_machine = machine;
			_isFirstStep = isFirst;
			_origin = origin;
			_destination = destination;
			_parameters = parameters;
		}
		#endregion

		#region 公共属性
		public StateMachine Machine
		{
			get
			{
				return _machine;
			}
		}

		public bool IsFirstStep
		{
			get
			{
				return _isFirstStep;
			}
		}

		public bool HasParameters
		{
			get
			{
				return _parameters != null && _parameters.Count > 0;
			}
		}

		public IDictionary<string, object> Parameters
		{
			get
			{
				if(_parameters == null)
					System.Threading.Interlocked.CompareExchange(ref _parameters, new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase), null);

				return _parameters;
			}
		}
		#endregion

		#region 内部属性
		internal Delegate StoppedThunk
		{
			get
			{
				return _stoppedThunk;
			}
			set
			{
				_stoppedThunk = value;
			}
		}

		internal Delegate StoppingThunk
		{
			get
			{
				return _stoppingThunk;
			}
			set
			{
				_stoppingThunk = value;
			}
		}

		internal State InnerOrigin
		{
			get
			{
				return _origin;
			}
		}

		internal State InnerDestination
		{
			get
			{
				return _destination;
			}
		}
		#endregion
	}
}

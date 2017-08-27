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
	public abstract class StateDiagramBase<T> where T : struct
	{
		#region 成员字段
		private ISet<StateVector<T>> _vectors;
		private ICollection<IStateTrigger<T>> _triggers;
		private ICollection<IStateTransfer<T>> _transfers;
		#endregion

		#region 构造函数
		protected StateDiagramBase()
		{
		}
		#endregion

		#region 公共属性
		public ISet<StateVector<T>> Vectors
		{
			get
			{
				if(_vectors == null)
					System.Threading.Interlocked.CompareExchange(ref _vectors, new HashSet<StateVector<T>>(), null);

				return _vectors;
			}
		}

		public ICollection<IStateTrigger<T>> Triggers
		{
			get
			{
				if(_triggers == null)
					System.Threading.Interlocked.CompareExchange(ref _triggers, new List<IStateTrigger<T>>(), null);

				return _triggers;
			}
		}

		public ICollection<IStateTransfer<T>> Transfers
		{
			get
			{
				if(_transfers == null)
					System.Threading.Interlocked.CompareExchange(ref _transfers, new List<IStateTransfer<T>>(), null);

				return _transfers;
			}
		}
		#endregion

		#region 公共方法
		public StateVector<T>? GetVector(T origin, T destination)
		{
			var vectors = _vectors;

			if(vectors == null || vectors.Count == 0)
				return null;

			foreach(var vector in vectors)
			{
				if(vector.Origin.Equals(origin) && vector.Destination.Equals(destination))
					return vector;
			}

			return null;
		}

		public bool CanVectoring(T origin, T destination)
		{
			var vectors = _vectors;

			if(vectors == null || vectors.Count == 0)
				return false;

			foreach(var vector in vectors)
			{
				if(vector.Origin.Equals(origin) && vector.Destination.Equals(destination))
					return true;
			}

			return false;
		}
		#endregion

		#region 保护方法
		protected int Map(T origin, params T[] destinations)
		{
			if(destinations == null || destinations.Length == 0)
				return 0;

			var count = 0;

			foreach(var destination in destinations)
			{
				var vector = new StateVector<T>(origin, destination);

				if(this.Vectors.Add(vector))
					count += 1;
			}

			return count;
		}
		#endregion

		#region 抽象方法
		internal protected abstract State<T> GetState(State<T> state);
		internal protected abstract bool SetState(State<T> state, IDictionary<string, object> parameters = null);
		#endregion
	}
}

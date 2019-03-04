﻿/*
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

namespace Zongsoft.Transitions
{
	public struct StateVector<T> : IEquatable<StateVector<T>> where T : struct
	{
		#region 构造函数
		public StateVector(T origin, T destination)
		{
			this.Origin = origin;
			this.Destination = destination;
		}
		#endregion

		#region 公共字段
		public readonly T Origin;
		public readonly T Destination;
		#endregion

		#region 重写方法
		public bool Equals(StateVector<T> other)
		{
			return this.Origin.Equals(other.Origin) &&
			       this.Destination.Equals(other.Destination);
		}

		public override bool Equals(object obj)
		{
			if(obj == null || obj.GetType() != this.GetType())
				return false;

			return this.Equals((StateVector<T>)obj);
		}

		public override int GetHashCode()
		{
			return this.Origin.GetHashCode() ^ this.Destination.GetHashCode();
		}

		public override string ToString()
		{
			return this.Origin.ToString() + "->" + this.Destination.ToString();
		}
		#endregion
	}
}

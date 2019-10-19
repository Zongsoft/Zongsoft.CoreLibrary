/*
 *   _____                                ______
 *  /_   /  ____  ____  ____  _________  / __/ /_
 *    / /  / __ \/ __ \/ __ \/ ___/ __ \/ /_/ __/
 *   / /__/ /_/ / / / / /_/ /\_ \/ /_/ / __/ /_
 *  /____/\____/_/ /_/\__  /____/\____/_/  \__/
 *                   /____/
 *
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@qq.com>
 *
 * Copyright (C) 2010-2019 Zongsoft Corporation <http://www.zongsoft.com>
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

namespace Zongsoft.Data
{
	/// <summary>
	/// 表示数据写入的递增(减)的数据量。
	/// </summary>
	public struct Interval : IEquatable<Interval>
	{
		#region 公共字段
		/// <summary>递增(减)的步长值。</summary>
		public readonly int Value;

		/// <summary>递增(减)的初始值。</summary>
		public readonly int Seed;
		#endregion

		#region 构造函数
		public Interval(int value = 1, int seed = 0)
		{
			this.Value = value;
			this.Seed = seed;
		}
		#endregion

		#region 重写方法
		public bool Equals(Interval other)
		{
			return this.Value == other.Value &&
			       this.Seed == other.Seed;
		}

		public override bool Equals(object obj)
		{
			if(obj == null || obj.GetType() != typeof(Interval))
				return false;

			return this.Equals((Interval)obj);
		}

		public override int GetHashCode()
		{
			return EqualityComparer<Interval>.Default.GetHashCode(this);
		}

		public override string ToString()
		{
			return this.Value.ToString() + "/" + this.Seed.ToString();
		}
		#endregion
	}
}

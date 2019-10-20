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

namespace Zongsoft.Data
{
	/// <summary>
	/// 表示数据写入的递增(减)的步长值。
	/// </summary>
	public struct Interval : IConvertible, IComparable<Interval>, IEquatable<Interval>, IFormattable
	{
		#region 静态字段
		/// <summary>表示递增一的步长。</summary>
		public static readonly Interval Increment = new Interval(1);

		/// <summary>表示递减一的步长。</summary>
		public static readonly Interval Decrement = new Interval(-1);
		#endregion

		#region 公共字段
		/// <summary>递增(减)的步长值。</summary>
		public readonly int Value;
		#endregion

		#region 构造函数
		public Interval(int value)
		{
			this.Value = value;
		}
		#endregion

		#region 重写方法
		public int CompareTo(Interval other)
		{
			return this.Value.CompareTo(other.Value);
		}

		public bool Equals(Interval other)
		{
			return this.Value == other.Value;
		}

		public override bool Equals(object obj)
		{
			if(obj == null || obj.GetType() != typeof(Interval))
				return false;

			return this.Equals((Interval)obj);
		}

		public override int GetHashCode()
		{
			return this.Value.GetHashCode();
		}

		public override string ToString()
		{
			return Value.ToString();
		}

		public string ToString(string format, IFormatProvider provider)
		{
			return Value.ToString(format, provider);
		}
		#endregion

		#region 转换方法
		public TypeCode GetTypeCode()
		{
			return this.Value.GetTypeCode();
		}

		object IConvertible.ToType(Type conversionType, IFormatProvider provider)
		{
			return ((IConvertible)Value).ToType(conversionType, provider);
		}

		bool IConvertible.ToBoolean(IFormatProvider provider)
		{
			return ((IConvertible)Value).ToBoolean(provider);
		}

		byte IConvertible.ToByte(IFormatProvider provider)
		{
			return ((IConvertible)Value).ToByte(provider);
		}

		char IConvertible.ToChar(IFormatProvider provider)
		{
			return ((IConvertible)Value).ToChar(provider);
		}

		DateTime IConvertible.ToDateTime(IFormatProvider provider)
		{
			return ((IConvertible)Value).ToDateTime(provider);
		}

		decimal IConvertible.ToDecimal(IFormatProvider provider)
		{
			return ((IConvertible)Value).ToDecimal(provider);
		}

		double IConvertible.ToDouble(IFormatProvider provider)
		{
			return ((IConvertible)Value).ToDouble(provider);
		}

		short IConvertible.ToInt16(IFormatProvider provider)
		{
			return ((IConvertible)Value).ToInt16(provider);
		}

		int IConvertible.ToInt32(IFormatProvider provider)
		{
			return ((IConvertible)Value).ToInt32(provider);
		}

		long IConvertible.ToInt64(IFormatProvider provider)
		{
			return ((IConvertible)Value).ToInt64(provider);
		}

		sbyte IConvertible.ToSByte(IFormatProvider provider)
		{
			return ((IConvertible)Value).ToSByte(provider);
		}

		float IConvertible.ToSingle(IFormatProvider provider)
		{
			return ((IConvertible)Value).ToSingle(provider);
		}

		string IConvertible.ToString(IFormatProvider provider)
		{
			return Value.ToString(provider);
		}

		ushort IConvertible.ToUInt16(IFormatProvider provider)
		{
			return ((IConvertible)Value).ToUInt16(provider);
		}

		uint IConvertible.ToUInt32(IFormatProvider provider)
		{
			return ((IConvertible)Value).ToUInt32(provider);
		}

		ulong IConvertible.ToUInt64(IFormatProvider provider)
		{
			return ((IConvertible)Value).ToUInt64(provider);
		}
		#endregion

		#region 运算符号
		public static bool operator ==(Interval a, Interval b)
		{
			return a.Value == b.Value;
		}

		public static bool operator !=(Interval a, Interval b)
		{
			return a.Value != b.Value;
		}

		public static bool operator <(Interval a, Interval b)
		{
			return a.Value < b.Value;
		}

		public static bool operator <=(Interval a, Interval b)
		{
			return a.Value <= b.Value;
		}

		public static bool operator >(Interval a, Interval b)
		{
			return a.Value > b.Value;
		}

		public static bool operator >=(Interval a, Interval b)
		{
			return a.Value >= b.Value;
		}

		public static implicit operator int(Interval interval)
		{
			return interval.Value;
		}

		public static implicit operator Interval(int value)
		{
			return new Interval(value);
		}
		#endregion
	}
}

/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
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

namespace Zongsoft.Data
{
	public class ConditionalValue<T> : IConvertible
	{
		#region 成员字段
		private T _value;
		#endregion

		#region 构造函数
		public ConditionalValue(T value)
		{
			_value = value;
		}
		#endregion

		#region 公共属性
		public T Value
		{
			get
			{
				return _value;
			}
			set
			{
				_value = value;
			}
		}
		#endregion

		#region 重写方法
		public override bool Equals(object obj)
		{
			if(obj == null)
				return _value == null;

			if(_value == null)
				return false;

			return object.Equals(_value, Common.Convert.ConvertValue<T>(obj));
		}

		public override int GetHashCode()
		{
			if(_value == null)
				return 0;

			return _value.GetHashCode();
		}

		public override string ToString()
		{
			if(_value == null)
				return string.Empty;

			return _value.ToString();
		}
		#endregion

		#region 符号重写
		public static implicit operator ConditionalValue<T>(T value)
		{
			return new ConditionalValue<T>(value);
		}

		public static explicit operator T (ConditionalValue<T> value)
		{
			return value._value;
		}
		#endregion

		#region 转换接口
		TypeCode IConvertible.GetTypeCode()
		{
			return Type.GetTypeCode(typeof(T));
		}

		bool IConvertible.ToBoolean(IFormatProvider provider)
		{
			return this.Convert<bool>(convertible => convertible.ToBoolean(provider),
			                          text =>
									  {
										  bool result;
										  return new Tuple<bool, bool>(bool.TryParse(text, out result), result);
									  });
		}

		byte IConvertible.ToByte(IFormatProvider provider)
		{
			return this.Convert<byte>(convertible => convertible.ToByte(provider),
									  text =>
									  {
										  byte result;
										  return new Tuple<bool, byte>(byte.TryParse(text, out result), result);
									  });
		}

		char IConvertible.ToChar(IFormatProvider provider)
		{
			return this.Convert<char>(convertible => convertible.ToChar(provider),
									  text =>
									  {
										  char result;
										  return new Tuple<bool, char>(char.TryParse(text, out result), result);
									  });
		}

		DateTime IConvertible.ToDateTime(IFormatProvider provider)
		{
			return this.Convert<DateTime>(convertible => convertible.ToDateTime(provider),
									  text =>
									  {
										  DateTime result;
										  return new Tuple<bool, DateTime>(DateTime.TryParse(text, out result), result);
									  });
		}

		decimal IConvertible.ToDecimal(IFormatProvider provider)
		{
			return this.Convert<decimal>(convertible => convertible.ToDecimal(provider),
									  text =>
									  {
										  decimal result;
										  return new Tuple<bool, decimal>(decimal.TryParse(text, out result), result);
									  });
		}

		double IConvertible.ToDouble(IFormatProvider provider)
		{
			return this.Convert<double>(convertible => convertible.ToDouble(provider),
									  text =>
									  {
										  double result;
										  return new Tuple<bool, double>(double.TryParse(text, out result), result);
									  });
		}

		short IConvertible.ToInt16(IFormatProvider provider)
		{
			return this.Convert<short>(convertible => convertible.ToInt16(provider),
									  text =>
									  {
										  short result;
										  return new Tuple<bool, short>(short.TryParse(text, out result), result);
									  });
		}

		int IConvertible.ToInt32(IFormatProvider provider)
		{
			return this.Convert<int>(convertible => convertible.ToInt32(provider),
									  text =>
									  {
										  int result;
										  return new Tuple<bool, int>(int.TryParse(text, out result), result);
									  });
		}

		long IConvertible.ToInt64(IFormatProvider provider)
		{
			return this.Convert<long>(convertible => convertible.ToInt64(provider),
									  text =>
									  {
										  long result;
										  return new Tuple<bool, long>(long.TryParse(text, out result), result);
									  });
		}

		sbyte IConvertible.ToSByte(IFormatProvider provider)
		{
			return this.Convert<sbyte>(convertible => convertible.ToSByte(provider),
									  text =>
									  {
										  sbyte result;
										  return new Tuple<bool, sbyte>(sbyte.TryParse(text, out result), result);
									  });
		}

		float IConvertible.ToSingle(IFormatProvider provider)
		{
			return this.Convert<float>(convertible => convertible.ToSingle(provider),
									  text =>
									  {
										  float result;
										  return new Tuple<bool, float>(float.TryParse(text, out result), result);
									  });
		}

		string IConvertible.ToString(IFormatProvider provider)
		{
			if(_value != null)
				return _value.ToString();

			return null;
		}

		object IConvertible.ToType(Type conversionType, IFormatProvider provider)
		{
			return this.Convert<object>(convertible => convertible.ToType(conversionType, provider),
									  text =>
									  {
										  return new Tuple<bool, object>(false, text);
									  });
		}

		ushort IConvertible.ToUInt16(IFormatProvider provider)
		{
			return this.Convert<ushort>(convertible => convertible.ToByte(provider),
									  text =>
									  {
										  ushort result;
										  return new Tuple<bool, ushort>(ushort.TryParse(text, out result), result);
									  });
		}

		uint IConvertible.ToUInt32(IFormatProvider provider)
		{
			return this.Convert<uint>(convertible => convertible.ToByte(provider),
									  text =>
									  {
										  uint result;
										  return new Tuple<bool, uint>(uint.TryParse(text, out result), result);
									  });
		}

		ulong IConvertible.ToUInt64(IFormatProvider provider)
		{
			return this.Convert<ulong>(convertible => convertible.ToByte(provider),
									  text =>
									  {
										  ulong result;
										  return new Tuple<bool, ulong>(ulong.TryParse(text, out result), result);
									  });
		}

		private TResult Convert<TResult>(Func<IConvertible, TResult> convert, Func<string, Tuple<bool, TResult>> parser)
		{
			if(_value == null)
				return default(TResult);

			var convertible = _value as IConvertible;

			if(convertible != null)
				return convert(convertible);

			if(typeof(T) == typeof(string))
			{
				var result = parser(_value.ToString());

				if(result.Item1)
					return result.Item2;
			}

			return default(TResult);
		}
		#endregion
	}
}

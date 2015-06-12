/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2010-2015 Zongsoft Corporation <http://www.zongsoft.com>
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
using System.Reflection;
using System.Globalization;
using System.ComponentModel;
using System.Collections.Generic;

namespace Zongsoft.ComponentModel
{
	public class EnumConverter : System.ComponentModel.EnumConverter
	{
		#region 成员变量
		private Zongsoft.Common.EnumEntry[] _entries;
		#endregion

		#region 构造函数
		public EnumConverter(Type enumType) : base(enumType)
		{
		}
		#endregion

		#region 重写方法
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			if(sourceType == typeof(string) || sourceType == typeof(DBNull) ||
			   sourceType == typeof(byte) || sourceType == typeof(sbyte) ||
			   sourceType == typeof(short) || sourceType == typeof(ushort) ||
			   sourceType == typeof(int) || sourceType == typeof(uint) ||
			   sourceType == typeof(long) || sourceType == typeof(ulong) ||
			   sourceType == typeof(decimal) || sourceType == typeof(double) || sourceType == typeof(float))
				return true;

			return base.CanConvertFrom(context, sourceType);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			if(value == null || System.Convert.IsDBNull(value))
				return Zongsoft.Common.Convert.GetDefaultValue(this.EnumType);

			if(value is string)
			{
				string valueString = (string)value;

				if(valueString.IndexOf(',') > 1)
				{
					long convertedValue = 0;
					string[] parts = valueString.Split(',');

					foreach(string part in parts)
					{
						if(!string.IsNullOrWhiteSpace(part))
							convertedValue |= GetEnumValue(part, true);
					}

					return Enum.ToObject(this.EnumType, convertedValue);
				}
				else
				{
					return Enum.ToObject(this.EnumType, GetEnumValue(valueString, true));
				}
			}
			else if(value.GetType().IsPrimitive || value.GetType() == typeof(decimal))
			{
				switch(Type.GetTypeCode(value.GetType()))
				{
					case TypeCode.Double:
					case TypeCode.Single:
					case TypeCode.Decimal:
						value = System.Convert.ToInt64(value);
						break;
				}

				return Enum.ToObject(this.EnumType, value);
			}
			else if(value is Enum[])
			{
				long enumValue = 0L;

				foreach(Enum item in (Enum[])value)
					enumValue |= Convert.ToInt64(item, culture);

				return Enum.ToObject(this.EnumType, enumValue);
			}

			return base.ConvertFrom(context, culture, value);
		}

		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
		{
			if(this.Values != null)
				return this.Values;

			int baseIndex = 0;
			Type enumType = this.EnumType;
			Type underlyingTypeOfNullable = Nullable.GetUnderlyingType(typeof(Enum));

			if(underlyingTypeOfNullable != null)
			{
				enumType = underlyingTypeOfNullable;
				baseIndex = 1;
			}

			Array array = Enum.GetValues(enumType);
			Enum[] values = new Enum[array.Length + baseIndex];
			array.CopyTo(values, baseIndex);

			this.Values = new StandardValuesCollection(values);
			return this.Values;
		}
		#endregion

		#region 私有方法
		private long GetEnumValue(string valueText, bool throwExceptions)
		{
			long result;

			if(!TryGetEnumValue(valueText, out result))
			{
				if(throwExceptions)
					throw new FormatException(string.Format("Can not from this '{0}' string convert to '{1}' enum.", valueText, this.EnumType.AssemblyQualifiedName));

				return 0;
			}

			return result;
		}

		private bool TryGetEnumValue(string valueText, out long underlyingValue)
		{
			underlyingValue = 0;

			if(string.IsNullOrWhiteSpace(valueText))
				return false;

			valueText = valueText.Trim();

			if(long.TryParse(valueText, out underlyingValue))
				return true;

			if(_entries == null)
				_entries = Common.EnumUtility.GetEnumEntries(this.EnumType, true);

			foreach(var entry in _entries)
			{
				if(entry.Value == null)
					continue;

				if(string.Equals(valueText, entry.Name, StringComparison.OrdinalIgnoreCase))
				{
					underlyingValue = Convert.ToInt64(entry.Value);
					return true;
				}

				if(string.Equals(valueText, entry.Value.ToString(), StringComparison.OrdinalIgnoreCase))
				{
					underlyingValue = Convert.ToInt64(entry.Value);
					return true;
				}

				if(string.Equals(valueText, entry.Alias.ToString(), StringComparison.OrdinalIgnoreCase))
				{
					underlyingValue = Convert.ToInt64(entry.Value);
					return true;
				}

				if(string.Equals(valueText, entry.Description, StringComparison.OrdinalIgnoreCase))
				{
					underlyingValue = Convert.ToInt64(entry.Value);
					return true;
				}
			}

			return false;
		}
		#endregion
	}
}

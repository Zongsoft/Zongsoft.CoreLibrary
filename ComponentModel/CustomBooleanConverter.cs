/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2010-2012 Zongsoft Corporation <http://www.zongsoft.com>
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
using System.ComponentModel;
using System.Collections.Generic;

namespace Zongsoft.ComponentModel
{
	public class CustomBooleanConverter : BooleanConverter
	{
		#region 成员变量
		private string _trueString;
		private string _falseString;
		#endregion

		#region 构造函数
		public CustomBooleanConverter() : this("是", "否")
		{
		}

		public CustomBooleanConverter([LocalizableAttribute(true)]string trueString, [LocalizableAttribute(true)]string falseString)
		{
			if(string.IsNullOrEmpty(trueString))
				_trueString = bool.TrueString;
			else
				_trueString = trueString;

			if(string.IsNullOrEmpty(falseString))
				_falseString = bool.FalseString;
			else
				_falseString = falseString;
		}
		#endregion

		#region 公共属性
		[LocalizableAttribute(true)]
		public string TrueString
		{
			get
			{
				return _trueString;
			}
			set
			{
				if(string.IsNullOrEmpty(value))
					value = bool.TrueString;

				if(string.Equals(_trueString, value, StringComparison.Ordinal))
					return;

				_trueString = value;
			}
		}

		[LocalizableAttribute(true)]
		public string FalseString
		{
			get
			{
				return _falseString;
			}
			set
			{
				if(string.IsNullOrEmpty(value))
					value = bool.FalseString;

				if(string.Equals(_falseString, value, StringComparison.Ordinal))
					return;

				_falseString = value;
			}
		}
		#endregion

		#region 重写方法
		public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
		{
			if(value is string)
			{
				bool result;

				if(bool.TryParse((string)value, out result))
					return result;

				if(string.Equals(_trueString, (string)value, StringComparison.OrdinalIgnoreCase))
					return true;
				if(string.Equals(_falseString, (string)value, StringComparison.OrdinalIgnoreCase))
					return false;

				if(string.IsNullOrEmpty((string)value) && this.IsNullable(context.PropertyDescriptor.PropertyType))
					return null;
				else
					throw new FormatException();
			}

			return base.ConvertFrom(context, culture, value);
		}

		public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
		{
			if(destinationType == typeof(string))
			{
				if(value == null)
					return null;

				return (bool)value ? _trueString : _falseString;
			}

			return base.ConvertTo(context, culture, value, destinationType);
		}

		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
		{
			if(this.IsNullable(context.PropertyDescriptor.PropertyType))
				return new TypeConverter.StandardValuesCollection(new object[] { null, true, false });
			else
				return new TypeConverter.StandardValuesCollection(new object[] { true, false });
		}
		#endregion

		#region 私有方法
		private bool IsNullable(Type propertyType)
		{
			if(propertyType == null)
				return false;

			return propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Nullable<>);
		}
		#endregion
	}
}

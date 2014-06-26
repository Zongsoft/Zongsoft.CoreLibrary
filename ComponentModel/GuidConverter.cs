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
using System.ComponentModel.Design.Serialization;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace Zongsoft.ComponentModel
{
	public class GuidConverter : System.ComponentModel.GuidConverter
	{
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			if(sourceType == typeof(byte[]))
				return true;

			return base.CanConvertFrom(context, sourceType);
		}

		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			if(destinationType == typeof(InstanceDescriptor) || destinationType == typeof(byte[]))
				return true;

			return base.CanConvertTo(context, destinationType);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			if(value is byte[])
			{
				byte[] array = value as byte[];

				if(array.Length == 16)
					return new Guid(array);

				if(array.Length > 16)
				{
					return new Guid(BitConverter.ToUInt32(array, 0), BitConverter.ToUInt16(array, 4), BitConverter.ToUInt16(array, 6),
						array[8], array[9], array[10], array[11], array[12], array[13], array[14], array[15]);
				}
			}

			return base.ConvertFrom(context, culture, value);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if(destinationType == null)
				throw new ArgumentNullException("destinationType");

			if(destinationType == typeof(InstanceDescriptor) && value is Guid)
			{
				ConstructorInfo ctor = typeof(Guid).GetConstructor(new Type[] { typeof(string) });

				if(ctor != null)
					return new InstanceDescriptor(ctor, new object[] { value.ToString() });
			}
			else if(destinationType == typeof(byte[]) && value is Guid)
				return ((Guid)value).ToByteArray();

			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
}

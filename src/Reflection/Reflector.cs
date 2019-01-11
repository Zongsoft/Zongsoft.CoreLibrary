/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@qq.com>
 *
 * Copyright (C) 2010-2018 Zongsoft Corporation <http://www.zongsoft.com>
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

namespace Zongsoft.Reflection
{
	public static class Reflector
	{
		public static object GetValue(this MemberInfo member, object target =null)
		{
			if(member == null)
				throw new ArgumentNullException(nameof(member));

			switch(member.MemberType)
			{
				case MemberTypes.Field:
					return GetValue((FieldInfo)member, target);
				case MemberTypes.Property:
					return GetValue((PropertyInfo)member, target);
				default:
					throw new NotSupportedException("Invalid member type.");
			}
		}

		public static object GetValue(this FieldInfo field, object target = null)
		{
			return field.GetValue(target);
		}

		public static object GetValue(this PropertyInfo property, object target)
		{
			return property.GetValue(target);
		}

		public static void SetValue(this MemberInfo member, object target, object value)
		{
			if(member == null)
				throw new ArgumentNullException(nameof(member));

			switch(member.MemberType)
			{
				case MemberTypes.Field:
					SetValue((FieldInfo)member, target, value);
					break;
				case MemberTypes.Property:
					SetValue((PropertyInfo)member, target, value);
					break;
				default:
					throw new NotSupportedException("Invalid member type.");
			}
		}

		public static void SetValue(this FieldInfo field, object target, object value)
		{
			field.SetValue(target, value);
		}

		public static void SetValue(this PropertyInfo property, object target, object value)
		{
			property.SetValue(target, value);
		}
	}
}

/*
 * Authors:
 *   钟峰(Popeye Zhong) <9555843@qq.com>
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
	/// <summary>
	/// 表示类型成员信息的类。
	/// </summary>
	public class MemberToken
	{
		#region 成员字段
		private string _name;
		private Type _type;
		private MemberKind _kind;
		private Func<object, object> _getter;
		#endregion

		#region 构造函数
		public MemberToken(PropertyInfo property)
		{
			if(property == null)
				throw new ArgumentNullException(nameof(property));

			_name = property.Name;
			_type = property.PropertyType;
			_kind = MemberKind.Property;

			if(property.CanRead)
				_getter = PropertyInfoExtension.GenerateGetter(property);
		}

		public MemberToken(FieldInfo field)
		{
			if(field == null)
				throw new ArgumentNullException(nameof(field));

			_name = field.Name;
			_type = field.FieldType;
			_kind = MemberKind.Field;

			_getter = FieldInfoExtension.GenerateGetter(field);
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取成员的名称。
		/// </summary>
		public string Name
		{
			get
			{
				return _name;
			}
		}

		/// <summary>
		/// 获取成员的类型。
		/// </summary>
		public Type Type
		{
			get
			{
				return _type;
			}
		}

		/// <summary>
		/// 获取成员的种类，指示成员是否为属性或字段。
		/// </summary>
		public MemberKind Kind
		{
			get
			{
				return _kind;
			}
		}
		#endregion

		#region 公共方法
		public object GetValue(object target)
		{
			if(_getter == null)
				throw new NotSupportedException();

			return _getter.Invoke(target);
		}
		#endregion
	}
}

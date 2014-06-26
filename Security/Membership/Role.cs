/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2003-2014 Zongsoft Corporation <http://www.zongsoft.com>
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

namespace Zongsoft.Security.Membership
{
	/// <summary>
	/// 表示角色的实体类。
	/// </summary>
	[Serializable]
	public class Role
	{
		#region 静态字段
		public static readonly string Administrators = "Administrators";
		public static readonly string Security = "Security";
		#endregion

		#region 成员字段
		private string _applicationId;
		private string _name;
		private string _fullName;
		private string _description;
		private DateTime _createdTime;
		#endregion

		#region 构造函数
		public Role(string applicationId, string name) : this(applicationId, name, string.Empty, string.Empty)
		{
		}

		public Role(string applicationId, string name, string fullName, string description)
		{
			if(string.IsNullOrWhiteSpace(applicationId))
				throw new ArgumentNullException("applicationId");

			if(string.IsNullOrWhiteSpace(name))
				throw new ArgumentNullException("name");

			_applicationId = applicationId.Trim();
			_name = name.Trim();
			_fullName = fullName;
			_description = description;
			_createdTime = DateTime.Now;
		}
		#endregion

		#region 公共属性
		public string ApplicationId
		{
			get
			{
				return _applicationId;
			}
		}

		public string Name
		{
			get
			{
				return _name;
			}
			set
			{
				if(string.IsNullOrWhiteSpace(value))
					throw new ArgumentNullException();

				_name = value.Trim();
			}
		}

		public string FullName
		{
			get
			{
				return _fullName;
			}
			set
			{
				_fullName = value;
			}
		}

		public string Description
		{
			get
			{
				return _description;
			}
			set
			{
				_description = value;
			}
		}

		public DateTime CreatedTime
		{
			get
			{
				return _createdTime;
			}
			set
			{
				_createdTime = value;
			}
		}
		#endregion

		#region 重写方法
		public override bool Equals(object obj)
		{
			if(obj == null || obj.GetType() != this.GetType())
				return false;

			var role = (Role)obj;

			return string.Equals(_applicationId, role._applicationId, StringComparison.OrdinalIgnoreCase) &&
				   string.Equals(_name, role._name, StringComparison.OrdinalIgnoreCase);
		}

		public override int GetHashCode()
		{
			return (_applicationId + ":" + _name).ToLowerInvariant().GetHashCode();
		}

		public override string ToString()
		{
			return string.Format("{0}[{1}]", _name, _applicationId);
		}
		#endregion

		#region 静态方法
		public static bool IsFixedRole(Role role)
		{
			if(role == null)
				return false;

			return IsFixedRole(role.Name);
		}

		public static bool IsFixedRole(string roleName)
		{
			return string.Equals(roleName, Role.Administrators, StringComparison.OrdinalIgnoreCase) ||
			       string.Equals(roleName, Role.Security, StringComparison.OrdinalIgnoreCase);
		}
		#endregion
	}
}

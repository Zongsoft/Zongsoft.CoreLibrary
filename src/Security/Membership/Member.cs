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
	/// 表示角色成员的实体类。
	/// </summary>
	[Serializable]
	public class Member
	{
		#region 成员字段
		private string _applicationId;
		private string _roleName;
		private string _memberName;
		private MemberType _memberType;
		#endregion

		#region 构造函数
		public Member(string applicationId, string roleName, string memberName, MemberType memberType)
		{
			if(string.IsNullOrWhiteSpace(applicationId))
				throw new ArgumentNullException("applicationId");

			if(string.IsNullOrWhiteSpace(roleName))
				throw new ArgumentNullException("roleName");

			if(string.IsNullOrWhiteSpace(memberName))
				throw new ArgumentNullException("memberName");

			_applicationId = applicationId.Trim();
			_roleName = roleName.Trim();
			_memberName = memberName.Trim();
			_memberType = memberType;
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

		public string RoleName
		{
			get
			{
				return _roleName;
			}
			set
			{
				if(string.IsNullOrWhiteSpace(value))
					throw new ArgumentNullException();

				_roleName = value.Trim();
			}
		}

		public string MemberName
		{
			get
			{
				return _memberName;
			}
			set
			{
				if(string.IsNullOrWhiteSpace(value))
					throw new ArgumentNullException();

				_memberName = value.Trim();
			}
		}

		public MemberType MemberType
		{
			get
			{
				return _memberType;
			}
			set
			{
				_memberType = value;
			}
		}
		#endregion

		#region 重写方法
		public override bool Equals(object obj)
		{
			if(obj == null || obj.GetType() != this.GetType())
				return false;

			var member = (Member)obj;

			return string.Equals(_applicationId, member._applicationId, StringComparison.OrdinalIgnoreCase) &&
				   string.Equals(_roleName, member._roleName, StringComparison.OrdinalIgnoreCase) &&
			       string.Equals(_memberName, member._memberName, StringComparison.OrdinalIgnoreCase) &&
				   _memberType == member._memberType;
		}

		public override int GetHashCode()
		{
			return (_applicationId + ":" + _roleName + ":" + _memberName + "[" + _memberType.ToString() + "]").ToLowerInvariant().GetHashCode();
		}

		public override string ToString()
		{
			return string.Format("{0}:{1}({2})[{3}]", _roleName, _memberName, _memberType, _applicationId);
		}
		#endregion
	}
}

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
		private int _roleId;
		private int _memberId;
		private MemberType _memberType;
		private Role _role;
		#endregion

		#region 构造函数
		public Member(int roleId, int memberId, MemberType memberType)
		{
			_roleId = roleId;
			_memberId = memberId;
			_memberType = memberType;
		}
		#endregion

		#region 公共属性
		public int RoleId
		{
			get
			{
				return _roleId;
			}
			set
			{
				_roleId = value;
			}
		}

		public Role Role
		{
			get
			{
				return _role;
			}
			set
			{
				_role = value;
			}
		}

		public int MemberId
		{
			get
			{
				return _memberId;
			}
			set
			{
				_memberId = value;
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

			var other = (Member)obj;

			return _roleId == other._roleId && _memberId == other._memberId && _memberType == other._memberType;
		}

		public override int GetHashCode()
		{
			return (_roleId + ":" + _memberId + "[" + _memberType.ToString() + "]").ToLowerInvariant().GetHashCode();
		}

		public override string ToString()
		{
			return string.Format("{0}:{1}({2})", _roleId, _memberId, _memberType);
		}
		#endregion
	}
}

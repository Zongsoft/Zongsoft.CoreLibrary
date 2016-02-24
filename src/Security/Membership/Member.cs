/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2003-2016 Zongsoft Corporation <http://www.zongsoft.com>
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
		private Role _role;
		private int _memberId;
		private MemberType _memberType;
		private object _memberObject;
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
		/// <summary>
		/// 获取或设置成员的父角色编号。
		/// </summary>
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

		/// <summary>
		/// 获取或设置成员的父角色对象。
		/// </summary>
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

		/// <summary>
		/// 获取或设置成员编号。
		/// </summary>
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

		/// <summary>
		/// 获取或设置成员类型。
		/// </summary>
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

		/// <summary>
		/// 获取或设置成员自身对象，具体类型由<see cref="MemberType"/>属性标示。
		/// </summary>
		/// <exception cref="ArgumentException">当设置的值不为空，并且不是<seealso cref="User"/>用户或<seealso cref="Role"/>角色类型。</exception>
		public object MemberObject
		{
			get
			{
				return _memberObject;
			}
			set
			{
				if(value == null || value is User || value is Role)
					_memberObject = value;
				else
					throw new ArgumentException("The type of value must be 'User' or 'Role'.");
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

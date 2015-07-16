/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2003-2015 Zongsoft Corporation <http://www.zongsoft.com>
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
using System.Linq;
using System.Text;

using Zongsoft.Data;
using Zongsoft.Options;

namespace Zongsoft.Security.Membership
{
	public class RoleProvider : ProviderBase, IRoleProvider, IMemberProvider
	{
		#region 构造函数
		public RoleProvider(ISettingsProvider settings) : base(settings)
		{
		}
		#endregion

		#region 角色管理
		public Role GetRole(int roleId)
		{
			var dataAccess = this.EnsureDataAccess();
			return dataAccess.Select<Role>(MembershipHelper.DATA_ENTITY_ROLE, new Condition("RoleId", roleId)).FirstOrDefault();
		}

		public IEnumerable<Role> GetAllRoles()
		{
			var dataAccess = this.EnsureDataAccess();

			return dataAccess.Select<Role>(MembershipHelper.DATA_ENTITY_ROLE, new ConditionCollection(ConditionCombine.And)
			{
				new Condition("Namespace", this.Namespace),
			});
		}

		public IEnumerable<Role> GetRoles(int memberId, MemberType memberType)
		{
			var dataAccess = this.EnsureDataAccess();

			var roles = dataAccess.Execute(MembershipHelper.DATA_ENTITY_ROLE, new Dictionary<string, object>
			{
				{"Namespace", this.Namespace},
				{"MemberId", memberId},
				{"MemberType", memberType},
			}) as IEnumerable<Role>;

			return roles;
		}

		public IEnumerable<Role> GetRoles(int memberId, MemberType memberType, int depth)
		{
			throw new NotImplementedException();
		}

		public int DeleteRoles(params int[] roleIds)
		{
			if(roleIds == null || roleIds.Length < 1)
				return 0;

			var dataAccess = this.EnsureDataAccess();
			return dataAccess.Delete(MembershipHelper.DATA_ENTITY_ROLE, new Condition("RoleId", roleIds, ConditionOperator.In));
		}

		public void CreateRoles(IEnumerable<Role> roles)
		{
			if(roles == null)
				return;

			var dataAccess = this.EnsureDataAccess();
			dataAccess.Insert(MembershipHelper.DATA_ENTITY_ROLE, roles);
		}

		public void UpdateRoles(IEnumerable<Role> roles)
		{
			if(roles == null)
				return;

			var dataAccess = this.EnsureDataAccess();
			dataAccess.Update(MembershipHelper.DATA_ENTITY_ROLE, roles);
		}
		#endregion

		#region 成员管理
		public bool InRole(int userId, int roleId)
		{
			return this.GetRoles(userId, MemberType.User, -1).Any(role => role.RoleId == roleId);
		}

		public IEnumerable<Member> GetMembers(int roleId)
		{
			var dataAccess = this.EnsureDataAccess();

			var members = dataAccess.Execute("Security.GetMembers", new Dictionary<string, object>
			{
				{"RoleId", roleId},
			}) as IEnumerable<Member>;

			return members;
		}

		public IEnumerable<Member> GetMembers(int roleId, int depth)
		{
			throw new NotImplementedException();
		}

		public void DeleteMember(int roleId, int memberId, MemberType memberType)
		{
			var dataAccess = this.EnsureDataAccess();

			dataAccess.Delete(MembershipHelper.DATA_ENTITY_MEMBER, new ConditionCollection(ConditionCombine.And)
			{
				new Condition("RoleId", roleId),
				new Condition("MemberId", memberId),
				new Condition("MemberType", memberType),
			});
		}

		public void CreateMember(int roleId, int memberId, MemberType memberType)
		{
			var dataAccess = this.EnsureDataAccess();

			dataAccess.Insert(MembershipHelper.DATA_ENTITY_MEMBER, new Member(roleId, memberId, memberType));
		}

		public void SetMembers(IEnumerable<Member> members)
		{
			if(members == null)
				return;

			var dataAccess = this.EnsureDataAccess();

			foreach(var member in members)
			{
				dataAccess.Execute("Security.SetMember", new Dictionary<string, object>
				{
					{"RoleId", member.RoleId},
					{"MemberName", member.MemberId},
					{"MemberType", member.MemberType},
				});
			}
		}
		#endregion
	}
}

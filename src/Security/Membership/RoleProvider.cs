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

namespace Zongsoft.Security.Membership
{
	public class RoleProvider : IRoleProvider, IMemberProvider
	{
		#region 成员字段
		private IDataAccess _dataAccess;
		private ICertificationProvider _certificationProvider;
		#endregion

		#region 公共属性
		public IDataAccess DataAccess
		{
			get
			{
				return _dataAccess;
			}
			set
			{
				if(value == null)
					throw new ArgumentNullException();

				_dataAccess = value;
			}
		}

		public ICertificationProvider CertificationProvider
		{
			get
			{
				return _certificationProvider;
			}
			set
			{
				if(value == null)
					throw new ArgumentNullException();

				_certificationProvider = value;
			}
		}
		#endregion

		#region 角色管理
		public Role GetRole(string certificationId, int roleId)
		{
			var objectAccess = this.GetDataAccess();

			return objectAccess.Select<Role>("Security.Role", new ConditionCollection(ConditionCombine.And)
			{
				new Condition("RoleId", roleId),
			}).FirstOrDefault();
		}

		public IEnumerable<Role> GetAllRoles(string certificationId)
		{
			var objectAccess = this.GetDataAccess();

			return objectAccess.Select<Role>("Security.Role", new ConditionCollection(ConditionCombine.And)
			{
				new Condition("Namespace", this.GetNamespace(certificationId)),
			});
		}

		public IEnumerable<Role> GetRoles(string certificationId, int memberId, MemberType memberType)
		{
			var objectAccess = this.GetDataAccess();

			var roles = objectAccess.Execute("Security.GetRoles", new Dictionary<string, object>
			{
				{"ApplicationId", this.GetNamespace(certificationId)},
				{"MemberId", memberId},
				{"MemberType", memberType},
			}) as IEnumerable<Role>;

			return roles;
		}

		public IEnumerable<Role> GetRoles(string certificationId, int memberId, MemberType memberType, int depth)
		{
			throw new NotImplementedException();
		}

		public int DeleteRoles(string certificationId, params int[] roleIds)
		{
			if(roleIds == null || roleIds.Length < 1)
				return 0;

			var objectAccess = this.GetDataAccess();

			return objectAccess.Delete("Security.Role", new Condition("RoleId", roleIds, ConditionOperator.In));
		}

		public void CreateRoles(string certificationId, IEnumerable<Role> roles)
		{
			if(roles == null)
				return;

			var objectAccess = this.GetDataAccess();
			objectAccess.Insert("Security.Role", roles);
		}

		public void UpdateRoles(string certificationId, IEnumerable<Role> roles)
		{
			if(roles == null)
				return;

			var objectAccess = this.GetDataAccess();

			foreach(var role in roles)
			{
				if(role == null)
					continue;

				objectAccess.Update("Security.Role", role, new ConditionCollection(ConditionCombine.And)
				{
					new Condition("RoleId", role.RoleId),
				});
			}
		}
		#endregion

		#region 成员管理
		public bool InRole(string certificationId, int userId, int roleId)
		{
			return this.GetRoles(certificationId, userId, MemberType.User, -1).Any(role => role.RoleId == roleId);
		}

		public IEnumerable<Member> GetMembers(string certificationId, int roleId)
		{
			var objectAccess = this.GetDataAccess();

			var members = objectAccess.Execute("Security.GetMembers", new Dictionary<string, object>
			{
				{"RoleId", roleId},
			}) as IEnumerable<Member>;

			return members;
		}

		public IEnumerable<Member> GetMembers(string certificationId, int roleId, int depth)
		{
			throw new NotImplementedException();
		}

		public void DeleteMember(string certificationId, int roleId, int memberId, MemberType memberType)
		{
			var objectAccess = this.GetDataAccess();

			objectAccess.Delete("Security.Member", new ConditionCollection(ConditionCombine.And)
			{
				new Condition("RoleId", roleId),
				new Condition("MemberId", memberId),
				new Condition("MemberType", memberType),
			});
		}

		public void CreateMember(string certificationId, int roleId, int memberId, MemberType memberType)
		{
			var objectAccess = this.GetDataAccess();

			objectAccess.Insert("Security.Member", new Member(roleId, memberId, memberType));
		}

		public void SetMembers(string certificationId, IEnumerable<Member> members)
		{
			if(members == null)
				return;

			var dataAccess = this.GetDataAccess();

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

		#region 私有方法
		private IDataAccess GetDataAccess()
		{
			if(_dataAccess == null)
				throw new InvalidOperationException("The value of 'DataAccess' property is null.");

			return _dataAccess;
		}

		private Certification GetCertification(string certificationId)
		{
			var certificationProvider = _certificationProvider;

			if(certificationProvider == null)
				throw new InvalidOperationException("The value of 'CertificationProvider' property is null.");

			return certificationProvider.GetCertification(certificationId);
		}

		private string GetNamespace(string certificationId)
		{
			var certificationProvider = _certificationProvider;

			if(certificationProvider == null)
				throw new InvalidOperationException("The value of 'CertificationProvider' property is null.");

			return certificationProvider.GetNamespace(certificationId);
		}
		#endregion
	}
}

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
using System.Linq;
using System.Text;

using Zongsoft.Data;

namespace Zongsoft.Security.Membership
{
	public class RoleProvider : IRoleProvider
	{
		#region 成员字段
		private IDataAccess _objectAccess;
		private ICertificationProvider _certificationProvider;
		#endregion

		#region 公共属性
		public IDataAccess ObjectAccess
		{
			get
			{
				return _objectAccess;
			}
			set
			{
				if(value == null)
					throw new ArgumentNullException();

				_objectAccess = value;
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
		public Role GetRole(string certificationId, string name)
		{
			var objectAccess = this.GetObjectAccess();

			return objectAccess.Select<Role>("Security.Role", new ClauseCollection(ClauseCombine.And)
			{
				new Clause("ApplicationId", this.GetApplicationId(certificationId)),
				new Clause("Name", name),
			}).FirstOrDefault();
		}

		public IEnumerable<Role> GetAllRoles(string certificationId)
		{
			var objectAccess = this.GetObjectAccess();

			return objectAccess.Select<Role>("Security.Role", new ClauseCollection(ClauseCombine.And)
			{
				new Clause("ApplicationId", this.GetApplicationId(certificationId)),
			});
		}

		public IEnumerable<Role> GetRoles(string certificationId, string memberName, MemberType memberType)
		{
			var objectAccess = this.GetObjectAccess();

			var roles = objectAccess.Execute("Security.GetRoles", new Dictionary<string, object>
			{
				{"ApplicationId", this.GetApplicationId(certificationId)},
				{"MemberName", memberName},
				{"MemberType", memberType},
			}) as IEnumerable<Role>;

			return roles;
		}

		public IEnumerable<Role> GetRoles(string certificationId, string memberName, MemberType memberType, int depth)
		{
			throw new NotImplementedException();
		}

		public int DeleteRoles(string certificationId, params string[] names)
		{
			if(names == null || names.Length < 1)
				return 0;

			var count = 0;
			var objectAccess = this.GetObjectAccess();

			foreach(var name in names)
			{
				if(string.IsNullOrWhiteSpace(name))
					continue;

				count += objectAccess.Delete("Security.Role", new ClauseCollection(ClauseCombine.And)
				{
					new Clause("ApplicationId", this.GetApplicationId(certificationId)),
					new Clause("Name", name),
				});
			}

			return count;
		}

		public void CreateRoles(string certificationId, IEnumerable<Role> roles)
		{
			if(roles == null)
				return;

			var objectAccess = this.GetObjectAccess();
			objectAccess.Insert("Security.Role", roles);
		}

		public void UpdateRoles(string certificationId, IEnumerable<Role> roles)
		{
			if(roles == null)
				return;

			var objectAccess = this.GetObjectAccess();

			foreach(var user in roles)
			{
				if(user == null)
					continue;

				objectAccess.Update("Security.Role", user, new ClauseCollection(ClauseCombine.And)
				{
					new Clause("ApplicationId", this.GetApplicationId(certificationId)),
					new Clause("Name", user.Name),
				});
			}
		}
		#endregion

		#region 成员管理
		public bool InRole(string certificationId, string roleName)
		{
			return this.InRole(certificationId, roleName, 0);
		}

		public bool InRole(string certificationId, string roleName, int depth)
		{
			var certification = this.GetCertification(certificationId);

			return this.GetRoles(certificationId, certification.UserName, MemberType.User, depth)
			           .Any(role => string.Equals(role.Name, roleName, StringComparison.OrdinalIgnoreCase));
		}

		public IEnumerable<Member> GetMembers(string certificationId, string roleName)
		{
			var objectAccess = this.GetObjectAccess();

			var members = objectAccess.Execute("Security.GetMembers", new Dictionary<string, object>
			{
				{"ApplicationId", this.GetApplicationId(certificationId)},
				{"RoleName", roleName},
			}) as IEnumerable<Member>;

			return members;
		}

		public IEnumerable<Member> GetMembers(string certificationId, string roleName, int depth)
		{
			throw new NotImplementedException();
		}

		public void DeleteMember(string certificationId, string roleName, string memberName, MemberType memberType)
		{
			if(string.IsNullOrWhiteSpace(roleName))
				throw new ArgumentNullException("roleName");

			if(string.IsNullOrWhiteSpace(memberName))
				throw new ArgumentNullException("memberName");

			var objectAccess = this.GetObjectAccess();

			objectAccess.Delete("Security.Member", new ClauseCollection(ClauseCombine.And)
			{
				new Clause("ApplicationId", this.GetApplicationId(certificationId)),
				new Clause("RoleName", roleName),
				new Clause("MemberName", memberName),
				new Clause("MemberType", memberType),
			});
		}

		public void CreateMember(string certificationId, string roleName, string memberName, MemberType memberType)
		{
			if(string.IsNullOrWhiteSpace(roleName))
				throw new ArgumentNullException("roleName");

			if(string.IsNullOrWhiteSpace(memberName))
				throw new ArgumentNullException("memberName");

			var objectAccess = this.GetObjectAccess();

			objectAccess.Insert("Security.Member",
			                    new Member(this.GetApplicationId(certificationId), roleName, memberName, memberType));
		}

		public void SetMembers(string certificationId, IEnumerable<Member> members)
		{
			if(members == null)
				return;

			var objectAccess = this.GetObjectAccess();
			var applicationId = this.GetApplicationId(certificationId);

			foreach(var member in members.Where(m => string.Equals(m.ApplicationId, applicationId, StringComparison.OrdinalIgnoreCase)))
			{
				objectAccess.Execute("Security.SetMember", new Dictionary<string, object>
				{
					{"ApplicationId", applicationId},
					{"RoleName", member.RoleName},
					{"MemberName", member.MemberName},
					{"MemberType", member.MemberType},
				});
			}
		}
		#endregion

		#region 私有方法
		private IDataAccess GetObjectAccess()
		{
			if(_objectAccess == null)
				throw new InvalidOperationException("The value of 'ObjectAccess' property is null.");

			return _objectAccess;
		}

		private Certification GetCertification(string certificationId)
		{
			var certificationProvider = _certificationProvider;

			if(certificationProvider == null)
				throw new InvalidOperationException("The value of 'CertificationProvider' property is null.");

			return certificationProvider.GetCertification(certificationId);
		}

		private string GetApplicationId(string certificationId)
		{
			var certificationProvider = _certificationProvider;

			if(certificationProvider == null)
				throw new InvalidOperationException("The value of 'CertificationProvider' property is null.");

			return certificationProvider.GetApplicationId(certificationId);
		}
		#endregion
	}
}

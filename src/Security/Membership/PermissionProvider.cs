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

using Zongsoft.Data;

namespace Zongsoft.Security.Membership
{
	public class PermissionProvider : IPermissionProvider
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

		#region 公共方法
		public IEnumerable<Permission> GetPermissions(string certificationId, string memberName, MemberType memberType)
		{
			var objectAccess = this.GetObjectAccess();

			return objectAccess.Select<Permission>("Security.Permission",
			                                       new ConditionCollection(ConditionCombine.And)
			                                       {
													   new Condition("ApplicationId", this.GetApplicationId(certificationId)),
													   new Condition("MemberName", memberName),
													   new Condition("MemberType", memberType),
			                                       });
		}

		public void SetPermissions(string certificationId, string memberName, MemberType memberType, IEnumerable<Permission> permissions)
		{
			if(string.IsNullOrWhiteSpace(memberName))
				throw new ArgumentNullException("memberName");

			if(permissions == null)
				throw new ArgumentNullException("permissions");

			var objectAccess = this.GetObjectAccess();
			var applicationId = this.GetApplicationId(certificationId);

			foreach(var permission in permissions)
			{
				objectAccess.Execute("Security.Permission.Set", new
				{
					ApplicationId = applicationId,
					MemberName = memberName,
					MemberType = memberType,
					SchemaId = permission.SchemaId,
					ActionId = permission.ActionId,
					Granted = permission.Granted,
				});
			}
		}

		public IEnumerable<PermissionFilter> GetPermissionFilters(string certificationId, string memberName, MemberType memberType)
		{
			var objectAccess = this.GetObjectAccess();

			return objectAccess.Select<PermissionFilter>("Security.PermissionFilter",
												         new ConditionCollection(ConditionCombine.And)
			                                             {
															 new Condition("ApplicationId", this.GetApplicationId(certificationId)),
															 new Condition("MemberName", memberName),
															 new Condition("MemberType", memberType),
			                                             });
		}

		public void SetPermissionFilters(string certificationId, string memberName, MemberType memberType, IEnumerable<PermissionFilter> permissionFilters)
		{
			if(string.IsNullOrWhiteSpace(memberName))
				throw new ArgumentNullException("memberName");

			if(permissionFilters == null)
				throw new ArgumentNullException("permissionFilters");

			var objectAccess = this.GetObjectAccess();
			var applicationId = this.GetApplicationId(certificationId);

			foreach(var permissionFilter in permissionFilters)
			{
				objectAccess.Execute("Security.PermissionFilter.Set", new
				{
					ApplicationId = applicationId,
					MemberName = memberName,
					MemberType = memberType,
					SchemaId = permissionFilter.SchemaId,
					ActionId = permissionFilter.ActionId,
					Filter = permissionFilter.Filter,
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

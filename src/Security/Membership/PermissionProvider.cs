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

using Zongsoft.Data;
using Zongsoft.Options;

namespace Zongsoft.Security.Membership
{
	public class PermissionProvider : ProviderBase, IPermissionProvider
	{
		#region 构造函数
		public PermissionProvider(ISettingsProvider settings) : base(settings)
		{
		}
		#endregion

		#region 公共方法
		public IEnumerable<Permission> GetPermissions(int memberId, MemberType memberType)
		{
			var objectAccess = this.EnsureDataAccess();

			return objectAccess.Select<Permission>(MembershipHelper.DATA_ENTITY_PERMISSION,
			                                       new ConditionCollection(ConditionCombine.And)
			                                       {
													   new Condition("MemberId", memberId),
													   new Condition("MemberType", memberType),
			                                       });
		}

		public void SetPermissions(int memberId, MemberType memberType, IEnumerable<Permission> permissions)
		{
			if(permissions == null)
				throw new ArgumentNullException("permissions");

			var objectAccess = this.EnsureDataAccess();

			foreach(var permission in permissions)
			{
				objectAccess.Execute("Security.Permission.Set", new
				{
					MemberId = memberId,
					MemberType = memberType,
					SchemaId = permission.SchemaId,
					ActionId = permission.ActionId,
					Granted = permission.Granted,
				});
			}
		}

		public IEnumerable<PermissionFilter> GetPermissionFilters(int memberId, MemberType memberType)
		{
			var objectAccess = this.EnsureDataAccess();

			return objectAccess.Select<PermissionFilter>("Security.PermissionFilter",
												         new ConditionCollection(ConditionCombine.And)
			                                             {
															 new Condition("MemberId", memberId),
															 new Condition("MemberType", memberType),
			                                             });
		}

		public void SetPermissionFilters(int memberId, MemberType memberType, IEnumerable<PermissionFilter> permissionFilters)
		{
			if(permissionFilters == null)
				throw new ArgumentNullException("permissionFilters");

			var objectAccess = this.EnsureDataAccess();

			foreach(var permissionFilter in permissionFilters)
			{
				objectAccess.Execute("Security.PermissionFilter.Set", new
				{
					MemberId = memberId,
					MemberType = memberType,
					SchemaId = permissionFilter.SchemaId,
					ActionId = permissionFilter.ActionId,
					Filter = permissionFilter.Filter,
				});
			}
		}
		#endregion
	}
}

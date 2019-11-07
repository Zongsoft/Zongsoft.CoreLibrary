/*
 *   _____                                ______
 *  /_   /  ____  ____  ____  _________  / __/ /_
 *    / /  / __ \/ __ \/ __ \/ ___/ __ \/ /_/ __/
 *   / /__/ /_/ / / / / /_/ /\_ \/ /_/ / __/ /_
 *  /____/\____/_/ /_/\__  /____/\____/_/  \__/
 *                   /____/
 *
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2015-2019 Zongsoft Corporation <http://www.zongsoft.com>
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
	/// 提供关于权限管理的接口。
	/// </summary>
	public interface IPermissionProvider
	{
		/// <summary>
		/// 获取指定用户或角色的权限集。
		/// </summary>
		/// <param name="memberId">指定要获取的权限集的成员编号(用户或角色)。</param>
		/// <param name="memberType">指定要获取的权限集的成员类型。</param>
		/// <param name="schemaId">指定的要获取的权限集的目标标识，如果为空(null)或空字符串则忽略该参数。</param>
		/// <returns>返回指定用户或角色的权限集。注意：该结果集不包含指定成员所属的上级角色的权限设置。</returns>
		IEnumerable<Permission> GetPermissions(uint memberId, MemberType memberType, string schemaId = null);

		/// <summary>
		/// 设置指定用户或角色的权限集。
		/// </summary>
		/// <param name="memberId">指定的要设置的权限集的成员编号(用户或角色)。</param>
		/// <param name="memberType">指定的要设置的权限集的成员类型。</param>
		/// <param name="permissions">要设置更新的权限集，如果为空则表示清空指定成员的权限集。</param>
		/// <param name="shouldResetting">指示是否以重置的方式更新权限集。如果为真表示写入之前先清空指定成员下的所有权限设置；否则如果指定的权限项存在则更新它，不存在则新增。</param>
		/// <returns>返回设置成功的记录数。</returns>
		int SetPermissions(uint memberId, MemberType memberType, IEnumerable<Permission> permissions, bool shouldResetting = false);

		/// <summary>
		/// 设置指定用户或角色的权限集。
		/// </summary>
		/// <param name="memberId">指定的要设置的权限集的成员编号(用户或角色)。</param>
		/// <param name="memberType">指定的要设置的权限集的成员类型。</param>
		/// <param name="schemaId">指定的要设置的权限集的目标标识，如果为空(null)或空字符串则忽略该参数。</param>
		/// <param name="permissions">要设置更新的权限集，如果为空则表示清空指定成员的权限集。</param>
		/// <param name="shouldResetting">指示是否以重置的方式更新权限集。如果为真表示写入之前先清空指定成员下的所有权限设置；否则如果指定的权限项存在则更新它，不存在则新增。</param>
		/// <returns>返回设置成功的记录数。</returns>
		int SetPermissions(uint memberId, MemberType memberType, string schemaId, IEnumerable<Permission> permissions, bool shouldResetting = false);

		/// <summary>
		/// 移除单个权限设置项。
		/// </summary>
		/// <param name="memberId">指定的要移除的权限成员编号(用户或角色)。</param>
		/// <param name="memberType">指定的要移除的权限成员类型。</param>
		/// <param name="schemaId">指定的要移除的权限目标标识。</param>
		/// <param name="actionId">指定的要移除的权限操作标识。</param>
		/// <returns>如果移除成功则返回真(True)，否则返回假(False)。</returns>
		bool RemovePermission(uint memberId, MemberType memberType, string schemaId, string actionId);

		/// <summary>
		/// 获取指定用户或角色的权限过滤集。
		/// </summary>
		/// <param name="memberId">指定要获取的权限过滤集的成员编号(用户或角色)。</param>
		/// <param name="memberType">指定要获取的权限过滤集的成员类型。</param>
		/// <param name="schemaId">指定的要获取的权限过滤集的目标标识，如果为空(null)或空字符串则忽略该参数。</param>
		/// <returns>返回指定用户或角色的权限过滤集。注意：该结果集不包含指定成员所属的上级角色的权限设置。</returns>
		IEnumerable<PermissionFilter> GetPermissionFilters(uint memberId, MemberType memberType, string schemaId = null);

		/// <summary>
		/// 设置指定用户或角色的权限过滤集。
		/// </summary>
		/// <param name="memberId">指定的要设置的权限过滤集的成员编号(用户或角色)。</param>
		/// <param name="memberType">指定的要设置的权限过滤集的成员类型。</param>
		/// <param name="permissionFilters">要设置更新的权限过滤集，如果为空则表示清空指定成员的权限过滤集。</param>
		/// <param name="shouldResetting">指示是否以重置的方式更新权限过滤集。如果为真表示写入之前先清空指定成员下的所有权限过滤设置；否则如果指定的权限过滤项存在则更新它，不存在则新增。</param>
		/// <returns>返回设置成功的记录数。</returns>
		int SetPermissionFilters(uint memberId, MemberType memberType, IEnumerable<PermissionFilter> permissionFilters, bool shouldResetting = false);

		/// <summary>
		/// 设置指定用户或角色的权限过滤集。
		/// </summary>
		/// <param name="memberId">指定的要设置的权限过滤集的成员编号(用户或角色)。</param>
		/// <param name="memberType">指定的要设置的权限过滤集的成员类型。</param>
		/// <param name="schemaId">指定的要设置的权限过滤集的目标标识，如果为空(null)或空字符串则忽略该参数。</param>
		/// <param name="permissionFilters">要设置更新的权限过滤集，如果为空则表示清空指定成员的权限过滤集。</param>
		/// <param name="shouldResetting">指示是否以重置的方式更新权限过滤集。如果为真表示写入之前先清空指定成员下的所有权限过滤设置；否则如果指定的权限过滤项存在则更新它，不存在则新增。</param>
		/// <returns>返回设置成功的记录数。</returns>
		int SetPermissionFilters(uint memberId, MemberType memberType, string schemaId, IEnumerable<PermissionFilter> permissionFilters, bool shouldResetting = false);

		/// <summary>
		/// 移除单个权限过滤设置项。
		/// </summary>
		/// <param name="memberId">指定的要移除的权限成员编号(用户或角色)。</param>
		/// <param name="memberType">指定的要移除的权限成员类型。</param>
		/// <param name="schemaId">指定的要移除的权限目标标识。</param>
		/// <param name="actionId">指定的要移除的权限操作标识。</param>
		/// <returns>如果移除成功则返回真(True)，否则返回假(False)。</returns>
		bool RemovePermissionFilter(uint memberId, MemberType memberType, string schemaId, string actionId);
	}
}

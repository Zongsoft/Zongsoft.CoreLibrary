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

namespace Zongsoft.Security.Membership
{
	/// <summary>
	/// 提供关于角色和角色成员管理的接口。
	/// </summary>
	public interface IRoleProvider
	{
		/// <summary>
		/// 获取指定名称对应的角色对象。
		/// </summary>
		/// <param name="certificationId">调用者的安全凭证号。</param>
		/// <param name="roleId">要查找的角色名。</param>
		/// <returns>返回由<paramref name="roleId"/>参数指定的角色对象，如果没有找到指定名称的角色则返回空。</returns>
		Role GetRole(string certificationId, int roleId);

		/// <summary>
		/// 获取当前命名空间中的所有角色。
		/// </summary>
		/// <param name="certificationId">调用者的安全凭证号。</param>
		/// <returns>返回当前系统中的所有角色对象集。</returns>
		IEnumerable<Role> GetAllRoles(string certificationId);

		/// <summary>
		/// 获取指定成员的父级角色集。
		/// </summary>
		/// <param name="certificationId">调用者的安全凭证号。</param>
		/// <param name="memberId">要搜索的成员编号(用户或角色)。</param>
		/// <param name="memberType">要搜索的成员类型。</param>
		/// <returns>返回指定成员的父级角色集。</returns>
		IEnumerable<Role> GetRoles(string certificationId, int memberId, MemberType memberType);

		/// <summary>
		/// 获取指定角色成员的上级角色集。
		/// </summary>
		/// <param name="certificationId">调用者的安全凭证号。</param>
		/// <param name="memberId">要搜索的角色成员编号。</param>
		/// <param name="memberType">要搜索的角色成员类型。</param>
		/// <param name="depth">对搜索角色成员隶属关系的遍历深度，如果不限深度则为负数。</param>
		/// <returns>返回指定成员的上级角色集。</returns>
		IEnumerable<Role> GetRoles(string certificationId, int memberId, MemberType memberType, int depth);

		/// <summary>
		/// 删除指定名称集的多个角色。
		/// </summary>
		/// <param name="certificationId">调用者的安全凭证号。</param>
		/// <param name="roleIds">要删除的角色编号集合。</param>
		/// <returns>如果删除成功则返回删除的数量，否则返回零。</returns>
		int DeleteRoles(string certificationId, params int[] roleIds);

		/// <summary>
		/// 创建单个或者多个角色。
		/// </summary>
		/// <param name="certificationId">调用者的安全凭证号。</param>
		/// <param name="roles">要创建的角色对象集。</param>
		/// <remarks>如果创建失败则抛出异常，并且整个事务会被回滚。</remarks>
		void CreateRoles(string certificationId, IEnumerable<Role> roles);

		/// <summary>
		/// 更新单个或多个角色信息。
		/// </summary>
		/// <param name="certificationId">调用者的安全凭证号。</param>
		/// <param name="roles">要更新的角色对象集。</param>
		/// <remarks>如果在批量更新中，如果待更新的角色对象在数据源中不存在则该项操作将被忽略，而不影响本次操作中的其他对象的更新。</remarks>
		void UpdateRoles(string certificationId, IEnumerable<Role> roles);
	}
}

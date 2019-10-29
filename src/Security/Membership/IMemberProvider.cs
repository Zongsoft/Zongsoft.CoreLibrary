/*
 *   _____                                ______
 *  /_   /  ____  ____  ____  _________  / __/ /_
 *    / /  / __ \/ __ \/ __ \/ ___/ __ \/ /_/ __/
 *   / /__/ /_/ / / / / /_/ /\_ \/ /_/ / __/ /_
 *  /____/\____/_/ /_/\__  /____/\____/_/  \__/
 *                   /____/
 *
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@qq.com>
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
	/// 提供关于角色成员管理的接口。
	/// </summary>
	public interface IMemberProvider
	{
		/// <summary>
		/// 获取指定成员的父级角色集。
		/// </summary>
		/// <param name="memberId">要搜索的成员编号(用户或角色)。</param>
		/// <param name="memberType">要搜索的成员类型。</param>
		/// <returns>返回指定成员的父级角色集。</returns>
		IEnumerable<IRole> GetRoles(uint memberId, MemberType memberType);

		/// <summary>
		/// 获取指定角色的直属成员集。
		/// </summary>
		/// <param name="roleId">要搜索的角色编号。</param>
		/// <param name="schema">指定的数据模式表达式文本。</param>
		/// <returns>返回隶属于指定角色的直属子级成员集。</returns>
		IEnumerable<Member> GetMembers(uint roleId, string schema = null);

		/// <summary>
		/// 设置更新指定角色下的成员。
		/// </summary>
		/// <param name="roleId">指定要更新的角色编号。</param>
		/// <param name="members">要更新的角色成员集。</param>
		/// <param name="shouldResetting">指示是否以重置的方式更新角色成员，即是否在更新角色成员之前先清空指定角色下的所有成员。默认值为假(False)。</param>
		/// <returns>如果更新成功则返回更新的数量，否则返回零。</returns>
		int SetMembers(uint roleId, IEnumerable<Member> members, bool shouldResetting = false);

		/// <summary>
		/// 删除单个角色成员。
		/// </summary>
		/// <param name="roleId">指定要删除成员所属的角色编号。</param>
		/// <param name="memberId">指定要删除的成员编号。</param>
		/// <param name="memberType">指定要删除的成员类型。</param>
		/// <returns>如果删除成功则返回真(True)，否则返回假(False)。</returns>
		bool Delete(uint roleId, uint memberId, MemberType memberType);

		/// <summary>
		/// 清空指定角色的成员设置。
		/// </summary>
		/// <param name="roleId">指定要清空的角色编号。</param>
		/// <returns>返回清空的成员数。</returns>
		int Delete(uint roleId);
	}
}

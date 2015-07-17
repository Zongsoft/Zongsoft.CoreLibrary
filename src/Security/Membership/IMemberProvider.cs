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
	/// 提供关于角色成员管理的接口。
	/// </summary>
	public interface IMemberProvider
	{
		/// <summary>
		/// 确定指定的用户是否属于指定的角色。
		/// </summary>
		/// <param name="userId">要检查的用户编号。</param>
		/// <param name="roleId">要确认的角色编号。</param>
		/// <returns>如果指定的用户是指定角色的成员，则为真(true)；否则为假(false)。</returns>
		bool InRole(int userId, int roleId);

		/// <summary>
		/// 获取指定角色的直属成员集。
		/// </summary>
		/// <param name="roleId">要搜索的角色编号。</param>
		/// <returns>返回隶属于指定角色的直属子级成员集。</returns>
		IEnumerable<Member> GetMembers(int roleId);

		/// <summary>
		/// 获取指定角色的成员集。
		/// </summary>
		/// <param name="roleId">要搜索的角色编号。</param>
		/// <param name="depth">对搜索角色成员隶属关系的遍历深度，如果不限深度则为负数。</param>
		/// <returns>返回隶属于指定角色下特定深度的所有成员集。</returns>
		IEnumerable<Member> GetMembers(int roleId, int depth);

		/// <summary>
		/// 删除隶属于指定角色下的成员。
		/// </summary>
		/// <param name="roleId">要删除的角色成员所属的角色名称。</param>
		/// <param name="memberId">要删除的角色成员名称。</param>
		/// <param name="memberType">要删除的角色成员类型。</param>
		void DeleteMember(int roleId, int memberId, MemberType memberType);

		/// <summary>
		/// 创建隶属于指定角色下的成员。
		/// </summary>
		/// <param name="roleId">待创建的角色成员所属的角色编号。</param>
		/// <param name="memberId">带创建的角色成员编号。</param>
		/// <param name="memberType">带创建的角色成员类型。</param>
		void CreateMember(int roleId, int memberId, MemberType memberType);

		/// <summary>
		/// 设置更新指定角色下的所有成员。
		/// </summary>
		/// <param name="roleId">指定要更新的角色编号。</param>
		/// <param name="members">要更新的角色成员集。</param>
		/// <remarks>
		///		<para>该方法默认以覆盖方式进行更新。即先清空指定角色下的所有成员记录，然后再将<paramref name="members"/>参数指定的成员插入其中。</para>
		/// </remarks>
		void SetMembers(int roleId, IEnumerable<Member> members);
	}
}

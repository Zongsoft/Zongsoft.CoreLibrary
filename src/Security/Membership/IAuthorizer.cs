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
	/// 提供关于用户授权相关功能的接口。
	/// </summary>
	public interface IAuthorizer
	{
		/// <summary>表示授权验证开始事件。</summary>
		event EventHandler<AuthorizationContext> Authorizing;
		/// <summary>表示授权验证完成事件。</summary>
		event EventHandler<AuthorizationContext> Authorized;

		/// <summary>
		/// 判断指定的用户对特定目标对象的特定行为是否具有授权。
		/// </summary>
		/// <param name="userId">指定的用户编号。</param>
		/// <param name="schema">指定是否授权的资源。</param>
		/// <param name="action">对应目标的特定行为。</param>
		/// <returns>如果具有授权则返回真(true)，否则返回假(false)。</returns>
		/// <remarks>
		/// 	<para>该验证会对指定的用户所属角色进行逐级向上展开做授权判断，因此只需对本方法一次调用即可得知当前用户对指定目标的特定行为的最终授权计算结果。</para>
		/// </remarks>
		bool Authorize(uint userId, string schema, string action);

		/// <summary>
		/// 判断指定的用户对特定目标对象的特定行为是否具有授权。
		/// </summary>
		/// <param name="user">指定的用户对象。</param>
		/// <param name="schema">指定是否授权的资源。</param>
		/// <param name="action">对应目标的特定行为。</param>
		/// <returns>如果具有授权则返回真(true)，否则返回假(false)。</returns>
		/// <remarks>
		/// 	<para>该验证会对指定的用户所属角色进行逐级向上展开做授权判断，因此只需对本方法一次调用即可得知当前用户对指定目标的特定行为的最终授权计算结果。</para>
		/// </remarks>
		bool Authorize(IUserIdentity user, string schema, string action);

		/// <summary>
		/// 获取指定用户的最终授权状态集。
		/// </summary>
		/// <param name="user">指定要获取的最终授权状态集的用户对象。</param>
		/// <returns>返回指定用户的最终授权状态集。注意：该集合仅包含了最终的已授权状态信息。</returns>
		/// <remarks>
		/// 	<para>注意：该集合仅包含了最终的已授权状态信息。</para>
		/// 	<para>该方法对指定用户及其所属角色进行逐级向上展开做授权计算，因此只需对本方法一次调用即可得知指定用户的最终授权计算结果。</para>
		/// </remarks>
		IEnumerable<AuthorizationToken> Authorizes(IUserIdentity user);

		/// <summary>
		/// 获取指定角色的最终授权状态集。
		/// </summary>
		/// <param name="role">指定要获取的最终授权状态集的角色对象。</param>
		/// <returns>返回指定角色的最终授权状态集。注意：该集合仅包含了最终的已授权状态信息。</returns>
		/// <remarks>
		/// 	<para>注意：该集合仅包含了最终的已授权状态信息。</para>
		/// 	<para>该方法对指定角色及其所属角色进行逐级向上展开做授权计算，因此只需对本方法一次调用即可得知指定角色的最终授权计算结果。</para>
		/// </remarks>
		IEnumerable<AuthorizationToken> Authorizes(IRole role);

		/// <summary>
		/// 获取指定用户或角色的最终授权状态集。
		/// </summary>
		/// <param name="memberId">指定要获取的最终授权状态集的成员编号(用户或角色编号)。</param>
		/// <param name="memberType">指定要获取的最终授权状态集的成员类型，默认为用户。</param>
		/// <returns>返回指定成员的最终授权状态集。注意：该集合仅包含了最终的已授权状态信息。</returns>
		/// <remarks>
		/// 	<para>注意：该集合仅包含了最终的已授权状态信息。</para>
		/// 	<para>该方法对指定用户及其所属角色进行逐级向上展开做授权计算，因此只需对本方法一次调用即可得知指定成员的最终授权计算结果。</para>
		/// </remarks>
		IEnumerable<AuthorizationToken> Authorizes(uint memberId, MemberType memberType = MemberType.User);

		/// <summary>
		/// 确定指定的用户是否属于指定的角色。
		/// </summary>
		/// <param name="userId">要检查的用户编号。</param>
		/// <param name="roleNames">要确认的角色名称数组。</param>
		/// <returns>如果指定的用户是指定角色名称中的任一成员则返回真(true)；否则返回假(false)。</returns>
		bool InRoles(uint userId, params string[] roleNames);

		/// <summary>
		/// 确定指定的用户是否属于指定的角色。
		/// </summary>
		/// <param name="user">要检查的用户对象。</param>
		/// <param name="roleNames">要确认的角色名称数组。</param>
		/// <returns>如果指定的用户是指定角色名称中的任一成员则返回真(true)；否则返回假(false)。</returns>
		bool InRoles(IUserIdentity user, params string[] roleNames);
	}
}

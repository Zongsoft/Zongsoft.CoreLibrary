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
 * Copyright (C) 2003-2018 Zongsoft Corporation <http://www.zongsoft.com>
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
	/// 表示权限系统角色的实体接口。
	/// </summary>
	[Zongsoft.Data.Model("Security.Role")]
	public interface IRole
	{
		/// <summary>
		/// 获取或设置角色编号。
		/// </summary>
		uint RoleId
		{
			get; set;
		}

		/// <summary>
		/// 获取或设置角色名称。
		/// </summary>
		string Name
		{
			get; set;
		}

		/// <summary>
		/// 获取或设置角色全称。
		/// </summary>
		string FullName
		{
			get; set;
		}

		/// <summary>
		/// 获取或设置角色所属的命名空间。
		/// </summary>
		string Namespace
		{
			get; set;
		}

		/// <summary>
		/// 获取或设置角色的描述信息。
		/// </summary>
		string Description
		{
			get; set;
		}

		/// <summary>
		/// 获取或设置角色成员集。
		/// </summary>
		IEnumerable<Member> Members
		{
			get; set;
		}
	}
}

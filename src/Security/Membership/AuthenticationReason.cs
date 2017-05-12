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
using System.ComponentModel;

namespace Zongsoft.Security.Membership
{
	/// <summary>
	/// 表示验证失败原因的枚举。
	/// </summary>
	public enum AuthenticationReason
	{
		/// <summary>验证成功</summary>
		[Description("${Text.AuthenticationReason.Succeed}")]
		Succeed = 0,

		/// <summary>未知的原因</summary>
		[Description("${Text.AuthenticationReason.Unknown}")]
		Unknown = -1,
		/// <summary>禁止验证通过</summary>
		[Description("${Text.AuthenticationReason.Forbidden}")]
		Forbidden = -2,

		/// <summary>无效的身份标识</summary>
		[Description("${Text.AuthenticationReason.InvalidIdentity}")]
		InvalidIdentity = 1,
		/// <summary>无效的密码</summary>
		[Description("${Text.AuthenticationReason.InvalidPassword}")]
		InvalidPassword = 2,
		/// <summary>帐户尚未批准</summary>
		[Description("${Text.AuthenticationReason.AccountUnapproved}")]
		AccountUnapproved = 3,
		/// <summary>帐户被暂时挂起（可能是因为密码验证失败次数过多。）</summary>
		[Description("${Text.AuthenticationReason.AccountSuspended}")]
		AccountSuspended = 4,
		/// <summary>帐户已被禁用</summary>
		[Description("${Text.AuthenticationReason.AccountDisabled}")]
		AccountDisabled = 5,
	}
}

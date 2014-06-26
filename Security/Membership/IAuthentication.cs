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

namespace Zongsoft.Security.Membership
{
	/// <summary>
	/// 提供关于用户安全验证的接口。
	/// </summary>
	public interface IAuthentication
	{
		/// <summary>
		/// 验证指定名称的用户是否有效并且和指定的密码是否完全匹配。
		/// </summary>
		/// <param name="applicationId">要验证的系统编号。</param>
		/// <param name="userName">要验证的用户的名称。</param>
		/// <param name="password">指定用户的密码。</param>
		/// <returns>如果验证成功则返回一个有效的<see cref="AuthenticationResult"/>对象。验证失败会抛出<seealso cref="Zongsoft.Security.Membership.AuthenticationException"/>异常。</returns>
		/// <exception cref="Zongsoft.Security.Membership.AuthenticationException">当验证失败。</exception>
		AuthenticationResult Authenticate(string applicationId, string userName, string password);
	}
}

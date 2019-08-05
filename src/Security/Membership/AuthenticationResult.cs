﻿/*
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
	/// 表示验证方法结果的结构。
	/// </summary>
	public struct AuthenticationResult
	{
		#region 公共字段
		/// <summary>
		/// 获取验证通过后的用户对象，如果验证失败则返回空(null)。
		/// </summary>
		public readonly IUserIdentity User;

		/// <summary>
		/// 获取身份验证的应用场景。
		/// </summary>
		public readonly string Scene;

		/// <summary>
		/// 获取验证结果的扩展参数集。
		/// </summary>
		public readonly IDictionary<string, object> Parameters;
		#endregion

		#region 构造函数
		public AuthenticationResult(IUserIdentity user, string scene, IDictionary<string, object> parameters = null)
		{
			this.User = user ?? throw new ArgumentNullException(nameof(user));
			this.Scene = scene;
			this.Parameters = parameters;
		}
		#endregion
	}
}

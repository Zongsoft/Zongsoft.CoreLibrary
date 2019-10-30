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
using System.Linq;
using System.ComponentModel;

namespace Zongsoft.Security.Membership
{
	[AttributeUsage((AttributeTargets.Class | AttributeTargets.Method), AllowMultiple = false, Inherited = true)]
	public class AuthorizationAttribute : Attribute
	{
		#region 构造函数
		public AuthorizationAttribute()
		{
		}

		public AuthorizationAttribute(string schema)
		{
			this.Schema = schema;
			this.Action = null;
		}

		public AuthorizationAttribute(string schema, string action)
		{
			this.Schema = schema;
			this.Action = action;
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取或设置一个值，指示是否禁止验证。
		/// </summary>
		public bool Suppressed
		{
			get; set;
		}

		/// <summary>
		/// 获取或设置当前身份必须所属角色集（注：多个角色名之间以逗号分隔）。
		/// </summary>
		public string Roles
		{
			get; set;
		}

		/// <summary>
		/// 获取或设置身份验证的质询器类型。
		/// </summary>
		public Type ChallengerType
		{
			get; set;
		}

		/// <summary>
		/// 获取或设置操作名。
		/// </summary>
		public string Action
		{
			get; set;
		}

		/// <summary>
		/// 获取或设置模式名。
		/// </summary>
		public string Schema
		{
			get; set;
		}
		#endregion

		#region 公共方法
		/// <summary>
		/// 尝试获取设置的待验证的角色名数组。
		/// </summary>
		/// <param name="roles">输出参数，返回待验证的角色名数组。</param>
		/// <returns>如果<see cref="Roles"/>属性不为空或空字符串则返回真(True)，否则返回假(False)。</returns>
		public bool TryGetRoles(out string[] roles)
		{
			var text = this.Roles;

			if(string.IsNullOrEmpty(text))
			{
				roles = Array.Empty<string>();
				return false;
			}

			roles = Common.StringExtension.Slice(text, separator => separator == ',' || separator == ';' || separator == '|').ToArray();
			return roles.Length > 0;
		}
		#endregion
	}
}

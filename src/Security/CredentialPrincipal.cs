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
using System.Security.Principal;

namespace Zongsoft.Security
{
	/// <summary>
	/// 表示带凭证的用户主体类。
	/// </summary>
	public class CredentialPrincipal : IPrincipal
	{
		#region 公共字段
		public static readonly CredentialPrincipal Empty = new CredentialPrincipal(CredentialIdentity.Empty);
		#endregion

		#region 成员字段
		private readonly CredentialIdentity _identity;
		private readonly ISet<string> _roles;
		#endregion

		#region 构造函数
		public CredentialPrincipal(CredentialIdentity identity, params string[] roles)
		{
			_identity = identity ?? CredentialIdentity.Empty;

			if(roles != null && roles.Length > 0)
				_roles = new HashSet<string>(roles, StringComparer.OrdinalIgnoreCase);
		}

		public CredentialPrincipal(Credential credential, params string[] roles)
		{
			_identity = new CredentialIdentity(credential);

			if(roles != null && roles.Length > 0)
				_roles = new HashSet<string>(roles, StringComparer.OrdinalIgnoreCase);
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取当前主体的标识对象。
		/// </summary>
		public virtual CredentialIdentity Identity
		{
			get
			{
				return _identity ?? CredentialIdentity.Empty;
			}
		}

		/// <summary>
		/// 获取当前主体是否为内置的系统管理员用户(Administrator)或隶属于系统管理员角色(Administrators)。
		/// </summary>
		public bool IsAdministrator
		{
			get
			{
				var identity = this.Identity;

				if(identity != null && string.Equals(identity.Name, "Administrator", StringComparison.OrdinalIgnoreCase))
					return true;

				return _roles != null && _roles.Count > 0 && _roles.Contains("Administrators");
			}
		}
		#endregion

		#region 公共方法
		public bool InRole(string roleName)
		{
			if(string.IsNullOrEmpty(roleName) || _roles == null || _roles.Count == 0)
				return false;

			return _roles.Contains(roleName);
		}
		#endregion

		#region 显式实现
		IIdentity IPrincipal.Identity
		{
			get
			{
				return this.Identity;
			}
		}

		bool IPrincipal.IsInRole(string roleName)
		{
			return this.InRole(roleName);
		}
		#endregion

		#region 重写方法
		public override string ToString()
		{
			var identity = _identity;

			if(identity == null)
				return this.GetType().Name + ":<Empty>";

			return this.GetType().Name + ":{" + identity.ToString() + "}";
		}
		#endregion
	}
}

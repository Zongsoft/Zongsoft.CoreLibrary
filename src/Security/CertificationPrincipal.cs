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
	/// 表示一般用户主体的类。
	/// </summary>
	public class CertificationPrincipal : MarshalByRefObject, IPrincipal
	{
		#region 公共字段
		public static readonly CertificationPrincipal Empty = new CertificationPrincipal(CertificationIdentity.Empty);
		#endregion

		#region 成员字段
		private CertificationIdentity _identity;
		private string[] _roles;
		#endregion

		#region 构造函数
		public CertificationPrincipal(CertificationIdentity identity, params string[] roles)
		{
			if(identity == null)
				throw new ArgumentNullException("identity");

			_identity = identity;
			_roles = roles;
		}
		#endregion

		#region 公共属性
		public virtual CertificationIdentity Identity
		{
			get
			{
				return _identity ?? CertificationIdentity.Empty;
			}
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
			if(string.IsNullOrWhiteSpace(roleName))
				throw new ArgumentNullException("roleName");

			if(_roles == null || _roles.Length < 1)
				return false;

			foreach(var role in _roles)
			{
				if(string.Equals(role, roleName, StringComparison.OrdinalIgnoreCase))
					return true;
			}

			return false;
		}
		#endregion
	}
}

/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2015 Zongsoft Corporation <http://www.zongsoft.com>
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
	[Serializable]
	public class AuthorizationEventArgs : EventArgs
	{
		#region 成员字段
		private CredentialPrincipal _principal;
		private uint _userId;
		private string _schemaId;
		private string _actionId;
		private bool _isAuthorized;
		#endregion

		#region 构造函数
		public AuthorizationEventArgs(uint userId, string schemaId, string actionId, bool isAuthorized)
		{
			_userId = userId;
			_schemaId = schemaId;
			_actionId = actionId;
			_isAuthorized = isAuthorized;
		}

		public AuthorizationEventArgs(CredentialPrincipal principal, string schemaId, string actionId, bool isAuthorized)
		{
			if(principal == null)
				throw new ArgumentNullException("principal");

			_principal = principal;
			_schemaId = schemaId;
			_actionId = actionId;
			_isAuthorized = isAuthorized;
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取待授权的<seealso cref="CredentialPrincipal"/>凭证主体。
		/// </summary>
		public CredentialPrincipal Principal
		{
			get
			{
				return _principal;
			}
		}

		/// <summary>
		/// 获取待授权的资源标识。
		/// </summary>
		public string SchemaId
		{
			get
			{
				return _schemaId;
			}
		}

		/// <summary>
		/// 获取待授权的行为标识。
		/// </summary>
		public string ActionId
		{
			get
			{
				return _actionId;
			}
		}

		/// <summary>
		/// 获取或设置是否授权通过。
		/// </summary>
		public bool IsAuthorized
		{
			get
			{
				return _isAuthorized;
			}
			set
			{
				_isAuthorized = value;
			}
		}
		#endregion
	}
}

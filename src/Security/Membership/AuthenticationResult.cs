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
using System.Collections.Generic;

namespace Zongsoft.Security.Membership
{
	/// <summary>
	/// 表示验证通过的结果。
	/// </summary>
	[Serializable]
	public class AuthenticationResult
	{
		#region 成员字段
		private string _certificationId;
		private string _message;
		private DateTime _expires;
		#endregion

		#region 构造函数
		public AuthenticationResult(string certificationId, DateTime expires) : this(certificationId, expires, string.Empty)
		{
		}

		public AuthenticationResult(string certificationId, DateTime expires, string message)
		{
			if(string.IsNullOrWhiteSpace(certificationId))
				throw new ArgumentNullException("certificationId");

			_certificationId = certificationId;
			_expires = expires;
			_message = message ?? string.Empty;
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取验证通过后的安全凭证号。
		/// </summary>
		public string CertificationId
		{
			get
			{
				return _certificationId;
			}
		}

		/// <summary>
		/// 获取安全凭证的过期时间。
		/// </summary>
		public DateTime Expires
		{
			get
			{
				return _expires;
			}
		}

		/// <summary>
		/// 获取验证通过后的消息文本。
		/// </summary>
		public string Message
		{
			get
			{
				return _message;
			}
		}
		#endregion
	}
}

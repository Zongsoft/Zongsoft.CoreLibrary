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
using System.Runtime.Serialization;

namespace Zongsoft.Security.Membership
{
	/// <summary>
	/// 身份验证失败时引发的异常。
	/// </summary>
	[Serializable]
	public class AuthenticationException : System.ApplicationException
	{
		#region 成员变量
		private AuthenticationReason _reason;
		private string _message;
		#endregion

		#region 构造函数
		public AuthenticationException()
		{
			_reason = AuthenticationReason.Unknown;
			_message = this.GetMessage(null);
		}

		public AuthenticationException(string message) : base(message, null)
		{
			_reason = AuthenticationReason.Unknown;
			_message = this.GetMessage(message);
		}

		public AuthenticationException(string message, Exception innerException) : base(message, innerException)
		{
			_reason = AuthenticationReason.Unknown;
			_message = this.GetMessage(message);
		}

		public AuthenticationException(AuthenticationReason reason) : this(reason, string.Empty, null)
		{
		}

		public AuthenticationException(AuthenticationReason reason, string message) : this(reason, message, null)
		{
		}

		public AuthenticationException(AuthenticationReason reason, string message, Exception innerException) : base(message, innerException)
		{
			_reason = reason;
			_message = this.GetMessage(message);
		}

		protected AuthenticationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			_reason = (AuthenticationReason)info.GetInt32("Reason");
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取验证失败的原因。
		/// </summary>
		public AuthenticationReason Reason
		{
			get
			{
				return _reason;
			}
		}

		public override string Message
		{
			get
			{
				return _message;
			}
		}
		#endregion

		#region 重写方法
		public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("Reason", _reason);
		}
		#endregion

		#region 私有方法
		private string GetMessage(string message)
		{
			if(string.IsNullOrWhiteSpace(message))
			{
				var entry = Zongsoft.Common.EnumUtility.GetEnumEntry(_reason);

				if(entry != null)
					return entry.Description;
			}

			return message;
		}
		#endregion
	}
}

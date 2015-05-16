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
using System.Runtime.Serialization;

namespace Zongsoft.Security.Membership
{
	/// <summary>
	/// 身份验证失败时引发的异常。
	/// </summary>
	[Serializable]
	public class AuthenticationException : System.ApplicationException
	{
		#region 常量定义
		public static readonly int UnknownFailed = -1;

		public static readonly int InvalidIdentity = 101;
		public static readonly int InvalidPassword = 102;
		public static readonly int AccountDisabled = 103;
		#endregion

		#region 成员变量
		private int _failureCode;
		#endregion

		#region 构造函数
		public AuthenticationException()
		{
		}

		public AuthenticationException(string message) : this(message, null)
		{
		}

		public AuthenticationException(string message, Exception innerException) : base(message, innerException)
		{
		}

		public AuthenticationException(int failureCode) : this(failureCode, string.Empty, null)
		{
		}

		public AuthenticationException(int failureCode, string message, Exception innerException) : base(message, innerException)
		{
			_failureCode = failureCode;
		}

		protected AuthenticationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			_failureCode = info.GetInt32("FailureCode");
		}
		#endregion

		#region 重写方法
		public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("FailureCode", _failureCode);
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 验证返回失败代码。
		/// </summary>
		public int FailureCode
		{
			get
			{
				return _failureCode;
			}
		}
		#endregion
	}
}

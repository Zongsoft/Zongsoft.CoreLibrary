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

namespace Zongsoft.Security
{
	/// <summary>
	/// 安全凭证操作相关的异常。
	/// </summary>
	[Serializable]
	public class CredentialException : System.ApplicationException
	{
		#region 成员字段
		private string _credentialId;
		private string _message;
		#endregion

		#region 构造函数
		public CredentialException()
		{
			_message = Resources.ResourceUtility.GetString("Text.CredentialException.Message");
		}

		public CredentialException(string message) : base(message, null)
		{
			_message = string.IsNullOrEmpty(message) ? Resources.ResourceUtility.GetString("Text.CredentialException.Message") : message;
		}

		public CredentialException(string message, Exception innerException) : base(message, innerException)
		{
			_message = string.IsNullOrEmpty(message) ? Resources.ResourceUtility.GetString("Text.CredentialException.Message") : message;
		}

		public CredentialException(string certificationId, string message) : this(certificationId, message, null)
		{
		}

		public CredentialException(string certificationId, string message, Exception innerException) : base(message, innerException)
		{
			_credentialId = certificationId;
			_message = string.IsNullOrEmpty(message) ? Resources.ResourceUtility.GetString("Text.CredentialException.Message") : message;
		}

		protected CredentialException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			_credentialId = info.GetString("CredentialId");
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取安全凭证号。
		/// </summary>
		public string CredentialId
		{
			get
			{
				return _credentialId;
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
			info.AddValue("CredentialId", _credentialId);
		}
		#endregion
	}
}

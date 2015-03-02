/*
 * Authors:
 *   邓祥云(X.Z. Deng) <627825056@qq.com>
 *
 * Copyright (C) 2011-2013 Zongsoft Corporation <http://www.zongsoft.com>
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
using System.Linq;
using System.Text;

namespace Zongsoft.Communication.Net
{
	public class FtpConfiguration
	{
		public FtpConfiguration()
		{
			Port = 21;
			DefaultEncoding = Encoding.ASCII;
			WelcomeMessage = "Zongsoft.FTP Server V1.0 ready.";
			ExitMessage = "Bye.";
			DataOperationTimeout = TimeSpan.FromMinutes(2);
			AllowAnonymous = false;
			Users = new Dictionary<string, FtpUserProfile>();
		}

		/// <summary>
		/// Ftp服务端口
		/// </summary>
		public int Port
		{
			get;
			set;
		}

		/// <summary>
		/// 字符编码
		/// </summary>
		public Encoding DefaultEncoding
		{
			get;
			set;
		}

		/// <summary>
		/// 欢迎信息
		/// </summary>
		public string WelcomeMessage
		{
			get;
			set;
		}

		/// <summary>
		/// 退出信息
		/// </summary>
		public string ExitMessage
		{
			get;
			set;
		}

		/// <summary>
		/// 允许匿名用户
		/// </summary>
		public bool AllowAnonymous
		{
			get;
			set;
		}

		/// <summary>
		/// 用户集合
		/// </summary>
		public IDictionary<string, FtpUserProfile> Users
		{
			get;
			set;
		}

		/// <summary>
		/// 匿名用户信息
		/// </summary>
		public FtpUserProfile AnonymousUser
		{
			get;
			set;
		}

		/// <summary>
		/// 操作超时时间
		/// </summary>
		public TimeSpan DataOperationTimeout
		{
			get;
			set;
		}
	}
}
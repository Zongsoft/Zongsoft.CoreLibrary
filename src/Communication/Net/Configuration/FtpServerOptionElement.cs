/*
 * Authors:
 *   邓祥云(X.Z. Deng) <627825056@qq.com>
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
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
using System.Text;

using Zongsoft.Options;
using Zongsoft.Options.Configuration;

namespace Zongsoft.Communication.Net.Configuration
{
	public class FtpServerOptionElement : OptionConfigurationElement
	{
		#region 常量定义
		private const string XML_PORT_ATTRIBUTE = "port";
		private const string XML_ENCODING_ATTRIBUTE = "encoding";
		private const string XML_TIMEOUT_ATTRIBUTE = "timeout";
		private const string XML_USERS_COLLECTION = "users";
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取或设置<see cref="FtpServer"/>服务器的侦听端口号，默认值为21。
		/// </summary>
		[OptionConfigurationProperty(XML_PORT_ATTRIBUTE, DefaultValue = 21)]
		public int Port
		{
			get
			{
				return (int)this[XML_PORT_ATTRIBUTE];
			}
			set
			{
				this[XML_PORT_ATTRIBUTE] = value;
			}
		}

		/// <summary>
		/// 获取或设置<see cref="FtpServer"/>服务器的文本编码类型，默认值为Ascii编码。
		/// </summary>
		[OptionConfigurationProperty(XML_ENCODING_ATTRIBUTE, DefaultValue = "ASCII")]
		public Encoding Encoding
		{
			get
			{
				return (Encoding)this[XML_ENCODING_ATTRIBUTE];
			}
			set
			{
				this[XML_ENCODING_ATTRIBUTE] = value;
			}
		}

		/// <summary>
		/// 获取或设置<see cref="FtpServer"/>服务器的操作超时时长，单位为秒。默认值为60秒。
		/// </summary>
		[OptionConfigurationProperty(XML_TIMEOUT_ATTRIBUTE, DefaultValue = 60)]
		public int Timeout
		{
			get
			{
				return (int)this[XML_TIMEOUT_ATTRIBUTE];
			}
			set
			{
				this[XML_TIMEOUT_ATTRIBUTE] = value;
			}
		}

		/// <summary>
		/// 获取<see cref="FtpServer"/>的登录用户集合。
		/// </summary>
		[OptionConfigurationProperty(XML_USERS_COLLECTION)]
		public FtpUserOptionElementCollection Users
		{
			get
			{
				return (FtpUserOptionElementCollection)this[XML_USERS_COLLECTION];
			}
		}
		#endregion
	}
}

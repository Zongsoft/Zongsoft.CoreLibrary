// Authors:
//    钟峰(Popeye Zhong) <zongsoft@gmail.com>
//  
// Copyright (C) 2011-2013 Zongsoft Corporation <http://www.zongsoft.com>
// 
// This file is part of Zongsoft.CoreLibrary.
// 
// Zongsoft.CoreLibrary is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// Zongsoft.CoreLibrary is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// Lesser General Public License for more details.
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with Zongsoft.CoreLibrary; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA

using System;
using System.Collections.Generic;

using Zongsoft.Options;
using Zongsoft.Options.Configuration;

namespace Zongsoft.Communication.Net.Configuration
{
	public class FtpUserOptionElement : OptionConfigurationElement
	{
		#region 常量定义
		private const string XML_USERNAME_ATTRIBUTE = "name";
		private const string XML_PASSWORD_ATTRIBUTE = "password";
		private const string XML_HOMEDIRECTORY_ATTRIBUTE = "homeDirectory";
		private const string XML_READONLY_ATTRIBUTE = "readOnly";
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取或设置<see cref="FtpServer"/>服务器的登录用户名。
		/// </summary>
		[OptionConfigurationProperty(XML_USERNAME_ATTRIBUTE, Type = typeof(string), Behavior = OptionConfigurationPropertyBehavior.IsKey)]
		public string Name
		{
			get
			{
				return (string)this[XML_USERNAME_ATTRIBUTE];
			}
			set
			{
				this[XML_USERNAME_ATTRIBUTE] = value;
			}
		}

		/// <summary>
		/// 获取或设置<see cref="FtpServer"/>服务器的登录用户密码。
		/// </summary>
		[OptionConfigurationProperty(XML_PASSWORD_ATTRIBUTE, Type = typeof(string))]
		public string Password
		{
			get
			{
				return (string)this[XML_PASSWORD_ATTRIBUTE];
			}
			set
			{
				this[XML_PASSWORD_ATTRIBUTE] = value;
			}
		}

		/// <summary>
		/// 获取或设置<see cref="FtpServer"/>服务器的用户的主目录路径。
		/// </summary>
		[OptionConfigurationProperty(XML_HOMEDIRECTORY_ATTRIBUTE, Type = typeof(string))]
		public string HomeDirectory
		{
			get
			{
				return (string)this[XML_HOMEDIRECTORY_ATTRIBUTE];
			}
			set
			{
				this[XML_HOMEDIRECTORY_ATTRIBUTE] = value;
			}
		}

		/// <summary>
		/// 获取或设置<see cref="FtpServer"/>服务器的用户是否只读，默认为只读(true)。
		/// </summary>
		[OptionConfigurationProperty(XML_READONLY_ATTRIBUTE, Type = typeof(bool), DefaultValue = true)]
		public bool ReadOnly
		{
			get
			{
				return (bool)this[XML_READONLY_ATTRIBUTE];
			}
			set
			{
				this[XML_READONLY_ATTRIBUTE] = value;
			}
		}
		#endregion
	}
}

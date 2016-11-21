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

using Zongsoft.Services;
using Zongsoft.Services.Composition;

namespace Zongsoft.Communication.Net.Ftp
{
    /// <summary>
    /// 登录用户名
    /// </summary>
    internal class FtpUserCommand : FtpCommand
    {
        public FtpUserCommand() : base("USER")
        {
        }

		protected override object OnExecute(FtpCommandContext context)
		{
			string result;
			var user = context.Server.Configuration.Users[context.Statement.Argument];

			if(user != null)
			{
				context.Channel.UserName = context.Statement.Argument;

				if(string.IsNullOrEmpty(user.Password))
				{
					context.Channel.User = user;
					result = "230 User successfully logged in.";
					context.Statement.Result = true;
				}
				else
				{
					result = "331 Password required for " + context.Statement.Argument;
					context.Statement.Result = false;
				}
			}
			else
			{
				result = "331 Password required for " + context.Statement.Argument;
				context.Statement.Result = false;
			}

			context.Channel.Send(result);

			return result;
		}
    }
}
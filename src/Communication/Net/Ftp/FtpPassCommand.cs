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

namespace Zongsoft.Communication.Net.Ftp
{
    /// <summary>
    /// 登录密码
    /// </summary>
    internal class FtpPassCommand : FtpCommand
    {
        public FtpPassCommand() : base("PASS")
        {
        }

		protected override void OnExecute(FtpCommandContext context)
        {
			var user = context.Server.Configuration.Users[context.Channel.UserName];

            if (user != null)
            {
                if (string.Equals(context.Statement.Argument, user.Password, StringComparison.OrdinalIgnoreCase))
                {
                    context.Channel.User = user;
                    context.Channel.Send("230 User successfully logged in.", null);
                    context.Statement.Result = true;
                    return;
                }
            }

            context.Channel.Send("530 Not logged in, user or password incorrect!", null);
            context.Statement.Result = false;
        }
    }
}
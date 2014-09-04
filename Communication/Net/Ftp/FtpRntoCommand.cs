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
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zongsoft.Communication.Net.Ftp
{
    /// <summary>
    /// 对上一次的“重命名(RNFR)”指令提到的文件重命名
    /// </summary>
    internal class FtpRntoCommand : FtpCommand
    {
        public FtpRntoCommand() : base("RNTO")
        {
        }

        protected override void Run(FtpCommandContext context)
        {
            context.Channel.CheckLogin();

            try
            {
                if (string.IsNullOrEmpty(context.Statement.Argument))
                    throw new SyntaxException();

                if (string.IsNullOrEmpty(context.Channel.RenamePath))
                    throw new BadSeqCommandsException();

                string destPath = context.Channel.MapVirtualPathToLocalPath(context.Statement.Argument);
                context.Statement.Result = destPath;

                try
                {
                    if (Directory.Exists(context.Channel.RenamePath))
                    {
                        Directory.Move(context.Channel.RenamePath, destPath);
                    }
                    else
                    {
                        File.Move(context.Channel.RenamePath, destPath);
                    }
                }
                catch (Exception)
                {
                    throw new InternalException("rename path");
                }

                context.Channel.Send("250 Rename successful.");
            }
            finally
            {
                context.Channel.RenamePath = null;
            }
        }
    }
}
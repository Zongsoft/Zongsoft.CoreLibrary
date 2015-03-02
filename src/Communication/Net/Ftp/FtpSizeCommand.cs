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
    /// 查询当前文件的大小
    /// </summary>
    internal class FtpSizeCommand : FtpCommand
    {
        public FtpSizeCommand() : base("SIZE")
        {
        }

        protected override void Run(FtpCommandContext context)
        {
            context.Channel.CheckLogin();

            if (string.IsNullOrEmpty(context.Statement.Argument))
                throw new SyntaxException();

            var file = context.Statement.Argument;
            if (string.IsNullOrEmpty(file))
                throw new SyntaxException();

            string localPath = context.Channel.MapVirtualPathToLocalPath(file);
            try
            {
                long length = 0;
                var info = new FileInfo(localPath);
                if (info.Exists)
                    length = info.Length;

                context.Channel.Send(string.Concat("213 ", length.ToString()));
            }
            catch (FtpException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new InternalException(e.Message);
            }
        }
    }
}
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
    /// 在命令行上列出对象的信息
    /// </summary>
    internal class FtpMlstCommand : FtpCommand
    {
        public FtpMlstCommand() : base("MLST")
        {
        }

        protected override void Run(FtpCommandContext context)
        {
            context.Channel.CheckLogin();

            try
            {
                var path = string.Empty;
                if (!string.IsNullOrWhiteSpace(context.Statement.Argument))
                    path = context.Statement.Argument;

                if (string.IsNullOrWhiteSpace(path))
                    path = context.Channel.CurrentDir;

                var localPath = context.Channel.MapVirtualPathToLocalPath(path);
                context.Statement.Result = localPath;

                FileSystemInfo fileInfo;

                if (File.Exists(localPath))
                    fileInfo = new FileInfo(localPath);
                else if (Directory.Exists(localPath))
                    fileInfo = new DirectoryInfo(localPath);
                else
                    throw new FileNotFoundException(path);
                
                var str = string.Format("250-Listing {0}\r\n{1}250 END", path, FtpMlstFileFormater.Format(context, fileInfo));
                context.Channel.Send(str);
            }
            catch (IOException e)
            {
                throw new InternalException(e.Message);
            }
        }
    }
}

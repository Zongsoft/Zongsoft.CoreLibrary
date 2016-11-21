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
    /// 列出文件列表
    /// </summary>
    internal class FtpMlsdCommand : FtpCommand
    {
        public FtpMlsdCommand() : base("MLSD")
        {
        }

		protected override object OnExecute(FtpCommandContext context)
        {
            context.Channel.CheckLogin();

            context.Channel.CheckDataChannel();
            context.Channel.DataChannel.Receive();

            try
            {
                context.Channel.Status = FtpSessionStatus.List;

                var path = string.Empty;
                if (!string.IsNullOrWhiteSpace(context.Statement.Argument))
                    path = context.Statement.Argument;

                if (string.IsNullOrWhiteSpace(path))
                    path = context.Channel.CurrentDir;

                var localPath = context.Channel.MapVirtualPathToLocalPath(path);
                context.Statement.Result = localPath;
            
                if (File.Exists(localPath))
                {
                    context.Channel.Send("150 Opening data connection for directory list.");

                    var file = new FileInfo(localPath);
                    WriteFileInfo(context, file);
                }
                else if (Directory.Exists(localPath))
                {
                    context.Channel.Send("150 Opening data connection for directory list.");

                    var localDir = new DirectoryInfo(localPath);

                    //列举目录
                    foreach (var dir in localDir.GetDirectories())
                    {
                        WriteFileInfo(context, dir);
                    }

                    //列举文件
                    foreach (FileInfo file in localDir.GetFiles())
                    {
                        WriteFileInfo(context, file);
                    }
                }
                else
                {
                    throw new DirectoryNotFoundException(path);
                }

                context.Channel.Send("226 Transfer completed.");
				return "226 Transfer completed.";
            }
            catch (IOException e)
            {
                throw new InternalException(e.Message);
            }
            finally
            {
                context.Channel.CloseDataChannel();
                context.Channel.Status = FtpSessionStatus.Wait;
            }
        }

        private void WriteFileInfo(FtpCommandContext context, FileSystemInfo fileInfo)
        {
            var result = FtpMlstFileFormater.Format(context, fileInfo);

            var data = context.Channel.Encoding.GetBytes(result);
            context.Channel.DataChannel.Send(data, 0, data.Length);
        }
    }
}

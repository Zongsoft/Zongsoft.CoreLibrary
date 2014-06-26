#region File Header

// Authors:
//    钟峰(Popeye Zhong) <zongsoft@gmail.com>
//    邓祥云(X.Z. Deng) <627825056@qq.com>
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

#endregion

#region Using

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

#endregion

namespace Zongsoft.Communication.Net.Ftp
{
    /// <summary>
    /// 从服务器上复制（下载）文件
    /// </summary>
    internal class FtpRetrCommand : FtpCommand
    {
        public FtpRetrCommand() : base("RETR")
        {
        }

        protected override void Run(FtpCommandContext context)
        {
            context.Channel.CheckLogin();

            if (string.IsNullOrEmpty(context.Statement.Argument))
                throw new SyntaxException();

            context.Channel.CheckDataChannel();

            try
            {
                //context.Channel.Status = FtpSessionStatus.Download;

                var path = context.Statement.Argument;
                string localPath = context.Channel.MapVirtualPathToLocalPath(path);
                context.Statement.Result = localPath;

                var fileInfo = new FileInfo(localPath);
                if (fileInfo.Exists)
                {
                    context.Channel.Send("150 Open data connection for file transfer.");

                    if (context.Channel.DataChannel.SendFile(fileInfo, context.Channel.FileOffset))
                        context.Channel.Send("226 Transfer complete.");
                    else
                        context.Channel.Send("426 Connection closed; transfer aborted.");

                    context.Channel.FileOffset = 0;
                }
                else
                {
                    throw new FileNotFoundException(path);
                }
            }
            catch (FtpException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new InternalException(e.Message);
            }
            finally
            {
                context.Channel.CloseDataChannel();
                //context.Channel.Status = FtpSessionStatus.Wait;
            }
        }
    }
}
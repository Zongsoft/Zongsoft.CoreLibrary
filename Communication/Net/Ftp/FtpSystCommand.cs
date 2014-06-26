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
using System.Linq;
using System.Text;

#endregion

namespace Zongsoft.Communication.Net.Ftp
{
    /// <summary>
    /// 返回服务器使用的操作系统，指令的回应是该系统当前版本名的第一个单词
    /// </summary>
    internal class FtpSystCommand : FtpCommand
    {
        public FtpSystCommand() : base("SYST")
        {
        }

        protected override void Run(FtpCommandContext context)
        {
            context.Channel.CheckLogin();

            context.Channel.Send("215 UNIX emulated by Zongsoft.FtpServer");
        }
    }
}
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
using Zongsoft.Services;

#endregion

namespace Zongsoft.Communication.Net.Ftp
{
    internal class FtpCommandLoader : ICommandLoader
    {
        public void Load(CommandTreeNode node)
        {
            node.Children.Add(new FtpAborCommand());
            node.Children.Add(new FtpAlloCommand());
            node.Children.Add(new FtpAppeCommand());
            node.Children.Add(new FtpCdupCommand());
            node.Children.Add(new FtpCwdCommand());
            node.Children.Add(new FtpDeleCommand());
            node.Children.Add(new FtpFeatCommand());
            node.Children.Add(new FtpHelpCommand());
            node.Children.Add(new FtpListCommand());
            node.Children.Add(new FtpMdtmCommand());
            node.Children.Add(new FtpMfmtCommand());
            node.Children.Add(new FtpMkdCommand());
            node.Children.Add(new FtpMlsdCommand());
            node.Children.Add(new FtpMlstCommand());
            node.Children.Add(new FtpNoopCommand());
            node.Children.Add(new FtpOptsCommand());
            node.Children.Add(new FtpPassCommand());
            node.Children.Add(new FtpPasvCommand());
            node.Children.Add(new FtpPortCommand());
            node.Children.Add(new FtpPwdCommand());
            node.Children.Add(new FtpQuitCommand());
            node.Children.Add(new FtpRestCommand());
            node.Children.Add(new FtpRetrCommand());
            node.Children.Add(new FtpRmdCommand());
            node.Children.Add(new FtpRnfrCommand());
            node.Children.Add(new FtpRntoCommand());
            node.Children.Add(new FtpSizeCommand());
            node.Children.Add(new FtpStorCommand());
            node.Children.Add(new FtpSystCommand());
            node.Children.Add(new FtpTypeCommand());
            node.Children.Add(new FtpUserCommand());
        }
    }
}
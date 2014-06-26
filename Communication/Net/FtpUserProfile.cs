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

namespace Zongsoft.Communication.Net
{
    public class FtpUserProfile
    {
        /// <summary>
        /// 每个IP的最大连接数
        /// </summary>
        public int ConnectionLimitPerIP { get; set; }

        /// <summary>
        /// 限制上传的最大文件长度
        /// (超过这个值，数据连接会被关闭,传输失败)
        /// </summary>
        public long MaxUploadFileLength { get; set; }

        /// <summary>
        /// 使用该帐号的连接最大连接数
        /// </summary>
        public int MaxConnectionCount { get; set; }

        /// <summary>
        /// 用户使用的本地目录(绝对路径)
        /// </summary>
        public string HomeDir { get; set; }

        /// <summary>
        /// 是否允许读(包括主要是FTP的List和Download操作)
        /// </summary>
        public bool AllowRead { get; set; }

        /// <summary>
        /// 是否允许写（主要涉及FTP中的Delete，Upload和Create操作)
        /// </summary>
        public bool AllowWrite { get; set; }

        /// <summary>
        /// 用户名称
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 用户密码
        /// </summary>
        public string Password { get; set; }
    }
}
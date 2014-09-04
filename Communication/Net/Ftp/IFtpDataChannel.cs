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

namespace Zongsoft.Communication.Net.Ftp
{
	#region 委托定义
	internal delegate void ReceiveDataEventHandler(object sender, byte[] buffer, int offset, int count);
	internal delegate void ErrorEventHandler(object sender, Exception exception);
	#endregion

	/// <summary>
    /// 定义FTP数据通道接口
    /// </summary>
    internal interface IFtpDataChannel
    {
        /// <summary>
        /// 通道异常
        /// </summary>
        event ErrorEventHandler Error;

        /// <summary>
        /// 通道连接完成
        /// </summary>
        event EventHandler Connected;

        /// <summary>
        /// 通道关闭
        /// </summary>
        event EventHandler Closed;

        /// <summary>
        /// 通道接收数据
        /// </summary>
        event ReceiveDataEventHandler Received;

        /// <summary>
        /// 通道是否以连接
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        /// 获取Socket对像
        /// </summary>
        System.Net.Sockets.Socket Socket { get; }

        /// <summary>
        /// 获取或设置 数据通道对应的命令通道对像
        /// </summary>
        FtpServerChannel ServerChannel { get; set; }


        /// <summary>
        /// 发送数据包
        /// </summary>
        /// <param name="buffer">数据缓冲区</param>
        /// <param name="offset">起始位置</param>
        /// <param name="count">发送长度</param>
        void Send(byte[] buffer, int offset, int count);

        /// <summary>
        /// 发送文件
        /// </summary>
        /// <param name="fileInfo">文件信息</param>
        /// <param name="offset">起始位置</param>
        /// <returns>是否发送完成</returns>
        bool SendFile(FileInfo fileInfo, long offset);

        /// <summary>
        /// 开始接收数据
        /// </summary>
        void Receive();

        /// <summary>
        /// 关闭数据通道
        /// </summary>
        void Close();
    }
}
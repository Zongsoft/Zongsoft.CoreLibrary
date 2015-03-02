/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
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
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Text;

using Zongsoft.Diagnostics;
using Zongsoft.ComponentModel;

namespace Zongsoft.Communication.Net
{
	public class TcpServerChannel : TcpChannel
	{
		#region 成员字段
		private DateTime _acceptedTime;
		private EndPoint _remoteEndPoint;
		private TcpServerChannelManager _channelManager;
		#endregion

		#region 构造函数
		internal protected TcpServerChannel(TcpServerChannelManager channelManager, int channelId) : base(channelId)
		{
			if(channelManager == null)
				throw new ArgumentNullException("channelManager");

			//设置通道管理器
			_channelManager = channelManager;

			//初始化接受连接的时间
			_acceptedTime = new DateTime(1900, 1, 1);
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取当前通道接受远程连接成功的时间。
		/// </summary>
		public DateTime AcceptedTime
		{
			get
			{
				return _acceptedTime;
			}
		}

		/// <summary>
		/// 获取当前通道所属的<see cref="TcpServerChannelManager"/>管理器对象。
		/// </summary>
		public TcpServerChannelManager ChannelManager
		{
			get
			{
				return _channelManager;
			}
		}

		/// <summary>
		/// 获取当前通道连接的远程网络端点。
		/// </summary>
		public override EndPoint RemoteEndPoint
		{
			get
			{
				if(_remoteEndPoint != null)
					return _remoteEndPoint;

				//返回基类同名属性值
				return base.RemoteEndPoint;
			}
		}
		#endregion

		#region 接受处理
		internal void OnAccept(SocketAsyncEventArgs asyncArgs, System.Action onAccepted)
		{
			if(asyncArgs.SocketError != SocketError.Success)
				return;

			//设置当前通道的Socket对象
			this.Socket = asyncArgs.AcceptSocket;

			//设置当前通道的接受时间
			_acceptedTime = DateTime.Now;

			//设置远程连接端的地址信息
			_remoteEndPoint = asyncArgs.RemoteEndPoint;

			//触发接受完成的委托回调
			if(onAccepted != null)
				onAccepted();

			//如果当前受理阶段中含有数据则执行数据收取操作
			if(asyncArgs.BytesTransferred > 0)
				this.OnReceive(asyncArgs);

			this.OnAccepted(asyncArgs);
		}

		protected virtual void OnAccepted(SocketAsyncEventArgs asyncArgs)
		{
			//启动异步接收数据
			this.Receive();
		}
		#endregion
	}
}

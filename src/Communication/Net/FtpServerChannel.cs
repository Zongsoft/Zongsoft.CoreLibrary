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
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

using Zongsoft.Diagnostics;
using Zongsoft.Communication.Net.Ftp;

namespace Zongsoft.Communication.Net
{
	public class FtpServerChannel : TcpServerChannel
	{
		#region 成员字段
		private readonly string _localPathSplit = Path.DirectorySeparatorChar.ToString();
		private const string _virtualPathSplit = "/";
		private const string Root = "/";

		private Encoding _encoding;
		private ManualResetEventSlim _waitDataChannel;
		private FtpPasvDataChannel _pasvDataChannel;
		#endregion

		#region 构造函数
		internal FtpServerChannel(FtpServerChannelManager channelManager, int channelId)
			: base(channelManager, channelId)
		{
			base.ReceivingBufferSize = 1024;
			_waitDataChannel = new ManualResetEventSlim(false);
			Packetizer = new FtpPacketizer();
			_pasvDataChannel = new Ftp.FtpPasvDataChannel();
		}
		#endregion

		#region 重载方法
		public override void Send(string text, Encoding encoding, object asyncState = null)
		{
			Console.WriteLine("[{0:000}]:{1}", this.ChannelId, text);
			base.Send(text + "\r\n", this.Encoding, asyncState);
		}

		protected override void OnReceived(ReceivedEventArgs args)
		{
			CurrentStatement = args.ReceivedObject as FtpStatement;

			Console.WriteLine("[{0:000}]:{1} {2}", this.ChannelId, CurrentStatement.Name, CurrentStatement.Argument);

			base.OnReceived(args);
		}

		protected override void OnAccepted(SocketAsyncEventArgs asyncArgs)
		{
			RestFtpSession();

			Send("220 FTP Server V1.0 ready.");

			base.OnAccepted(asyncArgs);
		}

		protected override void OnClosed(ChannelEventArgs args)
		{
			CloseDataChannel();

			if(_pasvDataChannel != null)
				_pasvDataChannel.StopListener();

			var stream = UpFileStream;
			UpFileStream = null;

			if(stream != null)
				stream.Dispose();

			base.OnClosed(args);
		}
		#endregion

		#region 成员属性
		/// <summary>
		/// 数据包解析器
		/// </summary>
		public new FtpPacketizer Packetizer
		{
			get
			{
				return base.Packetizer as FtpPacketizer;
			}
			set
			{
				base.Packetizer = value;
			}
		}

		/// <summary>
		/// FtpServer
		/// </summary>
		public FtpServer Server
		{
			get
			{
				return ((FtpServerChannelManager)ChannelManager).Server;
			}
		}

		/// <summary>
		/// 当前字符编码
		/// </summary>
		public Encoding Encoding
		{
			get
			{
				if(_encoding == null)
					return Server.Configuration.Encoding;

				return _encoding;
			}
			set
			{
				_encoding = value;
				Packetizer.Encoding = Encoding;
			}
		}

		/// <summary>
		/// 会话状态
		/// </summary>
		internal FtpSessionStatus Status
		{
			get;
			set;
		}

		/// <summary>
		/// 数据传输模式
		/// </summary>
		internal FtpTransferMode TransferMode
		{
			get;
			set;
		}

		/// <summary>
		/// 登录用户信息
		/// </summary>
		public Configuration.FtpUserOptionElement User
		{
			get;
			set;
		}

		/// <summary>
		/// 登录用户名称s
		/// </summary>
		public string UserName
		{
			get;
			set;
		}

		/// <summary>
		/// 当前目录
		/// </summary>
		public string CurrentDir
		{
			get;
			set;
		}

		/// <summary>
		/// 跟目录地址
		/// </summary>
		public string RootPath
		{
			get
			{
				return Root;
			}
		}

		/// <summary>
		/// 本地目录分割符
		/// </summary>
		public string LocalPathSplit
		{
			get
			{
				return _virtualPathSplit;
			}
		}

		/// <summary>
		/// 虚拟目录分割符
		/// </summary>
		public string VirtualPathSplit
		{
			get
			{
				return _virtualPathSplit;
			}
		}

		/// <summary>
		/// 要重命名的本地路径
		/// </summary>
		public string RenamePath
		{
			get;
			set;
		}

		/// <summary>
		/// 数据连接通道
		/// </summary>
		internal IFtpDataChannel DataChannel
		{
			get;
			set;
		}

		/// <summary>
		/// 上传文件时写入的流
		/// </summary>
		public Stream UpFileStream
		{
			get;
			set;
		}

		/// <summary>
		/// 上传文件的本地路径
		/// </summary>
		public string UpFileLocalPath
		{
			get;
			set;
		}

		/// <summary>
		/// 是否上传失败
		/// </summary>
		public bool UpFileFailed
		{
			get;
			set;
		}

		/// <summary>
		/// 重传文件的偏移量
		/// </summary>
		public long FileOffset
		{
			get;
			set;
		}

		/// <summary>
		/// 当前执行的Ftp命令
		/// </summary>
		public FtpStatement CurrentStatement
		{
			get;
			set;
		}
		#endregion

		#region Ftp会话方法
		public void RestFtpSession()
		{
			Packetizer.Reset();
			Encoding = Server.Configuration.Encoding;
			TransferMode = FtpTransferMode.Ascii;
			Status = FtpSessionStatus.NotLogin;
			User = null;
			CurrentStatement = null;
			CurrentDir = RootPath;
			FileOffset = 0;
			UpFileFailed = false;
			UpFileLocalPath = null;

			var stream = UpFileStream;
			UpFileStream = null;

			if(stream != null)
				stream.Close();
		}

		/// <summary>
		/// 虚拟路径映射到本地路径
		/// </summary>
		public string MapVirtualPathToLocalPath(string virtualPath)
		{
			if(!virtualPath.StartsWith(_virtualPathSplit))
				virtualPath = Path.Combine(CurrentDir, virtualPath);

			var locapPath = virtualPath.Replace(_virtualPathSplit, _localPathSplit);
			locapPath = locapPath.Substring(_localPathSplit.Length);
			locapPath = Path.Combine(User.HomeDirectory, locapPath);

			return locapPath;
		}

		/// <summary>
		/// 本地路径映射到虚拟路径
		/// </summary>
		public string MapLocalPathToVirtualPath(string localPath)
		{
			var virtualPath = localPath.Replace(this.User.HomeDirectory, string.Empty).Replace(_localPathSplit, _virtualPathSplit);

			if(!virtualPath.StartsWith(_virtualPathSplit))
				virtualPath = _virtualPathSplit + virtualPath;

			return virtualPath;
		}

		/// <summary>
		/// 检查用户是已登录，未登录则抛出异常 <see cref="NotLoginException"/>
		/// <exception cref="NotLoginException">用户未登录</exception>
		/// </summary>
		public void CheckLogin()
		{
			if(User == null)
				throw new NotLoginException();
		}

		/// <summary>
		/// 创建被动数据通道，并发送端口号到客户端
		/// </summary>
		public void CreatePasvDataChannel()
		{
			CloseDataChannel();

			try
			{
				var dataChannel = _pasvDataChannel;
				//var dataChannel = Server.PasvDataChannelPool.GetObject();
				dataChannel.StartListener();

				if(dataChannel.IsConnected)
					dataChannel.Close();

				InitDataChannel(dataChannel);

				var port = dataChannel.ListenPoint.Port;
				var ipAddress = ((IPEndPoint)Socket.LocalEndPoint).Address.ToString().Replace(".", ",");
				var reply = string.Format("227 Entering Passive Mode({0},{1},{2}).", ipAddress, port / 256, port % 256);

				Send(reply);
			}
			catch(Exception ex)
			{
				this.CloseDataChannel();
				throw new FtpException("502 Open the listener failure", ex);
			}
		}


		/// <summary>
		/// 创建主动数据通道，并连接到客户端
		/// </summary>
		/// <param name="address"></param>
		public void CreatePortDataChannel(IPEndPoint address)
		{
			CloseDataChannel();

			try
			{
				var dataChannel = new FtpPortDataChannel(address);
				InitDataChannel(dataChannel);

				dataChannel.Connect();

				var reply = string.Format("200 Entering Active Mode({0},{1},{2}).", address.Address, address.Port / 256, address.Port % 256);

				Send(reply);
			}
			catch(Exception)
			{
				CloseDataChannel();
				throw new FtpException("530 Connect Port failure");
			}
		}

		/// <summary>
		/// 设置DataChannel
		/// </summary>
		private void InitDataChannel(IFtpDataChannel dataChannel)
		{
			_waitDataChannel.Reset();

			DataChannel = dataChannel;
			DataChannel.ServerChannel = this;
			DataChannel.Connected += DataChannel_Connected;
		}

		private void DataChannel_Connected(object sender, EventArgs e)
		{
			_waitDataChannel.Set();
		}

		/// <summary>
		/// 检查数据连接是否可用，无数据连接则抛出 <see cref="DataConnNotReadyException"/>
		/// <exception cref="DataConnNotReadyException">无数据连接</exception>
		/// </summary>
		public void CheckDataChannel()
		{
			var channel = DataChannel;
			if(channel == null)
				throw new DataConnNotReadyException();

			//在规定时间内等待客户端的数据连接
			_waitDataChannel.Wait(TimeSpan.FromSeconds(5));

			if(!channel.IsConnected)
				throw new DataConnNotReadyException();
		}

		/// <summary>
		/// 关闭数据连接
		/// </summary>
		public void CloseDataChannel()
		{
			var dataChannel = DataChannel;
			DataChannel = null;

			if(dataChannel == null)
				return;

			dataChannel.Close();
			_waitDataChannel.Reset();
			dataChannel.Connected -= DataChannel_Connected;

			//var serverChannel = dataChannel as FtpPasvDataChannel;
			//if (serverChannel != null)
			//    Server.PasvDataChannelPool.Release(serverChannel);

			var portDataChannel = dataChannel as FtpPortDataChannel;
			if(portDataChannel != null)
				portDataChannel.Dispose();
		}
		#endregion
	}
}
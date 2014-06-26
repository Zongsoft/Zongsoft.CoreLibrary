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
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Zongsoft.Runtime.Caching;

namespace Zongsoft.Communication.Net
{
	public class TcpServer : ListenerBase
	{
		#region 事件定义
		public event EventHandler<ChannelEventArgs> Accepted;
		#endregion

		#region 私有方法
		private SocketAsyncEventArgs _acceptAsyncArgs;
		#endregion

		#region 成员变量
		private Socket _socket;
		private TcpServerChannelManager _channelManager;
		private Zongsoft.Runtime.Caching.IBufferManagerSelector _bufferSelector;
		#endregion

		#region 构造函数
		public TcpServer() : this(new IPEndPoint(0, 7969))
		{
		}

		public TcpServer(int port) : this(new IPEndPoint(0, port))
		{
		}

		public TcpServer([TypeConverter(typeof(IPEndPointConverter))]IPEndPoint localEP) : base("TcpServer", localEP)
		{
			//创建接受异步事件参数
			_acceptAsyncArgs = new SocketAsyncEventArgs();

			//挂载接受异步事件参数的Completed事件处理委托
			_acceptAsyncArgs.Completed += ((sender, asyncArgs) =>
			{
				if(asyncArgs.LastOperation == SocketAsyncOperation.Accept)
				{
					this.OnAcceptCompleted(asyncArgs);
				}
			});

			//创建当前服务器通道管理器
			_channelManager = this.CreateChannelManager();
		}
		#endregion

		#region 公共属性
		public TcpServerChannelManager ChannelManager
		{
			get
			{
				if(_channelManager == null)
					throw new ObjectDisposedException(string.Format("{0}[{1}]", this.Name, this.GetType().FullName));

				return _channelManager;
			}
		}

		public IBufferManagerSelector BufferSelector
		{
			get
			{
				if(_bufferSelector == null)
					System.Threading.Interlocked.CompareExchange(ref _bufferSelector, new BufferManagerSelector(), null);

				return _bufferSelector;
			}
			set
			{
				if(value == null)
					throw new ArgumentNullException();

				_bufferSelector = value;
			}
		}
		#endregion

		#region 虚拟方法
		protected virtual TcpServerChannelManager CreateChannelManager()
		{
			return new TcpServerChannelManager(this,
				   new PacketizerFactory<TcpPacketizer>(
							(channel) =>
							{
								return new TcpPacketizer(this.BufferSelector);
							}
					   ));
		}
		#endregion

		#region 启动关闭
		protected override void OnStart(string[] args)
		{
			if(_channelManager == null)
				throw new ObjectDisposedException(string.Format("{0}[{1}]", this.Name, this.GetType().FullName));

			int backlog = 10000;

			if(args != null && args.Length > 0)
			{
				if(int.TryParse(args[0], out backlog))
					backlog = Math.Max(backlog, 10);
			}

			//确认Socket已经存在
			this.EnsureListener();

			//绑定侦听地址和端口
			_socket.Bind(this.Address);

			//开始Socket网络侦听
			_socket.Listen(backlog);

			//异步开始接受网络连接
			this.Accept();
		}

		protected override void OnStop()
		{
			//原子性的将当前服务器的侦听Socket对象置空，并返回其原值引用
			var socket = System.Threading.Interlocked.Exchange(ref _socket, null);

			//如果原侦听Socket对象为空则表示已经关闭
			if(socket == null)
				return;

			//关闭当前Socket对象
			socket.Close();

			//获取当前服务器的通道管理器
			var channelManager = _channelManager;

			//关闭通道管理器由其关闭所有活动连接
			if(channelManager != null)
				channelManager.Close();
		}
		#endregion

		#region 接受连接
		private void Accept()
		{
			var socket = _socket;

			if(socket == null)
				return;

			//必须将异步事件参数的接受Socket对象置空
			_acceptAsyncArgs.AcceptSocket = null;

			try
			{
				if(!socket.AcceptAsync(_acceptAsyncArgs))
					this.OnAcceptCompleted(_acceptAsyncArgs);
			}
			catch(ObjectDisposedException)
			{
				/*
				 * 注意：该异常表示本服务器已被关闭，在此勿需重抛异常也勿虚激发失败事件。
				 */
				return;
			}
		}
		#endregion

		#region 接受回调
		private void OnAcceptCompleted(SocketAsyncEventArgs asyncArgs)
		{
			if(_channelManager == null)
				return;

			//以下状态表示服务被关闭或Socket被终止
			if(asyncArgs.SocketError == SocketError.OperationAborted ||
			   asyncArgs.SocketError == SocketError.Interrupted ||
			   asyncArgs.SocketError == SocketError.NotSocket)
				return;

			//判断通讯层是否有异常，如果成功是则接受本次连接
			if(asyncArgs.SocketError == SocketError.Success)
			{
				//获取一个可用的通道对象
				var channel = _channelManager.GetChannel();

				channel.Closed += Channel_Closed;
				channel.Failed += Channel_Failed;
				channel.Received += Channel_Received;

				//通知当前通道受理本次连接
				channel.OnAccept(asyncArgs, () =>
				{
					//激发“Accepted”事件
					this.OnAccepted(new ChannelEventArgs(channel));
				});
			}

			//继续等待其他连接
			this.Accept();
		}
		#endregion

		#region 激发事件
		protected virtual void OnAccepted(ChannelEventArgs args)
		{
			if(this.Accepted != null)
				this.Accepted(this, args);
		}
		#endregion

		#region 事件处理
		private void Channel_Received(object sender, ReceivedEventArgs e)
		{
			this.OnReceived(e);
		}

		private void Channel_Failed(object sender, ChannelFailureEventArgs e)
		{
			this.OnFailed(e);
		}

		private void Channel_Closed(object sender, ChannelEventArgs e)
		{
			var channel = (TcpServerChannel)e.Channel;

			channel.Closed -= Channel_Closed;
			channel.Failed -= Channel_Failed;
			channel.Received -= Channel_Received;
		}
		#endregion

		#region 释放资源
		protected override void Dispose(bool disposing)
		{
			//停止当前服务
			this.Stop();

			if(disposing)
			{
				var channelManager = _channelManager;

				if(channelManager != null)
					channelManager.Dispose();
			}

			//将通道管理器置空表示当前类已被处置
			_channelManager = null;

			//调用基类同名方法
			base.Dispose(disposing);
		}
		#endregion

		#region 私有方法
		private void EnsureListener()
		{
			if(_socket == null)
			{
				var socket = System.Threading.Interlocked.CompareExchange(ref _socket, new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp), null);

				if(socket == null)
				{
					_socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, false);
					_socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontLinger, true);
					_socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.NoDelay, true);
				}
			}
		}
		#endregion
	}
}

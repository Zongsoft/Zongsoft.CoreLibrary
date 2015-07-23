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
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;

using Zongsoft.Diagnostics;
using Zongsoft.ComponentModel;

namespace Zongsoft.Communication.Net
{
	public class TcpClientChannel : TcpChannel
	{
		#region 事件声明
		public event EventHandler<ChannelAsyncEventArgs> Connected;
		#endregion

		#region 私有变量
		private SocketAsyncEventArgs _connectAsyncArgs;
		private ManualResetEventSlim _connectWaitHandle;
		#endregion

		#region 成员字段
		private DateTime _lastConnectTime;
		private EndPoint _remoteEndPoint;
		#endregion

		#region 构造函数
		public TcpClientChannel(IPEndPoint remoteEP) : this(remoteEP, 0, null)
		{
		}

		public TcpClientChannel(IPEndPoint remoteEP, IPacketizer packetizer) : this(remoteEP, 0, packetizer)
		{
		}

		public TcpClientChannel(IPEndPoint remoteEP, int channelId) : this(remoteEP, channelId, null)
		{
		}

		public TcpClientChannel(IPEndPoint remoteEP, int channelId, IPacketizer packetizer) : base(channelId)
		{
			if(remoteEP == null)
				throw new ArgumentNullException("remoteEP");

			//设置远程网络端点
			_remoteEndPoint = remoteEP;

			//设置协议解析器
			this.Packetizer = packetizer;

			//初始化最后连接时间
			_lastConnectTime = new DateTime(1900, 1, 1);

			//创建连接信号量对象
			_connectWaitHandle = new ManualResetEventSlim(true);
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取最后一次进行连接操作的时间，而无论连接是否成功。
		/// </summary>
		public DateTime LastConnectTime
		{
			get
			{
				return _lastConnectTime;
			}
		}

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

		#region 连接判断
		/// <summary>
		/// 判断当前通道是否为处于已经连接成功的状态。
		/// </summary>
		/// <param name="microSeconds">确定连接状态的检测时长，单位为微秒。</param>
		/// <returns>返回是否处于已经连接成功的状态。</returns>
		/// <remarks>
		///		<para>该方法始终不抛出任何异常，重写该方法的实现者亦应遵守该原则。</para>
		/// </remarks>
		public virtual bool IsConnected(int microSeconds)
		{
			var socket = this.Socket;

			try
			{
				return socket != null &&
					   socket.Connected &&
					   socket.Poll(microSeconds, SelectMode.SelectWrite);
			}
			catch
			{
				return false;
			}
		}
		#endregion

		#region 连接方法
		/// <summary>
		/// 以同步方式连接到由<see cref="RemoteEndPoint"/>属性指定的远程网络端点。
		/// </summary>
		/// <returns>返回是否连接成功，返回真(true)表示连接成功否则连接失败。</returns>
		/// <remarks>
		///		<para>注意：该方法不会激发<see cref="Connected"/>连接完成事件，也不会因为连接失败而激发<seealso cref="ChannelBase.Failed"/>事件。判断该方法是否连接成功请使用方法的返回值。</para>
		/// </remarks>
		public bool Connect()
		{
			//等待其他线程完成连接操作
			_connectWaitHandle.Wait();

			//堵塞其他线程进行发送或者连接操作
			_connectWaitHandle.Reset();

			try
			{
				var socket = this.Socket;

				//如果当前Socket对象为空则创建一个新的连接Socket对象
				if(socket == null)
				{
					socket = this.CreateSocket();

					if(socket == null)
						throw new InvalidOperationException();
				}
				else
				{
					if(this.IsConnected(1000))
						return true;
				}

				try
				{
					//发起同步连接请求
					socket.Connect(_remoteEndPoint);

					//不管是否连接成功，都要更新最后连接时间
					_lastConnectTime = DateTime.Now;

					//获取当前连接是否成功
					var isConnected = socket.Connected;

					if(isConnected)
					{
						//设置当前通道的Socket对象
						this.Socket = socket;

						//启动异步接收数据
						this.Receive();
					}

					//返回是否连接成功
					return isConnected;
				}
				catch
				{
					//不管是否连接成功，都要更新最后连接时间
					_lastConnectTime = DateTime.Now;

					return false;
				}
			}
			finally
			{
				//释放其他线程的等待信号
				_connectWaitHandle.Set();
			}
		}

		/// <summary>
		/// 以异步方式连接到由<see cref="RemoteEndPoint"/>属性指定的远程网络端点。
		/// </summary>
		/// <param name="asyncState">传入的自定义对象，可传递给对应的<see cref="Connected"/>及其他异步事件参数中。</param>
		public void ConnectAsync(object asyncState)
		{
			//等待其他线程完成连接操作
			_connectWaitHandle.Wait();

			try
			{
				//堵塞其他线程进行发送或者连接操作
				_connectWaitHandle.Reset();

				var socket = this.Socket;

				if(socket == null)
				{
					//创建连接Socket对象
					socket = this.CreateSocket();

					//确认创建连接异步事件参数对象
					if(this.EnsureConnectAsyncArgs())
						_connectAsyncArgs.RemoteEndPoint = _remoteEndPoint;
				}
				else
				{
					if(this.IsConnected(1000))
						return;
				}

				//更新异步事件参数对象的相关属性值
				_connectAsyncArgs.UserToken = asyncState;

				try
				{
					if(!socket.ConnectAsync(_connectAsyncArgs))
						this.OnConnectCompleted(_connectAsyncArgs);
				}
				catch(Exception ex)
				{
					//不管是否连接成功，都要更新最后连接时间
					_lastConnectTime = DateTime.Now;

					//激发“Failed”事件
					this.OnFailed(new ChannelFailureEventArgs(this, ex, asyncState));
				}
			}
			catch
			{
				//释放其他线程的等待信号
				_connectWaitHandle.Set();
			}
		}

		/// <summary>
		/// 断开当前通道与远程网络端点的连接。
		/// </summary>
		public void Disconnect()
		{
			var socket = this.Socket;

			if(socket == null)
				return;

			socket.Disconnect(true);
		}
		#endregion

		#region 重写方法
		protected override bool PrepareSend(object asyncState)
		{
			var socket = this.Socket;

			if(socket == null)
			{
				bool connected = false;

				//当前通道的Socket为空则表示从未连接过。
				//如果此时距离上次连接操作的间隔超过2秒，则主动进行网络连接动作；
				//否则不进行网络连接，则由后续代码激发“Failed”事件并返回失败。
				if((DateTime.Now - _lastConnectTime).TotalSeconds > 2)
					connected = this.Connect();

				if(!connected)
				{
					//激发“Failed”事件
					this.OnFailed(new ChannelFailureEventArgs(this, "Connect faild.", asyncState));

					//连接失败则返回失败
					return false;
				}
			}

			//等待其他线程完成连接操作
			_connectWaitHandle.Wait();

			//调用基类的默认预检方法
			return base.PrepareSend(asyncState);
		}
		#endregion

		#region 虚拟方法
		/// <summary>
		/// 创建一个连接<see cref="System.Net.Sockets.Socket"/>对象。
		/// </summary>
		/// <returns>返回创建成功的<seealso cref="System.Net.Sockets.Socket"/>对象。</returns>
		protected virtual Socket CreateSocket()
		{
			var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

			socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, false);
			socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontLinger, true);
			socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.NoDelay, true);

			return socket;
		}
		#endregion

		#region 激发事件
		protected virtual void OnConnected(ChannelAsyncEventArgs args)
		{
			if(this.Connected != null)
				this.Connected(this, args);
		}
		#endregion

		#region 异步回调
		private void OnConnectCompleted(SocketAsyncEventArgs asyncArgs)
		{
			//不管是否连接成功，都要更新最后连接时间
			_lastConnectTime = DateTime.Now;

			if(asyncArgs.SocketError == SocketError.Success)
			{
				/*
				 * 注意：由于Mono中的SocketAsyncEventArgs类中没有ConnectSocket属性，
				 * 故在这里无法处理设置当前通道的Socket属性值，因此由EnsureConnectAsyncArgs()方法中来更新该属性值。
				 */
				//设置当前通道的Socket对象
				//this.Socket = asyncArgs.ConnectSocket;

				//在后台线程中激发“Connected”事件
				Task.Factory.StartNew(() =>
				{
					this.OnConnected(new ChannelAsyncEventArgs(this, asyncArgs.UserToken));
				});

				//启动异步接收数据
				this.Receive();
			}

			//释放其他线程的等待信号
			_connectWaitHandle.Set();

			if(asyncArgs.SocketError != SocketError.Success && asyncArgs.SocketError != SocketError.IsConnected)
				this.OnFailed(new ChannelFailureEventArgs(this, "Connect faild.", asyncArgs.UserToken));
		}
		#endregion

		#region 私有方法
		private bool EnsureConnectAsyncArgs()
		{
			//创建一个异步事件参数对象
			var connectAsyncArgs = Interlocked.CompareExchange(ref _connectAsyncArgs, 
				new SocketAsyncEventArgs()
				{
					DisconnectReuseSocket = true,
				}, null);

			if(connectAsyncArgs == null)
			{
				_connectAsyncArgs.Completed += (sender, asyncArgs) =>
				{
					switch(asyncArgs.LastOperation)
					{
						case SocketAsyncOperation.Connect:
							//注意：由于Mono中的SocketAsyncEventArgs类中没有ConnectSocket属性，故需要特殊处理
							if(asyncArgs.SocketError == SocketError.Success)
								this.Socket = (Socket)sender;

							this.OnConnectCompleted(asyncArgs);
							break;
					}
				};
			}

			return connectAsyncArgs == null;
		}
		#endregion
	}
}

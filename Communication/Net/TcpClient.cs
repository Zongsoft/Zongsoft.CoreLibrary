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
using System.ComponentModel;
using System.Threading;
using System.Text;

using Zongsoft.Services;
using Zongsoft.Services.Composition;
using Zongsoft.Runtime.Caching;

namespace Zongsoft.Communication.Net
{
	public class TcpClient : ISender, IReceiver
	{
		#region 事件声明
		public event EventHandler<ChannelAsyncEventArgs> Connected;
		public event EventHandler<ChannelFailureEventArgs> Failed;
		public event EventHandler<ReceivedEventArgs> Received;
		public event EventHandler<SentEventArgs> Sent;
		#endregion

		#region 成员字段
		private IPEndPoint _remoteEP;
		private TcpClientChannel _channel;
		private Executor _executor;
		private IBufferManagerSelector _bufferSelector;
		#endregion

		#region 构造函数
		public TcpClient() : this(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 7969))
		{
		}

		public TcpClient(string remoteEP)
		{
			if(string.IsNullOrWhiteSpace(remoteEP))
				throw new ArgumentNullException("remoteEP");

			IPEndPointConverter converter = new IPEndPointConverter();
			IPEndPoint endPoint = converter.ConvertFrom(remoteEP) as IPEndPoint;

			if(endPoint == null)
				throw new ArgumentException();

			if(endPoint.Port == 0)
				endPoint.Port = 7969;

			_remoteEP = endPoint;
		}

		public TcpClient([TypeConverter(typeof(IPEndPointConverter))]IPEndPoint remoteEP)
		{
			if(remoteEP == null)
				throw new ArgumentNullException("remoteEP");

			_remoteEP = remoteEP;
		}
		#endregion

		#region 公共属性
		public IBufferManagerSelector BufferSelector
		{
			get
			{
				if(_bufferSelector == null)
					Interlocked.CompareExchange(ref _bufferSelector, new BufferManagerSelector(), null);

				return _bufferSelector;
			}
			set
			{
				if(value == null)
					throw new ArgumentNullException();

				_bufferSelector = value;
			}
		}

		public TcpClientChannel Channel
		{
			get
			{
				if(_channel == null)
				{
					var channel = Interlocked.CompareExchange(
						ref _channel,
						new TcpClientChannel(
							_remoteEP,
							new TcpPacketizer(this.BufferSelector)
						), null);

					if(channel == null)
					{
						_channel.Connected += Channel_Connected;
						_channel.Failed += Channel_Failed;
						_channel.Received += Channel_Received;
						_channel.Sent += Channel_Sent;
					}
				}

				return _channel;
			}
		}

		public EndPoint LocalAddress
		{
			get
			{
				var channel = _channel;

				if(channel == null)
					return null;

				return channel.LocalEndPoint;
			}
		}

		[TypeConverter(typeof(IPEndPointConverter))]
		public IPEndPoint RemoteAddress
		{
			get
			{
				return _remoteEP;
			}
			set
			{
				if(_channel == null)
					_remoteEP = value;

				throw new InvalidOperationException();
			}
		}

		public Executor Executor
		{
			get
			{
				if(_executor == null)
					Interlocked.CompareExchange(ref _executor, new Executor(this), null);

				return _executor;
			}
			set
			{
				if(object.ReferenceEquals(_executor, value))
					return;

				//设置属性的成员字段
				_executor = value;
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
		public bool IsConnected(int microSeconds)
		{
			var channel = _channel;

			if(channel == null)
				return false;

			return channel.IsConnected(microSeconds);
		}
		#endregion

		#region 连接方法
		/// <summary>
		/// 以同步方式连接到由<see cref="RemoteAddress"/>属性指定的远程网络端点。
		/// </summary>
		/// <returns>返回是否连接成功，返回真(true)表示连接成功否则连接失败。</returns>
		/// <remarks>
		///		<para>注意：该方法不会激发<see cref="Connected"/>连接完成事件，也不会因为连接失败而激发<see cref="Failed"/>事件。判断该方法是否连接成功请使用方法的返回值。</para>
		/// </remarks>
		public bool Connect()
		{
			return this.Channel.Connect();
		}

		/// <summary>
		/// 以异步方式连接到由<see cref="RemoteAddress"/>属性指定的远程网络端点。
		/// </summary>
		/// <param name="asyncState">传入的自定义对象，可传递给对应的<see cref="Connected"/>及其他异步事件参数中。</param>
		public void ConnectAsync(object asyncState)
		{
			this.Channel.ConnectAsync(asyncState);
		}

		/// <summary>
		/// 断开当前通道与远程网络端点的连接。
		/// </summary>
		public void Disconnect()
		{
			var channel = _channel;

			if(channel != null)
				channel.Disconnect();
		}
		#endregion

		#region 发送方法
		public void Send(string text, object asyncState = null)
		{
			this.Channel.Send(text, asyncState);
		}

		public void Send(string text, Encoding encoding, object asyncState = null)
		{
			this.Channel.Send(text, encoding, asyncState);
		}

		public void Send(byte[] buffer, object asyncState = null)
		{
			this.Channel.Send(buffer, asyncState);
		}

		public void Send(byte[] buffer, int offset, object asyncState = null)
		{
			this.Channel.Send(buffer, offset, asyncState);
		}

		public void Send(byte[] buffer, int offset, int count, object asyncState = null)
		{
			this.Channel.Send(buffer, offset, count, asyncState);
		}

		public void Send(Stream stream, object asyncState = null)
		{
			this.Channel.Send(stream, asyncState);
		}
		#endregion

		#region 激发事件
		protected virtual void OnConnected(ChannelAsyncEventArgs args)
		{
			if(this.Connected != null)
				this.Connected(this, args);
		}

		protected virtual void OnFailed(ChannelFailureEventArgs args)
		{
			if(this.Failed != null)
				this.Failed(this, args);
		}

		protected virtual void OnReceived(ReceivedEventArgs args)
		{
			//处理接收到的数据
			ReceiverUtility.ProcessReceive(_executor, args);

			if(this.Received != null)
				this.Received(this, args);
		}

		protected virtual void OnSent(SentEventArgs args)
		{
			if(this.Sent != null)
				this.Sent(this, args);
		}
		#endregion

		#region 通道事件
		private void Channel_Connected(object sender, ChannelAsyncEventArgs e)
		{
			this.OnConnected(e);
		}

		private void Channel_Failed(object sender, ChannelFailureEventArgs e)
		{
			this.OnFailed(e);
		}

		private void Channel_Received(object sender, ReceivedEventArgs e)
		{
			this.OnReceived(e);
		}

		private void Channel_Sent(object sender, SentEventArgs e)
		{
			this.OnSent(e);
		}
		#endregion
	}
}

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
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace Zongsoft.Communication.Net
{
	/// <summary>
	/// 定义Tcp通道的基本功能。
	/// </summary>
	public class TcpChannel : ChannelBase
	{
		#region 私有变量
		private long _sequenceId;
		private int _receivingBufferSize;
		private SocketAsyncEventArgs _receivingAsyncArgs;
		private ManualResetEventSlim _sendingBlocking;
		private SocketAsyncEventArgsPool _sendingAsyncArgsPool;
		private ConcurrentDictionary<long, PackingState> _sendingCounter;
		#endregion

		#region 成员变量
		private Socket _socket;
		private IPacketizer _packetizer;
		#endregion

		#region 构造函数
		protected TcpChannel(int channelId) : base(channelId)
		{
			_receivingBufferSize = 32 * 1024;
			_sendingBlocking = new ManualResetEventSlim(true);
			_sendingCounter = new ConcurrentDictionary<long, PackingState>();
			_sendingAsyncArgsPool = new SocketAsyncEventArgsPool(SocketAsyncEventArgs_Completed);
		}
		#endregion

		#region 公共属性
		public override bool IsIdled
		{
			get
			{
				return _socket == null;
			}
		}

		public int ReceivingBufferSize
		{
			get
			{
				return _receivingBufferSize;
			}
			set
			{
				_receivingBufferSize = Math.Max(value, 1024);
			}
		}

		public EndPoint LocalEndPoint
		{
			get
			{
				Socket socket = _socket;

				if(socket != null)
					return socket.LocalEndPoint;

				return null;
			}
		}

		public virtual EndPoint RemoteEndPoint
		{
			get
			{
				Socket socket = _socket;

				if(socket != null)
					return socket.RemoteEndPoint;

				return null;
			}
		}

		/// <summary>
		/// 获取或设置当前通道的通讯协议解析器。
		/// </summary>
		public IPacketizer Packetizer
		{
			get
			{
				return _packetizer;
			}
			set
			{
				_packetizer = value;
			}
		}
		#endregion

		#region 保护属性
		/// <summary>
		/// 获取或设置当前通道接受或连接成功的<seealso cref="System.Net.Sockets.Socket"/>对象。
		/// </summary>
		protected Socket Socket
		{
			get
			{
				return _socket;
			}
			set
			{
				_socket = value;
			}
		}
		#endregion

		#region 准备发送
		protected virtual bool PrepareSend(object asyncState)
		{
			//获取当前通道的通讯协议解析器
			var packetizer = this.Packetizer;

			if(packetizer == null)
				throw new InvalidOperationException("The value of 'Packetizer' property in channel is null.");

			return _socket != null && (!this.IsIdled);
		}
		#endregion

		#region 发送方法
		public override void Send(byte[] buffer, int offset, int count, object asyncState = null)
		{
			if(buffer == null)
				throw new ArgumentNullException("buffer");

			if(offset < 0)
				throw new ArgumentOutOfRangeException("offset");

			if(count < 0)
				throw new ArgumentOutOfRangeException("count");

			if(offset + count > buffer.Length)
				throw new ArgumentException();

			this.SendCore(() =>
			{
				return this.Packetizer.Pack(new Zongsoft.Common.Buffer(buffer, offset, count));
			}, asyncState);
		}

		public override void Send(Stream stream, object asyncState = null)
		{
			if(stream == null)
				throw new ArgumentNullException("stream");

			this.SendCore(() =>
			{
				return this.Packetizer.Pack(stream);
			}, asyncState);
		}

		private void SendCore(Func<IEnumerable<Zongsoft.Common.Buffer>> pack, object asyncState)
		{
			//通知子类准备开始发送数据，如果没有准备好则直接退出。
			//通常子类会重写该方法进行发送前的各项准备工作，譬如自动网络连接等。
			if(!this.PrepareSend(asyncState))
				return;

			//首先本地保存对全局对象的引用，以防止其他线程修改全局变量的引用
			var socket = _socket;

			if(socket == null)
				return;

			//生成一个唯一的流水号
			var sequenceId = Interlocked.Increment(ref _sequenceId);
			var state = new PackingState();

			//将待发送的包加入到本地正在发送的包计数器中
			_sendingCounter[sequenceId] = state;

			foreach(var buffer in pack())
			{
				state.CountOfPack++;

				//从异步事件参数池中取出一个可用的对象出来
				var asyncArgs = _sendingAsyncArgsPool.GetObject();

				//设置要发送的缓存区状态
				asyncArgs.SetBuffer(buffer.Value, buffer.Offset, buffer.Count);

				//设置用户自定义对象
				asyncArgs.UserToken = new SendingToken(sequenceId, asyncState);

				try
				{
					//等待TCP协议栈缓存是否可用
					_sendingBlocking.Wait();

					if(!socket.SendAsync(asyncArgs))
						this.OnSendCompleted(asyncArgs);
				}
				catch(ObjectDisposedException)
				{
					this.RaiseSendFailed((SendingToken)asyncArgs.UserToken);
					return;
				}
				catch(SocketException)
				{
					this.RaiseSendFailed((SendingToken)asyncArgs.UserToken);
					return;
				}
			}

			//设置当前发送状态为打包完成
			state.IsPacked = true;

			if(this.EnsureSent(sequenceId))
				this.OnSent(new SentEventArgs(this, asyncState));
		}
		#endregion

		#region 异步事件
		private void SocketAsyncEventArgs_Completed(object sender, SocketAsyncEventArgs asyncArgs)
		{
			switch(asyncArgs.LastOperation)
			{
				case SocketAsyncOperation.Receive:
					this.OnReceiveCompleted(asyncArgs);
					break;
				case SocketAsyncOperation.Send:
					this.OnSendCompleted(asyncArgs);
					break;
			}
		}
		#endregion

		#region 网络处理
		private void OnReceiveCompleted(SocketAsyncEventArgs asyncArgs)
		{
			if(this.CheckUsable(asyncArgs, null))
			{
				//处理数据接收
				this.OnReceive(asyncArgs);

				//继续异步接收后续数据
				this.Receive();
			}
		}

		private void OnSendCompleted(SocketAsyncEventArgs asyncArgs)
		{
			if(asyncArgs.SocketError == SocketError.WouldBlock ||
			   asyncArgs.SocketError == SocketError.IOPending ||
			   asyncArgs.SocketError == SocketError.NoBufferSpaceAvailable)
			{
				//阻塞发送信号量
				_sendingBlocking.Reset();

				return;
			}

			if(this.CheckUsable(asyncArgs, asyncArgs.UserToken))
			{
				//放行发送信号量
				_sendingBlocking.Set();

				var token = (SendingToken)asyncArgs.UserToken;
				PackingState state;

				if(_sendingCounter.TryGetValue(token.SequenceId, out state))
				{
					Interlocked.Increment(ref state.CountOfSend);

					if(this.EnsureSent(token.SequenceId))
						this.OnSent(new SentEventArgs(this, token.AsyncState));
				}
			}

			//将当前异步事件参数对象回收到池中
			_sendingAsyncArgsPool.Release(asyncArgs);
		}
		#endregion

		#region 接收数据
		/// <summary>
		/// 启动异步接收数据。
		/// </summary>
		protected void Receive()
		{
			var socket = _socket;

			if(socket == null)
				return;

			if(_receivingAsyncArgs == null)
			{
				var original = Interlocked.CompareExchange(ref _receivingAsyncArgs, new SocketAsyncEventArgs(), null);

				if(original == null)
				{
					_receivingAsyncArgs.SetBuffer(new byte[_receivingBufferSize], 0, _receivingBufferSize);
					_receivingAsyncArgs.Completed += SocketAsyncEventArgs_Completed;
				}
			}

			try
			{
				//继续异步接收后续数据
				if(!socket.ReceiveAsync(_receivingAsyncArgs))
					this.OnReceiveCompleted(_receivingAsyncArgs);
			}
			catch
			{
				/*
				 * 注意：此处异常不需要重抛，该异常可能由于发送过程失败导致当前通道被关闭，
				 * 因此当前socket已被关闭而导致的异常，而该问题已经有发送操作激发过Failed事件了，
				 * 故此处异常捕获器不要虚重抛异常也不需要再重复关闭通道。
				 */
			}
		}

		/// <summary>
		/// 处理异步收到的数据，在此方法内进行解包处理。
		/// </summary>
		/// <param name="asyncArgs">接收到的异步事件参数。</param>
		protected void OnReceive(SocketAsyncEventArgs asyncArgs)
		{
			var packetizer = this.Packetizer;

			if(packetizer == null)
				throw new InvalidOperationException("The value of 'Packetizer' property is null.");

			var buffer = new Zongsoft.Common.Buffer(asyncArgs.Buffer, asyncArgs.Offset, asyncArgs.BytesTransferred);

			foreach(var receivedObject in packetizer.Unpack(buffer))
			{
				//激发数据接收完成事件
				this.RaiseReceived(receivedObject);
			}
		}
		#endregion

		#region 私有方法
		/// <summary>
		/// 确认指定的发送流水号对应的信息是否已经全部发送完毕。
		/// </summary>
		/// <param name="sequenceId"></param>
		/// <returns>返回</returns>
		private bool EnsureSent(long sequenceId)
		{
			PackingState state;

			//如果指定的流水号在发送计数器集合中未找到，则表示已经发送完成
			if(!_sendingCounter.TryGetValue(sequenceId, out state))
				return false;

			if(state.IsPacked && state.CountOfPack == state.CountOfSend)
			{
				//更新最后发送时间
				this.LastSendTime = DateTime.Now;

				//将当前发送描述从发送状态集合中删除
				_sendingCounter.TryRemove(sequenceId, out state);

				return true;
			}

			return false;
		}

		private bool CheckUsable(SocketAsyncEventArgs asyncArgs, object state)
		{
			switch(asyncArgs.LastOperation)
			{
				case SocketAsyncOperation.Receive:
					switch(asyncArgs.SocketError)
					{
						case SocketError.Success:
							if(asyncArgs.BytesTransferred > 0)
								return true;

							//在某些情况下可能返回的接收字节数是零，这表示远程端关闭连接，因此本地端也必须响应一个关闭。
							this.Close();
							return false;
						case SocketError.ConnectionReset:
							if(asyncArgs.BytesTransferred > 0)
								this.RaiseFailed("Receive failed.", state);
							else
								this.Close();

							return false;
						default:
							this.RaiseFailed("Receive failed.", state);
							return false;
					}
				case SocketAsyncOperation.Send:
					if(asyncArgs.SocketError == SocketError.Success && asyncArgs.BytesTransferred > 0)
						return true;

					this.RaiseSendFailed((SendingToken)asyncArgs.UserToken);

					return false;
			}

			return false;
		}
		#endregion

		#region 激发事件
		private void RaiseSendFailed(SendingToken token)
		{
			PackingState state;

			if(_sendingCounter.TryRemove(token.SequenceId, out state))
			{
				this.RaiseFailed("Send faild.", token.AsyncState);
			}
		}

		private void RaiseFailed(string message, object state)
		{
			try
			{
				//首先激发“Failed”事件，以免在通道关闭后，导致客户端取消了对“Failed”事件的关联
				this.OnFailed(new ChannelFailureEventArgs(this, message, state));
			}
			finally
			{
				//关闭当前通道对象
				this.Close();
			}
		}

		private void RaiseReceived(object receivedObject)
		{
			//更新最后接收到数据的时间，因为空包也要计算在内，所以必须放在下面的条件判断之前
			this.LastReceivedTime = DateTime.Now;

			//创建接收数据事件参数对象
			var eventArgs = new ReceivedEventArgs(this, receivedObject);

			//启动一个新异步任务进行事件回调
			System.Threading.Tasks.Task.Factory.StartNew(() =>
			{
				//激发当前通道的“Received”事件
				this.OnReceived(eventArgs);

				//如果接收到的对象是流则关闭它
				if(receivedObject is Stream)
					((Stream)receivedObject).Dispose();
			});
		}
		#endregion

		#region 关闭方法
		/// <summary>
		/// 实现<see cref="TcpChannel"/>网络通道的默认关闭逻辑。
		/// </summary>
		/// <remarks>
		///		<para>关闭当前通道对应的内部Socket对象。</para>
		/// </remarks>
		protected override void OnClose()
		{
			var socket = Interlocked.Exchange(ref _socket, null);

			//如果Socket变量为空则表明当前通道已经关闭了
			if(socket == null)
				return;

			try
			{
				try
				{
					socket.Shutdown(SocketShutdown.Both);
				}
				catch
				{
				}

				socket.Close();
			}
			finally
			{
				_sendingCounter.Clear();
			}
		}
		#endregion

		#region 嵌套子类
		private class PackingState
		{
			public bool IsPacked;
			public long CountOfPack;
			public long CountOfSend;
		}

		private class SendingToken
		{
			public SendingToken(long sequenceId, object asyncState)
			{
				this.SequenceId = sequenceId;
				this.AsyncState = asyncState;
			}

			public long SequenceId;
			public object AsyncState;
		}
		#endregion
	}
}

/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2015 Zongsoft Corporation <http://www.zongsoft.com>
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
using System.Threading;
using System.Threading.Tasks;

namespace Zongsoft.Messaging
{
	[System.ComponentModel.DisplayName("${Text.MessageQueueListener.Title}")]
	[System.ComponentModel.Description("${Text.MessageQueueListener.Description}")]
	public class MessageQueueListener : Zongsoft.Communication.ListenerBase
	{
		#region 成员字段
		private IMessageQueue _queue;
		private MessageQueueChannel _channel;
		#endregion

		#region 构造函数
		public MessageQueueListener(string name) : base(name)
		{
		}

		public MessageQueueListener(IMessageQueue queue) : base("MessageQueueListener")
		{
			if(queue == null)
				throw new ArgumentNullException("queue");

			_queue = queue;

			if(!string.IsNullOrWhiteSpace(queue.Name))
				base.Name = queue.Name;
		}
		#endregion

		#region 公共属性
		public IMessageQueue Queue
		{
			get
			{
				return _queue;
			}
			set
			{
				if(this.State == Services.WorkerState.Running)
					throw new InvalidOperationException();

				_queue = value ?? throw new ArgumentNullException();
			}
		}
		#endregion

		#region 重写方法
		protected override void OnStart(string[] args)
		{
#if DEBUG
			Zongsoft.Diagnostics.Logger.Trace(this.Name + " [Starting]");
#endif

			var queue = this.Queue;

			if(queue == null)
				throw new InvalidOperationException("Missing the 'Queue' for the operation.");

			this.Receiver = _channel = new MessageQueueChannel(1, queue);
			_channel.ReceiveAsync();
		}

		protected override void OnStop(string[] args)
		{
#if DEBUG
			Zongsoft.Diagnostics.Logger.Trace(this.Name + " [Stopping]");
#endif

			var channel = System.Threading.Interlocked.Exchange(ref _channel, null);
			this.Receiver = null;

			if(channel != null)
				channel.Dispose();
		}
		#endregion

		#region 嵌套子类
		private class MessageQueueChannel : Zongsoft.Communication.ChannelBase
		{
			#region 私有变量
			private CancellationTokenSource _cancellation;
			#endregion

			#region 构造函数
			public MessageQueueChannel(int channelId, IMessageQueue queue) : base(channelId, queue)
			{
				_cancellation = new CancellationTokenSource();
			}
			#endregion

			#region 公共属性
			public override bool IsIdled
			{
				get
				{
					var cancellation = _cancellation;
					return cancellation != null && !cancellation.IsCancellationRequested;
				}
			}
			#endregion

			#region 收取消息
			public void ReceiveAsync()
			{
				var cancellation = _cancellation;

				if(cancellation == null || cancellation.IsCancellationRequested)
					return;

				Task.Factory.StartNew(this.OnReceive, cancellation.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
			}

			private void OnReceive()
			{
				var queue = (IMessageQueue)this.Host;

				if(queue == null)
					return;

				var cancellation = _cancellation;
				MessageBase message = null;

				while(!cancellation.IsCancellationRequested)
				{
					try
					{
						//以同步方式从消息队列中获取一条消息
						message = queue.Dequeue(new MessageDequeueSettings(TimeSpan.FromSeconds(10)));
					}
					catch
					{
						message = null;
					}

					//如果消息获取失败则休息一小会
					if(message == null)
						Thread.Sleep(500);
					else //以异步方式激发消息接收事件
						Task.Run(() =>
						{
							try
							{
								this.OnReceived(message);
							}
							catch { }
						}, cancellation.Token);
				}
			}
			#endregion

			#region 发送方法
			public override void Send(Stream stream, object asyncState = null)
			{
				var queue = (IMessageQueue)this.Host;

				if(queue == null || _cancellation.IsCancellationRequested)
					return;

				queue.EnqueueAsync(stream).ContinueWith(_ =>
				{
					this.OnSent(asyncState);
				});
			}

			public override void Send(byte[] buffer, int offset, int count, object asyncState = null)
			{
				var queue = (IMessageQueue)this.Host;

				if(queue == null || _cancellation.IsCancellationRequested)
					return;

				if(buffer == null)
					throw new ArgumentNullException("buffer");

				if(offset < 0 || offset >= buffer.Length - 1)
					throw new ArgumentOutOfRangeException("offset");

				if(count < 0 || count > buffer.Length - offset)
					throw new ArgumentOutOfRangeException("count");

				var data = new byte[count];
				Array.Copy(buffer, offset, data, 0, count);

				queue.EnqueueAsync(data).ContinueWith(_ =>
				{
					this.OnSent(asyncState);
				});
			}
			#endregion

			#region 关闭处理
			protected override void OnClose()
			{
				var cancellation = Interlocked.Exchange(ref _cancellation, null);

				if(cancellation != null)
					cancellation.Cancel();
			}
			#endregion
		}
		#endregion
	}
}

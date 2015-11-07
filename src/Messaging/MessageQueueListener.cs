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
using System.ComponentModel;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Zongsoft.Messaging
{
	[DisplayName("${Text.MessageQueueListener.Title}")]
	[Description("${Text.MessageQueueListener.Description}")]
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
				if(value == null)
					throw new ArgumentNullException();

				if(this.State == Services.WorkerState.Running)
					throw new InvalidOperationException();

				_queue = value;
			}
		}
		#endregion

		#region 重写方法
		protected override void OnStart(string[] args)
		{
#if DEBUG
			Zongsoft.Diagnostics.Logger.Trace(this.GetType().FullName + " [Starting]");
#endif

			var queue = this.Queue;

			if(queue == null)
				throw new MissingMemberException(this.GetType().FullName, "Queue");

			_channel = new MessageQueueChannel(1, queue);
			this.Receiver = _channel;
			_channel.ReceiveAsync();
		}

		protected override void OnStop(string[] args)
		{
#if DEBUG
			Zongsoft.Diagnostics.Logger.Trace(this.GetType().FullName + " [Stopping]");
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
			private volatile int _isClosed;
			private IMessageQueue _queue;
			#endregion

			#region 构造函数
			public MessageQueueChannel(int channelId, IMessageQueue queue) : base(channelId)
			{
				if(queue == null)
					throw new ArgumentNullException("queue");

				_queue = queue;
			}
			#endregion

			#region 公共属性
			public override bool IsIdled
			{
				get
				{
					return _isClosed != 0;
				}
			}
			#endregion

			#region 收取消息
			public void ReceiveAsync()
			{
				var task = new Task(this.OnReceive, CancellationToken.None, TaskCreationOptions.LongRunning);
				task.Start();
			}

			private void OnReceive()
			{
				var queue = _queue;

				if(queue == null)
					return;

				while(_isClosed == 0)
				{
					var message = queue.Dequeue(new MessageDequeueSettings(TimeSpan.FromSeconds(30)));

					if(message != null)
						this.OnReceived(new Communication.ReceivedEventArgs(this, message));
				}
			}
			#endregion

			#region 发送方法
			public override void Send(Stream stream, object asyncState = null)
			{
				var queue = _queue;

				if(queue == null || _isClosed != 0)
					return;

				_queue.EnqueueAsync(stream).ContinueWith(_ =>
				{
					this.OnSent(new Communication.SentEventArgs(this, asyncState));
				});
			}

			public override void Send(byte[] buffer, int offset, int count, object asyncState = null)
			{
				var queue = _queue;

				if(queue == null || _isClosed != 0)
					return;

				if(buffer == null)
					throw new ArgumentNullException("buffer");

				if(offset < 0 || offset >= buffer.Length - 1)
					throw new ArgumentOutOfRangeException("offset");

				if(count < 0 || count > buffer.Length - offset)
					throw new ArgumentOutOfRangeException("count");

				var data = new byte[count];
				Array.Copy(buffer, offset, data, 0, count);

				_queue.EnqueueAsync(data).ContinueWith(_ =>
				{
					this.OnSent(new Communication.SentEventArgs(this, asyncState));
				});
			}
			#endregion

			#region 关闭处理
			protected override void OnClose()
			{
				_isClosed = 1;
			}
			#endregion
		}
		#endregion
	}
}

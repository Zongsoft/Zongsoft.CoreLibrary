/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2015-2017 Zongsoft Corporation <http://www.zongsoft.com>
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
using System.Collections.Concurrent;

using Zongsoft.Communication;
using Zongsoft.Communication.Composition;

namespace Zongsoft.Messaging
{
	public class TopicReceiverBase : Communication.IReceiver
	{
		#region 事件声明
		public event EventHandler<ChannelFailureEventArgs> Failed;
		public event EventHandler<ReceivedEventArgs> Received;
		#endregion

		#region 成员字段
		private ITopic _topic;
		private IExecutor _executor;
		#endregion

		#region 构造函数
		protected TopicReceiverBase(ITopic topic)
		{
			if(topic == null)
				throw new ArgumentNullException(nameof(topic));

			_topic = topic;
		}
		#endregion

		#region 公共属性
		public ITopic Topic
		{
			get
			{
				return _topic;
			}
		}

		public IExecutor Executor
		{
			get
			{
				if(_executor == null)
				{
					lock(this)
					{
						if(_executor == null)
							_executor = new Executor(this);
					}
				}

				return _executor;
			}
			set
			{
				if(value == null)
					throw new ArgumentNullException();

				_executor = value;
			}
		}
		#endregion

		#region 虚拟方法
		protected virtual void OnFail(Exception exception)
		{
			var failed = this.Failed;

			if(failed != null)
				failed(this, new ChannelFailureEventArgs(null, exception));
		}

		protected virtual void OnReceive(TopicMessage message)
		{
			if(message == null)
				return;

			var received = this.Received;

			if(received != null)
				received(this, new ReceivedEventArgs(null, message));

			if(_executor != null)
				_executor.Execute(message);
		}
		#endregion
	}
}

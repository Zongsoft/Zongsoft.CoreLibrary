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
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Zongsoft.Messaging
{
	/// <summary>
	/// 表示消息队列的接口。
	/// </summary>
	public interface IMessageQueue : Zongsoft.Collections.IQueue<MessageBase>
	{
		#region 入队方法
		void Enqueue(object item, MessageEnqueueSettings settings = null);
		Task EnqueueAsync(object item, MessageEnqueueSettings settings = null);

		int EnqueueMany<TItem>(IEnumerable<TItem> items, MessageEnqueueSettings settings = null);
		Task<int> EnqueueManyAsync<TItem>(IEnumerable<TItem> items, MessageEnqueueSettings settings = null);
		#endregion

		#region 出队方法
		MessageBase Dequeue(MessageDequeueSettings settings = null);
		IEnumerable<MessageBase> Dequeue(int count, MessageDequeueSettings settings = null);

		Task<MessageBase> DequeueAsync(MessageDequeueSettings settings = null);
		Task<IEnumerable<MessageBase>> DequeueAsync(int count, MessageDequeueSettings settings = null);
		#endregion
	}
}

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
using System.Linq;

namespace Zongsoft.Messaging
{
	public abstract class MessageQueueBase : Zongsoft.Collections.IQueue<MessageBase>
	{
		#region 事件定义
		public event EventHandler<Zongsoft.Collections.DequeuedEventArgs> Dequeued;
		public event EventHandler<Zongsoft.Collections.EnqueuedEventArgs> Enqueued;
		#endregion

		#region 成员字段
		private string _name;
		#endregion

		#region 构造函数
		protected MessageQueueBase(string name)
		{
			if(string.IsNullOrWhiteSpace(name))
				throw new ArgumentNullException("name");

			_name = name.Trim();
		}
		#endregion

		#region 公共属性
		public virtual string Name
		{
			get
			{
				return _name;
			}
		}

		public virtual long Count
		{
			get
			{
				return TaskUtility.ExecuteTask(() => this.GetCountAsync());
			}
		}
		#endregion

		#region 保护属性
		protected virtual int Capacity
		{
			get
			{
				return 0;
			}
		}
		#endregion

		#region 队列方法
		public abstract Task<long> GetCountAsync();

		public virtual void Enqueue(object item)
		{
			this.EnqueueMany(new object[] { item });
		}

		public virtual Task EnqueueAsync(object item)
		{
			return this.EnqueueManyAsync(new object[] { item });
		}

		public virtual int EnqueueMany<T>(IEnumerable<T> items)
		{
			return TaskUtility.ExecuteTask(() => this.EnqueueManyAsync(items));
		}

		public abstract Task<int> EnqueueManyAsync<TItem>(IEnumerable<TItem> items);

		public virtual MessageBase Dequeue()
		{
			var result = this.Dequeue(1);

			if(result == null)
				return null;

			return result.FirstOrDefault();
		}

		public virtual IEnumerable<MessageBase> Dequeue(int count)
		{
			return TaskUtility.ExecuteTask(() => this.DequeueAsync(count));
		}

		public virtual async Task<MessageBase> DequeueAsync()
		{
			var result = await this.DequeueAsync(1);

			if(result == null)
				return null;

			return result.FirstOrDefault();
		}

		public abstract Task<IEnumerable<MessageBase>> DequeueAsync(int count);

		public virtual MessageBase Peek()
		{
			var result = this.Peek(1);

			if(result == null)
				return null;
			else
				return result.FirstOrDefault();
		}

		public virtual IEnumerable<MessageBase> Peek(int count)
		{
			return TaskUtility.ExecuteTask(() => this.PeekAsync(count));
		}

		public virtual async Task<MessageBase> PeekAsync()
		{
			var result = await this.PeekAsync(1);

			if(result == null)
				return null;

			return result.FirstOrDefault();
		}

		public abstract Task<IEnumerable<MessageBase>> PeekAsync(int count);

		public virtual MessageBase Take(int startOffset)
		{
			var result = this.Take(startOffset, 1);

			if(result == null)
				return null;
			else
				return result.FirstOrDefault();
		}

		public virtual IEnumerable<MessageBase> Take(int startOffset, int count)
		{
			return TaskUtility.ExecuteTask(() => this.TakeAsync(startOffset, count));
		}

		public virtual async Task<MessageBase> TakeAsync(int startOffset)
		{
			var result = await this.TakeAsync(startOffset, 1);

			if(result == null)
				return null;

			return result.FirstOrDefault();
		}

		public abstract Task<IEnumerable<MessageBase>> TakeAsync(int startOffset, int count);

		void Zongsoft.Collections.IQueue.Clear()
		{
			this.ClearQueue();
		}

		object Zongsoft.Collections.IQueue.Dequeue()
		{
			return this.Dequeue();
		}

		IEnumerable Zongsoft.Collections.IQueue.Dequeue(int count)
		{
			return this.Dequeue(count);
		}

		object Zongsoft.Collections.IQueue.Peek()
		{
			return this.Peek();
		}

		IEnumerable Zongsoft.Collections.IQueue.Peek(int count)
		{
			return this.Peek(count);
		}

		object Zongsoft.Collections.IQueue.Take(int startOffset)
		{
			return this.TakeQueue(startOffset);
		}

		IEnumerable Zongsoft.Collections.IQueue.Take(int startOffset, int count)
		{
			return this.TakeQueue(startOffset, count);
		}
		#endregion

		#region 保护方法
		protected virtual void ClearQueue()
		{
			throw new NotSupportedException("The message queue does not support the operation.");
		}

		protected virtual object TakeQueue(int startOffset)
		{
			var result = this.TakeQueue(startOffset, 1);

			if(result == null)
				return null;

			return result.GetEnumerator().Current;
		}

		protected virtual IEnumerable TakeQueue(int startOffset, int count)
		{
			throw new NotSupportedException("The message queue does not support the operation.");
		}

		protected virtual void CopyQueueTo(Array array, int index)
		{
			throw new NotSupportedException("The message queue does not support the operation.");
		}
		#endregion

		#region 激发事件
		protected virtual void OnDequeued(Zongsoft.Collections.DequeuedEventArgs args)
		{
			var handler = this.Dequeued;

			if(handler != null)
				handler(this, args);
		}

		protected virtual void OnEnqueued(Zongsoft.Collections.EnqueuedEventArgs args)
		{
			var handler = this.Enqueued;

			if(handler != null)
				handler(this, args);
		}
		#endregion

		#region 显式实现
		int Zongsoft.Collections.IQueue.Capacity
		{
			get
			{
				return this.Capacity;
			}
		}

		int ICollection.Count
		{
			get
			{
				return (int)this.Count;
			}
		}

		bool ICollection.IsSynchronized
		{
			get
			{
				return false;
			}
		}

		object ICollection.SyncRoot
		{
			get
			{
				throw new NotSupportedException("The message queue does not support the operation.");
			}
		}

		void ICollection.CopyTo(Array array, int index)
		{
			this.CopyQueueTo(array, index);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			throw new NotSupportedException("The message queue does not support the operation.");
		}
		#endregion
	}
}

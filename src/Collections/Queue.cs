/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2011 Zongsoft Corporation <http://www.zongsoft.com>
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
using System.Collections.Specialized;

namespace Zongsoft.Collections
{
	/// <summary>
	/// 队列，表示先进先出的数据容器。
	/// </summary>
	/// <remarks>
	///		<para>本队列的默认实现不同步。</para>
	///		<para>本队列中的各种方法本质上不是一个线程安全的过程。若要确保各种操作过程中的线程安全性，可以在操作过程中锁定队列。若要允许多个线程访问集合以进行读写操作，则必须实现自己的同步。</para>
	///		<para>通过锁定实现于<seealso cref="System.Collections.ICollection"/>接口中的<seealso cref="System.Collections.ICollection.SyncRoot"/>属性值，达成同步访问的效果。</para>
	/// </remarks>
	public class Queue : Zongsoft.Collections.IQueue
	{
		#region 静态变量
		private static int _id;
		#endregion

		#region 事件声明
		/// <summary>
		/// 表示入队成功的事件。
		/// </summary>
		public event EventHandler<EnqueuedEventArgs> Enqueued;

		/// <summary>
		/// 表示出队成功的事件。
		/// </summary>
		public event EventHandler<DequeuedEventArgs> Dequeued;
		#endregion

		#region 私有变量
		private object _syncRoot;
		private int _head;
		private int _tail;
		private int _size;
		private int _growFactor;
		private string _name;
		private object[] _buffer;
		private int _maximumLimit;
		private int _minimumGrow = 0xFF;
		#endregion

		#region 构造函数
		/// <summary>
		/// 初始化<seealso cref="Zongsoft.Collections.Queue"/>类的新实例，该实例为空，其初始容量为32和2.0f的成长因子。
		/// </summary>
		public Queue() : this(32, 2.0f)
		{
		}

		/// <summary>
		/// 初始化<seealso cref="Zongsoft.Collections.Queue"/>类的新实例，该实例为空，具有指定的初始容量和默认为2.0f的成长因子。
		/// </summary>
		/// <param name="capacity">初始化的队列容量，即包含的初始元素数。</param>
		public Queue(int capacity) : this(capacity, 2.0f)
		{
		}

		/// <summary>
		/// 初始化<seealso cref="Zongsoft.Collections.Queue"/>类的新实例，该实例为空，具有指定的初始容量并使用指定的成长因子。
		/// </summary>
		/// <param name="capacity">初始化的队列容量，即包含的初始元素数。</param>
		/// <param name="growFactor">扩展队列容量要使用的因子。</param>
		/// <exception cref="ArgumentOutOfRangeException">当<paramref name="capacity"/>参数小于壹(1)，或者<paramref name="growFactor"/>参数小于1.0f或大于10.0f。</exception>
		public Queue(int capacity, float growFactor)
		{
			if(capacity < 1)
				throw new ArgumentOutOfRangeException();
			if(growFactor < 1.0f || growFactor > 10.0f)
				throw new ArgumentOutOfRangeException();

			_size = 0;
			_head = 0;
			_tail = 0;
			_maximumLimit = 0x00FFFFFF;
			_minimumGrow = 0xFF;
			_growFactor = (int)(growFactor * 100);
			_buffer = new object[capacity];
			_name = this.GetType().FullName + "#" + System.Threading.Interlocked.Increment(ref _id).ToString();
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取队列名称。
		/// </summary>
		/// <remarks>该队列名称确保进程范围内的唯一性。</remarks>
		public string Name
		{
			get
			{
				return _name;
			}
		}

		/// <summary>
		/// 获取队列中实际包含的元素数。
		/// </summary>
		public int Count
		{
			get
			{
				return _size;
			}
		}

		/// <summary>
		/// 获取队列容量值，即队列中已分配的可用元素数，如果该数值小于<seealso cref="MaximumLimit"/>属性值，待扩容时会增加此值。
		/// </summary>
		public int Capacity
		{
			get
			{
				return _buffer.Length;
			}
		}

		/// <summary>
		/// 获取或设置允许的最大队列元素数，默认值为0x00FFFFFF(十六进制)。
		/// </summary>
		/// <exception cref="System.ArgumentOutOfRangeException">当设置的值小于<seealso cref="Count"/>属性值。</exception>
		/// <remarks>该属性值在下次扩容的时候才会生效。</remarks>
		public int MaximumLimit
		{
			get
			{
				return _maximumLimit;
			}
			set
			{
				if(value < _size)
					throw new ArgumentOutOfRangeException();

				_maximumLimit = value;
			}
		}

		/// <summary>
		/// 获取或设置每次变化时的最小成长量，默认值为255。
		/// </summary>
		/// <exception cref="System.ArgumentOutOfRangeException">当设置值小于壹(1)。</exception>
		/// <remarks>
		///		<para>在扩容时，如果根据成长因子计算出来的成长量小于指定的最小成长量，则按此最小成长量增长。</para>
		///		<para>发生队列满溢时系统会自动进行出队，而每次满溢出队的量亦为此最小成长量。</para>
		/// </remarks>
		public int MinimumGrow
		{
			get
			{
				return _minimumGrow;
			}
			set
			{
				if(value < 1)
					throw new ArgumentOutOfRangeException();

				_minimumGrow = value;
			}
		}
		#endregion

		#region 公共方法
		/// <summary>
		/// 移除队列中的所有元素。
		/// </summary>
		/// <remarks>
		///		<para><see cref="Count"/>被设置为零，并且对来自该集合元素的其他对象的引用也被释放。</para>
		/// </remarks>
		public void Clear()
		{
			if(_size < 1)
				return;

			if(_head < _tail)
				Array.Clear(_buffer, _head, _size);
			else
			{
				Array.Clear(_buffer, _head, _buffer.Length - _head);
				Array.Clear(_buffer, 0, _tail);
			}

			_head = 0;
			_tail = 0;
			_size = 0;
		}

		public Object Clone()
		{
			Queue queue = new Queue(_size);
			queue._size = _size;

			int numToCopy = _size;
			int firstPart = (_buffer.Length - _head < numToCopy) ? _buffer.Length - _head : numToCopy;
			Array.Copy(_buffer, _head, queue._buffer, 0, firstPart);
			numToCopy -= firstPart;
			if(numToCopy > 0)
				Array.Copy(_buffer, 0, queue._buffer, _buffer.Length - _head, numToCopy);

			return queue;
		}

		public bool Contains(Object obj)
		{
			int index = _head;
			int count = _size;

			while(count-- > 0)
			{
				if(obj == null)
				{
					if(_buffer[index] == null)
						return true;
				}
				else
				{
					if(_buffer[index] != null && _buffer[index].Equals(obj))
						return true;
				}

				index = (index + 1) % _buffer.Length;
			}

			return false;
		}

		public void TrimToSize()
		{
			this.SetCapacity(_size);
		}

		public Object[] ToArray()
		{
			Object[] arr = new Object[_size];

			if(_size == 0)
				return arr;

			if(_head < _tail)
			{
				Array.Copy(_buffer, _head, arr, 0, _size);
			}
			else
			{
				Array.Copy(_buffer, _head, arr, 0, _buffer.Length - _head);
				Array.Copy(_buffer, 0, arr, _buffer.Length - _head, _tail);
			}

			return arr;
		}
		#endregion

		#region 出队操作
		/// <summary>
		/// 移除并返回位于<see cref="Zongsoft.Collections.Queue"/>开始处的对象。
		/// </summary>
		/// <returns>从队列的开头处移除的对象。</returns>
		/// <exception cref="System.InvalidOperationException">当队列为空，即<see cref="Count"/>属性为零。</exception>
		public object Dequeue()
		{
			if(_size == 0)
				throw new InvalidOperationException();

			object removed = _buffer[_head];
			_buffer[_head] = null;
			_head = (_head + 1) % _buffer.Length;
			_size--;

			//激发“Dequeued”出队事件
			this.OnDequeued(new DequeuedEventArgs(removed, false, CollectionRemovedReason.Remove));

			return removed;
		}

		/// <summary>
		/// 移除并返回从开始处的由<paramref name="count"/>参数指定的连续多个对象。
		/// </summary>
		/// <param name="count">指定要连续移除的元素数。</param>
		/// <returns>从队列的开头处指定的连续对象集。</returns>
		/// <exception cref="System.InvalidOperationException">当队列为空，即<see cref="Count"/>属性等于零。</exception>
		/// <exception cref="System.ArgumentOutOfRangeException"><paramref name="count"/>参数小于壹(1)。</exception>
		/// <remarks>如果<paramref name="count"/>参数指定的数值超出队列中可用的元素数，则忽略该参数值，而应用可用的元素数。</remarks>
		public IEnumerable Dequeue(int count)
		{
			return this.Dequeue(count, CollectionRemovedReason.Remove);
		}

		private IEnumerable Dequeue(int count, CollectionRemovedReason reason)
		{
			int actualLength;
			var result = this.GetElements(0, count, true, out actualLength);

			_head = (_head + actualLength) % _buffer.Length;
			_size -= actualLength;

			//激发“Dequeued”出队事件
			this.OnDequeued(new DequeuedEventArgs(result, true, reason));

			return result;
		}
		#endregion

		#region 入队操作
		/// <summary>
		/// 将指定集合中的所有元素依次添加到<seealso cref="Zongsoft.Collections.Queue"/>的结尾处。
		/// </summary>
		/// <param name="items">要入队的集合。</param>
		/// <param name="settings">不支持入队的选项参数设置，始终忽略该参数。</param>
		public int EnqueueMany<T>(IEnumerable<T> items, object settings = null)
		{
			if(items == null)
				throw new ArgumentNullException("items");

			int count = 0;

			foreach(var item in items)
			{
				this.Enqueue(item, settings);
				count++;
			}

			return count;
		}

		/// <summary>
		/// 将字符串文本添加到<seealso cref="Zongsoft.Collections.Queue"/>的结尾处。
		/// </summary>
		/// <param name="text">要入队的字符串文本，该值可以为空(null)。</param>
		/// <param name="settings">不支持入队的选项参数设置，始终忽略该参数。</param>
		public void Enqueue(string text, object settings = null)
		{
			this.Enqueue((object)text, settings);
		}

		/// <summary>
		/// 将对象添加到<seealso cref="Zongsoft.Collections.Queue"/>的结尾处。
		/// </summary>
		/// <param name="item">要入队的对象，该值可以为空(null)。</param>
		/// <param name="settings">不支持入队的选项参数设置，始终忽略该参数。</param>
		/// <remarks>
		///		<para>容量<seealso cref="Capacity"/>是指队列可以保存的元素数。随着入队操作（即向队列中添加元素），容量通过重新分配按需自动增加。但是增加到最大限制值(<seealso cref="MaximumLimit"/>)就不再扩容，而是首先导致出队以腾出空间再入队。</para>
		///		<para>成长因子是当需要更大容量时当前容量要乘以的数字。在构造<seealso cref="Zongsoft.Collections.Queue"/>时确定增长因子。无论增长因子是多少，队列的容量将始终增加一个最小值（即<see cref="MinimumGrow"/>属性值），即使1.0f的增长因子也不会阻止队列的扩容。</para>
		/// </remarks>
		public void Enqueue(object item, object settings = null)
		{
			if(_size == _buffer.Length)
			{
				if(_size < _maximumLimit)
				{
					int newCapacity = (int)((long)_buffer.Length * (long)_growFactor / 100);

					if(newCapacity < _buffer.Length + _minimumGrow)
						newCapacity = _buffer.Length + _minimumGrow;

					if(newCapacity > _maximumLimit)
						newCapacity = _maximumLimit;

					this.SetCapacity(newCapacity);
				}
				else
				{
					this.Dequeue(_minimumGrow, CollectionRemovedReason.Overflow);
				}
			}

			_buffer[_tail] = item;
			_tail = (_tail + 1) % _buffer.Length;
			_size++;

			//激发“Enqueued”入队事件
			this.OnEnqueued(new EnqueuedEventArgs(item, false));
		}
		#endregion

		#region 获取操作
		/// <summary>
		/// 返回从开始处的由<paramref name="count"/>参数指定的连续多个对象。
		/// </summary>
		/// <param name="count">指定要连续查看的元素数。</param>
		/// <returns>从队列的开头处指定的连续对象集。</returns>
		/// <exception cref="System.InvalidOperationException">当队列为空，即<see cref="Count"/>属性等于零。</exception>
		/// <exception cref="System.ArgumentOutOfRangeException"><paramref name="count"/>参数小于壹(1)。</exception>
		/// <remarks>如果<paramref name="count"/>参数指定的数值超出队列中可用的元素数，则忽略该参数值，而应用可用的元素数。</remarks>
		public IEnumerable Peek(int count)
		{
			return this.GetElements(0, count, false);
		}

		/// <summary>
		/// 返回位于队列开始处的对象但不将其移除。
		/// </summary>
		/// <returns>位于队列开头处的对象。</returns>
		/// <exception cref="System.InvalidOperationException">当队列为空，即<see cref="Count"/>属性等于零。</exception>
		/// <remarks>
		///		<para>此方法类似于<seealso cref="Dequeue()"/>出队方法，但本方法不修改<seealso cref="Zongsoft.Collections.Queue"/>队列。</para>
		/// </remarks>
		public object Peek()
		{
			if(_size == 0)
				throw new InvalidOperationException();

			return _buffer[_head];
		}

		/// <summary>
		/// 返回从队列开头处往后偏移由<paramref name="startOffset"/>参数指定长度后开始的由<paramref name="count"/>参数指定的连续多个对象。
		/// </summary>
		/// <param name="startOffset">从队列开头处往后偏移的长度。</param>
		/// <param name="count">要连续获取的元素数。</param>
		/// <returns>从队列的开头处指定偏移后的连续特定长度的对象集。</returns>
		/// <exception cref="System.InvalidOperationException">当队列为空，即<see cref="Count"/>属性等于零。</exception>
		/// <exception cref="System.ArgumentOutOfRangeException"><paramref name="count"/>参数小于壹(1)。</exception>
		/// <remarks>
		///		<para>如果<paramref name="count"/>参数指定的数值超出队列中可用的元素数，则忽略该参数值，而应用可用的元素数。</para>
		/// </remarks>
		public IEnumerable Take(int startOffset, int count)
		{
			return this.GetElements(startOffset, count, false);
		}

		/// <summary>
		/// 返回从队列开头处往后偏移由<paramref name="startOffset"/>参数指定长度后开始的元素值。
		/// </summary>
		/// <param name="startOffset">从队列开头处往后偏移的长度。</param>
		/// <returns>位于开头处偏移后的值。</returns>
		public object Take(int startOffset)
		{
			if(_size == 0)
				throw new InvalidOperationException();

			if(startOffset < 0)
				throw new ArgumentOutOfRangeException("startOffset");

			long finishIndex = _head < _tail ? _tail : _tail + _buffer.Length;
			long startIndex = Math.Min(_head + startOffset, finishIndex);
			return _buffer[startIndex % _buffer.Length];
		}
		#endregion

		#region 重写方法
		/// <summary>
		/// 返回当前队列的信息文本。
		/// </summary>
		/// <returns>
		///		<para>该方法重写了<seealso cref="System.Object"/>的同名方法，该重写返回如下格式的文本信息：</para>
		///		<para>(队头位置-队尾位置)[大小/容量]{最大限制, 最小成长量}</para>
		/// </returns>
		public override string ToString()
		{
			return string.Format("{0}({1}-{2}) [{3}/{4}] {{{5}, {6}}}",
			                     _name + Environment.NewLine,
								 _head, _tail,
								 _size, _buffer.Length,
								 _maximumLimit, _minimumGrow);
		}
		#endregion

		#region 激发事件
		protected virtual void OnDequeued(Zongsoft.Collections.DequeuedEventArgs args)
		{
			if(this.Dequeued != null)
				this.Dequeued(this, args);
		}

		protected virtual void OnEnqueued(Zongsoft.Collections.EnqueuedEventArgs args)
		{
			if(this.Enqueued != null)
				this.Enqueued(this, args);
		}
		#endregion

		#region 私有方法
		private ICollection GetElements(long startOffset, int count, bool cleanup)
		{
			int actualLength;
			return this.GetElements(startOffset, count, cleanup, out actualLength);
		}

		private ICollection GetElements(long startOffset, int count, bool cleanup, out int actualLength)
		{
			if(startOffset < 0)
				throw new ArgumentOutOfRangeException("startOffset");

			if(count < 1)
				throw new ArgumentOutOfRangeException("count");

			if(_size == 0)
				throw new InvalidOperationException();

			long startIndex = (_head + startOffset) % _buffer.Length;
			long finishIndex = _head < _tail ? _tail : _tail + _buffer.Length;
			actualLength = (int)Math.Min(finishIndex - startIndex + 1, count);

			object[] result = new object[actualLength];

			long availableLength = Math.Min(actualLength, _buffer.LongLength - (_head + startOffset));
			if(availableLength > 0)
				Array.Copy(_buffer, startIndex, result, 0, availableLength);

			if(actualLength > availableLength)
				Array.Copy(_buffer, 0, result, availableLength, actualLength - availableLength);

			//if(_head < _tail)
			//{
			//    Array.Copy(_buffer, startIndex, result, 0, length);
			//}
			//else
			//{
			//    long availableLength = Math.Min(actualLength, _buffer.LongLength - (_head + startOffset));
			//    Array.Copy(_buffer, startIndex, result, 0, availableLength);

			//    if(availableLength < length)
			//    {
			//        Array.Copy(_buffer, 0, result, availableLength, length - availableLength);
			//    }
			//}

			if(cleanup)
			{
				for(int i = 0; i < actualLength; i++)
					_buffer[(startIndex + i) % _buffer.Length] = null;
			}

			return result;
		}

		private void SetCapacity(int capacity)
		{
			if(capacity == this.Capacity)
				return;

			object[] array = new object[capacity];

			if(_size > 0)
			{
				if(_head < _tail)
				{
					Array.Copy(_buffer, _head, array, 0, _size);
				}
				else
				{
					Array.Copy(_buffer, _head, array, 0, _buffer.Length - _head);
					Array.Copy(_buffer, 0, array, _buffer.Length - _head, _tail);
				}
			}

			_buffer = array;
			_head = 0;
			_tail = (_size == capacity) ? 0 : _size;
		}
		#endregion

		#region 接口实现
		public IEnumerator GetEnumerator()
		{
			if(_size == 0)
				yield break;

			long finishIndex = _head < _tail ? _tail : _tail + _buffer.Length;
			long length = finishIndex - _head;

			for(long i = 0; i < length; i++)
				yield return _buffer[(_head + i) % _buffer.Length];
		}

		public virtual void CopyTo(Array array, int index)
		{
			if(array == null)
				throw new ArgumentNullException("array");

			if(array.Rank != 1)
				throw new ArgumentException();

			if(index < 0)
				throw new ArgumentOutOfRangeException("index");

			int arrayLen = array.Length;
			if(arrayLen - index < _size)
				throw new ArgumentException();

			int numToCopy = _size;
			if(numToCopy == 0)
				return;

			int firstPart = (_buffer.Length - _head < numToCopy) ? _buffer.Length - _head : numToCopy;
			Array.Copy(_buffer, _head, array, index, firstPart);
			numToCopy -= firstPart;
			if(numToCopy > 0)
				Array.Copy(_buffer, 0, array, index + _buffer.Length - _head, numToCopy);
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
				if(_syncRoot == null)
					System.Threading.Interlocked.CompareExchange(ref _syncRoot, new Object(), null);

				return _syncRoot;
			}
		}
		#endregion
	}
}

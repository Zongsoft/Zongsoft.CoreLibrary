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
using System.Linq;
using System.Text;

namespace Zongsoft.Collections
{
	/// <summary>
	/// 队列，表示先进先出的数据容器。
	/// </summary>
	public interface IQueue : ICollection
	{
		#region 事件定义
		/// <summary>
		/// 当入队成功后激发的事件。
		/// </summary>
		event EventHandler<EnqueuedEventArgs> Enqueued;

		/// <summary>
		/// 当出队成功后激发的事件。
		/// </summary>
		event EventHandler<DequeuedEventArgs> Dequeued;
		#endregion

		#region 属性定义
		/// <summary>
		/// 获取队列的名称，该名称应该为队列的唯一标识。
		/// </summary>
		string Name
		{
			get;
		}

		/// <summary>
		/// 获取队列容量值，即队列中已分配的可用元素数，该值在扩容时可能会增加。
		/// </summary>
		int Capacity
		{
			get;
		}
		#endregion

		#region 清空方法
		/// <summary>
		/// 移除队列中的所有元素。
		/// </summary>
		/// <remarks>
		///		<para><seealso cref="ICollection.Count"/>被设置为零，并且对来自该集合元素的其他对象的引用也被释放。</para>
		/// </remarks>
		void Clear();
		#endregion

		#region 入队方法
		/// <summary>
		/// 将对象添加到队列的结尾处。
		/// </summary>
		/// <param name="item">要入队的对象，该值可以为空(null)。</param>
		/// <param name="settings">指定入队的一些选项参数，具体内容请参考特定实现者的规范。</param>
		/// <remarks>
		///		<para>有些队列实现者可能忽略当<paramref name="item"/>参数为空的入队操作。</para>
		/// </remarks>
		void Enqueue(object item, object settings = null);

		/// <summary>
		/// 将指定集合中的所有元素依次添加到队列的结尾处。
		/// </summary>
		/// <param name="items">要入队的集合。</param>
		/// <param name="settings">指定入队的一些选项参数，具体内容请参考特定实现者的规范。</param>
		/// <remarks>
		///		<para>有些队列实现者可能忽略当<paramref name="items"/>参数集合中那些为空的元素的入队操作。</para>
		/// </remarks>
		int EnqueueMany<T>(IEnumerable<T> items, object settings = null);
		#endregion

		#region 出队方法
		/// <summary>
		/// 移除并返回位于队列开始处的对象。
		/// </summary>
		/// <returns>从队列的开头处移除的对象。</returns>
		/// <exception cref="System.InvalidOperationException">当队列为空，即<seealso cref="ICollection.Count"/>属性为零。</exception>
		object Dequeue();

		/// <summary>
		/// 移除并返回从开始处的由<paramref name="count"/>参数指定的连续多个对象。
		/// </summary>
		/// <param name="count">指定要连续移除的元素数。</param>
		/// <returns>从队列的开头处指定的连续对象集。</returns>
		/// <exception cref="System.InvalidOperationException">当队列为空，即<seealso cref="ICollection.Count"/>属性等于零。</exception>
		/// <exception cref="System.ArgumentOutOfRangeException"><paramref name="count"/>参数小于壹(1)。</exception>
		/// <remarks>如果<paramref name="count"/>参数指定的数值超出队列中可用的元素数，则忽略该参数值，而应用可用的元素数。</remarks>
		IEnumerable Dequeue(int count);
		#endregion

		#region 获取方法
		/// <summary>
		/// 返回位于队列开始处的对象但不将其移除。
		/// </summary>
		/// <returns>位于队列开头处的对象。</returns>
		/// <exception cref="System.InvalidOperationException">当队列为空，即<seealso cref="ICollection.Count"/>属性等于零。</exception>
		/// <remarks>
		///		<para>此方法类似于<seealso cref="Dequeue()"/>出队方法，但本方法不修改<seealso cref="Zongsoft.Collections.Queue"/>队列。</para>
		/// </remarks>
		object Peek();

		/// <summary>
		/// 返回从开始处的由<paramref name="count"/>参数指定的连续多个对象。
		/// </summary>
		/// <param name="count">指定要连续查看的元素数。</param>
		/// <returns>从队列的开头处指定的连续对象集。</returns>
		/// <exception cref="System.InvalidOperationException">当队列为空，即<seealso cref="ICollection.Count"/>属性等于零。</exception>
		/// <exception cref="System.ArgumentOutOfRangeException"><paramref name="count"/>参数小于壹(1)。</exception>
		/// <remarks>如果<paramref name="count"/>参数指定的数值超出队列中可用的元素数，则忽略该参数值，而应用可用的元素数。</remarks>
		IEnumerable Peek(int count);

		/// <summary>
		/// 返回从队列开头处往后偏移由<paramref name="startOffset"/>参数指定长度后开始的元素值。
		/// </summary>
		/// <param name="startOffset">从队列开头处往后偏移的长度。</param>
		/// <returns>位于开头处偏移后的值。</returns>
		object Take(int startOffset);

		/// <summary>
		/// 返回从队列开头处往后偏移由<paramref name="startOffset"/>参数指定长度后开始的由<paramref name="count"/>参数指定的连续多个对象。
		/// </summary>
		/// <param name="startOffset">从队列开头处往后偏移的长度。</param>
		/// <param name="count">要连续获取的元素数。</param>
		/// <returns>从队列的开头处指定偏移后的连续特定长度的对象集。</returns>
		/// <exception cref="System.InvalidOperationException">当队列为空，即<seealso cref="ICollection.Count"/>属性等于零。</exception>
		/// <exception cref="System.ArgumentOutOfRangeException"><paramref name="count"/>参数小于壹(1)。</exception>
		/// <remarks>
		///		<para>如果<paramref name="count"/>参数指定的数值超出队列中可用的元素数，则忽略该参数值，而应用可用的元素数。</para>
		/// </remarks>
		IEnumerable Take(int startOffset, int count);
		#endregion
	}
}

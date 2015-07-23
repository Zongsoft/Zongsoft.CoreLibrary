/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2011-2015 Zongsoft Corporation <http://www.zongsoft.com>
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
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Zongsoft.Collections
{
	public interface IQueue<T> : IQueue
	{
		Task EnqueueAsync(object item, object settings = null);

		Task<int> EnqueueManyAsync<TItem>(IEnumerable<TItem> items, object settings = null);

		T Dequeue(object settings = null);
		IEnumerable<T> Dequeue(int count, object settings = null);

		Task<T> DequeueAsync(object settings = null);
		Task<IEnumerable<T>> DequeueAsync(int count, object settings = null);

		new T Peek();
		new IEnumerable<T> Peek(int count);

		Task<T> PeekAsync();
		Task<IEnumerable<T>> PeekAsync(int count);

		new T Take(int startOffset);
		new IEnumerable<T> Take(int startOffset, int count);

		Task<T> TakeAsync(int startOffset);
		Task<IEnumerable<T>> TakeAsync(int startOffset, int count);
	}
}

/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2010-2013 Zongsoft Corporation <http://www.zongsoft.com>
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

namespace Zongsoft.Communication.Composition
{
	public class ExecutionPipelineCollection : System.Collections.ObjectModel.Collection<ExecutionPipeline>, ICollection<IExecutionHandler>
	{
		#region 构造函数
		public ExecutionPipelineCollection()
		{
		}

		public ExecutionPipelineCollection(IEnumerable<ExecutionPipeline> pipelines)
		{
			if(pipelines != null)
			{
				foreach(var pipeline in pipelines)
				{
					this.Add(pipeline);
				}
			}
		}
		#endregion

		#region 公共方法
		public ExecutionPipeline Add(IExecutionHandler handler, Services.IPredication predication = null)
		{
			if(handler == null)
				throw new ArgumentNullException("handler");

			var item = new ExecutionPipeline(handler, predication);
			base.Add(item);

			return item;
		}
		#endregion

		#region 显式实现
		bool ICollection<IExecutionHandler>.IsReadOnly
		{
			get
			{
				return false;
			}
		}

		void ICollection<IExecutionHandler>.Add(IExecutionHandler item)
		{
			this.Add(item, null);
		}

		void ICollection<IExecutionHandler>.CopyTo(IExecutionHandler[] array, int arrayIndex)
		{
			if(array == null)
				throw new ArgumentNullException(nameof(array));

			if(arrayIndex < 0 || arrayIndex >= array.Length)
				throw new ArgumentOutOfRangeException(nameof(arrayIndex));

			var iterator = base.GetEnumerator();

			for(int i = arrayIndex; i < array.Length; i++)
			{
				if(iterator.MoveNext())
					array[i] = iterator.Current?.Handler;
			}
		}

		bool ICollection<IExecutionHandler>.Contains(IExecutionHandler item)
		{
			throw new NotSupportedException();
		}

		bool ICollection<IExecutionHandler>.Remove(IExecutionHandler item)
		{
			throw new NotImplementedException();
		}

		IEnumerator<IExecutionHandler> IEnumerable<IExecutionHandler>.GetEnumerator()
		{
			var iterator = this.GetEnumerator();

			while(iterator.MoveNext())
			{
				yield return iterator.Current?.Handler;
			}
		}
		#endregion
	}
}

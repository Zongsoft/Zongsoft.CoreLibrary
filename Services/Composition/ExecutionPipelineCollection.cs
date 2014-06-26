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
using System.Collections.ObjectModel;

namespace Zongsoft.Services.Composition
{
	public class ExecutionPipelineCollection : Zongsoft.Collections.NamedCollectionBase<ExecutionPipeline>
	{
		#region 私有变量
		private int _index;
		#endregion

		#region 构造函数
		public ExecutionPipelineCollection()
		{
		}
		#endregion

		#region 重写方法
		protected override string GetKeyForItem(ExecutionPipeline item)
		{
			return item.Name;
		}

		protected override bool TryConvertItem(object value, out ExecutionPipeline item)
		{
			if(value is IExecutionHandler)
			{
				var index = System.Threading.Interlocked.Increment(ref _index);
				item = new ExecutionPipeline(string.Format("{0}#{1}", value.GetType().FullName, index), (IExecutionHandler)value);
				return true;
			}

			return base.TryConvertItem(value, out item);
		}
		#endregion

		#region 公共方法
		public ExecutionPipeline Add(string name, IExecutionHandler handler)
		{
			return this.Add(name, handler, null);
		}

		public ExecutionPipeline Add(string name, IExecutionHandler handler, IPredication predication)
		{
			if(handler == null)
				throw new ArgumentNullException("handler");

			var item = new ExecutionPipeline(name, handler, predication);
			base.Add(item);

			return item;
		}
		#endregion
	}
}

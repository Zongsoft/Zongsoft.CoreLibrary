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

namespace Zongsoft.Communication.Composition
{
	public class ExecutionPipelineCollection : Zongsoft.Collections.Collection<ExecutionPipeline>
	{
		#region 构造函数
		public ExecutionPipelineCollection()
		{
		}

		public ExecutionPipelineCollection(IEnumerable<ExecutionPipeline> pipelines) : base(pipelines)
		{
		}
		#endregion

		#region 重写方法
		protected override bool TryConvertItem(object value, out ExecutionPipeline item)
		{
			if(value is IExecutionHandler)
			{
				item = new ExecutionPipeline((IExecutionHandler)value);
				return true;
			}

			return base.TryConvertItem(value, out item);
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
	}
}

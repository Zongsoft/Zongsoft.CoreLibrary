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

namespace Zongsoft.Services.Composition
{
	public class ExecutionFilterComposite : IExecutionFilter
	{
		#region 成员字段
		private IExecutionFilter _filter;
		private IPredication _predication;
		#endregion

		#region 构造函数
		public ExecutionFilterComposite(IExecutionFilter filter) : this(filter, null)
		{
		}

		public ExecutionFilterComposite(IExecutionFilter filter, IPredication predication)
		{
			if(filter == null)
				throw new ArgumentNullException("filter");

			_filter = filter;
			_predication = predication;
		}
		#endregion

		#region 公共属性
		public virtual string Name
		{
			get
			{
				return _filter.Name;
			}
		}

		public IPredication Predication
		{
			get
			{
				return _predication;
			}
			set
			{
				_predication = value;
			}
		}

		public IExecutionFilter Filter
		{
			get
			{
				return _filter;
			}
			set
			{
				if(value == null)
					throw new ArgumentNullException();

				_filter = value;
			}
		}
		#endregion

		#region 显式实现
		void IExecutionFilter.OnExecuting(ExecutionPipelineContext context)
		{
			var predication = this.Predication;

			if(predication == null || predication.Predicate(context))
				_filter.OnExecuting(context);
		}

		void IExecutionFilter.OnExecuted(ExecutionPipelineContext context)
		{
			var predication = this.Predication;

			if(predication == null || predication.Predicate(context))
				_filter.OnExecuted(context);
		}
		#endregion
	}
}

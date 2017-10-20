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
	public class ExecutionPipeline
	{
		#region 成员字段
		private Services.IPredication _predication;
		private IExecutionHandler _handler;
		private ExecutionPipelineCollection _children;
		private ExecutionFilterCompositeCollection _filters;
		#endregion

		#region 构造函数
		public ExecutionPipeline() : this(null, null)
		{
		}

		public ExecutionPipeline(IExecutionHandler handler, Services.IPredication predication = null)
		{
			_handler = handler;
			_predication = predication;
		}
		#endregion

		#region 公共属性
		public bool HasChildren
		{
			get
			{
				return _children != null && _children.Count > 0;
			}
		}

		/// <summary>
		/// 获取当前管道的后续管道集合。
		/// </summary>
		public ExecutionPipelineCollection Children
		{
			get
			{
				if(_children == null)
					System.Threading.Interlocked.CompareExchange(ref _children, new ExecutionPipelineCollection(), null);

				return _children;
			}
		}

		public bool HasFilters
		{
			get
			{
				return _filters != null && _filters.Count > 0;
			}
		}

		/// <summary>
		/// 获取当前管道的过滤器集合。
		/// </summary>
		public ExecutionFilterCompositeCollection Filters
		{
			get
			{
				if(_filters == null)
					System.Threading.Interlocked.CompareExchange(ref _filters, new ExecutionFilterCompositeCollection(), null);

				return _filters;
			}
		}

		public Services.IPredication Predication
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

		public IExecutionHandler Handler
		{
			get
			{
				return _handler;
			}
			set
			{
				if(value == null)
					throw new ArgumentNullException();

				_handler = value;
			}
		}
		#endregion
	}
}

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
	public class ExecutionPipeline : MarshalByRefObject
	{
		#region 成员字段
		private ExecutionFilterCompositeCollection _filters;
		private IPredication _predication;
		private IExecutionHandler _handler;
		private ExecutionPipeline _next;
		#endregion

		#region 构造函数
		public ExecutionPipeline() : this(null, null)
		{
		}

		public ExecutionPipeline(IExecutionHandler handler, IPredication predication = null)
		{
			_handler = handler;
			_predication = predication;
			_filters = new ExecutionFilterCompositeCollection();
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取或设置当前管道的后续管道。
		/// </summary>
		public ExecutionPipeline Next
		{
			get
			{
				return _next;
			}
			set
			{
				_next = value;
			}
		}

		public ExecutionFilterCompositeCollection Filters
		{
			get
			{
				return _filters;
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

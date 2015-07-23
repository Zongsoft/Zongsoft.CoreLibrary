/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2014-2015 Zongsoft Corporation <http://www.zongsoft.com>
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
	public class ExecutionPipelineContext : ExecutionContext, IExecutionPipelineContext
	{
		#region 成员字段
		private ExecutionContext _context;
		private Exception _exception;
		private ExecutionPipeline _pipeline;
		private ExecutionPipelineCollection _children;
		#endregion

		#region 构造函数
		public ExecutionPipelineContext(ExecutionContext context, ExecutionPipeline pipeline, object parameter) : base(context.Executor, parameter)
		{
			if(pipeline == null)
				throw new ArgumentNullException("pipeline");

			_context = context;
			_pipeline = pipeline;
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取当前管道上下文的上级执行上下文。
		/// </summary>
		public IExecutionContext Context
		{
			get
			{
				return _context;
			}
		}

		/// <summary>
		/// 获取当前的<see cref="ExecutionPipeline"/>执行管道。
		/// </summary>
		public ExecutionPipeline Pipeline
		{
			get
			{
				return _pipeline;
			}
		}

		/// <summary>
		/// 获取当前管道执行中发生的异常。
		/// </summary>
		public override Exception Exception
		{
			get
			{
				return _exception;
			}
			protected internal set
			{
				_exception = value;

				if(value != null)
				{
					var exceptions = new List<Exception>(new Exception[] { value });

					if(_context.Exception != null)
					{
						var aggregateException = _context.Exception as AggregateException;

						if(aggregateException == null)
							exceptions.Add(_context.Exception);
						else
							exceptions.AddRange(aggregateException.InnerExceptions);
					}

					_context.Exception = new AggregateException(exceptions);
				}
			}
		}

		public virtual bool HasChildren
		{
			get
			{
				if(_children == null)
					return _pipeline.HasChildren;

				return _children != null && _children.Count > 0;
			}
		}

		public virtual ExecutionPipelineCollection Children
		{
			get
			{
				if(_children == null)
				{
					if(_pipeline.HasChildren)
						System.Threading.Interlocked.CompareExchange(ref _children, new ExecutionPipelineCollection(_pipeline.Children), null);
					else
						System.Threading.Interlocked.CompareExchange(ref _children, new ExecutionPipelineCollection(), null);
				}

				return _children;
			}
		}
		#endregion
	}
}

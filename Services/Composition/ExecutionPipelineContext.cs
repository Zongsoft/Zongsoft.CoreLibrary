/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2014 Zongsoft Corporation <http://www.zongsoft.com>
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
	public class ExecutionPipelineContext : MarshalByRefObject, IExecutionPipelineContext
	{
		#region 成员字段
		private ExecutionPipeline _pipeline;
		private ExecutionPipeline _previous;
		private ExecutionPipeline _next;
		private IExecutorContext _executorContext;
		private IDictionary<string, object> _extendedProperties;
		private object _result;
		#endregion

		#region 构造函数
		public ExecutionPipelineContext(IExecutorContext executorContext, ExecutionPipeline pipeline)
		{
			if(executorContext == null)
				throw new ArgumentNullException("executorContext");

			if(pipeline == null)
				throw new ArgumentNullException("pipeline");

			_pipeline = pipeline;
			_executorContext = executorContext;
			_result = executorContext.Result;
		}
		#endregion

		#region 公共属性
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
		/// 获取当前上下文的前一个管道。
		/// </summary>
		public ExecutionPipeline Previous
		{
			get
			{
				return _previous;
			}
			set
			{
				_previous = value;
			}
		}

		/// <summary>
		/// 获取当前上下文的后继管道(即下一个管道)。
		/// </summary>
		public ExecutionPipeline Next
		{
			get
			{
				return _next ?? _pipeline.Next;
			}
			set
			{
				_next = value;
			}
		}

		/// <summary>
		/// 获取处理本次执行请求的执行器。
		/// </summary>
		public IExecutor Executor
		{
			get
			{
				return _executorContext.Executor;
			}
		}

		/// <summary>
		/// 获取原始的<see cref="IExecutorContext"/>执行器上下文对象。
		/// </summary>
		public IExecutorContext ExecutorContext
		{
			get
			{
				return _executorContext;
			}
		}

		/// <summary>
		/// 获取扩展属性集是否有内容。
		/// </summary>
		/// <remarks>
		///		<para>在不确定扩展属性集是否含有内容之前，建议先使用该属性来检测。</para>
		/// </remarks>
		public virtual bool HasExtendedProperties
		{
			get
			{
				return (_extendedProperties != null);
			}
		}

		/// <summary>
		/// 获取扩展属性集。
		/// </summary>
		public virtual IDictionary<string, object> ExtendedProperties
		{
			get
			{
				if(_extendedProperties == null)
					System.Threading.Interlocked.CompareExchange(ref _extendedProperties, new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase), null);

				return _extendedProperties;
			}
		}

		/// <summary>
		/// 获取或设置当前执行管道的返回结果。
		/// </summary>
		public object Result
		{
			get
			{
				return _result;
			}
			set
			{
				_result = value;
			}
		}
		#endregion

		#region 内部方法
		bool IExecutionPipelineContext.ToNext(bool updatePrevious)
		{
			var current = _pipeline;

			if(current == null)
				return false;

			var next = current.Next;

			if(next == null)
				return false;

			if(updatePrevious)
				_previous = current;

			_pipeline = next;

			return next != null;
		}
		#endregion
	}
}

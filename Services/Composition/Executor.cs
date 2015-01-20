/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2010-2014 Zongsoft Corporation <http://www.zongsoft.com>
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
using System.Collections.Concurrent;
using System.Linq;

namespace Zongsoft.Services.Composition
{
	public class Executor : IExecutor
	{
		#region 事件定义
		/// <summary>
		/// 表示执行器对所有管道执行完成之后激发的事件。
		/// </summary>
		/// <remarks>
		///		<para>该事件确保执行器中的所有管道执行完毕后激发，而无论这些管道是否异步执行。</para>
		/// </remarks>
		public event EventHandler<ExecutedEventArgs> Executed;

		/// <summary>
		/// 表示执行器在执行操作之前激发的事件，该事件位于所有管道及过滤器之前被激发。
		/// </summary>
		public event EventHandler<ExecutingEventArgs> Executing;
		#endregion

		#region 成员字段
		private object _host;
		private IExecutionPipelineSelector _selector;
		private IExecutionCombiner _combiner;
		private ExecutionPipelineCollection _pipelines;
		private ExecutionFilterCompositeCollection _filters;
		#endregion

		#region 构造函数
		public Executor() : this(null)
		{
		}

		public Executor(object host)
		{
			_host = host;
			_pipelines = new ExecutionPipelineCollection();
			_filters = new ExecutionFilterCompositeCollection();
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取当前执行器的宿主对象。
		/// </summary>
		public object Host
		{
			get
			{
				return _host;
			}
		}

		/// <summary>
		/// 获取或设置一个<see cref="IExecutionPipelineSelector"/>管道执行选择器。
		/// </summary>
		public IExecutionPipelineSelector PipelineSelector
		{
			get
			{
				return _selector;
			}
			set
			{
				_selector = value;
			}
		}

		/// <summary>
		/// 获取或设置一个<see cref="IExecutionCombiner"/>执行结果合并器。
		/// </summary>
		public IExecutionCombiner Combiner
		{
			get
			{
				return _combiner;
			}
			set
			{
				_combiner = value;
			}
		}

		/// <summary>
		/// 获取当前执行器的管道集合。
		/// </summary>
		public ExecutionPipelineCollection Pipelines
		{
			get
			{
				return _pipelines;
			}
		}

		/// <summary>
		/// 获取当前执行器的全局过滤器集合。
		/// </summary>
		public ExecutionFilterCompositeCollection Filters
		{
			get
			{
				return _filters;
			}
		}
		#endregion

		#region 执行方法
		public object Execute(object parameter = null)
		{
			return this.Execute(this.CreateExecutorContext(parameter));
		}

		protected object Execute(IExecutorContext context)
		{
			if(context == null)
				throw new ArgumentNullException("context");

			//创建“Executing”事件的调用参数
			ExecutingEventArgs executingArgs = new ExecutingEventArgs(this, context.Parameter);

			//激发“Executing”事件
			this.OnExecuting(executingArgs);

			if(executingArgs.Cancel)
				return executingArgs.Result;

			//通过选择器获取当前请求对应的管道集
			var pipelines = this.SelectPipelines(context);

			//如果当前执行的处理管道数为零则退出
			if(pipelines == null || pipelines.Count() < 1)
				return null;

			//调用执行器过滤器的前半截
			var stack = ExecutionUtility.InvokeFiltersExecuting(context.Executor.Filters, filter => this.OnFilterExecuting(filter, context));

			//执行管道集，并将其执行的最终结果保存到执行器上下文的Result属性中
			context.Result = this.Execute(context, pipelines);

			//调用执行器过滤器的后半截
			ExecutionUtility.InvokeFiltersExecuted(stack, filter => this.OnFilterExecuted(filter, context));

			//激发“Executed”事件
			this.OnExecuted(new ExecutedEventArgs(this, context.Parameter, context.Result));

			return context.Result;
		}

		protected virtual object Execute(IExecutorContext context, IEnumerable<ExecutionPipeline> pipelines)
		{
			//创建管道上下文集合
			var pipelineContexts = new ConcurrentBag<IExecutionPipelineContext>();

			var parallelling = System.Threading.Tasks.Parallel.ForEach(pipelines, pipeline =>
			{
				//如果当前执行管道的处理器为空，则忽略对当前管道的执行
				if(pipeline.Handler == null)
					return;

				//创建管道上下文对象
				var pipelineContext = this.CreatePipelineContext(context, pipeline);

				//根据当前管道上下文对象获取对应的调用器
				var invoker = this.GetPipelineInvoker(pipelineContext);

				//通过管道调用器来调用管道处理程序，如果调用成功则将管道上下文加入到上下文集合中
				if(invoker != null && invoker.Invoke(pipelineContext))
					pipelineContexts.Add(pipelineContext);
			});

			if(parallelling.IsCompleted)
			{
				//合并结果集，并将最终合并的结果设置到上下文的结果属性中
				context.Result = this.CombineResult(pipelineContexts);
			}

			return context.Result;
		}
		#endregion

		#region 虚拟方法
		protected virtual IExecutorContext CreateExecutorContext(object parameter)
		{
			return new ExecutorContext(this, parameter);
		}

		protected virtual IExecutionPipelineContext CreatePipelineContext(IExecutorContext context, ExecutionPipeline pipeline)
		{
			return new ExecutionPipelineContext(context, pipeline);
		}

		protected virtual IExecutionPipelineInvoker GetPipelineInvoker(IExecutionPipelineContext context)
		{
			return ExecutionPipelineInvoker.Default;
		}

		protected virtual void OnFilterExecuting(IExecutionFilter filter, IExecutorContext context)
		{
			filter.OnExecuting(context);
		}

		protected virtual void OnFilterExecuted(IExecutionFilter filter, IExecutorContext context)
		{
			filter.OnExecuted(context);
		}

		protected virtual IEnumerable<ExecutionPipeline> SelectPipelines(IExecutorContext context)
		{
			var selector = this.PipelineSelector;

			if(selector != null)
				return selector.SelectPipelines(context);

			return _pipelines.ToArray();
		}

		protected virtual object CombineResult(IEnumerable<IExecutionPipelineContext> contexts)
		{
			return ExecutionUtility.CombineResult(this.Combiner, contexts);
		}
		#endregion

		#region 激发事件
		protected virtual void OnExecuted(ExecutedEventArgs args)
		{
			var executedEvent = this.Executed;

			if(executedEvent != null)
				executedEvent(this, args);
		}

		protected virtual void OnExecuting(ExecutingEventArgs args)
		{
			var executingEvent = this.Executing;

			if(executingEvent != null)
				executingEvent(this, args);
		}
		#endregion
	}
}

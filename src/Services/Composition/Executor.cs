/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2010-2015 Zongsoft Corporation <http://www.zongsoft.com>
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

		public event EventHandler<ExecutionPipelineExecutedEventArgs> PipelineExecuted;
		public event EventHandler<ExecutionPipelineExecutingEventArgs> PipelineExecuting;

		public event EventHandler<ExecutionPipelineExecutedEventArgs> HandlerExecuted;
		public event EventHandler<ExecutionPipelineExecutingEventArgs> HandlerExecuting;
		#endregion

		#region 成员字段
		private object _host;
		private IExecutionCombiner _combiner;
		private IExecutionPipelineSelector _selector;
		private ExecutionPipelineCollection _pipelines;
		private ExecutionFilterCompositeCollection _filters;
		#endregion

		#region 构造函数
		public Executor()
		{
		}

		public Executor(object host)
		{
			_host = host;
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
		/// 获取或设置一个<see cref="IExecutionCombiner"/>结果合并器。
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
				if(_pipelines == null)
					System.Threading.Interlocked.CompareExchange(ref _pipelines, new ExecutionPipelineCollection(), null);

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
				if(_filters == null)
					System.Threading.Interlocked.CompareExchange(ref _filters, new ExecutionFilterCompositeCollection(), null);

				return _filters;
			}
		}
		#endregion

		#region 执行方法
		public ExecutionResult Execute(object parameter = null)
		{
			return this.Execute(this.CreateContext(parameter));
		}

		protected ExecutionResult Execute(ExecutionContext context)
		{
			if(context == null)
				throw new ArgumentNullException("context");

			//创建“Executing”事件的调用参数
			ExecutingEventArgs executingArgs = new ExecutingEventArgs(context);

			//激发“Executing”事件
			this.OnExecuting(executingArgs);

			if(executingArgs.Cancel)
				return new ExecutionResult(context);

			//通过选择器获取当前请求对应的管道集
			var pipelines = this.SelectPipelines(context, _pipelines);

			//如果当前执行的处理管道数为零则退出
			if(pipelines == null || pipelines.Count() < 1)
				return null;

			//调用执行器过滤器的前半截
			var stack = ExecutionUtility.InvokeFiltersExecuting(context.Executor.Filters, filter => this.OnFilterExecuting(filter, context));

			//执行管道集，并将其执行的最终结果保存到执行器上下文的Result属性中
			context.Result = this.InvokePipelines(pipelines, p => this.CreatePipelineContext(context, p, context.Parameter));

			//调用执行器过滤器的后半截
			ExecutionUtility.InvokeFiltersExecuted(stack, filter => this.OnFilterExecuted(filter, context));

			//激发“Executed”事件
			this.OnExecuted(new ExecutedEventArgs(context));

			return new ExecutionResult(context);
		}
		#endregion

		#region 虚拟方法
		protected virtual ExecutionContext CreateContext(object parameter)
		{
			return new ExecutionContext(this, parameter);
		}

		protected virtual object InvokePipelines(IEnumerable<ExecutionPipeline> pipelines, Func<ExecutionPipeline, ExecutionPipelineContext> createContext)
		{
			if(pipelines == null)
				return null;

			//创建管道上下文集合
			var pipelineContexts = new ConcurrentBag<IExecutionPipelineContext>();

			var parallelling = System.Threading.Tasks.Parallel.ForEach(pipelines, pipeline =>
			{
				//如果当前执行管道的处理器为空，则忽略对当前管道的执行
				if(pipeline.Handler == null)
					return;

				//创建管道上下文对象
				var pipelineContext = createContext(pipeline);

				//定义管道是否处理完成的标志
				var isHandled = false;

				try
				{
					//执行当前管道
					isHandled = this.InvokePipeline(pipelineContext);
				}
				catch(Exception ex)
				{
					pipelineContext.Exception = ex;
				}

				if(isHandled)
					pipelineContexts.Add(pipelineContext);
			});

			object result = null;

			if(parallelling.IsCompleted)
			{
				//合并结果集，并将最终合并的结果设置到上下文的结果属性中
				result = this.CombineResult(pipelineContexts);
			}

			return result;
		}

		protected virtual bool InvokePipeline(ExecutionPipelineContext context)
		{
			var pipeline = context.Pipeline;

			if(pipeline == null)
				return false;

			//计算当前管道的执行条件，如果管道的条件评估器不为空则使用管道的评估器进行验证，否则使用管道的处理程序的CanHandle方法进行验证
			var enabled = pipeline.Predication != null ? pipeline.Predication.Predicate(context) : pipeline.Handler.CanHandle(context);

			if(!enabled)
				return false;

			//激发“PipelineExecuting”事件
			this.OnPipelineExecuting(new ExecutionPipelineExecutingEventArgs(context));

			Stack<IExecutionFilter> stack = null;

			//调用当前管道过滤器的前半截
			if(pipeline.HasFilters)
				stack = ExecutionUtility.InvokeFiltersExecuting(pipeline.Filters, filter => this.OnFilterExecuting(filter, context));

			//激发“HandlerExecuting”事件
			this.OnHandlerExecuting(new ExecutionPipelineExecutingEventArgs(context));

			//调用管道处理器处理当前请求
			var isHandled = this.InvokeHandler(context);

			//设置是否处理成功的值到到上下文的扩展属性集中
			context.ExtendedProperties["__IsHandled__"] = isHandled;

			//激发“HandlerExecuted”事件
			this.OnHandlerExecuted(new ExecutionPipelineExecutedEventArgs(context));

			//调用后续管道集合
			if(context.HasChildren)
				context.Result = this.InvokePipelines(context.Children, p => this.CreatePipelineContext(context, p, context.Result));

			//调用当前管道过滤器的后半截
			if(stack != null && stack.Count > 0)
				ExecutionUtility.InvokeFiltersExecuted(stack, filter => this.OnFilterExecuted(filter, context));

			//激发“PipelineExecuted”事件
			this.OnPipelineExecuted(new ExecutionPipelineExecutedEventArgs(context));

			return isHandled;
		}

		protected virtual bool InvokeHandler(IExecutionPipelineContext context)
		{
			var handler = context.Pipeline.Handler;
			var canHandle = handler != null && handler.CanHandle(context);

			if(canHandle)
				handler.Handle(context);

			return canHandle;
		}

		protected virtual ExecutionPipelineContext CreatePipelineContext(ExecutionContext context, ExecutionPipeline pipeline, object parameter)
		{
			return new ExecutionPipelineContext(context, pipeline, parameter);
		}

		protected virtual object CombineResult(IEnumerable<IExecutionPipelineContext> contexts)
		{
			var combiner = this.Combiner;

			if(combiner != null)
				return combiner.Combine(contexts);
			else
				return ExecutionUtility.CombineResult(contexts);
		}

		protected virtual void OnFilterExecuting(IExecutionFilter filter, IExecutionContext context)
		{
			filter.OnExecuting(context);
		}

		protected virtual void OnFilterExecuted(IExecutionFilter filter, IExecutionContext context)
		{
			filter.OnExecuted(context);
		}

		protected virtual IEnumerable<ExecutionPipeline> SelectPipelines(IExecutionContext context, IEnumerable<ExecutionPipeline> pipelines)
		{
			var selector = this.PipelineSelector;

			if(selector != null)
				return selector.SelectPipelines(context, pipelines);

			return this.Pipelines;
		}
		#endregion

		#region 激发事件
		protected virtual void OnExecuted(ExecutedEventArgs args)
		{
			var executed = this.Executed;

			if(executed != null)
				executed(this, args);
		}

		protected virtual void OnExecuting(ExecutingEventArgs args)
		{
			var executing = this.Executing;

			if(executing != null)
				executing(this, args);
		}

		protected virtual void OnPipelineExecuted(ExecutionPipelineExecutedEventArgs args)
		{
			var executed = this.PipelineExecuted;

			if(executed != null)
				executed(this, args);
		}

		protected virtual void OnPipelineExecuting(ExecutionPipelineExecutingEventArgs args)
		{
			var executing = this.PipelineExecuting;

			if(executing != null)
				executing(this, args);
		}

		protected virtual void OnHandlerExecuted(ExecutionPipelineExecutedEventArgs args)
		{
			var executed = this.HandlerExecuted;

			if(executed != null)
				executed(this, args);
		}

		protected virtual void OnHandlerExecuting(ExecutionPipelineExecutingEventArgs args)
		{
			var executing = this.HandlerExecuting;

			if(executing != null)
				executing(this, args);
		}
		#endregion
	}
}

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
using System.Linq;

namespace Zongsoft.Services.Composition
{
	public class Executor
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
		private IExecutionSelector _selector;
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
		/// 获取或设置一个<see cref="IExecutionSelector"/>管道执行选择器。
		/// </summary>
		public IExecutionSelector Selector
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
		/// <remarks>
		///		<para>全局过滤器即表示，当执行器被执行时全局过滤器优先于管道自身的过滤器被执行。</para>
		/// </remarks>
		public ExecutionFilterCompositeCollection Filters
		{
			get
			{
				return _filters;
			}
		}
		#endregion

		#region 公共方法
		public object Execute()
		{
			return this.Execute((pipeline) => this.CreateContext(null, pipeline));
		}

		public object Execute(object parameter)
		{
			return this.Execute((pipeline) => this.CreateContext(parameter, pipeline));
		}

		public object Execute(Func<ExecutionPipeline, ExecutionContext> createContext)
		{
			if(_pipelines.Count < 1)
				return null;

			object parameter = createContext == null ? null : createContext(null).Parameter;
			ExecutingEventArgs executingArgs = new ExecutingEventArgs(this, parameter);

			//激发“Executing”事件
			this.OnExecuting(executingArgs);

			if(executingArgs.Cancel)
				return executingArgs.Result;

			//通过选择器获取当前请求对应的管道集
			var pipelines = this.GetPipelines(parameter);
			var contexts = new System.Collections.Concurrent.ConcurrentBag<ExecutionContext>();

			var parallelling = System.Threading.Tasks.Parallel.ForEach(pipelines, pipeline =>
			{
				//如果当前执行管道的处理器为空，则忽略对当前管道的执行
				if(pipeline.Handler == null)
					return;

				//创建当前执行管道的上下文对象
				ExecutionContext context = createContext != null ? createContext(pipeline) : new ExecutionContext(this, null, pipeline);

				//将当前管道执行上下文加入到执行器的对应的集合中
				contexts.Add(context);

				//计算当前管道的执行条件，如果管道的条件评估器不为空则使用管道的评估器进行验证，否则使用管道的处理程序的CanHandle方法进行验证
				var enabled = pipeline.Predication != null ? pipeline.Predication.Predicate(context) : pipeline.Handler.CanHandle(context);

				if(!enabled)
					return;

				//获取当前管道的所有过滤器(包含全局过滤器)
				var filters = this.GetFilters(pipeline);
				IList<IExecutionFilter> executableFilters = null;

				if(filters != null && filters.Count > 0)
				{
					var filteringContext = new ExecutionFilteringContext(context);
					executableFilters = new List<IExecutionFilter>(filters.Count);

					foreach(var filter in filters)
					{
						if(filter.Predication == null || filter.Predication.Predicate(context))
						{
							//调用过滤器的OnExecuting方法
							filter.Filter.OnExecuting(filteringContext);

							//如果过滤器返回取消后续方法，则不将当前过滤器加入到后续处理列表中
							if(!filteringContext.Cancel)
								executableFilters.Add(filter.Filter);

							//重置过滤器上下文对象中的Cancel属性值
							filteringContext.Cancel = false;
						}
					}
				}

				//调用处理器的CanHandle方法，以判断当前管道的处理程序能否执行
				var canHandle = pipeline.Handler.CanHandle(context);

				if(canHandle)
					pipeline.Handler.Handle(context);

				if(executableFilters != null && executableFilters.Count > 0)
				{
					var filteredContext = new ExecutionFilteredContext(context, canHandle);

					foreach(var filter in executableFilters)
						filter.OnExecuted(filteredContext);
				}
			});

			object result = null;

			if(parallelling.IsCompleted)
			{
				var combiner = _combiner;

				if(combiner != null)
					result = combiner.Combine(contexts.ToArray());
				else
					result = contexts.Where(p => p.Result != null).Select(ctx => ctx.Result);

				//激发“Executed”事件
				this.OnExecuted(new ExecutedEventArgs(this, parameter, result));
			}

			return result;
		}
		#endregion

		#region 虚拟方法
		protected virtual ExecutionContext CreateContext(object parameter, ExecutionPipeline pipeline)
		{
			return new ExecutionContext(this, parameter, pipeline);
		}
		#endregion

		#region 私有方法
		private IEnumerable<ExecutionPipeline> GetPipelines(object parameter)
		{
			var selector = _selector;

			if(selector == null)
				return _pipelines.ToArray();

			return selector.GetPipelines(new ExecutionSelectorContext(this, parameter));
		}

		private ICollection<ExecutionFilterComposite> GetFilters(ExecutionPipeline pipeline)
		{
			if(_filters.Count + pipeline.Filters.Count == 0)
				return null;

			var filters = new List<ExecutionFilterComposite>(_filters.Count + pipeline.Filters.Count);

			filters.AddRange(_filters);
			filters.AddRange(pipeline.Filters);

			return filters;
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zongsoft.Services.Composition
{
	public class ExecutionInvoker : IExecutionInvoker
	{
		#region 执行方法
		public bool Invoke(ExecutionPipelineContext context)
		{
			var pipeline = context.Pipeline;

			//计算当前管道的执行条件，如果管道的条件评估器不为空则使用管道的评估器进行验证，否则使用管道的处理程序的CanHandle方法进行验证
			var enabled = pipeline.Predication != null ? pipeline.Predication.Predicate(context) : pipeline.Handler.CanHandle(context);

			if(!enabled)
				return false;

			//获取当前管道的所有过滤器(包含全局过滤器)
			var filters = this.GetFilters(context);

			foreach(var filter in filters)
			{
				filter.OnExecuting(context);
			}

			//调用管道处理器处理当前请求
			var isHandled = this.InvokeHandler(context);

			//设置是否处理成功的值到到上下文的扩展属性集中
			context.ExtendedProperties["__IsHandled__"] = isHandled;

			foreach(var filter in filters)
			{
				filter.OnExecuting(context);
			}

			return isHandled;
		}
		#endregion

		#region 虚拟方法
		protected virtual bool InvokeHandler(ExecutionPipelineContext context)
		{
			var handler = context.Pipeline.Handler;
			var canHandle = handler != null && handler.CanHandle(context);

			if(canHandle)
				handler.Handle(context);

			return canHandle;
		}

		protected virtual IEnumerable<IExecutionFilter> GetFilters(ExecutionPipelineContext context)
		{
			var pipeline = context.Pipeline;
			var globalFilters = context.Executor.Filters;

			if(globalFilters.Count + pipeline.Filters.Count < 1)
				return null;

			var result = new List<IExecutionFilter>(globalFilters.Count + pipeline.Filters.Count);

			result.AddRange(globalFilters);
			result.AddRange(pipeline.Filters);

			return result;
		}
		#endregion
	}
}

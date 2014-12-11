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
		public bool Invoke(IExecutionPipelineContext context)
		{
			var pipeline = context.Pipeline;

			if(pipeline == null)
				return false;

			//计算当前管道的执行条件，如果管道的条件评估器不为空则使用管道的评估器进行验证，否则使用管道的处理程序的CanHandle方法进行验证
			var enabled = pipeline.Predication != null ? pipeline.Predication.Predicate(context) : pipeline.Handler.CanHandle(context);

			if(!enabled)
				return false;

			//调用当前管道过滤器的前半截
			var stack = Utility.InvokeFiltersExecuting(pipeline.Filters, filter => this.OnFilterExecuting(filter, context));

			//调用管道处理器处理当前请求
			var isHandled = this.InvokeHandler(context);

			//设置是否处理成功的值到到上下文的扩展属性集中
			context.ExtendedProperties[string.Format("__{0}.IsHandled__", pipeline.Name)] = isHandled;

			//如果当前管道还有后续管道，则调用后续管道
			if(pipeline.Next != null)
			{
				//切换当前上下文到下一个管道
				context.ToNext(isHandled);

				isHandled |= this.Invoke(context);
			}

			//调用当前管道过滤器的后半截
			Utility.InvokeFiltersExecuted(stack, filter => this.OnFilterExecuted(filter, context));

			return isHandled;
		}
		#endregion

		#region 虚拟方法
		protected virtual bool InvokeHandler(IExecutionPipelineContext context)
		{
			var handler = context.Pipeline.Handler;
			var canHandle = handler != null && handler.CanHandle(context);

			if(canHandle)
				handler.Handle(context);

			return canHandle;
		}

		protected virtual void OnFilterExecuting(IExecutionFilter filter, IExecutionPipelineContext context)
		{
			filter.OnExecuting(context);
		}

		protected virtual void OnFilterExecuted(IExecutionFilter filter, IExecutionPipelineContext context)
		{
			filter.OnExecuted(context);
		}
		#endregion
	}
}

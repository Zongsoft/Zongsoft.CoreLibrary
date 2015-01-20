using System;
using System.Collections.Generic;
using System.Linq;

namespace Zongsoft.Services.Composition
{
	internal static class ExecutionUtility
	{
		public static Stack<IExecutionFilter> InvokeFiltersExecuting(ICollection<IExecutionFilter> filters, Action<IExecutionFilter> invoke)
		{
			if(filters == null || filters.Count < 1)
				return null;

			//定义过滤器的调用栈
			var stack = new Stack<IExecutionFilter>(filters);

			foreach(var filter in filters)
			{
				if(filter == null)
					continue;

				invoke(filter);
				stack.Push(filter);
			}

			return stack;
		}

		public static void InvokeFiltersExecuted(Stack<IExecutionFilter> stack, Action<IExecutionFilter> invoke)
		{
			if(stack == null)
				return;

			while(stack.Count > 0)
			{
				var filter = stack.Pop();

				if(filter != null)
					invoke(filter);
			}
		}

		public static object CombineResult(IExecutionCombiner combiner, IEnumerable<IExecutionPipelineContext> contexts)
		{
			if(combiner != null)
				return combiner.Combine(contexts);

			var result = new List<object>(contexts.Count());

			foreach(var context in contexts)
			{
				if(context.Result != null)
					result.Add(context.Result);
			}

			if(result.Count == 0)
				return null;

			if(result.Count == 1)
				return result[0];

			return result.ToArray();
		}
	}
}

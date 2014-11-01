using System;
using System.Collections.Generic;

namespace Zongsoft.Services.Composition
{
	internal static class Utility
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
	}
}

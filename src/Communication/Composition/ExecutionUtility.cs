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
using System.Linq;

namespace Zongsoft.Communication.Composition
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

		public static object CombineResult(IEnumerable<IExecutionPipelineContext> contexts)
		{
			if(contexts == null)
				return null;

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

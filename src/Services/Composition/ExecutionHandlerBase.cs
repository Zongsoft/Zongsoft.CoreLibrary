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
using System.Text;

namespace Zongsoft.Services.Composition
{
	public abstract class ExecutionHandlerBase : CommandBase<IExecutionPipelineContext>, IExecutionHandler
	{
		#region 成员字段
		private ExecutionFilterCollection _filters;
		#endregion

		#region 构造函数
		protected ExecutionHandlerBase() : this(null)
		{
		}

		protected ExecutionHandlerBase(string name) : base(name, true)
		{
		}
		#endregion

		#region 公共属性
		public ExecutionFilterCollection Filters
		{
			get
			{
				if(_filters == null)
					System.Threading.Interlocked.CompareExchange(ref _filters, new ExecutionFilterCollection(), null);

				return _filters;
			}
		}
		#endregion

		#region 重写方法
		protected override void OnExecuting(CommandExecutingEventArgs e)
		{
			//处理过滤器的前趋动作
			this.InvokeFilters((IExecutionPipelineContext)e.Parameter, (filter, context) => filter.OnExecuting(context));

			//先执行过滤器，再激发处理器的“Executing”事件
			base.OnExecuting(e);

			if(e.Cancel)
			{
				//处理过滤器的后续动作
				this.InvokeFilters((IExecutionPipelineContext)e.Parameter, (filter, context) => filter.OnExecuted(context));
			}
		}

		protected override void OnExecuted(CommandExecutedEventArgs e)
		{
			//先激发处理器的“Executed”事件，再执行过滤器
			base.OnExecuted(e);

			//处理过滤器的后续动作
			this.InvokeFilters((IExecutionPipelineContext)e.Parameter, (filter, context) => filter.OnExecuted(context));
		}
		#endregion

		#region 显式实现
		bool IExecutionHandler.CanHandle(IExecutionPipelineContext context)
		{
			return this.CanExecute(context);
		}

		void IExecutionHandler.Handle(IExecutionPipelineContext context)
		{
			this.Execute(context);
		}
		#endregion

		#region 私有方法
		private void InvokeFilters(IExecutionPipelineContext context, Action<IExecutionFilter, IExecutionPipelineContext> invoke)
		{
			if(_filters == null || invoke == null)
				return;

			foreach(var filter in _filters)
			{
				invoke(filter, context);
			}
		}
		#endregion
	}
}

using System;
using System.Collections.Generic;

namespace Zongsoft.Services.Composition
{
	/// <summary>
	/// 提供执行处理程序的功能。
	/// </summary>
	public interface IExecutionHandler<TContext> : IExecutionHandler where TContext : ExecutionPipelineContext
	{
		/// <summary>
		/// 确认当前处理程序能否处理本次执行请求。
		/// </summary>
		/// <param name="context">当前执行上下文对象。</param>
		/// <returns>如果能处理本次执行请求则返回真(true)，否则返回假(false)。</returns>
		bool CanHandle(TContext context);

		/// <summary>
		/// 处理本次执行请求。
		/// </summary>
		/// <param name="context">当前执行上下文对象。</param>
		void Handle(TContext context);
	}
}

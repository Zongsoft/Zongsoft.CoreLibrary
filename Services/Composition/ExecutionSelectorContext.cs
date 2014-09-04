using System;
using System.Collections.Generic;

namespace Zongsoft.Services.Composition
{
	public class ExecutionSelectorContext
	{
		#region 成员字段
		private object _parameter;
		private Executor _executor;
		#endregion

		#region 构造函数
		public ExecutionSelectorContext(Executor executor, object parameter)
		{
			if(executor == null)
				throw new ArgumentNullException("executor");

			_executor = executor;
			_parameter = parameter;
		}
		#endregion

		#region 公共属性
		public Executor Executor
		{
			get
			{
				return _executor;
			}
		}

		public object Parameter
		{
			get
			{
				return _parameter;
			}
		}
		#endregion
	}
}


using System;
using System.Collections.Generic;

namespace Zongsoft.Services
{
	public interface ICommandExecutor
	{
		#region 声明事件
		event EventHandler<CommandExecutorExecutingEventArgs> Executing;
		event EventHandler<CommandExecutorExecutedEventArgs> Executed;
		#endregion

		CommandTreeNode Root
		{
			get;
		}

		object Execute(string commandPath, object parameter = null);
		CommandTreeNode Find(string commandPath);
	}
}

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

namespace Zongsoft.Services
{
	public class CommandExecutorBase : MarshalByRefObject
	{
		#region 声明事件
		public event EventHandler<CommandExecutorExecutingEventArgs> Executing;
		public event EventHandler<CommandExecutorExecutedEventArgs> Executed;
		#endregion

		#region 成员字段
		private readonly CommandTreeNode _root;
		#endregion

		#region 构造函数
		protected CommandExecutorBase()
		{
			_root = new CommandTreeNode();
		}
		#endregion

		#region 公共属性
		public CommandTreeNode Root
		{
			get
			{
				return _root;
			}
		}
		#endregion

		#region 查找方法
		public virtual CommandTreeNode Find(string path)
		{
			return _root.Find(path);
		}
		#endregion

		#region 执行方法
		public object Execute(string commandPath)
		{
			return this.Execute(commandPath, null);
		}

		public virtual object Execute(string commandPath, object parameter)
		{
			if(string.IsNullOrWhiteSpace(commandPath))
				throw new ArgumentNullException("commandPath");

			//查找指定路径的命令对象
			var commandNode = this.Find(commandPath);

			//如果指定的路径在命令树中是不存在的则抛出异常
			if(commandNode == null)
				throw new CommandNotFoundException(commandPath);

			return this.Execute(commandNode, parameter, commandPath);
		}

		private object Execute(CommandTreeNode commandNode, object parameter, string commandText)
		{
			//创建事件参数对象
			var executingArgs = new CommandExecutorExecutingEventArgs(this, commandText, parameter, commandNode);

			//激发“Executing”事件
			this.OnExecuting(executingArgs);

			if(executingArgs.Cancel)
				return executingArgs.Result;

			//获取应该执行的命令对象，因为在Executing事件中可能会更改待执行的命令对象
			var command = executingArgs.Command;

			//定义返回结果的变量
			var result = executingArgs.Result;

			//有可能找到的是空命令树节点，因此必须再判断对应的命令是否存在
			if(command != null)
				result = this.OnExecute(new CommandExecutorContext(this, commandNode, command, parameter));

			//创建事件参数对象
			var executedArgs = new CommandExecutorExecutedEventArgs(this, commandText, parameter, commandNode, command, result);

			//激发“Executed”事件
			this.OnExecuted(executedArgs);

			//返回最终的执行结果
			return executedArgs.Result;
		}
		#endregion

		#region 执行实践
		protected virtual object OnExecute(CommandExecutorContext context)
		{
			return context.Command.Execute(context.Parameter);
		}
		#endregion

		#region 激发事件
		protected virtual void OnExecuting(CommandExecutorExecutingEventArgs args)
		{
			var executing = this.Executing;

			if(executing != null)
				executing(this, args);
		}

		protected virtual void OnExecuted(CommandExecutorExecutedEventArgs args)
		{
			var executed = this.Executed;

			if(executed != null)
				executed(this, args);
		}
		#endregion
	}
}

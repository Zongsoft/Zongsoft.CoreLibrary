/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2010-2016 Zongsoft Corporation <http://www.zongsoft.com>
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

namespace Zongsoft.Services
{
	public class CommandExecutor : MarshalByRefObject, ICommandExecutor
	{
		#region 声明事件
		public event EventHandler<CommandExecutorExecutingEventArgs> Executing;
		public event EventHandler<CommandExecutorExecutedEventArgs> Executed;
		#endregion

		#region 静态字段
		private static CommandExecutor _default;
		#endregion

		#region 成员字段
		private readonly CommandTreeNode _root;
		private ICommandExpressionParser _parser;
		#endregion

		#region 构造函数
		public CommandExecutor(ICommandExpressionParser parser = null)
		{
			_root = new CommandTreeNode();
			_parser = parser ?? CommandExpressionParser.Instance;
		}
		#endregion

		#region 静态属性
		/// <summary>
		/// 获取或设置默认的<see cref="CommandExecutor"/>命令执行器。
		/// </summary>
		public static CommandExecutor Default
		{
			get
			{
				if(_default == null)
					System.Threading.Interlocked.CompareExchange(ref _default, new CommandExecutor(), null);

				return _default;
			}
			set
			{
				if(value == null)
					throw new ArgumentNullException();

				_default = value;
			}
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

		public ICommandExpressionParser Parser
		{
			get
			{
				return _parser;
			}
			set
			{
				if(value == null)
					throw new ArgumentNullException();

				_parser = value;
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
		public object Execute(string commandText, object parameter = null)
		{
			if(string.IsNullOrWhiteSpace(commandText))
				throw new ArgumentNullException(nameof(commandText));

			//创建命令执行器上下文对象
			var context = this.CreateExecutorContext(commandText, parameter);

			if(context == null)
				throw new InvalidOperationException("The context of this command executor is null.");

			//创建事件参数对象
			var executingArgs = new CommandExecutorExecutingEventArgs(context);

			//激发“Executing”事件
			this.OnExecuting(executingArgs);

			if(executingArgs.Cancel)
				return executingArgs.Result;

			//调用执行请求
			var result = this.OnExecute(context);

			//创建事件参数对象
			var executedArgs = new CommandExecutorExecutedEventArgs(context, result);

			//激发“Executed”事件
			this.OnExecuted(executedArgs);

			//返回最终的执行结果
			return executedArgs.Result;
		}
		#endregion

		#region 执行实现
		protected virtual object OnExecute(CommandExecutorContext context)
		{
			var queue = new Queue<Tuple<CommandExpression, CommandTreeNode>>();
			var expression = context.Expression;

			while(expression != null)
			{
				//查找指定路径的命令节点
				var node = this.Find(expression.FullPath);

				//如果指定的路径在命令树中是不存在的则抛出异常
				if(node == null)
					throw new CommandNotFoundException(expression.FullPath);

				//将找到的命令表达式和对应的节点加入队列中
				queue.Enqueue(new Tuple<CommandExpression, CommandTreeNode>(expression, node));

				//设置下一个待搜索的命令表达式
				expression = expression.Next;
			}

			//如果队列为空则返回空
			if(queue.Count < 1)
				return null;

			//初始化第一个输入参数
			var parameter = context.Parameter;

			while(queue.Count > 0)
			{
				var entry = queue.Dequeue();

				//执行队列中的命令
				parameter = this.ExecuteCommand(context, entry.Item1, entry.Item2, parameter);
			}

			//返回最后一个命令的执行结果
			return parameter;
		}

		protected virtual object ExecuteCommand(CommandExecutorContext context, CommandExpression expression, CommandTreeNode node, object parameter)
		{
			if(context == null)
				throw new ArgumentNullException(nameof(context));

			if(node == null)
				throw new ArgumentNullException(nameof(node));

			return node?.Command.Execute(this.CreateCommandContext(expression, node, parameter));
		}
		#endregion

		#region 保护方法
		protected virtual CommandExecutorContext CreateExecutorContext(string commandText, object parameter)
		{
			//解析当前命令文本
			var expression = this.OnParse(commandText);

			if(expression == null)
				throw new InvalidOperationException($"Invalid command expression text: {commandText}.");

			return new CommandExecutorContext(this, expression, parameter);
		}

		protected virtual CommandContext CreateCommandContext(CommandExpression expression, CommandTreeNode node, object parameter)
		{
			return new CommandContext(this, expression, node, parameter);
		}

		protected virtual CommandExpression OnParse(string text)
		{
			return _parser.Parse(text);
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

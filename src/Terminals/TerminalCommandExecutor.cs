/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2011-2016 Zongsoft Corporation <http://www.zongsoft.com>
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
using System.IO;

using Zongsoft.Services;

namespace Zongsoft.Terminals
{
	public class TerminalCommandExecutor : Zongsoft.Services.CommandExecutor
	{
		#region 事件声明
		public event EventHandler CurrentChanged;
		public event EventHandler<ExitEventArgs> Exit;
		#endregion

		#region 成员字段
		private ITerminal _terminal;
		private CommandTreeNode _current;
		#endregion

		#region 构造函数
		public TerminalCommandExecutor()
		{
			_terminal = new ConsoleTerminal(this);
		}

		public TerminalCommandExecutor(ITerminal terminal)
		{
			if(terminal == null)
				throw new ArgumentNullException(nameof(terminal));

			_terminal = terminal;
		}

		public TerminalCommandExecutor(ITerminal terminal, ICommandExpressionParser parser) : base(parser)
		{
			if(terminal == null)
				throw new ArgumentNullException(nameof(terminal));

			_terminal = terminal;
		}
		#endregion

		#region 公共属性
		public ITerminal Terminal
		{
			get
			{
				return _terminal;
			}
		}

		public CommandTreeNode Current
		{
			get
			{
				return _current;
			}
			private set
			{
				if(object.ReferenceEquals(_current, value))
					return;

				_current = value;

				//激发“CurrentChanged”事件
				this.OnCurrentChanged(EventArgs.Empty);
			}
		}

		public override ICommandOutlet Output
		{
			get
			{
				return _terminal;
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		public override TextWriter Error
		{
			get
			{
				return _terminal.Error;
			}
			set
			{
				_terminal.Error = value ?? TextWriter.Null;
			}
		}
		#endregion

		#region 运行方法
		public int Run()
		{
			if(this.Root.Children.Count < 1)
				return 0;

			while(true)
			{
				//重置控制台，准备执行命令
				_terminal.Reset();

				try
				{
					var commandText = _terminal.Input.ReadLine();

					if(!string.IsNullOrWhiteSpace(commandText))
						this.Execute(commandText);
				}
				catch(ExitException ex)
				{
					if(this.RaiseExit(ex.ExitCode))
						return ex.ExitCode;
				}
			}
		}
		#endregion

		#region 查找方法
		public override CommandTreeNode Find(string path)
		{
			if(string.IsNullOrWhiteSpace(path))
				return null;

			var current = _current;

			if(current == null)
				return base.Find(path);

			//如果查找的路径完全等于当前节点名，则优先返回当前节点
			if(string.Equals(path, current.Name, StringComparison.OrdinalIgnoreCase))
				return current;

			//以当前节点为锚点开始查找
			var node = current.Find(path);

			//如果以当前节点为锚点查找失败，并且查找的路径没有指定特定的锚点，则再从根节点查找一次并返回其查找结果
			if(node == null && path[0] != '.' && path[0] != '/')
				return this.Root.Find(path);

			return node;
		}
		#endregion

		#region 重写方法
		protected override CommandContext CreateCommandContext(CommandExecutorContext session, CommandExpression expression, CommandTreeNode node, object parameter)
		{
			return new TerminalCommandContext(session, expression, node, parameter);
		}

		protected override void OnExecuted(CommandExecutorExecutedEventArgs args)
		{
			var last = args.Context.Expression;

			//从执行器的命令表达式中找出最后一个命令表达式
			while(last != null && last.Next != null)
			{
				last = last.Next;
			}

			//查找表达式中最后一个命令节点
			var node = this.Find(last.FullPath);

			//更新当前命令节点，只有命令树节点不是叶子节点并且为空命令节点
			if(node != null && node.Children.Count > 0 && node.Command == null)
				this.Current = node;

			//调用基类同名方法
			base.OnExecuted(args);
		}

		protected override void OnFailed(CommandExecutorFailureEventArgs args)
		{
			//调用基类同名方法
			base.OnFailed(args);

			if(args.Exception is ExitException)
				args.Exception = null;
			else
			{
				args.ExceptionHandled = true;
				this.Terminal.WriteLine(CommandOutletColor.Red, args.Exception.Message);
			}
		}
		#endregion

		#region 激发事件
		private bool RaiseExit(int exitCode)
		{
			var args = new ExitEventArgs(exitCode);

			//激发“Exit”退出事件
			this.OnExit(args);

			return !args.Cancel;
		}

		protected virtual void OnExit(ExitEventArgs args)
		{
			this.Exit?.Invoke(this, args);
		}

		protected virtual void OnCurrentChanged(EventArgs args)
		{
			this.CurrentChanged?.Invoke(this, args);
		}
		#endregion

		#region 嵌套子类
		[Serializable]
		public class ExitException : ApplicationException
		{
			#region 成员变量
			private int _exitCode;
			#endregion

			#region 构造函数
			public ExitException()
			{
			}

			public ExitException(int exitCode)
			{
				_exitCode = exitCode;
			}
			#endregion

			#region 公共属性
			public int ExitCode
			{
				get
				{
					return _exitCode;
				}
			}
			#endregion
		}

		public class ExitEventArgs : EventArgs
		{
			#region 成员变量
			private bool _cancel;
			private int _exitCode;
			#endregion

			#region 构造函数
			public ExitEventArgs(int exitCode) : this(exitCode, false)
			{
			}

			public ExitEventArgs(int exitCode, bool cancel)
			{
				_cancel = cancel;
				_exitCode = exitCode;
			}
			#endregion

			#region 公共属性
			public bool Cancel
			{
				get
				{
					return _cancel;
				}
			}

			public int ExitCode
			{
				get
				{
					return _exitCode;
				}
			}
			#endregion
		}
		#endregion
	}
}

/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2011-2013 Zongsoft Corporation <http://www.zongsoft.com>
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
using System.Linq;
using System.Collections.Generic;

using Zongsoft.Diagnostics;
using Zongsoft.Resources;
using Zongsoft.Services;

namespace Zongsoft.Terminals
{
	public class TerminalCommandExecutor : Zongsoft.Services.CommandExecutor
	{
		#region 事件声明
		public event EventHandler CurrentChanged;
		public event EventHandler<ExitEventArgs> Exit;
		public event EventHandler<FailureEventArgs> Failed;
		#endregion

		#region 成员字段
		private ITerminal _terminal;
		private CommandTreeNode _current;
		#endregion

		#region 构造函数
		public TerminalCommandExecutor(ITerminal terminal) : this(terminal, null)
		{
		}

		public TerminalCommandExecutor(ITerminal terminal, ICommandLineParser parser)
		{
			if(terminal == null)
				throw new ArgumentNullException("terminal");

			_terminal = terminal;
			this.Parser = parser;
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
					//执行单行命令
					this.Execute(_terminal.Input.ReadLine());
				}
				catch(ExitException ex)
				{
					if(this.RaiseExit(ex.ExitCode))
						return ex.ExitCode;
				}
				catch(Exception ex)
				{
					this.OnFailed(new FailureEventArgs(ex));
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

			return current.Find(path);
		}
		#endregion

		#region 重写方法
		protected override CommandLine OnParse(string commandText)
		{
			switch(commandText.Trim())
			{
				case "/":
					return new CommandLine("/", null, null);
				case ".":
					return new CommandLine((_current ?? this.Root).FullPath, null, null);
				case "..":
					return new CommandLine((_current == null || _current.Parent == null ? this.Root : _current.Parent).FullPath, null, null);
			}

			return base.OnParse(commandText);
		}

		protected override void OnExecute(CommandExecutorContext context)
		{
			var command = context.Command;

			if(command != null)
				context.Result = command.Execute(new TerminalCommandContext(this, context.CommandLine, context.CommandNode, context.Parameter));
		}

		protected override void OnExecuted(CommandExecutorExecutedEventArgs args)
		{
			var commandNode = args.CommandNode;

			//更新当前命令节点，只有当前命令树节点不是叶子节点并且为空命令节点
			if(commandNode != null && commandNode.Children.Count > 0 && commandNode.Command == null)
				this.Current = commandNode;

			base.OnExecuted(args);
		}
		#endregion

		#region 激发事件
		protected virtual void OnCurrentChanged(EventArgs args)
		{
			var currentChanged = this.CurrentChanged;

			if(currentChanged != null)
				currentChanged(this, args);
		}

		private bool RaiseExit(int exitCode)
		{
			var args = new ExitEventArgs(exitCode);

			//激发“Exit”退出事件
			this.OnExit(args);

			return !args.Cancel;
		}

		protected virtual void OnExit(ExitEventArgs args)
		{
			var exit = this.Exit;

			if(exit != null)
				exit(this, args);
		}

		protected virtual void OnFailed(FailureEventArgs args)
		{
			var failed = this.Failed;

			if(failed != null)
				failed(this, args);
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

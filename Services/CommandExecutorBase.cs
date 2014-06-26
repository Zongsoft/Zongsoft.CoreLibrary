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
		#region 私有变量
		private int _isLoaded;
		#endregion

		#region 声明事件
		public event EventHandler CurrentChanged;
		public event EventHandler<CommandExecutingEventArgs> Executing;
		public event EventHandler<CommandExecutedEventArgs> Executed;
		#endregion

		#region 成员字段
		private readonly CommandTreeNode _root;
		private CommandTreeNode _current;
		private ICommandLoader _loader;
		#endregion

		#region 构造函数
		protected CommandExecutorBase()
		{
			_root = new CommandTreeNode("/", null);
		}
		#endregion

		#region 公共属性
		public ICommandLoader Loader
		{
			get
			{
				return _loader;
			}
			set
			{
				if(_isLoaded != 0)
					throw new InvalidOperationException("The CommandLoader was loaded.");

				_loader = value;
			}
		}

		public CommandTreeNode Root
		{
			get
			{
				return _root;
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

		#region 查找方法
		public ICommand Find(string path)
		{
			CommandTreeNode node;
			return this.Find(path, out node);
		}

		public ICommand Find(string path, out CommandTreeNode node)
		{
			node = (_current ?? _root).Find(path);

			if(node != null)
				return node.Command;

			return null;
		}
		#endregion

		#region 执行方法
		public virtual object Execute(string commandPath)
		{
			return this.Execute(commandPath, null);
		}

		protected object Execute(string commandPath, object parameter)
		{
			if(string.IsNullOrWhiteSpace(commandPath))
				throw new ArgumentNullException("commandPath");

			if(_loader != null)
			{
				//判断命令加载器是否加载过
				var isLoaded = System.Threading.Interlocked.CompareExchange(ref _isLoaded, 1, 0);

				//如果命令加载器从未加载过则调用加载器来加载命令
				if(isLoaded == 0 && _loader != null)
					_loader.Load(_root);
			}

			switch(commandPath.Trim())
			{
				case "/":
					return this.Execute(_root, parameter);
				case ".":
					return this.Execute(_current, parameter);
				case "..":
					if(_current == null || _current.Parent == null)
						return null;

					return this.Execute(_current.Parent, parameter);
			}

			CommandTreeNode commandNode;

			//查找指定路径的命令对象
			var command = this.Find(commandPath, out commandNode);

			//如果指定的路径在命令树中是不存在的则抛出异常
			if(commandNode == null)
				throw new CommandNotFoundException(commandPath);

			return this.Execute(commandNode, parameter);
		}

		protected object Execute(CommandTreeNode commandNode, object parameter)
		{
			if(commandNode == null)
				return null;

			//更新当前命令节点
			if(commandNode.Children.Count > 0)
				this.Current = commandNode;

			var command = commandNode.Command;

			//有可能找到的是空命令树节点，因此必须再判断对应的命令是否存在
			if(command == null)
				return null;

			command.Executing += new EventHandler<CommandExecutingEventArgs>(Command_Executing);
			command.Executed += new EventHandler<CommandExecutedEventArgs>(Command_Executed);

			//执行当前命令
			return this.OnExecute(new CommandExecutorContext(this, commandNode, command, parameter));
		}
		#endregion

		#region 执行实践
		protected virtual object OnExecute(CommandExecutorContext context)
		{
			return context.Command.Execute(context.Parameter);
		}
		#endregion

		#region 激发事件
		protected virtual void OnCurrentChanged(EventArgs args)
		{
			if(this.CurrentChanged != null)
				this.CurrentChanged(this, args);
		}

		protected virtual void OnExecuting(CommandExecutingEventArgs args)
		{
			if(this.Executing != null)
				this.Executing(this, args);
		}

		protected virtual void OnExecuted(CommandExecutedEventArgs args)
		{
			if(this.Executed != null)
				this.Executed(this, args);
		}
		#endregion

		#region 命令事件
		private void Command_Executing(object sender, CommandExecutingEventArgs e)
		{
			((ICommand)sender).Executing -= Command_Executing;

			//调用命令准备执行虚拟方法
			this.OnExecuting(e);
		}

		private void Command_Executed(object sender, CommandExecutedEventArgs e)
		{
			((ICommand)sender).Executed -= Command_Executed;

			//调用命令执行完成虚拟方法
			this.OnExecuted(e);
		}
		#endregion
	}
}

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

namespace Zongsoft.Services
{
	public class CommandTreeNode : Zongsoft.Collections.HierarchicalNode
	{
		#region 成员字段
		private ICommand _command;
		private ICommandLoader _loader;
		private CommandTreeNodeCollection _children;
		#endregion

		#region 构造函数
		public CommandTreeNode()
		{
			_children = new CommandTreeNodeCollection(this);
		}

		public CommandTreeNode(string name, CommandTreeNode parent = null) : base(name, parent)
		{
			_children = new CommandTreeNodeCollection(this);
		}

		public CommandTreeNode(ICommand command, CommandTreeNode parent = null) : base(command.Name, parent)
		{
			if(command == null)
				throw new ArgumentNullException("command");

			_command = command;
			_children = new CommandTreeNodeCollection(this);
		}
		#endregion

		#region 公共属性
		public ICommand Command
		{
			get
			{
				return _command;
			}
			set
			{
				_command = value;
			}
		}

		public ICommandLoader Loader
		{
			get
			{
				return _loader;
			}
			set
			{
				_loader = value;
			}
		}

		public CommandTreeNode Parent
		{
			get
			{
				return (CommandTreeNode)this.InnerParent;
			}
		}

		public CommandTreeNodeCollection Children
		{
			get
			{
				//确保当前加载器已经被加载过
				this.EnsureChildren();

				//返回子节点集
				return _children;
			}
		}
		#endregion

		#region 公共方法
		public CommandTreeNode Find(string path)
		{
			return (CommandTreeNode)base.FindNode(path);
		}

		public CommandTreeNode Find(string[] parts)
		{
			return (CommandTreeNode)base.FindNode(parts);
		}

		public CommandTreeNode Find(ICommand command)
		{
			if(command == null)
				return null;

			//确保当前加载器已经被加载过
			this.EnsureChildren();

			return this.FindDown(this, node => node._command == command);
		}

		public CommandTreeNode Find(Predicate<CommandTreeNode> predicate)
		{
			if(predicate == null)
				return null;

			//确保当前加载器已经被加载过
			this.EnsureChildren();

			return this.FindDown(this, predicate);
		}
		#endregion

		#region 重写方法
		protected override Collections.HierarchicalNode GetChild(string name)
		{
			return _children[name];
		}

		protected override void LoadChildren()
		{
			var loader = _loader;

			if(loader != null && (!loader.IsLoaded))
				loader.Load(this);
		}
		#endregion

		#region 私有方法
		private CommandTreeNode FindDown(CommandTreeNode current, Predicate<CommandTreeNode> predicate)
		{
			if(current == null || predicate == null)
				return null;

			if(predicate(current))
				return current;

			foreach(var child in current._children)
			{
				if(this.FindDown(child, predicate) != null)
					return child;
			}

			return null;
		}
		#endregion
	}
}

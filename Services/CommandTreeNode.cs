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
	public class CommandTreeNode : MarshalByRefObject
	{
		#region 成员字段
		private string _name;
		private ICommand _command;
		private ICommandLoader _loader;
		private CommandTreeNode _parent;
		private CommandTreeNodeCollection _children;
		#endregion

		#region 构造函数
		public CommandTreeNode(string name, CommandTreeNode parent)
		{
			if(string.IsNullOrWhiteSpace(name))
				throw new ArgumentNullException("name");

			_name = name;
			_parent = parent;
			_children = new CommandTreeNodeCollection(this);
		}

		public CommandTreeNode(ICommand command, CommandTreeNode parent)
		{
			_command = command;
			_parent = parent;
			_children = new CommandTreeNodeCollection(this);
		}
		#endregion

		#region 公共属性
		public string Name
		{
			get
			{
				return _command == null ? _name : _command.Name;
			}
		}

		public string FullPath
		{
			get
			{
				if(_parent == null)
					return this.Name;
				else
					return _parent.FullPath.Trim('/') + "/" + this.Name;
			}
		}

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
				return _parent;
			}
			internal set
			{
				_parent = value;
			}
		}

		public CommandTreeNodeCollection Children
		{
			get
			{
				//确保当前加载器已经被加载过
				this.EnsureLoaded();

				//返回子节点集
				return _children;
			}
		}
		#endregion

		#region 公共方法
		public CommandTreeNode Find(string path)
		{
			if(string.IsNullOrWhiteSpace(path))
				return null;

			return this.Find(path.Split('/'));
		}

		public CommandTreeNode Find(string[] parts)
		{
			if(parts == null || parts.Length == 0)
				return null;

			//确保当前加载器已经被加载过
			this.EnsureLoaded();

			var current = this;

			for(int i = 0; i < parts.Length; i++ )
			{
				var part = string.IsNullOrWhiteSpace(parts[i]) ? string.Empty : parts[i].Trim();

				if(i == 0 && part == string.Empty)
				{
					current = this.FindRoot();
				}
				else
				{
					switch(part)
					{
						case "":
						case ".":
							continue;
						case "..":
							current = current._parent;
							break;
						default:
							current = current._children[part];
							break;
					}

					if(current == null)
						return null;
				}
			}

			return current;
		}

		public CommandTreeNode Find(ICommand command)
		{
			if(command == null)
				return null;

			//确保当前加载器已经被加载过
			this.EnsureLoaded();

			return this.FindDown(this, node => node._command == command);
		}

		public CommandTreeNode Find(Predicate<CommandTreeNode> predicate)
		{
			if(predicate == null)
				return null;

			//确保当前加载器已经被加载过
			this.EnsureLoaded();

			return this.FindDown(this, predicate);
		}
		#endregion

		#region 私有方法
		private void EnsureLoaded()
		{
			var loader = _loader;

			if(loader != null && (!loader.IsLoaded))
				loader.Load(this);
		}

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

		private CommandTreeNode FindRoot()
		{
			var current = this;

			while(current != null)
			{
				if(current._parent == null)
					return current;

				current = current._parent;
			}

			return current;
		}
		#endregion
	}
}

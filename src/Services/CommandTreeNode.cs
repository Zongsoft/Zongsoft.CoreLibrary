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
using System.Text.RegularExpressions;

namespace Zongsoft.Services
{
	[System.ComponentModel.DefaultProperty("Children")]
	public class CommandTreeNode : Zongsoft.Collections.HierarchicalNode
	{
		#region 私有变量
		private readonly Regex _regex = new Regex(@"^\s*((?<prefix>/|\.{1,2})/?)?(\s*(?<part>[^\.\\/]+|\.{2})?\s*[/.]?\s*)*", RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace | RegexOptions.ExplicitCapture);
		#endregion

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

		public CommandTreeNode(string name) : base(name)
		{
			_children = new CommandTreeNodeCollection(this);
		}

		public CommandTreeNode(string name, CommandTreeNode parent) : base(name, parent)
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
		/// <summary>
		/// 查找指定的命令路径的命令节点。
		/// </summary>
		/// <param name="path">指定的命令路径。</param>
		/// <returns>返回查找的结果，如果为空则表示没有找到指定路径的<see cref="CommandTreeNode"/>命令节点。</returns>
		/// <remarks>
		///		<para>如果路径以斜杠(/)打头则从根节点开始查找；如果以双点(../)打头则表示从上级节点开始查找；否则从当前节点开始查找。</para>
		/// </remarks>
		public CommandTreeNode Find(string path)
		{
			if(string.IsNullOrWhiteSpace(path))
				return null;

			var match = _regex.Match(path);
			string[] parts = null;

			if(match.Success)
			{
				int offset = string.IsNullOrWhiteSpace(match.Groups["prefix"].Value) ? 0 : 1;

				if(offset == 0)
				{
					parts = new string[match.Groups["part"].Captures.Count];
				}
				else
				{
					parts = new string[match.Groups["part"].Captures.Count + 1];
					parts[0] = match.Groups["prefix"].Value;
				}

				for(int i = 0; i < match.Groups["part"].Captures.Count; i++)
				{
					parts[i + offset] = match.Groups["part"].Captures[i].Value;
				}
			}

			if(parts == null)
				parts = new string[] { path };

			return (CommandTreeNode)base.FindNode(parts, null);
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
		public override string ToString()
		{
			return this.FullPath;
		}

		protected override Collections.HierarchicalNode GetChild(string name)
		{
			if(_children != null && _children.TryGet(name, out var child))
				return child;

			return null;
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

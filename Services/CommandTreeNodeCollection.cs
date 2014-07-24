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
using System.Collections;
using System.Collections.Generic;

namespace Zongsoft.Services
{
	public class CommandTreeNodeCollection : Zongsoft.Collections.NamedCollectionBase<CommandTreeNode>
	{
		#region 成员字段
		private CommandTreeNode _owner;
		#endregion

		#region 构造函数
		public CommandTreeNodeCollection(CommandTreeNode owner)
		{
			if(owner == null)
				throw new ArgumentNullException("owner");

			_owner = owner;
		}
		#endregion

		#region 公共方法
		public CommandTreeNode Add(string name)
		{
			if(string.IsNullOrWhiteSpace(name))
				throw new ArgumentNullException("name");

			var node = new CommandTreeNode(name, _owner);
			this.Add(node);
			return node;
		}

		public CommandTreeNode Add(ICommand command)
		{
			if(command == null)
				throw new ArgumentNullException("command");

			var node = new CommandTreeNode(command, _owner);
			this.Add(node);
			return node;
		}
		#endregion

		#region 重写方法
		protected override string GetKeyForItem(CommandTreeNode item)
		{
			return item.Name;
		}

		protected override bool TryConvertItem(object value, out CommandTreeNode item)
		{
			if(value is ICommand)
			{
				item = new CommandTreeNode((ICommand)value, _owner);
				return true;
			}

			return base.TryConvertItem(value, out item);
		}

		protected override void RemoveItem(int index)
		{
			var item = this.Items[index];

			if(item != null)
				item.Parent = null;

			base.RemoveItem(index);
		}

		protected override void InsertItem(int index, CommandTreeNode item)
		{
			if(item.Parent != null && (!object.ReferenceEquals(item.Parent, _owner)))
				throw new InvalidOperationException();

			item.Parent = _owner;

			base.InsertItem(index, item);
		}

		protected override void SetItem(int index, CommandTreeNode item)
		{
			if(item.Parent != null && (!object.ReferenceEquals(item.Parent, _owner)))
				throw new InvalidOperationException();

			item.Parent = _owner;

			base.SetItem(index, item);
		}
		#endregion
	}
}

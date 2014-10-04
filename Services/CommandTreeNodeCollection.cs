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
	public class CommandTreeNodeCollection : Zongsoft.Collections.HierarchicalNodeCollection<CommandTreeNode>
	{
		#region 构造函数
		public CommandTreeNodeCollection(CommandTreeNode owner) : base(owner)
		{
		}
		#endregion

		#region 公共方法
		public CommandTreeNode Add(string name)
		{
			if(string.IsNullOrWhiteSpace(name))
				throw new ArgumentNullException("name");

			var node = new CommandTreeNode(name, this.Owner);
			this.Add(node);
			return node;
		}

		public CommandTreeNode Add(ICommand command)
		{
			if(command == null)
				throw new ArgumentNullException("command");

			var node = new CommandTreeNode(command, this.Owner);
			this.Add(node);
			return node;
		}
		#endregion

		#region 重写方法
		protected override bool TryConvertItem(object value, out CommandTreeNode item)
		{
			if(value is ICommand)
			{
				item = new CommandTreeNode((ICommand)value, this.Owner);
				return true;
			}

			return base.TryConvertItem(value, out item);
		}
		#endregion
	}
}

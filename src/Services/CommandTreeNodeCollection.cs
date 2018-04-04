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
	public class CommandTreeNodeCollection : Zongsoft.Collections.HierarchicalNodeCollection<CommandTreeNode>, ICollection<ICommand>
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

		public bool Contains(ICommand command)
		{
			if(command == null)
				return false;

			return this.ContainsName(command.Name);
		}

		public void CopyTo(ICommand[] array, int arrayIndex)
		{
			if(array == null)
				throw new ArgumentNullException(nameof(array));

			if(arrayIndex < 0 || arrayIndex >= array.Length)
				throw new ArgumentOutOfRangeException(nameof(arrayIndex));

			var iterator = base.GetEnumerator();

			for(int i = arrayIndex; i < array.Length; i++)
			{
				if(iterator.MoveNext())
					array[i] = iterator.Current?.Command;
			}
		}

		public bool Remove(ICommand command)
		{
			if(command == null)
				return false;

			return base.RemoveItem(command.Name);
		}
		#endregion

		#region 重写方法
		protected override string GetKeyForItem(CommandTreeNode item)
		{
			return item.Name;
		}
		#endregion

		#region 接口实现
		bool ICollection<ICommand>.IsReadOnly
		{
			get
			{
				return false;
			}
		}

		void ICollection<ICommand>.Add(ICommand item)
		{
			this.Add(item);
		}

		IEnumerator<ICommand> IEnumerable<ICommand>.GetEnumerator()
		{
			var iterator = base.GetEnumerator();

			while(iterator.MoveNext())
			{
				yield return iterator.Current?.Command;
			}
		}
		#endregion
	}
}

/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2013 Zongsoft Corporation <http://www.zongsoft.com>
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

namespace Zongsoft.Collections
{
	[Serializable]
	public class HierarchicalNodeCollection<T> : NamedCollectionBase<T> where T : HierarchicalNode
	{
		#region 成员变量
		private T _owner;
		#endregion

		#region 构造函数
		protected HierarchicalNodeCollection(T owner)
		{
			_owner = owner;
		}
		#endregion

		#region 保护属性
		protected T Owner
		{
			get
			{
				return _owner;
			}
		}
		#endregion

		#region 重写方法
		protected override string GetKeyForItem(T item)
		{
			return item.Name;
		}

		protected override void InsertItems(int index, IEnumerable<T> items)
		{
			foreach(var item in items)
			{
				item.InnerParent = _owner;
			}

			base.InsertItems(index, items);
		}

		protected override void SetItem(int index, T item)
		{
			var oldItem = this.Items[index];

			if(oldItem != null)
				oldItem.InnerParent = null;

			item.InnerParent = _owner;

			base.SetItem(index, item);
		}

		protected override void RemoveItem(int index)
		{
			var item = this.Items[index];

			if(item != null)
				item.InnerParent = null;

			base.RemoveItem(index);
		}

		protected override void ClearItems()
		{
			foreach(var item in this.Items)
			{
				if(item != null)
					item.InnerParent = null;
			}

			base.ClearItems();
		}
		#endregion
	}
}

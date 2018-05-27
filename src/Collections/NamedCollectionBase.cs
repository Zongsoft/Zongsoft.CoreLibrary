/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2013-2018 Zongsoft Corporation <http://www.zongsoft.com>
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

namespace Zongsoft.Collections
{
	public abstract class NamedCollectionBase<T> : ReadOnlyNamedCollectionBase<T>, ICollection<T>, INamedCollection<T>
	{
		#region 构造函数
		protected NamedCollectionBase()
		{
		}

		protected NamedCollectionBase(StringComparer comparer) : base(comparer)
		{
		}
		#endregion

		#region 公共属性
		public new T this[string name]
		{
			get
			{
				return this.GetItem(name);
			}
			set
			{
				this.SetItem(name, value);
			}
		}
		#endregion

		#region 公共方法
		public void Clear()
		{
			this.ClearItems();
		}

		public void Add(T item)
		{
			this.AddItem(item);
		}

		public bool Remove(string name)
		{
			return this.RemoveItem(name);
		}
		#endregion

		#region 虚拟方法
		protected virtual void ClearItems()
		{
			this.InnerDictionary.Clear();
		}

		protected virtual void SetItem(string name, T value)
		{
			var key = this.GetKeyForItem(value);
			var comparer = (StringComparer)this.InnerDictionary.Comparer;

			if(comparer.Compare(key, name) != 0)
				throw new InvalidOperationException("Specified name not equal to computed key of the item.");

			this.InnerDictionary[name] = value;
		}

		protected virtual void AddItem(T item)
		{
			this.InnerDictionary.Add(this.GetKeyForItem(item), item);
		}

		protected virtual bool RemoveItem(string name)
		{
			return this.InnerDictionary.Remove(name);
		}
		#endregion

		#region 显式实现
		bool ICollection<T>.IsReadOnly
		{
			get
			{
				return false;
			}
		}

		bool ICollection<T>.Contains(T item)
		{
			return this.InnerDictionary.ContainsKey(this.GetKeyForItem(item));
		}

		bool ICollection<T>.Remove(T item)
		{
			return this.InnerDictionary.Remove(this.GetKeyForItem(item));
		}
		#endregion
	}
}

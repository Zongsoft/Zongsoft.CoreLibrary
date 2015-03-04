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
using System.Collections;
using System.Collections.Generic;

namespace Zongsoft.Collections
{
	public abstract class NamedCollectionBase<T> : Collection<T>
	{
		#region 成员字段
		private StringComparer _comparer;
		private IDictionary<string, T> _innerDictionary;
		#endregion

		#region 构造函数
		protected NamedCollectionBase() : this(null)
		{
		}

		protected NamedCollectionBase(StringComparer comparer)
		{
			_comparer = comparer ?? StringComparer.OrdinalIgnoreCase;
			_innerDictionary = new Dictionary<string, T>(_comparer);
		}
		#endregion

		#region 公共属性
		public T this[string name]
		{
			get
			{
				name = name ?? string.Empty;

				T result;

				if(_innerDictionary.TryGetValue(name, out result))
					return result;

				return default(T);
			}
			set
			{
				name = name ?? string.Empty;

				T result;

				if(_innerDictionary.TryGetValue(name, out result))
				{
					_innerDictionary[name] = value;

					int index = this.Items.IndexOf(value);
					if(index >= 0)
						this.Items[index] = value;
					else
						this.Items.Add(value);
				}
				else
				{
					_innerDictionary.Add(name, value);
					this.Items.Add(value);
				}
			}
		}

		public IEnumerable<string> Keys
		{
			get
			{
				return _innerDictionary.Keys;
			}
		}

		public IEnumerable<T> Values
		{
			get
			{
				T[] array = new T[this.Count];
				base.Items.CopyTo(array, 0);
				return array;
			}
		}
		#endregion

		#region 公共方法
		public bool Contains(string name)
		{
			return _innerDictionary.ContainsKey(name ?? string.Empty);
		}

		public bool Remove(string name)
		{
			name = name ?? string.Empty;

			T value;

			if(_innerDictionary.TryGetValue(name, out value))
			{
				_innerDictionary.Remove(name);
				base.Remove(value);
				return true;
			}

			return false;
		}
		#endregion

		#region 重写方法
		protected override void ClearItems()
		{
			_innerDictionary.Clear();
			base.ClearItems();
		}

		protected override void InsertItems(int index, IEnumerable<T> items)
		{
			foreach(var item in items)
			{
				_innerDictionary.Add(this.GetKeyForItem(item), item);
			}

			base.InsertItems(index, items);
		}

		protected override void RemoveItem(int index)
		{
			string key = this.GetKeyForItem(this.Items[index]);
			_innerDictionary.Remove(key);
			base.RemoveItem(index);
		}

		protected override void SetItem(int index, T item)
		{
			var newKey = this.GetKeyForItem(item);
			var oldKey = this.GetKeyForItem(this.Items[index]);

			if(_comparer.Equals(oldKey, newKey))
			{
				_innerDictionary[newKey] = item;
			}
			else
			{
				_innerDictionary[newKey] = item;
				_innerDictionary.Remove(oldKey);
			}

			base.SetItem(index, item);
		}
		#endregion

		#region 抽象方法
		protected abstract string GetKeyForItem(T item);
		#endregion
	}
}

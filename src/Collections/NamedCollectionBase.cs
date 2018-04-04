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
	public abstract class NamedCollectionBase<T> : ICollection<T>, ICollection, INamedCollection<T>
	{
		#region 成员字段
		private readonly Dictionary<string, T> _innerDictionary;
		#endregion

		#region 构造函数
		public NamedCollectionBase()
		{
			_innerDictionary = new Dictionary<string, T>(StringComparer.OrdinalIgnoreCase);
		}

		public NamedCollectionBase(StringComparer comparer)
		{
			_innerDictionary = new Dictionary<string, T>(comparer ?? StringComparer.OrdinalIgnoreCase);
		}
		#endregion

		#region 公共属性
		public T this[string name]
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

		public int Count
		{
			get
			{
				return _innerDictionary.Count;
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
				return _innerDictionary.Values;
			}
		}
		#endregion

		#region 保护方法
		protected IDictionary<string, T> InnerDictionary
		{
			get
			{
				return _innerDictionary;
			}
		}
		#endregion

		#region 公共方法
		public void Clear()
		{
			this.ClearItems();
		}

		public bool Contains(string name)
		{
			return this.ContainsName(name);
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			if(array == null)
				throw new ArgumentNullException(nameof(array));

			if(arrayIndex < 0 || arrayIndex >= array.Length)
				throw new ArgumentOutOfRangeException(nameof(arrayIndex));

			var iterator = _innerDictionary.GetEnumerator();

			for(int i = arrayIndex; i < array.Length; i++)
			{
				if(iterator.MoveNext())
					array[i] = iterator.Current.Value;
			}
		}

		public T Get(string name)
		{
			return this.GetItem(name);
		}

		public bool TryGet(string name, out T value)
		{
			return this.TryGetItem(name, out value);
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
			_innerDictionary.Clear();
		}

		protected virtual bool ContainsName(string name)
		{
			return _innerDictionary.ContainsKey(name);
		}

		protected virtual T GetItem(string name)
		{
			T value;

			if(_innerDictionary.TryGetValue(name, out value))
				return value;

			throw new KeyNotFoundException();
		}

		protected virtual void SetItem(string name, T value)
		{
			var key = this.GetKeyForItem(value);
			var comparer = (StringComparer)_innerDictionary.Comparer;

			if(comparer.Compare(key, name) != 0)
				throw new InvalidOperationException("Specified name not equal to computed key of the item.");

			_innerDictionary[name] = value;
		}

		protected virtual bool TryGetItem(string name, out T value)
		{
			return _innerDictionary.TryGetValue(name, out value);
		}

		protected virtual void AddItem(T item)
		{
			_innerDictionary.Add(this.GetKeyForItem(item), item);
		}

		protected virtual bool RemoveItem(string name)
		{
			return _innerDictionary.Remove(name);
		}
		#endregion

		#region 抽象方法
		protected abstract string GetKeyForItem(T item);
		#endregion

		#region 显式实现
		bool ICollection<T>.IsReadOnly
		{
			get
			{
				return false;
			}
		}

		bool ICollection.IsSynchronized
		{
			get
			{
				return ((ICollection)_innerDictionary).IsSynchronized;
			}
		}

		object ICollection.SyncRoot
		{
			get
			{
				return ((ICollection)_innerDictionary).SyncRoot;
			}
		}

		void ICollection.CopyTo(Array array, int arrayIndex)
		{
			if(array == null)
				throw new ArgumentNullException(nameof(array));

			if(arrayIndex < 0 || arrayIndex >= array.Length)
				throw new ArgumentOutOfRangeException(nameof(arrayIndex));

			var iterator = _innerDictionary.GetEnumerator();

			for(int i = arrayIndex; i < array.Length; i++)
			{
				if(iterator.MoveNext())
					array.SetValue(iterator.Current.Value, i);
			}
		}

		bool ICollection<T>.Contains(T item)
		{
			return _innerDictionary.ContainsKey(this.GetKeyForItem(item));
		}

		bool ICollection<T>.Remove(T item)
		{
			return _innerDictionary.Remove(this.GetKeyForItem(item));
		}
		#endregion

		#region 遍历枚举
		IEnumerator IEnumerable.GetEnumerator()
		{
			foreach(var entry in _innerDictionary)
			{
				yield return entry.Value;
			}
		}

		public IEnumerator<T> GetEnumerator()
		{
			foreach(var entry in _innerDictionary)
			{
				yield return entry.Value;
			}
		}
		#endregion
	}
}

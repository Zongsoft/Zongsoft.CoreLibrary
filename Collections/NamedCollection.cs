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
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Zongsoft.Collections
{
	/// <summary>
	/// 该类已被作废，请使用<see cref="NamedCollectionBase"/>类。
	/// </summary>
	/// <typeparam name="T"></typeparam>
	[Obsolete]
	public class NamedCollection<T> : IDictionary<string, T>, ICollection<T>, ICollection
	{
		#region 同步锁定
		private readonly object _syncRoot = new object();
		#endregion

		#region 成员字段
		private IList<NamedCollectionEntry> _innerList;
		private IDictionary<string, NamedCollectionEntry> _innerDictionary;
		#endregion

		#region 构造函数
		public NamedCollection() : this(StringComparer.OrdinalIgnoreCase)
		{
		}

		public NamedCollection(IEqualityComparer<string> comparer)
		{
			_innerList = new List<NamedCollectionEntry>();
			_innerDictionary = new Dictionary<string, NamedCollectionEntry>(comparer ?? StringComparer.OrdinalIgnoreCase);
		}
		#endregion

		#region 公共属性
		public int Count
		{
			get
			{
				return _innerList.Count;
			}
		}

		public T this[int index]
		{
			get
			{
				return _innerList[index].Value;
			}
			set
			{
				_innerList[index].Value = value;
			}
		}

		public T this[string name]
		{
			get
			{
				NamedCollectionEntry entry;

				if(_innerDictionary.TryGetValue(name, out entry))
					return entry.Value;

				return default(T);
			}
			set
			{
				NamedCollectionEntry entry;

				if(_innerDictionary.TryGetValue(name, out entry))
					entry.Value = value;
				else
					this.Add(name, value);
			}
		}

		public ICollection<string> Keys
		{
			get
			{
				return _innerDictionary.Keys;
			}
		}

		public ICollection<T> Values
		{
			get
			{
				return _innerList.Select(entry => entry.Value).ToArray();
			}
		}
		#endregion

		#region 公共方法
		public void Clear()
		{
			lock(_syncRoot)
			{
				_innerList.Clear();
				_innerDictionary.Clear();
			}
		}

		public bool Contains(string name)
		{
			return _innerDictionary.ContainsKey(name);
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			lock(_syncRoot)
			{
				foreach(var entry in _innerList)
				{
					array[arrayIndex++] = entry.Value;
				}
			}
		}

		public void Add(T value)
		{
			lock(_syncRoot)
			{
				int count = _innerList.Count;
				var entry = new NamedCollectionEntry(count, null, value);

				_innerList.Add(entry);
			}
		}

		public void Add(string name, T value)
		{
			if(string.IsNullOrWhiteSpace(name))
				throw new ArgumentNullException("name");

			lock(_syncRoot)
			{
				int count = _innerList.Count;
				var entry = new NamedCollectionEntry(count, name, value);

				_innerList.Add(entry);
				_innerDictionary.Add(name, entry);
			}
		}

		public void Insert(T value, int index)
		{
			lock(_syncRoot)
			{
				if(index < 0 || index > _innerList.Count)
				{
					index = _innerList.Count;
				}
				else
				{
					for(int i = index; i < _innerList.Count; i++)
					{
						_innerList[i].Index++;
					}
				}

				var entry = new NamedCollectionEntry(index, null, value);
				_innerList.Insert(index, entry);
			}
		}

		public void Insert(T value, string position)
		{
			NamedCollectionEntry entry;

			if(_innerDictionary.TryGetValue(position, out entry))
				this.Insert(value, entry.Index);
			else
				throw new KeyNotFoundException();
		}

		public void Insert(string name, T value, int index)
		{
			if(string.IsNullOrWhiteSpace(name))
				throw new ArgumentNullException("name");

			lock(_syncRoot)
			{
				if(index < 0 || index > _innerList.Count)
				{
					index = _innerList.Count;
				}
				else
				{
					for(int i = index; i < _innerList.Count; i++)
					{
						_innerList[i].Index++;
					}
				}

				var entry = new NamedCollectionEntry(index, name, value);
				_innerList.Insert(index, entry);
			}
		}

		public void Insert(string name, T value, string position)
		{
			NamedCollectionEntry entry;

			if(_innerDictionary.TryGetValue(position, out entry))
				this.Insert(name, value, entry.Index);
			else
				throw new KeyNotFoundException();
		}

		public bool Remove(string name)
		{
			lock(_syncRoot)
			{
				NamedCollectionEntry entry;

				if(_innerDictionary.TryGetValue(name, out entry))
				{
					_innerList.RemoveAt(entry.Index);
					_innerDictionary.Remove(name);
					return true;
				}
			}

			return false;
		}

		public void RemoveAt(int index)
		{
			lock(_syncRoot)
			{
				var entry = _innerList[index];

				_innerList.RemoveAt(index);
				_innerDictionary.Remove(entry.Name);
			}
		}
		#endregion

		#region 嵌套子类
		private class NamedCollectionEntry
		{
			internal NamedCollectionEntry(int index, string name, T value)
			{
				this.Name = name;
				this.Index = index;
				this.Value = value;
			}

			internal string Name;
			internal int Index;
			internal T Value;
		}
		#endregion

		#region 显式实现
		bool IDictionary<string, T>.ContainsKey(string name)
		{
			return _innerDictionary.ContainsKey(name);
		}

		bool IDictionary<string, T>.TryGetValue(string name, out T value)
		{
			value = default(T);
			NamedCollectionEntry entry;

			if(_innerDictionary.TryGetValue(name, out entry))
			{
				value = entry.Value;
				return true;
			}

			return false;
		}

		object ICollection.SyncRoot
		{
			get
			{
				return _syncRoot;
			}
		}

		bool ICollection.IsSynchronized
		{
			get
			{
				return true;
			}
		}

		void ICollection.CopyTo(Array array, int index)
		{
			lock(_syncRoot)
			{
				foreach(var entry in _innerList)
				{
					array.SetValue(entry.Value, index++);
				}
			}
		}

		bool ICollection<T>.IsReadOnly
		{
			get
			{
				return false;
			}
		}

		bool ICollection<T>.Contains(T item)
		{
			lock(_syncRoot)
			{
				foreach(var entry in _innerList)
				{
					if(object.Equals(entry.Value, item))
						return true;
				}
			}

			return false;
		}

		bool ICollection<T>.Remove(T item)
		{
			lock(_syncRoot)
			{
				foreach(var entry in _innerList)
				{
					if(object.Equals(entry.Value, item))
					{
						if(!string.IsNullOrEmpty(entry.Name))
							_innerDictionary.Remove(entry.Name);

						_innerList.Remove(entry);
						return true;
					}
				}
			}

			return false;
		}

		bool ICollection<KeyValuePair<string, T>>.IsReadOnly
		{
			get
			{
				return false;
			}
		}

		bool ICollection<KeyValuePair<string, T>>.Contains(KeyValuePair<string, T> item)
		{
			return _innerDictionary.ContainsKey(item.Key);
		}

		void ICollection<KeyValuePair<string, T>>.CopyTo(KeyValuePair<string, T>[] array, int arrayIndex)
		{
			foreach(var pair in _innerDictionary)
			{
				array[arrayIndex++] = new KeyValuePair<string, T>(pair.Key, pair.Value.Value);
			}
		}

		void ICollection<KeyValuePair<string, T>>.Add(KeyValuePair<string, T> item)
		{
			this.Add(item.Key, item.Value);
		}

		bool ICollection<KeyValuePair<string, T>>.Remove(KeyValuePair<string, T> item)
		{
			return this.Remove(item.Key);
		}
		#endregion

		#region 枚举遍历
		public IEnumerator<T> GetEnumerator()
		{
			foreach(var entry in _innerList)
			{
				yield return entry.Value;
			}
		}

		IEnumerator<KeyValuePair<string, T>> IEnumerable<KeyValuePair<string, T>>.GetEnumerator()
		{
			foreach(var pair in _innerDictionary)
			{
				yield return new KeyValuePair<string, T>(pair.Key, pair.Value.Value);
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}
		#endregion
	}
}

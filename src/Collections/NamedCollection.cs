/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2014 Zongsoft Corporation <http://www.zongsoft.com>
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
using System.Collections.Specialized;
using System.Linq;

namespace Zongsoft.Collections
{
	public class NamedCollection<T> : ICollection<T>, IDictionary<string, T>
	{
		#region 私有变量
		private Func<T, string> _getKeyForItem;
		private Func<object, bool> _onItemMatch;
		#endregion

		#region 成员字段
		private StringComparer _comparer;
		private IList _items;
		private Dictionary<string, T> _innerDictionary;
		#endregion

		#region 构造函数
		protected NamedCollection(IList items, StringComparer comparer = null)
		{
			this.Initialize(items, comparer);
		}

		public NamedCollection(IList items, Func<T, string> getKeyForItem, Func<object, bool> onItemMatch = null, StringComparer comparer = null)
		{
			if(getKeyForItem == null)
				throw new ArgumentNullException("getKeyForItem");

			_getKeyForItem = getKeyForItem;
			_onItemMatch = onItemMatch;

			//在初始化之前必须先为委托赋值
			this.Initialize(items, comparer);
		}
		#endregion

		#region 初始化器
		private void Initialize(IList items, StringComparer comparer = null)
		{
			if(items == null)
				throw new ArgumentNullException("items");

			_items = items;
			_comparer = comparer ?? StringComparer.OrdinalIgnoreCase;

			if(items is INotifyCollectionChanged)
			{
				_innerDictionary = new Dictionary<string, T>(items.Count, _comparer);

				foreach(var item in items)
				{
					if(this.OnItemMatch(item))
						_innerDictionary.Add(this.GetKeyForItem((T)item), (T)item);
				}

				((INotifyCollectionChanged)items).CollectionChanged += Items_CollectionChanged;
			}
		}
		#endregion

		#region 公共属性
		public T this[int index]
		{
			get
			{
				int i = 0;

				foreach(var item in _items)
				{
					if(this.OnItemMatch(item) && index == i++)
						return (T)item;
				}

				return default(T);
			}
		}

		public T this[string name]
		{
			get
			{
				T result;

				if(_innerDictionary == null)
				{
					foreach(var item in _items)
					{
						if(this.OnItemMatch(item) && _comparer.Compare(this.GetKeyForItem((T)item), name) == 0)
							return (T)item;
					}
				}
				else
				{
					if(_innerDictionary.TryGetValue(name, out result))
						return result;
				}

				return default(T);
			}
		}

		public int Count
		{
			get
			{
				if(_innerDictionary == null)
				{
					int count = 0;

					foreach(var item in _items)
					{
						if(this.OnItemMatch(item))
							count++;
					}

					return count;
				}

				return _innerDictionary.Count;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return _items.IsReadOnly;
			}
		}
		#endregion

		#region 公共方法
		public void Add(T item)
		{
			if(item == null)
				throw new ArgumentNullException("item");

			_items.Add(item);
		}

		public void Clear()
		{
			if(_innerDictionary == null)
			{
				var removedItems = new List<T>();

				foreach(var item in _items)
				{
					if(this.OnItemMatch(item))
						removedItems.Add((T)item);
				}

				foreach(var item in removedItems)
				{
					_items.Remove(item);
				}
			}
			else
			{
				foreach(var value in _innerDictionary.Values)
				{
					_items.Remove(value);
				}
			}
		}

		public bool Remove(string name)
		{
			if(_innerDictionary == null)
			{
				foreach(var item in _items)
				{
					if(this.OnItemMatch(item) && _comparer.Compare(this.GetKeyForItem((T)item), name) == 0)
					{
						_items.Remove(item);
						return true;
					}
				}

				return false;
			}
			else
			{
				T item;

				if(_innerDictionary.TryGetValue(name, out item))
				{
					_items.Remove(item);
					return true;
				}
			}

			return false;
		}

		public bool Remove(T item)
		{
			var result = _items.Contains(item);
			_items.Remove(item);
			return result;
		}

		public bool Contains(string name)
		{
			if(string.IsNullOrWhiteSpace(name))
				return false;

			if(_innerDictionary == null)
			{
				foreach(var item in _items)
				{
					if(this.OnItemMatch(item) && _comparer.Compare(this.GetKeyForItem((T)item), name) == 0)
						return true;
				}

				return false;
			}

			return _innerDictionary.ContainsKey(name);
		}

		public bool Contains(T item)
		{
			if(item == null)
				return false;

			if(_innerDictionary == null)
			{
				return _items.Contains(item);
			}
			else
			{
				T entry;

				if(_innerDictionary.TryGetValue(this.GetKeyForItem(item), out entry))
					return object.ReferenceEquals(item, entry);
			}

			return false;
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			if(array == null)
				return;

			if(arrayIndex >= array.Length)
				throw new ArgumentOutOfRangeException("arrayIndex");

			int index = 0;

			if(_innerDictionary == null)
			{
				foreach(var item in _items)
				{
					if(arrayIndex + index >= array.Length)
						return;

					if(this.OnItemMatch(item))
						array[arrayIndex + index++] = (T)item;
				}
			}
			else
			{
				foreach(var item in _innerDictionary.Values)
				{
					if(arrayIndex + index >= array.Length)
						return;

					array[arrayIndex + index++] = item;
				}
			}
		}
		#endregion

		#region 虚拟方法
		protected virtual string GetKeyForItem(T item)
		{
			var thunk = _getKeyForItem;

			if(thunk == null)
				throw new NotImplementedException();

			return thunk(item);
		}

		/// <summary>
		/// 当需要过滤集合元素时调用此方法被调用。
		/// </summary>
		/// <param name="item">指定要匹配的集合元素。</param>
		/// <returns>如果匹配成功则返回真(true)，否则返回假(false)。</returns>
		/// <remarks>
		///		<para>对实现者的要求：当该方法返回真(true)，则必须确保参数<paramref name="item"/>是可被直接转换为<typeparamref name="T"/>类型的。</para>
		/// </remarks>
		protected virtual bool OnItemMatch(object item)
		{
			var thunk = _onItemMatch;

			if(thunk == null)
				return item is T;

			return thunk(item);
		}
		#endregion

		#region 集合事件
		private void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			switch(e.Action)
			{
				case NotifyCollectionChangedAction.Add:
					foreach(var item in e.NewItems)
					{
						if(this.OnItemMatch(item))
						{
							_innerDictionary.Add(this.GetKeyForItem((T)item), (T)item);
						}
					}
					break;
				case NotifyCollectionChangedAction.Remove:
					foreach(var item in e.OldItems)
					{
						if(this.OnItemMatch(item))
						{
							_innerDictionary.Remove(this.GetKeyForItem((T)item));
						}
					}
					break;
				case NotifyCollectionChangedAction.Replace:
					foreach(var item in e.OldItems)
					{
						if(this.OnItemMatch(item))
						{
							_innerDictionary.Remove(this.GetKeyForItem((T)item));
						}
					}

					foreach(var item in e.NewItems)
					{
						if(this.OnItemMatch(item))
						{
							_innerDictionary.Add(this.GetKeyForItem((T)item), (T)item);
						}
					}
					break;
				case NotifyCollectionChangedAction.Reset:
					_innerDictionary.Clear();
					break;
			}
		}
		#endregion

		#region 遍历枚举
		public IEnumerator<T> GetEnumerator()
		{
			if(_innerDictionary == null)
			{
				foreach(var item in _items)
				{
					if(this.OnItemMatch(item))
						yield return (T)item;
				}
			}
			else
			{
				foreach(var item in _innerDictionary.Values)
					yield return item;
			}
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}
		#endregion

		#region 显式实现
		void IDictionary<string, T>.Add(string key, T value)
		{
			throw new NotSupportedException();
		}

		bool IDictionary<string, T>.ContainsKey(string key)
		{
			return this.Contains(key);
		}

		ICollection<string> IDictionary<string, T>.Keys
		{
			get
			{
				if(_innerDictionary != null)
					return _innerDictionary.Keys;

				var keys = new string[_items.Count];
				int index = 0;

				foreach(T item in _items)
					keys[index++] = this.GetKeyForItem(item);

				return keys;
			}
		}

		bool IDictionary<string, T>.TryGetValue(string key, out T value)
		{
			var result = this.Contains(key);

			if(result)
				value = this[key];
			else
				value = default(T);

			return result;
		}

		ICollection<T> IDictionary<string, T>.Values
		{
			get
			{
				if(_innerDictionary != null)
					return _innerDictionary.Values;

				var values = new T[_items.Count];
				int index = 0;

				foreach(T item in _items)
					values[index++] = item;

				return values;
			}
		}

		T IDictionary<string, T>.this[string key]
		{
			get
			{
				return this[key];
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		void ICollection<KeyValuePair<string, T>>.Add(KeyValuePair<string, T> item)
		{
			this.Add(item.Value);
		}

		bool ICollection<KeyValuePair<string, T>>.Contains(KeyValuePair<string, T> item)
		{
			return this.Contains(item.Key);
		}

		bool ICollection<KeyValuePair<string, T>>.Remove(KeyValuePair<string, T> item)
		{
			return this.Remove(item.Key);
		}

		void ICollection<KeyValuePair<string, T>>.CopyTo(KeyValuePair<string, T>[] array, int arrayIndex)
		{
			throw new NotImplementedException();
		}

		IEnumerator<KeyValuePair<string, T>> IEnumerable<KeyValuePair<string, T>>.GetEnumerator()
		{
			if(_innerDictionary == null)
			{
				foreach(var item in _items)
				{
					if(this.OnItemMatch(item))
						yield return new KeyValuePair<string, T>(this.GetKeyForItem((T)item), (T)item);
				}
			}
			else
			{
				foreach(var item in _innerDictionary)
					yield return item;
			}
		}
		#endregion
	}
}

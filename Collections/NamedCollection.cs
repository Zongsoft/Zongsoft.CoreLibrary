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
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace Zongsoft.Collections
{
	public class NamedCollection<T, TBase> : ICollection<T> where T : TBase
	{
		#region 私有变量
		private Func<T, string> _getKeyForItem;
		private Func<TBase, bool> _onItemMatch;
		#endregion

		#region 成员字段
		private StringComparer _comparer;
		private ICollection<TBase> _items;
		private Dictionary<string, T> _innerDictionary;
		#endregion

		#region 构造函数
		protected NamedCollection(ICollection<TBase> items, StringComparer comparer = null)
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

		public NamedCollection(ICollection<TBase> items, Func<T, string> getKeyForItem, Func<TBase, bool> onItemMatch = null, StringComparer comparer = null) : this(items, comparer)
		{
			if(getKeyForItem == null)
				throw new ArgumentNullException("getKeyForItem");

			_getKeyForItem = getKeyForItem;
			_onItemMatch = onItemMatch;
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
					foreach(TBase item in _items)
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
					return _items.Count(item => this.OnItemMatch(item));
				else
					return _innerDictionary.Count;
			}
		}

		bool ICollection<T>.IsReadOnly
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
			_items.Clear();
		}

		public bool Remove(T item)
		{
			return _items.Remove(item);
		}

		public bool Contains(string name)
		{
			if(string.IsNullOrWhiteSpace(name))
				return false;

			if(_innerDictionary == null)
				return _items.Any(item => this.OnItemMatch(item) && _comparer.Compare(this.GetKeyForItem((T)item), name) == 0);
			else
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
				foreach(TBase item in _items)
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
		protected virtual bool OnItemMatch(TBase item)
		{
			var thunk = _onItemMatch;

			if(thunk == null)
				return true;

			return thunk(item);
		}
		#endregion

		#region 集合事件
		private void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			switch(e.Action)
			{
				case NotifyCollectionChangedAction.Add:
					foreach(TBase item in e.NewItems)
					{
						if(this.OnItemMatch(item))
						{
							_innerDictionary.Add(this.GetKeyForItem((T)item), (T)item);
						}
					}
					break;
				case NotifyCollectionChangedAction.Remove:
					foreach(TBase item in e.OldItems)
					{
						if(this.OnItemMatch(item))
						{
							_innerDictionary.Remove(this.GetKeyForItem((T)item));
						}
					}
					break;
				case NotifyCollectionChangedAction.Replace:
					foreach(TBase item in e.OldItems)
					{
						if(this.OnItemMatch(item))
						{
							_innerDictionary.Remove(this.GetKeyForItem((T)item));
						}
					}

					foreach(TBase item in e.NewItems)
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
				foreach(TBase item in _items)
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
	}
}

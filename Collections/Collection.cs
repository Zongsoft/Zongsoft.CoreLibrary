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
	public class Collection<T> : IList<T>, IList
	{
		#region 成员字段
		private object _syncRoot;
		private IList<T> _items;
		#endregion

		#region 构造函数
		public Collection()
		{
			_items = new List<T>();
		}

		public Collection(IEnumerable<T> items)
		{
			if(items == null)
				_items = new List<T>();
			else
				_items = new List<T>(items);
		}
		#endregion

		#region 公共属性
		public int Count
		{
			get
			{
				return _items.Count;
			}
		}

		public T this[int index]
		{
			get
			{
				return _items[index];
			}
			set
			{
				if(_items.IsReadOnly)
					throw new NotSupportedException();

				if(index < 0 || index >= _items.Count)
					throw new ArgumentOutOfRangeException("index");

				this.SetItem(index, value);
			}
		}
		#endregion

		#region 保护属性
		protected IList<T> Items
		{
			get
			{
				return _items;
			}
		}
		#endregion

		#region 公共方法
		public void Add(T item)
		{
			if(_items.IsReadOnly)
				throw new NotSupportedException();

			int index = _items.Count;
			this.InsertItem(index, item);
		}

		public virtual void AddRange(IEnumerable<T> items)
		{
			if(_items.IsReadOnly)
				throw new NotSupportedException();

			if(items == null)
				return;

			foreach(var item in _items)
			{
				_items.Add(item);
			}
		}

		public void Clear()
		{
			if(_items.IsReadOnly)
				throw new NotSupportedException();

			this.ClearItems();
		}

		public void CopyTo(T[] array, int index)
		{
			_items.CopyTo(array, index);
		}

		public bool Contains(T item)
		{
			return _items.Contains(item);
		}

		public int IndexOf(T item)
		{
			return _items.IndexOf(item);
		}

		public void Insert(int index, T item)
		{
			if(_items.IsReadOnly)
				throw new NotSupportedException();

			if(index < 0 || index > _items.Count)
				throw new ArgumentOutOfRangeException();

			this.InsertItem(index, item);
		}

		public bool Remove(T item)
		{
			if(_items.IsReadOnly)
				throw new NotSupportedException();

			int index = _items.IndexOf(item);
			if(index < 0)
				return false;

			this.RemoveItem(index);
			return true;
		}

		public void RemoveAt(int index)
		{
			if(_items.IsReadOnly)
				throw new NotSupportedException();

			if(index < 0 || index >= _items.Count)
				throw new ArgumentOutOfRangeException();

			this.RemoveItem(index);
		}
		#endregion

		#region 虚拟方法
		protected virtual void ClearItems()
		{
			_items.Clear();
		}

		protected virtual void InsertItem(int index, T item)
		{
			_items.Insert(index, item);
		}

		protected virtual void RemoveItem(int index)
		{
			_items.RemoveAt(index);
		}

		protected virtual void SetItem(int index, T item)
		{
			_items[index] = item;
		}

		protected virtual bool TryConvertItem(object value, out T item)
		{
			return Zongsoft.Common.Convert.TryConvertValue<T>(value, out item);
		}
		#endregion

		#region 显式实现
		bool ICollection.IsSynchronized
		{
			get
			{
				return false;
			}
		}

		object ICollection.SyncRoot
		{
			get
			{
				if(_syncRoot == null)
				{
					ICollection c = _items as ICollection;

					if(c != null)
						_syncRoot = c.SyncRoot;
					else
						System.Threading.Interlocked.CompareExchange<Object>(ref _syncRoot, new Object(), null);
				}

				return _syncRoot;
			}
		}

		void ICollection.CopyTo(Array array, int index)
		{
			if(array == null)
				throw new ArgumentNullException("array");

			if(array.Rank != 1)
				throw new ArgumentException();

			if(array.GetLowerBound(0) != 0)
				throw new ArgumentException();

			if(index < 0 || array.Length - index < this.Count)
				throw new ArgumentOutOfRangeException("index");

			T[] tArray = array as T[];
			if(tArray != null)
			{
				_items.CopyTo(tArray, index);
			}
			else
			{
				Type targetType = array.GetType().GetElementType();
				Type sourceType = typeof(T);

				if(!(targetType.IsAssignableFrom(sourceType) || sourceType.IsAssignableFrom(targetType)))
					throw new ArgumentException();

				object[] objects = array as object[];
				if(objects == null)
					throw new ArgumentException();

				int count = _items.Count;

				for(int i = 0; i < count; i++)
				{
					objects[index++] = _items[i];
				}
			}
		}

		bool ICollection<T>.IsReadOnly
		{
			get
			{
				return _items.IsReadOnly;
			}
		}

		object IList.this[int index]
		{
			get
			{
				return _items[index];
			}
			set
			{
				if(value == null)
					throw new ArgumentNullException();

				T result;

				if(this.TryConvertItem(value, out result))
					this.SetItem(index, result);
				else
					throw new InvalidCastException();
			}
		}

		bool IList.IsReadOnly
		{
			get
			{
				return _items.IsReadOnly;
			}
		}

		bool IList.IsFixedSize
		{
			get
			{
				IList list = _items as IList;

				if(list != null)
					return list.IsFixedSize;

				return _items.IsReadOnly;
			}
		}

		int IList.Add(object value)
		{
			if(_items.IsReadOnly)
				throw new NotSupportedException();

			if(value == null)
				throw new ArgumentNullException();

			T result;

			if(this.TryConvertItem(value, out result))
				this.Add(result);
			else
				throw new InvalidCastException();

			return this.Count - 1;
		}

		bool IList.Contains(object value)
		{
			T result;

			if(this.TryConvertItem(value, out result))
				return Contains(result);

			return false;
		}

		int IList.IndexOf(object value)
		{
			T result;

			if(this.TryConvertItem(value, out result))
				return IndexOf(result);

			return -1;
		}

		void IList.Insert(int index, object value)
		{
			if(_items.IsReadOnly)
				throw new NotSupportedException();

			if(value == null)
				throw new ArgumentNullException();

			T result;

			if(this.TryConvertItem(value, out result))
				this.Insert(index, result);

			throw new InvalidCastException();
		}

		void IList.Remove(object value)
		{
			if(_items.IsReadOnly)
				throw new NotSupportedException();

			T result;

			if(this.TryConvertItem(value, out result))
				this.Remove(result);
		}
		#endregion

		#region 枚举遍历
		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable)_items).GetEnumerator();
		}

		public IEnumerator<T> GetEnumerator()
		{
			return _items.GetEnumerator();
		}
		#endregion
	}
}

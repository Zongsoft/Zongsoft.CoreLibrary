/*
 *   _____                                ______
 *  /_   /  ____  ____  ____  _________  / __/ /_
 *    / /  / __ \/ __ \/ __ \/ ___/ __ \/ /_/ __/
 *   / /__/ /_/ / / / / /_/ /\_ \/ /_/ / __/ /_
 *  /____/\____/_/ /_/\__  /____/\____/_/  \__/
 *                   /____/
 *
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@qq.com>
 *
 * Copyright (C) 2016-2018 Zongsoft Corporation <http://www.zongsoft.com>
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
using System.Linq.Expressions;

namespace Zongsoft.Data
{
	public static class DataDictionary
	{
		#region 工厂方法
		public static IDataDictionary GetDictionary(object data)
		{
			if(data == null)
				throw new ArgumentNullException(nameof(data));

			switch(data)
			{
				case IEntity entity:
					return new EntityDictionary(entity);
				case IDataDictionary dictionary:
					return dictionary;
				case IDictionary<string, object> dictionary:
					return new GenericDictionary(dictionary);
				case IDictionary dictionary:
					return new ClassicDictionary(dictionary);
			}

			return new ObjectDictionary(data);
		}

		public static IDataDictionary GetDictionary(IEntity entity)
		{
			if(entity == null)
				throw new ArgumentNullException(nameof(entity));

			return new EntityDictionary(entity);
		}

		public static IDataDictionary<T> GetDictionary<T>(object data)
		{
			if(data == null)
				throw new ArgumentNullException(nameof(data));

			switch(data)
			{
				case IEntity entity:
					return new EntityDictionary<T>(entity);
				case IDataDictionary<T> dictionary:
					return dictionary;
				case IDataDictionary dictionary:
					return GetDictionary<T>(dictionary.Data);
				case IDictionary<string, object> dictionary:
					return new GenericDictionary<T>(dictionary);
				case IDictionary dictionary:
					return new ClassicDictionary<T>(dictionary);
			}

			return new ObjectDictionary<T>(data);
		}

		public static IDataDictionary<T> GetDictionary<T>(IEntity entity)
		{
			if(entity == null)
				throw new ArgumentNullException(nameof(entity));

			return new EntityDictionary<T>(entity);
		}

		public static IEnumerable<IDataDictionary> GetDictionaries(IEnumerable items)
		{
			if(items == null)
				throw new ArgumentNullException(nameof(items));

			foreach(var item in items)
			{
				if(item != null)
					yield return GetDictionary(item);
			}
		}

		public static IEnumerable<IDataDictionary> GetDictionaries(IEnumerable<IEntity> entities)
		{
			if(entities == null)
				throw new ArgumentNullException(nameof(entities));

			foreach(var entity in entities)
			{
				if(entity != null)
					yield return GetDictionary(entity);
			}
		}

		public static IEnumerable<IDataDictionary<T>> GetDictionaries<T>(IEnumerable items)
		{
			if(items == null)
				throw new ArgumentNullException(nameof(items));

			foreach(var item in items)
			{
				if(item != null)
					yield return GetDictionary<T>(item);
			}
		}

		public static IEnumerable<IDataDictionary<T>> GetDictionaries<T>(IEnumerable<IEntity> entities)
		{
			if(entities == null)
				throw new ArgumentNullException(nameof(entities));

			foreach(var entity in entities)
			{
				if(entity != null)
					yield return GetDictionary<T>(entity);
			}
		}
		#endregion

		#region 嵌套子类
		internal class DictionaryEnumerator : IDictionaryEnumerator
		{
			private IEnumerator<KeyValuePair<string, object>> _iterator;

			public DictionaryEnumerator(IEnumerator<KeyValuePair<string, object>> iterator)
			{
				_iterator = iterator;
			}

			public object Key
			{
				get
				{
					return _iterator.Current.Key;
				}
			}

			public object Value
			{
				get
				{
					return _iterator.Current.Value;
				}
			}

			public DictionaryEntry Entry
			{
				get
				{
					var current = _iterator.Current;
					return new DictionaryEntry(current.Key, current.Value);
				}
			}

			public object Current
			{
				get
				{
					return _iterator.Current;
				}
			}

			public bool MoveNext()
			{
				return _iterator.MoveNext();
			}

			public void Reset()
			{
				_iterator.Reset();
			}
		}
		#endregion
	}

	internal class ClassicDictionary : IDataDictionary
	{
		#region 成员字段
		private readonly IDictionary _dictionary;
		#endregion

		#region 构造函数
		public ClassicDictionary(IDictionary dictionary)
		{
			_dictionary = dictionary ?? throw new ArgumentNullException(nameof(dictionary));
		}
		#endregion

		#region 公共属性
		public int Count
		{
			get
			{
				return _dictionary.Count;
			}
		}

		public object Data
		{
			get
			{
				return _dictionary;
			}
		}

		public object this[string name]
		{
			get
			{
				return _dictionary[name];
			}
			set
			{
				_dictionary[name] = value;
			}
		}
		#endregion

		#region 公共方法
		public bool Contains(string name)
		{
			return _dictionary.Contains(name);
		}

		public void Reset(params string[] names)
		{
			if(names == null || names.Length == 0)
			{
				_dictionary.Clear();
				return;
			}

			foreach(var name in names)
			{
				_dictionary.Remove(name);
			}
		}

		public object GetValue(string name)
		{
			return _dictionary[name];
		}

		public TValue GetValue<TValue>(string name, TValue defaultValue)
		{
			if(_dictionary.Contains(name))
				return Zongsoft.Common.Convert.ConvertValue<TValue>(_dictionary[name]);
			else
				return defaultValue;
		}

		public void SetValue<TValue>(string name, TValue value, Func<TValue, bool> predicate = null)
		{
			this.SetValue<TValue>(name, () => value, predicate);
		}

		public void SetValue<TValue>(string name, Func<TValue> valueFactory, Func<TValue, bool> predicate = null)
		{
			if(valueFactory == null)
				throw new ArgumentNullException(nameof(valueFactory));

			if(predicate != null)
			{
				object raw = null;

				if(_dictionary.Contains(name))
					raw = _dictionary[name];

				if(!predicate(raw == null ? default(TValue) : (TValue)raw))
					return;
			}

			_dictionary[name] = valueFactory();
		}

		public bool TryGetValue<TValue>(string name, out TValue value)
		{
			value = default(TValue);

			if(_dictionary.Contains(name))
			{
				try
				{
					value = Common.Convert.ConvertValue<TValue>(_dictionary[name]);
					return true;
				}
				catch
				{
					return false;
				}
			}

			return false;
		}

		public bool TryGetValue<TValue>(string name, Action<TValue> got)
		{
			object value;

			if(_dictionary.Contains(name))
			{
				try
				{
					value = _dictionary[name];
				}
				catch
				{
					return false;
				}

				got?.Invoke(Common.Convert.ConvertValue<TValue>(value));
				return true;
			}

			return false;
		}

		public bool TrySetValue<TValue>(string name, TValue value, Func<TValue, bool> predicate = null)
		{
			return this.TrySetValue<TValue>(name, () => value, predicate);
		}

		public bool TrySetValue<TValue>(string name, Func<TValue> valueFactory, Func<TValue, bool> predicate = null)
		{
			if(valueFactory == null)
				throw new ArgumentNullException(nameof(valueFactory));

			if(predicate != null)
			{
				object raw = null;

				if(_dictionary.Contains(name))
					raw = _dictionary[name];

				if(!predicate(raw == null ? default(TValue) : (TValue)raw))
					return false;
			}

			_dictionary[name] = valueFactory();
			return true;
		}
		#endregion

		#region 接口实现
		bool ICollection<KeyValuePair<string, object>>.IsReadOnly
		{
			get
			{
				return _dictionary.IsReadOnly;
			}
		}

		ICollection IDictionary.Keys
		{
			get
			{
				return _dictionary.Keys;
			}
		}

		ICollection IDictionary.Values
		{
			get
			{
				return _dictionary.Values;
			}
		}

		bool IDictionary.IsReadOnly
		{
			get
			{
				return _dictionary.IsReadOnly;
			}
		}

		bool IDictionary.IsFixedSize
		{
			get
			{
				return _dictionary.IsFixedSize;
			}
		}

		int ICollection.Count
		{
			get
			{
				return _dictionary.Count;
			}
		}

		object ICollection.SyncRoot
		{
			get
			{
				return _dictionary.SyncRoot;
			}
		}

		bool ICollection.IsSynchronized
		{
			get
			{
				return _dictionary.IsSynchronized;
			}
		}

		object IDictionary.this[object key]
		{
			get
			{
				return _dictionary[key];
			}
			set
			{
				_dictionary[key] = value;
			}
		}

		bool IDictionary.Contains(object key)
		{
			return _dictionary.Contains(key);
		}

		void IDictionary.Add(object key, object value)
		{
			_dictionary.Add(key, value);
		}

		void IDictionary.Clear()
		{
			_dictionary.Clear();
		}

		void IDictionary.Remove(object key)
		{
			_dictionary.Remove(key);
		}

		void ICollection.CopyTo(Array array, int arrayIndex)
		{
			_dictionary.CopyTo(array, arrayIndex);
		}

		ICollection<string> IDictionary<string, object>.Keys
		{
			get
			{
				var keys = new List<string>(_dictionary.Count);

				foreach(DictionaryEntry entry in _dictionary)
				{
					keys.Add(entry.Key?.ToString());
				}

				return keys;
			}
		}

		ICollection<object> IDictionary<string, object>.Values
		{
			get
			{
				var values = new List<object>(_dictionary.Count);

				foreach(DictionaryEntry entry in _dictionary)
				{
					values.Add(entry.Value);
				}

				return values;
			}
		}

		bool IDictionary<string, object>.ContainsKey(string key)
		{
			return _dictionary.Contains(key);
		}

		void IDictionary<string, object>.Add(string key, object value)
		{
			_dictionary.Add(key, value);
		}

		bool IDictionary<string, object>.Remove(string key)
		{
			var existed = _dictionary.Contains(key);
			_dictionary.Remove(key);
			return existed;
		}

		bool IDictionary<string, object>.TryGetValue(string key, out object value)
		{
			value = null;

			if(_dictionary.Contains(key))
			{
				try
				{
					value = _dictionary[key];
					return true;
				}
				catch
				{
					return false;
				}
			}

			return false;
		}

		void ICollection<KeyValuePair<string, object>>.Add(KeyValuePair<string, object> item)
		{
			_dictionary.Add(item.Key, item.Value);
		}

		void ICollection<KeyValuePair<string, object>>.Clear()
		{
			_dictionary.Clear();
		}

		bool ICollection<KeyValuePair<string, object>>.Contains(KeyValuePair<string, object> item)
		{
			return _dictionary.Contains(item.Key);
		}

		void ICollection<KeyValuePair<string, object>>.CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
		{
			if(array == null)
				throw new ArgumentNullException(nameof(array));

			var offset = 0;

			foreach(DictionaryEntry entry in _dictionary)
			{
				var index = arrayIndex + offset++;

				if(index < array.Length)
					array[index] = new KeyValuePair<string, object>(entry.Key?.ToString(), entry.Value);
			}
		}

		bool ICollection<KeyValuePair<string, object>>.Remove(KeyValuePair<string, object> item)
		{
			var existed = _dictionary.Contains(item.Key);
			_dictionary.Remove(item.Key);
			return existed;
		}

		public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
		{
			foreach(DictionaryEntry entry in _dictionary)
			{
				yield return new KeyValuePair<string, object>(entry.Key?.ToString(), entry.Value);
			}
		}

		IDictionaryEnumerator IDictionary.GetEnumerator()
		{
			return _dictionary.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _dictionary.GetEnumerator();
		}
		#endregion
	}

	internal class ClassicDictionary<T> : ClassicDictionary, IDataDictionary<T>
	{
		#region 构造函数
		public ClassicDictionary(IDictionary data) : base(data)
		{
		}
		#endregion

		#region 公共方法
		public bool Contains<TMember>(Expression<Func<T, TMember>> expression)
		{
			return this.Contains(Common.ExpressionUtility.GetMemberName(expression));
		}

		public void Reset<TMember>(Expression<Func<T, TMember>> expression)
		{
			this.Reset(Common.ExpressionUtility.GetMemberName(expression));
		}

		public TValue GetValue<TValue>(Expression<Func<T, TValue>> expression)
		{
			return (TValue)this.GetValue(Common.ExpressionUtility.GetMemberName(expression));
		}

		public TValue GetValue<TValue>(Expression<Func<T, TValue>> expression, TValue defaultValue)
		{
			return this.GetValue(Common.ExpressionUtility.GetMemberName(expression), defaultValue);
		}

		public void SetValue<TValue>(Expression<Func<T, TValue>> expression, TValue value, Func<TValue, bool> predicate = null)
		{
			this.SetValue(Common.ExpressionUtility.GetMemberName(expression), value, predicate);
		}

		public void SetValue<TValue>(Expression<Func<T, TValue>> expression, Func<string, TValue> valueFactory, Func<TValue, bool> predicate = null)
		{
			var name = Common.ExpressionUtility.GetMemberName(expression);
			this.SetValue(name, () => valueFactory(name), predicate);
		}

		public bool TryGetValue<TValue>(Expression<Func<T, TValue>> expression, out TValue value)
		{
			return this.TryGetValue(Common.ExpressionUtility.GetMemberName(expression), out value);
		}

		public bool TryGetValue<TValue>(Expression<Func<T, TValue>> expression, Action<string, TValue> got)
		{
			var name = Common.ExpressionUtility.GetMemberName(expression);
			return this.TryGetValue<TValue>(name, value => got(name, value));
		}

		public bool TrySetValue<TValue>(Expression<Func<T, TValue>> expression, TValue value, Func<TValue, bool> predicate = null)
		{
			return this.TrySetValue(Common.ExpressionUtility.GetMemberName(expression), value, predicate);
		}

		public bool TrySetValue<TValue>(Expression<Func<T, TValue>> expression, Func<string, TValue> valueFactory, Func<TValue, bool> predicate = null)
		{
			var name = Common.ExpressionUtility.GetMemberName(expression);
			return this.TrySetValue(name, () => valueFactory(name), predicate);
		}
		#endregion
	}

	internal class GenericDictionary : IDataDictionary
	{
		#region 成员字段
		private readonly IDictionary<string, object> _dictionary;
		#endregion

		#region 构造函数
		public GenericDictionary(IDictionary<string, object> dictionary)
		{
			_dictionary = dictionary ?? throw new ArgumentNullException(nameof(dictionary));
		}
		#endregion

		#region 公共属性
		public int Count
		{
			get
			{
				return _dictionary.Count;
			}
		}

		public object Data
		{
			get
			{
				return _dictionary;
			}
		}

		public object this[string name]
		{
			get
			{
				return _dictionary[name];
			}
			set
			{
				_dictionary[name] = value;
			}
		}
		#endregion

		#region 公共方法
		public bool Contains(string name)
		{
			return _dictionary.ContainsKey(name);
		}

		public void Reset(params string[] names)
		{
			if(names == null || names.Length == 0)
			{
				_dictionary.Clear();
				return;
			}

			foreach(var name in names)
			{
				_dictionary.Remove(name);
			}
		}

		public object GetValue(string name)
		{
			return _dictionary[name];
		}

		public TValue GetValue<TValue>(string name, TValue defaultValue)
		{
			if(_dictionary.TryGetValue(name, out var value))
				return Zongsoft.Common.Convert.ConvertValue<TValue>(value);

			return defaultValue;
		}

		public void SetValue<TValue>(string name, TValue value, Func<TValue, bool> predicate = null)
		{
			this.SetValue<TValue>(name, () => value, predicate);
		}

		public void SetValue<TValue>(string name, Func<TValue> valueFactory, Func<TValue, bool> predicate = null)
		{
			if(valueFactory == null)
				throw new ArgumentNullException(nameof(valueFactory));

			if(predicate != null)
			{
				_dictionary.TryGetValue(name, out var raw);

				if(!predicate(raw == null ? default(TValue) : (TValue)raw))
					return;
			}

			_dictionary[name] = valueFactory();
		}

		public bool TryGetValue<TValue>(string name, out TValue value)
		{
			if(_dictionary.TryGetValue(name, out var obj))
			{
				value = Common.Convert.ConvertValue<TValue>(obj);
				return true;
			}

			value = default(TValue);
			return false;
		}

		public bool TryGetValue<TValue>(string name, Action<TValue> got)
		{
			if(_dictionary.TryGetValue(name, out var value))
			{
				got?.Invoke(Common.Convert.ConvertValue<TValue>(value));
				return true;
			}

			return false;
		}

		public bool TrySetValue<TValue>(string name, TValue value, Func<TValue, bool> predicate = null)
		{
			return this.TrySetValue<TValue>(name, () => value, predicate);
		}

		public bool TrySetValue<TValue>(string name, Func<TValue> valueFactory, Func<TValue, bool> predicate = null)
		{
			if(valueFactory == null)
				throw new ArgumentNullException(nameof(valueFactory));

			if(predicate != null)
			{
				_dictionary.TryGetValue(name, out var raw);

				if(!predicate(raw == null ? default(TValue) : (TValue)raw))
					return false;
			}

			_dictionary[name] = valueFactory();
			return true;
		}
		#endregion

		#region 接口实现
		bool ICollection<KeyValuePair<string, object>>.IsReadOnly
		{
			get
			{
				return _dictionary.IsReadOnly;
			}
		}

		ICollection IDictionary.Keys
		{
			get
			{
				return (ICollection)_dictionary.Keys;
			}
		}

		ICollection IDictionary.Values
		{
			get
			{
				return (ICollection)_dictionary.Values;
			}
		}

		bool IDictionary.IsReadOnly
		{
			get
			{
				return _dictionary.IsReadOnly;
			}
		}

		bool IDictionary.IsFixedSize
		{
			get
			{
				return false;
			}
		}

		int ICollection.Count
		{
			get
			{
				return _dictionary.Count;
			}
		}

		object ICollection.SyncRoot
		{
			get
			{
				return ((ICollection)_dictionary).SyncRoot;
			}
		}

		bool ICollection.IsSynchronized
		{
			get
			{
				return false;
			}
		}

		object IDictionary.this[object key]
		{
			get
			{
				if(key == null)
					throw new ArgumentNullException(nameof(key));

				return _dictionary[key.ToString()];
			}
			set
			{
				if(key == null)
					throw new ArgumentNullException(nameof(key));

				_dictionary[key.ToString()] = value;
			}
		}

		bool IDictionary.Contains(object key)
		{
			if(key == null)
				throw new ArgumentNullException(nameof(key));

			return _dictionary.ContainsKey(key.ToString());
		}

		void IDictionary.Add(object key, object value)
		{
			if(key == null)
				throw new ArgumentNullException(nameof(key));

			_dictionary.Add(key.ToString(), value);
		}

		void IDictionary.Clear()
		{
			_dictionary.Clear();
		}

		void IDictionary.Remove(object key)
		{
			if(key != null)
				_dictionary.Remove(key.ToString());
		}

		void ICollection.CopyTo(Array array, int arrayIndex)
		{
			if(array == null)
				throw new ArgumentNullException(nameof(array));

			var offset = 0;

			foreach(var entry in _dictionary)
			{
				var index = arrayIndex + offset++;

				if(index < array.Length)
					array.SetValue(entry, index);
			}
		}

		ICollection<string> IDictionary<string, object>.Keys
		{
			get
			{
				return _dictionary.Keys;
			}
		}

		ICollection<object> IDictionary<string, object>.Values
		{
			get
			{
				return _dictionary.Values;
			}
		}

		bool IDictionary<string, object>.ContainsKey(string key)
		{
			return _dictionary.ContainsKey(key);
		}

		void IDictionary<string, object>.Add(string key, object value)
		{
			_dictionary.Add(key, value);
		}

		bool IDictionary<string, object>.Remove(string key)
		{
			return _dictionary.Remove(key);
		}

		bool IDictionary<string, object>.TryGetValue(string key, out object value)
		{
			return _dictionary.TryGetValue(key, out value);
		}

		void ICollection<KeyValuePair<string, object>>.Add(KeyValuePair<string, object> item)
		{
			_dictionary.Add(item.Key, item.Value);
		}

		void ICollection<KeyValuePair<string, object>>.Clear()
		{
			_dictionary.Clear();
		}

		bool ICollection<KeyValuePair<string, object>>.Contains(KeyValuePair<string, object> item)
		{
			return _dictionary.ContainsKey(item.Key);
		}

		void ICollection<KeyValuePair<string, object>>.CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
		{
			_dictionary.CopyTo(array, arrayIndex);
		}

		bool ICollection<KeyValuePair<string, object>>.Remove(KeyValuePair<string, object> item)
		{
			return _dictionary.Remove(item.Key);
		}

		public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
		{
			return _dictionary.GetEnumerator();
		}

		IDictionaryEnumerator IDictionary.GetEnumerator()
		{
			var iterator = _dictionary.GetEnumerator();

			if(iterator is IDictionaryEnumerator enumerator)
				return enumerator;
			else
				return new DataDictionary.DictionaryEnumerator(iterator);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _dictionary.GetEnumerator();
		}
		#endregion
	}

	internal class GenericDictionary<T> : GenericDictionary, IDataDictionary<T>
	{
		#region 构造函数
		public GenericDictionary(IDictionary<string, object> data) : base(data)
		{
		}
		#endregion

		#region 公共方法
		public bool Contains<TMember>(Expression<Func<T, TMember>> expression)
		{
			return this.Contains(Common.ExpressionUtility.GetMemberName(expression));
		}

		public void Reset<TMember>(Expression<Func<T, TMember>> expression)
		{
			this.Reset(Common.ExpressionUtility.GetMemberName(expression));
		}

		public TValue GetValue<TValue>(Expression<Func<T, TValue>> expression)
		{
			return (TValue)this.GetValue(Common.ExpressionUtility.GetMemberName(expression));
		}

		public TValue GetValue<TValue>(Expression<Func<T, TValue>> expression, TValue defaultValue)
		{
			return this.GetValue(Common.ExpressionUtility.GetMemberName(expression), defaultValue);
		}

		public void SetValue<TValue>(Expression<Func<T, TValue>> expression, TValue value, Func<TValue, bool> predicate = null)
		{
			this.SetValue(Common.ExpressionUtility.GetMemberName(expression), value, predicate);
		}

		public void SetValue<TValue>(Expression<Func<T, TValue>> expression, Func<string, TValue> valueFactory, Func<TValue, bool> predicate = null)
		{
			var name = Common.ExpressionUtility.GetMemberName(expression);
			this.SetValue(name, () => valueFactory(name), predicate);
		}

		public bool TryGetValue<TValue>(Expression<Func<T, TValue>> expression, out TValue value)
		{
			return this.TryGetValue(Common.ExpressionUtility.GetMemberName(expression), out value);
		}

		public bool TryGetValue<TValue>(Expression<Func<T, TValue>> expression, Action<string, TValue> got)
		{
			var name = Common.ExpressionUtility.GetMemberName(expression);
			return this.TryGetValue<TValue>(name, value => got(name, value));
		}

		public bool TrySetValue<TValue>(Expression<Func<T, TValue>> expression, TValue value, Func<TValue, bool> predicate = null)
		{
			return this.TrySetValue(Common.ExpressionUtility.GetMemberName(expression), value, predicate);
		}

		public bool TrySetValue<TValue>(Expression<Func<T, TValue>> expression, Func<string, TValue> valueFactory, Func<TValue, bool> predicate = null)
		{
			var name = Common.ExpressionUtility.GetMemberName(expression);
			return this.TrySetValue(name, () => valueFactory(name), predicate);
		}
		#endregion
	}

	internal class ObjectDictionary : IDataDictionary
	{
		#region 私有常量
		private const string KEYNOTFOUND_EXCEPTION_MESSAGE = "The specified '{0}' key does not exist in the object dictionary.";
		#endregion

		#region 成员字段
		private readonly object _data;
		#endregion

		#region 构造函数
		public ObjectDictionary(object data)
		{
			_data = data ?? throw new ArgumentNullException(nameof(data));
		}
		#endregion

		#region 公共属性
		public object Data
		{
			get
			{
				return _data;
			}
		}

		public int Count
		{
			get
			{
				return 0;
			}
		}

		public object this[string key]
		{
			get
			{
				return this.GetValue(key);
			}
			set
			{
				this.SetValue(key, value);
			}
		}

		public ICollection<string> Keys
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public ICollection<object> Values
		{
			get
			{
				throw new NotImplementedException();
			}
		}
		#endregion

		#region 公共方法
		public bool Contains(string name)
		{
			throw new NotImplementedException();
		}

		public void Reset(params string[] names)
		{
			throw new NotImplementedException();
		}

		public object GetValue(string name)
		{
			throw new NotImplementedException();
		}

		public TValue GetValue<TValue>(string name, TValue defaultValue)
		{
			throw new NotImplementedException();
		}

		public void SetValue<TValue>(string name, TValue value, Func<TValue, bool> predicate = null)
		{
			this.SetValue<TValue>(name, () => value, predicate);
		}

		public void SetValue<TValue>(string name, Func<TValue> valueFactory, Func<TValue, bool> predicate = null)
		{
			throw new NotImplementedException();
		}

		public bool TryGetValue<TValue>(string name, out TValue value)
		{
			throw new NotImplementedException();
		}

		public bool TryGetValue<TValue>(string name, Action<TValue> got)
		{
			throw new NotImplementedException();
		}

		public bool TrySetValue<TValue>(string name, TValue value, Func<TValue, bool> predicate = null)
		{
			return this.TrySetValue<TValue>(name, () => value, predicate);
		}

		public bool TrySetValue<TValue>(string name, Func<TValue> valueFactory, Func<TValue, bool> predicate = null)
		{
			throw new NotImplementedException();
		}
		#endregion

		#region 接口实现
		bool ICollection<KeyValuePair<string, object>>.IsReadOnly
		{
			get
			{
				return false;
			}
		}

		ICollection IDictionary.Keys
		{
			get
			{
				return (ICollection)this.Keys;
			}
		}

		ICollection IDictionary.Values
		{
			get
			{
				return (ICollection)this.Values;
			}
		}

		bool IDictionary.IsReadOnly
		{
			get
			{
				return false;
			}
		}

		bool IDictionary.IsFixedSize
		{
			get
			{
				return false;
			}
		}

		int ICollection.Count
		{
			get
			{
				return this.Count;
			}
		}

		private readonly object _syncRoot = new object();

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
				return false;
			}
		}

		object IDictionary.this[object key]
		{
			get
			{
				return key == null ? null : this[key.ToString()];
			}
			set
			{
				if(key == null)
					throw new ArgumentNullException(nameof(key));

				this[key.ToString()] = value;
			}
		}

		bool IDictionary.Contains(object key)
		{
			if(key == null)
				return false;

			return this.Contains(key.ToString());
		}

		void IDictionary.Add(object key, object value)
		{
			if(key == null)
				throw new ArgumentNullException(nameof(key));

			this.SetValue(key.ToString(), value);
		}

		void IDictionary.Clear()
		{
			throw new NotSupportedException();
		}

		void IDictionary.Remove(object key)
		{
			throw new NotSupportedException();
		}

		void ICollection.CopyTo(Array array, int arrayIndex)
		{
			if(array == null)
				throw new ArgumentNullException(nameof(array));

			var offset = 0;

			foreach(var entry in this)
			{
				var index = arrayIndex + offset++;

				if(index < array.Length)
					array.SetValue(entry, index);
			}
		}

		bool IDictionary<string, object>.ContainsKey(string key)
		{
			throw new NotImplementedException();
		}

		void IDictionary<string, object>.Add(string key, object value)
		{
			this.SetValue(key, value);
		}

		bool IDictionary<string, object>.Remove(string key)
		{
			throw new NotSupportedException();
		}

		bool IDictionary<string, object>.TryGetValue(string key, out object value)
		{
			throw new NotImplementedException();
		}

		void ICollection<KeyValuePair<string, object>>.Add(KeyValuePair<string, object> item)
		{
			this.SetValue(item.Key, item.Value);
		}

		void ICollection<KeyValuePair<string, object>>.Clear()
		{
			throw new NotSupportedException();
		}

		bool ICollection<KeyValuePair<string, object>>.Contains(KeyValuePair<string, object> item)
		{
			if(item.Key == null)
				return false;

			throw new NotImplementedException();
		}

		void ICollection<KeyValuePair<string, object>>.CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
		{
			if(array == null)
				throw new ArgumentNullException(nameof(array));

			var offset = 0;

			foreach(var entry in this)
			{
				var index = arrayIndex + offset++;

				if(index < array.Length)
					array[index] = entry;
			}
		}

		bool ICollection<KeyValuePair<string, object>>.Remove(KeyValuePair<string, object> item)
		{
			throw new NotSupportedException();
		}

		public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
		{
			throw new NotImplementedException();
		}

		IDictionaryEnumerator IDictionary.GetEnumerator()
		{
			var iterator = this.GetEnumerator();

			if(iterator is IDictionaryEnumerator enumerator)
				return enumerator;
			else
				return new DataDictionary.DictionaryEnumerator(iterator);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}
		#endregion
	}

	internal class ObjectDictionary<T> : ObjectDictionary, IDataDictionary<T>
	{
		#region 构造函数
		public ObjectDictionary(object data) : base(data)
		{
		}
		#endregion

		#region 公共方法
		public bool Contains<TMember>(Expression<Func<T, TMember>> expression)
		{
			return this.Contains(Common.ExpressionUtility.GetMemberName(expression));
		}

		public void Reset<TMember>(Expression<Func<T, TMember>> expression)
		{
			this.Reset(Common.ExpressionUtility.GetMemberName(expression));
		}

		public TValue GetValue<TValue>(Expression<Func<T, TValue>> expression)
		{
			return (TValue)this.GetValue(Common.ExpressionUtility.GetMemberName(expression));
		}

		public TValue GetValue<TValue>(Expression<Func<T, TValue>> expression, TValue defaultValue)
		{
			return this.GetValue(Common.ExpressionUtility.GetMemberName(expression), defaultValue);
		}

		public void SetValue<TValue>(Expression<Func<T, TValue>> expression, TValue value, Func<TValue, bool> predicate = null)
		{
			this.SetValue(Common.ExpressionUtility.GetMemberName(expression), value, predicate);
		}

		public void SetValue<TValue>(Expression<Func<T, TValue>> expression, Func<string, TValue> valueFactory, Func<TValue, bool> predicate = null)
		{
			var name = Common.ExpressionUtility.GetMemberName(expression);
			this.SetValue(name, () => valueFactory(name), predicate);
		}

		public bool TryGetValue<TValue>(Expression<Func<T, TValue>> expression, out TValue value)
		{
			return this.TryGetValue(Common.ExpressionUtility.GetMemberName(expression), out value);
		}

		public bool TryGetValue<TValue>(Expression<Func<T, TValue>> expression, Action<string, TValue> got)
		{
			var name = Common.ExpressionUtility.GetMemberName(expression);
			return this.TryGetValue<TValue>(name, value => got(name, value));
		}

		public bool TrySetValue<TValue>(Expression<Func<T, TValue>> expression, TValue value, Func<TValue, bool> predicate = null)
		{
			return this.TrySetValue(Common.ExpressionUtility.GetMemberName(expression), value, predicate);
		}

		public bool TrySetValue<TValue>(Expression<Func<T, TValue>> expression, Func<string, TValue> valueFactory, Func<TValue, bool> predicate = null)
		{
			var name = Common.ExpressionUtility.GetMemberName(expression);
			return this.TrySetValue(name, () => valueFactory(name), predicate);
		}
		#endregion
	}

	internal class EntityDictionary : IDataDictionary
	{
		#region 私有常量
		private const string KEYNOTFOUND_EXCEPTION_MESSAGE = "The specified '{0}' key does not exist in the entity dictionary.";
		#endregion

		#region 成员字段
		private readonly IEntity _entity;
		#endregion

		#region 构造函数
		public EntityDictionary(IEntity entity)
		{
			_entity = entity ?? throw new ArgumentNullException(nameof(entity));
		}
		#endregion

		#region 公共属性
		public object Data
		{
			get
			{
				return _entity;
			}
		}

		public IEntity Entity
		{
			get
			{
				return _entity;
			}
		}

		public int Count
		{
			get
			{
				return _entity.GetChanges().Count;
			}
		}

		public object this[string key]
		{
			get
			{
				return this.GetValue(key);
			}
			set
			{
				this.SetValue(key, value);
			}
		}

		public ICollection<string> Keys
		{
			get
			{
				return _entity.GetChanges().Keys;
			}
		}

		public ICollection<object> Values
		{
			get
			{
				return _entity.GetChanges().Values;
			}
		}
		#endregion

		#region 公共方法
		public bool Contains(string name)
		{
			return _entity.HasChanges(name);
		}

		public bool HasChanges(params string[] names)
		{
			return _entity.HasChanges(names);
		}

		public void Reset(params string[] names)
		{
			throw new NotImplementedException();
		}

		public object GetValue(string name)
		{
			if(_entity.TryGetValue(name, out var value))
				return value;

			throw new KeyNotFoundException(string.Format(KEYNOTFOUND_EXCEPTION_MESSAGE, name));
		}

		public TValue GetValue<TValue>(string name, TValue defaultValue)
		{
			if(_entity.TryGetValue(name, out var value))
				return Zongsoft.Common.Convert.ConvertValue<TValue>(value);

			return defaultValue;
		}

		public void SetValue<TValue>(string name, TValue value, Func<TValue, bool> predicate = null)
		{
			this.SetValue<TValue>(name, () => value, predicate);
		}

		public void SetValue<TValue>(string name, Func<TValue> valueFactory, Func<TValue, bool> predicate = null)
		{
			if(valueFactory == null)
				throw new ArgumentNullException(nameof(valueFactory));

			if(predicate != null)
			{
				_entity.TryGetValue(name, out var raw);

				if(!predicate(raw == null ? default(TValue) : (TValue)raw))
					return;
			}

			if(!_entity.TrySetValue(name, valueFactory()))
				throw new KeyNotFoundException(string.Format(KEYNOTFOUND_EXCEPTION_MESSAGE, name));
		}

		public bool TryGetValue<TValue>(string name, out TValue value)
		{
			if(_entity.TryGetValue(name, out var obj))
			{
				value = Common.Convert.ConvertValue<TValue>(obj);
				return true;
			}

			value = default(TValue);
			return false;
		}

		public bool TryGetValue<TValue>(string name, Action<TValue> got)
		{
			if(_entity.TryGetValue(name, out var value))
			{
				got?.Invoke(Common.Convert.ConvertValue<TValue>(value));
				return true;
			}

			return false;
		}

		public bool TrySetValue<TValue>(string name, TValue value, Func<TValue, bool> predicate = null)
		{
			return this.TrySetValue<TValue>(name, () => value, predicate);
		}

		public bool TrySetValue<TValue>(string name, Func<TValue> valueFactory, Func<TValue, bool> predicate = null)
		{
			if(valueFactory == null)
				throw new ArgumentNullException(nameof(valueFactory));

			if(predicate != null)
			{
				_entity.TryGetValue(name, out var raw);

				if(!predicate(raw == null ? default(TValue) : (TValue)raw))
					return false;
			}

			return _entity.TrySetValue(name, valueFactory());
		}
		#endregion

		#region 接口实现
		bool ICollection<KeyValuePair<string, object>>.IsReadOnly
		{
			get
			{
				return false;
			}
		}

		ICollection IDictionary.Keys
		{
			get
			{
				return (ICollection)this.Keys;
			}
		}

		ICollection IDictionary.Values
		{
			get
			{
				return (ICollection)this.Values;
			}
		}

		bool IDictionary.IsReadOnly
		{
			get
			{
				return false;
			}
		}

		bool IDictionary.IsFixedSize
		{
			get
			{
				return false;
			}
		}

		int ICollection.Count
		{
			get
			{
				return this.Count;
			}
		}

		private readonly object _syncRoot = new object();

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
				return false;
			}
		}

		object IDictionary.this[object key]
		{
			get
			{
				return key == null ? null : this[key.ToString()];
			}
			set
			{
				if(key == null)
					throw new ArgumentNullException(nameof(key));

				this[key.ToString()] = value;
			}
		}

		bool IDictionary.Contains(object key)
		{
			if(key == null)
				return false;

			return this.Contains(key.ToString());
		}

		void IDictionary.Add(object key, object value)
		{
			if(key == null)
				throw new ArgumentNullException(nameof(key));

			this.SetValue(key.ToString(), value);
		}

		void IDictionary.Clear()
		{
			throw new NotSupportedException();
		}

		void IDictionary.Remove(object key)
		{
			throw new NotSupportedException();
		}

		void ICollection.CopyTo(Array array, int arrayIndex)
		{
			if(array == null)
				throw new ArgumentNullException(nameof(array));

			var offset = 0;

			foreach(var entry in this)
			{
				var index = arrayIndex + offset++;

				if(index < array.Length)
					array.SetValue(entry, index);
			}
		}

		bool IDictionary<string, object>.ContainsKey(string key)
		{
			return _entity.HasChanges(key);
		}

		void IDictionary<string, object>.Add(string key, object value)
		{
			this.SetValue(key, value);
		}

		bool IDictionary<string, object>.Remove(string key)
		{
			throw new NotSupportedException();
		}

		bool IDictionary<string, object>.TryGetValue(string key, out object value)
		{
			return _entity.TryGetValue(key, out value);
		}

		void ICollection<KeyValuePair<string, object>>.Add(KeyValuePair<string, object> item)
		{
			this.SetValue(item.Key, item.Value);
		}

		void ICollection<KeyValuePair<string, object>>.Clear()
		{
			throw new NotSupportedException();
		}

		bool ICollection<KeyValuePair<string, object>>.Contains(KeyValuePair<string, object> item)
		{
			if(item.Key == null)
				return false;

			return _entity.HasChanges(item.Key);
		}

		void ICollection<KeyValuePair<string, object>>.CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
		{
			if(array == null)
				throw new ArgumentNullException(nameof(array));

			var offset = 0;

			foreach(var entry in this)
			{
				var index = arrayIndex + offset++;

				if(index < array.Length)
					array[index] = entry;
			}
		}

		bool ICollection<KeyValuePair<string, object>>.Remove(KeyValuePair<string, object> item)
		{
			throw new NotSupportedException();
		}

		public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
		{
			var items = _entity.GetChanges();

			if(items == null)
				return System.Linq.Enumerable.Empty<KeyValuePair<string, object>>().GetEnumerator();
			else
				return items.GetEnumerator();
		}

		IDictionaryEnumerator IDictionary.GetEnumerator()
		{
			var iterator = this.GetEnumerator();

			if(iterator is IDictionaryEnumerator enumerator)
				return enumerator;
			else
				return new DataDictionary.DictionaryEnumerator(iterator);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}
		#endregion
	}

	internal class EntityDictionary<T> : EntityDictionary, IDataDictionary<T>
	{
		#region 构造函数
		public EntityDictionary(IEntity entity) : base(entity)
		{
		}
		#endregion

		#region 公共方法
		public bool Contains<TMember>(Expression<Func<T, TMember>> expression)
		{
			return this.Contains(Common.ExpressionUtility.GetMemberName(expression));
		}

		public void Reset<TMember>(Expression<Func<T, TMember>> expression)
		{
			this.Reset(Common.ExpressionUtility.GetMemberName(expression));
		}

		public TValue GetValue<TValue>(Expression<Func<T, TValue>> expression)
		{
			return (TValue)this.GetValue(Common.ExpressionUtility.GetMemberName(expression));
		}

		public TValue GetValue<TValue>(Expression<Func<T, TValue>> expression, TValue defaultValue)
		{
			return this.GetValue(Common.ExpressionUtility.GetMemberName(expression), defaultValue);
		}

		public void SetValue<TValue>(Expression<Func<T, TValue>> expression, TValue value, Func<TValue, bool> predicate = null)
		{
			this.SetValue(Common.ExpressionUtility.GetMemberName(expression), value, predicate);
		}

		public void SetValue<TValue>(Expression<Func<T, TValue>> expression, Func<string, TValue> valueFactory, Func<TValue, bool> predicate = null)
		{
			var name = Common.ExpressionUtility.GetMemberName(expression);
			this.SetValue(name, () => valueFactory(name), predicate);
		}

		public bool TryGetValue<TValue>(Expression<Func<T, TValue>> expression, out TValue value)
		{
			return this.TryGetValue(Common.ExpressionUtility.GetMemberName(expression), out value);
		}

		public bool TryGetValue<TValue>(Expression<Func<T, TValue>> expression, Action<string, TValue> got)
		{
			var name = Common.ExpressionUtility.GetMemberName(expression);
			return this.TryGetValue<TValue>(name, value => got(name, value));
		}

		public bool TrySetValue<TValue>(Expression<Func<T, TValue>> expression, TValue value, Func<TValue, bool> predicate = null)
		{
			return this.TrySetValue(Common.ExpressionUtility.GetMemberName(expression), value, predicate);
		}

		public bool TrySetValue<TValue>(Expression<Func<T, TValue>> expression, Func<string, TValue> valueFactory, Func<TValue, bool> predicate = null)
		{
			var name = Common.ExpressionUtility.GetMemberName(expression);
			return this.TrySetValue(name, () => valueFactory(name), predicate);
		}
		#endregion
	}
}

/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2016 Zongsoft Corporation <http://www.zongsoft.com>
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
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Linq;

namespace Zongsoft.Data
{
	public class DataDictionary : IDictionary<string, object>
	{
		#region 成员字段
		private readonly object _data;
		private readonly PropertyDescriptorCollection _properties;
		private readonly ConcurrentDictionary<string, object> _values;
		#endregion

		#region 构造函数
		internal protected DataDictionary(object data)
		{
			if(data == null)
				throw new ArgumentNullException(nameof(data));

			_data = data;

			if(data is IDictionary || data is IDictionary<string, object> || data is IDictionary<string, string>)
			{
				_properties = null;
				_values = null;
			}
			else
			{
				_properties = TypeDescriptor.GetProperties(data);
				_values = new ConcurrentDictionary<string, object>(StringComparer.OrdinalIgnoreCase);
			}
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

		public ICollection<string> Keys
		{
			get
			{
				return this.Select(p => p.Key).ToArray();
			}
		}

		public ICollection<object> Values
		{
			get
			{
				return this.Select(p => p.Value).ToArray();
			}
		}

		public int Count
		{
			get
			{
				if(_properties != null)
					return _properties.Count;

				if(_data is ICollection)
					return ((ICollection)_data).Count;

				if(_data is IDictionary)
					return ((IDictionary)_data).Count;

				if(_data is IDictionary<string, object>)
					return ((IDictionary<string, object>)_data).Count;

				if(_data is IDictionary<string, string>)
					return ((IDictionary<string, string>)_data).Count;

				return 0;
			}
		}

		public object this[string key]
		{
			get
			{
				return this.Get(key);
			}
			set
			{
				this.Set(key, value);
			}
		}
		#endregion

		#region 公共方法
		public bool Contains(string key)
		{
			object value;
			return this.TryGet(key, out value);
		}

		/// <summary>
		/// 获取指定键的值，如果键不存在则激发异常。
		/// </summary>
		/// <param name="key">指定的键。</param>
		/// <returns>返回获取的值。</returns>
		/// <exception cref="KeyNotFoundException">当指定的键不存在。</exception>
		public object Get(string key)
		{
			object result;

			if(this.TryGet(key, out result))
				return result;

			throw new KeyNotFoundException(string.Format("The '{0}' property is not existed.", key));
		}

		/// <summary>
		/// 获取指定键的值，如果键不存在则返回<paramref name="defaultValue"/>参数值。
		/// </summary>
		/// <param name="key">指定的键。</param>
		/// <param name="defaultValue">指定的默认值，当键不存在则返回该参数值。</param>
		/// <returns>返回获取的值或默认值。</returns>
		public object Get(string key, object defaultValue)
		{
			object result;

			if(this.TryGet(key, out result))
				return result;

			return defaultValue;
		}

		public bool TryGet(string key, Action<object> onGot)
		{
			if(key == null)
				throw new ArgumentNullException("key");

			object value = null;

			if(this.TryGet(key, out value))
			{
				//执行成功回调方法
				if(onGot != null)
					onGot(value);

				return true;
			}

			return false;
		}

		public bool TryGet(string key, out object result)
		{
			if(key == null)
				throw new ArgumentNullException(nameof(key));

			if(_values == null)
			{
				if(_data is IDictionary)
					return Zongsoft.Collections.DictionaryExtension.TryGetValue((IDictionary)_data, key, out result);

				if(_data is IDictionary<string, object>)
					return ((IDictionary<string, object>)_data).TryGetValue(key, out result);

				if(_data is IDictionary<string, string>)
				{
					string text;

					if(((IDictionary<string, string>)_data).TryGetValue(key, out text))
					{
						result = text;
						return true;
					}
				}

				result = null;
				return false;
			}

			if(_values.TryGetValue(key, out result))
				return true;

			var property = _properties.Find(key, true);

			if(property == null)
				return false;

			result = _values[key] = property.GetValue(_data);

			return true;
		}

		public void Set(string key, object value, Func<object, bool> predicate = null)
		{
			this.Set(key, () => value, predicate);
		}

		public void Set(string key, Func<object> valueThunk, Func<object, bool> predicate = null)
		{
			var requiredThorws = true;
			var predicateProxy = predicate;

			//如果条件断言不为空，则必须进行是否抛出异常的条件处理
			if(predicate != null)
			{
				//如果是因为设置条件没有通过，则不能抛出异常，因为这是设置方法的正常逻辑
				predicateProxy = p => requiredThorws = predicate(p);
			}

			if(!this.TrySet(key, valueThunk, predicateProxy) && requiredThorws)
				throw new KeyNotFoundException(string.Format("The '{0}' property is not existed.", key));
		}

		public bool TrySet(string key, object value, Func<object, bool> predicate = null)
		{
			return this.TrySet(key, () => value, predicate);
		}

		public bool TrySet(string key, Func<object> valueThunk, Func<object, bool> predicate = null)
		{
			if(key == null)
				throw new ArgumentNullException(nameof(key));
			if(valueThunk == null)
				throw new ArgumentNullException(nameof(valueThunk));

			if(_properties == null)
			{
				object original;

				if(_data is IDictionary)
				{
					if(predicate != null)
					{
						Zongsoft.Collections.DictionaryExtension.TryGetValue((IDictionary)_data, key, out original);

						if(!predicate(original))
							return false;
					}

					((IDictionary)_data)[key] = valueThunk();
					return true;
				}

				if(_data is IDictionary<string, object>)
				{
					if(predicate != null)
					{
						((IDictionary<string, object>)_data).TryGetValue(key, out original);

						if(!predicate(original))
							return false;
					}

					((IDictionary<string, object>)_data)[key] = valueThunk;
					return true;
				}

				if(_data is IDictionary<string, string>)
				{
					if(predicate != null)
					{
						string originalText;
						((IDictionary<string, string>)_data).TryGetValue(key, out originalText);

						if(!predicate(originalText))
							return false;
					}

					((IDictionary<string, string>)_data)[key] = Zongsoft.Common.Convert.ConvertValue<string>(valueThunk());
					return true;
				}

				return false;
			}

			var property = _properties.Find(key, true);

			if(property == null || property.IsReadOnly)
				return false;

			if(predicate != null)
			{
				object original;

				if(!_values.TryGetValue(key, out original))
					original = property.GetValue(_data);

				if(!predicate(original))
					return false;
			}

			var value = valueThunk();

			//如果类型转换失败则返回
			if(!Zongsoft.Common.Convert.TryConvertValue(value, property.PropertyType, out value))
				return false;

			//首先设置容器对象的属性值
			property.SetValue(_data, value);

			//缓存设置的值
			_values[key] = value;

			return true;
		}
		#endregion

		#region 接口实现
		bool ICollection<KeyValuePair<string, object>>.IsReadOnly
		{
			get
			{
				return _properties != null;
			}
		}

		bool IDictionary<string, object>.ContainsKey(string key)
		{
			return this.Contains(key);
		}

		void IDictionary<string, object>.Add(string key, object value)
		{
			this.Set(key, value);
		}

		bool IDictionary<string, object>.Remove(string key)
		{
			throw new NotSupportedException();
		}

		bool IDictionary<string, object>.TryGetValue(string key, out object value)
		{
			return this.TryGet(key, out value);
		}

		void ICollection<KeyValuePair<string, object>>.Add(KeyValuePair<string, object> item)
		{
			this.Set(item.Key, item.Value);
		}

		void ICollection<KeyValuePair<string, object>>.Clear()
		{
			throw new NotSupportedException();
		}

		bool ICollection<KeyValuePair<string, object>>.Contains(KeyValuePair<string, object> item)
		{
			if(item.Key == null)
				return false;

			return this.Contains(item.Key);
		}

		void ICollection<KeyValuePair<string, object>>.CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
		{
			throw new NotImplementedException();
		}

		bool ICollection<KeyValuePair<string, object>>.Remove(KeyValuePair<string, object> item)
		{
			throw new NotSupportedException();
		}

		public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
		{
			if(_properties != null)
			{
				foreach(PropertyDescriptor property in _properties)
				{
					yield return new KeyValuePair<string, object>(property.Name,
																  _values.GetOrAdd(property.Name, key => property.GetValue(_data)));
				}
			}
			else
			{
				if(_data is IDictionary)
				{
					foreach(var key in ((IDictionary)_data).Keys)
					{
						yield return new KeyValuePair<string, object>(key == null ? null : key.ToString(), ((IDictionary)_data)[key]);
					}
				}
				else if(_data is IDictionary<string, object>)
				{
					foreach(var key in ((IDictionary<string, object>)_data).Keys)
					{
						yield return new KeyValuePair<string, object>(key, ((IDictionary<string, object>)_data)[key]);
					}
				}
				else if(_data is IDictionary<string, string>)
				{
					foreach(var key in ((IDictionary<string, string>)_data).Keys)
					{
						yield return new KeyValuePair<string, object>(key, ((IDictionary<string, string>)_data)[key]);
					}
				}
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}
		#endregion
	}
}

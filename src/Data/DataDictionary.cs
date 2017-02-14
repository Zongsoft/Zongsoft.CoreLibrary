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
		private object _data;
		private ObjectCache _cache;
		#endregion

		#region 构造函数
		internal protected DataDictionary(object data)
		{
			if(data == null)
				throw new ArgumentNullException("data");

			_data = data;
			_cache = new ObjectCache(_data);
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
				return _cache.Select(p => p.Key).ToArray();
			}
		}

		public ICollection<object> Values
		{
			get
			{
				return _cache.Select(p => p.Value).ToArray();
			}
		}

		public int Count
		{
			get
			{
				return _cache.Count;
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

			//首先直接查找全键，如果找到则返回
			if(_cache.TryGet(key, out value))
			{
				if(value != null && value is ObjectCache)
					value = ((ObjectCache)value).Container;

				//执行成功回调方法
				if(onGot != null)
					onGot(value);

				return true;
			}

			//将键进行分解
			var parts = key.Split('.');

			//如果键不可分解则返回失败（因为之前已经全键查找过一次）
			if(parts.Length == 1)
				return false;

			//设置分段查找的起始点
			value = _cache;

			foreach(var part in parts)
			{
				if(!ObjectCache.TryGet(value, part, out value))
					return false;
			}

			if(value != null && value is ObjectCache)
				value = ((ObjectCache)value).Container;

			//执行成功回调方法
			if(onGot != null)
				onGot(value);

			return true;
		}

		public bool TryGet(string key, out object result)
		{
			object foundValue = null;

			if(this.TryGet(key, value => foundValue = value))
			{
				result = foundValue;
				return true;
			}

			result = null;
			return false;
		}

		public void Set(string key, object value, Func<object, bool> predicate = null)
		{
			if(!this.TrySet(key, value, predicate))
				throw new KeyNotFoundException(string.Format("The '{0}' property is not existed.", key));
		}

		public bool TrySet(string key, object value, Func<object, bool> predicate = null)
		{
			return this.TrySet(key, () => value, predicate);
		}

		public bool TrySet(string key, Func<object> valueThunk, Func<object, bool> predicate = null)
		{
			if(key == null)
				throw new ArgumentNullException("key");

			if(valueThunk == null)
				throw new ArgumentNullException("valueThunk");

			//首先直接设置全键，如果成功则返回
			if(_cache.TrySet(key, valueThunk, predicate))
				return true;

			//将键进行分解
			var parts = key.Split('.');

			//如果键不可分解则返回失败（因为之前已经全键设置过一次）
			if(parts.Length == 1)
				return false;

			object target = _cache;

			for(int i = 0; i < parts.Length - 1; i++)
			{
				if(!ObjectCache.TryGet(target, parts[i], out target))
					return false;
			}

			if(target == null)
				return false;

			return ObjectCache.TrySet(target, parts[parts.Length - 1], valueThunk, predicate);
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

		bool IDictionary<string, object>.ContainsKey(string key)
		{
			return this.Contains(key);
		}

		void IDictionary<string, object>.Add(string key, object value)
		{
			throw new NotSupportedException();
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
			throw new NotImplementedException();
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
			return _cache.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _cache.GetEnumerator();
		}
		#endregion

		#region 嵌套子类
		private class ObjectCache : IEnumerable<KeyValuePair<string, object>>
		{
			#region 私有变量
			private readonly object _container;
			private readonly PropertyDescriptorCollection _properties;
			private readonly ConcurrentDictionary<string, object> _values;
			#endregion

			#region 构造函数
			public ObjectCache(object container)
			{
				if(container == null)
					throw new ArgumentNullException("container");

				_container = container;

				if(!(container is IDictionary || container is IDictionary<string, object> || container is IDictionary<string, string>))
				{
					_properties = TypeDescriptor.GetProperties(container);
					_values = new ConcurrentDictionary<string, object>(StringComparer.OrdinalIgnoreCase);
				}
			}
			#endregion

			#region 公共属性
			public object Container
			{
				get
				{
					return _container;
				}
			}

			public int Count
			{
				get
				{
					if(_properties != null)
						return _properties.Count;

					if(_container is ICollection)
						return ((ICollection)_container).Count;

					if(_container is IDictionary)
						return ((IDictionary)_container).Count;

					if(_container is IDictionary<string, object>)
						return ((IDictionary<string, object>)_container).Count;

					if(_container is IDictionary<string, string>)
						return ((IDictionary<string, string>)_container).Count;

					return 0;
				}
			}
			#endregion

			#region 公共方法
			public bool TryGet(string name, out object result)
			{
				if(name == null)
					throw new ArgumentNullException("name");

				result = null;
				object value = null;

				if(_values != null && _values.TryGetValue(name, out result))
					return true;

				if(TryGetDictionary(_container, name, out result))
					return true;

				if(_properties == null)
					return false;

				var property = _properties.Find(name, true);

				if(property == null)
					return false;

				value = property.GetValue(_container);

				if(value == null || Common.TypeExtension.IsScalarType(value.GetType()))
					result = _values[name] = value;
				else
					result = _values[name] = new ObjectCache(value);

				return true;
			}

			public bool TrySet(string name, Func<object> valueThunk, Func<object, bool> predicate)
			{
				if(name == null)
					throw new ArgumentNullException("name");

				if(TrySetDictionary(_container, name, valueThunk, predicate))
					return true;

				if(_properties == null)
					return false;

				var property = _properties.Find(name, true);

				if(property == null || property.IsReadOnly)
					return false;

				if(predicate != null)
				{
					object original;

					if(!_values.TryGetValue(name, out original))
						original = property.GetValue(_container);

					if(!predicate(original is ObjectCache ? ((ObjectCache)original).Container : original))
						return false;
				}

				//获取值并做类型转换
				var value = Zongsoft.Common.Convert.ConvertValue(valueThunk(), property.PropertyType);

				//首先设置容器对象的属性值
				property.SetValue(_container, value);

				if(value == null || Common.TypeExtension.IsScalarType(value.GetType()))
					_values[name] = value;
				else
					_values[name] = new ObjectCache(value);

				return true;
			}
			#endregion

			#region 静态方法
			public static bool TryGet(object container, string name, out object result)
			{
				if(container == null)
					throw new ArgumentNullException("container");

				if(container is ObjectCache)
					return ((ObjectCache)container).TryGet(name, out result);

				if(TryGetDictionary(container, name, out result))
					return true;

				result = Reflection.MemberAccess.GetMemberValue<object>(container, name);
				return result != null;
			}

			private static bool TryGetDictionary(object container, string name, out object result)
			{
				result = null;

				if(container is IDictionary)
					return Zongsoft.Collections.DictionaryExtension.TryGetValue((IDictionary)container, name, out result);

				if(container is IDictionary<string, object>)
					return ((IDictionary<string, object>)container).TryGetValue(name, out result);

				if(container is IDictionary<string, string>)
				{
					string text;

					if(((IDictionary<string, string>)container).TryGetValue(name, out text))
					{
						result = text;
						return true;
					}

					return false;
				}

				return false;
			}

			public static bool TrySet(object container, string name, Func<object> valueThunk, Func<object, bool> predicate)
			{
				if(container == null)
					throw new ArgumentNullException("container");

				if(valueThunk == null)
					throw new ArgumentNullException("valueThunk");

				if(container is ObjectCache)
					return ((ObjectCache)container).TrySet(name, valueThunk, predicate);

				if(TrySetDictionary(container, name, valueThunk, predicate))
					return true;

				var requireSetup = false;

				Reflection.MemberAccess.SetMemberValue<object>(container, name, valueThunk, null, ctx =>
				{
					requireSetup = predicate == null || predicate(ctx.GetMemberValue());

					if(requireSetup)
						ctx.Setup();
				});

				return requireSetup;
			}

			private static bool TrySetDictionary(object container, string name, Func<object> valueThunk, Func<object, bool> predicate)
			{
				object original = null;

				if(container is IDictionary)
				{
					if(predicate != null)
					{
						Zongsoft.Collections.DictionaryExtension.TryGetValue((IDictionary)container, name, out original);

						if(!predicate(original))
							return false;
					}

					((IDictionary)container)[name] = valueThunk();
					return true;
				}

				if(container is IDictionary<string, object>)
				{
					if(predicate != null)
					{
						((IDictionary<string, object>)container).TryGetValue(name, out original);

						if(!predicate(original))
							return false;
					}

					((IDictionary<string, object>)container)[name] = valueThunk;
					return true;
				}

				if(container is IDictionary<string, string>)
				{
					if(predicate != null)
					{
						string originalText;
						((IDictionary<string, string>)container).TryGetValue(name, out originalText);

						if(!predicate(originalText))
							return false;
					}

					((IDictionary<string, string>)container)[name] = Zongsoft.Common.Convert.ConvertValue<string>(valueThunk());
					return true;
				}

				return false;
			}
			#endregion

			#region 遍历枚举
			public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
			{
				if(_properties != null)
				{
					foreach(PropertyDescriptor property in _properties)
					{
						yield return new KeyValuePair<string, object>(property.Name,
																	  _values.GetOrAdd(property.Name, key => property.GetValue(_container)));
					}
				}
				else
				{
					if(_container is IDictionary)
					{
						foreach(var key in ((IDictionary)_container).Keys)
						{
							yield return new KeyValuePair<string, object>(key == null ? null : key.ToString(), ((IDictionary)_container)[key]);
						}
					}
					else if(_container is IDictionary<string, object>)
					{
						foreach(var key in ((IDictionary<string, object>)_container).Keys)
						{
							yield return new KeyValuePair<string, object>(key, ((IDictionary<string, object>)_container)[key]);
						}
					}
					else if(_container is IDictionary<string, string>)
					{
						foreach(var key in ((IDictionary<string, string>)_container).Keys)
						{
							yield return new KeyValuePair<string, object>(key, ((IDictionary<string, string>)_container)[key]);
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
		#endregion
	}
}

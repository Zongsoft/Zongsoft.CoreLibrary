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
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Zongsoft.Data
{
	public class DataDictionary<T>
	{
		private object _data;
		private ConcurrentDictionary<string, object> _properties;

		public DataDictionary(object data)
		{
			if(data == null)
				throw new ArgumentNullException("data");

			_data = data;
		}

		public bool IsDictionary
		{
			get
			{
				return _data is IDictionary || Zongsoft.Common.TypeExtension.IsAssignableFrom(typeof(IDictionary<,>), _data.GetType());
			}
		}

		public bool Contains(string key)
		{
			if(key == null)
				throw new ArgumentNullException("key");

			if(_data is IDictionary)
				return ((IDictionary)_data).Contains(key);
			if(_data is IDictionary<string, object>)
				return ((IDictionary<string, object>)_data).ContainsKey(key);

			return _properties.ContainsKey(key);
		}

		public bool Contains<TKey>(Expression<Func<T, TKey>> key)
		{
		}

		public object Get(string key)
		{
		}

		public bool Get(string key, Action<object> onGot)
		{
			if(onGot == null)
				throw new ArgumentNullException("onGot");


		}

		public bool TryGet(string key, out object result)
		{
		}

		public TKey Get<TKey>(Expression<Func<T, TKey>> key)
		{
		}

		public bool Get<TKey>(Expression<Func<T, TKey>> key, Action<TKey> onGot)
		{
			if(onGot == null)
				throw new ArgumentNullException("onGot");

		}

		public bool TryGet<TKey>(Expression<Func<T, TKey>> key, out TKey result)
		{
		}

		public void Set(string key, object value)
		{
		}

		public void Set<TKey>(Expression<Func<T, TKey>> key, TKey value)
		{
		}

		private class DataPropertyDescriptor
		{
			#region 静态字段
			public static readonly DataPropertyDescriptor Missing = new DataPropertyDescriptor();
			#endregion

			#region 静态方法
			public static bool IsMissing(DataPropertyDescriptor property)
			{
				return object.ReferenceEquals(property, Missing);
			}
			#endregion

			#region 构造函数
			private DataPropertyDescriptor()
			{
			}

			public DataPropertyDescriptor(Type propertyType)
			{
				if(propertyType == null)
					throw new ArgumentNullException("propertyType");

				this.PropertyType = propertyType;
			}
			#endregion

			public Type PropertyType;
			public object Value;
		}

		private class DataCache
		{
			private readonly ConcurrentDictionary<Type, IDictionary<string, object>> _cache;

			public IDictionary<string, object> GetProperties(Type type)
			{
				if(type == null)
					throw new ArgumentNullException("type");

				return _cache.GetOrAdd(type, t => CreateProperties(t));
			}

			private IDictionary<string, object> CreateProperties(Type type)
			{
				var dictionary = new ConcurrentDictionary<string, object>();
				var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);

				foreach(var property in properties)
				{
					if(property.CanRead && property.CanWrite)
					{
						dictionary.TryAdd(property.Name, null);
					}
				}

				return dictionary;
			}
		}
	}
}

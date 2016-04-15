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
using System.Reflection;
using System.Linq.Expressions;

namespace Zongsoft.Data
{
	public class DataDictionary<T>
	{
		#region 成员字段
		private object _data;
		private ObjectCache _cache;
		#endregion

		#region 构造函数
		public DataDictionary(object data)
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
		#endregion

		#region 公共方法
		public bool Contains(string key)
		{
			object value;
			return this.TryGet(key, out value);
		}

		public bool Contains<TMember>(Expression<Func<T, TMember>> member)
		{
			if(member == null)
				throw new ArgumentNullException("member");

			var tuple = ResolveExpression(member, new Stack<MemberInfo>());
			return tuple == null ? false : this.Contains(tuple.Item1);
		}

		public object Get(string key)
		{
			object result;

			if(this.TryGet(key, out result))
				return result;

			throw new KeyNotFoundException(string.Format("The '{0}' property is not existed.", key));
		}

		public TMember Get<TMember>(Expression<Func<T, TMember>> member)
		{
			if(member == null)
				throw new ArgumentNullException("member");

			var tuple = ResolveExpression(member, new Stack<MemberInfo>());

			if(tuple == null)
				throw new ArgumentException("Invalid member expression.");

			return (TMember)Zongsoft.Common.Convert.ConvertValue(this.Get(tuple.Item1), tuple.Item2);
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

		public bool TryGet<TMember>(Expression<Func<T, TMember>> member, Action<string, TMember> onGot)
		{
			if(member == null)
				throw new ArgumentNullException("member");

			if(onGot == null)
				throw new ArgumentNullException("onGot");

			var tuple = ResolveExpression(member, new Stack<MemberInfo>());

			if(tuple == null)
				return false;

			return this.TryGet(tuple.Item1, value => onGot(tuple.Item1, (TMember)Zongsoft.Common.Convert.ConvertValue(value, tuple.Item2)));
		}

		public bool TryGet<TMember>(Expression<Func<T, TMember>> member, out TMember result)
		{
			TMember memberValue = default(TMember);

			if(this.TryGet(member, (key, value) => memberValue = value))
			{
				result = memberValue;
				return true;
			}

			result = default(TMember);
			return false;
		}

		public void Set(string key, object value)
		{
			if(!this.TrySet(key, value))
				throw new KeyNotFoundException(string.Format("The '{0}' property is not existed.", key));
		}

		public void Set<TMember>(Expression<Func<T, TMember>> member, TMember value)
		{
			if(!this.TrySet(member, value))
				throw new KeyNotFoundException(string.Format("The '{0}' property is not existed.", member));
		}

		public bool TrySet(string key, object value)
		{
			if(key == null)
				throw new ArgumentNullException("key");

			//首先直接设置全键，如果成功则返回
			if(_cache.TrySet(key, value))
				return true;

			//将键进行分解
			var parts = key.Split('.');

			//如果键不可分解则返回失败（因为之前已经全键设置过一次）
			if(parts.Length == 1)
				throw new KeyNotFoundException(string.Format("The '{0}' property is not existed.", key));

			object target = _cache;

			for(int i = 0; i < parts.Length - 1; i++)
			{
				if(!ObjectCache.TryGet(target, parts[i], out target))
					return false;
			}

			if(target == null)
				return false;

			return ObjectCache.TrySet(target, parts[parts.Length - 1], value);
		}

		public bool TrySet<TMember>(Expression<Func<T, TMember>> member, TMember value)
		{
			if(member == null)
				throw new ArgumentNullException("member");

			var tuple = ResolveExpression(member, new Stack<MemberInfo>());

			if(tuple == null)
				throw new ArgumentException("Invalid member expression.");

			return this.TrySet(tuple.Item1, value);
		}
		#endregion

		#region 私有方法
		private static Tuple<string, Type> ResolveExpression(Expression expression, Stack<MemberInfo> stack)
		{
			if(expression.NodeType == ExpressionType.Lambda)
				return ResolveExpression(((LambdaExpression)expression).Body, stack);

			if(expression.NodeType == ExpressionType.MemberAccess)
			{
				stack.Push(((MemberExpression)expression).Member);

				if(((MemberExpression)expression).Expression != null)
					return ResolveExpression(((MemberExpression)expression).Expression, stack);
			}

			if(stack == null || stack.Count == 0)
				return null;

			var path = string.Empty;
			var type = typeof(object);
			MemberInfo member = null;

			while(stack.Count > 0)
			{
				member = stack.Pop();

				if(path.Length > 0)
					path += ".";

				path += member.Name;
			}

			switch(member.MemberType)
			{
				case MemberTypes.Property:
					type = ((PropertyInfo)member).PropertyType;
					break;
				case MemberTypes.Field:
					type = ((FieldInfo)member).FieldType;
					break;
				default:
					throw new InvalidOperationException("");
			}

			return new Tuple<string, Type>(path, type);
		}
		#endregion

		#region 嵌套子类
		private class ObjectCache
		{
			#region 私有变量
			private readonly object _container;
			private readonly PropertyDescriptorCollection _properties;
			private readonly ConcurrentDictionary<string, object> _cache;
			#endregion

			#region 构造函数
			public ObjectCache(object container)
			{
				if(container == null)
					throw new ArgumentNullException("container");

				_container = container;

				if(!(container is IDictionary || Zongsoft.Common.TypeExtension.IsAssignableFrom(typeof(IDictionary<,>), container.GetType())))
				{
					_properties = TypeDescriptor.GetProperties(container);
					_cache = new ConcurrentDictionary<string, object>();
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
			#endregion

			#region 公共方法
			public bool TryGet(string name, out object result)
			{
				if(name == null)
					throw new ArgumentNullException("name");

				result = null;
				object value = null;

				if(_container is IDictionary)
					return Zongsoft.Collections.DictionaryExtension.TryGetValue((IDictionary)_container, name, out result);

				if(_container is IDictionary<string, object>)
					return ((IDictionary<string, object>)_container).TryGetValue(name, out result);

				if(_container is IDictionary<string, string>)
				{
					string text;

					if(((IDictionary<string, string>)_container).TryGetValue(name, out text))
					{
						result = text;
						return true;
					}

					return false;
				}

				if(_cache.TryGetValue(name, out result))
					return true;

				var property = _properties.Find(name, true);

				if(property == null)
					return false;

				value = property.GetValue(_container);

				if(value == null || Common.TypeExtension.IsScalarType(value.GetType()))
					result = _cache[name] = value;
				else
					result = _cache[name] = new ObjectCache(value);

				return true;
			}

			public bool TrySet(string name, object value)
			{
				if(name == null)
					throw new ArgumentNullException("name");

				if(_container is IDictionary)
				{
					((IDictionary)_container)[name] = value;
					return true;
				}

				if(_container is IDictionary<string, object>)
				{
					((IDictionary<string, object>)_container)[name] = value;
					return true;
				}

				if(_container is IDictionary<string, string> && (value == null || value is string))
				{
					((IDictionary<string, string>)_container)[name] = (string)value;
					return true;
				}

				if(_properties != null && _cache != null)
				{
					var property = _properties.Find(name, true);

					if(property == null || property.IsReadOnly)
						return false;

					value = Zongsoft.Common.Convert.ConvertValue(value, property.PropertyType);
					property.SetValue(_container, value);

					if(value == null || Common.TypeExtension.IsScalarType(value.GetType()))
						_cache[name] = value;
					else
						_cache[name] = new ObjectCache(value);

					return true;
				}

				Zongsoft.Common.Convert.SetValue(_container, name, value);
				return true;
			}
			#endregion

			#region 静态方法
			public static bool TryGet(object container, string name, out object result)
			{
				if(container == null)
					throw new ArgumentNullException("container");

				if(container is IDictionary)
					return Zongsoft.Collections.DictionaryExtension.TryGetValue((IDictionary)container, name, out result);

				if(container is IDictionary<string, object>)
					return ((IDictionary<string, object>)container).TryGetValue(name, out result);

				if(container is IDictionary<string, string>)
				{
					result = null;
					string text;

					if(((IDictionary<string, string>)container).TryGetValue(name, out text))
					{
						result = text;
						return true;
					}

					return false;
				}

				if(container is ObjectCache)
					return ((ObjectCache)container).TryGet(name, out result);

				result = Zongsoft.Common.Convert.GetValue(container, name);
				return result != null;
			}

			public static bool TrySet(object container, string name, object value)
			{
				if(container == null)
					throw new ArgumentNullException("container");

				if(container is IDictionary)
				{
					((IDictionary)container)[name] = value;
					return true;
				}

				if(container is IDictionary<string, object>)
				{
					((IDictionary<string, object>)container)[name] = value;
					return true;
				}

				if(container is IDictionary<string, string> && (value == null || value is string))
				{
					((IDictionary<string, string>)container)[name] = (string)value;
					return true;
				}

				if(container is ObjectCache)
					return ((ObjectCache)container).TrySet(name, value);

				Zongsoft.Common.Convert.SetValue(container, name, value);
				return true;
			}
			#endregion
		}
		#endregion
	}
}

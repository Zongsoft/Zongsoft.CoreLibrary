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

		public TMember Get<TMember>(Expression<Func<T, TMember>> member)
		{
			if(member == null)
				throw new ArgumentNullException("member");

			var tuple = ResolveExpression(member, new Stack<MemberInfo>());

			if(tuple == null)
				throw new ArgumentException("Invalid member expression.");

			return (TMember)Zongsoft.Common.Convert.ConvertValue(this.Get(tuple.Item1), tuple.Item2);
		}

		public TMember Get<TMember>(Expression<Func<T, TMember>> member, TMember defaultValue)
		{
			if(member == null)
				throw new ArgumentNullException("member");

			var tuple = ResolveExpression(member, new Stack<MemberInfo>());

			if(tuple == null)
				return defaultValue;

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

		public void Set(string key, object value, Func<object, bool> predicate = null)
		{
			if(!this.TrySet(key, value, predicate))
				throw new KeyNotFoundException(string.Format("The '{0}' property is not existed.", key));
		}

		public void Set<TMember>(Expression<Func<T, TMember>> member, TMember value, Func<TMember, bool> predicate = null)
		{
			if(!this.TrySet(member, value, predicate))
				throw new KeyNotFoundException(string.Format("The '{0}' property is not existed.", member));
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
				throw new KeyNotFoundException(string.Format("The '{0}' property is not existed.", key));

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

		public bool TrySet<TMember>(Expression<Func<T, TMember>> member, TMember value, Func<TMember, bool> predicate = null)
		{
			return this.TrySet<TMember>(member, () => value, predicate);
		}

		public bool TrySet<TMember>(Expression<Func<T, TMember>> member, Expression<Func<T, TMember>> valueExpression, Func<TMember, bool> predicate = null)
		{
			if(member == null)
				throw new ArgumentNullException("member");

			if(valueExpression == null)
				throw new ArgumentNullException("valueExpression");

			return this.TrySet(member, () => this.Get(valueExpression), original =>
			{
				if(predicate == null)
					return this.Contains(valueExpression);
				else
					return predicate(original) && this.Contains(valueExpression);
			});
		}

		public bool TrySet<TMember>(Expression<Func<T, TMember>> member, Func<TMember> valueThunk, Func<TMember, bool> predicate = null)
		{
			if(member == null)
				throw new ArgumentNullException("member");

			if(valueThunk == null)
				throw new ArgumentNullException("valueThunk");

			var tuple = ResolveExpression(member, new Stack<MemberInfo>());

			if(tuple == null)
				throw new ArgumentException("Invalid member expression.");

			if(predicate == null)
				return this.TrySet(tuple.Item1, () => valueThunk(), null);
			else
				return this.TrySet(tuple.Item1, () => valueThunk(), original => predicate((TMember)Zongsoft.Common.Convert.ConvertValue(original, tuple.Item2)));
		}
		#endregion

		#region 私有方法
		private static Tuple<string, Type> ResolveExpression(Expression expression, Stack<MemberInfo> stack)
		{
			var tuple = ResolveExpressionMember(expression, stack);

			switch(tuple.Item2.MemberType)
			{
				case MemberTypes.Property:
					return new Tuple<string, Type>(tuple.Item1, ((PropertyInfo)tuple.Item2).PropertyType);
				case MemberTypes.Field:
					return new Tuple<string, Type>(tuple.Item1, ((FieldInfo)tuple.Item2).FieldType);
				default:
					throw new InvalidOperationException("Invalid expression.");
			}
		}

		private static Tuple<string, MemberInfo> ResolveExpressionMember(Expression expression, Stack<MemberInfo> stack)
		{
			if(expression.NodeType == ExpressionType.Lambda)
				return ResolveExpressionMember(((LambdaExpression)expression).Body, stack);

			if(expression.NodeType == ExpressionType.MemberAccess)
			{
				stack.Push(((MemberExpression)expression).Member);

				if(((MemberExpression)expression).Expression != null)
					return ResolveExpressionMember(((MemberExpression)expression).Expression, stack);
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

			return new Tuple<string, MemberInfo>(path, member);
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

				if(!(container is IDictionary || container is IDictionary<string, object> || container is IDictionary<string, string>))
				{
					_properties = TypeDescriptor.GetProperties(container);
					_cache = new ConcurrentDictionary<string, object>(StringComparer.OrdinalIgnoreCase);
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

				if(_cache.TryGetValue(name, out result))
					return true;

				if(TryGetDictionary(_container, name, out result))
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

			public bool TrySet(string name, Func<object> valueThunk, Func<object, bool> predicate)
			{
				if(name == null)
					throw new ArgumentNullException("name");

				if(TrySetDictionary(_container, name, valueThunk, predicate))
					return true;

				var property = _properties.Find(name, true);

				if(property == null || property.IsReadOnly)
					return false;

				if(predicate != null)
				{
					object original;

					if(!_cache.TryGetValue(name, out original))
						original = property.GetValue(_container);

					if(!predicate(original is ObjectCache ? ((ObjectCache)original).Container : original))
						return false;
				}

				//获取值并做类型转换
				var value = Zongsoft.Common.Convert.ConvertValue(valueThunk(), property.PropertyType);

				//首先设置容器对象的属性值
				property.SetValue(_container, value);

				if(value == null || Common.TypeExtension.IsScalarType(value.GetType()))
					_cache[name] = value;
				else
					_cache[name] = new ObjectCache(value);

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

				result = Zongsoft.Common.Convert.GetValue(container, name);
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

				bool isTerminated = false;

				Zongsoft.Common.Convert.SetValue(container, name, valueThunk, ctx =>
				{
					if(ctx.Direction == Common.Convert.ObjectResolvingDirection.Set && predicate != null)
						isTerminated = ctx.IsTerminated = !predicate(ctx.GetMemberValue());
				});

				return !isTerminated;
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
		}
		#endregion
	}
}

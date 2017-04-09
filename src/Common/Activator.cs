/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2017 Zongsoft Corporation <http://www.zongsoft.com>
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
using System.Linq;
using System.Reflection;

namespace Zongsoft.Common
{
	public static class Activator
	{
		#region 成员字段
		private static readonly ICollection<IActivator> _activators = new HashSet<IActivator>(new IActivator[] { new ObjectActivator(), new CollectionActivator() });
		#endregion

		#region 公共属性
		public static ICollection<IActivator> Activators
		{
			get
			{
				return _activators;
			}
		}
		#endregion

		#region 公共方法
		public static object CreateInstance(Type type, Func<ActivatorParameterDescriptor, bool> binder = null)
		{
			return CreateInstance(type, null, binder);
		}

		public static object CreateInstance(Type type, object argument, Func<ActivatorParameterDescriptor, bool> binder = null)
		{
			if(type == null)
				throw new ArgumentNullException(nameof(type));

			var activators = Activators;

			if(activators != null)
			{
				foreach(var activator in activators)
				{
					if(activator != null && activator.CanCreate(type, argument))
						return activator.Create(type, argument);
				}
			}

			if(argument == null)
				return System.Activator.CreateInstance(type);

			object[] args = null;

			if(argument.GetType().IsArray)
			{
				if(argument.GetType().GetElementType() == typeof(object))
					args = (object[])argument;
				else
				{
					args = new object[((Array)argument).GetLength(0)];
					Array.Copy((Array)argument, args, args.Length);
				}
			}
			else
			{
				args = new object[] { argument };
			}

			return System.Activator.CreateInstance(type, args);
		}

		public static T CreateInstance<T>(Func<ActivatorParameterDescriptor, bool> binder = null)
		{
			return (T)CreateInstance(typeof(T), null, binder);
		}

		public static T CreateInstance<T>(object argument, Func<ActivatorParameterDescriptor, bool> binder = null)
		{
			return (T)CreateInstance(typeof(T), argument, binder);
		}
		#endregion

		#region 嵌套子类
		private class ObjectActivator : IActivator
		{
			#region 公共方法
			public bool CanCreate(Type type, object argument)
			{
				return type != null && argument != null && argument.GetType().IsDictionary();
			}

			public object Create(Type type, object argument, Func<ActivatorParameterDescriptor, bool> binder = null)
			{
				if(type == null)
					throw new ArgumentNullException(nameof(type));

				if(argument == null || !argument.GetType().IsDictionary())
					return null;

				var constructors = type.GetConstructors(BindingFlags.Public).OrderByDescending(p => p.GetParameters().Length);

				foreach(var constructor in constructors)
				{
					object[] values;

					if(this.TryGetParameters(constructor, argument, binder, out values))
						return constructor.Invoke(values);
				}

				return null;
			}
			#endregion

			#region 私有方法
			private bool TryGetParameters(ConstructorInfo constructor, object argument, Func<ActivatorParameterDescriptor, bool> binder, out object[] values)
			{
				var parameters = constructor.GetParameters();
				values = new object[parameters.Length];

				if(binder == null)
					binder = ctx => this.TryGetParamterValue(ctx);

				for(int i = 0; i < parameters.Length; i++)
				{
					var context = new ActivatorParameterDescriptor(parameters[i].Name, parameters[i].ParameterType, argument);

					if(binder(context))
						values[i] = context.ParameterValue;
					else
						return false;
				}

				return true;
			}

			private bool TryGetParamterValue(ActivatorParameterDescriptor context)
			{
				var entries = Zongsoft.Collections.DictionaryExtension.ToDictionary((IEnumerable)context.Argument);

				foreach(var entry in entries)
				{
					//判断当前参数名是否与字典条目的键相同
					var found = string.Equals(entry.Key?.ToString(), context.ParameterName, StringComparison.OrdinalIgnoreCase);

					if(found)
					{
						object value;

						if(Common.Convert.TryConvertValue(entry.Value, context.ParameterType, out value))
						{
							context.ParameterValue = value;
							return true;
						}
						else
						{
							return false;
						}
					}
				}

				return false;
			}
			#endregion
		}

		private class CollectionActivator : IActivator
		{
			public bool CanCreate(Type type, object argument)
			{
				return type != null && type != typeof(string) && (type.IsArray || type.IsCollection() || type.IsDictionary());
			}

			public object Create(Type type, object argument, Func<ActivatorParameterDescriptor, bool> binder = null)
			{
				object result = null;

				if(type.IsArray)
				{
					var elementType = type.GetElementType();

					if(argument != null && argument is IEnumerable && result is IList)
					{
						var list = (IList)System.Activator.CreateInstance(typeof(List<>).MakeGenericType(elementType));

						foreach(var entry in (IEnumerable)argument)
						{
							list.Add(Convert.ConvertValue(entry, elementType));
						}

						result = Array.CreateInstance(elementType, list.Count);

						for(int i = 0; i < list.Count; i++)
						{
							((Array)result).SetValue(list[i], i);
						}

						return result;
					}

					return Array.CreateInstance(elementType, 0);
				}

				if(TypeExtension.IsAssignableFrom(typeof(IDictionary<,>), type))
				{
					var types = type.GetGenericArguments();

					if(type.IsInterface)
						result = System.Activator.CreateInstance(typeof(Dictionary<,>).MakeGenericType(types));
					else
						result = System.Activator.CreateInstance(type);

					if(argument != null && result is IDictionary)
					{
						var entries = Collections.DictionaryExtension.ToDictionary((IEnumerable)argument);

						foreach(var entry in entries)
						{
							((IDictionary)result).Add(
								Convert.ConvertValue(entry.Key, types[0]),
								Convert.ConvertValue(entry.Value, types[1]));
						}
					}
				}

				if(typeof(IDictionary).IsAssignableFrom(type))
				{
					if(type.IsInterface)
						result = System.Activator.CreateInstance<Dictionary<object, object>>();
					else
						result = System.Activator.CreateInstance(type);

					if(argument != null && result is IDictionary)
					{
						var entries = Collections.DictionaryExtension.ToDictionary((IEnumerable)argument);

						foreach(var entry in entries)
						{
							((IDictionary)result).Add(entry.Key, entry.Value);
						}
					}
				}

				if(TypeExtension.IsAssignableFrom(typeof(ICollection<>), type))
				{
					var types = type.GetGenericArguments();

					if(type.IsInterface)
						result = System.Activator.CreateInstance(typeof(List<>).MakeGenericType(types));
					else
						result = System.Activator.CreateInstance(type);

					if(argument != null && argument is IEnumerable && result is IList)
					{
						foreach(var entry in (IEnumerable)argument)
						{
							((IList)result).Add(Convert.ConvertValue(entry, types[0]));
						}
					}
				}

				if(typeof(IList).IsAssignableFrom(type))
				{
					if(type.IsInterface)
						result = System.Activator.CreateInstance<List<object>>();
					else
						result = System.Activator.CreateInstance(type);

					if(argument != null && argument is IEnumerable)
					{
						foreach(var entry in (IEnumerable)argument)
						{
							((IList)result).Add(entry);
						}
					}
				}

				if(typeof(ICollection).IsAssignableFrom(type))
				{
					if(type.IsInterface)
						result = System.Activator.CreateInstance<List<object>>();
					else
						result = System.Activator.CreateInstance(type);
				}

				return result;
			}
		}
		#endregion
	}
}

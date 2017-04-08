/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2015-2016 Zongsoft Corporation <http://www.zongsoft.com>
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
using System.Reflection;

using Zongsoft.Common;
using Zongsoft.Reflection;
using Zongsoft.Collections;

namespace Zongsoft.Runtime.Serialization
{
	public class DictionarySerializer : IDictionarySerializer
	{
		#region 单例字段
		public static readonly DictionarySerializer Default = new DictionarySerializer();
		#endregion

		#region 序列方法
		public IDictionary Serialize(object graph)
		{
			var dictionary = new Dictionary<string, object>();
			this.Serialize(graph, dictionary);
			return dictionary;
		}

		public void Serialize(object graph, IDictionary dictionary)
		{
			if(graph == null)
				return;

			if(dictionary == null)
				throw new ArgumentNullException(nameof(dictionary));

			this.Serialize(graph, dictionary, null, new HashSet<object>());
		}

		private void Serialize(object graph, IDictionary dictionary, string prefix, HashSet<object> hashset)
		{
			if(graph == null || hashset.Contains(graph))
				return;

			if(dictionary == null)
				throw new ArgumentNullException(nameof(dictionary));

			if(graph.GetType().IsScalarType())
			{
				if(graph.GetType().IsArray)
				{
					var index = 0;

					foreach(var entry in (IEnumerable)graph)
					{
						dictionary.Add((string.IsNullOrEmpty(prefix) ? string.Empty : prefix) + $"[{index++}]", entry);
					}
				}
				else
				{
					dictionary.Add(string.IsNullOrEmpty(prefix) ? string.Empty : prefix, graph);
				}

				return;
			}

			//将当前序列化对象加入到已解析的栈中
			hashset.Add(graph);

			//将当前序列化对象的类型名加入到字典中
			dictionary.Add(string.IsNullOrEmpty(prefix) ? "$type" : "$type:" + prefix, graph.GetType().AssemblyQualifiedName);

			//获取当前序列化对象的属性集
			var properties = graph.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);

			foreach(var property in properties)
			{
				if(!property.CanRead || property.GetIndexParameters().Length > 0)
					continue;

				var key = string.IsNullOrEmpty(prefix) ? property.Name : prefix + "." + property.Name;

				if(property.PropertyType.IsScalarType())
					dictionary.Add(key, property.GetValue(graph));
				else
				{
					if(property.PropertyType.IsDictionary())
					{
						var entries = (IEnumerable)property.GetValue(graph);

						foreach(var entry in entries)
						{
							var entryKey = entry.GetType().GetProperty("Key", BindingFlags.Public | BindingFlags.Instance).GetValue(entry);
							var entryValue = entry.GetType().GetProperty("Value", BindingFlags.Public | BindingFlags.Instance).GetValue(entry);

							this.Serialize(entry, dictionary, key + $"[{entryKey}]", hashset);
						}
					}
					else if(property.PropertyType.IsEnumerable())
					{
						var entries = (IEnumerable)property.GetValue(graph);
						var index = 0;

						foreach(var entry in entries)
						{
							this.Serialize(entry, dictionary, key + $"[{index++}]", hashset);
						}
					}
					else
					{
						this.Serialize(property.GetValue(graph), dictionary, key, hashset);
					}
				}
			}
		}
		#endregion

		#region 反序列化
		public T Deserialize<T>(IDictionary dictionary)
		{
			return (T)this.Deserialize(dictionary, typeof(T), null);
		}

		public T Deserialize<T>(IDictionary dictionary, Func<MemberGettingContext, MemberGettingResult> resolve)
		{
			return (T)this.Deserialize(dictionary, typeof(T), resolve);
		}

		public object Deserialize(IDictionary dictionary, Type type)
		{
			return this.Deserialize(dictionary, type, null);
		}

		public object Deserialize(IDictionary dictionary, Type type, Func<MemberGettingContext, MemberGettingResult> resolve)
		{
			if(dictionary == null)
				throw new ArgumentNullException(nameof(dictionary));
			if(type == null)
				throw new ArgumentNullException(nameof(type));

			var result = ActivatorProvider.Default.CreateInstance(type, dictionary);

			if(result == null)
				return null;

			var properties = result.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

			foreach(var property in properties)
			{
				if(!property.CanWrite || property.GetIndexParameters().Length > 0)
					continue;

				dictionary.TryGetValue(property.Name, value =>
				{
					object propertyValue;

					if(Common.Convert.TryConvertValue(value, property.PropertyType, out propertyValue))
						property.SetValue(result, propertyValue);
				});
			}

			throw new NotImplementedException();
		}
		#endregion
	}
}

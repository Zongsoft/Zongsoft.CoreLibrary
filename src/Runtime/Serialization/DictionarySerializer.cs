/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2015 Zongsoft Corporation <http://www.zongsoft.com>
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

using Zongsoft.Common;

namespace Zongsoft.Runtime.Serialization
{
	public class DictionarySerializer : IDictionarySerializer
	{
		#region 单例字段
		public static readonly DictionarySerializer Default = new DictionarySerializer();
		#endregion

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
				throw new ArgumentNullException("dictionary");

			throw new NotImplementedException();
		}

		public object Deserialize(IDictionary dictionary, Type type = null)
		{
			return this.Deserialize(dictionary, type, null);
		}

		public object Deserialize(IDictionary dictionary, Type type, Action<Zongsoft.Common.Convert.ObjectResolvingContext> resolve)
		{
			if(type == null)
				throw new ArgumentNullException("type");

			return this.Deserialize<object>(dictionary, () => Activator.CreateInstance(type), resolve);
		}

		public T Deserialize<T>(IDictionary dictionary, Func<T> creator = null)
		{
			return this.Deserialize<T>(dictionary, creator, null);
		}

		public T Deserialize<T>(IDictionary dictionary, Func<T> creator, Action<Zongsoft.Common.Convert.ObjectResolvingContext> resolve)
		{
			if(dictionary == null)
				return default(T);

			var result = creator != null ? creator() : Activator.CreateInstance<T>();

			if(resolve == null)
			{
				resolve = ctx =>
				{
					if(ctx.Direction == Zongsoft.Common.Convert.ObjectResolvingDirection.Get)
					{
						ctx.Value = ctx.GetMemberValue();

						if(ctx.Value == null)
						{
							ctx.Value = Activator.CreateInstance(ctx.MemberType);

							switch(ctx.Member.MemberType)
							{
								case MemberTypes.Field:
									((FieldInfo)ctx.Member).SetValue(ctx.Container, ctx.Value);
									break;
								case MemberTypes.Property:
									((PropertyInfo)ctx.Member).SetValue(ctx.Container, ctx.Value);
									break;
							}
						}
					}
				};
			}

			foreach(KeyValuePair<string, object> entry in dictionary)
			{
				if(string.IsNullOrWhiteSpace(entry.Key))
					continue;

				Zongsoft.Common.Convert.SetValue(result, entry.Key, entry.Value, resolve);
			}

			return result;
		}
	}
}

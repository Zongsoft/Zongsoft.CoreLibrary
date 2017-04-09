/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2010-2015 Zongsoft Corporation <http://www.zongsoft.com>
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

namespace Zongsoft.Collections
{
	public static class DictionaryExtension
	{
		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.Synchronized)]
		public static bool TryGetValue(this IDictionary dictionary, object key, out object value)
		{
			value = null;

			if(dictionary == null || dictionary.Count < 1)
				return false;

			var existed = dictionary.Contains(key);

			if(existed)
				value = dictionary[key];

			return existed;
		}

		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.Synchronized)]
		public static bool TryGetValue(this IDictionary dictionary, object key, Action<object> onGot)
		{
			if(dictionary == null || dictionary.Count < 1)
				return false;

			var existed = dictionary.Contains(key);

			if(existed && onGot != null)
				onGot(dictionary[key]);

			return existed;
		}

		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.Synchronized)]
		public static bool TryGetValue<TValue>(this IDictionary dictionary, object key, out TValue value)
		{
			value = default(TValue);

			if(dictionary == null || dictionary.Count < 1)
				return false;

			var existed = dictionary.Contains(key);

			if(existed)
				value = Zongsoft.Common.Convert.ConvertValue<TValue>(dictionary[key]);

			return existed;
		}

		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.Synchronized)]
		public static bool TryGetValue<TValue>(this IDictionary dictionary, object key, Action<object> onGot)
		{
			if(dictionary == null || dictionary.Count < 1)
				return false;

			var existed = dictionary.Contains(key);

			if(existed && onGot != null)
				onGot(Zongsoft.Common.Convert.ConvertValue<TValue>(dictionary[key]));

			return existed;
		}

		public static bool TryGetValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Action<TValue> onGot)
		{
			if(dictionary == null || dictionary.Count < 1)
				return false;

			TValue value;

			if(dictionary.TryGetValue(key, out value) && onGot != null)
			{
				onGot(value);
				return true;
			}

			return false;
		}

		public static IDictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IDictionary dictionary, Func<object, TKey> keyConvert = null, Func<object, TValue> valueConvert = null)
		{
			if(dictionary == null)
				return null;

			if(keyConvert == null)
				keyConvert = key => Zongsoft.Common.Convert.ConvertValue<TKey>(key);

			if(valueConvert == null)
				valueConvert = value => Zongsoft.Common.Convert.ConvertValue<TValue>(value);

			var result = new Dictionary<TKey, TValue>(dictionary.Count);

			foreach(DictionaryEntry entry in dictionary)
			{
				result.Add(keyConvert(entry.Key), valueConvert(entry.Value));
			}

			return result;
		}

		public static IEnumerable<DictionaryEntry> ToDictionary(this IEnumerable source)
		{
			if(source == null)
				yield break;

			if(source is IDictionary || source is IEnumerable<DictionaryEntry>)
			{
				foreach(var item in source)
					yield return (DictionaryEntry)item;
			}
			else
			{
				foreach(var item in source)
				{
					if(item == null)
						continue;

					if(item.GetType().IsGenericType && item.GetType().GetGenericTypeDefinition() == typeof(KeyValuePair<,>))
					{
						yield return new DictionaryEntry(
							item.GetType().GetProperty("Key", BindingFlags.Public | BindingFlags.Instance).GetValue(item),
							item.GetType().GetProperty("Value", BindingFlags.Public | BindingFlags.Instance).GetValue(item));
					}
				}
			}
		}
	}
}

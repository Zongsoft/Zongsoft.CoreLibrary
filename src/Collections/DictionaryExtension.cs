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

namespace Zongsoft.Collections
{
	public static class DictionaryExtension
	{
		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.Synchronized)]
		public static bool TryGetValue(this IDictionary dictionary, object key, out object value)
		{
			value = null;

			if(dictionary == null)
				return false;

			var result = dictionary.Contains(key);

			if(result)
				value = dictionary[key];

			return result;
		}

		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.Synchronized)]
		public static bool TryGetValue<TValue>(this IDictionary dictionary, object key, out TValue value)
		{
			value = default(TValue);

			if(dictionary == null)
				return false;

			var result = dictionary.Contains(key);

			if(result)
				value = Zongsoft.Common.Convert.ConvertValue<TValue>(dictionary[key]);

			return result;
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
	}
}

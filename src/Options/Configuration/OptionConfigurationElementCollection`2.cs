/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2013 Zongsoft Corporation <http://www.zongsoft.com>
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
using System.Collections.Generic;

namespace Zongsoft.Options.Configuration
{
	public class OptionConfigurationElementCollection<TElement, TContract> : OptionConfigurationElementCollection, ICollection<TContract>, IReadOnlyDictionary<string, TContract>, Collections.INamedCollection<TContract> where TElement : OptionConfigurationElement, TContract
	{
		#region 构造函数
		public OptionConfigurationElementCollection(string elementName, IEqualityComparer<string> comparer = null) : base(elementName, comparer)
		{
		}

		protected OptionConfigurationElementCollection()
		{
		}
		#endregion

		#region 公共属性
		public TContract this[int index]
		{
			get
			{
				return (TElement)base.GetElement(index);
			}
		}

		public new TContract this[string key]
		{
			get
			{
				return (TElement)base.GetElement(key);
			}
		}
		#endregion

		#region 重写方法
		protected override OptionConfigurationElement CreateNewElement()
		{
			return Activator.CreateInstance<TElement>();
		}

		protected override string GetElementKey(OptionConfigurationElement element)
		{
			var property = OptionConfigurationUtility.GetKeyProperty(element);
			if(property == null)
				throw new OptionConfigurationException();

			var value = element[property];
			if(value == null)
				throw new OptionConfigurationException();

			return (string)value.ToString();
		}
		#endregion

		#region 显式实现
		TContract Collections.INamedCollection<TContract>.Get(string name)
		{
			var result = base.GetElement(name);

			if(result == null)
				throw new KeyNotFoundException();

			return (TElement)result;
		}

		bool Collections.INamedCollection<TContract>.TryGet(string name, out TContract value)
		{
			value = (TElement)base.GetElement(name);
			return value != null;
		}

		bool ICollection<TContract>.IsReadOnly
		{
			get
			{
				return false;
			}
		}

		IEnumerable<string> IReadOnlyDictionary<string, TContract>.Keys
		{
			get
			{
				return base.InnerDictionary.Keys;
			}
		}

		IEnumerable<TContract> IReadOnlyDictionary<string, TContract>.Values
		{
			get
			{
				foreach(var item in base.InnerDictionary.Values)
				{
					yield return (TElement)item;
				}
			}
		}

		void ICollection<TContract>.Add(TContract item)
		{
			var element = item as OptionConfigurationElement;

			if(element == null)
				throw new ArgumentException();

			base.Add(element);
		}

		bool ICollection<TContract>.Contains(TContract item)
		{
			var element = item as OptionConfigurationElement;

			if(element == null)
				throw new ArgumentException();

			return base.Contains(element);
		}

		void ICollection<TContract>.CopyTo(TContract[] array, int arrayIndex)
		{
			if(array == null)
				throw new ArgumentNullException(nameof(array));

			if(array.Length == 0)
				return;

			if(arrayIndex < 0 || arrayIndex >= array.Length)
				throw new ArgumentOutOfRangeException(nameof(arrayIndex));

			var index = 0;

			foreach(var value in this.InnerDictionary.Values)
			{
				array[arrayIndex + index++] = (TElement)value;
			}
		}

		bool ICollection<TContract>.Remove(TContract item)
		{
			var element = item as OptionConfigurationElement;

			if(element == null)
				throw new ArgumentException();

			return base.Remove(element);
		}

		bool IReadOnlyDictionary<string, TContract>.ContainsKey(string key)
		{
			return base.InnerDictionary.ContainsKey(key);
		}

		bool IReadOnlyDictionary<string, TContract>.TryGetValue(string key, out TContract value)
		{
			OptionConfigurationElement element;

			if(base.InnerDictionary.TryGetValue(key, out element))
			{
				value = (TElement)element;
				return true;
			}

			value = default(TContract);
			return false;
		}

		IEnumerator<TContract> IEnumerable<TContract>.GetEnumerator()
		{
			var iterator = base.GetEnumerator();

			while(iterator.MoveNext())
			{
				yield return (TElement)iterator.Current;
			}
		}

		IEnumerator<KeyValuePair<string, TContract>> IEnumerable<KeyValuePair<string, TContract>>.GetEnumerator()
		{
			var iterator = base.InnerDictionary.GetEnumerator();

			while(iterator.MoveNext())
			{
				yield return new KeyValuePair<string, TContract>(iterator.Current.Key, (TElement)iterator.Current.Value);
			}
		}
		#endregion
	}
}

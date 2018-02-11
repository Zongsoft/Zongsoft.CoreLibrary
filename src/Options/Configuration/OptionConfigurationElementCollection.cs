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
using System.Xml;

namespace Zongsoft.Options.Configuration
{
	public abstract class OptionConfigurationElementCollection : OptionConfigurationElement, IReadOnlyDictionary<string, OptionConfigurationElement>, ICollection<OptionConfigurationElement>, Collections.INamedCollection<OptionConfigurationElement>
	{
		#region 成员字段
		private string _elementName;
		private IDictionary<string, OptionConfigurationElement> _dictionary;
		#endregion

		#region 构造函数
		protected OptionConfigurationElementCollection() : this(string.Empty, StringComparer.OrdinalIgnoreCase)
		{
		}

		protected OptionConfigurationElementCollection(string elementName) : this(elementName, null)
		{
		}

		protected OptionConfigurationElementCollection(IEqualityComparer<string> comparer) : this(string.Empty, comparer)
		{
		}

		protected OptionConfigurationElementCollection(string elementName, IEqualityComparer<string> comparer)
		{
			_elementName = elementName == null ? string.Empty : elementName.Trim();
			_dictionary = new Dictionary<string, OptionConfigurationElement>(comparer ?? StringComparer.OrdinalIgnoreCase);
		}
		#endregion

		#region 公共属性
		public int Count
		{
			get
			{
				return _dictionary.Count;
			}
		}
		#endregion

		#region 保护属性
		protected IDictionary<string, OptionConfigurationElement> InnerDictionary
		{
			get
			{
				return _dictionary;
			}
		}

		protected internal virtual string ElementName
		{
			get
			{
				return _elementName;
			}
		}
		#endregion

		#region 公共方法
		public void Add(OptionConfigurationElement item)
		{
			if(item == null)
				throw new ArgumentNullException("item");

			string key = this.GetElementKey(item);

			if(key != null)
				_dictionary.Add(key, item);
		}

		public void Clear()
		{
			_dictionary.Clear();
		}

		public bool Contains(string key)
		{
			return _dictionary.ContainsKey(key);
		}

		public bool Contains(OptionConfigurationElement item)
		{
			return _dictionary.Values.Contains(item);
		}

		public void CopyTo(OptionConfigurationElement[] array, int arrayIndex)
		{
			_dictionary.Values.CopyTo(array, arrayIndex);
		}

		public bool Remove(string key)
		{
			return _dictionary.Remove(key);
		}

		public bool Remove(OptionConfigurationElement item)
		{
			if(item == null)
				return false;

			var key = this.GetElementKey(item);
			return _dictionary.Remove(key);
		}
		#endregion

		#region 保护方法
		protected OptionConfigurationElement GetElement(int index)
		{
			var items = _dictionary.Values;

			if(index < 0 || index >= items.Count)
				throw new IndexOutOfRangeException();

			var iterator = items.GetEnumerator();

			for(int i = 0; i < index; i++)
			{
				iterator.MoveNext();
			}

			if(iterator.MoveNext())
				return iterator.Current;

			return null;
		}

		protected OptionConfigurationElement GetElement(string key)
		{
			OptionConfigurationElement result;

			if(_dictionary.TryGetValue(key, out result))
				return result;

			return null;
		}

		protected object GetAttributeValue(string name)
		{
			if(string.IsNullOrEmpty(name))
				throw new ArgumentNullException(nameof(name));

			OptionConfigurationProperty property;

			if(this.Properties.TryGetValue(name, out property))
				return this[property];

			throw new OptionConfigurationException(string.Format("The '{0}' attribute is not existed in the configuration collection.", name));
		}

		protected bool SetAttributeValue(string name, object value)
		{
			if(string.IsNullOrEmpty(name))
				throw new ArgumentNullException(nameof(name));

			OptionConfigurationProperty property;

			var result = this.Properties.TryGetValue(name, out property);

			if(result)
				this[property] = value;

			return result;
		}
		#endregion

		#region 抽象方法
		protected abstract OptionConfigurationElement CreateNewElement();
		protected abstract string GetElementKey(OptionConfigurationElement element);
		#endregion

		#region 重写方法
		/// <summary>
		/// 重写了基类默认读取选项配置XML文件内容的逻辑。
		/// </summary>
		/// <param name="reader">在选项配置文件中进行读取操作的<seealso cref="System.Xml.XmlReader"/>读取器。</param>
		/// <remarks>
		///		<para>注意：该方法传入的<paramref name="reader"/>参数的位置为集合中元素对应的XML节点。</para>
		/// </remarks>
		protected internal override void DeserializeElement(XmlReader reader)
		{
			if(reader.ReadState == ReadState.Initial)
			{
				if(!reader.Read())
					throw new OptionConfigurationException();
			}

			if(reader.NodeType == XmlNodeType.Element)
			{
				var elementName = this.ElementName;

				if(this.ElementProperty != null && !string.IsNullOrWhiteSpace(this.ElementProperty.ElementName))
					elementName = this.ElementProperty.ElementName;

				//如果当前配置属性定义项是默认集合(即其没有对应的XML集合元素)，则必须检查当前XML元素的名称是否与默认集合的元素名相同，如果不同则配置文件内容非法而抛出异常
				if(!string.Equals(reader.Name, elementName, StringComparison.OrdinalIgnoreCase))
					throw new OptionConfigurationException(string.Format("The '{0}' option configuration collection is unrecognized.", reader.Name));

				//创建集合元素对象
				var element = this.CreateNewElement();

				//调用元素的反序列化方法
				element.DeserializeElement(reader);

				//将集合元素对象加入到当前集合中
				this.Add(element);
			}
		}

		protected internal override void SerializeElement(XmlWriter writer)
		{
			var collectionName = this.ElementProperty != null ? this.ElementProperty.Name : string.Empty;

			if(!string.IsNullOrEmpty(collectionName))
				writer.WriteStartElement(collectionName);

			foreach(var item in _dictionary.Values)
			{
				writer.WriteStartElement(this.ElementName);
				item.SerializeElement(writer);
				writer.WriteEndElement();
			}

			if(!string.IsNullOrEmpty(collectionName))
				writer.WriteEndElement();
		}
		#endregion

		#region 显式实现
		OptionConfigurationElement Collections.INamedCollection<OptionConfigurationElement>.Get(string name)
		{
			if(name == null)
				throw new ArgumentNullException(nameof(name));

			OptionConfigurationElement element;

			if(_dictionary.TryGetValue(name, out element))
				return element;

			throw new KeyNotFoundException();
		}

		bool Collections.INamedCollection<OptionConfigurationElement>.TryGet(string name, out OptionConfigurationElement value)
		{
			if(name == null)
				throw new ArgumentNullException(nameof(name));

			return _dictionary.TryGetValue(name, out value);
		}

		bool ICollection<OptionConfigurationElement>.IsReadOnly
		{
			get
			{
				return false;
			}
		}

		IEnumerable<string> IReadOnlyDictionary<string, OptionConfigurationElement>.Keys
		{
			get
			{
				return _dictionary.Keys;
			}
		}

		IEnumerable<OptionConfigurationElement> IReadOnlyDictionary<string, OptionConfigurationElement>.Values
		{
			get
			{
				return _dictionary.Values;
			}
		}

		OptionConfigurationElement IReadOnlyDictionary<string, OptionConfigurationElement>.this[string key]
		{
			get
			{
				return _dictionary[key];
			}
		}

		bool IReadOnlyDictionary<string, OptionConfigurationElement>.ContainsKey(string key)
		{
			return _dictionary.ContainsKey(key);
		}

		bool IReadOnlyDictionary<string, OptionConfigurationElement>.TryGetValue(string key, out OptionConfigurationElement value)
		{
			return _dictionary.TryGetValue(key, out value);
		}
		#endregion

		#region 遍历枚举
		public IEnumerator<OptionConfigurationElement> GetEnumerator()
		{
			foreach(var item in _dictionary)
			{
				yield return item.Value;
			}
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		IEnumerator<KeyValuePair<string, OptionConfigurationElement>> IEnumerable<KeyValuePair<string, OptionConfigurationElement>>.GetEnumerator()
		{
			return _dictionary.GetEnumerator();
		}
		#endregion
	}
}

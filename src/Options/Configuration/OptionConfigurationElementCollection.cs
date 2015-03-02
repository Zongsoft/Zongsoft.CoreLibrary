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
using System.Text;
using System.Xml;

namespace Zongsoft.Options.Configuration
{
	public abstract class OptionConfigurationElementCollection : OptionConfigurationElement, ICollection<OptionConfigurationElement>
	{
		#region 成员字段
		private string _elementName;
		private IList<OptionConfigurationElement> _items;
		private IEqualityComparer<string> _comparer;
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
			_comparer = comparer ?? StringComparer.OrdinalIgnoreCase;
			_items = new List<OptionConfigurationElement>();
		}
		#endregion

		#region 公共属性
		public int Count
		{
			get
			{
				return _items.Count;
			}
		}

		bool ICollection<OptionConfigurationElement>.IsReadOnly
		{
		    get
		    {
		        return false;
		    }
		}
		#endregion

		#region 保护属性
		protected IList<OptionConfigurationElement> Items
		{
			get
			{
				return _items;
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

			lock(_items)
			{
				foreach(var existedItem in _items)
				{
					if(_comparer.Equals(this.GetElementKey(existedItem), key))
						throw new OptionConfigurationException();
				}

				_items.Add(item);
			}
		}

		public void Clear()
		{
			lock(_items)
			{
				_items.Clear();
			}
		}

		public bool ContainsKey(string key)
		{
			lock(_items)
			{
				foreach(var item in _items)
				{
					if(_comparer.Equals(this.GetElementKey(item), key))
						return true;
				}
			}

			return false;
		}

		public bool Contains(OptionConfigurationElement item)
		{
			return _items.Contains(item);
		}

		public void CopyTo(OptionConfigurationElement[] array, int arrayIndex)
		{
			_items.CopyTo(array, arrayIndex);
		}

		public bool Remove(string key)
		{
			lock(_items)
			{
				foreach(var item in _items)
				{
					if(_comparer.Equals(this.GetElementKey(item), key))
						return _items.Remove(item);
				}
			}

			return false;
		}

		public bool Remove(OptionConfigurationElement item)
		{
			lock(_items)
			{
				return _items.Remove(item);
			}
		}

		public void RemoveAt(int index)
		{
			lock(_items)
			{
				_items.RemoveAt(index);
			}
		}
		#endregion

		#region 保护方法
		protected OptionConfigurationElement Find(string key)
		{
			lock(_items)
			{
				foreach(var item in _items)
				{
					if(_comparer.Equals(this.GetElementKey(item), key))
						return item;
				}
			}

			return null;
		}
		#endregion

		#region 抽象方法
		protected abstract OptionConfigurationElement CreateNewElement();
		protected abstract string GetElementKey(OptionConfigurationElement element);
		#endregion

		#region 重写方法
		/// <summary>
		/// 重写了基类默认读取选项配置XML文件内容的逻辑，重写逻辑请参考备注说明。
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
				//如果当前配置属性定义项是默认集合(即其没有对应的XML集合元素)，则必须检查当前XML元素的名称是否与默认集合的元素名相同，如果不同则配置文件内容非法而抛出异常
				if(!string.Equals(reader.Name, this.ElementName, StringComparison.OrdinalIgnoreCase))
					throw new OptionConfigurationException();

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

			foreach(var item in _items)
			{
				writer.WriteStartElement(this.ElementName);
				item.SerializeElement(writer);
				writer.WriteEndElement();
			}

			if(!string.IsNullOrEmpty(collectionName))
				writer.WriteEndElement();
		}
		#endregion

		#region 遍历枚举
		public IEnumerator<OptionConfigurationElement> GetEnumerator()
		{
			foreach(var item in _items)
			{
				yield return item;
			}
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			foreach(var item in _items)
			{
				yield return item;
			}
		}
		#endregion
	}
}

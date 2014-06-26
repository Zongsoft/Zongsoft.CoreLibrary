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
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Zongsoft.Options.Configuration
{
	public class OptionConfigurationElement : System.ComponentModel.INotifyPropertyChanged
	{
		#region 事件声明
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
		#endregion

		#region 静态字段
		private static readonly Dictionary<Type, OptionConfigurationPropertyCollection> _propertyBags = new Dictionary<Type,OptionConfigurationPropertyCollection>();
		#endregion

		#region 实例字段
		private OptionConfigurationProperty _elementProperty;
		private Dictionary<OptionConfigurationProperty, object> _values;
		#endregion

		#region 构造函数
		protected OptionConfigurationElement()
		{
			_values = new Dictionary<OptionConfigurationProperty, object>();
		}
		#endregion

		#region 保护属性
		protected object this[string name]
		{
			get
			{
				OptionConfigurationProperty property;

				if(!this.Properties.TryGetValue(name, out property))
					throw new InvalidOperationException();

				object value;

				if(_values.TryGetValue(property, out value))
					return value;

				return property.DefaultValue;
			}
			set
			{
				OptionConfigurationProperty property;

				if(!this.Properties.TryGetValue(name, out property))
					throw new InvalidOperationException();

				this.SetPropertyValue(property, value);

				//激发“PropertyChanged”事件
				this.RaisePropertyChanged(name);
			}
		}

		protected internal object this[OptionConfigurationProperty property]
		{
			get
			{
				if(property == null)
					throw new ArgumentNullException("property");

				object value;

				if(_values.TryGetValue(property, out value))
					return value;

				return property.DefaultValue;
			}
			set
			{
				this.SetPropertyValue(property, value);

				//激发“PropertyChanged”事件
				this.RaisePropertyChanged(property.Name);
			}
		}

		protected internal OptionConfigurationProperty ElementProperty
		{
			get
			{
				return _elementProperty;
			}
		}

		protected internal virtual OptionConfigurationPropertyCollection Properties
		{
			get
			{
				return GetOptionPropertiesFromType(this.GetType());
			}
		}
		#endregion

		#region 虚拟方法
		/// <summary>
		/// 读取选项配置文件中的XML内容。
		/// </summary>
		/// <param name="reader">在选项配置文件中进行读取操作的<seealso cref="System.Xml.XmlReader"/>读取器。</param>
		protected internal virtual void DeserializeElement(XmlReader reader)
		{
			if(reader.ReadState == ReadState.Initial)
			{
				if(!reader.Read())
					throw new OptionConfigurationException();
			}

			OptionConfigurationProperty property;

			if(reader.NodeType == XmlNodeType.Element)
			{
				for(int i = 0; i < reader.AttributeCount; i++)
				{
					reader.MoveToAttribute(i);

					//XML特性名中间含点的，表示是系统内置特性因而无需解析处理
					if(reader.Name.Contains("."))
						continue;

					//获取当前XML元素特性对应的配置属性定义项
					if(this.Properties.TryGetValue(reader.Name, out property))
						this.SetPropertyValue(property, reader.Value);
				}

				//将当前读取器移到元素上
				reader.MoveToElement();
			}

			//如果当前XML元素是空元素(即其没有子节点的元素)，则返回
			if(reader.IsEmptyElement)
				return;

			//定义当前读取器的初始深度
			int depth = reader.Depth;
			//定义当前待解析子元素的当前配置项元素
			OptionConfigurationElement element = this;

			while(reader.Read() && reader.Depth > depth)
			{
				if(reader.NodeType != XmlNodeType.Element)
					continue;

				if(reader.Depth == depth + 1)
					element = this;

				//如果当前配置项元素是配置集合，则约定当前读取器所处的位置应当处于其下的集合元素的XML节点处
				if(typeof(OptionConfigurationElementCollection).IsAssignableFrom(element.GetType()))
				{
					//使用当前配置集合来解析当前读取器所处的XML节点内容
					element.DeserializeElement(reader.ReadSubtree());
					//忽略后续操作，直接进行后续XML内容处理
					continue;
				}

				//根据当前XML元素名获取对应的配置属性定义项，如果获取失败则获取当前配置项中的默认集合属性定义项
				if(!element.Properties.TryGetValue(reader.Name, out property))
					property = OptionConfigurationUtility.GetDefaultCollectionProperty(element.Properties);

				//如果对应的配置属性定义项均获取失败则抛出异常
				if(property == null)
					throw new OptionConfigurationException();

				//获取或创建当前配置属性定义项对应的目标元素对象
				element = element.EnsureElementPropertyValue(property);
				//判断获取的配置项元素是否为配置项集合
				var collection = element as OptionConfigurationElementCollection;

				if(collection != null)
				{
					//如果当前配置项集合不是默认集合(即集合有对应的XML节点)，则将当前读取器移动到其下的子元素的XML节点上
					if(!property.IsDefaultCollection)
						reader.ReadToDescendant(collection.ElementName);
				}

				//调用当前配置元素对象的反序列化方法
				element.DeserializeElement(reader.ReadSubtree());
			}
		}

		protected internal virtual void SerializeElement(XmlWriter writer)
		{
			var elementName = this.ElementProperty != null ? this.ElementProperty.Name : string.Empty;

			if(!string.IsNullOrEmpty(elementName))
				writer.WriteStartElement(elementName);

			object value;
			var elements = new List<OptionConfigurationElement>();

			foreach(var property in this.Properties)
			{
				if(typeof(OptionConfigurationElement).IsAssignableFrom(property.Type))
				{
					if(_values.TryGetValue(property, out value))
					{
						elements.Add((OptionConfigurationElement)value);
					}
				}
				else
				{
					if(_values.TryGetValue(property, out value))
					{
						writer.WriteStartAttribute(property.Name);
						writer.WriteString(OptionConfigurationUtility.GetValueString(value, property.Converter));
						writer.WriteEndAttribute();
					}
				}
			}

			if(elements.Count > 0)
			{
				foreach(var element in elements)
				{
					element.SerializeElement(writer);
				}
			}

			if(!string.IsNullOrEmpty(elementName))
				writer.WriteEndElement();
		}
		#endregion

		#region 激发事件
		protected void RaisePropertyChanged(string propertyName)
		{
			this.OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs(this.GetMappedPropertyName(propertyName)));
		}

		protected virtual string GetMappedPropertyName(string name)
		{
			return name;
		}

		protected virtual void OnPropertyChanged(System.ComponentModel.PropertyChangedEventArgs args)
		{
			if(this.PropertyChanged != null)
				this.PropertyChanged(this, args);
		}
		#endregion

		#region 私有方法
		private OptionConfigurationElement EnsureElementPropertyValue(OptionConfigurationProperty property)
		{
			if(property == null)
				throw new ArgumentNullException("property");

			object value = null;

			lock(_values)
			{
				_values.TryGetValue(property, out value);

				if(value == null)
				{
					//如果当前配置属性定义项的类型不是一个选项配置元素，则抛出异常。因为所有复合类型必须从选项配置元素继承
					if(!typeof(OptionConfigurationElement).IsAssignableFrom(property.Type))
						throw new OptionConfigurationException();

					//根据配置属性定义项的类型创建其对应的目标对象
					if(property.Type.IsGenericType && property.Type.GetGenericTypeDefinition() == typeof(OptionConfigurationElementCollection<>))
						value = Activator.CreateInstance(property.Type, new object[] { property.ElementName });
					else
						value = Activator.CreateInstance(property.Type);

					//设置创建的选项配置元素对象的表示其自身的属性定义项
					((OptionConfigurationElement)value)._elementProperty = property;

					//将创建的目标对象保存到对应的属性值中
					_values[property] = value;
				}
			}

			return (OptionConfigurationElement)value;
		}

		private void SetPropertyValue(OptionConfigurationProperty property, object value)
		{
			if(property == null)
				throw new ArgumentNullException("property");

			if(object.Equals(value, property.DefaultValue))
				_values.Remove(property);
			else
			{
				if(property.Type.IsAssignableFrom(value.GetType()))
					_values[property] = value;
				else
				{
					if(property.Converter == null)
						_values[property] = Zongsoft.Common.Convert.ConvertValue(value, property.Type, property.DefaultValue);
					else
						_values[property] = property.Converter.ConvertFrom(value);
				}
			}
		}
		#endregion

		#region 静态私有
		private static OptionConfigurationPropertyCollection GetOptionPropertiesFromType(Type type)
		{
			OptionConfigurationPropertyCollection properties;

			if(_propertyBags.TryGetValue(type, out properties))
				return properties;

			lock(_propertyBags)
			{
				if(_propertyBags.TryGetValue(type, out properties))
					return properties;

				properties = new OptionConfigurationPropertyCollection();

				foreach(var propertyInfo in type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
				{
					var property = CreateOptionPropertyFromAttribute(propertyInfo);

					if(property != null)
						properties.Add(property);
				}

				_propertyBags[type] = properties;
			}

			return properties;
		}

		private static OptionConfigurationProperty CreateOptionPropertyFromAttribute(PropertyInfo propertyInfo)
		{
			OptionConfigurationProperty result = null;

			var attribute = (OptionConfigurationPropertyAttribute)Attribute.GetCustomAttribute(propertyInfo, typeof(OptionConfigurationPropertyAttribute));

			if(attribute != null)
				result = new OptionConfigurationProperty(propertyInfo);

			return result;
		}
		#endregion
	}
}

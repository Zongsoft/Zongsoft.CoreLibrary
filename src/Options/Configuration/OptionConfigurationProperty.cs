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
using System.ComponentModel;

namespace Zongsoft.Options.Configuration
{
	public class OptionConfigurationProperty
	{
		#region 成员字段
		private string _name;
		private string _elementName;
		private Type _type;
		private object _defaultValue;
		private TypeConverter _converter;
		private OptionConfigurationPropertyBehavior _behavior;
		#endregion

		#region 构造函数
		public OptionConfigurationProperty(string name, Type type) : this(name, type, null, OptionConfigurationPropertyBehavior.None, null)
		{
		}

		public OptionConfigurationProperty(string name, string elementName, Type type) : this(name, type, null, OptionConfigurationPropertyBehavior.None, null)
		{
			if(string.IsNullOrWhiteSpace(elementName))
				throw new ArgumentNullException("elementName");

			_elementName = elementName.Trim();
		}

		public OptionConfigurationProperty(string name, Type type, object defaultValue) : this(name, type, defaultValue, OptionConfigurationPropertyBehavior.None, null)
		{
		}

		public OptionConfigurationProperty(string name, Type type, object defaultValue, OptionConfigurationPropertyBehavior behavior) : this(name, type, defaultValue, behavior, null)
		{
		}

		public OptionConfigurationProperty(string name, Type type, object defaultValue, OptionConfigurationPropertyBehavior behavior, TypeConverter converter)
		{
			if(type == null)
				throw new ArgumentNullException("type");

			_name = name == null ? string.Empty : name.Trim();
			_type = type;
			_behavior = behavior;
			_converter = converter;

			//注意：要最后设置默认属性的值
			this.DefaultValue = defaultValue;
		}

		internal OptionConfigurationProperty(PropertyInfo propertyInfo)
		{
			OptionConfigurationPropertyAttribute propertyAttribute = null;
			System.ComponentModel.TypeConverterAttribute converterAttribute = null;
			System.ComponentModel.DefaultValueAttribute defaultAttribute = null;

			foreach(var attribute in Attribute.GetCustomAttributes(propertyInfo))
			{
				if(attribute is OptionConfigurationPropertyAttribute)
					propertyAttribute = (OptionConfigurationPropertyAttribute)attribute;
				else if(attribute is System.ComponentModel.DefaultValueAttribute)
					defaultAttribute = (System.ComponentModel.DefaultValueAttribute)attribute;
				else if(attribute is System.ComponentModel.TypeConverterAttribute)
					converterAttribute = (System.ComponentModel.TypeConverterAttribute)attribute;
			}

			_name = propertyAttribute.Name;
			_elementName = propertyAttribute.ElementName;
			_type = propertyAttribute.Type ?? propertyInfo.PropertyType;
			_behavior = propertyAttribute.Behavior;

			if(propertyAttribute.Converter != null)
				_converter = propertyAttribute.Converter;
			else
			{
				if(converterAttribute != null && !string.IsNullOrEmpty(converterAttribute.ConverterTypeName))
				{
					Type type = Type.GetType(converterAttribute.ConverterTypeName, false);

					if(type != null)
						_converter = Activator.CreateInstance(type, true) as TypeConverter;
				}
			}

			//注意：要最后设置默认属性的值
			this.DefaultValue = defaultAttribute != null ? defaultAttribute.Value : propertyAttribute.DefaultValue;
		}
		#endregion

		#region 公共属性
		public string Name
		{
			get
			{
				return _name;
			}
		}

		public string ElementName
		{
			get
			{
				return _elementName;
			}
		}

		public Type Type
		{
			get
			{
				return _type;
			}
		}

		public object DefaultValue
		{
			get
			{
				return _defaultValue;
			}
			set
			{
				if(value == null)
				{
					if(_type.IsValueType)
						_defaultValue = Activator.CreateInstance(_type);
					else
						_defaultValue = null;
				}
				else
				{
					if(_converter != null && _converter.CanConvertFrom(value.GetType()))
						_defaultValue = _converter.ConvertFrom(value);
					else
						_defaultValue = Zongsoft.Common.Convert.ConvertValue(value, _type);
				}
			}
		}

		public TypeConverter Converter
		{
			get
			{
				return _converter;
			}
		}

		public OptionConfigurationPropertyBehavior Behavior
		{
			get
			{
				return _behavior;
			}
		}

		public bool IsKey
		{
			get
			{
				return (_behavior & OptionConfigurationPropertyBehavior.IsKey) == OptionConfigurationPropertyBehavior.IsKey;
			}
		}

		public bool IsRequired
		{
			get
			{
				return (_behavior & OptionConfigurationPropertyBehavior.IsRequired) == OptionConfigurationPropertyBehavior.IsRequired;
			}
		}

		public bool IsDefaultCollection
		{
			get
			{
				return string.IsNullOrEmpty(_name) && this.IsCollection;
			}
		}

		public bool IsCollection
		{
			get
			{
				return Common.TypeExtension.IsCollection(_type) || Common.TypeExtension.IsDictionary(_type);
			}
		}
		#endregion
	}
}

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
using System.Collections.Generic;

using Zongsoft.Options;
using Zongsoft.Options.Configuration;

namespace Zongsoft.Diagnostics.Configuration
{
	public class LoggerHandlerElement : OptionConfigurationElement
	{
		#region 常量定义
		private const string XML_NAME_ATTRIBUTE = "name";
		private const string XML_TYPE_ATTRIBUTE = "type";
		private const string XML_PREDICATION_ELEMENT = "predication";
		private const string XML_PROPERTIES_COLLECTION = "properties";
		#endregion

		#region 公共属性
		[OptionConfigurationProperty(XML_NAME_ATTRIBUTE, Behavior = OptionConfigurationPropertyBehavior.IsKey)]
		public string Name
		{
			get
			{
				return (string)this[XML_NAME_ATTRIBUTE];
			}
			set
			{
				if(string.IsNullOrWhiteSpace(value))
					throw new ArgumentNullException();

				this[XML_NAME_ATTRIBUTE] = value;
			}
		}

		[OptionConfigurationProperty(XML_TYPE_ATTRIBUTE, Behavior = OptionConfigurationPropertyBehavior.IsRequired)]
		public string Type
		{
			get
			{
				return (string)this[XML_TYPE_ATTRIBUTE];
			}
			set
			{
				if(string.IsNullOrWhiteSpace(value))
					throw new ArgumentNullException();

				this[XML_TYPE_ATTRIBUTE] = value;
			}
		}

		[OptionConfigurationProperty(XML_PREDICATION_ELEMENT)]
		public LoggerHandlerPredicationElement Predication
		{
			get
			{
				return (LoggerHandlerPredicationElement)this[XML_PREDICATION_ELEMENT];
			}
		}

		[OptionConfigurationProperty(XML_PROPERTIES_COLLECTION, ElementName = "property")]
		public new SettingElementCollection Properties
		{
			get
			{
				return (SettingElementCollection)this[XML_PROPERTIES_COLLECTION];
			}
		}
		#endregion

		#region 嵌套子类
		public class LoggerHandlerPredicationElement : OptionConfigurationElement
		{
			#region 常量定义
			private const string XML_SOURCE_ATTRIBUTE = "source";
			private const string XML_EXCEPTIONTYPE_ATTRIBUTE = "exceptionType";
			private const string XML_MINLEVEL_ATTRIBUTE = "minLevel";
			private const string XML_MAXLEVEL_ATTRIBUTE = "maxLevel";
			#endregion

			#region 公共属性
			[OptionConfigurationProperty(XML_SOURCE_ATTRIBUTE)]
			public string Source
			{
				get
				{
					return (string)this[XML_SOURCE_ATTRIBUTE];
				}
				set
				{
					this[XML_SOURCE_ATTRIBUTE] = value;
				}
			}

			[OptionConfigurationProperty(XML_EXCEPTIONTYPE_ATTRIBUTE)]
			public Type ExceptionType
			{
				get
				{
					return (Type)this[XML_EXCEPTIONTYPE_ATTRIBUTE];
				}
				set
				{
					this[XML_EXCEPTIONTYPE_ATTRIBUTE] = value;
				}
			}

			[OptionConfigurationProperty(XML_MINLEVEL_ATTRIBUTE)]
			public LogLevel? MinLevel
			{
				get
				{
					return (LogLevel?)this[XML_MINLEVEL_ATTRIBUTE];
				}
				set
				{
					this[XML_MINLEVEL_ATTRIBUTE] = value;
				}
			}

			[OptionConfigurationProperty(XML_MAXLEVEL_ATTRIBUTE)]
			public LogLevel? MaxLevel
			{
				get
				{
					return (LogLevel?)this[XML_MAXLEVEL_ATTRIBUTE];
				}
				set
				{
					this[XML_MAXLEVEL_ATTRIBUTE] = value;
				}
			}
			#endregion
		}
		#endregion
	}
}

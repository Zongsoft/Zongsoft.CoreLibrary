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
	public class SettingElement : OptionConfigurationElement
	{
		#region 常量定义
		private const string XML_NAME_ATTRIBUTE = "name";
		private const string XML_VALUE_ATTRIBUTE = "value";
		#endregion

		#region 构造函数
		internal SettingElement()
		{
		}

		internal SettingElement(string name, string value)
		{
			if(string.IsNullOrWhiteSpace(name))
				throw new ArgumentNullException("name");

			this.Name = name;
			this.Value = value;
		}
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

				this[XML_NAME_ATTRIBUTE] = value.Trim();
			}
		}

		[OptionConfigurationProperty(XML_VALUE_ATTRIBUTE)]
		public string Value
		{
			get
			{
				return (string)this[XML_VALUE_ATTRIBUTE];
			}
			set
			{
				this[XML_VALUE_ATTRIBUTE] = value;
			}
		}

		public bool HasExtendedProperties
		{
			get
			{
				return this.HasUnrecognizedProperties;
			}
		}

		public IDictionary<string, string> ExtendedProperties
		{
			get
			{
				return this.UnrecognizedProperties;
			}
		}
		#endregion

		#region 重写方法
		protected override bool OnDeserializeUnrecognizedAttribute(string name, string value)
		{
			this.UnrecognizedProperties.Add(name, value);
			return true;
		}
		#endregion
	}
}

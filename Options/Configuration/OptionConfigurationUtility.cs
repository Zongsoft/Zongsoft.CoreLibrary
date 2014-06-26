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
using System.Reflection;
using System.Linq;
using System.Xml;

namespace Zongsoft.Options.Configuration
{
	internal static class OptionConfigurationUtility
	{
		public static OptionConfigurationElement GetGlobalElement(string elementName)
		{
			if(!OptionConfiguration.Declarations.Contains(elementName))
				return null;

			var declaration = OptionConfiguration.Declarations[elementName];
			return Activator.CreateInstance(declaration.Type) as OptionConfigurationElement;
		}

		public static OptionConfigurationProperty GetKeyProperty(OptionConfigurationElement element)
		{
			if(element == null)
				return null;

			return element.Properties.FirstOrDefault(property => property.IsKey);
		}

		public static OptionConfigurationProperty GetDefaultCollectionProperty(OptionConfigurationPropertyCollection properties)
		{
			if(properties == null || properties.Count < 1)
				return null;

			return properties.FirstOrDefault(property => property.IsDefaultCollection);
		}

		public static string GetValueString(object value, System.ComponentModel.TypeConverter converter)
		{
			if(value == null)
				return string.Empty;

			if(value is string)
				return (string)value;

			if(converter != null)
				return converter.ConvertToString(value);
			else
				return Zongsoft.Common.Convert.ConvertValue<string>(value);
		}
	}
}

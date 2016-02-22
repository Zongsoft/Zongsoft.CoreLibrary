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

namespace Zongsoft.Options.Configuration
{
	public class ModuleElement : OptionConfigurationElement
	{
		#region 常量定义
		private const string XML_NAME_ATTRIBUTE = "name";
		private const string XML_TYPE_ATTRIBUTE = "type";
		#endregion

		#region 构造函数
		internal ModuleElement()
		{
		}

		internal ModuleElement(string name, string type)
		{
			if(string.IsNullOrWhiteSpace(name))
				throw new ArgumentNullException("name");

			if(string.IsNullOrWhiteSpace(type))
				throw new ArgumentNullException("type");

			this.Name = name;
			this.Type = type;
		}
		#endregion

		#region 公共属性
		[OptionConfigurationProperty(XML_NAME_ATTRIBUTE)]
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

		[OptionConfigurationProperty(XML_TYPE_ATTRIBUTE)]
		public string Type
		{
			get
			{
				return (string)this[XML_TYPE_ATTRIBUTE];
			}
			set
			{
				this[XML_TYPE_ATTRIBUTE] = value;
			}
		}
		#endregion

		#region 公共方法
		public Zongsoft.ComponentModel.IApplicationModule CreateModule()
		{
			var typeName = this.Type;

			if(string.IsNullOrWhiteSpace(typeName))
				throw new OptionConfigurationException("The module type is empty or unspecified.");

			var type = System.Type.GetType(typeName, false);

			if(type == null)
				throw new OptionConfigurationException(string.Format("Invalid '{0}' type of module, becase cann't load it.", typeName));

			if(!typeof(Zongsoft.ComponentModel.IApplicationModule).IsAssignableFrom(type))
				throw new OptionConfigurationException(string.Format("Invalid '{0}' type of module, it doesn't implemented IModule interface.", typeName));

			return Activator.CreateInstance(type) as Zongsoft.ComponentModel.IApplicationModule;
		}
		#endregion
	}
}

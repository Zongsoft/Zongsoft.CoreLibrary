/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2010-2013 Zongsoft Corporation <http://www.zongsoft.com>
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

using Zongsoft.Options;
using Zongsoft.Options.Configuration;

namespace Zongsoft.Diagnostics.Configuration
{
	public class TraceListenerElement : OptionConfigurationElement
	{
		#region 常量定义
		private const string XML_NAME_ATTRIBUTE		= "name";
		private const string XML_TYPE_ATTRIBUTE		= "type";
		#endregion

		#region 成员字段
		private Type _type;
		private ITraceListener _listener;
		#endregion

		#region 构造函数
		public TraceListenerElement()
		{
		}

		public TraceListenerElement(string name, string typeName)
		{
			this.Name = name;
			this.TypeName = typeName;
		}
		#endregion

		#region 公共属性
		[OptionConfigurationProperty(XML_NAME_ATTRIBUTE, OptionConfigurationPropertyBehavior.IsKey)]
		public string Name
		{
			get
			{
				return (string)this[XML_NAME_ATTRIBUTE];
			}
			set
			{
				this[XML_NAME_ATTRIBUTE] = value;
			}
		}

		public Type Type
		{
			get
			{
				if(_type == null)
					_type = Type.GetType(this.TypeName);

				return _type;
			}
			set
			{
				if(value == null)
					throw new ArgumentNullException();

				this.TypeName = value.AssemblyQualifiedName;
			}
		}

		[OptionConfigurationProperty(XML_TYPE_ATTRIBUTE, OptionConfigurationPropertyBehavior.IsRequired)]
		public string TypeName
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
				_type = null;
			}
		}

		public ITraceListener Listener
		{
			get
			{
				if(_listener == null)
				{
					if(this.Type == null)
						return null;

					if(this.Type.GetConstructors().Where(ctor => ctor.GetParameters().Length == 1 && ctor.GetParameters()[0].Name == "name" && ctor.GetParameters()[0].ParameterType == typeof(string)).Count() == 1)
						_listener = Activator.CreateInstance(this.Type, new object[] { this.Name }) as ITraceListener;
					else
						_listener = Activator.CreateInstance(this.Type) as ITraceListener;
				}

				return _listener;
			}
		}
		#endregion
	}
}

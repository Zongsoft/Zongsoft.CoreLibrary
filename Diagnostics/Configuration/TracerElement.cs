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
using System.Collections.Generic;
using System.Text;

using Zongsoft.Options;
using Zongsoft.Options.Configuration;

namespace Zongsoft.Diagnostics.Configuration
{
	public class TracerElement : OptionConfigurationElement
	{
		#region 静态成员变量
		private static readonly OptionConfigurationProperty _enabled;
		private static readonly OptionConfigurationProperty _listeners;
		private static OptionConfigurationPropertyCollection _properties;
		#endregion

		#region 类型构造函数
		static TracerElement()
		{
			_enabled = new OptionConfigurationProperty("enabled", typeof(bool), true);
			_listeners = new OptionConfigurationProperty("listeners", typeof(TraceListenerElementCollection), null);
			_properties = new OptionConfigurationPropertyCollection();
			_properties.Add(_enabled);
			_properties.Add(_listeners);
		}
		#endregion

		#region 重写成员
		protected internal override OptionConfigurationPropertyCollection Properties
		{
			get
			{
				return _properties;
			}
		}
		#endregion

		#region 公共属性
		public bool Enabled
		{
			get
			{
				return (bool)this[_enabled];
			}
			set
			{
				this[_enabled] = value;
			}
		}

		public TraceListenerElementCollection Listeners
		{
			get
			{
				return (TraceListenerElementCollection)base[_listeners];
			}
		}
		#endregion
	}
}

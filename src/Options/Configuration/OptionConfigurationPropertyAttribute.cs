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
using System.ComponentModel;

namespace Zongsoft.Options.Configuration
{
	/// <summary>
	/// 表示配置属性的标注类。
	/// </summary>
	/// <remarks>
	/// 如果需要定义类型转换器，请加注 <seealso cref="System.ComponentModel.TypeConverterAttribute"/> 标注标记。
	/// </remarks>
	[AttributeUsage(AttributeTargets.Property)]
	public class OptionConfigurationPropertyAttribute : Attribute
	{
		#region 成员变量
		private string _name;
		private string _elementName;
		private Type _type;
		private object _defaultValue;
		private OptionConfigurationPropertyBehavior _behavior;
		#endregion

		#region 构造函数
		public OptionConfigurationPropertyAttribute(string name) : this(name, null, null, OptionConfigurationPropertyBehavior.None)
		{
		}

		public OptionConfigurationPropertyAttribute(string name, OptionConfigurationPropertyBehavior behavior) : this(name, null, null, behavior)
		{
		}

		public OptionConfigurationPropertyAttribute(string name, object defaultValue, OptionConfigurationPropertyBehavior behavior = OptionConfigurationPropertyBehavior.None) : this(name, null, defaultValue, behavior)
		{
		}

		public OptionConfigurationPropertyAttribute(string name, Type type, object defaultValue = null, OptionConfigurationPropertyBehavior behavior = OptionConfigurationPropertyBehavior.None)
		{
			_name = name == null ? string.Empty : name.Trim();
			_type = type;
			_defaultValue = defaultValue;
			_behavior = behavior;
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
			set
			{
				if(string.IsNullOrWhiteSpace(value))
					throw new ArgumentNullException();

				_elementName = value.Trim();
			}
		}

		public Type Type
		{
			get
			{
				return _type;
			}
			set
			{
				_type = value;
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
				_defaultValue = value;
			}
		}

		public OptionConfigurationPropertyBehavior Behavior
		{
			get
			{
				return _behavior;
			}
			set
			{
				_behavior = value;
			}
		}
		#endregion
	}
}

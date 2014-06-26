/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2008-2012 Zongsoft Corporation <http://www.zongsoft.com>
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
using System.Collections.Generic;

namespace Zongsoft.Common
{
	/// <summary>
	/// 表示枚举项的描述。
	/// </summary>
	[Serializable]
	public struct EnumEntry
	{
		#region 成员变量
		private string _name;
		private object _value;
		private string _alias;
		private string _description;
		#endregion

		#region 构造函数
		public EnumEntry(string name, object value, string alias, string description)
		{
			_name = name;
			_value = value;
			_alias = alias ?? string.Empty;
			_description = description ?? string.Empty;
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取枚举项的名称。
		/// </summary>
		public string Name
		{
			get
			{
				return _name;
			}
		}

		/// <summary>
		/// 当前描述的枚举项值，该值有可能为枚举项的值也可能是对应的基元类型值。
		/// </summary>
		public object Value
		{
			get
			{
				return _value;
			}
		}

		/// <summary>
		/// 获取枚举项的别名，如果未定义建议创建者设置为枚举项的名称。
		/// </summary>
		/// <remarks>枚举项的别名由<seealso cref="Zongsoft.ComponentModel.AliasAttribute"/>自定义特性指定。</remarks>
		public string Alias
		{
			get
			{
				return _alias;
			}
		}

		/// <summary>
		/// 当前描述枚举项的描述文本，如果未定义建议创建者设置为枚举项的名称。
		/// </summary>
		/// <remarks>枚举项的描述由<seealso cref="System.ComponentModel.DescriptionAttribute"/>自定义特性指定。</remarks>
		public string Description
		{
			get
			{
				return _description;
			}
		}
		#endregion
	}
}
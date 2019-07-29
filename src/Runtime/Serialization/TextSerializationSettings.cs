/*
 *   _____                                ______
 *  /_   /  ____  ____  ____  _________  / __/ /_
 *    / /  / __ \/ __ \/ __ \/ ___/ __ \/ /_/ __/
 *   / /__/ /_/ / / / / /_/ /\_ \/ /_/ / __/ /_
 *  /____/\____/_/ /_/\__  /____/\____/_/  \__/
 *                   /____/
 *
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@qq.com>
 *
 * Copyright (C) 2015-2019 Zongsoft Corporation <http://www.zongsoft.com>
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

namespace Zongsoft.Runtime.Serialization
{
	public class TextSerializationSettings : SerializationSettings
	{
		#region 成员字段
		private bool _indented;
		private bool _typed;
		private string _dateTimeFormat;
		private SerializationNamingConvention _namingConvention;
		#endregion

		#region 构造函数
		public TextSerializationSettings()
		{
			_indented = false;
			_namingConvention = SerializationNamingConvention.None;
		}

		public TextSerializationSettings(SerializationNamingConvention naming, bool typed = false)
		{
			_typed = typed;
			_indented = false;
			_namingConvention = naming;
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取或设置一个值，指示序列化后的文本是否保持缩进风格。
		/// </summary>
		public bool Indented
		{
			get
			{
				return _indented;
			}
			set
			{
				_indented = value;
			}
		}

		/// <summary>
		/// 获取或设置一个值，指示序列化的文本是否保持强类型信息。
		/// </summary>
		public bool Typed
		{
			get
			{
				return _typed;
			}
			set
			{
				_typed = value;
			}
		}

		/// <summary>
		/// 获取或设置日期时间类型的格式字符串。
		/// </summary>
		public string DateTimeFormat
		{
			get
			{
				return _dateTimeFormat;
			}
			set
			{
				_dateTimeFormat = value;
			}
		}

		/// <summary>
		/// 获取或设置一个值，指示序列化成员的命名转换方式。
		/// </summary>
		public SerializationNamingConvention NamingConvention
		{
			get
			{
				return _namingConvention;
			}
			set
			{
				_namingConvention = value;
			}
		}
		#endregion

		#region 静态属性
		/// <summary>
		/// 获取一个以小驼峰命名转换的设置。
		/// </summary>
		public static TextSerializationSettings Camel
		{
			get => new TextSerializationSettings(SerializationNamingConvention.Camel);
		}

		/// <summary>
		/// 获取一个以大驼峰命名转换的设置。
		/// </summary>
		public static TextSerializationSettings Pascal
		{
			get => new TextSerializationSettings(SerializationNamingConvention.Pascal);
		}
		#endregion
	}
}

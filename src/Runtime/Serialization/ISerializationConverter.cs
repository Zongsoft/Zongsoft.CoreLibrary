/*
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

namespace Zongsoft.Runtime.Serialization
{
	/// <summary>
	/// 提供序列化类型转换功能的接口。
	/// </summary>
	/// <remarks>
	///		<para>该接口通常需要搭配<seealso cref="System.ComponentModel.TypeConverter"/>类型转换器使用，在序列化过程中指示某成员被序列化的新类型。</para>
	/// 	<para>譬如实体中有一个名为Tags的字符串数组类型的属性(字段)，需要序列化成以逗号分隔的字符串，则可通过类型转换器和该接口来实现这样的转换。</para>
	/// </remarks>
	public interface ISerializationConverter
	{
		/// <summary>
		/// 获取一个类型，表示序列化的类型。
		/// </summary>
		Type SerializationType
		{
			get;
		}
	}
}

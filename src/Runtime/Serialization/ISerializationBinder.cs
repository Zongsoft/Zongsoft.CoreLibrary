/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2015-2017 Zongsoft Corporation <http://www.zongsoft.com>
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
	/// 表示成员的反序列化绑定器。
	/// </summary>
	public interface ISerializationBinder
	{
		/// <summary>
		/// 获取一个值，指示是否使用默认的成员绑定。
		/// </summary>
		bool GetMemberValueSupported
		{
			get;
		}

		/// <summary>
		/// 获取反序列化的成员类型。
		/// </summary>
		/// <param name="name">反序列化的成员名称。</param>
		/// <param name="container">反序列的容器对象。</param>
		/// <returns>返回当前成员的真实类型。</returns>
		Type GetMemberType(string name, object container);

		/// <summary>
		/// 获取反序列化的成员值。
		/// </summary>
		/// <param name="name">反序列化的成员名称。</param>
		/// <param name="container">反序列的容器对象。</param>
		/// <param name="value">待转化的成员值。</param>
		/// <returns>返回当前成员的最终反序列化值。</returns>
		object GetMemberValue(string name, object container, object value);
	}
}

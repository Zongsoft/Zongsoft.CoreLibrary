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
using System.IO;

namespace Zongsoft.Runtime.Serialization
{
	/// <summary>
	/// 提供将对象序列化到流中和从流中反序列化对象的功能。
	/// </summary>
	public interface ISerializer
	{
		/// <summary>
		/// 反序列化指定<seealso cref="System.IO.Stream"/>包含的对象。
		/// </summary>
		/// <param name="serializationStream">待反序列化的流。</param>
		/// <returns>反序列化的结果。</returns>
		object Deserialize(Stream serializationStream);

		/// <summary>
		/// 将指定的对象序列化到指定的<seealso cref="System.IO.Stream"/>流中。
		/// </summary>
		/// <param name="serializationStream">要将对象序列化到的流。</param>
		/// <param name="graph">待序列化的目标对象。</param>
		void Serialize(Stream serializationStream, object graph);
	}
}

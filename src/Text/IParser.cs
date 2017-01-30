/*
 * Authors:
 *   钟峰(Popeye Zhong) <9555843@qq.com>
 *
 * Copyright (C) 2010-2017 Zongsoft Corporation <http://www.zongsoft.com>
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

namespace Zongsoft.Text
{
	/// <summary>
	/// 表示解析器的接口。
	/// </summary>
	public interface IParser
	{
		/// <summary>
		/// 获取此解析器的方案名。
		/// </summary>
		string Scheme
		{
			get;
		}

		/// <summary>
		/// 获取解析器返回的结果类型。
		/// </summary>
		/// <param name="context">解析器上下文对象。</param>
		/// <returns>返回的结果类型。</returns>
		/// <remarks>
		///		<para>在无法确认结果类型的情况下返回 <see cref="System.Object"/> 类型。</para>
		/// </remarks>
		Type GetResultType(ParserContextBase context);

		/// <summary>
		/// 执行解析操作并返回解析的结果。
		/// </summary>
		/// <param name="context">解析器上下文对象。</param>
		/// <returns>返回解析后的结果对象。</returns>
		object Parse(ParserContextBase context);
	}
}

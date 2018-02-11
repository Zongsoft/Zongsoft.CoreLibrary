/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2010-2018 Zongsoft Corporation <http://www.zongsoft.com>
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

namespace Zongsoft.Collections
{
	/// <summary>
	/// 表示提供查找功能的接口。
	/// </summary>
	/// <typeparam name="T">指示查找元素的泛型参数。</typeparam>
	public interface IFindable<T> : IFindable
	{
		/// <summary>
		/// 查找方法。
		/// </summary>
		/// <param name="parameter">指定的查找参数。</param>
		/// <param name="result">输出参数，表示查找成果的结果。</param>
		/// <returns>返回真(True)表示查找成果，否则表示查找失败。</returns>
		bool Find(object parameter, out T result);
	}
}

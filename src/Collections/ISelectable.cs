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
using System.Collections;
using System.Collections.Generic;

namespace Zongsoft.Collections
{
	/// <summary>
	/// 表示可匹配元素集的接口。
	/// </summary>
	public interface ISelectable
	{
		/// <summary>
		/// 选取匹配指定参数的元素。
		/// </summary>
		/// <typeparam name="T">指定要匹配的元素类型。</typeparam>
		/// <param name="parameter">指定要进行匹配的参数。</param>
		/// <returns>返回首个匹配成功的元素。</returns>
		T Select<T>(object parameter = null);

		/// <summary>
		/// 选取匹配指定参数的元素。
		/// </summary>
		/// <param name="type">指定要匹配的元素类型。</param>
		/// <param name="parameter">指定要进行匹配的参数。</param>
		/// <returns>返回首个匹配成功的元素。</returns>
		object Select(Type type, object parameter = null);

		/// <summary>
		/// 选取匹配指定参数的元素集。
		/// </summary>
		/// <typeparam name="T">指定要匹配的元素类型。</typeparam>
		/// <param name="parameter">指定要进行匹配的参数。</param>
		/// <returns>返回所有匹配成功的元素集。</returns>
		IEnumerable<T> SelectAll<T>(object parameter = null);

		/// <summary>
		/// 选取匹配指定参数的元素集。
		/// </summary>
		/// <param name="type">指定要匹配的元素类型。</param>
		/// <param name="parameter">指定要进行匹配的参数。</param>
		/// <returns>返回所有匹配成功的元素集。</returns>
		IEnumerable SelectAll(Type type, object parameter = null);
	}
}

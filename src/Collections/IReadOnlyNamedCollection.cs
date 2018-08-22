/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2013-2017 Zongsoft Corporation <http://www.zongsoft.com>
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

namespace Zongsoft.Collections
{
	/// <summary>
	/// 表示命名集合基类的接口。
	/// </summary>
	/// <typeparam name="T">集合成员的类型。</typeparam>
	public interface IReadOnlyNamedCollection<T> : IReadOnlyCollection<T>
	{
		/// <summary>
		/// 只读索引器，获取指定名称的元素。
		/// </summary>
		/// <param name="name">指定要获取的元素名。</param>
		/// <returns>返回指定名称的元素对象，如果没有找到则抛出<seealso cref="KeyNotFoundException"/>异常。</returns>
		/// <exception cref="KeyNotFoundException">当指定<paramref name="name"/>名称的元素不存在则激发该异常。</exception>
		T this[string name] { get; }

		/// <summary>
		/// 判断当前集合是否包含指定名称的元素。
		/// </summary>
		/// <param name="name">指定要判断的元素名。</param>
		/// <returns>如果指定名称的元素是存在的则返回真(True)，否则返回假(False)。</returns>
		bool Contains(string name);

		/// <summary>
		/// 获取指定名称的元素。
		/// </summary>
		/// <param name="name">指定要获取的元素名。</param>
		/// <returns>返回指定名称的元素对象，如果没有找到则抛出<seealso cref="KeyNotFoundException"/>异常。</returns>
		/// <exception cref="KeyNotFoundException">当指定<paramref name="name"/>名称的元素不存在则激发该异常。</exception>
		T Get(string name);

		/// <summary>
		/// 尝试获取指定名称的元素。
		/// </summary>
		/// <param name="name">指定要获取的元素名。</param>
		/// <param name="value">输出参数，包含指定名称的元素对象。</param>
		/// <returns>返回一个值，指示指定名称的元素是否获取成功。</returns>
		bool TryGet(string name, out T value);
	}
}

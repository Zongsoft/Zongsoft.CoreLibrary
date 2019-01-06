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
 * Copyright (C) 2016-2019 Zongsoft Corporation <http://www.zongsoft.com>
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

namespace Zongsoft.Data
{
	/// <summary>
	/// 表示数据模式的接口。
	/// </summary>
	public interface ISchema
	{
		/// <summary>
		/// 获取一个值，指示没有任何元素（即空模式）。
		/// </summary>
		bool IsEmpty
		{
			get;
		}

		/// <summary>
		/// 判断是否包含指定路径的元素。
		/// </summary>
		/// <param name="path">指定的元素路径。</param>
		/// <returns>如果包含指定路径的元素则返回真(True)，否则返回假(False)。</returns>
		bool Contains(string path);

		/// <summary>
		/// 移除模式的所有元素。
		/// </summary>
		/// <returns>返回被清空的当前模式。</returns>
		void Clear();

		/// <summary>
		/// 添加一个元素到位于指定路径处的元素集中。
		/// </summary>
		/// <param name="path">指定要添加的元素路径。</param>
		/// <returns>返回当前数据模式。</returns>
		ISchema Include(string path);

		/// <summary>
		/// 添加一个元素到位于指定路径处的元素集中。
		/// </summary>
		/// <param name="path">指定要添加的元素路径。</param>
		/// <param name="entry">输出参数，返回添加成功后的数据模式元素；如果添加失败则返回空(null)。</param>
		/// <returns>返回当前数据模式。</returns>
		ISchema Include(string path, out SchemaEntryBase entry);

		/// <summary>
		/// 从元素集中移除指定位置的元素。
		/// </summary>
		/// <param name="path">指定要移除的元素路径。</param>
		/// <returns>返回当前数据模式。</returns>
		ISchema Exclude(string path);

		/// <summary>
		/// 从元素集中移除指定位置的元素。
		/// </summary>
		/// <param name="path">指定要移除的元素路径。</param>
		/// <param name="entry">输出参数，返回移除成功后的数据模式元素；如果移除失败则返回空(null)。</param>
		/// <returns>返回当前数据模式。</returns>
		ISchema Exclude(string path, out SchemaEntryBase entry);
	}
}

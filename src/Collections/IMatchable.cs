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

namespace Zongsoft.Collections
{
	/// <summary>
	/// 提供一种特定于类型的通用匹配方法，某些同类型的类通过实现此接口对其进行更进一步的匹配。
	/// </summary>
	public interface IMatchable
	{
		/// <summary>
		/// 指示当前对象是否匹配指定参数的条件约束。
		/// </summary>
		/// <param name="parameter">指定是否匹配逻辑的参数。</param>
		/// <returns>如果当前对象符合<paramref name="parameter"/>参数的匹配规则，则为真(true)；否则为假(false)。</returns>
		bool IsMatch(object parameter);
	}
}

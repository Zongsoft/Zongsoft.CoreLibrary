/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2008-2013 Zongsoft Corporation <http://www.zongsoft.com>
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
	/// 条件接口。
	/// </summary>
	public interface ICondition
	{
		/// <summary>
		/// 判断当前条件语句中是否包含指定名称的条件项。
		/// </summary>
		/// <param name="name">指定的条件项名称。</param>
		/// <returns>如果存在则返回真(True)，否则返回假(False)。</returns>
		bool Contains(string name);

		/// <summary>
		/// 在条件语句中查找指定名称的条件项，如果匹配到则回调指定的匹配函数。
		/// </summary>
		/// <param name="name">指定要匹配的条件项名称。</param>
		/// <param name="matched">指定的匹配成功的回调函数。</param>
		/// <returns>如果匹配成功则返回真(True)，否则返回假(False)。</returns>
		bool Match(string name, Action<Condition> matched = null);

		/// <summary>
		/// 在条件语句中查找指定名称的所有条件项，如果匹配到则回调指定的匹配函数。
		/// </summary>
		/// <param name="name">指定要匹配的条件项名称。</param>
		/// <param name="matched">指定的匹配成功的回调函数。</param>
		/// <returns>返回匹配成功的条件项数量，如果为零则表示没有匹配到任何条件项。</returns>
		int Matches(string name, Action<Condition> matched = null);
	}
}

/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2016 Zongsoft Corporation <http://www.zongsoft.com>
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
	/// 表示条件设置实体的接口。
	/// </summary>
	public interface IConditional : ICondition, IEnumerable<ICondition>
	{
		/// <summary>
		/// 获取或设置条件的组合方式。
		/// </summary>
		ConditionCombination Combination
		{
			get;
			set;
		}

		/// <summary>
		/// 判断是否包含某个名称的成员。
		/// </summary>
		/// <param name="name">指定的成员名称。</param>
		/// <returns>如果包含指定名称的成员则返回真(True)，否则返回假(False)。</returns>
		bool Contains(string name);

		/// <summary>
		/// 查找指定名称的所有成员。
		/// </summary>
		/// <param name="name">指定的成员名称。</param>
		/// <returns>返回指定名称的成员数组。</returns>
		ICondition[] Find(string name);
	}
}

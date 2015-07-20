/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2015 Zongsoft Corporation <http://www.zongsoft.com>
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

namespace Zongsoft.Security
{
	/// <summary>
	/// 表示单词或词语的审查。
	/// </summary>
	public interface ICensorship
	{
		/// <summary>
		/// 获取词汇审查的名称或审查的类别。
		/// </summary>
		string Name
		{
			get;
		}

		/// <summary>
		/// 判断指定的<paramref name="word"/>词汇是否为非法的。
		/// </summary>
		/// <param name="word">指定要判断的单词或词语。</param>
		/// <returns>如果指定的单词或词语是非法的(敏感词)则返回真(True)，否则返回假(False)。</returns>
		bool IsBlocked(string word);
	}
}

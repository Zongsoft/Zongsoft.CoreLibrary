/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2015-2018 Zongsoft Corporation <http://www.zongsoft.com>
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

namespace Zongsoft.Common
{
	/// <summary>
	/// 表示提供有效性验证功能的接口。
	/// </summary>
	/// <typeparam name="T">指定</typeparam>
	public interface IValidator<in T>
	{
		/// <summary>
		/// 验证指定的数据是否有效。
		/// </summary>
		/// <param name="data">指定的待验证的数据。</param>
		/// <param name="failure">当内部验证失败的回调处理函数，传入的字符串参数表示验证失败的消息文本。</param>
		/// <returns>如果验证通过则返回真(True)，否则返回假(False)。</returns>
		bool Validate(T data, Action<string> failure = null);
	}
}

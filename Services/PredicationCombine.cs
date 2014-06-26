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

namespace Zongsoft.Services
{
	/// <summary>
	/// 表示断言的逻辑组合方式。
	/// </summary>
	public enum PredicationCombine
	{
		/// <summary>表示如果某个断言返回成功，则不再执行后续断言测试而直接返回成功；如果返回失败，则进行后续断言测试。即整个断言链中所有断言测试均失败则断言链返回失败。</summary>
		Or,
		/// <summary>表示如果某个断言返回成功，则进行下一个断言测试，如果返回失败，则不再执行后续断言测试而直接返回失败。即整个断言链中所有断言测试均成功则断言链返回成功。</summary>
		And,
	}
}

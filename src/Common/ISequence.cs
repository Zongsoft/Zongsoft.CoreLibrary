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

namespace Zongsoft.Common
{
	/// <summary>
	/// 提供序号递增(减)功能的接口。
	/// </summary>
	public interface ISequence
	{
		/// <summary>
		/// 递增指定序列号的数值，默认递增步长为1。
		/// </summary>
		/// <param name="key">指定递增的序列号键。</param>
		/// <param name="interval">指定的序列号递增的步长数，如果为零则表示获取当前值，而不会引发递增。</param>
		/// <param name="seed">初始化的种子数，如果指定名称的序列号不存在，则创建指定名称的序列号，并使用该种子数作为该序列号的初始值。</param>
		/// <returns>返回递增后的序列号数值。</returns>
		long Increment(string key, int interval = 1, int seed = 0);

		/// <summary>
		/// 递减指定序列号的数值，默认递减步长为1。
		/// </summary>
		/// <param name="key">指定递减的序列号键。</param>
		/// <param name="interval">指定的序列号递减的步长数，如果为零则表示获取当前值，而不会引发递减。</param>
		/// <param name="seed">初始化的种子数，如果指定名称的序列号不存在，则创建指定名称的序列号，并使用该种子数作为该序列号的初始值。</param>
		/// <returns>返回递减后的序列号数值。</returns>
		long Decrement(string key, int interval = 1, int seed = 0);

		/// <summary>
		/// 重置指定的序列号数值，如果指定键的序列号不存在则创建它。
		/// </summary>
		/// <param name="key">指定要重置的序列号键。</param>
		/// <param name="value">指定要重置的序列号当前值，默认为零。</param>
		void Reset(string key, int value = 0);
	}
}

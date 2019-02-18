/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2010-2018 Zongsoft Corporation <http://www.zongsoft.com>
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

namespace Zongsoft.Data
{
	/// <summary>
	/// 表示写入操作上下文的基础接口。
	/// </summary>
	public interface IDataMutateContextBase : IDataAccessContextBase
	{
		/// <summary>
		/// 获取或设置写入操作的受影响记录数。
		/// </summary>
		int Count
		{
			get; set;
		}

		/// <summary>
		/// 获取或设置写入操作的数据。
		/// </summary>
		object Data
		{
			get; set;
		}

		/// <summary>
		/// 获取一个值，指示是否为批量写入操作。
		/// </summary>
		bool IsMultiple
		{
			get;
		}
	}
}

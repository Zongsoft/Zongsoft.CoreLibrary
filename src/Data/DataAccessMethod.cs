/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2010-2017 Zongsoft Corporation <http://www.zongsoft.com>
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
using System.ComponentModel;

namespace Zongsoft.Data
{
	/// <summary>
	/// 表示数据访问的方法名的枚举。
	/// </summary>
	public enum DataAccessMethod
	{
		/// <summary>计数方法</summary>
		Count = 1,

		/// <summary>是否存在</summary>
		Exists,

		/// <summary>执行方法</summary>
		Execute,

		/// <summary>递增递减</summary>
		Increment,

		/// <summary>查询方法</summary>
		Select,

		/// <summary>删除方法</summary>
		Delete,

		/// <summary>新增方法</summary>
		Insert,

		/// <summary>更新方法</summary>
		Update,

		/// <summary>新增或更新</summary>
		Upsert,
	}
}

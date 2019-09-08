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
 * Copyright (C) 2015-2019 Zongsoft Corporation <http://www.zongsoft.com>
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

namespace Zongsoft.Data.Metadata
{
	/// <summary>
	/// 表示序号器的元数据接口。
	/// </summary>
	public interface IDataSequence
	{
		/// <summary>
		/// 获取序号所属的数据属性元素。
		/// </summary>
		IDataEntitySimplexProperty Property
		{
			get;
		}

		/// <summary>
		/// 获取序号器的名称。
		/// </summary>
		string Name
		{
			get;
		}

		/// <summary>
		/// 获取或设置序号器的种子数。
		/// </summary>
		int Seed
		{
			get; set;
		}

		/// <summary>
		/// 获取或设置序号器的递增量，默认为1。
		/// </summary>
		int Interval
		{
			get; set;
		}

		/// <summary>
		/// 获取一个值，指示是否采用数据库内置序号方案。
		/// </summary>
		bool IsBuiltin
		{
			get;
		}

		/// <summary>
		/// 获取一个值，指示是否采用外置序号器方案。
		/// </summary>
		bool IsExternal
		{
			get;
		}

		/// <summary>
		/// 获取序号的引用的属性数组。
		/// </summary>
		IDataEntitySimplexProperty[] References
		{
			get;
		}
	}
}

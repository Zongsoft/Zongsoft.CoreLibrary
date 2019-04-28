/*
 *   _____                                ______
 *  /_   /  ____  ____  ____  _________  / __/ /_
 *    / /  / __ \/ __ \/ __ \/ ___/ __ \/ /_/ __/
 *   / /__/ /_/ / / / / /_/ /\_ \/ /_/ / __/ /_
 *  /____/\____/_/ /_/\__  /____/\____/_/  \__/
 *                   /____/
 *
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2010-2019 Zongsoft Corporation <http://www.zongsoft.com>
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
	/// 表示数据访问的上下文的基本接口。
	/// </summary>
	public interface IDataAccessContextBase
	{
		/// <summary>
		/// 获取数据访问的名称。
		/// </summary>
		string Name
		{
			get;
		}

		/// <summary>
		/// 获取数据访问的方法。
		/// </summary>
		DataAccessMethod Method
		{
			get;
		}

		/// <summary>
		/// 获取当前上下文关联的数据访问器。
		/// </summary>
		IDataAccess DataAccess
		{
			get;
		}

		/// <summary>
		/// 获取一个值，指示当前上下文是否含有附加的状态数据。
		/// </summary>
		bool HasStates
		{
			get;
		}

		/// <summary>
		/// 获取当前上下文的附加状态数据集。
		/// </summary>
		IDictionary<string, object> States
		{
			get;
		}
	}
}

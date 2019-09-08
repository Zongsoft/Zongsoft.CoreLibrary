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
using System.Collections.Generic;

namespace Zongsoft.Data.Metadata
{
	/// <summary>
	/// 表示元数据的提供程序接口。
	/// </summary>
	public interface IDataMetadataProvider
	{
		#region 属性定义
		/// <summary>
		/// 获取元数据提供程序所属的应用名。
		/// </summary>
		string Name
		{
			get;
		}

		/// <summary>
		/// 获取元数据提供程序所属的管理器。
		/// </summary>
		IDataMetadataManager Manager
		{
			get;
			set;
		}

		/// <summary>
		/// 获取元数据提供程序中的数据实体定义集。
		/// </summary>
		Collections.INamedCollection<IDataEntity> Entities
		{
			get;
		}

		/// <summary>
		/// 获取元数据提供程序中的数据命令定义集。
		/// </summary>
		Collections.INamedCollection<IDataCommand> Commands
		{
			get;
		}
		#endregion
	}
}

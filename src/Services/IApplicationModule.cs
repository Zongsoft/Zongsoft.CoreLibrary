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
 * Copyright (C) 2016-2018 Zongsoft Corporation <http://www.zongsoft.com>
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

using Zongsoft.Options;

namespace Zongsoft.Services
{
	/// <summary>
	/// 表示应用模块（应用子系统）的接口。
	/// </summary>
	public interface IApplicationModule
	{
		/// <summary>
		/// 获取应用模块名称。
		/// </summary>
		string Name
		{
			get;
		}

		/// <summary>
		/// 获取或设置应用模块的标题。
		/// </summary>
		string Title
		{
			get;
			set;
		}

		/// <summary>
		/// 获取或设置应用模块的描述文本。
		/// </summary>
		string Description
		{
			get;
			set;
		}

		/// <summary>
		/// 获取应用模块的自定义设置提供程序。
		/// </summary>
		ISettingsProvider Settings
		{
			get;
		}

		/// <summary>
		/// 获取应用模块的服务容器。
		/// </summary>
		IServiceProvider Services
		{
			get;
		}
	}
}

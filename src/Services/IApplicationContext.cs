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
using Zongsoft.Options.Configuration;

namespace Zongsoft.Services
{
	/// <summary>
	/// 表示应用程序上下文的接口。
	/// </summary>
	public interface IApplicationContext : IApplicationModule
	{
		#region 事件定义
		event EventHandler Exiting;
		event EventHandler Starting;
		event EventHandler Started;
		#endregion

		#region 属性定义
		/// <summary>
		/// 获取当前应用程序的根目录。
		/// </summary>
		string ApplicationDirectory
		{
			get;
		}

		/// <summary>
		/// 获取当前应用程序的选项管理。
		/// </summary>
		IOptionProvider Options
		{
			get;
		}

		/// <summary>
		/// 获取当前应用程序的应用配置。
		/// </summary>
		OptionConfiguration Configuration
		{
			get;
		}

		/// <summary>
		/// 获取当前应用程序的安全主体。
		/// </summary>
		System.Security.Principal.IPrincipal Principal
		{
			get;
		}

		/// <summary>
		/// 获取当前应用程序的模块集。
		/// </summary>
		Collections.INamedCollection<IApplicationModule> Modules
		{
			get;
		}

		/// <summary>
		/// 获取当前应用程序的事件处理程序集。
		/// </summary>
		Collections.INamedCollection<IApplicationFilter> Filters
		{
			get;
		}

		/// <summary>
		/// 获取当前应用的状态字典。
		/// </summary>
		IDictionary<string, object> States
		{
			get;
		}
		#endregion

		#region 方法定义
		/// <summary>
		/// 确认指定的当前应用程序的相对目录是否存在，如果不存在则依次创建它们，并返回其对应的完整路径。
		/// </summary>
		/// <param name="relativePath">相对于应用程序根目录的相对路径，可使用'/'或'\'字符作为相对路径的分隔符。</param>
		/// <returns>如果<paramref name="relativePath"/>参数为空或者全空白字符则返回应用程序根目录(即<see cref="ApplicationDirectory"/>属性值。)，否则返回其相对路径的完整路径。</returns>
		string EnsureDirectory(string relativePath);
		#endregion
	}
}

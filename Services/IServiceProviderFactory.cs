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
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Zongsoft.Services
{
	/// <summary>
	/// 提供关于服务供应程序容器的功能。
	/// </summary>
	public interface IServiceProviderFactory
	{
		/// <summary>
		/// 获取或设置默认的服务供应程序。
		/// </summary>
		IServiceProvider Default
		{
			get;
		}

		/// <summary>
		/// 获取指定名称的服务供应程序。
		/// </summary>
		/// <param name="name">指定的服务供应程序名称。</param>
		/// <returns>返回指定名称的服务供应程序。</returns>
		IServiceProvider GetProvider(string name);
	}
}

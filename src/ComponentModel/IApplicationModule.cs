/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2016 Zongsoft Corporation <http://www.zongsoft.com>
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

namespace Zongsoft.ComponentModel
{
	/// <summary>
	/// 向实现类提供应用扩展模块初始化和处置事件。
	/// </summary>
	public interface IApplicationModule : IDisposable
	{
		/// <summary>
		/// 获取应用扩展模块名称。
		/// </summary>
		string Name
		{
			get;
		}

		/// <summary>
		/// 初始化应用扩展模块，并使其为处理请求做好准备。
		/// </summary>
		/// <param name="context">一个上下文对象，它提供对模块处理应用程序内所有应用程序对象的公用的方法、属性和事件的访问。</param>
		/// <remarks>使用 <c>Initialize</c> 将事件处理方法向具体事件进行注册等初始化操作。</remarks>
		void Initialize(ApplicationContextBase context);
	}
}

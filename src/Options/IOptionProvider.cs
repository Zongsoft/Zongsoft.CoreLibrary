/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2005-2008 Zongsoft Corporation <http://www.zongsoft.com>
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
using System.Collections.Generic;

namespace Zongsoft.Options
{
	/// <summary>
	/// 提供选项数据的获取与保存。
	/// </summary>
	public interface IOptionProvider
	{
		/// <summary>
		/// 根据指定的选项路径获取对应的选项数据。
		/// </summary>
		/// <param name="path">要获取的选项路径。</param>
		/// <returns>获取到的选项数据对象。</returns>
		object GetOptionObject(string path);

		/// <summary>
		/// 将指定的选项数据保存到指定路径的存储容器中。
		/// </summary>
		/// <param name="path">待保存的选项路径。</param>
		/// <param name="optionObject">待保存的选项对象。</param>
		void SetOptionObject(string path, object optionObject);
	}
}

/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2003-2014 Zongsoft Corporation <http://www.zongsoft.com>
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

namespace Zongsoft.Runtime.Caching
{
	/// <summary>
	/// 表示缓存字典的容器的接口。
	/// </summary>
	public interface IDictionaryCacheProvider
	{
		/// <summary>
		/// 获取指定名称的缓存字典。
		/// </summary>
		/// <param name="name">指定要获取的缓存字典的名称。</param>
		/// <returns>返回指定名称的缓存字典名称，如果指定名称的缓存字典不存在则返回由<see cref="IDictionaryCacheProvider"/>实现者的<see cref="OnCreate"/>方法返回的缓存字典。</returns>
		IDictionaryCache GetDictionaryCache(string name);

		/// <summary>
		/// 创建一个特定类型的缓存字典。
		/// </summary>
		/// <param name="name">创建的缓存字典的名称。</param>
		/// <returns>返回创建成功的缓存字典。</returns>
		IDictionaryCache OnCreate(string name);
	}
}

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
	/// 提供缓存字典的接口。
	/// </summary>
	public interface IDictionaryCache
	{
		/// <summary>
		/// 获取一个值，表示缓存字典内的记录总数。
		/// </summary>
		long Count
		{
			get;
		}

		/// <summary>
		/// 从缓存字典中获取指定键的缓存值。
		/// </summary>
		/// <param name="key">指定要获取的键。</param>
		/// <returns>如果指定的键存在则返回对应的值，如果不存在则返回空(null)。</returns>
		object GetValue(string key);

		/// <summary>
		/// 设置指定的值保存到缓存字典中。
		/// </summary>
		/// <param name="key">指定要保存的键。</param>
		/// <param name="value">指定要保存的值。</param>
		/// <remarks>
		///		<para>如果指定的<paramref name="key"/>键不存在则新增，否则覆盖现有值。</para>
		/// </remarks>
		void SetValue(string key, object value);

		/// <summary>
		/// 从缓存字典中删除指定键的缓存项。
		/// </summary>
		/// <param name="key">指定要删除的键。</param>
		void Remove(string key);

		/// <summary>
		/// 清空缓存字典中的所有数据。
		/// </summary>
		void Clear();
	}
}

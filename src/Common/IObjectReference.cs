/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2015 Zongsoft Corporation <http://www.zongsoft.com>
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

namespace Zongsoft.Common
{
	public interface IObjectReference<T> where T : class
	{
		/// <summary>
		/// 获取一个指示<seealso cref="Target"/>属性对应的目标对象是否可用的值。
		/// </summary>
		/// <remarks>
		///		<para>获取该属性始终不会抛出异常，如果目标对象已经是 Disposed 的则返回假(false)。</para>
		/// </remarks>
		bool HasTarget
		{
			get;
		}

		/// <summary>
		/// 获取引用的目标对象。
		/// </summary>
		T Target
		{
			get;
		}
	}
}

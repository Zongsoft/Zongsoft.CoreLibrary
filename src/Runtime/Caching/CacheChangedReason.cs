/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2013-2015 Zongsoft Corporation <http://www.zongsoft.com>
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

namespace Zongsoft.Runtime.Caching
{
	/// <summary>
	/// 表示缓存项发生改变的原因。
	/// </summary>
	public enum CacheChangedReason
	{
		/// <summary>系统内部原因。</summary>
		None,
		/// <summary>被手动删除的。</summary>
		Removed,
		/// <summary>被手动更新的。</summary>
		Updated,
		/// <summary>因为过期被删除。</summary>
		Expired,
		/// <summary>因为依赖被删除。</summary>
		Dependented,
	}
}

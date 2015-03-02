/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2014 Zongsoft Corporation <http://www.zongsoft.com>
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

namespace Zongsoft.Collections
{
	/// <summary>
	/// 表示集合元素被删除的原因。
	/// </summary>
	public enum CollectionRemovedReason
	{
		/// <summary>通过删除方法。</summary>
		[Description("${Text.CollectionRemovedReason.Remove}")]
		Remove,

		/// <summary>因为集合溢出而激发的自动删除。</summary>
		[Description("${Text.CollectionRemovedReason.Overflow}")]
		Overflow,

		/// <summary>因为缓存项过期而被删除。</summary>
		[Description("${Text.CollectionRemovedReason.Expired}")]
		Expired,

		/// <summary>其他原因。</summary>
		[Description("${Text.CollectionRemovedReason.Other}")]
		Other,
	}
}

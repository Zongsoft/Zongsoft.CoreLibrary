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
using System.Collections.Generic;

namespace Zongsoft.Collections
{
	public class CollectionRemovedEventArgs : EventArgs
	{
		#region 成员字段
		private CollectionRemovedReason _reason;
		private object _item;
		#endregion

		#region 构造函数
		public CollectionRemovedEventArgs(CollectionRemovedReason reason, object item)
		{
			_reason = reason;
			_item = item;
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取被删除的原因。
		/// </summary>
		public CollectionRemovedReason Reason
		{
			get
			{
				return _reason;
			}
		}

		/// <summary>
		/// 获取被删除的集合元素。
		/// </summary>
		public object Item
		{
			get
			{
				return _item;
			}
		}
		#endregion
	}
}

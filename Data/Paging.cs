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
using System.Collections.Generic;

namespace Zongsoft.Data
{
	public class Paging
	{
		#region 成员字段
		private int _pageIndex;
		private int _pageSize;
		private long _totalCount;
		#endregion

		#region 构造函数
		public Paging() : this(1, 20)
		{
		}

		public Paging(int pageIndex) : this(pageIndex, 20)
		{
		}

		public Paging(int pageIndex, int pageSize)
		{
			this.PageIndex = pageIndex;
			this.PageSize = pageSize;
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取或设置页大小，如果该属性值为零则表示不分页。
		/// </summary>
		public int PageSize
		{
			get
			{
				return _pageSize;
			}
			set
			{
				if(value < 0)
					throw new ArgumentOutOfRangeException();

				_pageSize = value;
			}
		}

		/// <summary>
		/// 获取或设置当前查询的页号，页号从1开始。
		/// </summary>
		public int PageIndex
		{
			get
			{
				return _pageIndex;
			}
			set
			{
				_pageIndex = Math.Max(value, 1);
			}
		}

		/// <summary>
		/// 获取查询结果的总页数。
		/// </summary>
		public int PageCount
		{
			get
			{
				if(_totalCount < 1)
					return 0;

				return (int)Math.Ceiling((double)_totalCount / _pageSize);
			}
		}

		/// <summary>
		/// 获取或设置查询结果的总记录数。
		/// </summary>
		public long TotalCount
		{
			get
			{
				return _totalCount;
			}
			set
			{
				_totalCount = Math.Max(value, -1);
			}
		}
		#endregion
	}
}

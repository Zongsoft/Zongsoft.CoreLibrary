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
using System.Collections.Generic;

namespace Zongsoft.Security.Membership
{
	[Serializable]
	public class PermissionFilter : Permission
	{
		#region 成员变量
		private string _filter;
		#endregion

		#region 构造函数
		public PermissionFilter()
		{
		}

		public PermissionFilter(uint memberId, MemberType memberType, string schemaId, string actionId, string filter) : base(memberId, memberType, schemaId, actionId, false)
		{
			if(string.IsNullOrWhiteSpace(filter))
				throw new ArgumentNullException("filter");

			_filter = filter.Trim();
		}
		#endregion

		#region 公共属性
		public string Filter
		{
			get
			{
				return _filter;
			}
			set
			{
				_filter = value;
			}
		}
		#endregion

		#region 重写方法
		public override string ToString()
		{
			if(string.IsNullOrEmpty(_filter))
				return base.ToString();

			return base.ToString() + Environment.NewLine + _filter;
		}
		#endregion
	}
}

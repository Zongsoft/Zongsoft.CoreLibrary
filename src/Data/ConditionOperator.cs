/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2008-2013 Zongsoft Corporation <http://www.zongsoft.com>
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

namespace Zongsoft.Data
{
	public enum ConditionOperator
	{
		/// <summary>等于</summary>
		[Zongsoft.ComponentModel.Alias("==")]
		Equal,

		/// <summary>不等于</summary>
		[Zongsoft.ComponentModel.Alias("!=")]
		NotEqual,

		/// <summary>大于</summary>
		[Zongsoft.ComponentModel.Alias(">")]
		GreaterThan,

		/// <summary>大于或等于</summary>
		[Zongsoft.ComponentModel.Alias(">=")]
		GreaterThanEqual,

		/// <summary>小于</summary>
		[Zongsoft.ComponentModel.Alias("<")]
		LessThan,

		/// <summary>小于或等于</summary>
		[Zongsoft.ComponentModel.Alias("<=")]
		LessThanEqual,

		/// <summary>模糊匹配</summary>
		[Zongsoft.ComponentModel.Alias("*=")]
		Like,

		/// <summary>介于</summary>
		[Zongsoft.ComponentModel.Alias("~")]
		Between,

		/// <summary>范围</summary>
		[Zongsoft.ComponentModel.Alias("^")]
		In,

		/// <summary>排除范围</summary>
		[Zongsoft.ComponentModel.Alias("!^")]
		NotIn,
	}
}

/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2008-2012 Zongsoft Corporation <http://www.zongsoft.com>
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

namespace Zongsoft.ComponentModel
{
	/// <summary>
	/// 表示查询条件的接口。
	/// </summary>
	[Obsolete]
	public interface ICondition : INotifyPropertyChanged
	{
		/// <summary>
		/// 获取当前查询的目标名称。
		/// </summary>
		string Name
		{
			get;
		}

		/// <summary>
		/// 获取或设置查询条件的组合方式。
		/// </summary>
		ConditionCombine ConditionCombine
		{
			get;
			set;
		}

		/// <summary>
		/// 获取或设置是否启用模糊查询。
		/// </summary>
		bool FuzzyInquiryEnabled
		{
			get;
			set;
		}

		/// <summary>
		/// 获取或设置页大小。
		/// </summary>
		int PageSize
		{
			get;
			set;
		}

		/// <summary>
		/// 获取或设置当前查询的页号。
		/// </summary>
		int PageIndex
		{
			get;
			set;
		}

		/// <summary>
		/// 获取或设置查询结果的总页数。
		/// </summary>
		int PageCount
		{
			get;
			set;
		}

		/// <summary>
		/// 获取或设置查询结果的总记录数。
		/// </summary>
		int TotalCount
		{
			get;
			set;
		}

		/// <summary>
		/// 获取或设置当前查询内部的表达式。
		/// </summary>
		object Expression
		{
			get;
			set;
		}

		/// <summary>
		/// 获取当前查询的输出参数集。
		/// </summary>
		IDictionary<string, object> Output
		{
			get;
		}

		/// <summary>
		/// 获取当前查询的子句集。
		/// </summary>
		ConditionClauseCollection Clauses
		{
			get;
		}
	}
}

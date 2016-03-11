/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2016 Zongsoft Corporation <http://www.zongsoft.com>
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
using System.Collections;
using System.Collections.Generic;

namespace Zongsoft.Data
{
	/// <summary>
	/// 为数据服务的获取事件提供数据。
	/// </summary>
	public class DataGettedEventArgs : EventArgs
	{
		#region 成员字段
		private string _name;
		private object _result;
		private ICondition _condition;
		private string _scope;
		private Paging _paging;
		private Sorting[] _sortings;
		#endregion

		#region 构造函数
		public DataGettedEventArgs(string name, ICondition condition, string scope, Paging paging, Sorting[] sortings, object result)
		{
			if(string.IsNullOrWhiteSpace(name))
				throw new ArgumentNullException("name");

			_name = name.Trim();
			_condition = condition;
			_scope = scope;
			_paging = paging;
			_sortings = sortings;
			_result = result;
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取数据访问的名称。
		/// </summary>
		public string Name
		{
			get
			{
				return _name;
			}
		}

		/// <summary>
		/// 获取或设置查询操作的结果。
		/// </summary>
		public object Result
		{
			get
			{
				return _result;
			}
			set
			{
				_result = value;
			}
		}

		/// <summary>
		/// 获取或设置查询操作的条件。
		/// </summary>
		public ICondition Condition
		{
			get
			{
				return _condition;
			}
			set
			{
				_condition = value;
			}
		}

		/// <summary>
		/// 获取或设置查询操作的分页设置。
		/// </summary>
		public Paging Paging
		{
			get
			{
				return _paging;
			}
			set
			{
				_paging = value;
			}
		}

		/// <summary>
		/// 获取或设置查询操作的包含成员。
		/// </summary>
		public string Scope
		{
			get
			{
				return _scope;
			}
			set
			{
				_scope = value;
			}
		}

		/// <summary>
		/// 获取或设置查询操作的排序设置。
		/// </summary>
		public Sorting[] Sortings
		{
			get
			{
				return _sortings;
			}
			set
			{
				_sortings = value;
			}
		}
		#endregion
	}
}

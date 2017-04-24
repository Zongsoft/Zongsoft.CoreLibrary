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
using System.Collections.Generic;

namespace Zongsoft.Data
{
	/// <summary>
	/// 为数据访问的删除事件提供数据。
	/// </summary>
	public class DataDeletedEventArgs : DataAccessEventArgs
	{
		#region 成员字段
		private int _result;
		private ICondition _condition;
		private string[] _cascades;
		#endregion

		#region 构造函数
		public DataDeletedEventArgs(string name, ICondition condition, string[] cascades, int result) : base(name)
		{
			_condition = condition;
			_cascades = cascades;
			_result = result;
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取或设置删除操作的结果。
		/// </summary>
		public int Result
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
		/// 获取或设置删除操作的条件。
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
		/// 获取或设置删除操作的关联成员。
		/// </summary>
		public string[] Cascades
		{
			get
			{
				return _cascades;
			}
			set
			{
				_cascades = value;
			}
		}
		#endregion
	}
}

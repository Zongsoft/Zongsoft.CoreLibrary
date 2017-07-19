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

namespace Zongsoft.Data
{
	/// <summary>
	/// 为数据访问的更新事件提供数据。
	/// </summary>
	public class DataUpdatedEventArgs : DataAccessEventArgs
	{
		#region 成员字段
		private int _count;
		private DataDictionary _data;
		private ICondition _condition;
		private string _scope;
		#endregion

		#region 构造函数
		public DataUpdatedEventArgs(string name, DataDictionary data, ICondition condition, string scope, int count) : base(name)
		{
			_data = data;
			_condition = condition;
			_scope = scope;
			_count = count;
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取或设置更新操作的受影响记录数。
		/// </summary>
		public int Count
		{
			get
			{
				return _count;
			}
			set
			{
				_count = value;
			}
		}

		/// <summary>
		/// 获取或设置更新操作的数据。
		/// </summary>
		public DataDictionary Data
		{
			get
			{
				return _data;
			}
			set
			{
				_data = value;
			}
		}

		/// <summary>
		/// 获取或设置更新操作的条件。
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
		/// 获取或设置更新操作的包含成员。
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
		#endregion
	}
}

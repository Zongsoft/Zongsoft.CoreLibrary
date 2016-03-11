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
	public class DataUpdatedEventArgs : EventArgs
	{
		#region 成员字段
		private string _name;
		private int _result;
		private object _data;
		private ICondition _condition;
		private string _scope;
		#endregion

		#region 构造函数
		public DataUpdatedEventArgs(string name, object data, ICondition condition, string scope, int result)
		{
			if(string.IsNullOrWhiteSpace(name))
				throw new ArgumentNullException("name");

			_name = name.Trim();
			_data = data;
			_condition = condition;
			_scope = scope;
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
		/// 获取或设置更新操作的结果。
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
		/// 获取或设置更新操作的数据。
		/// </summary>
		public object Data
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

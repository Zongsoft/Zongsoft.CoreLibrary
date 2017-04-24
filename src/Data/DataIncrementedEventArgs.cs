/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2010-2017 Zongsoft Corporation <http://www.zongsoft.com>
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

namespace Zongsoft.Data
{
	public class DataIncrementedEventArgs : DataAccessEventArgs
	{
		#region 成员字段
		private string _member;
		private ICondition _condition;
		private int _interval;
		private long _result;
		#endregion

		#region 构造函数
		public DataIncrementedEventArgs(string name, string member, ICondition condition, int interval = 1) : base(name)
		{
			if(string.IsNullOrWhiteSpace(member))
				throw new ArgumentNullException(nameof(member));

			if(condition == null)
				throw new ArgumentNullException(nameof(condition));

			_member = member;
			_condition = condition;
			_interval = interval;
		}

		public DataIncrementedEventArgs(string name, string member, ICondition condition, int interval, long result) : base(name)
		{
			if(string.IsNullOrWhiteSpace(member))
				throw new ArgumentNullException(nameof(member));

			if(condition == null)
				throw new ArgumentNullException(nameof(condition));

			_member = member;
			_condition = condition;
			_interval = interval;
			_result = result;
		}
		#endregion

		#region 公共属性
		public string Member
		{
			get
			{
				return _member;
			}
			set
			{
				if(string.IsNullOrWhiteSpace(value))
					throw new ArgumentNullException();

				_member = value;
			}
		}

		public ICondition Condition
		{
			get
			{
				return _condition;
			}
			set
			{
				if(value == null)
					throw new ArgumentNullException();

				_condition = value;
			}
		}

		public int Interval
		{
			get
			{
				return _interval;
			}
			set
			{
				_interval = value;
			}
		}

		public long Result
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
		#endregion
	}
}

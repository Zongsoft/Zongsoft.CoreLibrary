/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2010-2015 Zongsoft Corporation <http://www.zongsoft.com>
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

namespace Zongsoft.Data
{
	public class Grouping
	{
		#region 成员字段
		private string[] _members;
		private ICondition _condition;
		#endregion

		#region 构造函数
		public Grouping()
		{
		}

		public Grouping(params string[] members) : this(null, members)
		{
		}

		public Grouping(ICondition condition, params string[] members)
		{
			if(members == null || members.Length == 0)
				throw new ArgumentNullException("members");

			this.Condition = condition;
			this.Members = members;
		}
		#endregion

		#region 公共属性
		public string MembersText
		{
			get
			{
				if(_members == null || _members.Length < 1)
					return string.Empty;

				return string.Join(", ", _members);
			}
			set
			{
				if(string.IsNullOrWhiteSpace(value))
					throw new ArgumentNullException();

				this.Members = value.Split(',');
			}
		}

		public string[] Members
		{
			get
			{
				return _members;
			}
			set
			{
				if(value == null || value.Length == 0)
					throw new ArgumentNullException();

				var hashset = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

				foreach(var member in value)
				{
					if(!string.IsNullOrWhiteSpace(member))
						hashset.Add(member.Trim());
				}

				if(hashset == null)
					throw new ArgumentException();

				string[] members = new string[hashset.Count];
				hashset.CopyTo(members);

				_members = members;
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
				_condition = value;
			}
		}
		#endregion

		#region 重写方法
		public override string ToString()
		{
			return string.Format("{0} ({1})", this.MembersText, this.Condition);
		}
		#endregion
	}
}

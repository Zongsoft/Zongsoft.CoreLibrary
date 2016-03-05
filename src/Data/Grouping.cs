/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2010-2016 Zongsoft Corporation <http://www.zongsoft.com>
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
	/// <summary>
	/// 表示数据分组的设置项。
	/// </summary>
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
		/// <summary>
		/// 获取或设置分组成员的文本，各分组成员以逗号“,”分隔。
		/// </summary>
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

		/// <summary>
		/// 获取或设置分组的成员数组。
		/// </summary>
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

		/// <summary>
		/// 获取或设置分组的过滤条件，默认为空。
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
		#endregion

		#region 静态方法
		/// <summary>
		/// 创建一个分组。
		/// </summary>
		/// <param name="members">分组的成员集。</param>
		/// <returns>返回创建的分组设置。</returns>
		public static Grouping Group(params string[] members)
		{
			if(members == null || members.Length < 1)
				throw new ArgumentNullException("members");

			return new Grouping(members);
		}
		#endregion

		#region 公共方法
		/// <summary>
		/// 设置分组的过滤条件。
		/// </summary>
		/// <param name="condition">指定的过滤条件。</param>
		/// <returns>返回带过滤条件的分组设置。</returns>
		public Grouping Filter(ICondition condition)
		{
			_condition = condition;
			return this;
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

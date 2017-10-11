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
using System.Collections;
using System.Collections.Generic;

namespace Zongsoft.Data
{
	/// <summary>
	/// 表示数据分组的设置项。
	/// </summary>
	public class Grouping
	{
		#region 成员字段
		private GroupMember[] _keys;
		private ICondition _condition;
		private GroupAggregationCollection _aggregations;
		#endregion

		#region 构造函数
		private Grouping(ICondition condition, params GroupMember[] keys)
		{
			if(keys == null || keys.Length == 0)
				throw new ArgumentNullException(nameof(keys));

			_keys = keys;
			_condition = condition;
			_aggregations = new GroupAggregationCollection(this);
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取分组键的成员数组。
		/// </summary>
		public GroupMember[] Keys
		{
			get
			{
				return _keys;
			}
		}

		/// <summary>
		/// 获取分组的聚合成员集合。
		/// </summary>
		public GroupAggregationCollection Aggregations
		{
			get
			{
				return _aggregations;
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
		/// 创建一个分组设置。
		/// </summary>
		/// <param name="keys">分组键成员数组。</param>
		/// <returns>返回创建的分组设置。</returns>
		public static Grouping Group(params string[] keys)
		{
			return Group(null, keys);
		}

		/// <summary>
		/// 创建一个分组设置。
		/// </summary>
		/// <param name="filter">分组的过滤条件。</param>
		/// <param name="keys">分组键成员数组。</param>
		/// <returns>返回创建的分组设置。</returns>
		public static Grouping Group(ICondition filter, params string[] keys)
		{
			if(keys == null || keys.Length < 1)
				throw new ArgumentNullException(nameof(keys));

			var members = new List<GroupMember>(keys.Length);

			foreach(var key in keys)
			{
				if(string.IsNullOrEmpty(key))
					continue;

				var index = key.IndexOf(':');

				if(index > 0)
					members.Add(new GroupMember(key.Substring(0, index), key.Substring(index)));
				else
					members.Add(new GroupMember(key, null));
			}

			return new Grouping(filter, members.ToArray());
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
			var text = new System.Text.StringBuilder();

			if(_keys != null && _keys.Length > 0)
			{
				text.Append("Keys: ");

				foreach(var key in _keys)
				{
					text.Append(key.Name);

					if(key.Alias != null && key.Alias.Length > 0)
						text.Append(" '" + key.Alias + "'");
				}

				text.AppendLine();
			}

			if(_aggregations != null)
			{
				foreach(var aggregation in _aggregations)
				{
					text.Append(aggregation.Method.ToString() + ": " + aggregation.Name);

					if(aggregation.Alias != null && aggregation.Alias.Length > 0)
						text.Append(" '" + aggregation.Alias + "'");

					text.AppendLine();
				}
			}

			if(_condition != null)
			{
				text.AppendLine("Filter: " + _condition.ToString());
			}

			if(text == null)
				return string.Empty;
			else
				return text.ToString();
		}
		#endregion

		#region 嵌套结构
		public struct GroupMember
		{
			public string Name;
			public string Alias;

			public GroupMember(string name, string alias)
			{
				this.Name = name;
				this.Alias = alias;
			}
		}

		public struct GroupAggregation
		{
			public string Name;
			public string Alias;
			public GroupAggregationMethod Method;

			public GroupAggregation(GroupAggregationMethod method, string name, string alias)
			{
				this.Method = method;
				this.Name = name;
				this.Alias = alias;
			}
		}

		public class GroupAggregationCollection : IEnumerable<GroupAggregation>
		{
			#region 私有变量
			private Grouping _grouping;
			private ICollection<GroupAggregation> _members;
			#endregion

			#region 私有构造
			internal GroupAggregationCollection(Grouping grouping)
			{
				_grouping = grouping;
				_members = new List<GroupAggregation>();
			}
			#endregion

			#region 公共方法
			public GroupAggregationCollection Count(string name, string alias = null)
			{
				return this.Aggregate(GroupAggregationMethod.Count, name, alias);
			}

			public GroupAggregationCollection Sum(string name, string alias = null)
			{
				return this.Aggregate(GroupAggregationMethod.Sum, name, alias);
			}

			public GroupAggregationCollection Average(string name, string alias = null)
			{
				return this.Aggregate(GroupAggregationMethod.Average, name, alias);
			}

			public GroupAggregationCollection Median(string name, string alias = null)
			{
				return this.Aggregate(GroupAggregationMethod.Median, name, alias);
			}

			public GroupAggregationCollection Maximum(string name, string alias = null)
			{
				return this.Aggregate(GroupAggregationMethod.Maximum, name, alias);
			}

			public GroupAggregationCollection Minimum(string name, string alias = null)
			{
				return this.Aggregate(GroupAggregationMethod.Minimum, name, alias);
			}

			public GroupAggregationCollection Deviation(string name, string alias = null)
			{
				return this.Aggregate(GroupAggregationMethod.Deviation, name, alias);
			}

			public GroupAggregationCollection DeviationPopulation(string name, string alias = null)
			{
				return this.Aggregate(GroupAggregationMethod.DeviationPopulation, name, alias);
			}

			public GroupAggregationCollection Variance(string name, string alias = null)
			{
				return this.Aggregate(GroupAggregationMethod.Variance, name, alias);
			}

			public GroupAggregationCollection VariancePopulation(string name, string alias = null)
			{
				return this.Aggregate(GroupAggregationMethod.VariancePopulation, name, alias);
			}
			#endregion

			#region 私有方法
			private GroupAggregationCollection Aggregate(GroupAggregationMethod method, string name, string alias = null)
			{
				if(string.IsNullOrEmpty(name))
					throw new ArgumentNullException(nameof(name));

				_members.Add(new GroupAggregation(GroupAggregationMethod.Count, name, alias));

				return this;
			}
			#endregion

			#region 遍历实现
			public IEnumerator<GroupAggregation> GetEnumerator()
			{
				foreach(var member in _members)
					yield return member;
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return this.GetEnumerator();
			}
			#endregion
		}

		public enum GroupAggregationMethod
		{
			/// <summary>数量</summary>
			Count,

			/// <summary>总和</summary>
			Sum,

			/// <summary>平均值</summary>
			Average,

			/// <summary>中间值</summary>
			Median,

			/// <summary>最大值</summary>
			Maximum,

			/// <summary>最小值</summary>
			Minimum,

			/// <summary>标准偏差</summary>
			Deviation,

			/// <summary>总体标准偏差</summary>
			DeviationPopulation,

			/// <summary>方差</summary>
			Variance,

			/// <summary>总体方差</summary>
			VariancePopulation,
		}
		#endregion
	}
}

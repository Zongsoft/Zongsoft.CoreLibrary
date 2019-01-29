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
		private GroupKey[] _keys;
		private ICondition _filter;
		private AggregateCollection _aggregates;
		#endregion

		#region 构造函数
		private Grouping(ICondition filter, params GroupKey[] keys)
		{
			if(keys == null || keys.Length == 0)
				throw new ArgumentNullException(nameof(keys));

			_keys = keys;
			_filter = filter;
			_aggregates = new AggregateCollection(this);
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取分组键的成员数组。
		/// </summary>
		public GroupKey[] Keys
		{
			get
			{
				return _keys;
			}
		}

		/// <summary>
		/// 获取分组的聚合成员集合。
		/// </summary>
		public AggregateCollection Aggregates
		{
			get
			{
				return _aggregates;
			}
		}

		/// <summary>
		/// 获取或设置分组的过滤条件，默认为空。
		/// </summary>
		public ICondition Filter
		{
			get
			{
				return _filter;
			}
			set
			{
				_filter = value;
			}
		}
		#endregion

		#region 公共方法
		public Grouping Count(string name, string alias = null)
		{
			_aggregates.Count(name, alias);
			return this;
		}

		public Grouping Sum(string name, string alias = null)
		{
			_aggregates.Sum(name, alias);
			return this;
		}

		public Grouping Average(string name, string alias = null)
		{
			_aggregates.Average(name, alias);
			return this;
		}

		public Grouping Median(string name, string alias = null)
		{
			_aggregates.Median(name, alias);
			return this;
		}

		public Grouping Maximum(string name, string alias = null)
		{
			_aggregates.Maximum(name, alias);
			return this;
		}

		public Grouping Minimum(string name, string alias = null)
		{
			_aggregates.Minimum(name, alias);
			return this;
		}

		public Grouping Deviation(string name, string alias = null)
		{
			_aggregates.Deviation(name, alias);
			return this;
		}

		public Grouping DeviationPopulation(string name, string alias = null)
		{
			_aggregates.DeviationPopulation(name, alias);
			return this;
		}

		public Grouping Variance(string name, string alias = null)
		{
			_aggregates.Variance(name, alias);
			return this;
		}

		public Grouping VariancePopulation(string name, string alias = null)
		{
			_aggregates.VariancePopulation(name, alias);
			return this;
		}
		#endregion

		#region 静态方法
		/// <summary>
		/// 创建一个分组设置。
		/// </summary>
		/// <param name="keys">分组键成员数组，分组键元素使用冒号分隔成员的名称和别名。</param>
		/// <returns>返回创建的分组设置。</returns>
		public static Grouping Group(params string[] keys)
		{
			return Group(null, keys);
		}

		/// <summary>
		/// 创建一个分组设置。
		/// </summary>
		/// <param name="filter">分组的过滤条件。</param>
		/// <param name="keys">分组键成员数组，分组键元素使用冒号分隔成员的名称和别名。</param>
		/// <returns>返回创建的分组设置。</returns>
		public static Grouping Group(ICondition filter, params string[] keys)
		{
			if(keys == null || keys.Length < 1)
				throw new ArgumentNullException(nameof(keys));

			var list = new List<GroupKey>(keys.Length);

			foreach(var key in keys)
			{
				if(string.IsNullOrEmpty(key))
					continue;

				var index = key.IndexOf(':');

				if(index > 0)
					list.Add(new GroupKey(key.Substring(0, index), key.Substring(index + 1)));
				else
					list.Add(new GroupKey(key, null));
			}

			return new Grouping(filter, list.ToArray());
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

			if(_aggregates != null)
			{
				foreach(var aggregate in _aggregates)
				{
					text.Append(aggregate.Method.ToString() + ": " + aggregate.Name);

					if(aggregate.Alias != null && aggregate.Alias.Length > 0)
						text.Append(" '" + aggregate.Alias + "'");

					text.AppendLine();
				}
			}

			if(_filter != null)
			{
				text.AppendLine("Filter: " + _filter.ToString());
			}

			if(text == null)
				return string.Empty;
			else
				return text.ToString();
		}
		#endregion

		#region 嵌套结构
		public struct GroupKey
		{
			public string Name;
			public string Alias;

			public GroupKey(string name, string alias)
			{
				this.Name = name;
				this.Alias = alias;
			}
		}

		public struct Aggregate
		{
			public string Name;
			public string Alias;
			public AggregateMethod Method;

			public Aggregate(AggregateMethod method, string name, string alias)
			{
				this.Method = method;
				this.Name = name;
				this.Alias = alias;
			}
		}

		public class AggregateCollection : IEnumerable<Aggregate>
		{
			#region 私有变量
			private Grouping _grouping;
			private ICollection<Aggregate> _members;
			#endregion

			#region 私有构造
			internal AggregateCollection(Grouping grouping)
			{
				_grouping = grouping;
				_members = new List<Aggregate>();
			}
			#endregion

			#region 公共方法
			public AggregateCollection Count(string name, string alias = null)
			{
				return this.Aggregate(AggregateMethod.Count, name, alias);
			}

			public AggregateCollection Sum(string name, string alias = null)
			{
				return this.Aggregate(AggregateMethod.Sum, name, alias);
			}

			public AggregateCollection Average(string name, string alias = null)
			{
				return this.Aggregate(AggregateMethod.Average, name, alias);
			}

			public AggregateCollection Median(string name, string alias = null)
			{
				return this.Aggregate(AggregateMethod.Median, name, alias);
			}

			public AggregateCollection Maximum(string name, string alias = null)
			{
				return this.Aggregate(AggregateMethod.Maximum, name, alias);
			}

			public AggregateCollection Minimum(string name, string alias = null)
			{
				return this.Aggregate(AggregateMethod.Minimum, name, alias);
			}

			public AggregateCollection Deviation(string name, string alias = null)
			{
				return this.Aggregate(AggregateMethod.Deviation, name, alias);
			}

			public AggregateCollection DeviationPopulation(string name, string alias = null)
			{
				return this.Aggregate(AggregateMethod.DeviationPopulation, name, alias);
			}

			public AggregateCollection Variance(string name, string alias = null)
			{
				return this.Aggregate(AggregateMethod.Variance, name, alias);
			}

			public AggregateCollection VariancePopulation(string name, string alias = null)
			{
				return this.Aggregate(AggregateMethod.VariancePopulation, name, alias);
			}
			#endregion

			#region 私有方法
			private AggregateCollection Aggregate(AggregateMethod method, string name, string alias = null)
			{
				if(string.IsNullOrEmpty(name) && method != AggregateMethod.Count)
					throw new ArgumentNullException(nameof(name));

				_members.Add(new Aggregate(method, name, alias));

				return this;
			}
			#endregion

			#region 遍历实现
			public IEnumerator<Aggregate> GetEnumerator()
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

		/// <summary>
		/// 表示聚合方法的枚举。
		/// </summary>
		public enum AggregateMethod
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

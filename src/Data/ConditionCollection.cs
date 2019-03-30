/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2008-2016 Zongsoft Corporation <http://www.zongsoft.com>
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
using System.Linq;
using System.Collections.Generic;

namespace Zongsoft.Data
{
	public class ConditionCollection : System.Collections.ObjectModel.Collection<ICondition>, ICondition
	{
		#region 成员字段
		private ConditionCombination _conditionCombination;
		#endregion

		#region 构造函数
		public ConditionCollection(ConditionCombination conditionCombination)
		{
			_conditionCombination = conditionCombination;
		}

		public ConditionCollection(ConditionCombination conditionCombination, IEnumerable<ICondition> items)
		{
			_conditionCombination = conditionCombination;

			if(items != null)
			{
				foreach(var item in items)
				{
					if(item != null)
						this.Add(item);
				}
			}
		}

		public ConditionCollection(ConditionCombination conditionCombination, params ICondition[] items)
		{
			_conditionCombination = conditionCombination;

			if(items != null && items.Length > 0)
			{
				foreach(var item in items)
				{
					if(item != null)
						this.Add(item);
				}
			}
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取或设置查询条件的组合方式。
		/// </summary>
		public ConditionCombination Combination
		{
			get
			{
				return _conditionCombination;
			}
			set
			{
				_conditionCombination = value;
			}
		}
		#endregion

		#region 符号重写
		public static ConditionCollection operator +(Condition condition, ConditionCollection conditions)
		{
			if(conditions == null)
				throw new ArgumentNullException(nameof(conditions));

			if(condition == null)
				return conditions;

			return new ConditionCollection(conditions.Combination, Combine(condition, conditions));
		}

		public static ConditionCollection operator +(ConditionCollection conditions, Condition condition)
		{
			if(conditions == null)
				throw new ArgumentNullException(nameof(conditions));

			if(condition == null)
				return conditions;

			return new ConditionCollection(conditions.Combination, Combine(conditions, condition));
		}

		public static ConditionCollection operator &(Condition condition, ConditionCollection conditions)
		{
			if(condition == null)
				return conditions;

			if(conditions == null)
				return new ConditionCollection(ConditionCombination.And, condition);

			if(conditions.Combination == ConditionCombination.And)
				return new ConditionCollection(ConditionCombination.And, Combine(condition, conditions));
			else
				return new ConditionCollection(ConditionCombination.And, condition, conditions);
		}

		public static ConditionCollection operator &(ConditionCollection conditions, Condition condition)
		{
			if(condition == null)
				return conditions;

			if(conditions == null)
				return new ConditionCollection(ConditionCombination.And, condition);

			if(conditions.Combination == ConditionCombination.And)
				return new ConditionCollection(ConditionCombination.And, Combine(conditions, condition));
			else
				return new ConditionCollection(ConditionCombination.And, conditions, condition);
		}

		public static ConditionCollection operator &(ConditionCollection left, ConditionCollection right)
		{
			if(left == null)
				return right;

			if(right == null)
				return left;

			if(left.Combination == ConditionCombination.And)
			{
				if(right.Combination == ConditionCombination.And)
					return new ConditionCollection(ConditionCombination.And, Combine(left, right));
				else
					return left.Append(right);
			}
			else
			{
				if(right.Combination == ConditionCombination.And)
					return right.Prepend(left);
				else
					return new ConditionCollection(ConditionCombination.And, left, right);
			}
		}

		public static ConditionCollection operator |(Condition condition, ConditionCollection conditions)
		{
			if(condition == null)
				return conditions;

			if(conditions == null)
				return new ConditionCollection(ConditionCombination.Or, condition);

			if(conditions.Combination == ConditionCombination.Or)
				return new ConditionCollection(ConditionCombination.Or, Combine(condition, conditions));
			else
				return new ConditionCollection(ConditionCombination.Or, condition, conditions);
		}

		public static ConditionCollection operator |(ConditionCollection conditions, Condition condition)
		{
			if(condition == null)
				return conditions;

			if(conditions == null)
				return new ConditionCollection(ConditionCombination.Or, condition);

			if(conditions.Combination == ConditionCombination.Or)
				return new ConditionCollection(ConditionCombination.Or, Combine(conditions, condition));
			else
				return new ConditionCollection(ConditionCombination.Or, conditions, condition);
		}

		public static ConditionCollection operator |(ConditionCollection left, ConditionCollection right)
		{
			if(left == null)
				return right;

			if(right == null)
				return left;

			if(left.Combination == ConditionCombination.Or)
			{
				if(right.Combination == ConditionCombination.Or)
					return new ConditionCollection(ConditionCombination.Or, Combine(left, right));
				else
					return left.Append(right);
			}
			else
			{
				if(right.Combination == ConditionCombination.Or)
					return right.Prepend(left);
				else
					return new ConditionCollection(ConditionCombination.Or, left, right);
			}
		}
		#endregion

		#region 公共静态
		public static ConditionCollection Or(IEnumerable<ICondition> items)
		{
			return new ConditionCollection(ConditionCombination.Or, items);
		}

		public static ConditionCollection Or(params ICondition[] items)
		{
			return new ConditionCollection(ConditionCombination.Or, items);
		}

		public static ConditionCollection And(IEnumerable<ICondition> items)
		{
			return new ConditionCollection(ConditionCombination.And, items);
		}

		public static ConditionCollection And(params ICondition[] items)
		{
			return new ConditionCollection(ConditionCombination.And, items);
		}
		#endregion

		#region 公共方法
		public bool Contains(string name)
		{
			if(string.IsNullOrEmpty(name))
				return false;

			return this.Matches(this, name, condition => true) > 0;
		}

		public int Remove(params string[] names)
		{
			if(names == null || names.Length == 0)
				return 0;

			var count = 0;

			foreach(var name in names)
			{
				if(!string.IsNullOrEmpty(name))
					count += this.Remove(this, name);
			}

			return count;
		}

		public bool Match(string name, Action<Condition> matched = null)
		{
			if(string.IsNullOrEmpty(name))
				return false;

			return this.Matches(this, name, condition => { matched?.Invoke(condition); return true; }) > 0;
		}

		public int Matches(string name, Action<Condition> matched = null)
		{
			if(string.IsNullOrEmpty(name))
				return 0;

			return this.Matches(this, name, condition => { matched?.Invoke(condition); return false; });
		}

		/// <summary>
		/// 创建一个与当前集合内容相同的新条件集，并将指定的条件项追加到新集中。
		/// </summary>
		/// <param name="items">要追加的条件项数组。</param>
		/// <returns>返回新建的条件集合。</returns>
		public ConditionCollection Append(params ICondition[] items)
		{
			if(items == null || items.Length < 1)
				return this;

			return new ConditionCollection(this.Combination, this.Items.Concat(items).Where(item => item != null));
		}

		/// <summary>
		/// 创建一个与当前集合内容相同的新条件集，并将指定的条件项置顶到新集中。
		/// </summary>
		/// <param name="items">要置顶的条件项数组。</param>
		/// <returns>返回新建的条件集合。</returns>
		public ConditionCollection Prepend(params ICondition[] items)
		{
			if(items == null || items.Length < 1)
				return this;

			return new ConditionCollection(this.Combination, items.Concat(this.Items).Where(item => item != null));
		}
		#endregion

		#region 重写方法
		protected override void InsertItem(int index, ICondition item)
		{
			if(item != null)
				base.InsertItem(index, item);
		}

		protected override void SetItem(int index, ICondition item)
		{
			if(item != null)
				base.SetItem(index, item);
		}

		public override string ToString()
		{
			var combiner = _conditionCombination.ToString().ToUpperInvariant();

			if(this.Count < 1)
				return combiner;

			var text = new System.Text.StringBuilder();

			foreach(var item in this.Items)
			{
				if(text.Length > 0)
					text.Append(" " + combiner + " ");

				text.Append(item.ToString());
			}

			return "(" + text.ToString() + ")";
		}
		#endregion

		#region 私有静态
		private static IEnumerable<ICondition> Combine(params ICondition[] conditions)
		{
			if(conditions == null)
				yield break;

			for(int i = 0; i < conditions.Length; i++)
			{
				if(conditions[i] == null)
					continue;

				var items = conditions[i] as IEnumerable<ICondition>;

				if(items == null)
					yield return conditions[i];
				else
				{
					foreach(var item in items)
					{
						yield return item;
					}
				}
			}
		}
		#endregion

		#region 私有方法
		private int Remove(ICollection<ICondition> conditions, string name)
		{
			var count = 0;
			IList<Condition> matches = null;

			foreach(var condition in conditions)
			{
				if(condition is Condition c)
				{
					if(string.Equals(c.Name, name, StringComparison.OrdinalIgnoreCase))
					{
						count++;

						if(matches == null)
							matches = new List<Condition>();

						matches.Add(c);
					}
				}
				else if(condition is ConditionCollection cs)
				{
					count += this.Remove(cs, name);
				}
			}

			if(matches != null && matches.Count > 0)
			{
				foreach(var match in matches)
				{
					conditions.Remove(match);
				}
			}

			return count;
		}

		private int Matches(ICollection<ICondition> conditions, string name, Predicate<Condition> matched)
		{
			int count = 0;

			foreach(var condition in conditions)
			{
				if(condition is Condition c)
				{
					if(string.Equals(c.Name, name, StringComparison.OrdinalIgnoreCase))
					{
						count++;

						if(matched != null && matched.Invoke(c))
							return count;
					}
				}
				else if(condition is ConditionCollection cs)
				{
					this.Matches(cs, name, matched);
				}
			}

			return count;
		}
		#endregion
	}
}

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
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Zongsoft.Data
{
	public class ConditionCollection : Zongsoft.Collections.Collection<ICondition>, ICondition
	{
		#region 成员字段
		private ConditionCombine _conditionCombine;
		#endregion

		#region 构造函数
		public ConditionCollection(ConditionCombine conditionCombine)
		{
			_conditionCombine = conditionCombine;
		}

		public ConditionCollection(ConditionCombine conditionCombine, IEnumerable<ICondition> items) : base(items)
		{
			_conditionCombine = conditionCombine;
		}

		public ConditionCollection(ConditionCombine conditionCombine, params ICondition[] items) : base(items)
		{
			_conditionCombine = conditionCombine;
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取或设置查询条件的组合方式。
		/// </summary>
		public ConditionCombine ConditionCombine
		{
			get
			{
				return _conditionCombine;
			}
			set
			{
				_conditionCombine = value;
			}
		}
		#endregion

		#region 符号重写
		public static explicit operator ConditionCollection(IConditional value)
		{
			if(value == null)
				return null;

			value.ToCondition();
		}

		public static implicit operator ConditionCollection(IConditional value)
		{
			if(value == null)
				return null;

			value.ToCondition();
		}

		public static ConditionCollection operator +(Condition condition, ConditionCollection conditions)
		{
			if(conditions == null)
				throw new ArgumentNullException("conditions");

			if(condition == null)
				return conditions;

			return new ConditionCollection(conditions.ConditionCombine, Combine(condition, conditions));
		}

		public static ConditionCollection operator +(ConditionCollection conditions, Condition condition)
		{
			if(conditions == null)
				throw new ArgumentNullException("conditions");

			if(condition == null)
				return conditions;

			return new ConditionCollection(conditions.ConditionCombine, Combine(conditions, condition));
		}

		public static ConditionCollection operator &(Condition condition, ConditionCollection conditions)
		{
			if(conditions == null)
				throw new ArgumentNullException("conditions");

			if(condition == null)
				return conditions;

			if(conditions.ConditionCombine == ConditionCombine.And)
				return new ConditionCollection(ConditionCombine.And, Combine(condition, conditions));
			else
				return new ConditionCollection(ConditionCombine.And, condition, conditions);
		}

		public static ConditionCollection operator &(ConditionCollection conditions, Condition condition)
		{
			if(conditions == null)
				throw new ArgumentNullException("conditions");

			if(condition == null)
				return conditions;

			if(conditions.ConditionCombine == ConditionCombine.And)
				return new ConditionCollection(ConditionCombine.And, Combine(conditions, condition));
			else
				return new ConditionCollection(ConditionCombine.And, conditions, condition);
		}

		public static ConditionCollection operator &(ConditionCollection left, ConditionCollection right)
		{
			if(left.ConditionCombine == ConditionCombine.And)
			{
				if(right.ConditionCombine == ConditionCombine.And)
					return new ConditionCollection(ConditionCombine.And, Combine(left, right));
				else
					return left.Append(right);
			}
			else
			{
				if(right.ConditionCombine == ConditionCombine.And)
					return right.Prepend(left);
				else
					return new ConditionCollection(ConditionCombine.And, left, right);
			}
		}

		public static ConditionCollection operator |(Condition condition, ConditionCollection conditions)
		{
			if(conditions == null)
				throw new ArgumentNullException("conditions");

			if(condition == null)
				return conditions;

			if(conditions.ConditionCombine == ConditionCombine.Or)
				return new ConditionCollection(ConditionCombine.Or, Combine(condition, conditions));
			else
				return new ConditionCollection(ConditionCombine.Or, condition, conditions);
		}

		public static ConditionCollection operator |(ConditionCollection conditions, Condition condition)
		{
			if(conditions == null)
				throw new ArgumentNullException("conditions");

			if(condition == null)
				return conditions;

			if(conditions.ConditionCombine == ConditionCombine.Or)
				return new ConditionCollection(ConditionCombine.Or, Combine(conditions, condition));
			else
				return new ConditionCollection(ConditionCombine.Or, conditions, condition);
		}

		public static ConditionCollection operator |(ConditionCollection left, ConditionCollection right)
		{
			if(left.ConditionCombine == ConditionCombine.Or)
			{
				if(right.ConditionCombine == ConditionCombine.Or)
					return new ConditionCollection(ConditionCombine.Or, Combine(left, right));
				else
					return left.Append(right);
			}
			else
			{
				if(right.ConditionCombine == ConditionCombine.Or)
					return right.Prepend(left);
				else
					return new ConditionCollection(ConditionCombine.Or, left, right);
			}
		}
		#endregion

		#region 公共方法
		/// <summary>
		/// 创建一个与当前集合内容相同的新条件集，并将指定的条件项追加到新集中。
		/// </summary>
		/// <param name="items">要追加的条件项数组。</param>
		/// <returns>返回新建的条件集合。</returns>
		public ConditionCollection Append(params ICondition[] items)
		{
			if(items == null || items.Length < 1)
				return this;

			return new ConditionCollection(this.ConditionCombine, this.Items.Concat(items).Where(item => item != null));
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

			return new ConditionCollection(this.ConditionCombine, items.Concat(this.Items).Where(item => item != null));
		}
		#endregion

		#region 重写方法
		public override string ToString()
		{
			var combiner = _conditionCombine.ToString().ToUpperInvariant();

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
	}
}

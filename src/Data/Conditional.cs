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
	public abstract class Conditional : Zongsoft.ComponentModel.NotifyObject, IConditional
	{
		#region 成员字段
		private ConditionCombine _conditionCombine;
		#endregion

		#region 构造函数
		protected Conditional()
		{
		}
		#endregion

		#region 保护属性
		protected ConditionCombine ConditionCombine
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
		public static implicit operator ConditionCollection(Conditional conditional)
		{
			if(conditional == null)
				return null;

			return conditional.ToConditions();
		}

		public static ConditionCollection operator &(Condition condition, Conditional conditional)
		{
			if(conditional == null)
				return null;

			return condition & conditional.ToConditions();
		}

		public static ConditionCollection operator &(Conditional conditional, Condition condition)
		{
			if(conditional == null)
				return null;

			return conditional.ToConditions() & condition;
		}

		public static ConditionCollection operator &(Conditional left, Conditional right)
		{
			if(left == null)
				return right;

			if(right == null)
				return left;

			return left.ToConditions() & right.ToConditions();
		}

		public static ConditionCollection operator |(Condition condition, Conditional conditional)
		{
			if(conditional == null)
				return null;

			return condition | conditional.ToConditions();
		}

		public static ConditionCollection operator |(Conditional conditional, Condition condition)
		{
			if(conditional == null)
				return null;

			return conditional.ToConditions() | condition;
		}

		public static ConditionCollection operator |(Conditional left, Conditional right)
		{
			if(left == null)
				return right;

			if(right == null)
				return left;

			return left.ToConditions() | right.ToConditions();
		}
		#endregion

		#region 公共方法
		public abstract ConditionCollection ToConditions();
		#endregion
	}
}

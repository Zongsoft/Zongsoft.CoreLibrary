/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2016-2017 Zongsoft Corporation <http://www.zongsoft.com>
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
using System.ComponentModel;
using System.Globalization;

namespace Zongsoft.Data
{
	public class ConditionalConverter : IConditionalConverter
	{
		#region 私有变量
		private static IConditionalConverter _default;
		#endregion

		#region 单例属性
		public static IConditionalConverter Default
		{
			get
			{
				if(_default == null)
					System.Threading.Interlocked.CompareExchange(ref _default, new ConditionalConverter(), null);

				return _default;
			}
			set
			{
				if(value == null)
					throw new ArgumentNullException();

				_default = value;
			}
		}
		#endregion

		#region 成员字段
		private char _wildcard;
		#endregion

		#region 构造函数
		public ConditionalConverter()
		{
			_wildcard = '%';
		}
		#endregion

		#region 公共属性
		public char Wildcard
		{
			get
			{
				return _wildcard;
			}
			set
			{
				_wildcard = value;
			}
		}
		#endregion

		#region 公共方法
		public virtual ICondition Convert(ConditionalConverterContext context)
		{
			//判断当前属性是否可以忽略
			if(this.IsIgnorable(context))
				return null;

			var opt = context.Operator;
			var isRange = typeof(IConditionalRange).IsAssignableFrom(context.Type);

			//只有当属性没有指定运算符并且不是区间属性，才需要生成运算符
			if(opt == null && (!isRange))
			{
				opt = ConditionOperator.Equal;

				if(context.Type == typeof(string) && context.Value != null)
					opt = ConditionOperator.Like;
				else if(typeof(IEnumerable).IsAssignableFrom(context.Type) || Zongsoft.Common.TypeExtension.IsAssignableFrom(typeof(IEnumerable<>), context.Type))
					opt = ConditionOperator.In;
			}

			//如果当前属性只对应一个条件
			if(context.Names.Length == 1)
			{
				if(isRange)
					return ((IConditionalRange)context.Value).ToCondition(context.Names[0]);
				else
					return new Condition(context.Names[0], this.GetValue(opt, context.Value), opt.Value);
			}

			//当一个属性对应多个条件，则这些条件之间以“或”关系进行组合
			var conditions = new ConditionCollection(ConditionCombination.Or);

			foreach(var name in context.Names)
			{
				if(isRange)
					conditions.Add(((IConditionalRange)context.Value).ToCondition(name));
				else
					conditions.Add(new Condition(name, this.GetValue(opt, context.Value), opt.Value));
			}

			return conditions;
		}
		#endregion

		#region 保护方法
		protected bool IsIgnorable(ConditionalConverterContext context)
		{
			if((context.Behaviors & ConditionalBehaviors.IgnoreNull) == ConditionalBehaviors.IgnoreNull && context.Value == null)
				return true;

			if((context.Behaviors & ConditionalBehaviors.IgnoreEmpty) == ConditionalBehaviors.IgnoreEmpty && context.Type == typeof(string) && string.IsNullOrWhiteSpace((string)context.Value))
				return true;

			if(typeof(IConditionalRange).IsAssignableFrom(context.Type))
				return context.Value == null || ((IConditionalRange)context.Value).HasValue == false;

			return false;
		}
		#endregion

		#region 私有方法
		private object GetValue(ConditionOperator? @operator, object value)
		{
			if(value == null || System.Convert.IsDBNull(value))
				return value;

			if(@operator == ConditionOperator.Like && _wildcard != '\0')
				return _wildcard + value.ToString().Trim(_wildcard) + _wildcard;

			return value;
		}
		#endregion
	}
}

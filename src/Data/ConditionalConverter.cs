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
using System.Collections.Generic;

namespace Zongsoft.Data
{
	public class ConditionalConverter : IConditionalConverter
	{
		#region 单例字段
		public static readonly ConditionalConverter Default = new ConditionalConverter();
		#endregion

		#region 公共方法
		public virtual ICondition Convert(ConditionalConverterContext context)
		{
			//如果当前属性值为默认值，则忽略它
			if(this.IsDefaultValue(context))
				return null;

			var opt = context.Operator;
			var isRange = Zongsoft.Common.TypeExtension.IsAssignableFrom(typeof(ConditionalRange), context.Type);

			//只有当属性没有指定运算符并且不是区间属性，才需要生成运算符
			if(opt == null && (!isRange))
			{
				opt = ConditionOperator.Equal;

				if(context.Type == typeof(string))
					opt = ConditionOperator.Like;
				else if(typeof(IEnumerable).IsAssignableFrom(context.Type) || Zongsoft.Common.TypeExtension.IsAssignableFrom(typeof(IEnumerable<>), context.Type))
					opt = ConditionOperator.In;
			}

			//如果当前属性只对应一个条件
			if(context.Names.Length == 1)
			{
				if(isRange)
					return ((ConditionalRange)context.Value).ToCondition(context.Names[0]);
				else
					return new Condition(context.Names[0], context.Value, opt.Value);
			}

			//当一个属性对应多个条件，则这些条件之间以“或”关系进行组合
			var conditions = new ConditionCollection(ConditionCombination.Or);

			foreach(var name in context.Names)
			{
				if(isRange)
					conditions.Add(((ConditionalRange)context.Value).ToCondition(name));
				else
					conditions.Add(new Condition(name, context.Value, opt.Value));
			}

			return conditions;
		}
		#endregion

		#region 保护方法
		protected bool IsDefaultValue(ConditionalConverterContext context)
		{
			if(Zongsoft.Common.TypeExtension.IsAssignableFrom(typeof(ConditionalRange), context.Type))
				return ConditionalRange.IsEmpty(context.Value as ConditionalRange);

			if(context.Value == null || System.Convert.IsDBNull(context.Value))
				return context.DefaultValue == null || System.Convert.IsDBNull(context.DefaultValue);

			if(context.DefaultValue == null)
				return false;

			object defaultValue;

			return Zongsoft.Common.Convert.TryConvertValue(context.DefaultValue, context.Value.GetType(), out defaultValue) &&
				   object.Equals(context.Value, defaultValue);
		}
		#endregion
	}
}

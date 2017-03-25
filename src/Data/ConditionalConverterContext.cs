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
	public class ConditionalConverterContext
	{
		#region 成员字段
		private IConditional _conditional;
		private string[] _names;
		private Type _type;
		private object _value;
		private ConditionOperator? _operator;
		#endregion

		#region 构造函数
		public ConditionalConverterContext(IConditional conditional, string[] names, Type type, object value, ConditionOperator? @operator = null)
		{
			if(conditional == null)
				throw new ArgumentNullException("conditional");

			if(names == null || names.Length < 1)
				throw new ArgumentNullException("names");

			if(type == null)
				throw new ArgumentNullException("type");

			_conditional = conditional;
			_names = names;
			_type = type;
			_value = value;
			_operator = @operator;
		}
		#endregion

		#region 公共属性
		public IConditional Conditional
		{
			get
			{
				return _conditional;
			}
		}

		public string[] Names
		{
			get
			{
				return _names;
			}
		}

		public Type Type
		{
			get
			{
				return _type;
			}
		}

		public object Value
		{
			get
			{
				return _value;
			}
		}

		public ConditionOperator? Operator
		{
			get
			{
				return _operator;
			}
		}
		#endregion
	}
}

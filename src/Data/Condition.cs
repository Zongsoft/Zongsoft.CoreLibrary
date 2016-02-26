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
using System.Text;

namespace Zongsoft.Data
{
	public class Condition : ICondition
	{
		#region 成员字段
		private string _name;
		private object _value;
		private ConditionOperator _operator;
		#endregion

		#region 构造函数
		public Condition(Condition condition)
		{
			if(condition == null)
				throw new ArgumentNullException("condition");

			_name = condition.Name;
			_value = condition.Value;
			_operator = condition.Operator;
		}

		public Condition(string name, object value, ConditionOperator @operator = ConditionOperator.Equal)
		{
			if(string.IsNullOrWhiteSpace(name))
				throw new ArgumentNullException("name");

			_name = name.Trim();
			_value = value;
			_operator = @operator;
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取或设置子句的标量名称。
		/// </summary>
		public string Name
		{
			get
			{
				return _name;
			}
			set
			{
				if(string.IsNullOrWhiteSpace(value))
					throw new ArgumentNullException();

				_name = value.Trim();
			}
		}

		/// <summary>
		/// 获取或设置子句的标量值。
		/// </summary>
		public object Value
		{
			get
			{
				return _value;
			}
			set
			{
				_value = value;
			}
		}

		/// <summary>
		/// 获取或设置子句的标量操作符。
		/// </summary>
		public ConditionOperator Operator
		{
			get
			{
				return _operator;
			}
			set
			{
				_operator = value;
			}
		}
		#endregion

		#region 公共方法
		public IEnumerable GetValues()
		{
			var items = _value as IEnumerable;

			if(items == null)
				yield return _value;
			else
			{
				foreach(var item in items)
					yield return item;
			}
		}

		public IEnumerable<T> GetValues<T>()
		{
			var items = _value as IEnumerable<T>;

			if(items == null)
				yield return (T)_value;
			else
			{
				foreach(T item in items)
					yield return item;
			}
		}
		#endregion

		#region 符号重写
		public static ConditionCollection operator &(Condition a, Condition b)
		{
			return new ConditionCollection(ConditionCombine.And, a, b);
		}

		public static ConditionCollection operator |(Condition a, Condition b)
		{
			return new ConditionCollection(ConditionCombine.Or, a, b);
		}
		#endregion

		#region 重写方法
		public override string ToString()
		{
			var value = this.Value;
			var text = this.GetValueText(value);

			switch(this.Operator)
			{
				case ConditionOperator.Equal:
					return this.IsNull(value) ? _name + " IS NULL" : string.Format("{0} == {1}", _name, text);
				case ConditionOperator.NotEqual:
					return this.IsNull(value) ? _name + " IS NOT NULL" : string.Format("{0} != {1}", _name, text);
				case ConditionOperator.GreaterThan:
					return string.Format("{0} > {1}", _name, text);
				case ConditionOperator.GreaterThanEqual:
					return string.Format("{0} >= {1}", _name, text);
				case ConditionOperator.LessThan:
					return string.Format("{0} < {1}", _name, text);
				case ConditionOperator.LessThanEqual:
					return string.Format("{0} <= {1}", _name, text);
				case ConditionOperator.Like:
					return string.Format("{0} LIKE {1}", _name, text);
				case ConditionOperator.Between:
					return string.Format("{0} BETWEEN [{1}]", _name, text);
				case ConditionOperator.In:
					return string.Format("{0} IN [{1}]", _name, text);
				case ConditionOperator.NotIn:
					return string.Format("{0} NOT IN [{1}]", _name, text);
			}

			return "*** invalid condition ***" + Environment.NewLine +
				   "{" + Environment.NewLine +
				   "\tName : " + _name + "," + Environment.NewLine +
				   "\tOperator : " + _operator + "," + Environment.NewLine +
				   "\tValue : " + text + Environment.NewLine +
				   "}";
		}
		#endregion

		#region 私有方法
		private bool IsNull(object value)
		{
			return value == null || System.Convert.IsDBNull(value);
		}

		private string GetValueText(object value)
		{
			if(this.IsNull(value))
				return "NULL";

			switch(Type.GetTypeCode(value.GetType()))
			{
				case TypeCode.Boolean:
				case TypeCode.Byte:
				case TypeCode.SByte:
				case TypeCode.Single:
				case TypeCode.Double:
				case TypeCode.Decimal:
				case TypeCode.Int16:
				case TypeCode.Int32:
				case TypeCode.Int64:
				case TypeCode.UInt16:
				case TypeCode.UInt32:
				case TypeCode.UInt64:
					return value.ToString();
				case TypeCode.Char:
					return "'" + value.ToString() + "'";
			}

			if(value is IEnumerable && (value.GetType() != typeof(string) && value.GetType() != typeof(StringBuilder)))
			{
				var text = new StringBuilder();

				foreach(var item in (IEnumerable)value)
				{
					if(text.Length > 0)
						text.Append(", ");

					text.Append(this.GetValueText(item));
				}

				return text.ToString();
			}

			return "\"" + value.ToString() + "\"";
		}
		#endregion
	}
}

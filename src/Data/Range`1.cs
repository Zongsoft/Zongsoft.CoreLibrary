﻿/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2016-2018 Zongsoft Corporation <http://www.zongsoft.com>
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

namespace Zongsoft.Data
{
	public struct Range<T> where T : struct, IComparable
	{
		#region 成员字段
		private T? _minimum;
		private T? _maximum;
		#endregion

		#region 构造函数
		public Range(T? minimum, T? maximum)
		{
			//如果两个参数都有值并且起始值大于截止值，则进行交换赋值
			if(minimum.HasValue && maximum.HasValue && minimum.Value.CompareTo(maximum.Value) > 0)
			{
				_minimum = maximum;
				_maximum = minimum;

				return;
			}

			_minimum = minimum;
			_maximum = maximum;
		}
		#endregion

		#region 公共属性
		public T? Minimum
		{
			get
			{
				return _minimum;
			}
			set
			{
				//确保设置的范围起始值小于截止值
				_minimum = this.EnsureMinimum(value);
			}
		}

		public T? Maximum
		{
			get
			{
				return _maximum;
			}
			set
			{
				//确保设置的范围截止值大于起始值
				_maximum = this.EnsureMaximum(value);
			}
		}

		[Runtime.Serialization.SerializationMember(Behavior = Runtime.Serialization.SerializationMemberBehavior.Ignored)]
		public bool HasValue
		{
			get
			{
				return _minimum.HasValue || _maximum.HasValue;
			}
		}

		[Runtime.Serialization.SerializationMember(Behavior = Runtime.Serialization.SerializationMemberBehavior.Ignored)]
		public bool IsEmpty
		{
			get
			{
				return _minimum == null && _maximum == null;
			}
		}
		#endregion

		#region 公共方法
		public Condition ToCondition(string name)
		{
			if(_minimum == null)
				return _maximum == null ? null : new Condition(name, _maximum, ConditionOperator.LessThanEqual);
			else
				return _maximum == null ? new Condition(name, _minimum, ConditionOperator.GreaterThanEqual) : new Condition(name, new object[] { _minimum, _maximum }, ConditionOperator.Between);
		}
		#endregion

		#region 静态方法
		public static Range<T> Parse(string text)
		{
			Range<T> result;

			if(TryParse(text, out result))
				return result;

			throw new ArgumentException(string.Format("Invalid value '{0}' of the argument.", text));
		}

		public static bool TryParse(string text, out Range<T> result)
		{
			result = default(Range<T>);

			if(string.IsNullOrWhiteSpace(text))
				return false;

			text = text.Trim().Trim('(', ')');

			if(string.IsNullOrWhiteSpace(text))
				return false;

			var parts = text.Split('~').Select(p => p.Trim().Trim('?', '*')).ToArray();

			if(!string.IsNullOrWhiteSpace(parts[0]))
			{
				T from;

				if(!Zongsoft.Common.Convert.TryConvertValue(parts[0], out from))
					return false;

				result.Minimum = from;
			}

			if(parts.Length > 1 && !string.IsNullOrWhiteSpace(parts[1]))
			{
				T to;

				if(!Zongsoft.Common.Convert.TryConvertValue(parts[1], out to))
					return false;

				result.Maximum = to;
			}

			//最后返回真（即使输出参数为空）
			return true;
		}
		#endregion

		#region 私有方法
		private T? EnsureMinimum(T? value)
		{
			if(value != null)
			{
				var to = _maximum;

				if(to != null && to.Value.CompareTo(value.Value) < 0)
				{
					_maximum = value;
					return to;
				}
			}

			return value;
		}

		private T? EnsureMaximum(T? value)
		{
			if(value != null)
			{
				var from = _minimum;

				if(from != null && from.Value.CompareTo(value.Value) > 0)
				{
					_minimum = value;
					return from;
				}
			}

			return value;
		}
		#endregion

		#region 重写方法
		public override string ToString()
		{
			if(_minimum == null)
			{
				if(_maximum == null)
					return string.Empty;
				else
					return string.Format("(~{0})", _maximum.ToString());
			}
			else
			{
				if(_maximum == null)
					return string.Format("({0})", _minimum.ToString());
				else
					return string.Format("({0}~{1})", _minimum.ToString(), _maximum.ToString());
			}
		}
		#endregion
	}
}

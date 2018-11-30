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
using System.Linq;

namespace Zongsoft.Data
{
	[Obsolete("Please user Range<> class.")]
	public class ConditionalRange<T> : IConditionalRange where T : struct, IComparable
	{
		#region 成员字段
		private T? _from;
		private T? _to;
		#endregion

		#region 构造函数
		public ConditionalRange()
		{
		}

		public ConditionalRange(T? from, T? to)
		{
			this.From = from;
			this.To = to;
		}
		#endregion

		#region 公共属性
		public T? From
		{
			get
			{
				return _from;
			}
			set
			{
				//确保设置的范围起始值小于截止值
				_from = this.EnsureFrom(value);
			}
		}

		public T? To
		{
			get
			{
				return _to;
			}
			set
			{
				//确保设置的范围截止值大于起始值
				_to = this.EnsureTo(value);
			}
		}

		[Runtime.Serialization.SerializationMember(Behavior = Runtime.Serialization.SerializationMemberBehavior.Ignored)]
		public bool HasValue
		{
			get
			{
				return _from.HasValue || _to.HasValue;
			}
		}

		[Runtime.Serialization.SerializationMember(Behavior = Runtime.Serialization.SerializationMemberBehavior.Ignored)]
		public bool IsEmpty
		{
			get
			{
				return _from == null && _to == null;
			}
		}
		#endregion

		#region 公共方法
		public Condition ToCondition(string name)
		{
			if(_from == null)
				return _to == null ? null : new Condition(name, _to, ConditionOperator.LessThanEqual);
			else
				return _to == null ? new Condition(name, _from, ConditionOperator.GreaterThanEqual) : new Condition(name, new object[] { _from, _to }, ConditionOperator.Between);
		}
		#endregion

		#region 静态方法
		public static ConditionalRange<T> Parse(string text)
		{
			ConditionalRange<T> result;

			if(TryParse(text, out result))
				return result;

			throw new ArgumentException(string.Format("Invalid value '{0}' of the argument.", text));
		}

		public static bool TryParse(string text, out ConditionalRange<T> result)
		{
			result = null;

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

				if(result == null)
					result = new ConditionalRange<T>();

				result.From = from;
			}

			if(parts.Length > 1 && !string.IsNullOrWhiteSpace(parts[1]))
			{
				T to;

				if(!Zongsoft.Common.Convert.TryConvertValue(parts[1], out to))
					return false;

				if(result == null)
					result = new ConditionalRange<T>();

				result.To = to;
			}

			//最后返回真（即使输出参数为空）
			return true;
		}
		#endregion

		#region 私有方法
		private T? EnsureFrom(T? value)
		{
			if(value != null)
			{
				var to = _to;

				if(to != null && to.Value.CompareTo(value.Value) < 0)
				{
					_to = value;
					return to;
				}
			}

			return value;
		}

		private T? EnsureTo(T? value)
		{
			if(value != null)
			{
				var from = _from;

				if(from != null && from.Value.CompareTo(value.Value) > 0)
				{
					_from = value;
					return from;
				}
			}

			return value;
		}
		#endregion

		#region 重写方法
		public override string ToString()
		{
			if(_from == null)
			{
				if(_to == null)
					return string.Empty;
				else
					return string.Format("(~{0})", _to.ToString());
			}
			else
			{
				if(_to == null)
					return string.Format("({0})", _from.ToString());
				else
					return string.Format("({0}~{1})", _from.ToString(), _to.ToString());
			}
		}
		#endregion
	}
}

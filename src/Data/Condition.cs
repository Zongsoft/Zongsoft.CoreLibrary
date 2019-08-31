/*
 *   _____                                ______
 *  /_   /  ____  ____  ____  _________  / __/ /_
 *    / /  / __ \/ __ \/ __ \/ ___/ __ \/ /_/ __/
 *   / /__/ /_/ / / / / /_/ /\_ \/ /_/ / __/ /_
 *  /____/\____/_/ /_/\__  /____/\____/_/  \__/
 *                   /____/
 *
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@qq.com>
 *
 * Copyright (C) 2008-2019 Zongsoft Corporation <http://www.zongsoft.com>
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
using System.Linq.Expressions;
using System.Text;

namespace Zongsoft.Data
{
	/// <summary>
	/// 表示数据过滤条件的设置项。
	/// </summary>
	public class Condition : ICondition, IEquatable<Condition>
	{
		#region 成员字段
		private string _name;
		private object _value;
		private ConditionOperator _operator;
		#endregion

		#region 构造函数
		public Condition(string name, object value, ConditionOperator @operator = ConditionOperator.Equal)
		{
			if(string.IsNullOrWhiteSpace(name))
				throw new ArgumentNullException(nameof(name));

			_name = name.Trim();
			_value = value;
			_operator = @operator;
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取或设置条件项的名称。
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
		/// 获取或设置条件项的值。
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
		/// 获取或设置条件项的操作符。
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

		#region 静态方法
		public static Condition Equal(string name, object value)
		{
			return new Condition(name, value, ConditionOperator.Equal);
		}

		public static Condition NotEqual(string name, object value)
		{
			return new Condition(name, value, ConditionOperator.NotEqual);
		}

		public static Condition GreaterThan(string name, object value)
		{
			return new Condition(name, value, ConditionOperator.GreaterThan);
		}

		public static Condition GreaterThanEqual(string name, object value)
		{
			return new Condition(name, value, ConditionOperator.GreaterThanEqual);
		}

		public static Condition LessThan(string name, object value)
		{
			return new Condition(name, value, ConditionOperator.LessThan);
		}

		public static Condition LessThanEqual(string name, object value)
		{
			return new Condition(name, value, ConditionOperator.LessThanEqual);
		}

		public static Condition Like(string name, string value)
		{
			if(string.IsNullOrEmpty(value))
				return new Condition(name, value, ConditionOperator.Equal);
			else
				return new Condition(name, value, ConditionOperator.Like);
		}

		public static Condition Between<T>(string name, Range<T> range) where T : struct, IComparable<T>
		{
			return range.ToCondition(name);
		}

		public static Condition Between<T>(string name, T begin, T end) where T : struct, IComparable<T>
		{
			T[] value;

			//如果起始值大于截止值则交换位置
			if(Comparer<T>.Default.Compare(begin, end) > 0)
				value = new T[] { end, begin };
			else
				value = new T[] { begin, end };

			return new Condition(name, value, ConditionOperator.Between);
		}

		public static Condition Between<T>(string name, T? begin, T? end) where T : struct, IComparable<T>
		{
			//如果两个参数都有值并且起始值大于截止值，则进行交换赋值
			if(begin.HasValue && end.HasValue)
			{
				if(Comparer<T>.Default.Compare(begin.Value, end.Value) > 0)
					return new Condition(name, new T[] { end.Value, begin.Value }, ConditionOperator.Between);
				else
					return new Condition(name, new T[] { begin.Value, end.Value }, ConditionOperator.Between);
			}

			if(begin.HasValue)
				return Condition.GreaterThanEqual(name, begin.Value);
			else if(end.HasValue)
				return Condition.LessThanEqual(name, end.Value);

			return null;
		}

		public static Condition In<T>(string name, IEnumerable<T> values) where T : IEquatable<T>
		{
			if(values == null)
				throw new ArgumentNullException("values");

			return new Condition(name, values, ConditionOperator.In);
		}

		public static Condition In<T>(string name, params T[] values) where T : IEquatable<T>
		{
			if(values == null)
				throw new ArgumentNullException("values");

			return new Condition(name, values, ConditionOperator.In);
		}

		public static Condition NotIn<T>(string name, IEnumerable<T> values) where T : IEquatable<T>
		{
			if(values == null)
				throw new ArgumentNullException("values");

			return new Condition(name, values, ConditionOperator.NotIn);
		}

		public static Condition NotIn<T>(string name, params T[] values) where T : IEquatable<T>
		{
			if(values == null)
				throw new ArgumentNullException("values");

			return new Condition(name, values, ConditionOperator.NotIn);
		}

		public static Condition Exists(string name)
		{
			return new Condition(name, null, ConditionOperator.Exists);
		}

		public static Condition NotExists(string name)
		{
			return new Condition(name, null, ConditionOperator.NotExists);
		}

		public static bool GetBetween(Condition condition, out object begin, out object end)
		{
			begin = end = null;

			if(condition == null || condition.Operator != ConditionOperator.Between || condition.Value == null)
				return false;

			if(condition.Value is IEnumerable)
			{
				int index = 0;

				foreach(var item in (IEnumerable)condition.Value)
				{
					if(index++ == 0)
						begin = item;
					else
						end = item;

					if(index >= 2)
						break;
				}

				return index > 0;
			}

			if(Zongsoft.Common.TypeExtension.IsAssignableFrom(typeof(Tuple<,>), condition.Value.GetType()))
			{
				begin = Reflection.Reflector.GetValue(condition.Value, "Item1");
				end = Reflection.Reflector.GetValue(condition.Value, "Item2");

				return true;
			}

			return false;
		}
		#endregion

		#region 符号重写
		public static ConditionCollection operator &(Condition a, Condition b)
		{
			if(a == null)
			{
				if(b == null)
					return null;
				else
					return new ConditionCollection(ConditionCombination.And, b);
			}
			else
			{
				if(b == null)
					return new ConditionCollection(ConditionCombination.And, a);
				else
					return new ConditionCollection(ConditionCombination.And, a, b);
			}
		}

		public static ConditionCollection operator |(Condition a, Condition b)
		{
			if(a == null)
			{
				if(b == null)
					return null;
				else
					return new ConditionCollection(ConditionCombination.Or, b);
			}
			else
			{
				if(b == null)
					return new ConditionCollection(ConditionCombination.Or, a);
				else
					return new ConditionCollection(ConditionCombination.Or, a, b);
			}
		}
		#endregion

		#region 重写方法
		public bool Equals(Condition other)
		{
			if(other == null)
				return false;

			return string.Equals(_name, other.Name, StringComparison.OrdinalIgnoreCase) &&
			       _operator == other.Operator && _value == other.Value;
		}

		public override bool Equals(object obj)
		{
			return obj != null && this.Equals(obj as Condition);
		}

		public override int GetHashCode()
		{
			if(_value == null)
				return _name.GetHashCode() ^ _operator.GetHashCode();
			else
				return _name.GetHashCode() ^ _operator.GetHashCode() ^ _value.GetHashCode();
		}

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

		#region 显式实现
		bool ICondition.Contains(string name)
		{
			return string.Equals(name, _name, StringComparison.OrdinalIgnoreCase);
		}

		bool ICondition.Match(string name, Action<Condition> matched)
		{
			if(name != null && name.Length > 0 && string.Equals(name, _name, StringComparison.OrdinalIgnoreCase))
			{
				matched?.Invoke(this);
				return true;
			}

			return false;
		}

		int ICondition.Matches(string name, Action<Condition> matched)
		{
			if(name != null && name.Length > 0 && string.Equals(name, _name, StringComparison.OrdinalIgnoreCase))
			{
				matched?.Invoke(this);
				return 1;
			}

			return 0;
		}
		#endregion

		#region 嵌套子类
		public class Builder<T>
		{
			#region 构造函数
			protected Builder()
			{
			}
			#endregion

			#region 文本版本
			public static Condition Equal(string name, object value)
			{
				return Condition.Equal(name, value);
			}

			public static Condition NotEqual(string name, object value)
			{
				return Condition.NotEqual(name, value);
			}

			public static Condition GreaterThan(string name, object value)
			{
				return Condition.GreaterThan(name, value);
			}

			public static Condition GreaterThanEqual(string name, object value)
			{
				return Condition.GreaterThanEqual(name, value);
			}

			public static Condition LessThan(string name, object value)
			{
				return Condition.LessThan(name, value);
			}

			public static Condition LessThanEqual(string name, object value)
			{
				return Condition.LessThanEqual(name, value);
			}

			public static Condition Like(string name, string value)
			{
				return Condition.Like(name, value);
			}

			public static Condition Between<TValue>(string name, Range<TValue> range) where TValue : struct, IComparable<TValue>
			{
				return Condition.Between<TValue>(name, range);
			}

			public static Condition Between<TValue>(string name, TValue begin, TValue end) where TValue : struct, IComparable<TValue>
			{
				return Condition.Between<TValue>(name, begin, end);
			}

			public static Condition Between<TValue>(string name, TValue? begin, TValue? end) where TValue : struct, IComparable<TValue>
			{
				return Condition.Between<TValue>(name, begin, end);
			}

			public static Condition In<TValue>(string name, IEnumerable<TValue> values) where TValue : IEquatable<TValue>
			{
				return Condition.In<TValue>(name, values);
			}

			public static Condition In<TValue>(string name, params TValue[] values) where TValue : IEquatable<TValue>
			{
				return Condition.In<TValue>(name, values);
			}

			public static Condition NotIn<TValue>(string name, IEnumerable<TValue> values) where TValue : IEquatable<TValue>
			{
				return Condition.NotIn<TValue>(name, values);
			}

			public static Condition NotIn<TValue>(string name, params TValue[] values) where TValue : IEquatable<TValue>
			{
				return Condition.NotIn<TValue>(name, values);
			}

			public static Condition Exists(string name)
			{
				return Condition.Exists(name);
			}

			public static Condition NotExists(string name)
			{
				return Condition.NotExists(name);
			}
			#endregion

			#region 表达式版本
			public static Condition Equal<TValue>(Expression<Func<T, TValue>> member, TValue value)
			{
				return Condition.Equal(Reflection.ExpressionUtility.GetMemberName(member), value);
			}

			public static Condition NotEqual<TValue>(Expression<Func<T, TValue>> member, TValue value)
			{
				return Condition.NotEqual(Reflection.ExpressionUtility.GetMemberName(member), value);
			}

			public static Condition GreaterThan<TValue>(Expression<Func<T, TValue>> member, TValue value)
			{
				return Condition.GreaterThan(Reflection.ExpressionUtility.GetMemberName(member), value);
			}

			public static Condition GreaterThanEqual<TValue>(Expression<Func<T, TValue>> member, TValue value)
			{
				return Condition.GreaterThanEqual(Reflection.ExpressionUtility.GetMemberName(member), value);
			}

			public static Condition LessThan<TValue>(Expression<Func<T, TValue>> member, TValue value)
			{
				return Condition.LessThan(Reflection.ExpressionUtility.GetMemberName(member), value);
			}

			public static Condition LessThanEqual<TValue>(Expression<Func<T, TValue>> member, TValue value)
			{
				return Condition.LessThanEqual(Reflection.ExpressionUtility.GetMemberName(member), value);
			}

			public static Condition Like(Expression<Func<T, string>> member, string value)
			{
				return Condition.Like(Reflection.ExpressionUtility.GetMemberName(member), value);
			}

			public static Condition Between<TValue>(Expression<Func<T, TValue>> member, Range<TValue> range) where TValue : struct, IComparable<TValue>
			{
				return Condition.Between<TValue>(Reflection.ExpressionUtility.GetMemberName(member), range);
			}

			public static Condition Between<TValue>(Expression<Func<T, TValue>> member, TValue begin, TValue end) where TValue : struct, IComparable<TValue>
			{
				return Condition.Between(Reflection.ExpressionUtility.GetMemberName(member), begin, end);
			}

			public static Condition Between<TValue>(Expression<Func<T, TValue>> member, TValue? begin, TValue? end) where TValue : struct, IComparable<TValue>
			{
				return Condition.Between(Reflection.ExpressionUtility.GetMemberName(member), begin, end);
			}

			public static Condition In<TValue>(Expression<Func<T, TValue>> member, params TValue[] values) where TValue : IEquatable<TValue>
			{
				return Condition.In(Reflection.ExpressionUtility.GetMemberName(member), values);
			}

			public static Condition In<TValue>(Expression<Func<T, TValue>> member, IEnumerable<TValue> values) where TValue : IEquatable<TValue>
			{
				return Condition.In(Reflection.ExpressionUtility.GetMemberName(member), values);
			}

			public static Condition NotIn<TValue>(Expression<Func<T, TValue>> member, params TValue[] values) where TValue : IEquatable<TValue>
			{
				return Condition.NotIn(Reflection.ExpressionUtility.GetMemberName(member), values);
			}

			public static Condition NotIn<TValue>(Expression<Func<T, TValue>> member, IEnumerable<TValue> values) where TValue : IEquatable<TValue>
			{
				return Condition.NotIn(Reflection.ExpressionUtility.GetMemberName(member), values);
			}
			#endregion
		}
		#endregion
	}
}

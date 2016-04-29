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
using System.Collections.Concurrent;
using System.Linq.Expressions;

namespace Zongsoft.Data
{
	public class ConditionBuilder : IConditionBuilder
	{
		#region 成员字段
		private static readonly ConcurrentDictionary<Type, ConditionBuilder> _builders = new ConcurrentDictionary<Type, ConditionBuilder>();
		#endregion

		#region 构造函数
		internal ConditionBuilder()
		{
		}
		#endregion

		#region 构建方法
		public Condition Equal(string name, object value)
		{
			return Condition.Equal(name, value);
		}

		public Condition NotEqual(string name, object value)
		{
			return Condition.NotEqual(name, value);
		}

		public Condition GreaterThan(string name, object value)
		{
			return Condition.GreaterThan(name, value);
		}

		public Condition GreaterThanEqual(string name, object value)
		{
			return Condition.GreaterThanEqual(name, value);
		}

		public Condition LessThan(string name, object value)
		{
			return Condition.LessThan(name, value);
		}

		public Condition LessThanEqual(string name, object value)
		{
			return Condition.LessThanEqual(name, value);
		}

		public Condition Like(string name, string value)
		{
			return Condition.Like(name, value);
		}

		public Condition Between<TValue>(string name, TValue begin, TValue end) where TValue : IComparable<TValue>
		{
			return Condition.Between<TValue>(name, begin, end);
		}

		public Condition Between<TValue>(string name, TValue? begin, TValue? end) where TValue : struct, IComparable<TValue>
		{
			return Condition.Between<TValue>(name, begin, end);
		}

		public Condition In<TValue>(string name, IEnumerable<TValue> values) where TValue : IEquatable<TValue>
		{
			return Condition.In<TValue>(name, values);
		}

		public Condition In<TValue>(string name, params TValue[] values) where TValue : IEquatable<TValue>
		{
			return Condition.In<TValue>(name, values);
		}

		public Condition NotIn<TValue>(string name, IEnumerable<TValue> values) where TValue : IEquatable<TValue>
		{
			return Condition.NotIn<TValue>(name, values);
		}

		public Condition NotIn<TValue>(string name, params TValue[] values) where TValue : IEquatable<TValue>
		{
			return Condition.NotIn<TValue>(name, values);
		}
		#endregion

		#region 工厂方法
		public static IConditionBuilder<T> Get<T>()
		{
			return (IConditionBuilder<T>)_builders.GetOrAdd(typeof(T), key =>
			{
				return new ConditionBuilder<T>();
			});
		}
		#endregion
	}

	internal sealed class ConditionBuilder<T> : ConditionBuilder, IConditionBuilder<T>
	{
		#region 构造函数
		internal ConditionBuilder()
		{
		}
		#endregion

		#region 构建方法
		public Condition Equal<TMember>(Expression<Func<T, TMember>> member, TMember value)
		{
			return Condition.Equal(Common.ExpressionUtility.GetMemberName(member), value);
		}

		public Condition NotEqual<TMember>(Expression<Func<T, TMember>> member, TMember value)
		{
			return Condition.NotEqual(Common.ExpressionUtility.GetMemberName(member), value);
		}

		public Condition GreaterThan<TMember>(Expression<Func<T, TMember>> member, TMember value)
		{
			return Condition.GreaterThan(Common.ExpressionUtility.GetMemberName(member), value);
		}

		public Condition GreaterThanEqual<TMember>(Expression<Func<T, TMember>> member, TMember value)
		{
			return Condition.GreaterThanEqual(Common.ExpressionUtility.GetMemberName(member), value);
		}

		public Condition LessThan<TMember>(Expression<Func<T, TMember>> member, TMember value)
		{
			return Condition.LessThan(Common.ExpressionUtility.GetMemberName(member), value);
		}

		public Condition LessThanEqual<TMember>(Expression<Func<T, TMember>> member, TMember value)
		{
			return Condition.LessThanEqual(Common.ExpressionUtility.GetMemberName(member), value);
		}

		public Condition Like(Expression<Func<T, string>> member, string value)
		{
			return Condition.Like(Common.ExpressionUtility.GetMemberName(member), value);
		}

		public Condition Between<TMember>(Expression<Func<T, TMember>> member, TMember? begin, TMember? end) where TMember : struct, IComparable<TMember>
		{
			return Condition.Between(Common.ExpressionUtility.GetMemberName(member), begin, end);
		}

		public Condition Between<TMember>(Expression<Func<T, TMember>> member, TMember begin, TMember end) where TMember : IComparable<TMember>
		{
			return Condition.Between(Common.ExpressionUtility.GetMemberName(member), begin, end);
		}

		public Condition In<TMember>(Expression<Func<T, TMember>> member, params TMember[] values) where TMember : IEquatable<TMember>
		{
			return Condition.In(Common.ExpressionUtility.GetMemberName(member), values);
		}

		public Condition In<TMember>(Expression<Func<T, TMember>> member, IEnumerable<TMember> values) where TMember : IEquatable<TMember>
		{
			return Condition.In(Common.ExpressionUtility.GetMemberName(member), values);
		}

		public Condition NotIn<TMember>(Expression<Func<T, TMember>> member, params TMember[] values) where TMember : IEquatable<TMember>
		{
			return Condition.NotIn(Common.ExpressionUtility.GetMemberName(member), values);
		}

		public Condition NotIn<TMember>(Expression<Func<T, TMember>> member, IEnumerable<TMember> values) where TMember : IEquatable<TMember>
		{
			return Condition.NotIn(Common.ExpressionUtility.GetMemberName(member), values);
		}
		#endregion
	}
}

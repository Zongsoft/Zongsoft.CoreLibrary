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
using System.Linq.Expressions;

namespace Zongsoft.Data
{
	public interface IConditionBuilder
	{
		Condition Equal(string name, object value);

		Condition NotEqual(string name, object value);

		Condition GreaterThan(string name, object value);

		Condition GreaterThanEqual(string name, object value);

		Condition LessThan(string name, object value);

		Condition LessThanEqual(string name, object value);

		Condition Like(string name, string value);

		Condition Between<TValue>(string name, TValue begin, TValue end) where TValue : IComparable<TValue>;

		Condition Between<TValue>(string name, TValue? begin, TValue? end) where TValue : struct, IComparable<TValue>;

		Condition In<TValue>(string name, IEnumerable<TValue> values) where TValue : IEquatable<TValue>;

		Condition In<TValue>(string name, params TValue[] values) where TValue : IEquatable<TValue>;

		Condition NotIn<TValue>(string name, IEnumerable<TValue> values) where TValue : IEquatable<TValue>;

		Condition NotIn<TValue>(string name, params TValue[] values) where TValue : IEquatable<TValue>;
	}

	public interface IConditionBuilder<T> : IConditionBuilder
	{
		Condition Equal<TMember>(Expression<Func<T, TMember>> member, TMember value);

		Condition NotEqual<TMember>(Expression<Func<T, TMember>> member, TMember value);

		Condition GreaterThan<TMember>(Expression<Func<T, TMember>> member, TMember value);

		Condition GreaterThanEqual<TMember>(Expression<Func<T, TMember>> member, TMember value);

		Condition LessThan<TMember>(Expression<Func<T, TMember>> member, TMember value);

		Condition LessThanEqual<TMember>(Expression<Func<T, TMember>> member, TMember value);

		Condition Like(Expression<Func<T, string>> member, string value);

		Condition Between<TMember>(Expression<Func<T, TMember>> member, TMember begin, TMember end) where TMember : IComparable<TMember>;

		Condition Between<TMember>(Expression<Func<T, TMember>> member, TMember? begin, TMember? end) where TMember : struct, IComparable<TMember>;

		Condition In<TMember>(Expression<Func<T, TMember>> member, IEnumerable<TMember> values) where TMember : IEquatable<TMember>;

		Condition In<TMember>(Expression<Func<T, TMember>> member, params TMember[] values) where TMember : IEquatable<TMember>;

		Condition NotIn<TMember>(Expression<Func<T, TMember>> member, IEnumerable<TMember> values) where TMember : IEquatable<TMember>;

		Condition NotIn<TMember>(Expression<Func<T, TMember>> member, params TMember[] values) where TMember : IEquatable<TMember>;
	}
}

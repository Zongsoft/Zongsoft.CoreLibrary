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
using System.Linq.Expressions;

namespace Zongsoft.Data
{
	public interface IDataDictionary<T> : IDataDictionary
	{
		bool Contains<TMember>(Expression<Func<T, TMember>> expression);

		bool Reset<TValue>(Expression<Func<T, TValue>> expression, out TValue value);

		TValue GetValue<TValue>(Expression<Func<T, TValue>> expression);
		TValue GetValue<TValue>(Expression<Func<T, TValue>> expression, TValue defaultValue);

		void SetValue<TValue>(Expression<Func<T, TValue>> expression, TValue value, Func<TValue, bool> predicate = null);
		void SetValue<TValue>(Expression<Func<T, TValue>> expression, Func<string, TValue> valueFactory, Func<TValue, bool> predicate = null);

		bool TryGetValue<TValue>(Expression<Func<T, TValue>> expression, out TValue value);
		bool TryGetValue<TValue>(Expression<Func<T, TValue>> expression, Action<string, TValue> got);

		bool TrySetValue<TValue>(Expression<Func<T, TValue>> expression, TValue value, Func<TValue, bool> predicate = null);
		bool TrySetValue<TValue>(Expression<Func<T, TValue>> expression, Func<string, TValue> valueFactory, Func<TValue, bool> predicate = null);
	}
}

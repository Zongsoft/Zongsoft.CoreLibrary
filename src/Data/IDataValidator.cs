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
 * Copyright (C) 2016-2019 Zongsoft Corporation <http://www.zongsoft.com>
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
	/// <summary>
	/// 提供数据访问中的条件和数据进行验证修正的接口。
	/// </summary>
	public interface IDataValidator
	{
		/// <summary>
		/// 验证并修正当前执行上下文中的操作条件。
		/// </summary>
		/// <param name="context">当前执行上下文。</param>
		/// <param name="criteria">当前数据访问条件。</param>
		/// <returns>返回修正验证后的数据访问条件。</returns>
		ICondition Validate(IDataAccessContextBase context, ICondition criteria);

		/// <summary>
		/// 当新增(Insert,Upsert)操作时，验证和提供指定属性的值。
		/// </summary>
		/// <param name="context">当前数据访问上下文对象。</param>
		/// <param name="property">新增的属性。</param>
		/// <param name="value">输出参数，尝试写入的属性值。如果返回<see cref="IDataValueBinder"/>类型，则表示写入的值由后期动态绑定。</param>
		/// <returns>如果指定的<paramref name="property"/>属性有必须要写入数据则返回真(True)，否则返回假(False)。</returns>
		bool OnInsert(IDataMutateContextBase context, Metadata.IDataEntityProperty property, out object value);

		/// <summary>
		/// 当更新(Update,Upsert)操作时，验证和提供指定属性的值。
		/// </summary>
		/// <param name="context">当前数据访问上下文对象。</param>
		/// <param name="property">新增的属性。</param>
		/// <param name="value">输出参数，尝试写入的属性值。如果返回<see cref="IDataValueBinder"/>类型，则表示写入的值由后期动态绑定。</param>
		/// <returns>如果指定的<paramref name="property"/>属性有必须要写入数据则返回真(True)，否则返回假(False)。</returns>
		bool OnUpdate(IDataMutateContextBase context, Metadata.IDataEntityProperty property, out object value);
	}
}

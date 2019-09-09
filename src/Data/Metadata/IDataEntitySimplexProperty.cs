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
 * Copyright (C) 2015-2019 Zongsoft Corporation <http://www.zongsoft.com>
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

namespace Zongsoft.Data.Metadata
{
	/// <summary>
	/// 表示数据实体单值属性的元数据类。
	/// </summary>
	public interface IDataEntitySimplexProperty : IDataEntityProperty
	{
		/// <summary>
		/// 获取或设置默认值。
		/// </summary>
		object Value
		{
			get; set;
		}

		/// <summary>
		/// 获取或设置文本或数组属性的最大长度，单位：字节。
		/// </summary>
		int Length
		{
			get; set;
		}

		/// <summary>
		/// 获取或设置数值属性的精度。
		/// </summary>
		byte Precision
		{
			get; set;
		}

		/// <summary>
		/// 获取或设置数值属性的小数点位数。
		/// </summary>
		byte Scale
		{
			get; set;
		}

		/// <summary>
		/// 获取或设置属性是否允许为空。
		/// </summary>
		bool Nullable
		{
			get; set;
		}

		/// <summary>
		/// 获取或设置属性是否可以参与排序。
		/// </summary>
		bool Sortable
		{
			get; set;
		}

		/// <summary>
		/// 获取数据序号器元数据。
		/// </summary>
		IDataEntityPropertySequence Sequence
		{
			get;
		}
	}
}

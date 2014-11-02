/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2014 Zongsoft Corporation <http://www.zongsoft.com>
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

namespace Zongsoft.Services.Composition
{
	public interface IExecutionContext
	{
		/// <summary>
		/// 获取处理本次执行请求的执行器。
		/// </summary>
		IExecutor Executor
		{
			get;
		}

		/// <summary>
		/// 获取扩展属性集是否有内容。
		/// </summary>
		/// <remarks>
		///		<para>在不确定扩展属性集是否含有内容之前，建议先使用该属性来检测。</para>
		/// </remarks>
		bool HasExtendedProperties
		{
			get;
		}

		/// <summary>
		/// 获取扩展属性集。
		/// </summary>
		IDictionary<string, object> ExtendedProperties
		{
			get;
		}

		/// <summary>
		/// 获取或设置当前执行的返回结果。
		/// </summary>
		object Result
		{
			get;
			set;
		}
	}
}

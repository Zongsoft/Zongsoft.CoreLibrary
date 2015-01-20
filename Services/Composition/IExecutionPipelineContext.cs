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
	public interface IExecutionPipelineContext : IExecutionContext
	{
		/// <summary>
		/// 获取原始的<see cref="IExecutorContext"/>执行器上下文对象。
		/// </summary>
		IExecutorContext ExecutorContext
		{
			get;
		}

		/// <summary>
		/// 获取当前上下文所属的执行管道。
		/// </summary>
		ExecutionPipeline Pipeline
		{
			get;
		}

		/// <summary>
		/// 获取当前上下文的前一个管道。
		/// </summary>
		ExecutionPipeline Previous
		{
			get;
		}

		/// <summary>
		/// 获取或设置当前上下文的下一个管道。
		/// </summary>
		ExecutionPipeline Next
		{
			get;
			set;
		}

		/// <summary>
		/// 切换当前上下文到下一个管道。
		/// </summary>
		/// <param name="updatePrevious">是否更新<seealso cref="Previous"/>属性值。</param>
		/// <returns>如果切换成功则返回真(true)，否则返回假(false)。</returns>
		bool ToNext(bool updatePrevious);
	}
}

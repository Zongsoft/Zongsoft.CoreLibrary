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

namespace Zongsoft.Communication.Composition
{
	public interface IExecutionPipelineContext : IExecutionContext
	{
		/// <summary>
		/// 获取当前上下文所属的执行管道。
		/// </summary>
		ExecutionPipeline Pipeline
		{
			get;
		}

		/// <summary>
		/// 判断当前管道是否有子管道集。
		/// </summary>
		bool HasChildren
		{
			get;
		}

		/// <summary>
		/// 获取当前管道的子管道集合。
		/// </summary>
		ExecutionPipelineCollection Children
		{
			get;
		}
	}
}

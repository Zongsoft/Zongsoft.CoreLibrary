/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2010-2013 Zongsoft Corporation <http://www.zongsoft.com>
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
	public class ExecutionFilteredContext : ExecutionContext
	{
		#region 成员字段
		private bool _isHandled;
		#endregion

		#region 构造函数
		internal ExecutionFilteredContext(ExecutionContext context, bool isHandled) : base(context)
		{
			_isHandled = isHandled;
		}

		internal ExecutionFilteredContext(Executor executor, object parameter, ExecutionPipeline pipeline, bool isHandled) : base(executor, parameter, pipeline)
		{
			_isHandled = isHandled;
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取一个值，表示当前执行管道中的处理程序是否通过执行。
		/// </summary>
		/// <remarks>
		///		<para>由于执行管道中的处理程序可能没有通过其条件检测，因此其未必会得到执行。那么在管道中的过滤器的后置事件中可通过该属性来获取其是否成功被调用的指示。</para>
		/// </remarks>
		public bool IsHandled
		{
			get
			{
				return _isHandled;
			}
		}
		#endregion
	}
}

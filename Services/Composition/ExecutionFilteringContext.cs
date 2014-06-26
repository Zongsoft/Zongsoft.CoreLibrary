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
	public class ExecutionFilteringContext : ExecutionContext
	{
		#region 成员字段
		private bool _cancel;
		#endregion

		#region 构造函数
		internal ExecutionFilteringContext(ExecutionContext context) : base(context)
		{
			_cancel = false;
		}

		internal ExecutionFilteringContext(Executor executor, object parameter, ExecutionPipeline pipeline) : base(executor, parameter, pipeline)
		{
			_cancel = false;
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取或设置指示是否应取消后续过滤器事件的值，默认为否(flase)。
		/// </summary>
		/// <remarks>
		///		<para>该属性值如果设置为真(true)，表示当前过滤器的<see cref="IExecutionFilter.OnExecuted"/>方法将不会被调用。</para>
		/// </remarks>
		public bool Cancel
		{
			get
			{
				return _cancel;
			}
			set
			{
				_cancel = value;
			}
		}
		#endregion
	}
}

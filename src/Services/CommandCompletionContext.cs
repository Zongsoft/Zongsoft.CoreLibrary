/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2010-2017 Zongsoft Corporation <http://www.zongsoft.com>
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

namespace Zongsoft.Services
{
	public class CommandCompletionContext : CommandExecutorContext
	{
		#region 成员字段
		private object _result;
		private Exception _exception;
		#endregion

		#region 构造函数
		public CommandCompletionContext(ICommandExecutor executor, CommandExpression expression, object parameter, object result, Exception exception = null) : base(executor, expression, parameter)
		{
			_result = result;
			_exception = exception;
		}
		#endregion

		#region 公共属性
		public object Result
		{
			get
			{
				return _result;
			}
			set
			{
				_result = value;
			}
		}

		public Exception Exception
		{
			get
			{
				return _exception;
			}
			set
			{
				_exception = value;
			}
		}
		#endregion

		#region 静态方法
		public static CommandCompletionContext Create(CommandExecutorContext context, object result, Exception exception = null)
		{
			if(context == null)
				throw new ArgumentNullException(nameof(context));

			return new CommandCompletionContext(context.Executor, context.Expression, context.Parameter, result, exception);
		}
		#endregion
	}
}

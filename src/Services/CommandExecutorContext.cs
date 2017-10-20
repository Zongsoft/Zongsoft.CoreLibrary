/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2010-2016 Zongsoft Corporation <http://www.zongsoft.com>
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
using System.IO;

namespace Zongsoft.Services
{
	public class CommandExecutorContext
	{
		#region 成员字段
		private ICommandExecutor _executor;
		private CommandExpression _expression;
		private object _parameter;
		#endregion

		#region 构造函数
		public CommandExecutorContext(ICommandExecutor executor, CommandExpression expression, object parameter)
		{
			if(executor == null)
				throw new ArgumentNullException(nameof(executor));

			if(expression == null)
				throw new ArgumentNullException(nameof(expression));

			_executor = executor;
			_expression = expression;
			_parameter = parameter;
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取当前命令执行器对象。
		/// </summary>
		public ICommandExecutor Executor
		{
			get
			{
				return _executor;
			}
		}

		/// <summary>
		/// 获取当前命令执行器的命令表达式。
		/// </summary>
		public CommandExpression Expression
		{
			get
			{
				return _expression;
			}
		}

		/// <summary>
		/// 获取从命令执行器传入的参数值。
		/// </summary>
		public object Parameter
		{
			get
			{
				return _parameter;
			}
		}

		/// <summary>
		/// 获取当前命令执行器的标准输出器。
		/// </summary>
		public ICommandOutlet Output
		{
			get
			{
				return _executor.Output;
			}
		}

		/// <summary>
		/// 获取当前命令执行器的错误输出器。
		/// </summary>
		public TextWriter Error
		{
			get
			{
				return _executor.Error;
			}
		}
		#endregion
	}
}

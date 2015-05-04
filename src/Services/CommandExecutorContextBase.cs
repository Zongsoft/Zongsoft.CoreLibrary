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

namespace Zongsoft.Services
{
	public class CommandExecutorContextBase : MarshalByRefObject
	{
		#region 成员字段
		private ICommandExecutor _executor;
		private string _commandText;
		private CommandTreeNode _commandNode;
		private ICommand _command;
		private object _parameter;
		private object _result;
		#endregion

		#region 构造函数
		protected CommandExecutorContextBase(ICommandExecutor executor, string commandText, CommandTreeNode commandNode, object parameter)
		{
			if(executor == null)
				throw new ArgumentNullException("executor");

			_executor = executor;
			_commandText = commandText;
			_commandNode = commandNode;
			_command = commandNode == null ? null : commandNode.Command;
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
		/// 获取当前执行的命令文本。
		/// </summary>
		public virtual string CommandText
		{
			get
			{
				return _commandText;
			}
		}

		/// <summary>
		/// 获取当前执行命令所在的命令树节点。
		/// </summary>
		public CommandTreeNode CommandNode
		{
			get
			{
				return _commandNode;
			}
		}

		/// <summary>
		/// 获取当前执行命令。
		/// </summary>
		public ICommand Command
		{
			get
			{
				return _command;
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
		/// 获取或设置命令执行器的执行结果。
		/// </summary>
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
		#endregion
	}
}

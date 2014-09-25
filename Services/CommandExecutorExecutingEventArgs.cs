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

namespace Zongsoft.Services
{
	[Serializable]
	public class CommandExecutorExecutingEventArgs : EventArgs
	{
		#region 成员字段
		private CommandExecutorBase _commandExecutor;
		private string _commandText;
		private object _parameter;
		private CommandTreeNode _commandNode;
		private ICommand _command;
		private object _result;
		private bool _cancel;
		#endregion

		#region 构造函数
		public CommandExecutorExecutingEventArgs(CommandExecutorBase commandExecutor, string commandText, object parameter, CommandTreeNode commandNode)
		{
			if(commandExecutor == null)
				throw new ArgumentNullException("commandExecutor");

			_commandExecutor = commandExecutor;
			_commandText = commandText;
			_parameter = parameter;
			_commandNode = commandNode;
			_command = commandNode == null ? null : commandNode.Command;
			_cancel = commandNode == null;
		}
		#endregion

		#region 公共属性
		public CommandExecutorBase CommandExecutor
		{
			get
			{
				return _commandExecutor;
			}
		}

		public string CommandText
		{
			get
			{
				return _commandText;
			}
		}

		public object Parameter
		{
			get
			{
				return _parameter;
			}
		}

		public CommandTreeNode CommandNode
		{
			get
			{
				return _commandNode;
			}
			set
			{
				_commandNode = value;
			}
		}

		public ICommand Command
		{
			get
			{
				return _command;
			}
			set
			{
				_command = value;
			}
		}

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

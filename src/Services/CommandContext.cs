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
using System.Collections.Generic;

namespace Zongsoft.Services
{
	/// <summary>
	/// 表示命令执行的上下文类。
	/// </summary>
	public class CommandContext
	{
		#region 成员字段
		private ICommand _command;
		private CommandTreeNode _commandNode;
		private CommandExpression _expression;
		private CommandExecutorContext _session;
		private object _parameter;
		private IDictionary<string, object> _states;
		#endregion

		#region 构造函数
		public CommandContext(CommandExecutorContext session, CommandExpression expression, ICommand command, object parameter, IDictionary<string, object> extendedProperties = null)
		{
			if(command == null)
				throw new ArgumentNullException("command");

			_session = session;
			_command = command;
			_parameter = parameter;
			_expression = expression;

			if(extendedProperties != null && extendedProperties.Count > 0)
				_states = new Dictionary<string, object>(extendedProperties, StringComparer.OrdinalIgnoreCase);
		}

		public CommandContext(CommandExecutorContext session, CommandExpression expression, CommandTreeNode commandNode, object parameter, IDictionary<string, object> extendedProperties = null)
		{
			if(commandNode == null)
				throw new ArgumentNullException("commandNode");

			if(commandNode.Command == null)
				throw new ArgumentException(string.Format("The Command property of '{0}' command-node is null.", commandNode.FullPath));

			_session = session;
			_commandNode = commandNode;
			_command = commandNode.Command;
			_parameter = parameter;
			_expression = expression;

			if(extendedProperties != null && extendedProperties.Count > 0)
				_states = new Dictionary<string, object>(extendedProperties, StringComparer.OrdinalIgnoreCase);
		}

		protected CommandContext(CommandContext context)
		{
			if(context == null)
				throw new ArgumentNullException(nameof(context));

			_session = context._session;
			_expression = context._expression;
			_command = context._command;
			_commandNode = context._commandNode;
			_parameter = context._parameter;
			_states = context._states;
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取执行的命令对象。
		/// </summary>
		public ICommand Command
		{
			get
			{
				return _command;
			}
		}

		/// <summary>
		/// 获取执行的命令所在节点。
		/// </summary>
		public CommandTreeNode CommandNode
		{
			get
			{
				return _commandNode;
			}
		}

		/// <summary>
		/// 获取命令执行的传入参数。
		/// </summary>
		public object Parameter
		{
			get
			{
				return _parameter;
			}
		}

		/// <summary>
		/// 获取当前命令对应的表达式。
		/// </summary>
		public CommandExpression Expression
		{
			get
			{
				return _expression;
			}
		}

		/// <summary>
		/// 获取一个值，指示当前上下文是否包含状态字典。
		/// </summary>
		/// <remarks>
		///		<para>在不确定状态字典是否含有内容之前，建议先使用该属性来检测。</para>
		/// </remarks>
		public bool HasStates
		{
			get
			{
				return _states != null && _states.Count > 0;
			}
		}

		/// <summary>
		/// 获取当前上下文的状态字典。
		/// </summary>
		public IDictionary<string, object> States
		{
			get
			{
				if(_states == null)
					System.Threading.Interlocked.CompareExchange(ref _states, new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase), null);

				return _states;
			}
		}

		/// <summary>
		/// 获取当前命令的标准输出器。
		/// </summary>
		public virtual ICommandOutlet Output
		{
			get
			{
				return _session?.Executor?.Output;
			}
		}

		/// <summary>
		/// 获取当前命令的错误输出器。
		/// </summary>
		public virtual TextWriter Error
		{
			get
			{
				return _session?.Executor?.Error;
			}
		}

		/// <summary>
		/// 获取命令所在的命令执行器。
		/// </summary>
		public ICommandExecutor Executor
		{
			get
			{
				return _session?.Executor;
			}
		}

		/// <summary>
		/// 获取当前执行命令的会话，即命令管道执行上下文。
		/// </summary>
		public CommandExecutorContext Session
		{
			get
			{
				return _session;
			}
		}
		#endregion
	}
}

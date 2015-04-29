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
	public class CommandContextBase : MarshalByRefObject
	{
		#region 成员字段
		private ICommand _command;
		private CommandTreeNode _commandNode;
		private ICommandExecutor _executor;
		private IDictionary<ICommandExecutor, Dictionary<string, object>> _statesProvider;
		private object _result;
		#endregion

		#region 构造函数
		protected CommandContextBase(ICommand command) : this(command, null)
		{
		}

		protected CommandContextBase(ICommand command, ICommandExecutor executor)
		{
			if(command == null)
				throw new ArgumentNullException("command");

			_command = command;
			_executor = executor;
		}

		protected CommandContextBase(CommandTreeNode commandNode) : this(commandNode, null)
		{
		}

		protected CommandContextBase(CommandTreeNode commandNode, ICommandExecutor executor)
		{
			if(commandNode == null)
				throw new ArgumentNullException("commandNode");

			if(commandNode.Command == null)
				throw new ArgumentException(string.Format("The Command property of '{0}' command-node is null.", commandNode.FullPath));

			_commandNode = commandNode;
			_command = commandNode.Command;
			_executor = executor;
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
		/// 获取执行命令所在的命令执行器。
		/// </summary>
		public ICommandExecutor Executor
		{
			get
			{
				return _executor;
			}
		}

		/// <summary>
		/// 获取一个由当前命令执行器为宿主的字典容器。
		/// </summary>
		/// <remarks>
		///		<para>在本属性返回的字典集合中的内容对于相同<see cref="ICommandExecutor"/>中的命令而言都是可见(读写)的，但对于不同<seealso cref="ICommandExecutor"/>下的命令而言，这些字典集合内的内容则是不可见的。</para>
		/// </remarks>
		public IDictionary<string, object> States
		{
			get
			{
				if(_statesProvider == null)
					System.Threading.Interlocked.CompareExchange(ref _statesProvider, new Dictionary<ICommandExecutor, Dictionary<string, object>>(), null);

				Dictionary<string, object> states;
				if(_statesProvider.TryGetValue(_executor, out states))
					return states;

				lock(_statesProvider)
				{
					if(_statesProvider.TryGetValue(_executor, out states))
						return states;

					states = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
					_statesProvider.Add(_executor, states);
				}

				return states;
			}
		}

		/// <summary>
		/// 获取或设置命令的执行结果。
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

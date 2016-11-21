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
using System.Collections.Concurrent;

namespace Zongsoft.Services
{
	public class CommandContext : MarshalByRefObject
	{
		#region 静态字段
		private static ConcurrentDictionary<string, object> _states;
		private static ConcurrentDictionary<ICommandExecutor, ConcurrentDictionary<string, object>> _statesProvider;
		#endregion

		#region 成员字段
		private ICommand _command;
		private CommandTreeNode _commandNode;
		private CommandExpression _expression;
		private ICommandExecutor _executor;
		private object _parameter;
		private CommandOptionCollection _options;
		private IDictionary<string, object> _extendedProperties;
		#endregion

		#region 构造函数
		public CommandContext(ICommandExecutor executor, CommandExpression expression, ICommand command, object parameter, IDictionary<string, object> extendedProperties = null)
		{
			if(command == null)
				throw new ArgumentNullException("command");

			_executor = executor;
			_command = command;
			_parameter = parameter;
			_expression = expression;

			if(expression == null)
				_options = new CommandOptionCollection(command);
			else
				_options = new CommandOptionCollection(command, (System.Collections.IDictionary)expression.Options);

			if(extendedProperties != null && extendedProperties.Count > 0)
				_extendedProperties = new Dictionary<string, object>(extendedProperties, StringComparer.OrdinalIgnoreCase);
		}

		public CommandContext(ICommandExecutor executor, CommandExpression expression, CommandTreeNode commandNode, object parameter, IDictionary<string, object> extendedProperties = null)
		{
			if(commandNode == null)
				throw new ArgumentNullException("commandNode");

			if(commandNode.Command == null)
				throw new ArgumentException(string.Format("The Command property of '{0}' command-node is null.", commandNode.FullPath));

			_executor = executor;
			_commandNode = commandNode;
			_command = commandNode.Command;
			_parameter = parameter;
			_expression = expression;

			if(expression == null)
				_options = new CommandOptionCollection(commandNode.Command);
			else
				_options = new CommandOptionCollection(commandNode.Command, (System.Collections.IDictionary)expression.Options);

			if(extendedProperties != null && extendedProperties.Count > 0)
				_extendedProperties = new Dictionary<string, object>(extendedProperties, StringComparer.OrdinalIgnoreCase);
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
		/// 获取当前命令对应的表达式选项集。
		/// </summary>
		public CommandOptionCollection Options
		{
			get
			{
				return _options;
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
		/// 获取扩展属性集是否有内容。
		/// </summary>
		/// <remarks>
		///		<para>在不确定扩展属性集是否含有内容之前，建议先使用该属性来检测。</para>
		/// </remarks>
		public bool HasExtendedProperties
		{
			get
			{
				return _extendedProperties != null && _extendedProperties.Count > 0;
			}
		}

		/// <summary>
		/// 获取可用于在本次执行过程中在各处理模块之间组织和共享数据的键/值集合。
		/// </summary>
		public IDictionary<string, object> ExtendedProperties
		{
			get
			{
				if(_extendedProperties == null)
					System.Threading.Interlocked.CompareExchange(ref _extendedProperties, new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase), null);

				return _extendedProperties;
			}
		}

		/// <summary>
		/// 获取当前命令的标准输出器。
		/// </summary>
		public virtual ICommandOutlet Output
		{
			get
			{
				return _executor.Output;
			}
		}

		/// <summary>
		/// 获取当前命令的错误输出器。
		/// </summary>
		public virtual TextWriter Error
		{
			get
			{
				return _executor.Error;
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
		public ConcurrentDictionary<string, object> States
		{
			get
			{
				if(_executor == null)
				{
					if(_states == null)
						System.Threading.Interlocked.CompareExchange(ref _states, new ConcurrentDictionary<string, object>(StringComparer.OrdinalIgnoreCase), null);

					return _states;
				}

				if(_statesProvider == null)
					System.Threading.Interlocked.CompareExchange(ref _statesProvider, new ConcurrentDictionary<ICommandExecutor, ConcurrentDictionary<string, object>>(), null);

				return _statesProvider.GetOrAdd(_executor, _ => new ConcurrentDictionary<string, object>(StringComparer.OrdinalIgnoreCase));
			}
		}
		#endregion
	}
}

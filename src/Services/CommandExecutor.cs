/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2011-2013 Zongsoft Corporation <http://www.zongsoft.com>
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
using System.Linq;
using System.Collections.Generic;

using Zongsoft.Diagnostics;
using Zongsoft.Resources;
using Zongsoft.Services;

namespace Zongsoft.Services
{
	public class CommandExecutor : Zongsoft.Services.CommandExecutorBase<CommandExecutorContext>
	{
		#region 静态字段
		private static CommandExecutor _default;
		#endregion

		#region 静态属性
		/// <summary>
		/// 获取或设置默认的<see cref="CommandExecutor"/>命令执行器。
		/// </summary>
		/// <remarks>
		///		<para>注意：如果已经设置过该属性，则不允许再更改其值。</para>
		/// </remarks>
		public static CommandExecutor Default
		{
			get
			{
				return _default;
			}
			set
			{
				if(value == null)
					throw new ArgumentNullException();

				if(_default == null)
					System.Threading.Interlocked.CompareExchange(ref _default, value, null);
			}
		}
		#endregion

		#region 成员变量
		private ICommandLineParser _parser;
		#endregion

		#region 构造函数
		public CommandExecutor()
		{
		}
		#endregion

		#region 公共属性
		public ICommandLineParser Parser
		{
			get
			{
				return _parser ?? CommandLine.CommandLineParser.Instance;
			}
			set
			{
				_parser = value;
			}
		}
		#endregion

		#region 重写方法
		protected override void OnExecute(CommandExecutorContext context)
		{
			var command = context.Command;

			if(command != null)
				context.Result = command.Execute(new CommandContext(this, context.CommandLine, context.CommandNode, context.Parameter));
		}

		protected override CommandExecutorContext CreateContext(string commandText, object parameter)
		{
			//解析当前命令文本
			var commandLine = this.OnParse(commandText);

			if(commandLine == null)
				throw new ArgumentException(string.Format("Invalid command-line text format of <{0}>.", commandText));

			//查找指定路径的命令对象
			var commandNode = this.Find(commandLine.FullPath);

			//如果指定的路径在命令树中是不存在的则抛出异常
			if(commandNode == null)
				throw new CommandNotFoundException(commandText);

			return new CommandExecutorContext(this, commandLine, commandNode, parameter);
		}
		#endregion

		#region 虚拟方法
		protected virtual CommandLine OnParse(string commandText)
		{
			var parser = this.Parser;

			return parser == null ? null : parser.Parse(commandText);
		}
		#endregion
	}
}

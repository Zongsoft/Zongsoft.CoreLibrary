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
	public class CommandExecutor : Zongsoft.Services.CommandExecutorBase
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

		#region 执行方法
		public override object Execute(string commandText, object parameter)
		{
			if(string.IsNullOrWhiteSpace(commandText))
				return null;

			//解析当前命令文本
			var commandLine = this.OnParse(commandText);

			if(commandLine == null)
				throw new ArgumentException(string.Format("Invalid command-line text format of <{0}>.", commandText));

			return base.Execute(commandLine.FullPath, commandLine);
		}
		#endregion

		#region 虚拟方法
		protected virtual CommandLine OnParse(string commandText)
		{
			var parser = this.Parser;

			return parser == null ? null : parser.Parse(commandText);
		}
		#endregion

		#region 重写方法
		protected override object OnExecute(CommandExecutorContext context)
		{
			return context.Command.Execute(new CommandContext(context.Command, this, (CommandLine)context.Parameter));
		}
		#endregion
	}
}

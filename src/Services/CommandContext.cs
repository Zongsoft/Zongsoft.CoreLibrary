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
using System.Collections.Generic;

using Zongsoft.Services;

namespace Zongsoft.Services
{
	public class CommandContext : Zongsoft.Services.CommandContextBase
	{
		#region 成员字段
		private CommandLine _commandLine;
		private CommandOptionCollection _options;
		#endregion

		#region 构造函数
		public CommandContext(ICommand command, ICommandExecutor executor, CommandLine commandLine) : base(command, executor)
		{
			_commandLine = commandLine;

			if(commandLine == null)
				_options = new CommandOptionCollection(command);
			else
				_options = new CommandOptionCollection(command, (System.Collections.IDictionary)commandLine.Options);
		}
		#endregion

		#region 公共属性
		public CommandLine CommandLine
		{
			get
			{
				return _commandLine;
			}
		}

		public CommandOptionCollection Options
		{
			get
			{
				return _options;
			}
		}

		public string[] Arguments
		{
			get
			{
				if(_commandLine == null)
					return new string[0];
				else
					return _commandLine.Arguments;
			}
		}
		#endregion
	}
}

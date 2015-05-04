/*
 * Authors:
 *   邓祥云(X.Z. Deng) <627825056@qq.com>
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
using System.Linq;
using System.Text;

namespace Zongsoft.Communication.Net.Ftp
{
	/// <summary>
	/// 返回帮助信息，打印出当前支持的所有命令
	/// </summary>
	internal class FtpHelpCommand : FtpCommand
	{
		public FtpHelpCommand() : base("HELP")
		{
		}

		protected override void OnExecute(FtpCommandContext context)
		{
			if(string.IsNullOrEmpty(context.Statement.Argument))
			{
				var cmd = context.Statement.Argument;
				if(context.Executor.Root.Children.Contains(cmd))
					context.Channel.Send(string.Format("214 Command {0} is supported by Ftp Server", cmd.ToUpper()));
				else
					context.Channel.Send(string.Format("502 Command {0} is not recognized or supported by Ftp Server",
													   cmd.ToUpper()));
			}
			else
			{
				var cmds = context.Executor.Root.Children.Keys.ToArray();

				var str = new StringBuilder();
				str.Append("214-The following commands are recognized:");

				for(int i = 0; i < cmds.Length; i++)
				{
					if(i % 8 == 0)
						str.Append("\r\n");

					str.AppendFormat("    {0}", cmds[i]);
				}

				context.Channel.Send(str.ToString());
			}
		}
	}
}
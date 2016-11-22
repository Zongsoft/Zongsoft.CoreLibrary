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
using System.ComponentModel;
using System.Diagnostics;
using System.Collections.Generic;

namespace Zongsoft.Terminals.Commands
{
	[DisplayName("${Text.ShellCommand.Title}")]
	[Description("${Text.ShellCommand.Description}")]
	[Zongsoft.Services.CommandOption("timeout", Type = typeof(int), DefaultValue = 1000, Description = "${Text.ShellCommand.Options.Timeout}")]
	public class ShellCommand : Zongsoft.Services.CommandBase<TerminalCommandContext>
	{
		#region 构造函数
		public ShellCommand() : base("Shell")
		{
		}

		public ShellCommand(string name) : base(name)
		{
		}
		#endregion

		#region 重写方法
		protected override object OnExecute(TerminalCommandContext context)
		{
			if(Environment.OSVersion.Platform == PlatformID.MacOSX ||
			   Environment.OSVersion.Platform == PlatformID.Unix)
				throw new NotSupportedException(string.Format("Not supported in the {0} OS.", Environment.OSVersion));

			if(context.Expression.Arguments.Length < 1)
				return 0;

			ProcessStartInfo info = new ProcessStartInfo(@"cmd.exe", " /C " + context.Expression.Arguments[0])
			{
				CreateNoWindow = true,
				UseShellExecute = false,
				RedirectStandardError = true,
				RedirectStandardInput = true,
				RedirectStandardOutput = true,
			};

			using(var process = Process.Start(info))
			{
				process.OutputDataReceived += delegate(object sender, DataReceivedEventArgs eventArgs)
				{
					context.Terminal.WriteLine(eventArgs.Data);
				};

				process.BeginOutputReadLine();

				//while(!process.StandardOutput.EndOfStream)
				//{
				//	context.Terminal.WriteLine(process.StandardOutput.ReadLine());
				//}

				//context.Terminal.Write(process.StandardOutput.ReadToEnd());

				//process.WaitForExit();

				if(!process.HasExited)
				{
					var timeout = context.Expression.Options.GetValue<int>("timeout");

					if(!process.WaitForExit(timeout > 0 ? timeout : int.MaxValue))
					{
						process.Close();
						return -1;
					}
				}

				return process.ExitCode;
			}
		}
		#endregion
	}
}

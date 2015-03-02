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

using Zongsoft.Services;
using Zongsoft.Resources;

namespace Zongsoft.Terminals.Commands
{
	[DisplayName("${Text.HelpCommand.Title}")]
	[Description("${Text.HelpCommand.Description}")]
	public class HelpCommand : CommandBase<TerminalCommandContext>
	{
		#region 构造函数
		public HelpCommand() : base("Help")
		{
		}

		public HelpCommand(string name) : base(name)
		{
		}
		#endregion

		#region 重写方法
		protected override void Run(TerminalCommandContext context)
		{
			if(context.Arguments.Length < 1)
			{
				foreach(var node in context.Executor.Root.Children)
				{
					this.DisplayCommandNode(context, node, 0);
				}

				return;
			}

			foreach(var argument in context.Arguments)
			{
				if(argument == "?")
				{
					CommandHelper.DisplayCommandInfo(context.Terminal, this);
					continue;
				}

				CommandTreeNode node = context.Executor.Find(argument);

				if(node == null)
				{
					context.Terminal.WriteLine(TerminalColor.Red, ResourceUtility.GetString("CommandNotFound", argument));
					continue;
				}

				if(node != null && node.Command != null)
				{
					context.Terminal.WriteLine(node.FullPath);
					CommandHelper.DisplayCommandInfo(context.Terminal, node.Command);
				}
			}
		}
		#endregion

		#region 私有方法
		private void DisplayCommandNode(TerminalCommandContext context, CommandTreeNode node, int depth)
		{
			if(node == null)
				return;

			var indent = depth > 0 ? new string(' ', depth * 2) : string.Empty;

			if(node.Command == null)
				context.Terminal.WriteLine("{1}[{0}]", node.FullPath, indent);
			else
				context.Terminal.WriteLine("{1}{0}", node.FullPath, indent);

			if(node.Children.Count > 0)
			{
				foreach(var child in node.Children)
					this.DisplayCommandNode(context, child, depth + 1);
			}
		}
		#endregion
	}
}

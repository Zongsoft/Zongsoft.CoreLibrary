/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2018 Zongsoft Corporation <http://www.zongsoft.com>
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

namespace Zongsoft.Services.Commands
{
	public class WorkerInfoCommand : CommandBase<CommandContext>
	{
		#region 单例字段
		public static readonly WorkerInfoCommand Default = new WorkerInfoCommand();
		#endregion

		#region 构造函数
		public WorkerInfoCommand() : base("Info")
		{
		}

		public WorkerInfoCommand(string name) : base(name)
		{
		}
		#endregion

		#region 执行方法
		protected override object OnExecute(CommandContext context)
		{
			//向上查找工作者命令对象，如果找到则获取其对应的工作者对象
			var worker = context.CommandNode.Find<WorkerCommandBase>(true)?.Worker;

			//如果指定的工作器查找失败，则抛出异常
			if(worker == null)
				throw new CommandException("Missing required worker of depends on.");

			//处理工作者信息
			this.Info(context, worker);

			//返回执行成功的工作者
			return worker;
		}
		#endregion

		#region 虚拟方法
		protected virtual void Info(CommandContext context, IWorker worker)
		{
			context.Output.WriteLine(GetInfo(worker));
		}
		#endregion

		#region 内部方法
		internal static CommandOutletContent GetInfo(IWorker worker)
		{
			//构建状态内容部分
			var content = CommandOutletContent.Create(WorkerCommandBase.GetStateColor(worker.State), $"[{worker.State}]");

			//构建可用内容部分
			if(!worker.Enabled)
			{
				content.Append(CommandOutletColor.Gray, "(");
				content.Append(CommandOutletColor.Red, Properties.Resources.Disabled);
				content.Append(CommandOutletColor.Gray, ")");
			}

			//构建名称内容部分
			return content.Append(" " + worker.Name);
		}
		#endregion
	}
}

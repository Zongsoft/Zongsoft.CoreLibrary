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
using System.Threading;

namespace Zongsoft.Services.Commands
{
	[CommandOption(KEY_TIMEOUT_OPTION, typeof(TimeSpan), DefaultValue = "5s", Description = "${Text.Command.Options.Timeout}")]
	public class WorkerPauseCommand : CommandBase<CommandContext>
	{
		#region 单例字段
		public static readonly WorkerPauseCommand Default = new WorkerPauseCommand();
		#endregion

		#region 常量定义
		private const string KEY_TIMEOUT_OPTION = "timeout";
		#endregion

		#region 构造函数
		public WorkerPauseCommand() : base("Pause")
		{
		}

		public WorkerPauseCommand(string name) : base(name)
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

			//如果当前工作器状态不是运行中，则忽略该请求
			if(worker.State != WorkerState.Running)
				return worker;

			//暂停工作者
			worker.Pause();

			//调用暂停完成方法
			this.OnPaused(context, worker, context.Expression.Options.GetValue<TimeSpan>(KEY_TIMEOUT_OPTION));

			//返回执行成功的工作者
			return worker;
		}
		#endregion

		#region 虚拟方法
		protected virtual void OnPaused(CommandContext context, IWorker worker, TimeSpan timeout)
		{
			if(timeout <= TimeSpan.Zero)
				timeout = TimeSpan.FromSeconds(5);
			else if(timeout.TotalSeconds > 30)
				timeout = TimeSpan.FromSeconds(30);

			switch(worker.State)
			{
				case WorkerState.Paused:
					this.OnSucceed(context.Output, worker);
					break;
				case WorkerState.Running:
					this.OnFailed(context.Output, worker);
					break;
				case WorkerState.Pausing:
					SpinWait.SpinUntil(() => worker.State == WorkerState.Paused, timeout);

					if(worker.State == WorkerState.Paused)
						this.OnSucceed(context.Output, worker);
					else
						this.OnFailed(context.Output, worker);

					break;
			}
		}
		#endregion

		#region 私有方法
		private void OnFailed(ICommandOutlet output, IWorker worker)
		{
			output.WriteLine(Utility.GetWorkerActionContent(worker, string.Format(Properties.Resources.Text_Command_ExecutionFailed_Message, Properties.Resources.Text_WorkerPauseCommand_Name), CommandOutletColor.DarkRed));
		}

		private void OnSucceed(ICommandOutlet output, IWorker worker)
		{
			output.WriteLine(Utility.GetWorkerActionContent(worker, string.Format(Properties.Resources.Text_Command_ExecutionSucceed_Message, Properties.Resources.Text_WorkerPauseCommand_Name), CommandOutletColor.DarkGreen));
		}
		#endregion
	}
}

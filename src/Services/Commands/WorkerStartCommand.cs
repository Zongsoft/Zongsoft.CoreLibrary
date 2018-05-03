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
	[CommandOption(KEY_FORCE_OPTION, Description = "${Text.WorkerStartCommand.Options.Force}")]
	[CommandOption(KEY_TIMEOUT_OPTION, typeof(TimeSpan), DefaultValue = "5s", Description = "${Text.Command.Options.Timeout}")]
	public class WorkerStartCommand : CommandBase<CommandContext>
	{
		#region 常量定义
		private const string KEY_FORCE_OPTION = "force";
		private const string KEY_TIMEOUT_OPTION = "timeout";
		#endregion

		#region 构造函数
		public WorkerStartCommand() : base("Start")
		{
		}

		public WorkerStartCommand(string name) : base(name)
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

			//获取是否开启了强制启动选项
			var force = context.Expression.Options.GetValue<bool>(KEY_FORCE_OPTION);

			//如果没有开启强制启动选项并且当前工作器不可用，则抛出异常
			if(!force && !worker.Enabled)
				throw new CommandException($"The '{worker.Name}' worker are disabled.");

			//启动工作者
			worker.Start(context.Expression.Arguments);

			//调用启动完成方法
			this.OnStarted(context, worker, context.Expression.Options.GetValue<TimeSpan>(KEY_TIMEOUT_OPTION));

			//返回执行成功的工作者
			return worker;
		}
		#endregion

		#region 虚拟方法
		protected virtual void OnStarted(CommandContext context, IWorker worker, TimeSpan timeout)
		{
			if(timeout <= TimeSpan.Zero)
				timeout = TimeSpan.FromSeconds(5);
			else if(timeout.TotalSeconds > 30)
				timeout = TimeSpan.FromSeconds(30);

			switch(worker.State)
			{
				case WorkerState.Running:
					this.OnSucceed(context.Output, worker);
					break;
				case WorkerState.Stopped:
				case WorkerState.Stopping:
					this.OnFailed(context.Output, worker);
					break;
				case WorkerState.Starting:
					SpinWait.SpinUntil(() => worker.State == WorkerState.Running, timeout);

					if(worker.State == WorkerState.Running)
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
			if(worker.State == WorkerState.Stopped)
				output.WriteLine(CommandOutletColor.DarkRed, $"The {worker.Name} worker was startup failed.");
			else
				output.WriteLine(CommandOutletColor.DarkRed, $"[{worker.State}] The {worker.Name} worker was startup failed.");
		}

		private void OnSucceed(ICommandOutlet output, IWorker worker)
		{
			output.WriteLine(CommandOutletColor.Green, $"The {worker.Name} worker was started successfully.");
		}
		#endregion
	}
}

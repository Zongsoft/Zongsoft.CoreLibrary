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
	public class WorkerListenCommand : CommandBase<CommandContext>
	{
		#region 常量定义
		#endregion

		#region 私有变量
		private CommandContext _context;
		private AutoResetEvent _semaphore;
		#endregion

		#region 构造函数
		public WorkerListenCommand() : base("Listen")
		{
			//创建信号量，默认为堵塞状态
			_semaphore = new AutoResetEvent(false);
		}

		public WorkerListenCommand(string name) : base(name)
		{
			//创建信号量，默认为堵塞状态
			_semaphore = new AutoResetEvent(false);
		}
		#endregion

		#region 保护属性
		protected CommandContext Context
		{
			get
			{
				return _context;
			}
		}
		#endregion

		#region 执行方法
		protected override object OnExecute(CommandContext context)
		{
			//获取当前命令执行器对应的终端
			var terminal = (context.Executor as Zongsoft.Terminals.TerminalCommandExecutor)?.Terminal;

			//如果当前命令执行器不是终端命令执行器则抛出不支持的异常
			if(terminal == null)
				throw new NotSupportedException("The listen command must be run in terminal executor.");

			//向上查找工作者命令对象，如果找到则获取其对应的工作者对象
			var worker = context.CommandNode.Find<WorkerCommandBase>(true)?.Worker;

			//如果指定的工作器查找失败，则抛出异常
			if(worker == null)
				throw new CommandException("Missing required worker of depends on.");

			//保持当前命令执行上下文
			_context = context;

			//挂载当前终端的中断事件
			terminal.Aborting += this.Terminal_Aborting;

			//挂载工作器的状态变更事件
			worker.StateChanged += this.Worker_StateChanged;

			//调用侦听开始方法
			this.OnListening(context, worker);

			//等待信号量
			_semaphore.WaitOne();

			//注销工作器的状态变更事件
			worker.StateChanged -= this.Worker_StateChanged;

			//注销当前终端的中断事件
			terminal.Aborting -= this.Terminal_Aborting;

			//调用侦听结束方法
			this.OnListened(context, worker);

			//将当前命令执行上下文置空
			_context = null;

			//返回执行成功的工作者
			return worker;
		}
		#endregion

		#region 虚拟方法
		protected virtual void OnListening(CommandContext context, IWorker worker)
		{
			context.Output.WriteLine($"Welcome to the {worker.Name} listening mode.");
			context.Output.WriteLine(CommandOutletColor.DarkYellow, "Press the Ctrl+C key to exit the listening." + Environment.NewLine);
		}

		protected virtual void OnListened(CommandContext context, IWorker worker)
		{
		}

		protected virtual void OnStateChanged(IWorker worker, WorkerStateChangedEventArgs args)
		{
			if(!worker.Enabled)
			{
				//打印禁用的信息
				_context.Output.WriteLine(CommandOutletColor.DarkGray, $"[{args.State}](Disabled) {worker.Name}");

				//退出
				return;
			}

			//默认运行中为绿色
			var color = CommandOutletColor.Green;

			switch(args.State)
			{
				case WorkerState.Pausing:
				case WorkerState.Paused:
					color = CommandOutletColor.DarkYellow;
					break;
				case WorkerState.Resuming:
				case WorkerState.Starting:
					color = CommandOutletColor.DarkGreen;
					break;
				case WorkerState.Stopped:
				case WorkerState.Stopping:
					color = CommandOutletColor.Gray;
					break;
			}

			_context.Output.WriteLine(color, $"[{args.State}] {worker.Name}");
		}
		#endregion

		#region 事件处理
		private void Worker_StateChanged(object sender, WorkerStateChangedEventArgs e)
		{
			this.OnStateChanged((IWorker)sender, e);
		}

		private void Terminal_Aborting(object sender, System.ComponentModel.CancelEventArgs e)
		{
			//阻止命令执行器被关闭
			e.Cancel = true;

			//释放信号量
			_semaphore.Set();
		}
		#endregion
	}
}

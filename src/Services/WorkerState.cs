/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2010-2013 Zongsoft Corporation <http://www.zongsoft.com>
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

namespace Zongsoft.Services
{
	/// <summary>
	/// 关于<seealso cref="IWorker"/>的状态信息。
	/// </summary>
	public enum WorkerState
	{
		/// <summary>未运行/已停止。</summary>
		[Description("${Text.WorkerState.Stopped}")]
		Stopped = 0,
		/// <summary>运行中。</summary>
		[Description("${Text.WorkerState.Running}")]
		Running = 1,

		/// <summary>正在启动中。</summary>
		[Description("${Text.WorkerState.Starting}")]
		Starting,
		/// <summary>正在停止中。</summary>
		[Description("${Text.WorkerState.Stopping}")]
		Stopping,

		/// <summary>正在暂停中。</summary>
		[Description("${Text.WorkerState.Pausing}")]
		Pausing,

		/// <summary>暂停中。</summary>
		[Description("${Text.WorkerState.Paused}")]
		Paused,

		/// <summary>正在恢复中。</summary>
		[Description("${Text.WorkerState.Resuming}")]
		Resuming,
	}
}

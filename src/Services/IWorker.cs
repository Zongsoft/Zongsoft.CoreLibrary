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
using System.Collections.Generic;
using System.Text;

namespace Zongsoft.Services
{
	/// <summary>
	/// 关于工作器的接口。
	/// </summary>
	public interface IWorker
	{
		/// <summary>表示状态发生了改变。</summary>
		event EventHandler<WorkerStateChangedEventArgs> StateChanged;

		/// <summary>
		/// 获取当前工作器的名称。
		/// </summary>
		string Name
		{
			get;
		}

		/// <summary>
		/// 获取当前工作器的状态。
		/// </summary>
		WorkerState State
		{
			get;
		}

		/// <summary>
		/// 获取或设置是否禁用工作器。
		/// </summary>
		bool Disabled
		{
			get;
			set;
		}

		/// <summary>
		/// 获取工作器是否允许暂停和继续。
		/// </summary>
		bool CanPauseAndContinue
		{
			get;
		}

		/// <summary>
		/// 启动工作器。
		/// </summary>
		/// <param name="args">启动的参数。</param>
		void Start(params string[] args);

		/// <summary>
		/// 停止工作器。
		/// </summary>
		/// <param name="args">停止的参数。</param>
		void Stop(params string[] args);

		/// <summary>
		/// 暂停工作器。
		/// </summary>
		void Pause();

		/// <summary>
		/// 恢复工作器，继续运行。
		/// </summary>
		void Resume();
	}
}

/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2010-2016 Zongsoft Corporation <http://www.zongsoft.com>
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
using System.IO;

namespace Zongsoft.Services
{
	/// <summary>
	/// 表示命令执行器的接口。
	/// </summary>
	public interface ICommandExecutor
	{
		#region 声明事件
		event EventHandler<CommandExecutorFailureEventArgs> Failed;
		event EventHandler<CommandExecutorExecutingEventArgs> Executing;
		event EventHandler<CommandExecutorExecutedEventArgs> Executed;
		#endregion

		#region 属性定义
		/// <summary>
		/// 获取命令执行器的根节点。
		/// </summary>
		CommandTreeNode Root
		{
			get;
		}

		/// <summary>
		/// 获取或设置命令表达式解析器。
		/// </summary>
		ICommandExpressionParser Parser
		{
			get;
			set;
		}

		/// <summary>
		/// 获取或设置命令执行器的标准输出器。
		/// </summary>
		ICommandOutlet Output
		{
			get;
			set;
		}

		/// <summary>
		/// 获取或设置命令执行器的错误输出器。
		/// </summary>
		TextWriter Error
		{
			get;
			set;
		}
		#endregion

		/// <summary>
		/// 执行命令。
		/// </summary>
		/// <param name="commandText">指定要执行的命令表达式文本。</param>
		/// <param name="parameter">指定的输入参数。</param>
		/// <returns>返回命令执行的结果。</returns>
		object Execute(string commandText, object parameter = null);

		/// <summary>
		/// 查找指定命令路径对应的命令节点。
		/// </summary>
		/// <param name="commandPath">指定的命令路径。</param>
		/// <returns>返回指定命令路径对应的命令节点，如果指定的路径不存在则返回空(null)。</returns>
		CommandTreeNode Find(string commandPath);
	}
}

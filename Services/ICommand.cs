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
using System.Collections.Generic;

namespace Zongsoft.Services
{
	/// <summary>
	/// 扩展命令接口。
	/// </summary>
	public interface ICommand
	{
		/// <summary>在 <seealso cref="Enabled"/> 属性发生改变之后发生。</summary>
		event EventHandler EnabledChanged;
		/// <summary>在 <seealso cref="Execute"/> 方法执行之前发生。可以通过事件参数取消后续的执行。</summary>
		event EventHandler<CommandExecutedEventArgs> Executed;
		/// <summary>在 <seealso cref="Execute"/> 方法执行之后发生。</summary>
		event EventHandler<CommandExecutingEventArgs> Executing;

		/// <summary>
		/// 获取命令的名称。
		/// </summary>
		string Name
		{
			get;
		}

		/// <summary>
		/// 获取或设置一个值，该值指示命令是否可以执行。
		/// </summary>
		bool Enabled
		{
			get;
			set;
		}

		/// <summary>
		/// 判断当前命令能否依据给定的选项和参数执行。
		/// </summary>
		/// <param name="parameter">判断命令能否执行的参数对象。</param>
		/// <returns>返回能否执行的结果。</returns>
		bool CanExecute(object parameter);

		/// <summary>
		/// 执行命令。
		/// </summary>
		/// <param name="parameter">执行命令的参数对象。</param>
		/// <returns>返回执行的返回结果。</returns>
		/// <remarks>
		///		<para>对实现着的要求：应该在本方法的实现中首先调用<see cref="CanExecute"/>方法，以确保阻止非法的调用。</para>
		/// </remarks>
		object Execute(object parameter);
	}
}

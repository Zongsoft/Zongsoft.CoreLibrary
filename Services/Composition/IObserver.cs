/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2012-2013 Zongsoft Corporation <http://www.zongsoft.com>
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

namespace Zongsoft.Services.Composition
{
	/// <summary>
	/// 提供用于接收基于推送的通知的机制的观察者。
	/// </summary>
	public interface IObserver
	{
		/// <summary>
		/// 初始化当前观察者。
		/// </summary>
		/// <param name="observable">被注册到的观察容器，即被消息提供者。</param>
		/// <remarks>
		///		<para>该方法由观察容器进行调用，通常当观察者被注册到容器中时被调用。</para>
		/// </remarks>
		void Initialize(IObservable observable);

		/// <summary>
		/// 向观察者提供新的信息。
		/// </summary>
		/// <param name="value">当前的通知信息。</param>
		void OnNotified(object value);

		/// <summary>
		/// 通知观察者，提供程序已完成发送基于推送的通知。
		/// </summary>
		/// <param name="value">当前的通知信息。</param>
		void OnCompleted(object value);
	}
}

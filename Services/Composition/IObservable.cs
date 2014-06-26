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
	/// 提供基于推送机制的通知提供者。
	/// </summary>
	public interface IObservable
	{
		/// <summary>
		/// 向当前通知提供者订阅，表示某观察器将要接收通知。
		/// </summary>
		/// <param name="observer">要接收通知的观察者对象。</param>
		void Subscribe(IObserver observer);

		/// <summary>
		/// 向当前通知提供者退订，表示某观察器取消接收通知。
		/// </summary>
		/// <param name="observer">要取消接收通知的观察者对象。</param>
		void Unsubscribe(IObservable observer);

		/// <summary>
		/// 通知所有观察者，即向已注册的所有观察者发送通知。
		/// </summary>
		/// <param name="value">发送的信息。</param>
		void Notify(object value);

		/// <summary>
		/// 通知观察者，提供程序已完成发送基于推送的通知。
		/// </summary>
		/// <param name="value">发送的信息。</param>
		void Complete(object value);
	}
}

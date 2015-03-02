/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2011-2013 Zongsoft Corporation <http://www.zongsoft.com>
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
using System.Net;

namespace Zongsoft.Communication
{
	/// <summary>
	/// 提供关于通讯侦听的功能的接口。
	/// </summary>
	public interface IListener : IDisposable
	{
		/// <summary>
		/// 获取当前是否处于侦听状态。
		/// </summary>
		bool IsListening
		{
			get;
		}

		/// <summary>
		/// 获取通讯侦听器的信息接收处理器对象。
		/// </summary>
		IReceiver Receiver
		{
			get;
		}

		/// <summary>
		/// 获取通讯侦听的地址。
		/// </summary>
		IPEndPoint Address
		{
			get;
		}

		/// <summary>
		/// 开启侦听。
		/// </summary>
		void Start(params string[] args);

		/// <summary>
		/// 停止侦听。
		/// </summary>
		void Stop(params string[] args);
	}
}

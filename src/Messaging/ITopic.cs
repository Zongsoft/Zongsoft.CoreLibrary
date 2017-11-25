/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2015-2017 Zongsoft Corporation <http://www.zongsoft.com>
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

namespace Zongsoft.Messaging
{
	/// <summary>
	/// 表示主题服务的接口，该接口提供主题操作相关的功能。
	/// </summary>
	public interface ITopic : Zongsoft.Communication.ISender
	{
		/// <summary>
		/// 获取主题名称。
		/// </summary>
		string Name
		{
			get;
		}

		/// <summary>
		/// 获取主题订阅服务。
		/// </summary>
		/// <returns></returns>
		ITopicSubscription GetSubscription();

		/// <summary>
		/// 发送消息操作。
		/// </summary>
		/// <param name="data">指定要发送的数据。</param>
		/// <param name="tags">指定发送消息关联的标签，主题的订阅者可能会根据消息标签做过滤。</param>
		/// <param name="state">指定的自定义状态对象。</param>
		void Send(byte[] data, string tags, object state = null);
	}
}

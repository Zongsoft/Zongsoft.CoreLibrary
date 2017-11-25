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
	/// 提供主题订阅相关功能的接口。
	/// </summary>
	public interface ITopicSubscription
	{
		/// <summary>
		/// 获取指定订阅名称的信息。
		/// </summary>
		/// <param name="name">指定要获取的订阅名称。</param>
		/// <returns>返回指定名称的订阅信息，如果指定名称的订阅不存在则返回空(null)。</returns>
		TopicSubscription Get(string name);

		/// <summary>
		/// 创建一个主题订阅。
		/// </summary>
		/// <param name="name">指定要创建的订阅名称，必须确保在相同主题中订阅名不能同名。</param>
		/// <param name="url">指定要创建的订阅回调URL，有关URL的具体定义参考不同的实现。</param>
		/// <param name="state">指定的自定义状态对象。</param>
		/// <returns>返回一个值，指示订阅创建是否成功。</returns>
		/// <remarks>对实现的约定：如果指定名称的主题订阅已经存在，应返回假(False)而不是抛出异常。</remarks>
		bool Subscribe(string name, string url, object state = null);

		/// <summary>
		/// 创建一个主题订阅。
		/// </summary>
		/// <param name="name">指定要创建的订阅名称，必须确保在相同主题中订阅名不能同名。</param>
		/// <param name="url">指定要创建的订阅回调URL，有关URL的具体定义参考不同的实现。</param>
		/// <param name="behavior">指定要创建的订阅回调失败的重试策略。</param>
		/// <param name="state">指定的自定义状态对象。</param>
		/// <returns>返回一个值，指示订阅创建是否成功。</returns>
		/// <remarks>对实现的约定：如果指定名称的主题订阅已经存在，应返回假(False)而不是抛出异常。</remarks>
		bool Subscribe(string name, string url, TopicSubscriptionFallbackBehavior behavior, object state = null);

		/// <summary>
		/// 创建一个主题订阅。
		/// </summary>
		/// <param name="name">指定要创建的订阅名称，必须确保在相同主题中订阅名不能同名。</param>
		/// <param name="url">指定要创建的订阅回调URL，有关URL的具体定义参考不同的实现。</param>
		/// <param name="tags">指定要创建的订阅的过滤标签。</param>
		/// <param name="state">指定的自定义状态对象。</param>
		/// <returns>返回一个值，指示订阅创建是否成功。</returns>
		/// <remarks>对实现的约定：如果指定名称的主题订阅已经存在，应返回假(False)而不是抛出异常。</remarks>
		bool Subscribe(string name, string url, string tags, object state = null);

		/// <summary>
		/// 创建一个主题订阅。
		/// </summary>
		/// <param name="name">指定要创建的订阅名称，必须确保在相同主题中订阅名不能同名。</param>
		/// <param name="url">指定要创建的订阅回调URL，有关URL的具体定义参考不同的实现。</param>
		/// <param name="tags">指定要创建的订阅的过滤标签。</param>
		/// <param name="behavior">指定要创建的订阅回调失败的重试策略。</param>
		/// <param name="state">指定的自定义状态对象。</param>
		/// <returns>返回一个值，指示订阅创建是否成功。</returns>
		/// <remarks>对实现的约定：如果指定名称的主题订阅已经存在，应返回假(False)而不是抛出异常。</remarks>
		bool Subscribe(string name, string url, string tags, TopicSubscriptionFallbackBehavior behavior, object state = null);

		/// <summary>
		/// 取消指定名称的主题订阅。
		/// </summary>
		/// <param name="name">指定要删除的订阅（通道）名称。</param>
		/// <returns>返回一个值，指示删除操作是否成功。</returns>
		/// <remarks>对实现的约定：如果删除操作失败，应返回假(False)，并尽量避免抛出异常。</remarks>
		bool Unsubscribe(string name);
	}
}

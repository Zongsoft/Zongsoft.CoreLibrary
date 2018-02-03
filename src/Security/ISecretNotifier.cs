/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2015-2018 Zongsoft Corporation <http://www.zongsoft.com>
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
using System.Threading.Tasks;

namespace Zongsoft.Security
{
	/// <summary>
	/// 提供秘密校验通知功能的接口。
	/// </summary>
	public interface ISecretNotifier
	{
		/// <summary>
		/// 发送通知。
		/// </summary>
		/// <param name="name">指定的通知名。</param>
		/// <param name="parameter">激发通知的参数对象。</param>
		/// <param name="state">激发通知的附加状态值。</param>
		/// <returns>返回的通知结果。</returns>
		Common.IExecutionResult Notify(string name, object parameter, object state = null);

		/// <summary>
		/// 异步发送通知。
		/// </summary>
		/// <param name="name">指定的通知名。</param>
		/// <param name="parameter">激发通知的参数对象。</param>
		/// <param name="state">激发通知的附加状态值。</param>
		/// <returns>返回的通知结果。</returns>
		Task<Common.IExecutionResult> NotifyAsync(string name, object parameter, object state = null);
	}
}

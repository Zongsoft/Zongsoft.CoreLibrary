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

namespace Zongsoft.Communication
{
	/// <summary>
	/// 表示通知激发器的接口。
	/// </summary>
	public interface INotifier
	{
		/// <summary>
		/// 激发一个通知给指定的接受者。
		/// </summary>
		/// <param name="name">指定要激发的通知名。</param>
		/// <param name="content">指定的通知内容。</param>
		/// <param name="destination">指定通知的接受者，由具体实现者定义支持的接受者类型。</param>
		/// <param name="settings">指定的通知设置。</param>
		/// <returns>返回通知结果对象，由具体实现者定义。</returns>
		object Notify(string name, object content, object destination, object settings = null);

		/// <summary>
		/// 异步激发一个通知给指定的接受者。
		/// </summary>
		/// <param name="name">指定要激发的通知名。</param>
		/// <param name="content">指定的通知内容。</param>
		/// <param name="destination">指定通知的接受者，由具体实现者定义支持的接受者类型。</param>
		/// <param name="settings">指定的通知设置。</param>
		/// <returns>返回通知结果对象，由具体实现者定义。</returns>
		Task<object> NotifyAsync(string name, object content, object destination, object settings = null);
	}
}

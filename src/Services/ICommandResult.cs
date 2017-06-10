/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2016-2017 Zongsoft Corporation <http://www.zongsoft.com>
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

namespace Zongsoft.Services
{
	/// <summary>
	/// 表示包装命令执行结果的接口。
	/// </summary>
	public interface ICommandResult
	{
		/// <summary>
		/// 获取一个值，指示命令是否执行成功。
		/// </summary>
		bool Succeed
		{
			get;
		}

		/// <summary>
		/// 获取命令执行失败的原因代号。
		/// </summary>
		string Code
		{
			get;
		}

		/// <summary>
		/// 获取命令执行失败的消息。
		/// </summary>
		string Message
		{
			get;
		}

		/// <summary>
		/// 获取命令执行结果。
		/// </summary>
		object Result
		{
			get;
		}
	}
}

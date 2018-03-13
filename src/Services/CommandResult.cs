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
using System.Collections.Generic;

namespace Zongsoft.Services
{
	/// <summary>
	/// 表示命令执行结果的包装类。
	/// </summary>
	[Obsolete]
	public class CommandResult : ICommandResult
	{
		#region 成员字段
		private bool _succeed;
		private string _code;
		private string _message;
		private object _result;
		#endregion

		#region 构造函数
		public CommandResult(bool succeed, object result, string code, string message)
		{
			_succeed = succeed;
			_code = code;
			_message = message;
			_result = result;
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取一个值，指示命令执行是否成功。
		/// </summary>
		public bool Succeed
		{
			get
			{
				return _succeed;
			}
		}

		/// <summary>
		/// 获取命令执行失败的代码。
		/// </summary>
		public string Code
		{
			get
			{
				return _code;
			}
		}

		/// <summary>
		/// 获取命令执行失败的消息。
		/// </summary>
		public string Message
		{
			get
			{
				return _message;
			}
		}

		/// <summary>
		/// 获取命令执行的结果。
		/// </summary>
		public object Result
		{
			get
			{
				return _result;
			}
		}
		#endregion

		#region 静态方法
		public static CommandResult Success(object result, string message = null, string code = null)
		{
			return new CommandResult(true, result, code, message);
		}

		public static CommandResult Failure(string code, string message, object result = null)
		{
			return new CommandResult(false, result, code, message);
		}
		#endregion
	}
}

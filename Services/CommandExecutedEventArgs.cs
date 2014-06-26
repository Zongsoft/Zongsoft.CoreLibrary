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
using System.Text;

namespace Zongsoft.Services
{
	[Serializable]
	public class CommandExecutedEventArgs : EventArgs
	{
		#region 成员变量
		private bool _exceptionHandled;
		private Exception _exception;
		private object _parameter;
		private object _result;
		#endregion

		#region 构造函数
		/// <summary>
		/// 构造一个命令执行成功的事件参数对象。
		/// </summary>
		/// <param name="parameter">命令执行参数对象。</param>
		public CommandExecutedEventArgs(object parameter, object result)
		{
			_parameter = parameter;
			_result = result;
		}

		/// <summary>
		/// 构造一个命令执行失败的事件参数对象。
		/// </summary>
		/// <param name="parameter">命令执行参数对象。</param>
		/// <param name="exception">命令执行失败的异常对象。</param>
		public CommandExecutedEventArgs(object parameter, Exception exception)
		{
			_parameter = parameter;
			_exception = exception;
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取命令执行过程中的异常，如果返回空则表示为发生异常。
		/// </summary>
		public Exception Exception
		{
			get
			{
				return _exception;
			}
		}

		/// <summary>
		/// 获取或设置异常是否处理完成，如果返回假(false)则异常信息将被抛出。
		/// </summary>
		public bool ExceptionHandled
		{
			get
			{
				return _exceptionHandled;
			}
			set
			{
				_exceptionHandled = value;
			}
		}

		/// <summary>
		/// 获取命令的执行参数对象。
		/// </summary>
		public object Parameter
		{
			get
			{
				return _parameter;
			}
		}

		public object Result
		{
			get
			{
				return _result;
			}
			set
			{
				_result = value;
			}
		}
		#endregion
	}
}

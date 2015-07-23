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

namespace Zongsoft.Diagnostics
{
	/// <summary>
	/// 定义了异常处理的通用功能。
	/// </summary>
	public interface IExceptionHandler
	{
		/// <summary>
		/// 获取当前异常处理程序支持的所能处理的异常列表。
		/// </summary>
		List<Type> CanHandledExceptionTypes
		{
			get;
		}

		/// <summary>
		/// 判断当前异常处理器是否支持指定的异常类型。
		/// </summary>
		/// <param name="exceptionType">要判断的异常类型。</param>
		/// <returns>支持指定的异常类型则返回真(True)，否则返回假(False)。</returns>
		bool CanHandle(Type exceptionType);

		/// <summary>
		/// 处理指定的异常。
		/// </summary>
		/// <param name="exception">要处理的异常对象。</param>
		/// <returns>如果当前处理器已经对参数<paramref name="exception"/>指定的异常对象，处理完毕则返回为空，如果当前异常处理器还需要后续的其他处理器对返回的新异常对象继续处理的话，则返回一个新异常。</returns>
		Exception Handle(Exception exception);
	}
}

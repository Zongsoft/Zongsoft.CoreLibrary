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

namespace Zongsoft.Diagnostics
{
	public abstract class ExceptionHandlerBase : IExceptionHandler
	{
		#region 成员变量
		private readonly List<Type> _canHandledExceptionTypes;
		#endregion

		#region 构造函数
		protected ExceptionHandlerBase(Type[] canHandledExceptionTypes)
		{
			if(canHandledExceptionTypes == null || canHandledExceptionTypes.Length <= 0)
				throw new ArgumentNullException("canHandledExceptionTypes");

			_canHandledExceptionTypes = new List<Type>();

			foreach(Type exceptionType in canHandledExceptionTypes)
			{
				if(exceptionType != typeof(Exception) && (!exceptionType.IsSubclassOf(typeof(Exception))))
					throw new ArgumentException();

				_canHandledExceptionTypes.Add(exceptionType);
			}
		}
		#endregion

		#region 公共属性
		public List<Type> CanHandledExceptionTypes
		{
			get
			{
				return _canHandledExceptionTypes;
			}
		}
		#endregion

		#region 虚拟方法
		public virtual bool CanHandle(Type exceptionType)
		{
			if(exceptionType == null)
				throw new ArgumentNullException("exceptionType");

			foreach(Type type in _canHandledExceptionTypes)
			{
				if(type == exceptionType)
					return true;
			}

			return false;
		}
		#endregion

		#region 抽象方法
		public abstract Exception Handle(Exception exception);
		#endregion
	}
}

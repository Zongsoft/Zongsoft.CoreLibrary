/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2015 Zongsoft Corporation <http://www.zongsoft.com>
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
	public class LoggerHandler : Zongsoft.Services.CommandBase
	{
		#region 成员字段
		private ILogger _logger;
		#endregion

		#region 构造函数
		public LoggerHandler()
		{
			this.Predication = new LoggerHandlerPredication();
		}
		#endregion

		#region 公共属性
		public ILogger Logger
		{
			get
			{
				return _logger;
			}
			set
			{
				if(value == null)
					throw new ArgumentNullException();

				_logger = value;
			}
		}
		#endregion

		#region 重写方法
		protected override bool CanExecute(object parameter)
		{
			return parameter is LogEntry && this.Logger != null && base.CanExecute(parameter);
		}

		protected override object OnExecute(object parameter)
		{
			var logger = this.Logger;

			if(logger != null)
				logger.Log(parameter as LogEntry);

			return null;
		}
		#endregion

		#region 公共方法
		public void Handle(LogEntry entry)
		{
			this.Execute(entry);
		}
		#endregion
	}
}

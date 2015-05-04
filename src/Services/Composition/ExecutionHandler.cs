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

namespace Zongsoft.Services.Composition
{
	public class ExecutionHandler : ExecutionHandlerBase
	{
		#region 成员字段
		private ICommand _command;
		#endregion

		#region 构造函数
		public ExecutionHandler()
		{
		}

		public ExecutionHandler(ICommand command)
		{
			if(command == null)
				throw new ArgumentNullException("command");

			_command = command;
			this.Name = command.Name;
		}
		#endregion

		#region 公共属性
		public ICommand Command
		{
			get
			{
				return _command;
			}
			set
			{
				if(value == null)
					throw new ArgumentNullException();

				_command = value;
			}
		}
		#endregion

		#region 重写方法
		public override bool CanHandle(IExecutionPipelineContext context)
		{
			if(_command == null)
				return false;

			return base.CanHandle(context) && _command.CanExecute(context);
		}

		protected override void OnExecute(IExecutionPipelineContext context)
		{
			if(_command != null)
				context.Result = _command.Execute(context);
		}
		#endregion
	}
}

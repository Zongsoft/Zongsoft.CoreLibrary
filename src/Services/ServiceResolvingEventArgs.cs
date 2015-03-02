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
using System.Linq;
using System.Text;

namespace Zongsoft.Services
{
	public class ServiceResolvingEventArgs : System.ComponentModel.CancelEventArgs
	{
		#region 成员字段
		private string _serviceName;
		private Type _contractType;
		private object _parameter;
		private bool _isResolveAll;
		private object _result;
		#endregion

		#region 构造函数
		public ServiceResolvingEventArgs(string serviceName)
		{
			if(string.IsNullOrWhiteSpace(serviceName))
				throw new ArgumentNullException("serviceName");

			_serviceName = serviceName.Trim();
			_isResolveAll = false;
		}

		public ServiceResolvingEventArgs(Type contractType, object parameter, bool isResolveAll)
		{
			if(contractType == null)
				throw new ArgumentNullException("contractType");

			_contractType = contractType;
			_parameter = parameter;
			_isResolveAll = isResolveAll;
		}
		#endregion

		#region 公共属性
		public string ServiceName
		{
			get
			{
				return _serviceName;
			}
		}

		public Type ContractType
		{
			get
			{
				return _contractType;
			}
		}

		public object Parameter
		{
			get
			{
				return _parameter;
			}
		}

		public bool IsResolveAll
		{
			get
			{
				return _isResolveAll;
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

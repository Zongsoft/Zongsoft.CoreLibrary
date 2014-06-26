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

namespace Zongsoft.Services
{
	public class ServiceProviderChangedEventArgs : EventArgs
	{
		#region 成员字段
		private string _name;
		private object _instance;
		private Type _instanceType;
		private Type[] _contractTypes;
		#endregion

		#region 构造函数
		public ServiceProviderChangedEventArgs(string name, object instance, Type[] contractTypes)
		{
			if(instance == null)
				throw new ArgumentNullException("instance");

			_name = name;
			_instance = instance;
			_contractTypes = contractTypes;
		}

		public ServiceProviderChangedEventArgs(string name, Type instanceType, Type[] contractTypes)
		{
			if(instanceType == null)
				throw new ArgumentNullException("instanceType");

			_name = name;
			_instanceType = instanceType;
			_contractTypes = contractTypes;
		}
		#endregion

		#region 公共属性
		public string Name
		{
			get
			{
				return _name;
			}
		}

		public object Instance
		{
			get
			{
				return _instance;
			}
		}

		public Type InstanceType
		{
			get
			{
				if(_instanceType == null && _instance != null)
					_instanceType = _instance.GetType();

				return _instanceType;
			}
		}

		public Type[] ContractTypes
		{
			get
			{
				return _contractTypes ?? new Type[0];
			}
		}
		#endregion
	}
}

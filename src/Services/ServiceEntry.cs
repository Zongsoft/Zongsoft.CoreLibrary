/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2010-2014 Zongsoft Corporation <http://www.zongsoft.com>
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
	public class ServiceEntry
	{
		#region 私有变量
		private readonly object _syncRoot = new object();
		#endregion

		#region 成员字段
		private string _name;
		private object _service;
		private Type _serviceType;
		private Type[] _contractTypes;
		private object _userToken;

		private IServiceBuilder _builder;
		private IServiceLifetime _lifetime;
		#endregion

		#region 构造函数
		public ServiceEntry(string name, object service, Type[] contractTypes, object userToken = null)
		{
			if(string.IsNullOrWhiteSpace(name))
				throw new ArgumentNullException("name");

			if(service == null)
				throw new ArgumentNullException("service");

			_name = name.Trim();
			_service = service;
			_contractTypes = contractTypes;
			_userToken = userToken;
		}

		public ServiceEntry(string name, Type serviceType, Type[] contractTypes, object userToken = null)
		{
			if(string.IsNullOrWhiteSpace(name))
				throw new ArgumentNullException("name");

			if(serviceType == null)
				throw new ArgumentNullException("serviceType");

			_name = name.Trim();
			_serviceType = serviceType;
			_contractTypes = contractTypes;
			_userToken = userToken;
		}

		public ServiceEntry(object service, Type[] contractTypes, object userToken = null)
		{
			if(service == null)
				throw new ArgumentNullException("service");

			_service = service;
			_contractTypes = contractTypes;
			_userToken = userToken;
		}

		public ServiceEntry(Type serviceType, Type[] contractTypes, object userToken = null)
		{
			if(serviceType == null)
				throw new ArgumentNullException("serviceType");

			_serviceType = serviceType;
			_contractTypes = contractTypes;
			_userToken = userToken;
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

		public Type ServiceType
		{
			get
			{
				if(_serviceType == null)
				{
					object instance = this.Service;

					if(instance != null)
						_serviceType = instance.GetType();
				}

				return _serviceType;
			}
		}

		public virtual object Service
		{
			get
			{
				var result = _service;

				if(result == null)
				{
					lock(_syncRoot)
					{
						if(_service == null)
						{
							//创建一个新的服务实例
							_service = this.CreateService();

							//重置当前服务类型
							_serviceType = null;

							return _service;
						}
					}
				}

				var lifetime = _lifetime;

				//如果没有指定服务的生命期或者当前服务是可用的则返回它
				if(lifetime == null || lifetime.IsAlive(this))
					return result;

				//至此，表明当前服务已被判定过期不可用，则重新创建一个新的服务实例(并确保当前服务没有被修改过)
				System.Threading.Interlocked.CompareExchange(ref _service, this.CreateService(), result);

				//重置服务类型
				_serviceType = null;

				return _service;
			}
		}

		public bool HasService
		{
			get
			{
				return _service != null;
			}
		}

		public Type[] ContractTypes
		{
			get
			{
				return _contractTypes;
			}
		}

		public object UserToken
		{
			get
			{
				return _userToken;
			}
			set
			{
				_userToken = value;
			}
		}

		public IServiceBuilder Builder
		{
			get
			{
				return _builder;
			}
			set
			{
				_builder = value;
			}
		}

		public IServiceLifetime Lifetime
		{
			get
			{
				return _lifetime;
			}
			set
			{
				_lifetime = value;
			}
		}
		#endregion

		#region 虚拟方法
		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.Synchronized)]
		protected virtual object CreateService()
		{
			var builder = _builder;

			if(builder != null)
				return builder.Build(this);

			var type = _serviceType;

			if(type != null)
				return Activator.CreateInstance(type);

			return null;
		}
		#endregion
	}
}

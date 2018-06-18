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
	public class ServiceEntry : IEquatable<ServiceEntry>
	{
		#region 私有变量
		private readonly object _syncRoot = new object();
		#endregion

		#region 成员字段
		private readonly string _name;
		private object _service;
		private Type _serviceType;
		private Type[] _contractTypes;
		private object _userToken;

		private IServiceBuilder _builder;
		private IServiceLifetime _lifetime;
		#endregion

		#region 构造函数
		protected ServiceEntry()
		{
		}

		protected ServiceEntry(string name)
		{
			if(string.IsNullOrWhiteSpace(name))
				throw new ArgumentNullException(nameof(name));

			_name = name.Trim();
		}

		public ServiceEntry(string name, object service, Type[] contractTypes, object userToken = null)
		{
			if(string.IsNullOrWhiteSpace(name))
				throw new ArgumentNullException(nameof(name));

			_name = name.Trim();
			_service = service ?? throw new ArgumentNullException(nameof(service));
			_serviceType = service.GetType();
			_contractTypes = contractTypes;
			_userToken = userToken;
		}

		public ServiceEntry(string name, Type serviceType, Type[] contractTypes, object userToken = null)
		{
			if(string.IsNullOrWhiteSpace(name))
				throw new ArgumentNullException(nameof(name));

			_name = name.Trim();
			_serviceType = serviceType ?? throw new ArgumentNullException(nameof(serviceType));
			_contractTypes = contractTypes;
			_userToken = userToken;
		}

		public ServiceEntry(object service, Type[] contractTypes, object userToken = null)
		{
			_service = service ?? throw new ArgumentNullException(nameof(service));
			_serviceType = service.GetType();
			_contractTypes = contractTypes;
			_userToken = userToken;
		}

		public ServiceEntry(Type serviceType, Type[] contractTypes, object userToken = null)
		{
			_serviceType = serviceType ?? throw new ArgumentNullException(nameof(serviceType));
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
					_serviceType = this.GetServiceType();

				return _serviceType;
			}
		}

		public object Service
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

		public bool HasContracts
		{
			get
			{
				return _contractTypes != null && _contractTypes.Length > 0;
			}
		}

		public virtual Type[] ContractTypes
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
		/// <summary>
		/// 获取当前服务的类型。
		/// </summary>
		/// <returns>返回当前的服务类型。</returns>
		/// <remarks>
		///		<para>注意：实现者不应该在该方法内调用<see cref="CreateService()"/>方法或获取<see cref="Service"/>属性值，否则可能会引发程序死锁（StackOverflow）。</para>
		/// </remarks>
		protected virtual Type GetServiceType()
		{
			return _serviceType;
		}

		protected virtual object CreateService()
		{
			var builder = _builder;

			if(builder != null)
			{
				var instance = builder.Build(this);

				if(instance != null)
					_serviceType = instance.GetType();

				return instance;
			}

			var serviceType = _serviceType ?? this.GetServiceType();

			if(serviceType != null)
				return Activator.CreateInstance(serviceType);

			return null;
		}
		#endregion

		#region 重写方法
		public bool Equals(ServiceEntry other)
		{
			if(other == null)
				return false;

			if(string.IsNullOrEmpty(_name))
				return this.ServiceType == other.ServiceType;
			else
				return string.Equals(_name, other.Name);
		}

		public override bool Equals(object obj)
		{
			if(obj == null || obj.GetType() != typeof(ServiceEntry))
				return false;

			return this.Equals(obj as ServiceEntry);
		}

		public override int GetHashCode()
		{
			if(string.IsNullOrEmpty(_name))
			{
				var serviceType = this.ServiceType;
				return serviceType == null ? 0 : serviceType.GetHashCode();
			}

			return _name.GetHashCode();
		}

		public override string ToString()
		{
			if(string.IsNullOrEmpty(_name))
				return this.ServiceType.FullName;
			else
				return $"{_name} ({this.ServiceType.FullName})";
		}
		#endregion
	}
}

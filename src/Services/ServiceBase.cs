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

namespace Zongsoft.Services
{
	public abstract class ServiceBase : MarshalByRefObject
	{
		#region 成员字段
		private IServiceProvider _serviceProvider;
		#endregion

		#region 构造函数
		protected ServiceBase(IServiceProvider serviceProvider)
		{
			if(serviceProvider == null)
				throw new ArgumentNullException("serviceProvider");

			_serviceProvider = serviceProvider;
		}
		#endregion

		#region 公共属性
		protected virtual IServiceProvider ServiceProvider
		{
			get
			{
				return _serviceProvider;
			}
			set
			{
				if(value == null)
					throw new ArgumentNullException();

				_serviceProvider = value;
			}
		}
		#endregion

		#region 获取方法
		protected virtual object EnsureService(string name)
		{
			return ServiceProviderExtension.EnsureResolve(this.EnsureProvider(), name);
		}

		protected virtual object EnsureService(Type type)
		{
			return ServiceProviderExtension.EnsureResolve(this.EnsureProvider(), type);
		}

		protected virtual object EnsureService(Type type, object parameter)
		{
			return ServiceProviderExtension.EnsureResolve(this.EnsureProvider(), type, parameter);
		}

		protected virtual T EnsureService<T>() where T : class
		{
			return ServiceProviderExtension.EnsureResolve<T>(this.EnsureProvider());
		}

		protected virtual T EnsureService<T>(object parameter) where T : class
		{
			return ServiceProviderExtension.EnsureResolve<T>(this.EnsureProvider(), parameter);
		}
		#endregion

		#region 私有方法
		private IServiceProvider EnsureProvider()
		{
			var provider = this.ServiceProvider;

			if(provider == null)
				throw new InvalidOperationException("Missing service provider.");

			return provider;
		}
		#endregion
	}
}

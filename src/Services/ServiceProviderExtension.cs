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

namespace Zongsoft.Services
{
	public static class ServiceProviderExtension
	{
		public static object EnsureResolve(this IServiceProvider provider, string name)
		{
			if(provider == null)
				throw new ArgumentNullException("provider");

			var result = provider.Resolve(name);

			if(result != null)
				return result;

			throw new ServiceNotFoundException(string.Format("The named '{1}' of service is not found in '{0}' provider.", provider, name));
		}

		public static object EnsureResolve(this IServiceProvider provider, Type type)
		{
			if(provider == null)
				throw new ArgumentNullException("provider");

			var result = provider.Resolve(type);

			if(result != null)
				return result;

			throw new ServiceNotFoundException(string.Format("The typed '{1}' of service is not found in '{0}' provider.", provider, type.FullName));
		}

		public static object EnsureResolve(this IServiceProvider provider, Type type, object parameter)
		{
			if(provider == null)
				throw new ArgumentNullException("provider");

			var result = provider.Resolve(type);

			if(result != null)
				return result;

			throw new ServiceNotFoundException(string.Format("The typed '{1}' of service with '{2}' parameter is not found in '{0}' provider.", provider, type.FullName, parameter));
		}

		public static T EnsureResolve<T>(this IServiceProvider provider) where T : class
		{
			if(provider == null)
				throw new ArgumentNullException("provider");

			var result = provider.Resolve<T>();

			if(result != null)
				return result;

			throw new ServiceNotFoundException(string.Format("The typed '{1}' of service is not found in '{0}' provider.", provider, typeof(T).FullName));
		}

		public static T EnsureResolve<T>(this IServiceProvider provider, object parameter) where T : class
		{
			if(provider == null)
				throw new ArgumentNullException("provider");

			var result = provider.Resolve<T>(parameter);

			if(result != null)
				return result;

			throw new ServiceNotFoundException(string.Format("The typed '{1}' of service with '{2}' parameter is not found in '{0}' provider.", provider, typeof(T).FullName, parameter));
		}
	}
}

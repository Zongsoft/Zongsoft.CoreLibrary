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
	public interface IServiceProvider : System.IServiceProvider
	{
		#region 事件定义
		event EventHandler<ServiceRegisteredEventArgs> Registered;
		event EventHandler<ServiceUnregisteredEventArgs> Unregistered;
		#endregion

		#region 注册方法
		void Register(string name, Type serviceType);
		void Register(string name, Type serviceType, Type contractType);
		void Register(string name, Type serviceType, Type[] contractTypes);

		void Register(string name, object service);
		void Register(string name, object service, Type contractType);
		void Register(string name, object service, Type[] contractTypes);

		void Register(Type serviceType, Type contractType);
		void Register(Type serviceType, Type[] contractTypes);
		void Register(object service, Type contractType);
		void Register(object service, Type[] contractTypes);

		void Unregister(string name);
		#endregion

		#region 解析方法
		object Resolve(string name);
		object Resolve(Type type);
		object Resolve(Type type, object parameter);

		object ResolveRequired(string name);
		object ResolveRequired(Type type);
		object ResolveRequired(Type type, object parameter);

		T Resolve<T>() where T : class;
		T Resolve<T>(object parameter) where T : class;

		T ResolveRequired<T>() where T : class;
		T ResolveRequired<T>(object parameter) where T : class;

		IEnumerable<object> ResolveAll(Type type);
		IEnumerable<object> ResolveAll(Type type, object parameter);

		IEnumerable<T> ResolveAll<T>() where T : class;
		IEnumerable<T> ResolveAll<T>(object parameter) where T : class;
		#endregion
	}
}

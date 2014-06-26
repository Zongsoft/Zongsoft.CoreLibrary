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
using System.Collections;
using System.Collections.Generic;

namespace Zongsoft.Services
{
	public interface IServiceProvider : System.IServiceProvider
	{
		#region 事件定义
		event EventHandler<ServiceProviderChangedEventArgs> Registered;
		event EventHandler<ServiceProviderChangedEventArgs> Unregistered;
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取当前服务供应程序的名称。
		/// </summary>
		//string Name
		//{
		//    get;
		//}

		/// <summary>
		/// 获取或设置当前服务供应程序的解析比对器。
		/// </summary>
		//Collections.IMatcher Matcher
		//{
		//    get;
		//    set;
		//}
		#endregion

		#region 注册方法
		void Register(string name, Type instanceType);
		void Register(string name, Type instanceType, Type contractType);
		void Register(string name, Type instanceType, Type[] contractTypes);

		void Register(string name, object instance);
		void Register(string name, object instance, Type contractType);
		void Register(string name, object instance, Type[] contractTypes);

		void Register(Type instanceType, Type contractType);
		void Register(Type instanceType, Type[] contractTypes);
		void Register(object instance, Type contractType);
		void Register(object instance, Type[] contractTypes);

		void Unregister(string name);
		#endregion

		#region 解析方法
		object Resolve(string name);
		object Resolve(Type type);
		object Resolve(Type type, object parameter);

		T Resolve<T>() where T : class;
		T Resolve<T>(object parameter) where T : class;

		IEnumerable<object> ResolveAll(Type type);
		IEnumerable<object> ResolveAll(Type type, object parameter);

		IEnumerable<T> ResolveAll<T>() where T : class;
		IEnumerable<T> ResolveAll<T>(object parameter) where T : class;
		#endregion
	}
}

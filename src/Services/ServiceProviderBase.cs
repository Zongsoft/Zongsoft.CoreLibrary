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
using System.Linq;

namespace Zongsoft.Services
{
	public class ServiceProviderBase : MarshalByRefObject, IServiceProvider, System.IServiceProvider
	{
		#region 事件声明
		public event EventHandler<ServiceRegisteredEventArgs> Registered;
		public event EventHandler<ServiceUnregisteredEventArgs> Unregistered;

		public event EventHandler<ServiceResolvingEventArgs> Resolving;
		public event EventHandler<ServiceResolvedEventArgs> Resolved;
		#endregion

		#region 成员字段
		private IServiceStorage _storage;
		private IServiceBuilder _builder;
		#endregion

		#region 构造函数
		protected ServiceProviderBase(IServiceStorage storage) : this(storage, null)
		{
		}

		protected ServiceProviderBase(IServiceStorage storage, IServiceBuilder builder)
		{
			if(storage == null)
				throw new ArgumentNullException("storage");

			_storage = storage;
			_builder = builder;
		}
		#endregion

		#region 公共属性
		public IServiceBuilder Builder
		{
			get
			{
				return _builder;
			}
			protected set
			{
				_builder = value;
			}
		}

		public IServiceStorage Storage
		{
			get
			{
				return _storage;
			}
		}
		#endregion

		#region 注册方法
		public void Register(string name, Type serviceType)
		{
			this.Register(name, serviceType, (Type[])null);
		}

		public void Register(string name, Type serviceType, Type contractType)
		{
			this.Register(name, serviceType, new Type[] { contractType });
		}

		public void Register(string name, Type serviceType, Type[] contractTypes)
		{
			//创建一个服务描述项对象
			var entry = this.CreateEntry(name, serviceType, contractTypes);

			//执行具体的注册操作
			this.Register(entry);
		}

		public void Register(string name, object service)
		{
			this.Register(name, service, (Type[])null);
		}

		public void Register(string name, object service, Type contractType)
		{
			this.Register(name, service, new Type[] { contractType });
		}

		public void Register(string name, object service, Type[] contractTypes)
		{
			//创建一个服务描述项对象
			var entry = this.CreateEntry(name, service, contractTypes);

			//执行具体的注册操作
			this.Register(entry);
		}

		public void Register(Type serviceType, Type contractType)
		{
			this.Register(serviceType, new Type[] { contractType });
		}

		public void Register(Type serviceType, Type[] contractTypes)
		{
			//创建一个服务描述项对象
			var entry = this.CreateEntry(serviceType, contractTypes);

			//执行具体的注册操作
			this.Register(entry);
		}

		public void Register(object service, Type contractType)
		{
			this.Register(service, new Type[] { contractType });
		}

		public void Register(object service, Type[] contractTypes)
		{
			//创建一个服务描述项对象
			var entry = this.CreateEntry(service, contractTypes);

			//执行具体的注册操作
			this.Register(entry);
		}

		public void Unregister(string name)
		{
			var entry = _storage.Remove(name);

			if(entry != null)
				this.OnUnregistered(new ServiceUnregisteredEventArgs(entry));
		}
		#endregion

		#region 解析方法
		object System.IServiceProvider.GetService(Type serviceType)
		{
			return this.Resolve(serviceType);
		}

		public object Resolve(string name)
		{
			if(string.IsNullOrWhiteSpace(name))
				throw new ArgumentNullException("name");

			var args = new ServiceResolvingEventArgs(name);

			//激发“Resolving”事件
			this.OnResolving(args);

			if(args.Cancel)
				return args.Result;

			object result = null;
			var entry = _storage.Get(name);

			if(entry != null)
				result = this.GetService(entry);

			//激发“Resolved”事件
			this.OnResolved(new ServiceResolvedEventArgs(name, result));

			//返回解析的结果
			return result;
		}

		public object Resolve(Type type)
		{
			return this.Resolve(type, null);
		}

		public object Resolve(Type type, object parameter)
		{
			if(type == null)
				throw new ArgumentNullException("type");

			var args = new ServiceResolvingEventArgs(type, parameter, false);

			//激发“Resolving”事件
			this.OnResolving(args);

			if(args.Cancel)
				return args.Result;

			object result = null;
			var entry = _storage.Get(type, parameter);

			if(entry != null)
				result = this.GetService(entry);

			//激发“Resolved”事件
			this.OnResolved(new ServiceResolvedEventArgs(type, parameter, false, result));

			//返回解析的结果
			return result;
		}

		public object ResolveRequired(string name)
		{
			var result = this.Resolve(name);

			if(result == null)
				throw new ServiceNotFoundException(string.Format("The named '{1}' of service is not found in '{0}' provider.", this, name));

			return result;
		}

		public object ResolveRequired(Type type)
		{
			var result = this.Resolve(type);

			if(result == null)
				throw new ServiceNotFoundException(string.Format("The typed '{1}' of service is not found in '{0}' provider.", this, type.FullName));

			return result;
		}

		public object ResolveRequired(Type type, object parameter)
		{
			var result = this.Resolve(type);

			if(result == null)
				throw new ServiceNotFoundException(string.Format("The typed '{1}' of service with '{2}' parameter is not found in '{0}' provider.", this, type.FullName, parameter));

			return result;
		}

		public T Resolve<T>() where T : class
		{
			return (T)this.Resolve(typeof(T), null);
		}

		public T Resolve<T>(object parameter) where T : class
		{
			return (T)this.Resolve(typeof(T), parameter);
		}

		public T ResolveRequired<T>() where T : class
		{
			var result = this.Resolve<T>();

			if(result == null)
				throw new ServiceNotFoundException(string.Format("The typed '{1}' of service is not found in '{0}' provider.", this, typeof(T).FullName));

			return result;
		}

		public T ResolveRequired<T>(object parameter) where T : class
		{
			var result = this.Resolve<T>(parameter);

			if(result == null)
				throw new ServiceNotFoundException(string.Format("The typed '{1}' of service with '{2}' parameter is not found in '{0}' provider.", this, typeof(T).FullName, parameter));

			return result;
		}

		public IEnumerable<object> ResolveAll(Type type)
		{
			return this.ResolveAll(type, null);
		}

		public IEnumerable<object> ResolveAll(Type type, object parameter)
		{
			if(type == null)
				throw new ArgumentNullException("type");

			var args = new ServiceResolvingEventArgs(type, parameter, true);

			//激发“Resolving”事件
			this.OnResolving(args);

			if(args.Cancel)
				return args.Result as IEnumerable<object>;

			IEnumerable<object> result;
			var entries = _storage.GetAll(type, parameter);

			if(entries == null)
				result = Enumerable.Empty<object>();
			else
				result = entries.Select(entry => this.GetService(entry));

			//激发“Resolved”事件
			this.OnResolved(new ServiceResolvedEventArgs(type, parameter, true, result));

			//返回结果集
			return result;
		}

		public IEnumerable<T> ResolveAll<T>() where T : class
		{
			return this.ResolveAll(typeof(T), null).Cast<T>();
		}

		public IEnumerable<T> ResolveAll<T>(object parameter) where T : class
		{
			return this.ResolveAll(typeof(T), parameter).Cast<T>();
		}
		#endregion

		#region 虚拟方法
		protected virtual object GetService(ServiceEntry entry)
		{
			if(entry == null)
				return null;

			var result = entry.Service;

			if(result == null)
			{
				var builder = _builder;

				if(builder != null)
					result = builder.Build(entry);
			}

			return result;
		}

		protected virtual ServiceEntry CreateEntry(object service, Type[] contractTypes)
		{
			return new ServiceEntry(service, contractTypes);
		}

		protected virtual ServiceEntry CreateEntry(Type serviceType, Type[] contractTypes)
		{
			return new ServiceEntry(serviceType, contractTypes);
		}

		protected virtual ServiceEntry CreateEntry(string name, object service, Type[] contractTypes)
		{
			return new ServiceEntry(name, service, contractTypes);
		}

		protected virtual ServiceEntry CreateEntry(string name, Type serviceType, Type[] contractTypes)
		{
			return new ServiceEntry(name, serviceType, contractTypes);
		}
		#endregion

		#region 激发事件
		protected virtual void OnResolving(ServiceResolvingEventArgs args)
		{
			var resolving = this.Resolving;

			if(resolving != null)
				resolving(this, args);
		}

		protected virtual void OnResolved(ServiceResolvedEventArgs args)
		{
			var resolved = this.Resolved;

			if(resolved != null)
				resolved(this, args);
		}

		protected virtual void OnRegistered(ServiceRegisteredEventArgs args)
		{
			var registered = this.Registered;

			if(registered != null)
				registered(this, args);
		}

		protected virtual void OnUnregistered(ServiceUnregisteredEventArgs args)
		{
			var unregistered = this.Unregistered;

			if(unregistered != null)
				unregistered(this, args);
		}
		#endregion

		#region 私有方法
		private void Register(ServiceEntry entry)
		{
			if(entry == null)
				throw new InvalidOperationException(string.Format("Can not register for the {0}@{1}", entry.Name, entry.ServiceType));

			//将服务描述项保存到服务容器中
			_storage.Add(entry);

			//激发“Registered”事件
			this.OnRegistered(new ServiceRegisteredEventArgs(entry));
		}
		#endregion
	}
}

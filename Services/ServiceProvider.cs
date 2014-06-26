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
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Zongsoft.Services
{
	public class ServiceProvider : MarshalByRefObject, IServiceProvider, System.IServiceProvider
	{
		#region 事件声明
		public event EventHandler<ServiceProviderChangedEventArgs> Registered;
		public event EventHandler<ServiceProviderChangedEventArgs> Unregistered;

		public event EventHandler<ServiceResolvingEventArgs> Resolving;
		public event EventHandler<ServiceResolvedEventArgs> Resolved;
		#endregion

		#region 成员字段
		private string _name;
		private ServiceStubHelper _helper;
		#endregion

		#region 构造函数
		public ServiceProvider() : this("Default")
		{
		}

		public ServiceProvider(string name)
		{
			if(string.IsNullOrWhiteSpace(name))
				throw new ArgumentNullException("name");

			_name = name.Trim();
			_helper = new ServiceStubHelper();
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
		#endregion

		#region 注册方法
		public void Unregister(string name)
		{
			var stub = _helper.Remove(name);

			if(stub != null)
			{
				//激发“Unregistered”注销完成事件
				if(stub.HasInstance)
					this.OnUnregistered(new ServiceProviderChangedEventArgs(stub.Name, stub.Instance, stub.ContractTypes));
				else
					this.OnUnregistered(new ServiceProviderChangedEventArgs(stub.Name, stub.InstanceType, stub.ContractTypes));
			}
		}

		public void Register(string name, Type instanceType)
		{
			this.Register(name, instanceType, (Type[])null);
		}

		public void Register(string name, Type instanceType, Type contractType)
		{
			this.Register(name, instanceType, new Type[] { contractType });
		}

		public void Register(string name, Type instanceType, Type[] contractTypes)
		{
			if(string.IsNullOrWhiteSpace(name))
				throw new ArgumentNullException("name");

			if(instanceType == null)
				throw new ArgumentNullException("instanceType");

			_helper.Add(this.CreateStub(name, instanceType, contractTypes), false);

			//激发“Registered”注册完成事件
			this.OnRegistered(new ServiceProviderChangedEventArgs(name, instanceType, contractTypes));
		}

		public void Register(string name, object instance)
		{
			this.Register(name, instance, (Type[])null);
		}

		public void Register(string name, object instance, Type contractType)
		{
			this.Register(name, instance, new Type[] { contractType });
		}

		public void Register(string name, object instance, Type[] contractTypes)
		{
			if(string.IsNullOrWhiteSpace(name))
				throw new ArgumentNullException("name");

			if(instance == null)
				throw new ArgumentNullException("instance");

			_helper.Add(this.CreateStub(name, instance, contractTypes), false);

			//激发“Registered”注册完成事件
			this.OnRegistered(new ServiceProviderChangedEventArgs(name, instance, contractTypes));
		}

		public void Register(Type instanceType, Type contractType)
		{
			this.Register(instanceType, new Type[] { contractType });
		}

		public void Register(Type instanceType, Type[] contractTypes)
		{
			if(instanceType == null)
				throw new ArgumentNullException("instanceType");

			_helper.Add(this.CreateStub(instanceType, contractTypes), false);

			//激发“Registered”注册完成事件
			this.OnRegistered(new ServiceProviderChangedEventArgs(null, instanceType, contractTypes));
		}

		public void Register(object instance, Type contractType)
		{
			this.Register(instance, new Type[] { contractType });
		}

		public void Register(object instance, Type[] contractTypes)
		{
			if(instance == null)
				throw new ArgumentNullException("instance");

			_helper.Add(this.CreateStub(instance, contractTypes), false);

			//激发“Registered”注册完成事件
			this.OnRegistered(new ServiceProviderChangedEventArgs(null, instance, contractTypes));
		}
		#endregion

		#region 解析服务
		object System.IServiceProvider.GetService(Type serviceType)
		{
			return this.Resolve(serviceType, null);
		}

		public object Resolve(string name)
		{
			if(string.IsNullOrWhiteSpace(name))
				throw new ArgumentNullException("name");

			//激发“Resolving”事件
			var resolvingArgs = this.RaiseResolving(name);

			if(resolvingArgs.Cancel)
				return resolvingArgs.Result;

			//执行服务查询操作
			var result = _helper.Get(name);

			//激发“Resolved”事件，
			return this.RaiseResolved(name, result).Result;
		}

		public object Resolve(Type type)
		{
			return this.Resolve(type, null);
		}

		public object Resolve(Type type, object parameter)
		{
			if(type == null)
				throw new ArgumentNullException("type");

			//激发“Resolving”事件
			var resolvingArgs = this.RaiseResolving(type, parameter, false);

			if(resolvingArgs.Cancel)
				return resolvingArgs.Result;

			//执行服务查询操作
			var result = _helper.Get(type, parameter);

			//激发“Resolved”事件，
			return this.RaiseResolved(type, parameter, false, result).Result;
		}

		public T Resolve<T>() where T : class
		{
			return (T)this.Resolve(typeof(T), null);
		}

		public T Resolve<T>(object parameter) where T : class
		{
			return (T)this.Resolve(typeof(T), parameter);
		}

		public IEnumerable<object> ResolveAll(Type type)
		{
			return this.ResolveAll(type, null);
		}

		public IEnumerable<object> ResolveAll(Type type, object parameter)
		{
			if(type == null)
				throw new ArgumentNullException("type");

			//激发“Resolving”事件
			var resolvingArgs = this.RaiseResolving(type, parameter, true);

			if(resolvingArgs.Cancel)
				return resolvingArgs.Result as IEnumerable<object>;

			//执行服务查询操作
			var result = _helper.GetAll(type, parameter);

			//激发“Resolved”事件，
			var resolvedArgs = this.RaiseResolved(type, parameter, true, result);

			return resolvedArgs.Result as IEnumerable<object> ?? Enumerable.Empty<object>();
		}

		public IEnumerable<T> ResolveAll<T>() where T : class
		{
			return this.ResolveAll<T>(null);
		}

		public IEnumerable<T> ResolveAll<T>(object parameter) where T : class
		{
			return this.ResolveAll(typeof(T), parameter).Cast<T>();
		}
		#endregion

		#region 虚拟方法
		protected virtual ServiceStub CreateStub(string name, object instance, Type[] contractTypes)
		{
			return new ServiceStub(name, instance, contractTypes);
		}

		protected virtual ServiceStub CreateStub(string name, Type instanceType, Type[] contractTypes)
		{
			return new ServiceStub(name, instanceType, contractTypes);
		}

		protected virtual ServiceStub CreateStub(object instance, Type[] contractTypes)
		{
			return new ServiceStub(instance, contractTypes);
		}

		protected virtual ServiceStub CreateStub(Type instanceType, Type[] contractTypes)
		{
			return new ServiceStub(instanceType, contractTypes);
		}
		#endregion

		#region 激发事件
		protected ServiceResolvingEventArgs RaiseResolving(string serviceName)
		{
			var args = new ServiceResolvingEventArgs(serviceName);

			//调用虚拟方法，以激发事件并允许子类重写激发逻辑
			this.OnResolving(args);

			return args;
		}

		protected ServiceResolvingEventArgs RaiseResolving(Type contractType, object parameter, bool isResolveAll)
		{
			var args = new ServiceResolvingEventArgs(contractType, parameter, isResolveAll);

			//调用虚拟方法，以激发事件并允许子类重写激发逻辑
			this.OnResolving(args);

			return args;
		}

		/// <summary>
		/// 激发<seealso cref="Resolved"/>服务解析完成事件。
		/// </summary>
		/// <param name="serviceName">解析的服务名称。</param>
		/// <param name="result">解析完成的服务对象。</param>
		/// <returns>返回<see cref="ServiceResolvedEventArgs"/>事件参数对象。</returns>
		/// <remarks>
		///		<para>注意本方法内部会对参数进行进行转换，因此返回的事件参数对象中的Result属性将为目标对象。</para>
		/// </remarks>
		protected ServiceResolvedEventArgs RaiseResolved(string serviceName, object result)
		{
			var token = result as ServiceStub;

			if(token != null)
				result = token.Instance;
			else if(result is IEnumerable<ServiceStub>)
				result = ((IEnumerable<ServiceStub>)result).Select(p => p.Instance);

			var args = new ServiceResolvedEventArgs(serviceName, result);

			//调用虚拟方法，以激发事件并允许子类重写激发逻辑
			this.OnResolved(args);

			return args;
		}

		protected ServiceResolvedEventArgs RaiseResolved(Type contractType, object parameter, bool isResolveAll, object result)
		{
			var tokens = result as IEnumerable<ServiceStub>;

			if(tokens != null)
				result = tokens.Select(token => token.Instance);
			else if(result is ServiceStub)
				result = ((ServiceStub)result).Instance;

			var args = new ServiceResolvedEventArgs(contractType, parameter, isResolveAll, result);

			//调用虚拟方法，以激发事件并允许子类重写激发逻辑
			this.OnResolved(args);

			return args;
		}

		protected virtual void OnResolving(ServiceResolvingEventArgs args)
		{
			if(this.Resolving != null)
				this.Resolving(this, args);
		}

		protected virtual void OnResolved(ServiceResolvedEventArgs args)
		{
			if(this.Resolved != null)
				this.Resolved(this, args);
		}

		protected virtual void OnRegistered(ServiceProviderChangedEventArgs args)
		{
			if(this.Registered != null)
				this.Registered(this, args);
		}

		protected virtual void OnUnregistered(ServiceProviderChangedEventArgs args)
		{
			if(this.Unregistered != null)
				this.Unregistered(this, args);
		}
		#endregion

		#region 保护方法
		protected bool IsMatch(object input, object parameter)
		{
			if(input == null)
				return true;

			ServiceStub stub = input as ServiceStub;

			if(stub != null && typeof(IMatchable).IsAssignableFrom(stub.InstanceType))
				input = stub.Instance;

			IMatchable matchable = input as IMatchable;

			if(matchable != null)
				return matchable.IsMatch(parameter);

			return true;
		}
		#endregion

		#region 嵌套子类
		public class ServiceStub
		{
			#region 成员字段
			private string _name;
			private object _instance;
			private Type _instanceType;
			private Type[] _contractTypes;
			#endregion

			#region 构造函数
			internal protected ServiceStub(string name, object instance, Type[] contractTypes)
			{
				if(string.IsNullOrWhiteSpace(name))
					throw new ArgumentNullException("name");

				if(instance == null)
					throw new ArgumentNullException("instance");

				_name = name.Trim();
				_instance = instance;
				_contractTypes = contractTypes;
			}

			internal protected ServiceStub(string name, Type instanceType, Type[] contractTypes)
			{
				if(string.IsNullOrWhiteSpace(name))
					throw new ArgumentNullException("name");

				if(instanceType == null)
					throw new ArgumentNullException("instanceType");

				_name = name.Trim();
				_instanceType = instanceType;
				_contractTypes = contractTypes;
			}

			internal protected ServiceStub(object instance, Type[] contractTypes)
			{
				if(instance == null)
					throw new ArgumentNullException("instance");

				_instance = instance;
				_contractTypes = contractTypes;
			}

			internal protected ServiceStub(Type instanceType, Type[] contractTypes)
			{
				if(instanceType == null)
					throw new ArgumentNullException("instanceType");

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

			public Type InstanceType
			{
				get
				{
					if(_instanceType == null)
					{
						object instance = this.Instance;

						if(instance != null)
							_instanceType = instance.GetType();
					}

					return _instanceType;
				}
			}

			public virtual object Instance
			{
				get
				{
					if(_instance == null)
						System.Threading.Interlocked.CompareExchange(ref _instance, this.CreateInstance(), null);

					return _instance;
				}
			}

			public bool HasInstance
			{
				get
				{
					return _instance != null;
				}
			}

			public Type[] ContractTypes
			{
				get
				{
					return _contractTypes;
				}
			}
			#endregion

			#region 保护方法
			protected virtual object CreateInstance()
			{
				if(_instanceType != null)
					return Activator.CreateInstance(_instanceType);

				return null;
			}
			#endregion
		}

		private class ServiceStubHelper
		{
			#region 成员字段
			private IList<ServiceStub> _list;
			private IDictionary<string, ServiceStub> _namedDictionary;
			#endregion

			#region 构造函数
			internal ServiceStubHelper()
			{
				_list = new List<ServiceStub>();
				_namedDictionary = new Dictionary<string, ServiceStub>(StringComparer.OrdinalIgnoreCase);
			}
			#endregion

			#region 公共方法
			public void Add(ServiceStub value, bool throwsExceptionOnConflict)
			{
				if(value == null)
					return;

				if(!string.IsNullOrEmpty(value.Name))
				{
					if(throwsExceptionOnConflict && _namedDictionary.ContainsKey(value.Name))
						throw new InvalidOperationException();

					_namedDictionary[value.Name] = value;
				}

				_list.Add(value);
			}

			public ServiceStub Remove(string name)
			{
				if(string.IsNullOrWhiteSpace(name))
					return null;

				name = name.Trim();
				ServiceStub value = null;

				if(_namedDictionary.TryGetValue(name, out value))
				{
					_namedDictionary.Remove(name);
					_list.Remove(value);
				}

				return value;
			}

			public ServiceStub Get(string name)
			{
				if(string.IsNullOrWhiteSpace(name))
					return null;

				name = name.Trim();
				ServiceStub value;

				if(_namedDictionary.TryGetValue(name, out value))
					return value;

				return null;
			}

			public ServiceStub Get(Type contractType, object parameter)
			{
				if(contractType == null)
					return null;

				IList<ServiceStub> list = new List<ServiceStub>();

				foreach(var stub in _list)
				{
					if(stub.ContractTypes == null || stub.ContractTypes.Length < 1)
						list.Add(stub);
					else if(stub.ContractTypes.Contains(contractType))
					{
						if(this.OnMatch(stub, parameter))
							return stub;
					}
				}

				if(list != null && list.Count > 0)
				{
					foreach(var stub in list)
					{
						if(contractType.IsAssignableFrom(stub.InstanceType))
						{
							if(this.OnMatch(stub, parameter))
								return stub;
						}
					}
				}

				return null;
			}

			public IEnumerable<ServiceStub> GetAll(Type contractType, object parameter)
			{
				if(contractType == null)
					return null;

				IList<ServiceStub> list = new List<ServiceStub>();
				IList<ServiceStub> result = new List<ServiceStub>();

				foreach(var stub in _list)
				{
					if(stub.ContractTypes == null || stub.ContractTypes.Length < 1)
						list.Add(stub);
					else if(stub.ContractTypes.Contains(contractType))
					{
						if(this.OnMatch(stub, parameter))
							result.Add(stub);
					}
				}

				if(list != null && list.Count > 0)
				{
					foreach(var stub in list)
					{
						if(contractType.IsAssignableFrom(stub.InstanceType))
						{
							if(this.OnMatch(stub, parameter))
								result.Add(stub);
						}
					}
				}

				return result.ToArray();
			}
			#endregion

			#region 私有方法
			private bool OnMatch(ServiceStub stub, object parameter)
			{
				if(typeof(IMatchable).IsAssignableFrom(stub.InstanceType))
				{
					var instance = (IMatchable)stub.Instance;

					if(instance != null)
						return instance.IsMatch(parameter);
					else
						return false;
				}

				return true;
			}
			#endregion
		}
		#endregion
	}
}

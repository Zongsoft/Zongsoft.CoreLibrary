/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2010-2016 Zongsoft Corporation <http://www.zongsoft.com>
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
using System.Collections.Concurrent;
using System.Linq;

namespace Zongsoft.Services
{
	public class ServiceStorage : MarshalByRefObject, IServiceStorage, ICollection, ICollection<ServiceEntry>
	{
		#region 成员字段
		private IMatcher _matcher;
		private List<ServiceEntry> _entries;
		private ConcurrentDictionary<string, ServiceEntry> _namedEntries;
		#endregion

		#region 构造函数
		public ServiceStorage() : this(Zongsoft.Services.Matcher.Default)
		{
		}

		public ServiceStorage(IMatcher matcher)
		{
			_matcher = matcher;
			_entries = new List<ServiceEntry>();
			_namedEntries = new ConcurrentDictionary<string, ServiceEntry>(StringComparer.OrdinalIgnoreCase);
		}
		#endregion

		#region 公共属性
		public virtual int Count
		{
			get
			{
				return _entries.Count;
			}
		}

		public IMatcher Matcher
		{
			get
			{
				return _matcher;
			}
			set
			{
				_matcher = value;
			}
		}
		#endregion

		#region 公共方法
		public ServiceEntry Add(object service)
		{
			return this.Add(service, null);
		}

		public ServiceEntry Add(object service, Type[] contractTypes)
		{
			var entry = new ServiceEntry(service, contractTypes);
			this.Add(entry);
			return entry;
		}

		public ServiceEntry Add(Type serviceType)
		{
			return this.Add(serviceType, null);
		}

		public ServiceEntry Add(Type serviceType, Type[] contractTypes)
		{
			var entry = new ServiceEntry(serviceType, contractTypes);
			this.Add(entry);
			return entry;
		}

		public ServiceEntry Add(string name, object service)
		{
			return this.Add(name, service, null);
		}

		public ServiceEntry Add(string name, object service, Type[] contractTypes)
		{
			var entry = new ServiceEntry(name, service, contractTypes);
			this.Add(entry);
			return entry;
		}

		public ServiceEntry Add(string name, Type serviceType)
		{
			return this.Add(name, serviceType, null);
		}

		public ServiceEntry Add(string name, Type serviceType, Type[] contractTypes)
		{
			var entry = new ServiceEntry(name, serviceType, contractTypes);
			this.Add(entry);
			return entry;
		}

		public virtual void Add(ServiceEntry entry)
		{
			if(entry == null)
				return;

			if(!string.IsNullOrWhiteSpace(entry.Name))
				_namedEntries[entry.Name] = entry;

			_entries.Add(entry);
		}

		public virtual void Clear()
		{
			_namedEntries.Clear();
			_entries.Clear();
		}

		public virtual ServiceEntry Remove(string name)
		{
			if(string.IsNullOrWhiteSpace(name))
				return null;

			ServiceEntry entry = null;

			if(_namedEntries.TryRemove(name, out entry))
				_entries.Remove(entry);

			return entry;
		}

		public virtual ServiceEntry Get(string name)
		{
			if(string.IsNullOrWhiteSpace(name))
				return null;

			name = name.Trim();
			ServiceEntry entry;

			if(_namedEntries.TryGetValue(name, out entry))
				return entry;

			return this.GetEntryFromOtherStorage(name);
		}

		public virtual ServiceEntry Get(Type type, object parameter = null)
		{
			if(type == null)
				return null;

			var entries = new List<ServiceEntry>();

			foreach(var entry in _entries)
			{
				if(entry.ContractTypes == null || entry.ContractTypes.Length < 1)
					entries.Add(entry);
				else if(entry.ContractTypes.Contains(type))
				{
					if(this.OnMatch(entry, parameter))
						return entry;
				}
			}

			if(entries != null && entries.Count > 0)
			{
				foreach(var entry in entries)
				{
					if(type.IsAssignableFrom(entry.ServiceType))
					{
						if(this.OnMatch(entry, parameter))
							return entry;
					}
				}
			}

			return this.GetEntryFromOtherStorage(type, parameter);
		}

		public virtual IEnumerable<ServiceEntry> GetAll(Type type, object parameter = null)
		{
			if(type == null)
				return null;

			var list = new List<ServiceEntry>();
			var result = new List<ServiceEntry>();

			foreach(var entry in _entries)
			{
				if(entry.ContractTypes == null || entry.ContractTypes.Length < 1)
					list.Add(entry);
				else if(entry.ContractTypes.Contains(type))
				{
					if(this.OnMatch(entry, parameter))
						result.Add(entry);
				}
			}

			if(list != null && list.Count > 0)
			{
				foreach(var entry in list)
				{
					if(type.IsAssignableFrom(entry.ServiceType))
					{
						if(this.OnMatch(entry, parameter))
							result.Add(entry);
					}
				}
			}

			return result.ToArray();
		}
		#endregion

		#region 匹配方法
		protected virtual bool OnMatch(ServiceEntry entry, object parameter)
		{
			if(entry == null)
				return false;

			var matchable = typeof(IMatchable).IsAssignableFrom(entry.ServiceType);

			if(typeof(IMatchable).IsAssignableFrom(entry.ServiceType))
				return ((IMatchable)entry.Service).IsMatch(parameter);

			var attribute = (MatcherAttribute)Attribute.GetCustomAttribute(entry.ServiceType, typeof(MatcherAttribute), true);

			if(attribute != null && attribute.Matcher != null)
				return attribute.Matcher.Match(entry.Service, parameter);

			return true;
		}
		#endregion

		#region 显式实现
		void ICollection.CopyTo(Array array, int index)
		{
			for(int i = index; i < array.Length; index++)
			{
				array.SetValue(_entries[i - index], i);
			}
		}

		bool ICollection.IsSynchronized
		{
			get
			{
				return true;
			}
		}

		private object _syncRoot;

		object ICollection.SyncRoot
		{
			get
			{
				if(_syncRoot == null)
					System.Threading.Interlocked.CompareExchange(ref _syncRoot, new object(), null);

				return _syncRoot;
			}
		}

		bool ICollection<ServiceEntry>.Contains(ServiceEntry item)
		{
			return _entries.Contains(item);
		}

		void ICollection<ServiceEntry>.CopyTo(ServiceEntry[] array, int arrayIndex)
		{
			_entries.CopyTo(array, arrayIndex);
		}


		bool ICollection<ServiceEntry>.IsReadOnly
		{
			get
			{
				return false;
			}
		}

		bool ICollection<ServiceEntry>.Remove(ServiceEntry item)
		{
			throw new NotSupportedException();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			foreach(var entry in _entries)
				yield return entry;
		}

		IEnumerator<ServiceEntry> IEnumerable<ServiceEntry>.GetEnumerator()
		{
			foreach(var entry in _entries)
				yield return entry;
		}
		#endregion

		#region 私有方法
		private ServiceEntry GetEntryFromOtherStorage(string name)
		{
			foreach(var entry in _entries.ToArray())
			{
				var result = this.GetEntryFromOtherStorage(entry, storage => storage.Get(name));

				if(result != null)
					return result;
			}

			return null;
		}

		private ServiceEntry GetEntryFromOtherStorage(Type type, object parameter)
		{
			foreach(var entry in _entries.ToArray())
			{
				var result = this.GetEntryFromOtherStorage(entry, storage => storage.Get(type, parameter));

				if(result != null)
					return result;
			}

			return null;
		}

		private ServiceEntry GetEntryFromOtherStorage(ServiceEntry entry, Func<IServiceStorage, ServiceEntry> getThunk)
		{
			if(entry == null || getThunk == null)
				return null;

			if(typeof(IServiceProvider).IsAssignableFrom(entry.ServiceType))
			{
				var provider = (IServiceProvider)entry.Service;

				if(provider != null && provider.Storage != null)
				{
					var result = getThunk(provider.Storage);

					if(result != null)
						return result;
				}
			}

			return null;
		}
		#endregion
	}
}

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
using System.Collections.Concurrent;
using System.Linq;

namespace Zongsoft.Services
{
	public class ServiceStorage : MarshalByRefObject, IServiceStorage
	{
		#region 成员字段
		private IMatcher _matcher;
		private List<ServiceEntry> _list;
		private ConcurrentDictionary<string, ServiceEntry> _namedDictionary;
		#endregion

		#region 构造函数
		public ServiceStorage() : this(Zongsoft.Services.Matcher.Default)
		{
		}

		public ServiceStorage(IMatcher matcher)
		{
			_matcher = matcher;
			_list = new List<ServiceEntry>();
			_namedDictionary = new ConcurrentDictionary<string, ServiceEntry>(StringComparer.OrdinalIgnoreCase);
		}
		#endregion

		#region 公共属性
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
		public virtual void Add(ServiceEntry entry)
		{
			if(entry == null)
				return;

			if(!string.IsNullOrWhiteSpace(entry.Name))
				_namedDictionary[entry.Name] = entry;

			_list.Add(entry);
		}

		public virtual ServiceEntry Remove(string name)
		{
			if(string.IsNullOrWhiteSpace(name))
				return null;

			ServiceEntry entry = null;

			if(_namedDictionary.TryRemove(name, out entry))
				_list.Remove(entry);

			return entry;
		}

		public virtual ServiceEntry Get(string name)
		{
			if(string.IsNullOrWhiteSpace(name))
				return null;

			name = name.Trim();
			ServiceEntry entry;

			if(_namedDictionary.TryGetValue(name, out entry))
				return entry;

			return null;
		}

		public virtual ServiceEntry Get(Type type, object parameter = null)
		{
			if(type == null)
				return null;

			var entries = new List<ServiceEntry>();

			foreach(var entry in _list)
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

			return null;
		}

		public virtual IEnumerable<ServiceEntry> GetAll(Type type, object parameter = null)
		{
			if(type == null)
				return null;

			var list = new List<ServiceEntry>();
			var result = new List<ServiceEntry>();

			foreach(var entry in _list)
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
	}
}

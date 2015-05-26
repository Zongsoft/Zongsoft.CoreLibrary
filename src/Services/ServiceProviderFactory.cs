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
using System.Collections.Concurrent;
using System.Linq;
using System.Text;

namespace Zongsoft.Services
{
	public class ServiceProviderFactory : MarshalByRefObject, IServiceProviderFactory, IEnumerable<KeyValuePair<string, IServiceProvider>>
	{
		#region 成员字段
		private string _defaultName;
		private ConcurrentDictionary<string, IServiceProvider> _providers;
		#endregion

		#region 构造函数
		public ServiceProviderFactory() : this(string.Empty)
		{
		}

		public ServiceProviderFactory(string defaultName)
		{
			_defaultName = string.IsNullOrWhiteSpace(defaultName) ? string.Empty : defaultName.Trim();
			_providers = new ConcurrentDictionary<string, IServiceProvider>(StringComparer.OrdinalIgnoreCase);
		}
		#endregion

		#region 公共属性
		public int Count
		{
			get
			{
				return _providers.Count;
			}
		}

		public virtual IServiceProvider Default
		{
			get
			{
				return this.GetProvider(_defaultName);
			}
			set
			{
				if(value == null)
					throw new ArgumentNullException();

				this.Register(_defaultName, value);
			}
		}
		#endregion

		#region 保护属性
		protected string DefaultName
		{
			get
			{
				return _defaultName;
			}
		}
		#endregion

		#region 公共方法
		public void Register(string name, IServiceProvider provider)
		{
			this.Register(name, provider, true);
		}

		public bool Register(string name, IServiceProvider provider, bool throwExceptionOnConflict)
		{
			if(provider == null)
				throw new ArgumentNullException("provider");

			name = string.IsNullOrWhiteSpace(name) ? string.Empty : name.Trim();

			//将服务提供程序描述项加入列表中
			if(!_providers.TryAdd(name, provider))
			{
				if(throwExceptionOnConflict)
					throw new ArgumentException();
				else
					return false;
			}

			//返回成功
			return true;
		}

		/// <summary>
		/// 注销服务供应程序。
		/// </summary>
		/// <param name="name">要注销服务供应程序的名称。</param>
		public void Unregister(string name)
		{
			IServiceProvider temp;
			_providers.TryRemove(string.IsNullOrWhiteSpace(name) ? string.Empty : name.Trim(), out temp);
		}

		/// <summary>
		/// 获取指定名称的服务供应程序。具体的获取策略请参考更详细的备注说明。
		/// </summary>
		/// <param name="name">待获取的服务供应程序名。</param>
		/// <returns>如果指定名称的供应程序回存在则返它，否则返回空(null)。</returns>
		public virtual IServiceProvider GetProvider(string name)
		{
			IServiceProvider result;

			if(_providers.TryGetValue(string.IsNullOrWhiteSpace(name) ? string.Empty : name.Trim(), out result))
				return result;

			return null;
		}
		#endregion

		#region 显式实现
		IEnumerator<KeyValuePair<string, IServiceProvider>> IEnumerable<KeyValuePair<string, IServiceProvider>>.GetEnumerator()
		{
			return _providers.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return _providers.GetEnumerator();
		}
		#endregion
	}
}

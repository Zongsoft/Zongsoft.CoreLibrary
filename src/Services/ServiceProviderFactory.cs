/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2010-2018 Zongsoft Corporation <http://www.zongsoft.com>
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
	public class ServiceProviderFactory : IServiceProviderFactory, ICollection<IServiceProvider>
	{
		#region 单例字段
		private static ServiceProviderFactory _instance;
		#endregion

		#region 成员字段
		private string _defaultName;
		private readonly IDictionary<string, IServiceProvider> _providers;
		#endregion

		#region 构造函数
		protected ServiceProviderFactory()
		{
			_defaultName = string.Empty;
			_providers = new Dictionary<string, IServiceProvider>(StringComparer.OrdinalIgnoreCase);
		}

		protected ServiceProviderFactory(string defaultName)
		{
			_defaultName = string.IsNullOrWhiteSpace(defaultName) ? string.Empty : defaultName.Trim();
			_providers = new Dictionary<string, IServiceProvider>(StringComparer.OrdinalIgnoreCase);
		}
		#endregion

		#region 单例属性
		public static ServiceProviderFactory Instance
		{
			get
			{
				if(_instance == null)
					System.Threading.Interlocked.CompareExchange(ref _instance, new ServiceProviderFactory(), null);

				return _instance;
			}
			set
			{
				_instance = value;
			}
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

				this.Register(value);

				//更新默认服务名字
				_defaultName = value.Name;
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
		public void Register(IServiceProvider provider)
		{
			if(provider == null)
				throw new ArgumentNullException(nameof(provider));

			_providers[provider.Name ?? string.Empty] = provider;
		}

		public void Register(IServiceProvider provider, bool replaceOnExists)
		{
			if(provider == null)
				throw new ArgumentNullException(nameof(provider));

			if(replaceOnExists)
				_providers[provider.Name ?? string.Empty] = provider;
			else
				_providers.Add(provider.Name ?? string.Empty, provider);
		}

		/// <summary>
		/// 注销服务供应程序。
		/// </summary>
		/// <param name="name">要注销服务供应程序的名称。</param>
		/// <returns>如果注销成功则返回真(True)，否则返回假(False)。</returns>
		public bool Unregister(string name)
		{
			return _providers.Remove(name ?? string.Empty);
		}

		/// <summary>
		/// 获取指定名称的服务供应程序。具体的获取策略请参考更详细的备注说明。
		/// </summary>
		/// <param name="name">待获取的服务供应程序名。</param>
		/// <returns>如果指定名称的供应程序回存在则返它，否则返回空(null)。</returns>
		public virtual IServiceProvider GetProvider(string name)
		{
			IServiceProvider result;

			if(_providers.TryGetValue(name ?? string.Empty, out result))
				return result;

			return null;
		}
		#endregion

		#region 显式实现
		bool ICollection<IServiceProvider>.IsReadOnly
		{
			get
			{
				return false;
			}
		}

		void ICollection<IServiceProvider>.Add(IServiceProvider item)
		{
			_providers.Add(item.Name ?? string.Empty, item);
		}

		void ICollection<IServiceProvider>.Clear()
		{
			_providers.Clear();
		}

		bool ICollection<IServiceProvider>.Contains(IServiceProvider item)
		{
			return _providers.ContainsKey(item.Name ?? string.Empty);
		}

		void ICollection<IServiceProvider>.CopyTo(IServiceProvider[] array, int arrayIndex)
		{
			_providers.Values.CopyTo(array, arrayIndex);
		}

		bool ICollection<IServiceProvider>.Remove(IServiceProvider item)
		{
			return _providers.Remove(item.Name ?? string.Empty);
		}

		IEnumerator<IServiceProvider> IEnumerable<IServiceProvider>.GetEnumerator()
		{
			return _providers.Values.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return _providers.Values.GetEnumerator();
		}
		#endregion
	}
}

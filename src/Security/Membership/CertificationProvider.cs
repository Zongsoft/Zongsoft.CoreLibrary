/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2003-2015 Zongsoft Corporation <http://www.zongsoft.com>
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

namespace Zongsoft.Security.Membership
{
	public class CertificationProvider : ICertificationProvider
	{
		#region 私有常量
		private const string DefaultCacheName = "Zongsoft.Security.Membership.Certification";
		#endregion

		#region 成员字段
		private Zongsoft.Runtime.Caching.ICache _cache;
		private Zongsoft.Runtime.Caching.ICacheProvider _cacheProvider;
		#endregion

		#region 构造函数
		public CertificationProvider()
		{
		}

		public CertificationProvider(Zongsoft.Runtime.Caching.ICache cache)
		{
			if(cache == null)
				throw new ArgumentNullException("cache");

			_cache = cache;
		}

		public CertificationProvider(Zongsoft.Runtime.Caching.ICacheProvider cacheProvider)
		{
			if(cacheProvider == null)
				throw new ArgumentNullException("cacheProvider");

			_cacheProvider = cacheProvider;
		}
		#endregion

		#region 公共属性
		public Zongsoft.Runtime.Caching.ICache Cache
		{
			get
			{
				if(_cache == null)
				{
					var cacheProvider = _cacheProvider;

					if(cacheProvider != null)
						System.Threading.Interlocked.CompareExchange(ref _cache, cacheProvider.GetCache(DefaultCacheName), null);
				}

				return _cache;
			}
			set
			{
				_cache = value;
			}
		}

		public Zongsoft.Runtime.Caching.ICacheProvider CacheProvider
		{
			get
			{
				return _cacheProvider;
			}
			set
			{
				_cacheProvider = value;
			}
		}
		#endregion

		#region 公共方法
		public Certification Register(int userId, string @namespace, IDictionary<string, object> extendedProperties)
		{
			var certification = this.CreateCertification(userId, @namespace, extendedProperties);
			this.Register(certification);
			return certification;
		}

		public void Unregister(string certificationId)
		{
			var cache = this.Cache;

			if(cache != null)
			{
				string applicationId = cache.GetValue(certificationId) as string;
				cache.Remove(certificationId);

				cache = _cacheProvider.GetCache(applicationId);
				if(cache != null)
					cache.Remove(certificationId);
			}
		}

		public void Renew(string certificationId)
		{
			this.Renew(certificationId, new TimeSpan(8, 0, 0));
		}

		public void Renew(string certificationId, TimeSpan duration)
		{
			var certification = this.GetCertification(certificationId);

			if(certification != null)
			{
				certification.Expires += duration;

				var cache = _cacheProvider.GetCache(certification.Namespace);
				if(cache != null)
					cache.SetValue(certificationId, certification);
			}
		}

		public int GetCount()
		{
			return this.GetCount(null);
		}

		public int GetCount(string @namespace)
		{
			Zongsoft.Runtime.Caching.ICache cache;

			if(string.IsNullOrWhiteSpace(@namespace))
				cache = _cacheProvider.GetCache(DefaultCacheName);
			else
				cache = _cacheProvider.GetCache(@namespace);

			if(cache == null)
				return 0;

			return (int)cache.Count;
		}

		public void Validate(string certificationId)
		{
			var certification = this.GetCertification(certificationId);

			if(certification == null)
				throw new CertificationException(certificationId, "The certification is not exists.");

			if(DateTime.Now > certification.Expires)
				throw new CertificationException(certificationId, "The certification was expired.");
		}

		public string GetNamespace(string certificationId)
		{
			if(string.IsNullOrWhiteSpace(certificationId))
				throw new CertificationException("Not specified certification id.");

			var cache = _cacheProvider.GetCache(DefaultCacheName);

			if(cache == null)
				throw new InvalidOperationException("Can not obtain the certification cache provider.");

			var applicationId = cache.GetValue(certificationId) as string;

			if(applicationId == null)
				throw new CertificationException("Invalid certification id.");

			return applicationId;
		}

		public Certification GetCertification(string certificationId)
		{
			var applicationId = this.GetNamespace(certificationId);

			var cache = _cacheProvider.GetCache(applicationId);

			if(cache != null)
				return cache.GetValue(certificationId) as Certification;

			return null;
		}

		public IEnumerable<Certification> GetCertifications(string @namespace)
		{
			return _cacheProvider.GetCache(@namespace) as IEnumerable<Certification>;
		}

		public IEnumerable<Certification> GetCertifications(int userId)
		{
			return _cacheProvider.GetCache(userId.ToString()) as IEnumerable<Certification>;
		}
		#endregion

		#region 虚拟方法
		protected virtual Certification CreateCertification(int userId, string @namespace, IDictionary<string, object> extendedProperties)
		{
			return new Certification(Guid.NewGuid().ToString("D"), @namespace, userId, DateTime.Now.AddDays(1));
		}

		protected virtual void Register(Certification certification)
		{
			if(_cacheProvider == null)
				throw new InvalidOperationException("The value of 'Cache' property is null.");

			var masterCache = _cacheProvider.GetCache(DefaultCacheName);

			if(masterCache == null)
				throw new InvalidOperationException("Can not obtain the certification cache provider.");

			//在默认缓存字典中添加一条记录
			masterCache.SetValue(certification.CertificationId, certification.Namespace);

			try
			{
				//获取当前安全凭证所属应用的缓存字典
				var slaverCache = _cacheProvider.GetCache(certification.Namespace);

				if(slaverCache == null)
				{
					//将默认缓存字典中刚添加的记录删除掉
					masterCache.Remove(certification.CertificationId);
					throw new CertificationException(string.Format("Can not obtain the certification cache for '{0}' application.", certification.Namespace));
				}

				//在当前应用缓存容器中添加一条记录
				slaverCache.SetValue(certification.CertificationId, certification);
			}
			catch
			{
				masterCache.Remove(certification.CertificationId);
			}
		}
		#endregion
	}
}

/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2003-2014 Zongsoft Corporation <http://www.zongsoft.com>
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
		private Zongsoft.Runtime.Caching.IDictionaryCacheProvider _cache;
		#endregion

		#region 构造函数
		public CertificationProvider()
		{
		}

		public CertificationProvider(Zongsoft.Runtime.Caching.IDictionaryCacheProvider cache)
		{
			if(cache == null)
				throw new ArgumentNullException("cache");

			_cache = cache;
		}
		#endregion

		#region 公共属性
		public Zongsoft.Runtime.Caching.IDictionaryCacheProvider Cache
		{
			get
			{
				return _cache;
			}
			set
			{
				if(value == null)
					throw new ArgumentNullException();

				_cache = value;
			}
		}
		#endregion

		#region 公共方法
		public Certification Register(string applicationId, string userName)
		{
			var certification = this.CreateCertification(applicationId, userName);
			this.Register(certification);
			return certification;
		}

		public void Unregister(string certificationId)
		{
			var cache = _cache.GetDictionaryCache(DefaultCacheName);

			if(cache != null)
			{
				string applicationId = cache.GetValue(certificationId) as string;
				cache.Remove(certificationId);

				cache = _cache.GetDictionaryCache(applicationId);
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

				var cache = _cache.GetDictionaryCache(certification.ApplicationId);
				if(cache != null)
					cache.SetValue(certificationId, certification);
			}
		}

		public int GetCount()
		{
			return this.GetCount(null);
		}

		public int GetCount(string applicationId)
		{
			Zongsoft.Runtime.Caching.IDictionaryCache cache;

			if(applicationId == null)
				cache = _cache.GetDictionaryCache(DefaultCacheName);
			else
				cache = _cache.GetDictionaryCache(applicationId);

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

		public string GetApplicationId(string certificationId)
		{
			if(string.IsNullOrWhiteSpace(certificationId))
				throw new CertificationException("Not specified certification id.");

			var cache = _cache.GetDictionaryCache(DefaultCacheName);

			if(cache == null)
				throw new InvalidOperationException("Can not obtain the certification cache provider.");

			var applicationId = cache.GetValue(certificationId) as string;

			if(applicationId == null)
				throw new CertificationException("Invalid certification id.");

			return applicationId;
		}

		public Certification GetCertification(string certificationId)
		{
			var applicationId = this.GetApplicationId(certificationId);

			var cache = _cache.GetDictionaryCache(applicationId);

			if(cache != null)
				return cache.GetValue(certificationId) as Certification;

			return null;
		}

		public IEnumerable<Certification> GetCertifications(string applicationId)
		{
			return _cache.GetDictionaryCache(applicationId) as IEnumerable<Certification>;
		}

		public IEnumerable<Certification> GetCertifications(string applicationId, string userName)
		{
			var certifications = this.GetCertifications(applicationId);

			if(certifications == null)
				return Enumerable.Empty<Certification>();

			return certifications.Where(certification => string.Equals(certification.UserName, userName, StringComparison.OrdinalIgnoreCase));
		}
		#endregion

		#region 虚拟方法
		protected virtual Certification CreateCertification(string applicationId, string userName)
		{
			return new Certification(Guid.NewGuid().ToString("D"), applicationId, userName, DateTime.Now.AddDays(1));
		}

		protected virtual void Register(Certification certification)
		{
			if(_cache == null)
				throw new InvalidOperationException("The value of 'Cache' property is null.");

			var masterCache = _cache.GetDictionaryCache(DefaultCacheName);

			if(masterCache == null)
				throw new InvalidOperationException("Can not obtain the certification cache provider.");

			//在默认缓存字典中添加一条记录
			masterCache.SetValue(certification.CertificationId, certification.ApplicationId);

			try
			{
				//获取当前安全凭证所属应用的缓存字典
				var slaverCache = _cache.GetDictionaryCache(certification.ApplicationId);

				if(slaverCache == null)
				{
					//将默认缓存字典中刚添加的记录删除掉
					masterCache.Remove(certification.CertificationId);
					throw new CertificationException(string.Format("Can not obtain the certification cache for '{0}' application.", certification.ApplicationId));
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

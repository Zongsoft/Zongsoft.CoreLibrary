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

namespace Zongsoft.Security.Membership
{
	/// <summary>
	/// 表示安全凭证的实体类。
	/// </summary>
	[Serializable]
	public class Certification
	{
		#region 成员字段
		private string _certificationId;
		private string _applicationId;
		private string _userName;
		private DateTime _expires;
		private DateTime _createdTime;
		private IList<DateTime> _slidingList;
		private IDictionary<string, object> _extendedProperties;
		#endregion

		#region 构造函数
		public Certification(string certificationId, string applicationId, string userName, DateTime expires) : this(certificationId, applicationId, userName, expires, DateTime.Now)
		{
		}

		public Certification(string certificationId, string applicationId, string userName, DateTime expires, DateTime createdTime)
		{
			if(string.IsNullOrWhiteSpace(certificationId))
				throw new ArgumentNullException("certificationId");

			if(string.IsNullOrWhiteSpace(applicationId))
				throw new ArgumentNullException("applicationId");

			if(string.IsNullOrWhiteSpace(userName))
				throw new ArgumentNullException("userName");

			_certificationId = certificationId.Trim();
			_applicationId = applicationId.Trim();
			_userName = userName.Trim();
			_expires = expires;
			_createdTime = createdTime;
			_slidingList = new List<DateTime>();
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取安全凭证编号。
		/// </summary>
		public string CertificationId
		{
			get
			{
				return _certificationId;
			}
		}

		/// <summary>
		/// 获取安全凭证的所属应用编号。
		/// </summary>
		public string ApplicationId
		{
			get
			{
				return _applicationId;
			}
		}

		/// <summary>
		/// 获取安全凭证对应的用户名称。
		/// </summary>
		public string UserName
		{
			get
			{
				return _userName;
			}
		}

		/// <summary>
		/// 获取或设置安全凭证的过期时间。
		/// </summary>
		public DateTime Expires
		{
			get
			{
				return _expires;
			}
			set
			{
				_expires = value;
			}
		}

		/// <summary>
		/// 获取安全凭证的创建时间。
		/// </summary>
		public DateTime CreatedTime
		{
			get
			{
				return _createdTime;
			}
		}

		/// <summary>
		/// 获取安全凭证的滑动记录列表。
		/// </summary>
		/// <remarks>
		///		<para>每续约一次，就会在滑动记录列表中追加一条记录。</para>
		/// </remarks>
		public IList<DateTime> SlidingList
		{
			get
			{
				return _slidingList;
			}
		}

		/// <summary>
		/// 获取安全凭证的扩展属性。
		/// </summary>
		public IDictionary<string, object> ExtendedProperties
		{
			get
			{
				if(_extendedProperties == null)
					System.Threading.Interlocked.CompareExchange(ref _extendedProperties, new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase), null);

				return _extendedProperties;
			}
		}
		#endregion

		#region 重写方法
		public override bool Equals(object obj)
		{
			if(obj == null || obj.GetType() != this.GetType())
				return false;

			var state = (Certification)obj;

			return string.Equals(_certificationId, state._certificationId, StringComparison.OrdinalIgnoreCase) &&
				   string.Equals(_applicationId, state._applicationId, StringComparison.OrdinalIgnoreCase) &&
				   string.Equals(_userName, state._userName, StringComparison.OrdinalIgnoreCase);
		}

		public override int GetHashCode()
		{
			return (_applicationId + ":" + _userName + ":" + _certificationId).ToLowerInvariant().GetHashCode();
		}

		public override string ToString()
		{
			return string.Format("[{0}]{1}@{2}", _certificationId, _userName, _applicationId);
		}
		#endregion
	}
}

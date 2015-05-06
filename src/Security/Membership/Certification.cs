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
		private string _namespace;
		private int _userId;
		private DateTime _expires;
		private DateTime _issuedTime;
		private IReadOnlyList<DateTime> _slidingList;
		private IDictionary<string, object> _extendedProperties;
		#endregion

		#region 构造函数
		public Certification(string certificationId, string @namespace, int userId, DateTime expires) : this(certificationId, @namespace, userId, expires, DateTime.Now)
		{
		}

		public Certification(string certificationId, string @namespace, int userId, DateTime expires, DateTime issuedTime)
		{
			if(string.IsNullOrWhiteSpace(certificationId))
				throw new ArgumentNullException("certificationId");

			if(string.IsNullOrWhiteSpace(@namespace))
				throw new ArgumentNullException("namespace");

			_certificationId = certificationId.Trim();
			_namespace = @namespace.Trim();
			_userId = userId;
			_expires = expires;
			_issuedTime = issuedTime;
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
		/// 获取安全凭证所属的命令空间。
		/// </summary>
		public string Namespace
		{
			get
			{
				return _namespace;
			}
		}

		/// <summary>
		/// 获取安全凭证对应的用户编号。
		/// </summary>
		public int UserId
		{
			get
			{
				return _userId;
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
		/// 获取安全凭证的签发时间。
		/// </summary>
		public DateTime IssuedTime
		{
			get
			{
				return _issuedTime;
			}
		}

		/// <summary>
		/// 获取安全凭证的滑动记录列表。
		/// </summary>
		/// <remarks>
		///		<para>每续约一次，就会在滑动记录列表中追加一条记录。</para>
		/// </remarks>
		public IReadOnlyList<DateTime> SlidingList
		{
			get
			{
				return _slidingList;
			}
			internal set
			{
				_slidingList = value;
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

			var other = (Certification)obj;

			return string.Equals(_certificationId, other._certificationId, StringComparison.OrdinalIgnoreCase) &&
				   string.Equals(_namespace, other._namespace, StringComparison.OrdinalIgnoreCase) &&
				   _userId == other._userId;
		}

		public override int GetHashCode()
		{
			return (_certificationId + ":" + _userId.ToString()).ToLowerInvariant().GetHashCode();
		}

		public override string ToString()
		{
			if(string.IsNullOrWhiteSpace(_namespace))
				return string.Format("[{0}] {1}", _certificationId, _userId);
			else
				return string.Format("[{0}] {1}@{2}", _certificationId, _userId, _namespace);
		}
		#endregion
	}
}

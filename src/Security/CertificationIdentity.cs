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
using System.Security.Principal;

namespace Zongsoft.Security
{
	/// <summary>
	/// 表示一般用户的标识类。
	/// </summary>
	public class CertificationIdentity : MarshalByRefObject, IIdentity
	{
		#region 公共字段
		public static readonly CertificationIdentity Empty = new CertificationIdentity();
		#endregion

		#region 成员字段
		private string _certificationId;
		private Certification _certification;
		private ICertificationProvider _provider;
		#endregion

		#region 构造函数
		private CertificationIdentity()
		{
			_certificationId = string.Empty;
		}

		public CertificationIdentity(string certificationId, ICertificationProvider provider = null)
		{
			if(string.IsNullOrWhiteSpace(certificationId))
				throw new ArgumentNullException("certificationId");

			_certificationId = certificationId;
			_provider = provider;
		}
		#endregion

		#region 公共属性
		public string Name
		{
			get
			{
				var certification = this.Certification;

				if(certification == null || certification.User == null)
					return string.Empty;

				return certification.User.Name;
			}
		}

		public bool IsAuthenticated
		{
			get
			{
				return !string.IsNullOrWhiteSpace(_certificationId);
			}
		}

		public string CertificationId
		{
			get
			{
				return _certificationId;
			}
		}

		public virtual Certification Certification
		{
			[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.Synchronized)]
			get
			{
				if(string.IsNullOrWhiteSpace(_certificationId))
					return null;

				if(_certification == null)
				{
					var provider = this.Provider;

					if(provider == null)
						return null;

					_certification = provider.GetCertification(_certificationId);
				}

				return _certification;
			}
		}

		public ICertificationProvider Provider
		{
			get
			{
				return _provider;
			}
			set
			{
				if(value == null)
					throw new ArgumentNullException();

				_provider = value;
			}
		}
		#endregion

		#region 显式实现
		string IIdentity.AuthenticationType
		{
			get
			{
				return "Zongsoft Certification Authentication System";
			}
		}
		#endregion
	}
}

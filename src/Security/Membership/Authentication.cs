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
using System.Security.Cryptography;

using Zongsoft.Data;

namespace Zongsoft.Security.Membership
{
	public class Authentication : IAuthentication
	{
		#region 成员字段
		private string _namespace;
		private IDataAccess _dataAccess;
		private ICertificationProvider _certificationProvider;
		#endregion

		#region 构造函数
		public Authentication()
		{
		}

		public Authentication(IDataAccess objectAccess, ICertificationProvider certificationProvider)
		{
			if(objectAccess == null)
				throw new ArgumentNullException("objectAccess");

			if(certificationProvider == null)
				throw new ArgumentNullException("certificationProvider");

			_dataAccess = objectAccess;
			_certificationProvider = certificationProvider;
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取或设置所属的命名空间。
		/// </summary>
		public string Namespace
		{
			get
			{
				return _namespace;
			}
			set
			{
				_namespace = value;
			}
		}

		/// <summary>
		/// 获取或设置<see cref="Zongsoft.Data.IDataAccess"/>数据访问对象。
		/// </summary>
		public IDataAccess DataAccess
		{
			get
			{
				return _dataAccess;
			}
			set
			{
				if(value == null)
					throw new ArgumentNullException();

				_dataAccess = value;
			}
		}

		/// <summary>
		/// 获取或设置<see cref="ICertificationProvider"/>安全凭证提供程序。
		/// </summary>
		public ICertificationProvider CertificationProvider
		{
			get
			{
				return _certificationProvider;
			}
			set
			{
				if(value == null)
					throw new ArgumentNullException();

				_certificationProvider = value;
			}
		}
		#endregion

		#region 验证方法
		public AuthenticationResult Authenticate(string identity, string password)
		{
			if(_dataAccess == null)
				throw new InvalidOperationException("The value of 'ObjectAccess' property is null.");

			if(_certificationProvider == null)
				throw new InvalidOperationException("The value of 'CertificationProvider' property is null.");

			if(string.IsNullOrWhiteSpace(identity))
				throw new ArgumentNullException("identity");

			byte[] storedPassword;
			byte[] storedPasswordSalt;

			//获取当前用户的密码及密码盐
			var user = this.GetPassword(identity, out storedPassword, out storedPasswordSalt);

			if(user != null)
			{
				if(PasswordUtility.VerifyPassword(password, storedPassword, storedPasswordSalt, "SHA1"))
				{
					//通过安全凭证提供程序注册凭证
					var certification = _certificationProvider.Register(user.UserId, user.Namespace, new Dictionary<string, object>()
					{
						{"Name", user.Name},
						{"FullName", user.FullName},
						{"Email", user.Email},
						{"PhoneNumber", user.PhoneNumber},
						{"Principal", user.Principal},
						{"Approved", user.Approved},
						{"Suspended", user.Suspended}
					});

					//如果安全凭证注册失败则抛出异常
					if(certification == null)
						throw new AuthenticationException(string.Format("Register certification faild for '{0}'.", identity));

					//验证成功，返回验证结果
					return new AuthenticationResult(certification.CertificationId, certification.Expires);
				}

				//密码校验失败则抛出验证异常
				throw new AuthenticationException("Invalid password.");
			}

			//指定的用户名如果不存在则抛出验证异常
			throw new AuthenticationException("Invalid account.");
		}
		#endregion

		#region 虚拟方法
		protected virtual User GetPassword(string identity, out byte[] password, out byte[] passwordSalt)
		{
			var identityType = MembershipHelper.GetUserIdentityType(identity);

			switch(identityType)
			{
				case UserIdentityType.Email:
					return MembershipHelper.GetPasswordByEmail(_dataAccess, _namespace, identity, out password, out passwordSalt);
				case UserIdentityType.PhoneNumber:
					return MembershipHelper.GetPasswordByPhoneNumber(_dataAccess, _namespace, identity, out password, out passwordSalt);
				default:
					return MembershipHelper.GetPasswordByUserName(_dataAccess, _namespace, identity, out password, out passwordSalt);
			}
		}
		#endregion
	}
}

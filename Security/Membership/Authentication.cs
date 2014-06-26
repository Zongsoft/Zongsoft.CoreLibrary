using System;
using System.Collections.Generic;
using System.Security.Cryptography;

using Zongsoft.Data;
using Zongsoft.Data.Entities;

namespace Zongsoft.Security.Membership
{
	public class Authentication : IAuthentication
	{
		#region 成员字段
		private IObjectAccess _objectAccess;
		private ICertificationProvider _certificationProvider;
		#endregion

		#region 构造函数
		public Authentication()
		{
		}

		public Authentication(IObjectAccess objectAccess, ICertificationProvider certificationProvider)
		{
			if(objectAccess == null)
				throw new ArgumentNullException("objectAccess");

			if(certificationProvider == null)
				throw new ArgumentNullException("certificationProvider");

			_objectAccess = objectAccess;
			_certificationProvider = certificationProvider;
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取或设置<see cref="Zongsoft.Data.Entities.IObjectAccess"/>数据访问对象。
		/// </summary>
		public IObjectAccess ObjectAccess
		{
			get
			{
				return _objectAccess;
			}
			set
			{
				if(value == null)
					throw new ArgumentNullException();

				_objectAccess = value;
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
		public AuthenticationResult Authenticate(string applicationId, string userName, string password)
		{
			if(_objectAccess == null)
				throw new InvalidOperationException("The value of 'ObjectAccess' property is null.");

			if(_certificationProvider == null)
				throw new InvalidOperationException("The value of 'CertificationProvider' property is null.");

			if(string.IsNullOrWhiteSpace(applicationId))
				throw new ArgumentNullException("applicationId");

			if(string.IsNullOrWhiteSpace(userName))
				throw new ArgumentNullException("userName");

			byte[] storedPassword;
			byte[] storedPasswordSalt;

			//获取当前用户的密码及密码盐
			if(this.GetPassword(applicationId, userName, out storedPassword, out storedPasswordSalt))
			{
				if(PasswordUtility.VerifyPassword(password, storedPassword, storedPasswordSalt, "SHA1"))
				{
					//通过安全凭证提供程序注册凭证
					var certification = _certificationProvider.Register(applicationId, userName);

					//如果安全凭证注册失败则抛出异常
					if(certification == null)
						throw new AuthenticationException(string.Format("Register certification faild for '{0}'.", userName));

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
		protected virtual bool GetPassword(string applicationId, string userName, out byte[] password, out byte[] passwordSalt)
		{
			return MembershipHelper.GetPassword(_objectAccess, applicationId, userName, out password, out passwordSalt);
		}
		#endregion
	}
}

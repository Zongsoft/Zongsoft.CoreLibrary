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
using Zongsoft.Options;

namespace Zongsoft.Security.Membership
{
	public class Authentication : ProviderBase, IAuthentication
	{
		#region 事件声明
		public event EventHandler<AuthenticatedEventArgs> Authenticated;
		#endregion

		#region 构造函数
		public Authentication(ISettingsProvider settings) : base(settings)
		{
		}
		#endregion

		#region 验证方法
		public AuthenticationResult Authenticate(string identity, string password, string scene)
		{
			if(string.IsNullOrWhiteSpace(identity))
				throw new ArgumentNullException("identity");

			byte[] storedPassword;
			byte[] storedPasswordSalt;

			//获取当前用户的密码及密码向量
			var userId = this.GetPassword(identity, out storedPassword, out storedPasswordSalt);

			if(userId == null)
			{
				//激发“Authenticated”事件
				this.OnAuthenticated(new AuthenticatedEventArgs(identity, this.Namespace, scene, false));

				//指定的用户名如果不存在则抛出验证异常
				throw new AuthenticationException(AuthenticationException.InvalidIdentity);
			}

			//如果验证成功，则返回验证结果
			if(PasswordUtility.VerifyPassword(password, storedPassword, storedPasswordSalt, "SHA1"))
			{
				//获取指定用户编号对应的用户对象
				var user = MembershipHelper.GetUser(this.EnsureDataAccess(), userId.Value);

				//创建“Authenticated”事件参数
				var eventArgs = new AuthenticatedEventArgs(identity, this.Namespace, scene, true, user);

				//激发“Authenticated”事件
				this.OnAuthenticated(eventArgs);

				//返回成功的验证结果
				return new AuthenticationResult(eventArgs.User ?? user);
			}

			//激发“Authenticated”事件
			this.OnAuthenticated(new AuthenticatedEventArgs(identity, this.Namespace, scene, false));

			//密码校验失败则抛出验证异常
			throw new AuthenticationException(AuthenticationException.InvalidPassword);
		}
		#endregion

		#region 虚拟方法
		protected virtual int? GetPassword(string identity, out byte[] password, out byte[] passwordSalt)
		{
			#region 测试代码
			password = null;
			passwordSalt = null;
			return 1;
			#endregion

			if(string.IsNullOrWhiteSpace(identity))
				throw new ArgumentNullException("identity");

			var identityType = MembershipHelper.GetUserIdentityType(identity);

			switch(identityType)
			{
				case UserIdentityType.Email:
					return MembershipHelper.GetPasswordByEmail(this.EnsureDataAccess(), this.Namespace, identity, out password, out passwordSalt);
				case UserIdentityType.PhoneNumber:
					return MembershipHelper.GetPasswordByPhone(this.EnsureDataAccess(), this.Namespace, identity, out password, out passwordSalt);
				default:
					return MembershipHelper.GetPasswordByName(this.EnsureDataAccess(), this.Namespace, identity, out password, out passwordSalt);
			}
		}
		#endregion

		#region 激发事件
		protected virtual void OnAuthenticated(AuthenticatedEventArgs args)
		{
			var handler = this.Authenticated;

			if(handler != null)
				handler(this, args);
		}
		#endregion
	}
}

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
using System.Linq;
using System.Text;

using Zongsoft.Data;

namespace Zongsoft.Security.Membership
{
	public class UserProvider : IUserProvider
	{
		#region 成员字段
		private IDataAccess _dataAccess;
		private ICertificationProvider _certificationProvider;
		#endregion

		#region 公共属性
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

		#region 用户管理
		public User GetUser(string certificationId, int userId)
		{
			var objectAccess = this.GetDataAccess();

			return objectAccess.Select<User>("Security.User", new ConditionCollection(ConditionCombine.And)
			{
				new Condition("UserId", userId),
			}).FirstOrDefault();
		}

		public User GetUser(string certificationId, string identity)
		{
			ICondition condition = null;
			var identityType = MembershipHelper.GetUserIdentityType(identity);

			switch(identityType)
			{
				case UserIdentityType.Email:
					condition = new Condition("Email", identity);
					break;
				case UserIdentityType.PhoneNumber:
					condition = new Condition("PhoneNumber", identity);
					break;
				default:
					condition = new Condition("Name", identity);
					break;
			}

			var objectAccess = this.GetDataAccess();

			return objectAccess.Select<User>("Security.User", condition).FirstOrDefault();
		}

		public IEnumerable<User> GetAllUsers(string certificationId)
		{
			var dataAccess = this.GetDataAccess();
			var @namespace = this.GetNamespace(certificationId);

			if(string.IsNullOrWhiteSpace(@namespace))
				return dataAccess.Select<User>("Security.User");
			else
				return dataAccess.Select<User>("Security.User", new Condition("Namespace", @namespace));
		}

		public int DeleteUsers(string certificationId, params int[] userIds)
		{
			if(userIds == null || userIds.Length < 1)
				return 0;

			var dataAccess = this.GetDataAccess();

			return dataAccess.Delete("Security.User", new Condition("UserId", userIds, ConditionOperator.In));
		}

		public void CreateUsers(string certificationId, IEnumerable<User> users)
		{
			if(users == null)
				return;

			var objectAccess = this.GetDataAccess();
			objectAccess.Insert("Security.User", users);
		}

		public void UpdateUsers(string certificationId, IEnumerable<User> users)
		{
			if(users == null)
				return;

			var objectAccess = this.GetDataAccess();

			foreach(var user in users)
			{
				if(user == null)
					continue;

				objectAccess.Update("Security.User", user, new ConditionCollection(ConditionCombine.And)
				{
					new Condition("UserId", user.UserId),
				});
			}
		}
		#endregion

		#region 密码管理
		public void ChangePassword(string certificationId, string oldPassword, string newPassword)
		{
			var dataAccess = this.GetDataAccess();
			var certification = this.GetCertification(certificationId);

			byte[] storedPassword;
			byte[] storedPasswordSalt;

			var user = MembershipHelper.GetPassword(dataAccess, certification.UserId, out storedPassword, out storedPasswordSalt);

			if(user == null)
				throw new InvalidOperationException("Invalid account.");

			if(!PasswordUtility.VerifyPassword(oldPassword, storedPassword, storedPasswordSalt))
				throw new AuthenticationException("Invalid password.");

			//重新生成密码随机数
			storedPasswordSalt = PasswordUtility.GeneratePasswordSalt(4);

			dataAccess.Execute("Security.User.SetPassword", new Dictionary<string, object>
			{
				{"UserId", certification.UserId},
				{"Password", PasswordUtility.HashPassword(newPassword, storedPasswordSalt)},
				{"PasswordSalt", storedPasswordSalt},
			});
		}

		public string ResetPassword(string certificationId, string passwordAnswer)
		{
			var objectAccess = this.GetDataAccess();
			var certification = this.GetCertification(certificationId);

			IDictionary<string, object> outParameters;

			objectAccess.Execute("Security.User.GetPasswordAnswer", new Dictionary<string, object>
			{
				{"UserId", certification.UserId},
			}, out outParameters);

			object answerValue;

			if(!outParameters.TryGetValue("PasswordAnswer", out answerValue))
				throw new InvalidOperationException("Can not obtain the Password-Answer.");

			if(!Zongsoft.Collections.BinaryComparer.Default.Equals(PasswordUtility.HashPassword(passwordAnswer), (byte[])answerValue))
				throw new AuthenticationException("Invalid value of the Password-Answer.");

			var password = PasswordUtility.GeneratePassword();
			var passwordSalt = PasswordUtility.GeneratePasswordSalt(4);

			objectAccess.Execute("Security.User.SetPassword", new Dictionary<string, object>
			{
				{"UserId", certification.UserId},
				{"Password", PasswordUtility.HashPassword(password, passwordSalt)},
				{"PasswordSalt", passwordSalt},
			});

			return password;
		}

		public string GetPasswordQuestion(string certificationId)
		{
			var objectAccess = this.GetDataAccess();
			var certification = this.GetCertification(certificationId);

			IDictionary<string, object> outParameters;

			objectAccess.Execute("Security.User.GetPasswordQuestion", new Dictionary<string, object>
			{
				{"UserId", certification.UserId},
			}, out outParameters);

			object questionValue;

			if(outParameters.TryGetValue("PasswordQuestion", out questionValue))
				return questionValue as string;

			return null;
		}

		public void SetPasswordQuestionAndAnswer(string certificationId, string password, string passwordQuestion, string passwordAnswer)
		{
			var objectAccess = this.GetDataAccess();
			var certification = this.GetCertification(certificationId);

			byte[] storedPassword;
			byte[] storedPasswordSalt;

			var user = MembershipHelper.GetPassword(objectAccess, certification.UserId, out storedPassword, out storedPasswordSalt);

			if(user == null)
				throw new InvalidOperationException("Invalid account.");

			if(!PasswordUtility.VerifyPassword(password, storedPassword, storedPasswordSalt))
				throw new AuthenticationException("Invalid password.");

			objectAccess.Execute("Security.User.SetPasswordQuestionAndAnswer", new Dictionary<string, object>
			{
				{"ApplicationId", certification.Namespace},
				{"Name", certification.UserId},
				{"PasswordQuestion", passwordQuestion},
				{"PasswordAnswer", PasswordUtility.HashPassword(passwordAnswer)},
			});
		}
		#endregion

		#region 私有方法
		private IDataAccess GetDataAccess()
		{
			if(_dataAccess == null)
				throw new InvalidOperationException("The value of 'DataAccess' property is null.");

			return _dataAccess;
		}

		private Certification GetCertification(string certificationId)
		{
			var certificationProvider = _certificationProvider;

			if(certificationProvider == null)
				throw new InvalidOperationException("The value of 'CertificationProvider' property is null.");

			return certificationProvider.GetCertification(certificationId);
		}

		private string GetNamespace(string certificationId)
		{
			var certificationProvider = _certificationProvider;

			if(certificationProvider == null)
				throw new InvalidOperationException("The value of 'CertificationProvider' property is null.");

			return certificationProvider.GetNamespace(certificationId);
		}
		#endregion
	}
}

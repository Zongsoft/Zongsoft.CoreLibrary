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
using Zongsoft.Options;

namespace Zongsoft.Security.Membership
{
	public class UserProvider : ProviderBase, IUserProvider
	{
		#region 构造函数
		public UserProvider(ISettingsProvider settings) : base(settings)
		{
		}
		#endregion

		#region 用户管理
		public User GetUser(int userId)
		{
			var dataAccess = this.EnsureDataAccess();
			return MembershipHelper.GetUser(dataAccess, userId);
		}

		public User GetUser(string identity)
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

			var objectAccess = this.EnsureDataAccess();

			return objectAccess.Select<User>("Security.User", condition).FirstOrDefault();
		}

		public IEnumerable<User> GetAllUsers()
		{
			var dataAccess = this.EnsureDataAccess();

			if(string.IsNullOrWhiteSpace(this.Namespace))
				return dataAccess.Select<User>("Security.User");
			else
				return dataAccess.Select<User>("Security.User", new Condition("Namespace", this.Namespace));
		}

		public bool SetPrincipal(int userId, string principal)
		{
			var dataAccess = this.EnsureDataAccess();

			return dataAccess.Update("Security.User", new { Principal = principal }, new ConditionCollection(ConditionCombine.And)
			{
				new Condition("UserId", userId),
			}) > 0;
		}

		public int DeleteUsers(params int[] userIds)
		{
			if(userIds == null || userIds.Length < 1)
				return 0;

			var dataAccess = this.EnsureDataAccess();

			return dataAccess.Delete("Security.User", new Condition("UserId", userIds, ConditionOperator.In));
		}

		public void CreateUsers(IEnumerable<User> users)
		{
			if(users == null)
				return;

			var objectAccess = this.EnsureDataAccess();
			objectAccess.Insert("Security.User", users);
		}

		public void UpdateUsers(IEnumerable<User> users)
		{
			if(users == null)
				return;

			var objectAccess = this.EnsureDataAccess();
			objectAccess.Update("Security.User", users);
		}
		#endregion

		#region 密码管理
		public bool ChangePassword(int userId, string oldPassword, string newPassword)
		{
			var dataAccess = this.EnsureDataAccess();

			byte[] storedPassword;
			byte[] storedPasswordSalt;

			if(!MembershipHelper.GetPassword(dataAccess, userId, out storedPassword, out storedPasswordSalt))
				return false;

			if(!PasswordUtility.VerifyPassword(oldPassword, storedPassword, storedPasswordSalt))
				throw new AuthenticationException("Invalid password.");

			//重新生成密码随机数
			storedPasswordSalt = PasswordUtility.GeneratePasswordSalt(4);

			dataAccess.Execute("Security.User.SetPassword", new Dictionary<string, object>
			{
				{"UserId", userId},
				{"Password", PasswordUtility.HashPassword(newPassword, storedPasswordSalt)},
				{"PasswordSalt", storedPasswordSalt},
			});

			return true;
		}

		public bool ForgetPassword(string identity, out int userId, out string secret, out string token)
		{
			userId = 0;
			secret = null;
			token = null;

			return false;
		}

		public bool ResetPassword(int userId, string token, string newPassword = null)
		{
			return false;
		}

		public bool ResetPassword(string identity, string secret, string newPassword = null)
		{
			return false;
		}

		public bool ResetPassword(string identity, string[] passwordAnswers, string newPassword = null)
		{
			//var objectAccess = this.EnsureDataAccess();
			//var certification = this.GetCertification(certificationId);

			//IDictionary<string, object> outParameters;

			//objectAccess.Execute("Security.User.GetPasswordAnswer", new Dictionary<string, object>
			//{
			//	{"UserId", certification.User.UserId},
			//}, out outParameters);

			//object answerValue;

			//if(!outParameters.TryGetValue("PasswordAnswer", out answerValue))
			//	throw new InvalidOperationException("Can not obtain the Password-Answer.");

			//if(!Zongsoft.Collections.BinaryComparer.Default.Equals(PasswordUtility.HashPassword(passwordAnswer), (byte[])answerValue))
			//	throw new AuthenticationException("Invalid value of the Password-Answer.");

			//var password = PasswordUtility.GeneratePassword();
			//var passwordSalt = PasswordUtility.GeneratePasswordSalt(4);

			//objectAccess.Execute("Security.User.SetPassword", new Dictionary<string, object>
			//{
			//	{"UserId", certification.User.UserId},
			//	{"Password", PasswordUtility.HashPassword(password, passwordSalt)},
			//	{"PasswordSalt", passwordSalt},
			//});

			return true;
		}

		public string[] GetPasswordQuestions(string identity)
		{
			//var objectAccess = this.EnsureDataAccess();
			//var certification = this.GetCertification(certificationId);

			//IDictionary<string, object> outParameters;

			//objectAccess.Execute("Security.User.GetPasswordQuestion", new Dictionary<string, object>
			//{
			//	{"UserId", certification.User.UserId},
			//}, out outParameters);

			//object questionValue;

			//if(outParameters.TryGetValue("PasswordQuestion", out questionValue))
			//	return questionValue as string;

			return null;
		}

		public void SetPasswordQuestionsAndAnswers(int userId, string password, string[] passwordQuestions, string[] passwordAnswers)
		{
			//var objectAccess = this.EnsureDataAccess();
			//var certification = this.GetCertification(certificationId);

			//byte[] storedPassword;
			//byte[] storedPasswordSalt;

			//var user = MembershipHelper.GetPassword(objectAccess, certification.User.UserId, out storedPassword, out storedPasswordSalt);

			//if(user == null)
			//	throw new InvalidOperationException("Invalid account.");

			//if(!PasswordUtility.VerifyPassword(password, storedPassword, storedPasswordSalt))
			//	throw new AuthenticationException("Invalid password.");

			//objectAccess.Execute("Security.User.SetPasswordQuestionAndAnswer", new Dictionary<string, object>
			//{
			//	{"ApplicationId", certification.Namespace},
			//	{"UserId", certification.User.UserId},
			//	{"PasswordQuestion", passwordQuestion},
			//	{"PasswordAnswer", PasswordUtility.HashPassword(passwordAnswer)},
			//});
		}
		#endregion
	}
}

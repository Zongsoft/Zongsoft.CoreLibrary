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
using System.Text.RegularExpressions;

using Zongsoft.Data;

namespace Zongsoft.Security.Membership
{
	internal static class MembershipHelper
	{
		#region 正则表达
		private static readonly Regex _regexEmail = new Regex(@"^\s*\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*\s*$", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
		private static readonly Regex _regexPhone = new Regex(@"^\s*\d{6,11}\s*$", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
		#endregion

		#region 公共方法
		public static UserIdentityType GetUserIdentityType(string identity)
		{
			if(string.IsNullOrWhiteSpace(identity))
				throw new ArgumentNullException("identity");

			if(_regexEmail.IsMatch(identity))
				return UserIdentityType.Email;

			if(_regexPhone.IsMatch(identity))
				return UserIdentityType.PhoneNumber;

			return UserIdentityType.Name;
		}

		public static User GetPassword(IDataAccess dataAccess, int userId, out byte[] password, out byte[] passwordSalt)
		{
			return ExecuteCommand(dataAccess, "Security.User.GetPassword", new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
				{
					{"UserId", userId},
				}, out password, out passwordSalt);
		}

		public static User GetPasswordByUserName(IDataAccess dataAccess, string @namespace, string userName, out byte[] password, out byte[] passwordSalt)
		{
			return ExecuteCommand(dataAccess, "Security.User.GetPasswordByUserName", new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
				{
					{"Namespace", @namespace},
					{"UserName", userName},
				}, out password, out passwordSalt);
		}

		public static User GetPasswordByEmail(IDataAccess dataAccess, string @namespace, string email, out byte[] password, out byte[] passwordSalt)
		{
			return ExecuteCommand(dataAccess, "Security.User.GetPasswordByEmail", new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
				{
					{"Namespace", @namespace},
					{"Email", email},
				}, out password, out passwordSalt);
		}

		public static User GetPasswordByPhoneNumber(IDataAccess dataAccess, string @namespace, string phoneNumber, out byte[] password, out byte[] passwordSalt)
		{
			return ExecuteCommand(dataAccess, "Security.User.GetPasswordByPhoneNumber", new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
				{
					{"Namespace", @namespace},
					{"PhoneNumber", phoneNumber},
				}, out password, out passwordSalt);
		}
		#endregion

		#region 私有方法
		private static User ExecuteCommand(IDataAccess dataAccess, string commandName, IDictionary<string, object> parameters, out byte[] password, out byte[] passwordSalt)
		{
			if(dataAccess == null)
				throw new ArgumentNullException("dataAccess");

			if(string.IsNullOrWhiteSpace(commandName))
				throw new ArgumentNullException("commandName");

			//定义数据殷勤的返回参数字典
			IDictionary<string, object> outParameters = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

			//从数据引擎中获取指定应用下的指定用户的密码数据
			dataAccess.Execute(commandName, parameters, out outParameters);

			object storedPassword;
			object storedPasswordSalt;

			outParameters.TryGetValue("Password", out storedPassword);
			outParameters.TryGetValue("PasswordSalt", out storedPasswordSalt);

			password = storedPassword as byte[];
			passwordSalt = storedPasswordSalt as byte[];

			return new User((int)outParameters["UserId"], (string)outParameters["UserName"], (string)outParameters["Namespace"]);
		}
		#endregion
	}
}

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
		#region 常量定义
		internal const string DATA_CONTAINER_NAME = "Security";

		internal const string DATA_COMMAND_SETPASSWORD = DATA_CONTAINER_NAME + ".SetPassword";
		internal const string DATA_COMMAND_GETPASSWORD = DATA_CONTAINER_NAME + ".GetPassword";
		internal const string DATA_COMMAND_GETPASSWORDBYNAME = DATA_CONTAINER_NAME + ".GetPasswordByName";
		internal const string DATA_COMMAND_GETPASSWORDBYEMAIL = DATA_CONTAINER_NAME + ".GetPasswordByEmail";
		internal const string DATA_COMMAND_GETPASSWORDBYPHONE = DATA_CONTAINER_NAME + ".GetPasswordByPhone";

		internal const string DATA_COMMAND_SETPREMISSION = DATA_CONTAINER_NAME + ".SetPermission";
		internal const string DATA_COMMAND_SETPREMISSIONFILTER = DATA_CONTAINER_NAME + ".SetPermissionFilter";

		internal const string DATA_ENTITY_USER = DATA_CONTAINER_NAME + ".User";
		internal const string DATA_ENTITY_ROLE = DATA_CONTAINER_NAME + ".Role";
		internal const string DATA_ENTITY_MEMBER = DATA_CONTAINER_NAME + ".Member";
		internal const string DATA_ENTITY_PERMISSION = DATA_CONTAINER_NAME + ".Permission";
		internal const string DATA_ENTITY_PERMISSION_FILTER = DATA_CONTAINER_NAME + ".PermissionFilter";
		#endregion

		#region 公共方法
		public static UserIdentityType GetUserIdentityType(string identity)
		{
			if(string.IsNullOrWhiteSpace(identity))
				throw new ArgumentNullException("identity");

			if(Zongsoft.Text.TextRegular.Web.Email.IsMatch(identity))
				return UserIdentityType.Email;

			if(Zongsoft.Text.TextRegular.Chinese.Cellphone.IsMatch(identity))
				return UserIdentityType.PhoneNumber;

			return UserIdentityType.Name;
		}

		public static bool GetPassword(IDataAccess dataAccess, int userId, out byte[] password, out byte[] passwordSalt)
		{
			return ExecuteGetPasswordCommand(dataAccess, DATA_COMMAND_GETPASSWORD, new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
				{
					{"UserId", userId},
				}, out password, out passwordSalt) != 0;
		}

		public static int? GetPasswordByName(IDataAccess dataAccess, string @namespace, string userName, out byte[] password, out byte[] passwordSalt)
		{
			return ExecuteGetPasswordCommand(dataAccess, DATA_COMMAND_GETPASSWORDBYNAME, new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
				{
					{"Namespace", @namespace},
					{"UserName", userName},
				}, out password, out passwordSalt);
		}

		public static int? GetPasswordByEmail(IDataAccess dataAccess, string @namespace, string email, out byte[] password, out byte[] passwordSalt)
		{
			return ExecuteGetPasswordCommand(dataAccess, DATA_COMMAND_GETPASSWORDBYEMAIL, new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
				{
					{"Namespace", @namespace},
					{"Email", email},
				}, out password, out passwordSalt);
		}

		public static int? GetPasswordByPhone(IDataAccess dataAccess, string @namespace, string phoneNumber, out byte[] password, out byte[] passwordSalt)
		{
			return ExecuteGetPasswordCommand(dataAccess, DATA_COMMAND_GETPASSWORDBYPHONE, new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
				{
					{"Namespace", @namespace},
					{"PhoneNumber", phoneNumber},
				}, out password, out passwordSalt);
		}

		public static User GetUser(IDataAccess dataAccess, int userId)
		{
			#region 测试代码
			return new User(1, "Administrator");
			#endregion

			if(dataAccess == null)
				throw new ArgumentNullException("dataAccess");

			return dataAccess.Select<User>(DATA_ENTITY_USER, new ConditionCollection(ConditionCombine.And)
			{
				new Condition("UserId", userId),
			}).FirstOrDefault();
		}
		#endregion

		#region 私有方法
		private static int? ExecuteGetPasswordCommand(IDataAccess dataAccess, string commandName, IDictionary<string, object> parameters, out byte[] password, out byte[] passwordSalt)
		{
			if(dataAccess == null)
				throw new ArgumentNullException("dataAccess");

			if(string.IsNullOrWhiteSpace(commandName))
				throw new ArgumentNullException("commandName");

			password = null;
			passwordSalt = null;

			//定义数据殷勤的返回参数字典
			IDictionary<string, object> outParameters;

			//从数据引擎中获取指定应用下的指定用户的密码数据
			dataAccess.Execute(commandName, parameters, out outParameters);

			if(outParameters == null || outParameters.Count < 1)
				return null;

			object storedPassword;
			object storedPasswordSalt;

			outParameters.TryGetValue("Password", out storedPassword);
			outParameters.TryGetValue("PasswordSalt", out storedPasswordSalt);

			password = storedPassword as byte[];
			passwordSalt = storedPasswordSalt as byte[];

			object result;

			if(outParameters.TryGetValue("UserId", out result))
				return Zongsoft.Common.Convert.ConvertValue<int?>(result, () => null);

			return null;
		}
		#endregion
	}
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Zongsoft.Data;
using Zongsoft.Data.Entities;

namespace Zongsoft.Security.Membership
{
	internal static class MembershipHelper
	{
		public static bool GetPassword(IObjectAccess objectAccess, string applicationId, string userName, out byte[] password, out byte[] passwordSalt)
		{
			if(objectAccess == null)
				throw new ArgumentNullException("objectAccess");

			//定义数据殷勤的返回参数字典
			IDictionary<string, object> outParameters = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

			//从数据引擎中获取指定应用下的指定用户的密码数据
			objectAccess.Execute("Security.User.GetPassword", new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
			{
				{"ApplicationId", applicationId},
				{"UserName", userName},
			}, out outParameters);

			object storedPassword;
			object storedPasswordSalt;

			outParameters.TryGetValue("Password", out storedPassword);
			outParameters.TryGetValue("PasswordSalt", out storedPasswordSalt);

			password = storedPassword as byte[];
			passwordSalt = storedPasswordSalt as byte[];

			return password != null;
		}
	}
}

using System;
using System.Collections.Generic;

namespace Zongsoft.Samples.DataEntity.Models
{
	public static class UserExtension
	{
		public static string GetAvatar(IUserEntity user, string value)
		{
			return value;
		}

		public static bool SetAvatar(IUserEntity user, string value)
		{
			return true;
		}

		public static string GetAvatarUrl(IUserEntity user)
		{
			var avatar = user.Avatar;

			if(string.IsNullOrEmpty(avatar))
				return string.Empty;

			return Zongsoft.IO.FileSystem.GetUrl(avatar);
		}
	}
}

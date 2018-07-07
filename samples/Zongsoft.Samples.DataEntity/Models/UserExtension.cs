using System;
using System.Collections.Generic;

namespace Zongsoft.Samples.DataEntity.Models
{
	public static class UserExtension
	{
		public static string GetAvatar(IUserEntity user)
		{
			return null;
		}

		public static bool SetAvatar(IUserEntity user, string name, string value)
		{
			return false;
		}

		public static string GetAvatarUrl(IUserEntity user, string name)
		{
			var avatar = user.Avatar;

			if(string.IsNullOrEmpty(avatar))
				return string.Empty;

			return Zongsoft.IO.FileSystem.GetUrl(avatar);
		}
	}
}

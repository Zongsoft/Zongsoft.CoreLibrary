using System;
using System.Collections.Generic;

namespace Zongsoft.Samples.Entities.Models
{
	public static class UserExtension
	{
		public static string GetAvatar(IUserEntity user, string value)
		{
			return value;
		}

		//public static bool SetAvatar(IUserEntity user, string value)
		//{
		//	return true;
		//}

		public static string GetAvatarUrl(IUserEntity user, string value = null)
		{
			var avatar = user.Avatar;

			if(string.IsNullOrEmpty(avatar))
				return string.Empty;

			return "url:" + avatar;
		}

		public static ICollection<object> GetAssets(IUserEntity user)
		{
			return new System.Collections.ObjectModel.ObservableCollection<object>();
		}
	}
}

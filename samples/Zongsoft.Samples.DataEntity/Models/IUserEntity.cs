using System;
using System.Collections.Generic;

namespace Zongsoft.Samples.DataEntity.Models
{
	[Zongsoft.Data.DataAccess("Security.User")]
	public interface IUserEntity : IPerson
	{
		[Zongsoft.Data.Conditional(ConverterType = typeof(object))]
		uint UserId
		{
			get; set;
		}

		[System.ComponentModel.DefaultValue("Zongsoft")]
		string Namespace
		{
			get; set;
		}

		string Email
		{
			get; set;
		}

		string PhoneNumber
		{
			get; set;
		}

		[Zongsoft.Data.EntityBehavior(typeof(UserExtension))]
		string Avatar
		{
			get; set;
		}

		[Zongsoft.Data.EntityBehavior(typeof(UserExtension))]
		string AvatarUrl
		{
			get;
		}

		[Zongsoft.Data.EntityBehavior()]
		ICollection<string> Assets
		{
			get;
		}

		byte Status
		{
			get; set;
		}

		DateTime? StatusTimestamp
		{
			get; set;
		}

		string PrincipalId
		{
			get; set;
		}

		DateTime CreatedTime
		{
			get; set;
		}

		string Description
		{
			get; set;
		}
	}
}

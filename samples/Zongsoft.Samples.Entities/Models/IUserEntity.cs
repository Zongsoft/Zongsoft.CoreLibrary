using System;
using System.ComponentModel;
using System.Collections.Generic;

namespace Zongsoft.Samples.Entities.Models
{
	[Zongsoft.Data.Entity("Security.User")]
	public interface IUserEntity : IPerson
	{
		[Zongsoft.Data.Conditional(ConverterType = typeof(object))]
		uint UserId
		{
			get; set;
		}

		//[DefaultValue("Zongsoft")]
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

		[Zongsoft.Data.Entity.Property(Data.Entity.PropertyImplementationMode.Extension, typeof(UserExtension))]
		string Avatar
		{
			get; set;
		}

		//[DefaultValue("xoxo")]
		[Zongsoft.Data.Entity.Property(Data.Entity.PropertyImplementationMode.Extension, typeof(UserExtension))]
		string AvatarUrl
		{
			get;
		}

		//[DefaultValue(typeof(List<object>))]
		//[DefaultValue(typeof(UserExtension))]
		//[Zongsoft.Data.Entity.Property(Data.Entity.PropertyImplementationMode.Singleton, typeof(UserExtension))]
		ICollection<object> Assets
		{
			get;
		}

		//[Zongsoft.Data.Entity.Property(IsExplicitImplementation = true)]
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

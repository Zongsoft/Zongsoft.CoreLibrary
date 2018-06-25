using System;
using System.Collections.Generic;

namespace Zongsoft.Samples.DataEntity.Models
{
	public interface IUserEntity : Zongsoft.Data.IDataEntity
	{
		uint UserId
		{
			get; set;
		}

		[System.ComponentModel.DefaultValue("Zongsoft")]
		string Namespace
		{
			get; set;
		}

		string Name
		{
			get; set;
		}

		string FullName
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

		string Avatar
		{
			get; set;
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

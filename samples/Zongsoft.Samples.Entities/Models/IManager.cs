using System;
using System.Collections.Generic;

namespace Zongsoft.Samples.Entities.Models
{
	[Zongsoft.Data.DataAccess("Manager")]
	public interface IManager : IEmployee
	{
		bool IsAdvance
		{
			get;
			set;
		}
	}
}

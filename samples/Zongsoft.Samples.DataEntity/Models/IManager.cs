using System;
using System.Collections.Generic;

namespace Zongsoft.Samples.DataEntity.Models
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

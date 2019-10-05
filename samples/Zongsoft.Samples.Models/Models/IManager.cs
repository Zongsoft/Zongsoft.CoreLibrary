using System;
using System.Collections.Generic;

namespace Zongsoft.Samples.Models
{
	[Zongsoft.Data.Model("Manager")]
	public interface IManager : IEmployee
	{
		bool IsAdvance
		{
			get;
			set;
		}
	}
}

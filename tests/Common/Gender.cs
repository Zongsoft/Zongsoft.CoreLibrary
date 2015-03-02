using System;
using System.ComponentModel;

namespace Zongsoft.Common.Tests
{
	public enum Gender
	{
		[Zongsoft.ComponentModel.Alias("M")]
		[Description("男士")]
		Male,

		[Zongsoft.ComponentModel.Alias("F")]
		[Description("女士")]
		Female,
	}
}

using System;
using System.ComponentModel;

namespace Zongsoft.Tests
{
	public enum Gender
	{
		[Zongsoft.ComponentModel.Alias("M")]
		//[Description("先生")]
		[Description("${Text.Gender.Male}")]
		Male,

		[Zongsoft.ComponentModel.Alias("F")]
		//[Description("女士")]
		[Description("${Text.Gender.Female}")]
		Female,
	}
}

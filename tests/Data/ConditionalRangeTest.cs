using System;
using System.Collections.Generic;

using Xunit;

namespace Zongsoft.Data
{
	public class ConditionalRangeTest
	{
		[Fact]
		public void Test()
		{
			var age = new ConditionalRange<byte>(100, 18);
			var ageText = age.ToString();

			var birthdate = new ConditionalRange<DateTime>(new DateTime(1979, 6, 9), null);
			var birthdateText = birthdate.ToString();

			var range = ConditionalRange<int>.Parse(ageText);
			Assert.NotNull(range);
			Assert.Equal(18, range.From);
			Assert.Equal(100, range.To);

			range = Common.Convert.ConvertValue<ConditionalRange<int>>(ageText);
			Assert.NotNull(range);
			Assert.Equal(18, range.From);
			Assert.Equal(100, range.To);
		}
	}
}

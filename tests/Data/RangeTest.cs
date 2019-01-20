using System;
using System.Collections.Generic;

using Xunit;

namespace Zongsoft.Data
{
	public class RangeTest
	{
		[Fact]
		public void Test()
		{
			object min, max;

			var age = new Range<byte>(100, 18);
			var ageText = age.ToString();

			var birthdate = new Range<DateTime>(new DateTime(1979, 6, 9), null);
			var birthdateText = birthdate.ToString();

			var range = Range<int>.Parse(ageText);
			Assert.NotNull(range);
			Assert.Equal(18, range.Minimum);
			Assert.Equal(100, range.Maximum);

			Assert.True(Range.TryGetRange(age, out min, out max));
			Assert.Equal(18, (byte)min);
			Assert.Equal(100, (byte)max);

			Assert.True(Range.TryGetRange(birthdate, out min, out max));
			Assert.Equal(new DateTime(1979, 6, 9), (DateTime)min);
			Assert.Null(max);
		}
	}
}

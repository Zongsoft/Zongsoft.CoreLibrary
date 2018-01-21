using System;
using System.Collections.Generic;

using Xunit;

namespace Zongsoft.Data
{
	public class ScopingTest
	{
		[Fact]
		public void Test()
		{
			var scoping = Scoping.Parse(null);

			Assert.Equal(0, scoping.Count);
			Assert.True(string.IsNullOrEmpty(scoping.ToString()));

			scoping.Include("UserId");
			Assert.Equal(1, scoping.Count);

			scoping.Add("*, !CreatedTime");
			Assert.Equal(3, scoping.Count);

			scoping.Add("CreatedTime");
			Assert.Equal(3, scoping.Count);

			scoping.Add("!");
			Assert.Equal(1, scoping.Count);
		}
	}
}

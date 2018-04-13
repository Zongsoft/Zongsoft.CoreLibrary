using System;

using Xunit;

namespace Zongsoft.Data
{
	public class SortingTest
	{
		[Fact]
		public void Test()
		{
			var sorting1 = Sorting.Ascending("Name");

			Assert.Equal(SortingMode.Ascending, sorting1.Mode);
			Assert.Equal("Name", sorting1.Name);

			var sorting2 = Sorting.Descending("Name");

			Assert.Equal(SortingMode.Descending, sorting2.Mode);
			Assert.Equal("Name", sorting2.Name);

			var sortings1 = sorting1 + sorting2;
			Assert.Equal(1, sortings1.Length);

			Assert.Equal(SortingMode.Descending, sortings1[0].Mode);
			Assert.Equal("Name", sortings1[0].Name);

			var sortings2 = sortings1 + Sorting.Ascending("PhoneNumber");
			Assert.Equal(2, sortings2.Length);

			Assert.Equal(SortingMode.Ascending, sortings2[1].Mode);
			Assert.Equal("PhoneNumber", sortings2[1].Name);
		}
	}
}

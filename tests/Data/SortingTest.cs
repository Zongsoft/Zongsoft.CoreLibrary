using System;

using Xunit;

namespace Zongsoft.Data
{
	public class SortingTest
	{
		[Fact]
		public void Test()
		{
			var sorting1 = Sorting.Ascending("Id", "Name");

			Assert.Equal(SortingMode.Ascending, sorting1.Mode);
			Assert.Equal("Id, Name", sorting1.MembersText);
			Assert.Equal(2, sorting1.Members.Length);
			Assert.Equal("Id", sorting1.Members[0]);
			Assert.Equal("Name", sorting1.Members[1]);

			var sorting2 = Sorting.Descending("Amount", "CreatedTime");

			Assert.Equal(SortingMode.Descending, sorting2.Mode);
			Assert.Equal("Amount, CreatedTime", sorting2.MembersText);
			Assert.Equal(2, sorting2.Members.Length);
			Assert.Equal("Amount", sorting2.Members[0]);
			Assert.Equal("CreatedTime", sorting2.Members[1]);

			var sortings1 = sorting1 + sorting2;
			Assert.Equal(2, sortings1.Length);

			Assert.Equal(SortingMode.Ascending, sortings1[0].Mode);
			Assert.Equal("Id, Name", sortings1[0].MembersText);

			Assert.Equal(SortingMode.Descending, sortings1[1].Mode);
			Assert.Equal("Amount, CreatedTime", sortings1[1].MembersText);

			var sortings2 = sortings1 + Sorting.Ascending("PhoneNumber, HomeAddress.City");
			Assert.Equal(3, sortings2.Length);

			Assert.Equal(SortingMode.Ascending, sortings2[0].Mode);
			Assert.Equal("Id, Name", sortings2[0].MembersText);

			Assert.Equal(SortingMode.Descending, sortings2[1].Mode);
			Assert.Equal("Amount, CreatedTime", sortings2[1].MembersText);

			Assert.Equal(SortingMode.Ascending, sortings2[2].Mode);
			Assert.Equal("PhoneNumber, HomeAddress.City", sortings2[2].MembersText);
		}
	}
}

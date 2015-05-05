using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Zongsoft.Data
{
	[TestClass]
	public class SortingTest
	{
		[TestMethod]
		public void Test()
		{
			var sorting1 = Sorting.Ascending("Id", "Name");

			Assert.AreEqual(SortingMode.Ascending, sorting1.Mode);
			Assert.AreEqual("Id, Name", sorting1.FieldsText);
			Assert.AreEqual(2, sorting1.Fields.Length);
			Assert.AreEqual("Id", sorting1.Fields[0]);
			Assert.AreEqual("Name", sorting1.Fields[1]);

			var sorting2 = Sorting.Descending("Amount", "CreatedTime");

			Assert.AreEqual(SortingMode.Descending, sorting2.Mode);
			Assert.AreEqual("Amount, CreatedTime", sorting2.FieldsText);
			Assert.AreEqual(2, sorting2.Fields.Length);
			Assert.AreEqual("Amount", sorting2.Fields[0]);
			Assert.AreEqual("CreatedTime", sorting2.Fields[1]);

			var sortings1 = sorting1 + sorting2;
			Assert.AreEqual(2, sortings1.Length);

			Assert.AreEqual(SortingMode.Ascending, sortings1[0].Mode);
			Assert.AreEqual("Id, Name", sortings1[0].FieldsText);

			Assert.AreEqual(SortingMode.Descending, sortings1[1].Mode);
			Assert.AreEqual("Amount, CreatedTime", sortings1[1].FieldsText);

			var sortings2 = sortings1 + Sorting.Ascending("PhoneNumber, HomeAddress.City");
			Assert.AreEqual(3, sortings2.Length);

			Assert.AreEqual(SortingMode.Ascending, sortings2[0].Mode);
			Assert.AreEqual("Id, Name", sortings2[0].FieldsText);

			Assert.AreEqual(SortingMode.Descending, sortings2[1].Mode);
			Assert.AreEqual("Amount, CreatedTime", sortings2[1].FieldsText);

			Assert.AreEqual(SortingMode.Ascending, sortings2[2].Mode);
			Assert.AreEqual("PhoneNumber, HomeAddress.City", sortings2[2].FieldsText);
		}
	}
}

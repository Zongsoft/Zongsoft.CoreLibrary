using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Zongsoft.Common;
using Zongsoft.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Zongsoft.Common.Tests
{
	[TestClass]
	public class EnumUtilityTests
	{
		[TestMethod]
		public void GetEnumEntryTest()
		{
			var entry = EnumUtility.GetEnumEntry(Gender.Female);

			Assert.AreEqual("Female", entry.Name);
			Assert.AreEqual(Gender.Female, entry.Value); //注意：entry.Value 为枚举类型
			Assert.AreEqual("F", entry.Alias);
			Assert.AreEqual("女士", entry.Description);

			entry = EnumUtility.GetEnumEntry(Gender.Male, true);

			Assert.AreEqual("Male", entry.Name);
			Assert.AreEqual(0, entry.Value); //注意：entry.Value 为枚举项的基元类型
			Assert.AreEqual("M", entry.Alias);
			Assert.AreEqual("先生", entry.Description);
		}

		[TestMethod]
		public void GetEnumEntriesTest()
		{
			var entries = EnumUtility.GetEnumEntries(typeof(Gender), true);

			Assert.AreEqual(2, entries.Length);
			Assert.AreEqual("Male", entries[0].Name);
			Assert.AreEqual("Female", entries[1].Name);

			entries = EnumUtility.GetEnumEntries(typeof(Nullable<Gender>), true, null, "<Unknown>");

			Assert.AreEqual(3, entries.Length);
			Assert.AreEqual("", entries[0].Name);
			Assert.AreEqual(null, entries[0].Value);
			Assert.AreEqual("<Unknown>", entries[0].Description);

			Assert.AreEqual("Male", entries[1].Name);
			Assert.AreEqual("Female", entries[2].Name);
		}
	}
}

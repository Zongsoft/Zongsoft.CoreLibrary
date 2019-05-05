using System;
using System.Collections.Generic;

using Zongsoft.Common;
using Zongsoft.Tests;

using Xunit;

namespace Zongsoft.Common.Tests
{
	public class EnumUtilityTests
	{
		[Fact]
		public void GetEnumEntryTest()
		{
			var entry = EnumUtility.GetEnumEntry(Gender.Female);

			Assert.Equal("Female", entry.Name);
			Assert.Equal(Gender.Female, entry.Value); //注意：entry.Value 为枚举类型
			Assert.Equal("F", entry.Alias);
			Assert.Equal("女士", entry.Description);

			entry = EnumUtility.GetEnumEntry(Gender.Male, true);

			Assert.Equal("Male", entry.Name);
			Assert.Equal(0, entry.Value); //注意：entry.Value 为枚举项的基元类型
			Assert.Equal("M", entry.Alias);
			Assert.Equal("先生", entry.Description);
		}

		[Fact]
		public void GetEnumEntriesTest()
		{
			var entries = EnumUtility.GetEnumEntries(typeof(Gender), true);

			Assert.Equal(2, entries.Length);
			Assert.Equal("Male", entries[0].Name);
			Assert.Equal("Female", entries[1].Name);

			entries = EnumUtility.GetEnumEntries(typeof(Nullable<Gender>), true, null, "<Unknown>");

			Assert.Equal(3, entries.Length);
			Assert.Equal("", entries[0].Name);
			Assert.Equal(null, entries[0].Value);
			Assert.Equal("<Unknown>", entries[0].Description);

			Assert.Equal("Male", entries[1].Name);
			Assert.Equal("Female", entries[2].Name);
		}
	}
}

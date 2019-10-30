using System;
using System.Collections.Generic;
using System.Linq;

using Xunit;

namespace Zongsoft.Common.Tests
{
	public class StringExtensionTests
	{
		[Fact]
		public void TestRemoveAny()
		{
			const string TEXT = @"Read^me??.txt";

			Assert.Equal("Read^me.txt", StringExtension.RemoveAny(TEXT, '?'));
			Assert.Equal("Readme.txt", StringExtension.RemoveAny(TEXT, new[] { '?', '^' }));
		}

		[Fact]
		public void TestTrimString()
		{
			Assert.Equal("ContentSuffix", StringExtension.Trim("PrefixPrefixContentSuffix", "Prefix"));
			Assert.Equal("PrefixPrefixContent", StringExtension.Trim("PrefixPrefixContentSuffix", "Suffix"));
			Assert.Equal("Content", StringExtension.Trim("PrefixPrefixContentSuffix", "Prefix", "Suffix"));
		}

		[Fact]
		public void TestIsDigits()
		{
			string digits;

			Assert.True(StringExtension.IsDigits("123", out digits));
			Assert.Equal("123", digits);

			Assert.True(StringExtension.IsDigits(" \t123   ", out digits));
			Assert.Equal("123", digits);

			Assert.False(StringExtension.IsDigits("1 23"));
			Assert.False(StringExtension.IsDigits("1#23"));
			Assert.False(StringExtension.IsDigits("$123"));
		}
	}
}

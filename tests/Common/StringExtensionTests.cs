using System;
using System.Collections.Generic;
using System.Linq;

using Xunit;

namespace Zongsoft.Common.Tests
{
	public class StringExtensionTests
	{
		[Fact]
		public void TestGetStringHashCode()
		{
			const int LENGTH = 100;

			var data1 = new byte[LENGTH];
			var data2 = new byte[LENGTH];

			for(var i = 0; i < LENGTH; i++)
			{
				data1[i] = (byte)i;
				data2[i] = (byte)(LENGTH - 1 - i);
			}

			var text1 = System.Convert.ToBase64String(data1);
			var text2 = System.Convert.ToBase64String(data2);

			Assert.NotEqual(StringExtension.GetStringHashCode(text1),
			                StringExtension.GetStringHashCode(text2));
			
		}

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
	}
}

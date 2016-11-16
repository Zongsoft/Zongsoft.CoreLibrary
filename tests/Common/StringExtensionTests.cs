using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Zongsoft.Common;

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

			Assert.NotEqual(Zongsoft.Common.StringExtension.GetStringHashCode(text1),
			                Zongsoft.Common.StringExtension.GetStringHashCode(text2));
			
		}

		[Fact]
		public void TestContainsCharacters()
		{
			Assert.True(Zongsoft.Common.StringExtension.ContainsCharacters("abcdefghijk", "big"));
			Assert.True(Zongsoft.Common.StringExtension.ContainsCharacters("abcdefghijk", "biger"));
			Assert.False(Zongsoft.Common.StringExtension.ContainsCharacters("abcdefghijk", "xyz"));
		}

		[Fact]
		public void TestRemoveCharacters()
		{
			const string TEXT = @"Read^me??.txt";

			Assert.Equal("Read^me.txt", Zongsoft.Common.StringExtension.RemoveCharacters(TEXT, "?"));
			Assert.Equal("Readme.txt", Zongsoft.Common.StringExtension.RemoveCharacters(TEXT, "?^"));
		}

		[Fact]
		public void TestTrimString()
		{
			Assert.Equal("ContentSuffix", Zongsoft.Common.StringExtension.TrimString("PrefixPrefixContentSuffix", "Prefix"));
			Assert.Equal("PrefixPrefixContent", Zongsoft.Common.StringExtension.TrimString("PrefixPrefixContentSuffix", "Suffix"));
			Assert.Equal("Content", Zongsoft.Common.StringExtension.TrimString("PrefixPrefixContentSuffix", "Prefix", "Suffix"));
		}

		[Fact]
		public void TestEscape()
		{
			Assert.Equal(@"", StringExtension.Escape(@"").FirstOrDefault());
			Assert.Equal(@"	a\b\c	\def'\""$", StringExtension.Escape(@"\ta\b\c\t\\def'\""$").FirstOrDefault());

			var items = StringExtension.Escape(" \t abc\tdef", '"', '\'').ToArray();
			Assert.Equal(2, items.Length);
			Assert.Equal("abc", items[0]);
			Assert.Equal("def", items[1]);
		}
	}
}

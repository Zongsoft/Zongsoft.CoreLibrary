using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Zongsoft.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Zongsoft.Common.Tests
{
	[TestClass]
	public class StringExtensionTests
	{
		[TestMethod]
		public void GetStringHashCodeTest()
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

			Assert.AreNotEqual(Zongsoft.Common.StringExtension.GetStringHashCode(text1),
			                   Zongsoft.Common.StringExtension.GetStringHashCode(text2));
			
		}

		[TestMethod]
		public void ContainsCharactersTest()
		{
			Assert.IsTrue(Zongsoft.Common.StringExtension.ContainsCharacters("abcdefghijk", "big"));
			Assert.IsTrue(Zongsoft.Common.StringExtension.ContainsCharacters("abcdefghijk", "biger"));
			Assert.IsFalse(Zongsoft.Common.StringExtension.ContainsCharacters("abcdefghijk", "xyz"));
		}

		[TestMethod]
		public void RemoveCharactersTest()
		{
			const string TEXT = @"Read^me??.txt";

			Assert.AreEqual("Read^me.txt", Zongsoft.Common.StringExtension.RemoveCharacters(TEXT, "?"));
			Assert.AreEqual("Readme.txt", Zongsoft.Common.StringExtension.RemoveCharacters(TEXT, "?^"));
		}

		[TestMethod]
		public void TrimStringTest()
		{
			Assert.AreEqual("ContentSuffix", Zongsoft.Common.StringExtension.TrimString("PrefixPrefixContentSuffix", "Prefix"));
			Assert.AreEqual("PrefixPrefixContent", Zongsoft.Common.StringExtension.TrimString("PrefixPrefixContentSuffix", "Suffix"));
			Assert.AreEqual("Content", Zongsoft.Common.StringExtension.TrimString("PrefixPrefixContentSuffix", "Prefix", "Suffix"));
		}
	}
}

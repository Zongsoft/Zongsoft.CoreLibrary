using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Zongsoft.Common.Tests
{
	[TestClass]
	public class RandomGeneratorTests
	{
		[TestMethod]
		public void TestGenerateString()
		{
			var data = new string[100];

			for(int i = 0; i < data.Length; i++)
			{
				for(int j = 1; j <= 128; j++)
				{
					data[i] = Zongsoft.Common.RandomGenerator.GenerateString(j);

					Assert.IsTrue(!string.IsNullOrEmpty(data[i]) && data[i].Length == j);
				}
			}
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xunit;

namespace Zongsoft.Common.Tests
{
	public class RandomGeneratorTests
	{
		[Fact]
		public void TestGenerateString()
		{
			var data = new string[100];

			for(int i = 0; i < data.Length; i++)
			{
				for(int j = 1; j <= 128; j++)
				{
					data[i] = Zongsoft.Common.Randomizer.GenerateString(j);

					Assert.True(!string.IsNullOrEmpty(data[i]) && data[i].Length == j);
				}
			}
		}
	}
}

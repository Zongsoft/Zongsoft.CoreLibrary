using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Zongsoft.IO
{
	[TestClass]
	public class PathTest
	{
		[TestMethod]
		public void PathParseTest()
		{
			var text = @"zfs.local:/data/images/1/year/month-day/[1]123.jpg";
			var path = Path.Parse(text);

			Assert.AreEqual("zfs.local", path.Schema);
			Assert.AreEqual("/data/images/1/year/month-day/[1]123.jpg", path.FullPath);
			Assert.AreEqual("/data/images/1/year/month-day/", path.DirectoryName);
			Assert.AreEqual("[1]123.jpg", path.FileName);
		}
	}
}

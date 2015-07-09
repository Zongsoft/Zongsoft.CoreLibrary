using System;
using System.IO;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Zongsoft.IO
{
	[TestClass]
	public class FileSystemTest
	{
		public FileSystemTest()
		{
			FileSystem.Providers.Register(LocalFileSystem.Instance, typeof(IFileSystem));
		}

		[TestMethod]
		public void DirectoryCreate()
		{
			var text = @"zfs.local:/c/temp/year/month-day/[1]123.jpg";
			var path = Path.Parse(text);

			var directoryPath = path.Schema + ":" + path.DirectoryName;

			FileSystem.Directory.Create(directoryPath);

			Assert.IsTrue(FileSystem.Directory.Exists(directoryPath));
			Assert.IsTrue(FileSystem.Directory.Delete(directoryPath));
		}
	}
}

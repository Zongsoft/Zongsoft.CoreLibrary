using System;

using Xunit;

namespace Zongsoft.IO
{
	public class PathTest
	{
		[Fact]
		public void PathParseTest()
		{
			var text = @"zfs.local:/data/images/1/year/month-day/[1]123.jpg";
			var path = Path.Parse(text);

			Assert.Equal("zfs.local", path.Scheme);
			Assert.Equal("/data/images/1/year/month-day/[1]123.jpg", path.FullPath);
			Assert.Equal("/data/images/1/year/month-day/", path.DirectoryName);
			Assert.Equal("[1]123.jpg", path.FileName);

			Assert.True(Zongsoft.IO.Path.TryParse("/images/avatar/large/steve.jpg", out path));
			Assert.Null(path.Scheme);
			Assert.True(path.IsFile);

			Assert.True(Zongsoft.IO.Path.TryParse("zs:", out path));
			Assert.Equal("zs", path.Scheme);
			Assert.True(path.IsDirectory);
			Assert.Equal("/", path.FullPath);
			Assert.Equal("zs:/", path.Url);
		}

		[Fact]
		public void PathCombineTest()
		{
			var baseDirectory = "zfs.local:/data/images/";
			var selfDirectory = "./bin";
			var parentDirectory = "../bin/Debug";

			Assert.Equal("zfs.local:/data/images/bin", Path.Combine(baseDirectory, selfDirectory));
			Assert.Equal("zfs.local:/data/bin/Debug", Path.Combine(baseDirectory, parentDirectory));
			Assert.Equal("zfs.local:/data/images/bin/Debug", Path.Combine(baseDirectory, selfDirectory, parentDirectory));
			Assert.Equal("/root", Path.Combine(baseDirectory, "/root"));
			Assert.Equal("/root/", Path.Combine(baseDirectory, selfDirectory, parentDirectory, "/root/"));

			Assert.Equal(@"D:/data/images/avatars/001.jpg", Path.Combine(@"D:\data\images\", "avatars/001.jpg"));
			Assert.Equal(@"D:/data/images/avatars/001.jpg", Path.Combine(@"D:\data\images\", ". /avatars / 001.jpg"));
			Assert.Equal(@"D:/data/avatars/001.jpg", Path.Combine(@"D:\data\images\", "../avatars / 001.jpg"));
			Assert.Equal(@"/avatars/001.jpg", Path.Combine(@"D:\data\images\", "/avatars/001.jpg"));
			Assert.Equal(@"/final.ext", Path.Combine(@"D:\data\images\", "avatars / 001.jpg", " / final.ext"));
			Assert.Equal(@"/final.ext/tail", Path.Combine(@"D:\data\images\", "avatars / 001.jpg", " / final.ext \t ", "tail  "));
		}
	}
}

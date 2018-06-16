using System;
using System.IO;

using Xunit;

namespace Zongsoft.IO
{
	public class FileSystemTest
	{
		[Fact]
		public void DirectoryCreate()
		{
			var text = @"zfs.local:/d/temp/sub-dir/[1]123.jpg";
			var path = Path.Parse(text);

			var directoryUrl = path.Scheme + ":" + path.DirectoryName;

			//创建目录
			FileSystem.Directory.Create(directoryUrl);

			Assert.True(FileSystem.Directory.Exists(directoryUrl));
			Assert.True(FileSystem.Directory.Delete(directoryUrl));
		}

		[Fact]
		public void DirectoryGetFilesTest()
		{
			const int FILE_COUNT = 10;

			if(!FileSystem.Directory.Exists("zfs.local:/d/temp/"))
				FileSystem.Directory.Create("zfs.local:/d/temp/");

			//生成一个随机的文件前缀名
			var prefix = string.Format("[{0:yyyyMMdd}]{1:N}", DateTime.Today, Guid.NewGuid());

			for(int i = 1; i <= FILE_COUNT; i++)
			{
				//创建临时文件
				using(var stream = FileSystem.File.Open(string.Format("zfs.local:/d/temp/{1}({0}).{2}", i, prefix, (i % 2 == 0 ? "log" : "txt")), FileMode.Create, FileAccess.Write))
				{
					Zongsoft.Runtime.Serialization.Serializer.Text.Serialize(stream, i.ToString());
				}
			}

			var count = 0;
			//使用正则匹配的方式查找文件
			var infos = LocalFileSystem.Instance.Directory.GetFiles(@"D:\temp", prefix + @"(/\d+/).log");

			foreach(var info in infos)
			{
				Assert.True(info.Name.StartsWith(prefix + "("));
				Assert.True(info.Name.EndsWith(").log"));

				count++;
			}

			Assert.True(count >= FILE_COUNT / 2);
			count = 0;

			//使用文件系统匹配的方式查找文件
			infos = LocalFileSystem.Instance.Directory.GetFiles(@"D:\temp", prefix + "*");

			foreach(var info in infos)
			{
				Assert.True(info.Name.StartsWith(prefix + "("));
				Assert.True(info.Name.EndsWith(").log") || info.Name.EndsWith(").txt"));

				//删除临时文件
				LocalFileSystem.Instance.File.Delete(info.Path.Url);

				count++;
			}

			Assert.True(count == FILE_COUNT);
		}
	}
}

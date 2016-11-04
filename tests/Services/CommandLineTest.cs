using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xunit;

namespace Zongsoft.Services.Tests
{
	public class CommandLineTest
	{
		[Fact]
		public void TestCommandLineParse()
		{
			Assert.Null(CommandLine.Parse(""));

			CommandLine line = null;

			//line = CommandLine.Parse("/");
			//Assert.NotNull(line);
			//Assert.Equal("/", line.Name);
			//Assert.Equal("", line.Path);
			//Assert.Equal("/", line.FullPath);

			//line = CommandLine.Parse(".");
			//Assert.NotNull(line);
			//Assert.Equal(".", line.Name);
			//Assert.Equal("", line.Path);
			//Assert.Equal(".", line.FullPath);

			//line = CommandLine.Parse("..");
			//Assert.NotNull(line);
			//Assert.Equal("..", line.Name);
			//Assert.Equal("", line.Path);
			//Assert.Equal("..", line.FullPath);

			line = CommandLine.Parse("send");
			Assert.Equal("send", line.Name);
			Assert.Equal("", line.Path);
			Assert.Equal("send", line.FullPath);

			line = CommandLine.Parse("/send");
			Assert.Equal("send", line.Name);
			Assert.Equal("/", line.Path);
			Assert.Equal("/send", line.FullPath);

			line = CommandLine.Parse("./send");
			Assert.Equal("send", line.Name);
			Assert.Equal("", line.Path);
			Assert.Equal("send", line.FullPath);

			line = CommandLine.Parse("../send");
			Assert.Equal("send", line.Name);
			Assert.Equal("..", line.Path);
			Assert.Equal("../send", line.FullPath);

			line = CommandLine.Parse("sms.send");
			Assert.Equal("send", line.Name);
			Assert.Equal("sms", line.Path);
			Assert.Equal("sms/send", line.FullPath);

			line = CommandLine.Parse("/sms.send");
			Assert.Equal("send", line.Name);
			Assert.Equal("/sms", line.Path);
			Assert.Equal("/sms/send", line.FullPath);

			line = CommandLine.Parse("./sms.send");
			Assert.Equal("send", line.Name);
			Assert.Equal("sms", line.Path);
			Assert.Equal("sms/send", line.FullPath);

			line = CommandLine.Parse("../sms.send");
			Assert.Equal("send", line.Name);
			Assert.Equal("../sms", line.Path);
			Assert.Equal("../sms/send", line.FullPath);

		}
	}
}

using System;
using System.Collections.Generic;

using Zongsoft.IO;

using Xunit;

namespace Zongsoft.Services.Tests
{
	public class CommandExpressionTest
	{
		[Fact]
		public void TestCommandExpressionParse()
		{
			var expression = CommandExpression.Parse("");
			Assert.Null(expression);

			expression = CommandExpression.Parse("/");
			Assert.NotNull(expression);
			Assert.Equal("/", expression.Name);
			Assert.Equal("", expression.Path);
			Assert.Equal("/", expression.FullPath);
			Assert.Equal(PathAnchor.None, expression.Anchor);

			expression = CommandExpression.Parse(".");
			Assert.NotNull(expression);
			Assert.Equal(".", expression.Name);
			Assert.Equal("", expression.Path);
			Assert.Equal(".", expression.FullPath);
			Assert.Equal(PathAnchor.None, expression.Anchor);

			expression = CommandExpression.Parse("..");
			Assert.NotNull(expression);
			Assert.Equal("..", expression.Name);
			Assert.Equal("", expression.Path);
			Assert.Equal("..", expression.FullPath);
			Assert.Equal(PathAnchor.None, expression.Anchor);

			expression = CommandExpression.Parse("send");
			Assert.Equal("send", expression.Name);
			Assert.Equal("", expression.Path);
			Assert.Equal("send", expression.FullPath);
			Assert.Equal(PathAnchor.None, expression.Anchor);

			expression = CommandExpression.Parse("/send");
			Assert.Equal("send", expression.Name);
			Assert.Equal("/", expression.Path);
			Assert.Equal("/send", expression.FullPath);
			Assert.Equal(PathAnchor.Root, expression.Anchor);

			expression = CommandExpression.Parse("./send");
			Assert.Equal("send", expression.Name);
			Assert.Equal("./", expression.Path);
			Assert.Equal("./send", expression.FullPath);
			Assert.Equal(PathAnchor.Current, expression.Anchor);

			expression = CommandExpression.Parse("../send");
			Assert.Equal("send", expression.Name);
			Assert.Equal("../", expression.Path);
			Assert.Equal("../send", expression.FullPath);
			Assert.Equal(PathAnchor.Parent, expression.Anchor);

			expression = CommandExpression.Parse("sms.send");
			Assert.Equal("send", expression.Name);
			Assert.Equal("sms/", expression.Path);
			Assert.Equal("sms/send", expression.FullPath);
			Assert.Equal(PathAnchor.None, expression.Anchor);

			expression = CommandExpression.Parse("/sms.send");
			Assert.Equal("send", expression.Name);
			Assert.Equal("/sms/", expression.Path);
			Assert.Equal("/sms/send", expression.FullPath);
			Assert.Equal(PathAnchor.Root, expression.Anchor);

			expression = CommandExpression.Parse("./sms.send");
			Assert.Equal("send", expression.Name);
			Assert.Equal("./sms/", expression.Path);
			Assert.Equal("./sms/send", expression.FullPath);
			Assert.Equal(PathAnchor.Current, expression.Anchor);

			expression = CommandExpression.Parse("../sms.send");
			Assert.Equal("send", expression.Name);
			Assert.Equal("../sms/", expression.Path);
			Assert.Equal("../sms/send", expression.FullPath);
			Assert.Equal(PathAnchor.Parent, expression.Anchor);

			Assert.Throws<CommandExpressionException>(() => CommandExpression.Parse("./"));
			Assert.Throws<CommandExpressionException>(() => CommandExpression.Parse("../"));
			Assert.Throws<CommandExpressionException>(() => CommandExpression.Parse("help/"));
			Assert.Throws<CommandExpressionException>(() => CommandExpression.Parse("/help/"));
			Assert.Throws<CommandExpressionException>(() => CommandExpression.Parse("./help/"));
			Assert.Throws<CommandExpressionException>(() => CommandExpression.Parse("../help/"));
			Assert.Throws<CommandExpressionException>(() => CommandExpression.Parse("queue.in/"));
			Assert.Throws<CommandExpressionException>(() => CommandExpression.Parse("/queue.in/"));
			Assert.Throws<CommandExpressionException>(() => CommandExpression.Parse("./queue.in/"));
			Assert.Throws<CommandExpressionException>(() => CommandExpression.Parse("../queue.in/"));
		}
	}
}

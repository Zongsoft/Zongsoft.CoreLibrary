using System;
using System.Collections.Generic;

using Xunit;

namespace Zongsoft.Expressions.Tests
{
	public class LexerTest
	{
		[Fact]
		public void Test()
		{
			const string EXPRESSION = @"1+2f	_abc123'text\'suffix'	-30L*4.5 / 5.5m (true || FALSE?yes:no)null??nothing";

			var scanner = Lexer.Instance.GetScanner(EXPRESSION);
			Assert.NotNull(scanner);

			var token = scanner.Scan();
			Assert.NotNull(token);
			Assert.Equal(TokenType.Constant, token.Type);
			Assert.IsType(typeof(int), token.Value);
			Assert.Equal(1, (int)token.Value);

			token = scanner.Scan();
			Assert.NotNull(token);
			Assert.Equal(SymbolToken.Plus, token);

			token = scanner.Scan();
			Assert.NotNull(token);
			Assert.Equal(TokenType.Constant, token.Type);
			Assert.IsType(typeof(float), token.Value);
			Assert.Equal(2.0f, (float)token.Value);

			token = scanner.Scan();
			Assert.NotNull(token);
			Assert.Equal(TokenType.Identifier, token.Type);
			Assert.IsType(typeof(string), token.Value);
			Assert.Equal("_abc123", (string)token.Value);

			token = scanner.Scan();
			Assert.NotNull(token);
			Assert.Equal(TokenType.Constant, token.Type);
			Assert.IsType(typeof(string), token.Value);
			Assert.Equal("text'suffix", (string)token.Value);

			token = scanner.Scan();
			Assert.NotNull(token);
			Assert.Equal(SymbolToken.Minus, token);

			token = scanner.Scan();
			Assert.NotNull(token);
			Assert.Equal(TokenType.Constant, token.Type);
			Assert.IsType(typeof(long), token.Value);
			Assert.Equal(30L, (long)token.Value);

			token = scanner.Scan();
			Assert.NotNull(token);
			Assert.Equal(SymbolToken.Multiply, token);

			token = scanner.Scan();
			Assert.NotNull(token);
			Assert.Equal(TokenType.Constant, token.Type);
			Assert.IsType(typeof(double), token.Value);
			Assert.Equal(4.5, (double)token.Value);

			token = scanner.Scan();
			Assert.NotNull(token);
			Assert.Equal(SymbolToken.Divide, token);

			token = scanner.Scan();
			Assert.NotNull(token);
			Assert.Equal(TokenType.Constant, token.Type);
			Assert.IsType(typeof(decimal), token.Value);
			Assert.Equal(5.5m, (decimal)token.Value);

			token = scanner.Scan();
			Assert.NotNull(token);
			Assert.Equal(SymbolToken.OpeningParenthesis, token);

			token = scanner.Scan();
			Assert.NotNull(token);
			Assert.Equal(Token.True, token);

			token = scanner.Scan();
			Assert.NotNull(token);
			Assert.Equal(SymbolToken.OrElse, token);

			token = scanner.Scan();
			Assert.NotNull(token);
			Assert.Equal(Token.False, token);

			token = scanner.Scan();
			Assert.NotNull(token);
			Assert.Equal(SymbolToken.Question, token);

			token = scanner.Scan();
			Assert.NotNull(token);
			Assert.Equal(TokenType.Identifier, token.Type);
			Assert.IsType(typeof(string), token.Value);
			Assert.Equal("yes", (string)token.Value);

			token = scanner.Scan();
			Assert.NotNull(token);
			Assert.Equal(SymbolToken.Colon, token);

			token = scanner.Scan();
			Assert.NotNull(token);
			Assert.Equal(TokenType.Identifier, token.Type);
			Assert.IsType(typeof(string), token.Value);
			Assert.Equal("no", (string)token.Value);

			token = scanner.Scan();
			Assert.NotNull(token);
			Assert.Equal(SymbolToken.ClosingParenthesis, token);

			token = scanner.Scan();
			Assert.NotNull(token);
			Assert.Equal(Token.Null, token);

			token = scanner.Scan();
			Assert.NotNull(token);
			Assert.Equal(SymbolToken.Coalesce, token);

			token = scanner.Scan();
			Assert.NotNull(token);
			Assert.Equal(TokenType.Identifier, token.Type);
			Assert.IsType(typeof(string), token.Value);
			Assert.Equal("nothing", (string)token.Value);

			token = scanner.Scan();
			Assert.Null(token);
		}
	}
}

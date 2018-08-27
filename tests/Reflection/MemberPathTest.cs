using System;
using System.Collections.Generic;

using Xunit;

namespace Zongsoft.Reflection.Expressions.Tests
{
	public class MemberPathTest
	{
		[Fact]
		public void Test()
		{
			Test1();
			Test2();
			Test3();
			Test4();
		}

		[Fact]
		public void Test1()
		{
			IMemberExpression expression;

			Assert.False(MemberExpression.TryParse("", out expression));
			Assert.False(MemberExpression.TryParse("  \t  ", out expression));

			Assert.True(MemberExpression.TryParse("  abc ", out expression));
			Assert.Equal(MemberExpressionType.Identifier, expression.ExpressionType);
			Assert.Equal("abc", ((IdentifierExpression)expression).Name);
			Assert.Null(expression.Next);
			Assert.Null(expression.Previous);

			Assert.True(MemberExpression.TryParse("abc (  ) ", out expression));
			Assert.Equal(MemberExpressionType.Method, expression.ExpressionType);
			Assert.Equal("abc", ((MethodExpression)expression).Name);
			Assert.Equal(0, ((MethodExpression)expression).Arguments.Count);
			Assert.Null(expression.Previous);
			Assert.Null(expression.Next);

			Assert.True(MemberExpression.TryParse("abc ( def) ", out expression));
			Assert.Equal(MemberExpressionType.Method, expression.ExpressionType);
			Assert.Equal("abc", ((MethodExpression)expression).Name);
			Assert.Equal(1, ((MethodExpression)expression).Arguments.Count);
			Assert.Equal(MemberExpressionType.Identifier, ((MethodExpression)expression).Arguments[0].ExpressionType);
			Assert.Equal("def", ((IdentifierExpression)((MethodExpression)expression).Arguments[0]).Name);
			Assert.Null(expression.Previous);
			Assert.Null(expression.Next);

			Assert.True(MemberExpression.TryParse("abc ( 'xyz' ) ", out expression));
			Assert.Equal(MemberExpressionType.Method, expression.ExpressionType);
			Assert.Equal("abc", ((MethodExpression)expression).Name);
			Assert.Equal(1, ((MethodExpression)expression).Arguments.Count);
			Assert.Equal(MemberExpressionType.Constant, ((MethodExpression)expression).Arguments[0].ExpressionType);
			Assert.Equal("xyz", ((ConstantExpression)((MethodExpression)expression).Arguments[0]).Value);
			Assert.Null(expression.Previous);
			Assert.Null(expression.Next);

			Assert.True(MemberExpression.TryParse("abc ( 123 ) ", out expression));
			Assert.Equal(MemberExpressionType.Method, expression.ExpressionType);
			Assert.Equal("abc", ((MethodExpression)expression).Name);
			Assert.Equal(1, ((MethodExpression)expression).Arguments.Count);
			Assert.Equal(MemberExpressionType.Constant, ((MethodExpression)expression).Arguments[0].ExpressionType);
			Assert.Equal(123, ((ConstantExpression)((MethodExpression)expression).Arguments[0]).Value);
			Assert.Null(expression.Previous);
			Assert.Null(expression.Next);

			Assert.True(MemberExpression.TryParse("abc ( 123.45f ) ", out expression));
			Assert.Equal(MemberExpressionType.Method, expression.ExpressionType);
			Assert.Equal("abc", ((MethodExpression)expression).Name);
			Assert.Equal(1, ((MethodExpression)expression).Arguments.Count);
			Assert.Equal(MemberExpressionType.Constant, ((MethodExpression)expression).Arguments[0].ExpressionType);
			Assert.Equal(123.45f, ((ConstantExpression)((MethodExpression)expression).Arguments[0]).Value);
			Assert.Null(expression.Previous);
			Assert.Null(expression.Next);

			Assert.True(MemberExpression.TryParse("[  123 ]", out expression));
			Assert.Equal(MemberExpressionType.Indexer, expression.ExpressionType);
			Assert.Equal(1, ((IndexerExpression)expression).Arguments.Count);
			Assert.Equal(MemberExpressionType.Constant, ((IndexerExpression)expression).Arguments[0].ExpressionType);
			Assert.Equal(123, ((ConstantExpression)((IndexerExpression)expression).Arguments[0]).Value);
			Assert.Null(expression.Next);
			Assert.Null(expression.Previous);

			Assert.True(MemberExpression.TryParse("[  'very sexy' ]", out expression));
			Assert.Equal(MemberExpressionType.Indexer, expression.ExpressionType);
			Assert.Equal(1, ((IndexerExpression)expression).Arguments.Count);
			Assert.Equal(MemberExpressionType.Constant, ((IndexerExpression)expression).Arguments[0].ExpressionType);
			Assert.Equal("very sexy", ((ConstantExpression)((IndexerExpression)expression).Arguments[0]).Value);
			Assert.Null(expression.Next);
			Assert.Null(expression.Previous);

			Assert.True(MemberExpression.TryParse("[  123.45 ]", out expression));
			Assert.Equal(MemberExpressionType.Indexer, expression.ExpressionType);
			Assert.Equal(1, ((IndexerExpression)expression).Arguments.Count);
			Assert.Equal(MemberExpressionType.Constant, ((IndexerExpression)expression).Arguments[0].ExpressionType);
			Assert.Equal(123.45, ((ConstantExpression)((IndexerExpression)expression).Arguments[0]).Value);
			Assert.Null(expression.Next);
			Assert.Null(expression.Previous);

			Assert.True(MemberExpression.TryParse("[  123.45m ]", out expression));
			Assert.Equal(MemberExpressionType.Indexer, expression.ExpressionType);
			Assert.Equal(1, ((IndexerExpression)expression).Arguments.Count);
			Assert.Equal(MemberExpressionType.Constant, ((IndexerExpression)expression).Arguments[0].ExpressionType);
			Assert.Equal(123.45M, ((ConstantExpression)((IndexerExpression)expression).Arguments[0]).Value);
			Assert.Null(expression.Next);
			Assert.Null(expression.Previous);

			Assert.True(MemberExpression.TryParse("[  abc ]", out expression));
			Assert.Equal(MemberExpressionType.Indexer, expression.ExpressionType);
			Assert.Equal(1, ((IndexerExpression)expression).Arguments.Count);
			Assert.Equal(MemberExpressionType.Identifier, ((IndexerExpression)expression).Arguments[0].ExpressionType);
			Assert.Equal("abc", ((IdentifierExpression)((IndexerExpression)expression).Arguments[0]).Name);
			Assert.Null(expression.Next);
			Assert.Null(expression.Previous);

			Assert.True(MemberExpression.TryParse("[ a . b ]", out _));
			Assert.True(MemberExpression.TryParse("[ a . bc]", out _));
			Assert.True(MemberExpression.TryParse(" [ abc . def . xyz , 'OK',100] ", out _));
			Assert.True(MemberExpression.TryParse(" ab . cd ", out _));

			Assert.False(MemberExpression.TryParse("[]", out _));
			Assert.False(MemberExpression.TryParse("[ a b]", out _));
			Assert.False(MemberExpression.TryParse("[12 3]", out _));
			Assert.False(MemberExpression.TryParse("ab cd", out _));
			Assert.False(MemberExpression.TryParse("1234", out _));
			Assert.False(MemberExpression.TryParse("1 'xyz'", out _));
		}

		[Fact]
		public void Test2()
		{
			IMemberExpression expression;

			Assert.True(MemberExpression.TryParse(" abc .  def ", out expression));
			Assert.Equal(MemberExpressionType.Identifier, expression.ExpressionType);
			Assert.Equal("abc", ((IdentifierExpression)expression).Name);
			Assert.Null(expression.Previous);
			Assert.NotNull(expression.Next);
			expression = expression.Next;
			Assert.Equal(MemberExpressionType.Identifier, expression.ExpressionType);
			Assert.Equal("def", ((IdentifierExpression)expression).Name);
			Assert.NotNull(expression.Previous);
			Assert.Null(expression.Next);

			Assert.True(MemberExpression.TryParse(" abc [def] ", out expression));
			Assert.Equal(MemberExpressionType.Identifier, expression.ExpressionType);
			Assert.Equal("abc", ((IdentifierExpression)expression).Name);
			Assert.Null(expression.Previous);
			Assert.NotNull(expression.Next);
			expression = expression.Next;
			Assert.Equal(MemberExpressionType.Indexer, expression.ExpressionType);
			Assert.Equal(1, ((IndexerExpression)expression).Arguments.Count);
			Assert.Equal(MemberExpressionType.Identifier, ((IndexerExpression)expression).Arguments[0].ExpressionType);
			Assert.Equal("def", ((IdentifierExpression)((IndexerExpression)expression).Arguments[0]).Name);
			Assert.NotNull(expression.Previous);
			Assert.Null(expression.Next);

			Assert.True(MemberExpression.TryParse("abc ( def, xyz) ", out expression));
			Assert.Equal(MemberExpressionType.Method, expression.ExpressionType);
			Assert.Equal("abc", ((MethodExpression)expression).Name);
			Assert.Equal(2, ((MethodExpression)expression).Arguments.Count);
			Assert.Equal(MemberExpressionType.Identifier, ((MethodExpression)expression).Arguments[0].ExpressionType);
			Assert.Equal("def", ((IdentifierExpression)((MethodExpression)expression).Arguments[0]).Name);
			Assert.Equal(MemberExpressionType.Identifier, ((MethodExpression)expression).Arguments[1].ExpressionType);
			Assert.Equal("xyz", ((IdentifierExpression)((MethodExpression)expression).Arguments[1]).Name);
			Assert.Null(expression.Previous);
			Assert.Null(expression.Next);

			Assert.True(MemberExpression.TryParse(" abc ( def, 123 ) ", out expression));
			Assert.Equal(MemberExpressionType.Method, expression.ExpressionType);
			Assert.Equal("abc", ((MethodExpression)expression).Name);
			Assert.Equal(2, ((MethodExpression)expression).Arguments.Count);
			Assert.Equal(MemberExpressionType.Identifier, ((MethodExpression)expression).Arguments[0].ExpressionType);
			Assert.Equal("def", ((IdentifierExpression)((MethodExpression)expression).Arguments[0]).Name);
			Assert.Equal(MemberExpressionType.Constant, ((MethodExpression)expression).Arguments[1].ExpressionType);
			Assert.Equal(123, ((ConstantExpression)((MethodExpression)expression).Arguments[1]).Value);
			Assert.Null(expression.Previous);
			Assert.Null(expression.Next);

			Assert.True(MemberExpression.TryParse("aaa.	bbb ( def, xyz) ", out expression));
			Assert.Equal(MemberExpressionType.Identifier, expression.ExpressionType);
			Assert.Equal("aaa", ((IdentifierExpression)expression).Name);
			Assert.Null(expression.Previous);
			Assert.NotNull(expression.Next);
			expression = expression.Next;
			Assert.Equal(MemberExpressionType.Method, expression.ExpressionType);
			Assert.Equal("bbb", ((MethodExpression)expression).Name);
			Assert.Equal(2, ((MethodExpression)expression).Arguments.Count);
			Assert.Equal(MemberExpressionType.Identifier, ((MethodExpression)expression).Arguments[0].ExpressionType);
			Assert.Equal("def", ((IdentifierExpression)((MethodExpression)expression).Arguments[0]).Name);
			Assert.Equal(MemberExpressionType.Identifier, ((MethodExpression)expression).Arguments[1].ExpressionType);
			Assert.Equal("xyz", ((IdentifierExpression)((MethodExpression)expression).Arguments[1]).Name);
			Assert.NotNull(expression.Previous);
			Assert.Null(expression.Next);

			Assert.True(MemberExpression.TryParse("aaa.	bbb ( 'def', xyz) ", out expression));
			Assert.Equal(MemberExpressionType.Identifier, expression.ExpressionType);
			Assert.Equal("aaa", ((IdentifierExpression)expression).Name);
			Assert.Null(expression.Previous);
			Assert.NotNull(expression.Next);
			expression = expression.Next;
			Assert.Equal(MemberExpressionType.Method, expression.ExpressionType);
			Assert.Equal("bbb", ((MethodExpression)expression).Name);
			Assert.Equal(2, ((MethodExpression)expression).Arguments.Count);
			Assert.Equal(MemberExpressionType.Constant, ((MethodExpression)expression).Arguments[0].ExpressionType);
			Assert.Equal("def", ((ConstantExpression)((MethodExpression)expression).Arguments[0]).Value);
			Assert.Equal(MemberExpressionType.Identifier, ((MethodExpression)expression).Arguments[1].ExpressionType);
			Assert.Equal("xyz", ((IdentifierExpression)((MethodExpression)expression).Arguments[1]).Name);
			Assert.NotNull(expression.Previous);
			Assert.Null(expression.Next);

			Assert.True(MemberExpression.TryParse("aaa ( 'def', xyz).bbb ", out expression));
			Assert.Equal(MemberExpressionType.Method, expression.ExpressionType);
			Assert.Equal("aaa", ((MethodExpression)expression).Name);
			Assert.Equal(2, ((MethodExpression)expression).Arguments.Count);
			Assert.Equal(MemberExpressionType.Constant, ((MethodExpression)expression).Arguments[0].ExpressionType);
			Assert.Equal("def", ((ConstantExpression)((MethodExpression)expression).Arguments[0]).Value);
			Assert.Equal(MemberExpressionType.Identifier, ((MethodExpression)expression).Arguments[1].ExpressionType);
			Assert.Equal("xyz", ((IdentifierExpression)((MethodExpression)expression).Arguments[1]).Name);
			Assert.Null(expression.Previous);
			Assert.NotNull(expression.Next);
			expression = expression.Next;
			Assert.Equal(MemberExpressionType.Identifier, expression.ExpressionType);
			Assert.Equal("bbb", ((IdentifierExpression)expression).Name);
			Assert.NotNull(expression.Previous);
			Assert.Null(expression.Next);

			Assert.True(MemberExpression.TryParse("aaa ( 'def', xyz) [123] ", out expression));
			Assert.Equal(MemberExpressionType.Method, expression.ExpressionType);
			Assert.Equal("aaa", ((MethodExpression)expression).Name);
			Assert.Equal(2, ((MethodExpression)expression).Arguments.Count);
			Assert.Equal(MemberExpressionType.Constant, ((MethodExpression)expression).Arguments[0].ExpressionType);
			Assert.Equal("def", ((ConstantExpression)((MethodExpression)expression).Arguments[0]).Value);
			Assert.Equal(MemberExpressionType.Identifier, ((MethodExpression)expression).Arguments[1].ExpressionType);
			Assert.Equal("xyz", ((IdentifierExpression)((MethodExpression)expression).Arguments[1]).Name);
			Assert.Null(expression.Previous);
			Assert.NotNull(expression.Next);
			expression = expression.Next;
			Assert.Equal(MemberExpressionType.Indexer, expression.ExpressionType);
			Assert.Equal(1, ((IndexerExpression)expression).Arguments.Count);
			Assert.Equal(MemberExpressionType.Constant, ((IndexerExpression)expression).Arguments[0].ExpressionType);
			Assert.Equal(123, ((ConstantExpression)((IndexerExpression)expression).Arguments[0]).Value);
			Assert.NotNull(expression.Previous);
			Assert.Null(expression.Next);

			Assert.True(MemberExpression.TryParse("[123L, 789.01m]", out expression));
			Assert.Equal(MemberExpressionType.Indexer, expression.ExpressionType);
			Assert.Equal(2, ((IndexerExpression)expression).Arguments.Count);
			Assert.Equal(MemberExpressionType.Constant, ((IndexerExpression)expression).Arguments[0].ExpressionType);
			Assert.Equal(123L, ((ConstantExpression)((IndexerExpression)expression).Arguments[0]).Value);
			Assert.Equal(MemberExpressionType.Constant, ((IndexerExpression)expression).Arguments[1].ExpressionType);
			Assert.Equal(789.01m, ((ConstantExpression)((IndexerExpression)expression).Arguments[1]).Value);
			Assert.Null(expression.Next);
			Assert.Null(expression.Previous);

			Assert.True(MemberExpression.TryParse("[123, abc ]", out expression));
			Assert.Equal(MemberExpressionType.Indexer, expression.ExpressionType);
			Assert.Equal(2, ((IndexerExpression)expression).Arguments.Count);
			Assert.Equal(MemberExpressionType.Constant, ((IndexerExpression)expression).Arguments[0].ExpressionType);
			Assert.Equal(123, ((ConstantExpression)((IndexerExpression)expression).Arguments[0]).Value);
			Assert.Equal(MemberExpressionType.Identifier, ((IndexerExpression)expression).Arguments[1].ExpressionType);
			Assert.Equal("abc", ((IdentifierExpression)((IndexerExpression)expression).Arguments[1]).Name);
			Assert.Null(expression.Next);
			Assert.Null(expression.Previous);

			Assert.True(MemberExpression.TryParse("[abc , 'OK' ]", out expression));
			Assert.Equal(MemberExpressionType.Indexer, expression.ExpressionType);
			Assert.Equal(2, ((IndexerExpression)expression).Arguments.Count);
			Assert.Equal(MemberExpressionType.Identifier, ((IndexerExpression)expression).Arguments[0].ExpressionType);
			Assert.Equal("abc", ((IdentifierExpression)((IndexerExpression)expression).Arguments[0]).Name);
			Assert.Equal(MemberExpressionType.Constant, ((IndexerExpression)expression).Arguments[1].ExpressionType);
			Assert.Equal("OK", ((ConstantExpression)((IndexerExpression)expression).Arguments[1]).Value);
			Assert.Null(expression.Next);
			Assert.Null(expression.Previous);

			Assert.True(MemberExpression.TryParse("[123] .	bbb ( 'def', xyz) ", out expression));
			Assert.Equal(MemberExpressionType.Indexer, expression.ExpressionType);
			Assert.Equal(1, ((IndexerExpression)expression).Arguments.Count);
			Assert.Equal(MemberExpressionType.Constant, ((IndexerExpression)expression).Arguments[0].ExpressionType);
			Assert.Equal(123, ((ConstantExpression)((IndexerExpression)expression).Arguments[0]).Value);
			Assert.Null(expression.Previous);
			Assert.NotNull(expression.Next);
			expression = expression.Next;
			Assert.Equal(MemberExpressionType.Method, expression.ExpressionType);
			Assert.Equal("bbb", ((MethodExpression)expression).Name);
			Assert.Equal(2, ((MethodExpression)expression).Arguments.Count);
			Assert.Equal(MemberExpressionType.Constant, ((MethodExpression)expression).Arguments[0].ExpressionType);
			Assert.Equal("def", ((ConstantExpression)((MethodExpression)expression).Arguments[0]).Value);
			Assert.Equal(MemberExpressionType.Identifier, ((MethodExpression)expression).Arguments[1].ExpressionType);
			Assert.Equal("xyz", ((IdentifierExpression)((MethodExpression)expression).Arguments[1]).Name);
			Assert.NotNull(expression.Previous);
			Assert.Null(expression.Next);
		}

		[Fact]
		public void Test3()
		{
			IMemberExpression expression;

			Assert.True(MemberExpression.TryParse(" abc .  def .xyz", out expression));
			Assert.Equal(MemberExpressionType.Identifier, expression.ExpressionType);
			Assert.Equal("abc", ((IdentifierExpression)expression).Name);
			Assert.Null(expression.Previous);
			Assert.NotNull(expression.Next);
			expression = expression.Next;
			Assert.Equal(MemberExpressionType.Identifier, expression.ExpressionType);
			Assert.Equal("def", ((IdentifierExpression)expression).Name);
			Assert.NotNull(expression.Previous);
			Assert.NotNull(expression.Next);
			expression = expression.Next;
			Assert.Equal(MemberExpressionType.Identifier, expression.ExpressionType);
			Assert.Equal("xyz", ((IdentifierExpression)expression).Name);
			Assert.NotNull(expression.Previous);
			Assert.Null(expression.Next);

			Assert.True(MemberExpression.TryParse("abc [  def] .xyz", out expression));
			Assert.Equal(MemberExpressionType.Identifier, expression.ExpressionType);
			Assert.Equal("abc", ((IdentifierExpression)expression).Name);
			Assert.Null(expression.Previous);
			Assert.NotNull(expression.Next);
			expression = expression.Next;
			Assert.Equal(MemberExpressionType.Indexer, expression.ExpressionType);
			Assert.Equal(1, ((IndexerExpression)expression).Arguments.Count);
			Assert.Equal(MemberExpressionType.Identifier, ((IndexerExpression)expression).Arguments[0].ExpressionType);
			Assert.Equal("def", ((IdentifierExpression)((IndexerExpression)expression).Arguments[0]).Name);
			Assert.NotNull(expression.Previous);
			Assert.NotNull(expression.Next);
			expression = expression.Next;
			Assert.Equal(MemberExpressionType.Identifier, expression.ExpressionType);
			Assert.Equal("xyz", ((IdentifierExpression)expression).Name);
			Assert.NotNull(expression.Previous);
			Assert.Null(expression.Next);

			Assert.True(MemberExpression.TryParse("[abc]  . def .xyz", out expression));
			Assert.Equal(MemberExpressionType.Indexer, expression.ExpressionType);
			Assert.Equal(1, ((IndexerExpression)expression).Arguments.Count);
			Assert.Equal(MemberExpressionType.Identifier, ((IndexerExpression)expression).Arguments[0].ExpressionType);
			Assert.Equal("abc", ((IdentifierExpression)((IndexerExpression)expression).Arguments[0]).Name);
			Assert.Null(expression.Previous);
			Assert.NotNull(expression.Next);
			expression = expression.Next;
			Assert.Equal(MemberExpressionType.Identifier, expression.ExpressionType);
			Assert.Equal("def", ((IdentifierExpression)expression).Name);
			Assert.NotNull(expression.Previous);
			Assert.NotNull(expression.Next);
			expression = expression.Next;
			Assert.Equal(MemberExpressionType.Identifier, expression.ExpressionType);
			Assert.Equal("xyz", ((IdentifierExpression)expression).Name);
			Assert.NotNull(expression.Previous);
			Assert.Null(expression.Next);

			Assert.True(MemberExpression.TryParse(" [abc ]  [ def ] . xyz", out expression));
			Assert.Equal(MemberExpressionType.Indexer, expression.ExpressionType);
			Assert.Equal(1, ((IndexerExpression)expression).Arguments.Count);
			Assert.Equal(MemberExpressionType.Identifier, ((IndexerExpression)expression).Arguments[0].ExpressionType);
			Assert.Equal("abc", ((IdentifierExpression)((IndexerExpression)expression).Arguments[0]).Name);
			Assert.Null(expression.Previous);
			Assert.NotNull(expression.Next);
			expression = expression.Next;
			Assert.Equal(MemberExpressionType.Indexer, expression.ExpressionType);
			Assert.Equal(1, ((IndexerExpression)expression).Arguments.Count);
			Assert.Equal(MemberExpressionType.Identifier, ((IndexerExpression)expression).Arguments[0].ExpressionType);
			Assert.Equal("def", ((IdentifierExpression)((IndexerExpression)expression).Arguments[0]).Name);
			Assert.NotNull(expression.Previous);
			Assert.NotNull(expression.Next);
			expression = expression.Next;
			Assert.Equal(MemberExpressionType.Identifier, expression.ExpressionType);
			Assert.Equal("xyz", ((IdentifierExpression)expression).Name);
			Assert.NotNull(expression.Previous);
			Assert.Null(expression.Next);

			Assert.True(MemberExpression.TryParse(" [abc ]  [ 123 ] . xyz", out expression));
			Assert.Equal(MemberExpressionType.Indexer, expression.ExpressionType);
			Assert.Equal(1, ((IndexerExpression)expression).Arguments.Count);
			Assert.Equal(MemberExpressionType.Identifier, ((IndexerExpression)expression).Arguments[0].ExpressionType);
			Assert.Equal("abc", ((IdentifierExpression)((IndexerExpression)expression).Arguments[0]).Name);
			Assert.Null(expression.Previous);
			Assert.NotNull(expression.Next);
			expression = expression.Next;
			Assert.Equal(MemberExpressionType.Indexer, expression.ExpressionType);
			Assert.Equal(1, ((IndexerExpression)expression).Arguments.Count);
			Assert.Equal(MemberExpressionType.Constant, ((IndexerExpression)expression).Arguments[0].ExpressionType);
			Assert.Equal(123, ((ConstantExpression)((IndexerExpression)expression).Arguments[0]).Value);
			Assert.NotNull(expression.Previous);
			Assert.NotNull(expression.Next);
			expression = expression.Next;
			Assert.Equal(MemberExpressionType.Identifier, expression.ExpressionType);
			Assert.Equal("xyz", ((IdentifierExpression)expression).Name);
			Assert.NotNull(expression.Previous);
			Assert.Null(expression.Next);

			Assert.True(MemberExpression.TryParse(" [abc ]  [ 123 ]['xyz']", out expression));
			Assert.Equal(MemberExpressionType.Indexer, expression.ExpressionType);
			Assert.Equal(1, ((IndexerExpression)expression).Arguments.Count);
			Assert.Equal(MemberExpressionType.Identifier, ((IndexerExpression)expression).Arguments[0].ExpressionType);
			Assert.Equal("abc", ((IdentifierExpression)((IndexerExpression)expression).Arguments[0]).Name);
			Assert.Null(expression.Previous);
			Assert.NotNull(expression.Next);
			expression = expression.Next;
			Assert.Equal(MemberExpressionType.Indexer, expression.ExpressionType);
			Assert.Equal(1, ((IndexerExpression)expression).Arguments.Count);
			Assert.Equal(MemberExpressionType.Constant, ((IndexerExpression)expression).Arguments[0].ExpressionType);
			Assert.Equal(123, ((ConstantExpression)((IndexerExpression)expression).Arguments[0]).Value);
			Assert.NotNull(expression.Previous);
			Assert.NotNull(expression.Next);
			expression = expression.Next;
			Assert.Equal(MemberExpressionType.Indexer, expression.ExpressionType);
			Assert.Equal(1, ((IndexerExpression)expression).Arguments.Count);
			Assert.Equal(MemberExpressionType.Constant, ((IndexerExpression)expression).Arguments[0].ExpressionType);
			Assert.Equal("xyz", ((ConstantExpression)((IndexerExpression)expression).Arguments[0]).Value);
			Assert.NotNull(expression.Previous);
			Assert.Null(expression.Next);

			Assert.True(MemberExpression.TryParse(" [abc ]  [ 123 ].xyz()", out expression));
			Assert.Equal(MemberExpressionType.Indexer, expression.ExpressionType);
			Assert.Equal(1, ((IndexerExpression)expression).Arguments.Count);
			Assert.Equal(MemberExpressionType.Identifier, ((IndexerExpression)expression).Arguments[0].ExpressionType);
			Assert.Equal("abc", ((IdentifierExpression)((IndexerExpression)expression).Arguments[0]).Name);
			Assert.Null(expression.Previous);
			Assert.NotNull(expression.Next);
			expression = expression.Next;
			Assert.Equal(MemberExpressionType.Indexer, expression.ExpressionType);
			Assert.Equal(1, ((IndexerExpression)expression).Arguments.Count);
			Assert.Equal(MemberExpressionType.Constant, ((IndexerExpression)expression).Arguments[0].ExpressionType);
			Assert.Equal(123, ((ConstantExpression)((IndexerExpression)expression).Arguments[0]).Value);
			Assert.NotNull(expression.Previous);
			Assert.NotNull(expression.Next);
			expression = expression.Next;
			Assert.Equal(MemberExpressionType.Method, expression.ExpressionType);
			Assert.Equal("xyz", ((MethodExpression)expression).Name);
			Assert.Equal(0, ((MethodExpression)expression).Arguments.Count);
			Assert.NotNull(expression.Previous);
			Assert.Null(expression.Next);

			Assert.True(MemberExpression.TryParse(" [abc ]  .foo( 123 ).xyz ( 'OK')", out expression));
			Assert.Equal(MemberExpressionType.Indexer, expression.ExpressionType);
			Assert.Equal(1, ((IndexerExpression)expression).Arguments.Count);
			Assert.Equal(MemberExpressionType.Identifier, ((IndexerExpression)expression).Arguments[0].ExpressionType);
			Assert.Equal("abc", ((IdentifierExpression)((IndexerExpression)expression).Arguments[0]).Name);
			Assert.Null(expression.Previous);
			Assert.NotNull(expression.Next);
			expression = expression.Next;
			Assert.Equal(MemberExpressionType.Method, expression.ExpressionType);
			Assert.Equal("foo", ((MethodExpression)expression).Name);
			Assert.Equal(1, ((MethodExpression)expression).Arguments.Count);
			Assert.Equal(MemberExpressionType.Constant, ((MethodExpression)expression).Arguments[0].ExpressionType);
			Assert.Equal(123, ((ConstantExpression)((MethodExpression)expression).Arguments[0]).Value);
			Assert.NotNull(expression.Previous);
			Assert.NotNull(expression.Next);
			expression = expression.Next;
			Assert.Equal(MemberExpressionType.Method, expression.ExpressionType);
			Assert.Equal("xyz", ((MethodExpression)expression).Name);
			Assert.Equal(1, ((MethodExpression)expression).Arguments.Count);
			Assert.Equal(MemberExpressionType.Constant, ((MethodExpression)expression).Arguments[0].ExpressionType);
			Assert.Equal("OK", ((ConstantExpression)((MethodExpression)expression).Arguments[0]).Value);
			Assert.NotNull(expression.Previous);
			Assert.Null(expression.Next);
		}

		[Fact]
		public void Test4()
		{
			IMemberExpression expression;

			Assert.True(MemberExpression.TryParse("[123L, 789.01m,a.b.c]", out expression));
			Assert.Equal(MemberExpressionType.Indexer, expression.ExpressionType);
			Assert.Equal(3, ((IndexerExpression)expression).Arguments.Count);
			Assert.Equal(MemberExpressionType.Constant, ((IndexerExpression)expression).Arguments[0].ExpressionType);
			Assert.Equal(123L, ((ConstantExpression)((IndexerExpression)expression).Arguments[0]).Value);
			Assert.Equal(MemberExpressionType.Constant, ((IndexerExpression)expression).Arguments[1].ExpressionType);
			Assert.Equal(789.01m, ((ConstantExpression)((IndexerExpression)expression).Arguments[1]).Value);

			var argument = ((IndexerExpression)expression).Arguments[2];
			Assert.Equal(MemberExpressionType.Identifier, argument.ExpressionType);
			Assert.Equal("a", ((IdentifierExpression)argument).Name);
			Assert.Null(argument.Previous);
			Assert.NotNull(argument.Next);
			argument = argument.Next;
			Assert.Equal(MemberExpressionType.Identifier, argument.ExpressionType);
			Assert.Equal("b", ((IdentifierExpression)argument).Name);
			Assert.NotNull(argument.Previous);
			Assert.NotNull(argument.Next);
			argument = argument.Next;
			Assert.Equal(MemberExpressionType.Identifier, argument.ExpressionType);
			Assert.Equal("c", ((IdentifierExpression)argument).Name);
			Assert.NotNull(argument.Previous);
			Assert.Null(argument.Next);
			Assert.Null(expression.Next);
			Assert.Null(expression.Previous);

			Assert.True(MemberExpression.TryParse("[123L, [abc]]", out expression));
			Assert.Equal(MemberExpressionType.Indexer, expression.ExpressionType);
			Assert.Equal(2, ((IndexerExpression)expression).Arguments.Count);
			Assert.Equal(MemberExpressionType.Constant, ((IndexerExpression)expression).Arguments[0].ExpressionType);
			Assert.Equal(123L, ((ConstantExpression)((IndexerExpression)expression).Arguments[0]).Value);
			argument = ((IndexerExpression)expression).Arguments[1];
			Assert.Equal(MemberExpressionType.Indexer, argument.ExpressionType);
			Assert.Equal(1, ((IndexerExpression)argument).Arguments.Count);
			Assert.Equal(MemberExpressionType.Identifier, ((IndexerExpression)argument).Arguments[0].ExpressionType);
			Assert.Equal("abc", ((IdentifierExpression)((IndexerExpression)argument).Arguments[0]).Name);

			Assert.True(MemberExpression.TryParse("[[abc], 'OK']", out expression));
			Assert.Equal(MemberExpressionType.Indexer, expression.ExpressionType);
			Assert.Equal(2, ((IndexerExpression)expression).Arguments.Count);
			argument = ((IndexerExpression)expression).Arguments[0];
			Assert.Equal(MemberExpressionType.Indexer, argument.ExpressionType);
			Assert.Equal(1, ((IndexerExpression)argument).Arguments.Count);
			Assert.Equal(MemberExpressionType.Identifier, ((IndexerExpression)argument).Arguments[0].ExpressionType);
			Assert.Equal("abc", ((IdentifierExpression)((IndexerExpression)argument).Arguments[0]).Name);
			Assert.Equal(MemberExpressionType.Constant, ((IndexerExpression)expression).Arguments[1].ExpressionType);
			Assert.Equal("OK", ((ConstantExpression)((IndexerExpression)expression).Arguments[1]).Value);

			Assert.True(MemberExpression.TryParse("[[abc], foo()]", out expression));
			Assert.Equal(MemberExpressionType.Indexer, expression.ExpressionType);
			Assert.Equal(2, ((IndexerExpression)expression).Arguments.Count);
			argument = ((IndexerExpression)expression).Arguments[0];
			Assert.Equal(MemberExpressionType.Indexer, argument.ExpressionType);
			Assert.Equal(1, ((IndexerExpression)argument).Arguments.Count);
			Assert.Equal(MemberExpressionType.Identifier, ((IndexerExpression)argument).Arguments[0].ExpressionType);
			Assert.Equal("abc", ((IdentifierExpression)((IndexerExpression)argument).Arguments[0]).Name);
			Assert.Equal(MemberExpressionType.Method, ((IndexerExpression)expression).Arguments[1].ExpressionType);
			argument = ((IndexerExpression)expression).Arguments[1];
			Assert.Equal(MemberExpressionType.Method, argument.ExpressionType);
			Assert.Equal("foo", ((MethodExpression)argument).Name);
			Assert.Equal(0, ((MethodExpression)argument).Arguments.Count);

			Assert.True(MemberExpression.TryParse("[[abc], foo(1, ['OK'], bar(xyz))]", out expression));
			Assert.Equal(MemberExpressionType.Indexer, expression.ExpressionType);
			Assert.Equal(2, ((IndexerExpression)expression).Arguments.Count);
			argument = ((IndexerExpression)expression).Arguments[0];
			Assert.Equal(MemberExpressionType.Indexer, argument.ExpressionType);
			Assert.Equal(1, ((IndexerExpression)argument).Arguments.Count);
			Assert.Equal(MemberExpressionType.Identifier, ((IndexerExpression)argument).Arguments[0].ExpressionType);
			Assert.Equal("abc", ((IdentifierExpression)((IndexerExpression)argument).Arguments[0]).Name);
			Assert.Equal(MemberExpressionType.Method, ((IndexerExpression)expression).Arguments[1].ExpressionType);
			argument = ((IndexerExpression)expression).Arguments[1];
			Assert.Equal(MemberExpressionType.Method, argument.ExpressionType);

			var method = (MethodExpression)argument;
			Assert.Equal("foo", method.Name);
			Assert.Equal(3, method.Arguments.Count);
			Assert.Equal(MemberExpressionType.Constant, method.Arguments[0].ExpressionType);
			Assert.Equal(1, ((ConstantExpression)method.Arguments[0]).Value);
			Assert.Equal(MemberExpressionType.Indexer, method.Arguments[1].ExpressionType);
			Assert.Equal(1, ((IndexerExpression)method.Arguments[1]).Arguments.Count);
			Assert.Equal(MemberExpressionType.Constant, ((IndexerExpression)method.Arguments[1]).Arguments[0].ExpressionType);
			Assert.Equal("OK", ((ConstantExpression)((IndexerExpression)method.Arguments[1]).Arguments[0]).Value);
			Assert.Equal(MemberExpressionType.Method, method.Arguments[2].ExpressionType);
			method = (MethodExpression)method.Arguments[2];
			Assert.Equal(1, method.Arguments.Count);
			Assert.Equal(MemberExpressionType.Identifier, method.Arguments[0].ExpressionType);
			Assert.Equal("xyz", ((IdentifierExpression)method.Arguments[0]).Name);
		}
	}
}

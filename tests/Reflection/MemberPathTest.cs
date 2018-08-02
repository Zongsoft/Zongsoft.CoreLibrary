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
			Test5();
		}

		[Fact]
		public void Test1()
		{
			IMemberPathExpression expression;

			Assert.False(MemberPath.TryParse("", out expression));
			Assert.False(MemberPath.TryParse("  \t  ", out expression));

			Assert.True(MemberPath.TryParse("  abc ", out expression));
			Assert.Equal(MemberPathExpressionType.Identifier, expression.ExpressionType);
			Assert.Equal("abc", ((IdentifierExpression)expression).Name);
			Assert.Null(expression.Next);
			Assert.Null(expression.Previous);

			Assert.True(MemberPath.TryParse("abc (  ) ", out expression));
			Assert.Equal(MemberPathExpressionType.Method, expression.ExpressionType);
			Assert.Equal("abc", ((MethodExpression)expression).Name);
			Assert.Equal(0, ((MethodExpression)expression).Parameters.Count);
			Assert.Null(expression.Previous);
			Assert.Null(expression.Next);

			Assert.True(MemberPath.TryParse("abc ( def) ", out expression));
			Assert.Equal(MemberPathExpressionType.Method, expression.ExpressionType);
			Assert.Equal("abc", ((MethodExpression)expression).Name);
			Assert.Equal(1, ((MethodExpression)expression).Parameters.Count);
			Assert.Equal(MemberPathExpressionType.Identifier, ((MethodExpression)expression).Parameters[0].ExpressionType);
			Assert.Equal("def", ((IdentifierExpression)((MethodExpression)expression).Parameters[0]).Name);
			Assert.Null(expression.Previous);
			Assert.Null(expression.Next);

			Assert.True(MemberPath.TryParse("abc ( 'xyz' ) ", out expression));
			Assert.Equal(MemberPathExpressionType.Method, expression.ExpressionType);
			Assert.Equal("abc", ((MethodExpression)expression).Name);
			Assert.Equal(1, ((MethodExpression)expression).Parameters.Count);
			Assert.Equal(MemberPathExpressionType.Constant, ((MethodExpression)expression).Parameters[0].ExpressionType);
			Assert.Equal("xyz", ((ConstantExpression)((MethodExpression)expression).Parameters[0]).Value);
			Assert.Null(expression.Previous);
			Assert.Null(expression.Next);

			Assert.True(MemberPath.TryParse("abc ( 123 ) ", out expression));
			Assert.Equal(MemberPathExpressionType.Method, expression.ExpressionType);
			Assert.Equal("abc", ((MethodExpression)expression).Name);
			Assert.Equal(1, ((MethodExpression)expression).Parameters.Count);
			Assert.Equal(MemberPathExpressionType.Constant, ((MethodExpression)expression).Parameters[0].ExpressionType);
			Assert.Equal(123, ((ConstantExpression)((MethodExpression)expression).Parameters[0]).Value);
			Assert.Null(expression.Previous);
			Assert.Null(expression.Next);

			Assert.True(MemberPath.TryParse("abc ( 123.45f ) ", out expression));
			Assert.Equal(MemberPathExpressionType.Method, expression.ExpressionType);
			Assert.Equal("abc", ((MethodExpression)expression).Name);
			Assert.Equal(1, ((MethodExpression)expression).Parameters.Count);
			Assert.Equal(MemberPathExpressionType.Constant, ((MethodExpression)expression).Parameters[0].ExpressionType);
			Assert.Equal(123.45f, ((ConstantExpression)((MethodExpression)expression).Parameters[0]).Value);
			Assert.Null(expression.Previous);
			Assert.Null(expression.Next);

			Assert.True(MemberPath.TryParse("[  123 ]", out expression));
			Assert.Equal(MemberPathExpressionType.Indexer, expression.ExpressionType);
			Assert.Equal(1, ((IndexerExpression)expression).Parameters.Count);
			Assert.Equal(MemberPathExpressionType.Constant, ((IndexerExpression)expression).Parameters[0].ExpressionType);
			Assert.Equal(123, ((ConstantExpression)((IndexerExpression)expression).Parameters[0]).Value);
			Assert.Null(expression.Next);
			Assert.Null(expression.Previous);

			Assert.True(MemberPath.TryParse("[  'very sexy' ]", out expression));
			Assert.Equal(MemberPathExpressionType.Indexer, expression.ExpressionType);
			Assert.Equal(1, ((IndexerExpression)expression).Parameters.Count);
			Assert.Equal(MemberPathExpressionType.Constant, ((IndexerExpression)expression).Parameters[0].ExpressionType);
			Assert.Equal("very sexy", ((ConstantExpression)((IndexerExpression)expression).Parameters[0]).Value);
			Assert.Null(expression.Next);
			Assert.Null(expression.Previous);

			Assert.True(MemberPath.TryParse("[  123.45 ]", out expression));
			Assert.Equal(MemberPathExpressionType.Indexer, expression.ExpressionType);
			Assert.Equal(1, ((IndexerExpression)expression).Parameters.Count);
			Assert.Equal(MemberPathExpressionType.Constant, ((IndexerExpression)expression).Parameters[0].ExpressionType);
			Assert.Equal(123.45, ((ConstantExpression)((IndexerExpression)expression).Parameters[0]).Value);
			Assert.Null(expression.Next);
			Assert.Null(expression.Previous);

			Assert.True(MemberPath.TryParse("[  123.45m ]", out expression));
			Assert.Equal(MemberPathExpressionType.Indexer, expression.ExpressionType);
			Assert.Equal(1, ((IndexerExpression)expression).Parameters.Count);
			Assert.Equal(MemberPathExpressionType.Constant, ((IndexerExpression)expression).Parameters[0].ExpressionType);
			Assert.Equal(123.45M, ((ConstantExpression)((IndexerExpression)expression).Parameters[0]).Value);
			Assert.Null(expression.Next);
			Assert.Null(expression.Previous);

			Assert.True(MemberPath.TryParse("[  abc ]", out expression));
			Assert.Equal(MemberPathExpressionType.Indexer, expression.ExpressionType);
			Assert.Equal(1, ((IndexerExpression)expression).Parameters.Count);
			Assert.Equal(MemberPathExpressionType.Identifier, ((IndexerExpression)expression).Parameters[0].ExpressionType);
			Assert.Equal("abc", ((IdentifierExpression)((IndexerExpression)expression).Parameters[0]).Name);
			Assert.Null(expression.Next);
			Assert.Null(expression.Previous);

			Assert.True(MemberPath.TryParse("[ a . b ]", out _));
			Assert.True(MemberPath.TryParse("[ a . bc]", out _));
			Assert.True(MemberPath.TryParse(" [ abc . def . xyz , 'OK',100] ", out _));
			Assert.True(MemberPath.TryParse(" ab . cd ", out _));

			Assert.False(MemberPath.TryParse("[]", out _));
			Assert.False(MemberPath.TryParse("[ a b]", out _));
			Assert.False(MemberPath.TryParse("[12 3]", out _));
			Assert.False(MemberPath.TryParse("ab cd", out _));
			Assert.False(MemberPath.TryParse("1234", out _));
			Assert.False(MemberPath.TryParse("1 'xyz'", out _));
		}

		[Fact]
		public void Test2()
		{
			IMemberPathExpression expression;

			Assert.True(MemberPath.TryParse(" abc .  def ", out expression));
			Assert.Equal(MemberPathExpressionType.Identifier, expression.ExpressionType);
			Assert.Equal("abc", ((IdentifierExpression)expression).Name);
			Assert.Null(expression.Previous);
			Assert.NotNull(expression.Next);
			expression = expression.Next;
			Assert.Equal(MemberPathExpressionType.Identifier, expression.ExpressionType);
			Assert.Equal("def", ((IdentifierExpression)expression).Name);
			Assert.NotNull(expression.Previous);
			Assert.Null(expression.Next);

			Assert.True(MemberPath.TryParse(" abc [def] ", out expression));
			Assert.Equal(MemberPathExpressionType.Identifier, expression.ExpressionType);
			Assert.Equal("abc", ((IdentifierExpression)expression).Name);
			Assert.Null(expression.Previous);
			Assert.NotNull(expression.Next);
			expression = expression.Next;
			Assert.Equal(MemberPathExpressionType.Indexer, expression.ExpressionType);
			Assert.Equal(1, ((IndexerExpression)expression).Parameters.Count);
			Assert.Equal(MemberPathExpressionType.Identifier, ((IndexerExpression)expression).Parameters[0].ExpressionType);
			Assert.Equal("def", ((IdentifierExpression)((IndexerExpression)expression).Parameters[0]).Name);
			Assert.NotNull(expression.Previous);
			Assert.Null(expression.Next);

			Assert.True(MemberPath.TryParse("abc ( def, xyz) ", out expression));
			Assert.Equal(MemberPathExpressionType.Method, expression.ExpressionType);
			Assert.Equal("abc", ((MethodExpression)expression).Name);
			Assert.Equal(2, ((MethodExpression)expression).Parameters.Count);
			Assert.Equal(MemberPathExpressionType.Identifier, ((MethodExpression)expression).Parameters[0].ExpressionType);
			Assert.Equal("def", ((IdentifierExpression)((MethodExpression)expression).Parameters[0]).Name);
			Assert.Equal(MemberPathExpressionType.Identifier, ((MethodExpression)expression).Parameters[1].ExpressionType);
			Assert.Equal("xyz", ((IdentifierExpression)((MethodExpression)expression).Parameters[1]).Name);
			Assert.Null(expression.Previous);
			Assert.Null(expression.Next);

			Assert.True(MemberPath.TryParse(" abc ( def, 123 ) ", out expression));
			Assert.Equal(MemberPathExpressionType.Method, expression.ExpressionType);
			Assert.Equal("abc", ((MethodExpression)expression).Name);
			Assert.Equal(2, ((MethodExpression)expression).Parameters.Count);
			Assert.Equal(MemberPathExpressionType.Identifier, ((MethodExpression)expression).Parameters[0].ExpressionType);
			Assert.Equal("def", ((IdentifierExpression)((MethodExpression)expression).Parameters[0]).Name);
			Assert.Equal(MemberPathExpressionType.Constant, ((MethodExpression)expression).Parameters[1].ExpressionType);
			Assert.Equal(123, ((ConstantExpression)((MethodExpression)expression).Parameters[1]).Value);
			Assert.Null(expression.Previous);
			Assert.Null(expression.Next);

			Assert.True(MemberPath.TryParse("aaa.	bbb ( def, xyz) ", out expression));
			Assert.Equal(MemberPathExpressionType.Identifier, expression.ExpressionType);
			Assert.Equal("aaa", ((IdentifierExpression)expression).Name);
			Assert.Null(expression.Previous);
			Assert.NotNull(expression.Next);
			expression = expression.Next;
			Assert.Equal(MemberPathExpressionType.Method, expression.ExpressionType);
			Assert.Equal("bbb", ((MethodExpression)expression).Name);
			Assert.Equal(2, ((MethodExpression)expression).Parameters.Count);
			Assert.Equal(MemberPathExpressionType.Identifier, ((MethodExpression)expression).Parameters[0].ExpressionType);
			Assert.Equal("def", ((IdentifierExpression)((MethodExpression)expression).Parameters[0]).Name);
			Assert.Equal(MemberPathExpressionType.Identifier, ((MethodExpression)expression).Parameters[1].ExpressionType);
			Assert.Equal("xyz", ((IdentifierExpression)((MethodExpression)expression).Parameters[1]).Name);
			Assert.NotNull(expression.Previous);
			Assert.Null(expression.Next);

			Assert.True(MemberPath.TryParse("aaa.	bbb ( 'def', xyz) ", out expression));
			Assert.Equal(MemberPathExpressionType.Identifier, expression.ExpressionType);
			Assert.Equal("aaa", ((IdentifierExpression)expression).Name);
			Assert.Null(expression.Previous);
			Assert.NotNull(expression.Next);
			expression = expression.Next;
			Assert.Equal(MemberPathExpressionType.Method, expression.ExpressionType);
			Assert.Equal("bbb", ((MethodExpression)expression).Name);
			Assert.Equal(2, ((MethodExpression)expression).Parameters.Count);
			Assert.Equal(MemberPathExpressionType.Constant, ((MethodExpression)expression).Parameters[0].ExpressionType);
			Assert.Equal("def", ((ConstantExpression)((MethodExpression)expression).Parameters[0]).Value);
			Assert.Equal(MemberPathExpressionType.Identifier, ((MethodExpression)expression).Parameters[1].ExpressionType);
			Assert.Equal("xyz", ((IdentifierExpression)((MethodExpression)expression).Parameters[1]).Name);
			Assert.NotNull(expression.Previous);
			Assert.Null(expression.Next);

			Assert.True(MemberPath.TryParse("aaa ( 'def', xyz).bbb ", out expression));
			Assert.Equal(MemberPathExpressionType.Method, expression.ExpressionType);
			Assert.Equal("aaa", ((MethodExpression)expression).Name);
			Assert.Equal(2, ((MethodExpression)expression).Parameters.Count);
			Assert.Equal(MemberPathExpressionType.Constant, ((MethodExpression)expression).Parameters[0].ExpressionType);
			Assert.Equal("def", ((ConstantExpression)((MethodExpression)expression).Parameters[0]).Value);
			Assert.Equal(MemberPathExpressionType.Identifier, ((MethodExpression)expression).Parameters[1].ExpressionType);
			Assert.Equal("xyz", ((IdentifierExpression)((MethodExpression)expression).Parameters[1]).Name);
			Assert.Null(expression.Previous);
			Assert.NotNull(expression.Next);
			expression = expression.Next;
			Assert.Equal(MemberPathExpressionType.Identifier, expression.ExpressionType);
			Assert.Equal("bbb", ((IdentifierExpression)expression).Name);
			Assert.NotNull(expression.Previous);
			Assert.Null(expression.Next);

			Assert.True(MemberPath.TryParse("aaa ( 'def', xyz) [123] ", out expression));
			Assert.Equal(MemberPathExpressionType.Method, expression.ExpressionType);
			Assert.Equal("aaa", ((MethodExpression)expression).Name);
			Assert.Equal(2, ((MethodExpression)expression).Parameters.Count);
			Assert.Equal(MemberPathExpressionType.Constant, ((MethodExpression)expression).Parameters[0].ExpressionType);
			Assert.Equal("def", ((ConstantExpression)((MethodExpression)expression).Parameters[0]).Value);
			Assert.Equal(MemberPathExpressionType.Identifier, ((MethodExpression)expression).Parameters[1].ExpressionType);
			Assert.Equal("xyz", ((IdentifierExpression)((MethodExpression)expression).Parameters[1]).Name);
			Assert.Null(expression.Previous);
			Assert.NotNull(expression.Next);
			expression = expression.Next;
			Assert.Equal(MemberPathExpressionType.Indexer, expression.ExpressionType);
			Assert.Equal(1, ((IndexerExpression)expression).Parameters.Count);
			Assert.Equal(MemberPathExpressionType.Constant, ((IndexerExpression)expression).Parameters[0].ExpressionType);
			Assert.Equal(123, ((ConstantExpression)((IndexerExpression)expression).Parameters[0]).Value);
			Assert.NotNull(expression.Previous);
			Assert.Null(expression.Next);

			Assert.True(MemberPath.TryParse("[123L, 789.01m]", out expression));
			Assert.Equal(MemberPathExpressionType.Indexer, expression.ExpressionType);
			Assert.Equal(2, ((IndexerExpression)expression).Parameters.Count);
			Assert.Equal(MemberPathExpressionType.Constant, ((IndexerExpression)expression).Parameters[0].ExpressionType);
			Assert.Equal(123L, ((ConstantExpression)((IndexerExpression)expression).Parameters[0]).Value);
			Assert.Equal(MemberPathExpressionType.Constant, ((IndexerExpression)expression).Parameters[1].ExpressionType);
			Assert.Equal(789.01m, ((ConstantExpression)((IndexerExpression)expression).Parameters[1]).Value);
			Assert.Null(expression.Next);
			Assert.Null(expression.Previous);

			Assert.True(MemberPath.TryParse("[123, abc ]", out expression));
			Assert.Equal(MemberPathExpressionType.Indexer, expression.ExpressionType);
			Assert.Equal(2, ((IndexerExpression)expression).Parameters.Count);
			Assert.Equal(MemberPathExpressionType.Constant, ((IndexerExpression)expression).Parameters[0].ExpressionType);
			Assert.Equal(123, ((ConstantExpression)((IndexerExpression)expression).Parameters[0]).Value);
			Assert.Equal(MemberPathExpressionType.Identifier, ((IndexerExpression)expression).Parameters[1].ExpressionType);
			Assert.Equal("abc", ((IdentifierExpression)((IndexerExpression)expression).Parameters[1]).Name);
			Assert.Null(expression.Next);
			Assert.Null(expression.Previous);

			Assert.True(MemberPath.TryParse("[abc , 'OK' ]", out expression));
			Assert.Equal(MemberPathExpressionType.Indexer, expression.ExpressionType);
			Assert.Equal(2, ((IndexerExpression)expression).Parameters.Count);
			Assert.Equal(MemberPathExpressionType.Identifier, ((IndexerExpression)expression).Parameters[0].ExpressionType);
			Assert.Equal("abc", ((IdentifierExpression)((IndexerExpression)expression).Parameters[0]).Name);
			Assert.Equal(MemberPathExpressionType.Constant, ((IndexerExpression)expression).Parameters[1].ExpressionType);
			Assert.Equal("OK", ((ConstantExpression)((IndexerExpression)expression).Parameters[1]).Value);
			Assert.Null(expression.Next);
			Assert.Null(expression.Previous);

			Assert.True(MemberPath.TryParse("[123] .	bbb ( 'def', xyz) ", out expression));
			Assert.Equal(MemberPathExpressionType.Indexer, expression.ExpressionType);
			Assert.Equal(1, ((IndexerExpression)expression).Parameters.Count);
			Assert.Equal(MemberPathExpressionType.Constant, ((IndexerExpression)expression).Parameters[0].ExpressionType);
			Assert.Equal(123, ((ConstantExpression)((IndexerExpression)expression).Parameters[0]).Value);
			Assert.Null(expression.Previous);
			Assert.NotNull(expression.Next);
			expression = expression.Next;
			Assert.Equal(MemberPathExpressionType.Method, expression.ExpressionType);
			Assert.Equal("bbb", ((MethodExpression)expression).Name);
			Assert.Equal(2, ((MethodExpression)expression).Parameters.Count);
			Assert.Equal(MemberPathExpressionType.Constant, ((MethodExpression)expression).Parameters[0].ExpressionType);
			Assert.Equal("def", ((ConstantExpression)((MethodExpression)expression).Parameters[0]).Value);
			Assert.Equal(MemberPathExpressionType.Identifier, ((MethodExpression)expression).Parameters[1].ExpressionType);
			Assert.Equal("xyz", ((IdentifierExpression)((MethodExpression)expression).Parameters[1]).Name);
			Assert.NotNull(expression.Previous);
			Assert.Null(expression.Next);
		}

		[Fact]
		public void Test3()
		{
			IMemberPathExpression expression;

			Assert.True(MemberPath.TryParse(" abc .  def .xyz", out expression));
			Assert.Equal(MemberPathExpressionType.Identifier, expression.ExpressionType);
			Assert.Equal("abc", ((IdentifierExpression)expression).Name);
			Assert.Null(expression.Previous);
			Assert.NotNull(expression.Next);
			expression = expression.Next;
			Assert.Equal(MemberPathExpressionType.Identifier, expression.ExpressionType);
			Assert.Equal("def", ((IdentifierExpression)expression).Name);
			Assert.NotNull(expression.Previous);
			Assert.NotNull(expression.Next);
			expression = expression.Next;
			Assert.Equal(MemberPathExpressionType.Identifier, expression.ExpressionType);
			Assert.Equal("xyz", ((IdentifierExpression)expression).Name);
			Assert.NotNull(expression.Previous);
			Assert.Null(expression.Next);

			Assert.True(MemberPath.TryParse("abc [  def] .xyz", out expression));
			Assert.Equal(MemberPathExpressionType.Identifier, expression.ExpressionType);
			Assert.Equal("abc", ((IdentifierExpression)expression).Name);
			Assert.Null(expression.Previous);
			Assert.NotNull(expression.Next);
			expression = expression.Next;
			Assert.Equal(MemberPathExpressionType.Indexer, expression.ExpressionType);
			Assert.Equal(1, ((IndexerExpression)expression).Parameters.Count);
			Assert.Equal(MemberPathExpressionType.Identifier, ((IndexerExpression)expression).Parameters[0].ExpressionType);
			Assert.Equal("def", ((IdentifierExpression)((IndexerExpression)expression).Parameters[0]).Name);
			Assert.NotNull(expression.Previous);
			Assert.NotNull(expression.Next);
			expression = expression.Next;
			Assert.Equal(MemberPathExpressionType.Identifier, expression.ExpressionType);
			Assert.Equal("xyz", ((IdentifierExpression)expression).Name);
			Assert.NotNull(expression.Previous);
			Assert.Null(expression.Next);

			Assert.True(MemberPath.TryParse("[abc]  . def .xyz", out expression));
			Assert.Equal(MemberPathExpressionType.Indexer, expression.ExpressionType);
			Assert.Equal(1, ((IndexerExpression)expression).Parameters.Count);
			Assert.Equal(MemberPathExpressionType.Identifier, ((IndexerExpression)expression).Parameters[0].ExpressionType);
			Assert.Equal("abc", ((IdentifierExpression)((IndexerExpression)expression).Parameters[0]).Name);
			Assert.Null(expression.Previous);
			Assert.NotNull(expression.Next);
			expression = expression.Next;
			Assert.Equal(MemberPathExpressionType.Identifier, expression.ExpressionType);
			Assert.Equal("def", ((IdentifierExpression)expression).Name);
			Assert.NotNull(expression.Previous);
			Assert.NotNull(expression.Next);
			expression = expression.Next;
			Assert.Equal(MemberPathExpressionType.Identifier, expression.ExpressionType);
			Assert.Equal("xyz", ((IdentifierExpression)expression).Name);
			Assert.NotNull(expression.Previous);
			Assert.Null(expression.Next);

			Assert.True(MemberPath.TryParse(" [abc ]  [ def ] . xyz", out expression));
			Assert.Equal(MemberPathExpressionType.Indexer, expression.ExpressionType);
			Assert.Equal(1, ((IndexerExpression)expression).Parameters.Count);
			Assert.Equal(MemberPathExpressionType.Identifier, ((IndexerExpression)expression).Parameters[0].ExpressionType);
			Assert.Equal("abc", ((IdentifierExpression)((IndexerExpression)expression).Parameters[0]).Name);
			Assert.Null(expression.Previous);
			Assert.NotNull(expression.Next);
			expression = expression.Next;
			Assert.Equal(MemberPathExpressionType.Indexer, expression.ExpressionType);
			Assert.Equal(1, ((IndexerExpression)expression).Parameters.Count);
			Assert.Equal(MemberPathExpressionType.Identifier, ((IndexerExpression)expression).Parameters[0].ExpressionType);
			Assert.Equal("def", ((IdentifierExpression)((IndexerExpression)expression).Parameters[0]).Name);
			Assert.NotNull(expression.Previous);
			Assert.NotNull(expression.Next);
			expression = expression.Next;
			Assert.Equal(MemberPathExpressionType.Identifier, expression.ExpressionType);
			Assert.Equal("xyz", ((IdentifierExpression)expression).Name);
			Assert.NotNull(expression.Previous);
			Assert.Null(expression.Next);

			Assert.True(MemberPath.TryParse(" [abc ]  [ 123 ] . xyz", out expression));
			Assert.Equal(MemberPathExpressionType.Indexer, expression.ExpressionType);
			Assert.Equal(1, ((IndexerExpression)expression).Parameters.Count);
			Assert.Equal(MemberPathExpressionType.Identifier, ((IndexerExpression)expression).Parameters[0].ExpressionType);
			Assert.Equal("abc", ((IdentifierExpression)((IndexerExpression)expression).Parameters[0]).Name);
			Assert.Null(expression.Previous);
			Assert.NotNull(expression.Next);
			expression = expression.Next;
			Assert.Equal(MemberPathExpressionType.Indexer, expression.ExpressionType);
			Assert.Equal(1, ((IndexerExpression)expression).Parameters.Count);
			Assert.Equal(MemberPathExpressionType.Constant, ((IndexerExpression)expression).Parameters[0].ExpressionType);
			Assert.Equal(123, ((ConstantExpression)((IndexerExpression)expression).Parameters[0]).Value);
			Assert.NotNull(expression.Previous);
			Assert.NotNull(expression.Next);
			expression = expression.Next;
			Assert.Equal(MemberPathExpressionType.Identifier, expression.ExpressionType);
			Assert.Equal("xyz", ((IdentifierExpression)expression).Name);
			Assert.NotNull(expression.Previous);
			Assert.Null(expression.Next);

			Assert.True(MemberPath.TryParse(" [abc ]  [ 123 ]['xyz']", out expression));
			Assert.Equal(MemberPathExpressionType.Indexer, expression.ExpressionType);
			Assert.Equal(1, ((IndexerExpression)expression).Parameters.Count);
			Assert.Equal(MemberPathExpressionType.Identifier, ((IndexerExpression)expression).Parameters[0].ExpressionType);
			Assert.Equal("abc", ((IdentifierExpression)((IndexerExpression)expression).Parameters[0]).Name);
			Assert.Null(expression.Previous);
			Assert.NotNull(expression.Next);
			expression = expression.Next;
			Assert.Equal(MemberPathExpressionType.Indexer, expression.ExpressionType);
			Assert.Equal(1, ((IndexerExpression)expression).Parameters.Count);
			Assert.Equal(MemberPathExpressionType.Constant, ((IndexerExpression)expression).Parameters[0].ExpressionType);
			Assert.Equal(123, ((ConstantExpression)((IndexerExpression)expression).Parameters[0]).Value);
			Assert.NotNull(expression.Previous);
			Assert.NotNull(expression.Next);
			expression = expression.Next;
			Assert.Equal(MemberPathExpressionType.Indexer, expression.ExpressionType);
			Assert.Equal(1, ((IndexerExpression)expression).Parameters.Count);
			Assert.Equal(MemberPathExpressionType.Constant, ((IndexerExpression)expression).Parameters[0].ExpressionType);
			Assert.Equal("xyz", ((ConstantExpression)((IndexerExpression)expression).Parameters[0]).Value);
			Assert.NotNull(expression.Previous);
			Assert.Null(expression.Next);

			Assert.True(MemberPath.TryParse(" [abc ]  [ 123 ].xyz()", out expression));
			Assert.Equal(MemberPathExpressionType.Indexer, expression.ExpressionType);
			Assert.Equal(1, ((IndexerExpression)expression).Parameters.Count);
			Assert.Equal(MemberPathExpressionType.Identifier, ((IndexerExpression)expression).Parameters[0].ExpressionType);
			Assert.Equal("abc", ((IdentifierExpression)((IndexerExpression)expression).Parameters[0]).Name);
			Assert.Null(expression.Previous);
			Assert.NotNull(expression.Next);
			expression = expression.Next;
			Assert.Equal(MemberPathExpressionType.Indexer, expression.ExpressionType);
			Assert.Equal(1, ((IndexerExpression)expression).Parameters.Count);
			Assert.Equal(MemberPathExpressionType.Constant, ((IndexerExpression)expression).Parameters[0].ExpressionType);
			Assert.Equal(123, ((ConstantExpression)((IndexerExpression)expression).Parameters[0]).Value);
			Assert.NotNull(expression.Previous);
			Assert.NotNull(expression.Next);
			expression = expression.Next;
			Assert.Equal(MemberPathExpressionType.Method, expression.ExpressionType);
			Assert.Equal("xyz", ((MethodExpression)expression).Name);
			Assert.Equal(0, ((MethodExpression)expression).Parameters.Count);
			Assert.NotNull(expression.Previous);
			Assert.Null(expression.Next);

			Assert.True(MemberPath.TryParse(" [abc ]  .foo( 123 ).xyz ( 'OK')", out expression));
			Assert.Equal(MemberPathExpressionType.Indexer, expression.ExpressionType);
			Assert.Equal(1, ((IndexerExpression)expression).Parameters.Count);
			Assert.Equal(MemberPathExpressionType.Identifier, ((IndexerExpression)expression).Parameters[0].ExpressionType);
			Assert.Equal("abc", ((IdentifierExpression)((IndexerExpression)expression).Parameters[0]).Name);
			Assert.Null(expression.Previous);
			Assert.NotNull(expression.Next);
			expression = expression.Next;
			Assert.Equal(MemberPathExpressionType.Method, expression.ExpressionType);
			Assert.Equal("foo", ((MethodExpression)expression).Name);
			Assert.Equal(1, ((MethodExpression)expression).Parameters.Count);
			Assert.Equal(MemberPathExpressionType.Constant, ((MethodExpression)expression).Parameters[0].ExpressionType);
			Assert.Equal(123, ((ConstantExpression)((MethodExpression)expression).Parameters[0]).Value);
			Assert.NotNull(expression.Previous);
			Assert.NotNull(expression.Next);
			expression = expression.Next;
			Assert.Equal(MemberPathExpressionType.Method, expression.ExpressionType);
			Assert.Equal("xyz", ((MethodExpression)expression).Name);
			Assert.Equal(1, ((MethodExpression)expression).Parameters.Count);
			Assert.Equal(MemberPathExpressionType.Constant, ((MethodExpression)expression).Parameters[0].ExpressionType);
			Assert.Equal("OK", ((ConstantExpression)((MethodExpression)expression).Parameters[0]).Value);
			Assert.NotNull(expression.Previous);
			Assert.Null(expression.Next);
		}

		[Fact]
		public void Test4()
		{
			IMemberPathExpression expression;

			Assert.True(MemberPath.TryParse("[123L, 789.01m,a.b.c]", out expression));
			Assert.Equal(MemberPathExpressionType.Indexer, expression.ExpressionType);
			Assert.Equal(3, ((IndexerExpression)expression).Parameters.Count);
			Assert.Equal(MemberPathExpressionType.Constant, ((IndexerExpression)expression).Parameters[0].ExpressionType);
			Assert.Equal(123L, ((ConstantExpression)((IndexerExpression)expression).Parameters[0]).Value);
			Assert.Equal(MemberPathExpressionType.Constant, ((IndexerExpression)expression).Parameters[1].ExpressionType);
			Assert.Equal(789.01m, ((ConstantExpression)((IndexerExpression)expression).Parameters[1]).Value);

			var parameter = ((IndexerExpression)expression).Parameters[2];
			Assert.Equal(MemberPathExpressionType.Identifier, parameter.ExpressionType);
			Assert.Equal("a", ((IdentifierExpression)parameter).Name);
			Assert.Null(parameter.Previous);
			Assert.NotNull(parameter.Next);
			parameter = parameter.Next;
			Assert.Equal(MemberPathExpressionType.Identifier, parameter.ExpressionType);
			Assert.Equal("b", ((IdentifierExpression)parameter).Name);
			Assert.NotNull(parameter.Previous);
			Assert.NotNull(parameter.Next);
			parameter = parameter.Next;
			Assert.Equal(MemberPathExpressionType.Identifier, parameter.ExpressionType);
			Assert.Equal("c", ((IdentifierExpression)parameter).Name);
			Assert.NotNull(parameter.Previous);
			Assert.Null(parameter.Next);
			Assert.Null(expression.Next);
			Assert.Null(expression.Previous);

			Assert.True(MemberPath.TryParse("[123L, [abc]]", out expression));
			Assert.Equal(MemberPathExpressionType.Indexer, expression.ExpressionType);
			Assert.Equal(2, ((IndexerExpression)expression).Parameters.Count);
			Assert.Equal(MemberPathExpressionType.Constant, ((IndexerExpression)expression).Parameters[0].ExpressionType);
			Assert.Equal(123L, ((ConstantExpression)((IndexerExpression)expression).Parameters[0]).Value);
			parameter = ((IndexerExpression)expression).Parameters[1];
			Assert.Equal(MemberPathExpressionType.Indexer, parameter.ExpressionType);
			Assert.Equal(1, ((IndexerExpression)parameter).Parameters.Count);
			Assert.Equal(MemberPathExpressionType.Identifier, ((IndexerExpression)parameter).Parameters[0].ExpressionType);
			Assert.Equal("abc", ((IdentifierExpression)((IndexerExpression)parameter).Parameters[0]).Name);

			Assert.True(MemberPath.TryParse("[[abc], 'OK']", out expression));
			Assert.Equal(MemberPathExpressionType.Indexer, expression.ExpressionType);
			Assert.Equal(2, ((IndexerExpression)expression).Parameters.Count);
			parameter = ((IndexerExpression)expression).Parameters[0];
			Assert.Equal(MemberPathExpressionType.Indexer, parameter.ExpressionType);
			Assert.Equal(1, ((IndexerExpression)parameter).Parameters.Count);
			Assert.Equal(MemberPathExpressionType.Identifier, ((IndexerExpression)parameter).Parameters[0].ExpressionType);
			Assert.Equal("abc", ((IdentifierExpression)((IndexerExpression)parameter).Parameters[0]).Name);
			Assert.Equal(MemberPathExpressionType.Constant, ((IndexerExpression)expression).Parameters[1].ExpressionType);
			Assert.Equal("OK", ((ConstantExpression)((IndexerExpression)expression).Parameters[1]).Value);

			Assert.True(MemberPath.TryParse("[[abc], foo()]", out expression));
			Assert.Equal(MemberPathExpressionType.Indexer, expression.ExpressionType);
			Assert.Equal(2, ((IndexerExpression)expression).Parameters.Count);
			parameter = ((IndexerExpression)expression).Parameters[0];
			Assert.Equal(MemberPathExpressionType.Indexer, parameter.ExpressionType);
			Assert.Equal(1, ((IndexerExpression)parameter).Parameters.Count);
			Assert.Equal(MemberPathExpressionType.Identifier, ((IndexerExpression)parameter).Parameters[0].ExpressionType);
			Assert.Equal("abc", ((IdentifierExpression)((IndexerExpression)parameter).Parameters[0]).Name);
			Assert.Equal(MemberPathExpressionType.Method, ((IndexerExpression)expression).Parameters[1].ExpressionType);
			parameter = ((IndexerExpression)expression).Parameters[1];
			Assert.Equal(MemberPathExpressionType.Method, parameter.ExpressionType);
			Assert.Equal("foo", ((MethodExpression)parameter).Name);
			Assert.Equal(0, ((MethodExpression)parameter).Parameters.Count);

			Assert.True(MemberPath.TryParse("[[abc], foo(1, ['OK'], bar(xyz))]", out expression));
			Assert.Equal(MemberPathExpressionType.Indexer, expression.ExpressionType);
			Assert.Equal(2, ((IndexerExpression)expression).Parameters.Count);
			parameter = ((IndexerExpression)expression).Parameters[0];
			Assert.Equal(MemberPathExpressionType.Indexer, parameter.ExpressionType);
			Assert.Equal(1, ((IndexerExpression)parameter).Parameters.Count);
			Assert.Equal(MemberPathExpressionType.Identifier, ((IndexerExpression)parameter).Parameters[0].ExpressionType);
			Assert.Equal("abc", ((IdentifierExpression)((IndexerExpression)parameter).Parameters[0]).Name);
			Assert.Equal(MemberPathExpressionType.Method, ((IndexerExpression)expression).Parameters[1].ExpressionType);
			parameter = ((IndexerExpression)expression).Parameters[1];
			Assert.Equal(MemberPathExpressionType.Method, parameter.ExpressionType);

			var method = (MethodExpression)parameter;
			Assert.Equal("foo", method.Name);
			Assert.Equal(3, method.Parameters.Count);
			Assert.Equal(MemberPathExpressionType.Constant, method.Parameters[0].ExpressionType);
			Assert.Equal(1, ((ConstantExpression)method.Parameters[0]).Value);
			Assert.Equal(MemberPathExpressionType.Indexer, method.Parameters[1].ExpressionType);
			Assert.Equal(1, ((IndexerExpression)method.Parameters[1]).Parameters.Count);
			Assert.Equal(MemberPathExpressionType.Constant, ((IndexerExpression)method.Parameters[1]).Parameters[0].ExpressionType);
			Assert.Equal("OK", ((ConstantExpression)((IndexerExpression)method.Parameters[1]).Parameters[0]).Value);
			Assert.Equal(MemberPathExpressionType.Method, method.Parameters[2].ExpressionType);
			method = (MethodExpression)method.Parameters[2];
			Assert.Equal(1, method.Parameters.Count);
			Assert.Equal(MemberPathExpressionType.Identifier, method.Parameters[0].ExpressionType);
			Assert.Equal("xyz", ((IdentifierExpression)method.Parameters[0]).Name);
		}

		[Fact]
		public void Test5()
		{
			IMemberPathExpression expression;

		}
	}
}

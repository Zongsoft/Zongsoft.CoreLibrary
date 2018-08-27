/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@qq.com>
 *
 * Copyright (C) 2010-2018 Zongsoft Corporation <http://www.zongsoft.com>
 *
 * This file is part of Zongsoft.CoreLibrary.
 *
 * Zongsoft.CoreLibrary is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 *
 * Zongsoft.CoreLibrary is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
 * Lesser General Public License for more details.
 *
 * The above copyright notice and this permission notice shall be
 * included in all copies or substantial portions of the Software.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with Zongsoft.CoreLibrary; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA
 */

using System;
using System.Collections.Generic;

namespace Zongsoft.Reflection.Expressions
{
	internal static class MemberExpressionParser
	{
		#region 常量定义
		private const string EXCEPTION_UNKNOWN_MESSAGE = "An unknown error occurred in the parser.";
		private const string EXCEPTION_ILLEGAL_CHARACTER_MESSAGE = "The '{0}' character at the {1} is an illegal character.";
		private const string EXCEPTION_IDENTIFIER_WHITESPACE_MESSAGE = "Contains illegal whitespace characters before the {0} character in the identifier.";
		#endregion

		#region 公共方法
		public static bool TryParse(string text, out IMemberExpression expression)
		{
			return (expression = Parse(text, null)) != null;
		}

		public static IMemberExpression Parse(string text)
		{
			return Parse(text, message => throw new InvalidOperationException(message));
		}

		public static IMemberExpression Parse(string text, Action<string> onError)
		{
			if(string.IsNullOrEmpty(text))
				return null;

			//创建解析上下文对象
			var context = new StateContext(text.Length, onError);

			//状态迁移驱动
			for(int i = 0; i < text.Length; i++)
			{
				context.Character = text[i];

				switch(context.State)
				{
					case State.None:
						if(!DoNone(ref context, i))
							return null;

						break;
					case State.Gutter:
						if(!DoGutter(ref context, i))
							return null;

						break;
					case State.Separator:
						if(!DoSeparator(ref context, i))
							return null;

						break;
					case State.Identifier:
						if(!DoIdentifier(ref context, i))
							return null;

						break;
					case State.Method:
						if(!DoMethod(ref context, i))
							return null;

						break;
					case State.Indexer:
						if(!DoIndexer(ref context, i))
							return null;

						break;
					case State.Parameter:
						if(!DoParameter(ref context, i))
							return null;

						break;
					case State.Number:
						if(!DoNumber(ref context, i))
							return null;

						break;
					case State.String:
						if(!DoString(ref context, i))
							return null;

						break;
				}
			}

			//获取最终的解析结果
			return context.GetResult();
		}
		#endregion

		#region 状态处理
		private static bool DoNone(ref StateContext context, int position)
		{
			if(context.IsWhitespace())
				return true;

			if(context.IsLetterOrUnderscore())
			{
				context.State = State.Identifier;
				context.Accept();
				return true;
			}

			if(context.Character == '[')
			{
				context.State = State.Indexer;
				context.Stack.Push(context.Head = MemberExpression.Indexer());
				return true;
			}

			context.OnError(GetIllegalCharacterExceptionMessage(context.Character, position));
			return false;
		}

		private static bool DoGutter(ref StateContext context, int position)
		{
			switch(context.Character)
			{
				case '.':
					context.State = State.Separator;
					return true;
				case '[':
					context.State = State.Indexer;

					//追加索引器表达式，并将索引器加入到递归栈中
					context.AppendIndexer();

					return true;
				default:
					if(context.IsWhitespace())
						return true;

					context.OnError(GetIllegalCharacterExceptionMessage(context.Character, position));
					return false;
			}
		}

		private static bool DoSeparator(ref StateContext context, int position)
		{
			if(context.IsWhitespace())
				return true;

			if(context.IsLetterOrUnderscore())
			{
				context.State = State.Identifier;
				context.Accept();
				return true;
			}

			context.OnError(GetIllegalCharacterExceptionMessage(context.Character, position));
			return false;
		}

		private static bool DoIdentifier(ref StateContext context, int position)
		{
			switch(context.Character)
			{
				case '.':
					context.State = State.Separator;

					context.AppendIdentifier(context.Peek());
					context.Flags.IsAttaching(true);

					break;
				case '[':
					context.State = State.Indexer;

					context.AppendIdentifier(context.Peek());
					context.AppendIndexer();
					context.Flags.IsAttaching(false);

					break;
				case '(':
					context.State = State.Method;

					context.AppendMethod();
					context.Flags.IsAttaching(false);

					break;
				case ',':
					var ownerState = context.GetOwnerState();

					if(ownerState == State.Indexer || ownerState == State.Method)
						context.State = ownerState;
					else
					{
						context.OnError(GetIllegalCharacterExceptionMessage(context.Character, position));
						return false;
					}

					context.AppendIdentifier(context.Peek());
					context.Flags.IsAttaching(false);

					break;
				case ']':
					if(context.GetOwnerState() != State.Indexer)
					{
						context.OnError(GetIllegalCharacterExceptionMessage(context.Character, position));
						return false;
					}

					context.AppendIdentifier(context.Stack.Pop());
					context.Flags.IsAttaching(false);

					//重置状态
					context.ResetState();

					break;
				case ')':
					if(context.GetOwnerState() != State.Method)
					{
						context.OnError(GetIllegalCharacterExceptionMessage(context.Character, position));
						return false;
					}

					context.AppendIdentifier(context.Stack.Pop());
					context.Flags.IsAttaching(false);

					//重置状态
					context.ResetState();

					break;
				default:
					if(context.IsLetterOrDigitOrUnderscore())
					{
						//判断标识表达式中间是否含有空白字符
						if(context.Flags.HasWhitespace())
						{
							context.OnError(string.Format(EXCEPTION_IDENTIFIER_WHITESPACE_MESSAGE, position));
							return false;
						}

						context.Accept();
					}
					else if(!context.IsWhitespace())
					{
						context.OnError(GetIllegalCharacterExceptionMessage(context.Character, position));
						return false;
					}

					break;
			}

			//重置是否含有空格的标记
			context.Flags.HasWhitespace(context.IsWhitespace());

			return true;
		}

		private static bool DoMethod(ref StateContext context, int position)
		{
			if(context.IsWhitespace())
				return true;

			if(context.Character == '\'' || context.Character == '\"')
			{
				context.State = State.String;
				context.Flags.SetConstantType(TypeCode.String);
				context.Flags.SetStringQuote(context.Character);
				return true;
			}

			if(context.Character >= '0' && context.Character <= '9')
			{
				context.State = State.Number;
				context.Flags.SetConstantType(TypeCode.Int32);
				context.Accept();
				return true;
			}

			if(context.IsLetterOrUnderscore())
			{
				context.State = State.Identifier;
				context.Accept();
				return true;
			}

			if(context.Character == '[')
			{
				context.AppendIndexer();
				return true;
			}

			if(context.Character == ')')
			{
				context.Stack.Pop();
				context.ResetState();
				return true;
			}

			context.OnError(GetIllegalCharacterExceptionMessage(context.Character, position));
			return false;
		}

		private static bool DoIndexer(ref StateContext context, int position)
		{
			if(context.IsWhitespace())
				return true;

			if(context.Character == '\'' || context.Character == '\"')
			{
				context.State = State.String;
				context.Flags.SetConstantType(TypeCode.String);
				context.Flags.SetStringQuote(context.Character);
				return true;
			}

			if(context.Character >= '0' && context.Character <= '9')
			{
				context.State = State.Number;
				context.Flags.SetConstantType(TypeCode.Int32);
				context.Accept();
				return true;
			}

			if(context.IsLetterOrUnderscore())
			{
				context.State = State.Identifier;
				context.Accept();
				return true;
			}

			if(context.Character == '[')
			{
				context.AppendIndexer();
				return true;
			}

			context.OnError(GetIllegalCharacterExceptionMessage(context.Character, position));
			return false;
		}

		private static bool DoParameter(ref StateContext context, int position)
		{
			if(context.IsWhitespace())
				return true;

			switch(context.Character)
			{
				case ',':
					context.State = context.GetOwnerState();
					break;
				case ']':
					if(context.GetOwnerState() != State.Indexer)
					{
						context.OnError(GetIllegalCharacterExceptionMessage(context.Character, position));
						return false;
					}

					context.Stack.Pop();
					context.ResetState();

					break;
				case ')':
					if(context.GetOwnerState() != State.Method)
					{
						context.OnError(GetIllegalCharacterExceptionMessage(context.Character, position));
						return false;
					}

					context.Stack.Pop();
					context.ResetState();

					break;
				default:
					context.OnError(GetIllegalCharacterExceptionMessage(context.Character, position));
					return false;
			}

			return true;
		}

		private static bool DoNumber(ref StateContext context, int position)
		{
			switch(context.Character)
			{
				case ',':
					var ownerState = context.GetOwnerState();

					if(ownerState != State.Indexer && ownerState != State.Method)
					{
						context.OnError(EXCEPTION_UNKNOWN_MESSAGE);
						return false;
					}

					context.State = ownerState;
					context.AppendParameterConstant(context.Stack.Peek());

					break;
				case ']':
					if(context.GetOwnerState() != State.Indexer)
					{
						context.OnError(GetIllegalCharacterExceptionMessage(context.Character, position));
						return false;
					}

					context.AppendParameterConstant(context.Stack.Pop());
					context.ResetState();

					break;
				case ')':
					if(context.GetOwnerState() != State.Method)
					{
						context.OnError(GetIllegalCharacterExceptionMessage(context.Character, position));
						return false;
					}

					context.AppendParameterConstant(context.Stack.Pop());
					context.ResetState();

					break;
				case '.':
					var numberType = context.Flags.GetConstantType();

					if(numberType == TypeCode.Double || numberType == TypeCode.Single || numberType == TypeCode.Decimal)
					{
						context.OnError(GetIllegalCharacterExceptionMessage(context.Character, position));
						return false;
					}

					context.Flags.SetConstantType(TypeCode.Double);
					context.Accept();

					break;
				case 'L':
					context.State = State.Parameter;
					context.Flags.SetConstantType(TypeCode.Int64);
					context.AppendParameterConstant(context.Stack.Peek());

					break;
				case 'f':
				case 'F':
					context.State = State.Parameter;
					context.Flags.SetConstantType(TypeCode.Single);
					context.AppendParameterConstant(context.Stack.Peek());

					break;
				case 'm':
				case 'M':
					context.State = State.Parameter;
					context.Flags.SetConstantType(TypeCode.Decimal);
					context.AppendParameterConstant(context.Stack.Peek());

					break;
				default:
					if(context.Character >= '0' && context.Character <= '9')
					{
						context.Accept();
					}
					else if(context.IsWhitespace())
					{
						context.State = State.Parameter;
						context.AppendParameterConstant(context.Stack.Peek());
					}
					else
					{
						context.OnError(GetIllegalCharacterExceptionMessage(context.Character, position));
						return false;
					}

					break;
			}

			return true;
		}

		private static bool DoString(ref StateContext context, int position)
		{
			if(context.Flags.IsEscaping())
			{
				context.Accept(Escape(context.Character));
				context.Flags.IsEscaping(false);
			}
			else
			{
				if(context.Character == '\\')
				{
					context.Flags.IsEscaping(true);
				}
				else if(context.Character == context.Flags.GetStringQuote())
				{
					context.State = State.Parameter;
					context.AppendParameterConstant(context.Stack.Peek());
				}
				else
				{
					context.Accept();
				}
			}

			return true;
		}
		#endregion

		#region 私有方法
		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
		private static char Escape(char chr)
		{
			switch(chr)
			{
				case '\\':
					return '\\';
				case '\'':
					return '\'';
				case '\"':
					return '\"';
				case 't':
					return '\t';
				case 'b':
					return '\b';
				case 's':
					return ' ';
				case 'n':
					return '\n';
				case 'r':
					return '\r';
				default:
					return chr;
			}
		}

		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
		private static string GetIllegalCharacterExceptionMessage(char chr, int position)
		{
			return string.Format(EXCEPTION_ILLEGAL_CHARACTER_MESSAGE, chr, position);
		}
		#endregion

		#region 嵌套结构
		private enum State
		{
			None,
			Gutter,
			Separator,
			Identifier,
			Indexer,
			Method,
			Parameter,
			String,
			Number,
		}

		private struct StateContext
		{
			#region 私有变量
			private int _bufferIndex;
			private readonly char[] _buffer;
			private readonly Action<string> _onError;
			#endregion

			#region 公共字段
			public State State;
			public char Character;
			public StateVector Flags;
			public MemberExpression Head;
			public readonly Stack<MemberExpression> Stack;
			#endregion

			#region 构造函数
			public StateContext(int length, Action<string> onError)
			{
				_onError = onError;
				_buffer = new char[length];
				_bufferIndex = 0;

				this.Character = '\0';
				this.Head = null;
				this.Flags = new StateVector();
				this.State = State.None;
				this.Stack = new Stack<MemberExpression>();
			}
			#endregion

			#region 公共方法
			public void Accept(char? chr = null)
			{
				_buffer[_bufferIndex++] = chr ?? Character;
			}

			public void OnError(string message)
			{
				_onError?.Invoke(message);
			}

			public MemberExpression Peek()
			{
				return Stack.Count > 0 ? Stack.Peek() : null;
			}

			public bool IsWhitespace()
			{
				return char.IsWhiteSpace(Character);
			}

			public bool IsLetterOrUnderscore()
			{
				return (Character >= 'a' && Character <= 'z') ||
				       (Character >= 'A' && Character <= 'Z') || Character == '_';
			}

			public bool IsLetterOrDigitOrUnderscore()
			{
				return (Character >= 'a' && Character <= 'z') ||
				       (Character >= 'A' && Character <= 'Z') ||
				       (Character >= '0' && Character <= '9') || Character == '_';
			}

			public void AppendParameterConstant(IMemberExpression owner)
			{
				if(owner == null)
					throw new InvalidOperationException(EXCEPTION_UNKNOWN_MESSAGE);

				void Add(ICollection<IMemberExpression> parameters, string content, TypeCode type)
				{
					parameters.Add(MemberExpression.Constant(content, type));
				};

				if(owner is IndexerExpression indexer)
					Add(indexer.Arguments, GetBufferContent(), Flags.GetConstantType());
				else if(owner is MethodExpression method)
					Add(method.Arguments, GetBufferContent(), Flags.GetConstantType());
				else
					throw new InvalidOperationException(EXCEPTION_UNKNOWN_MESSAGE);
			}

			public IdentifierExpression AppendIdentifier(IMemberExpression owner)
			{
				var current = MemberExpression.Identifier(GetBufferContent());

				if(owner == null)
				{
					if(Head == null)
						Head = current;
					else
						Head = Head.Append(current);

					return current;
				}

				if(owner is IndexerExpression indexer)
				{
					if(Flags.IsAttaching())
					{
						if(indexer.Arguments.Count == 0 || indexer.Arguments[indexer.Arguments.Count - 1].ExpressionType != MemberExpressionType.Identifier)
							throw new InvalidOperationException(EXCEPTION_UNKNOWN_MESSAGE);

						return ((MemberExpression)((IdentifierExpression)indexer.Arguments[indexer.Arguments.Count - 1]).Last()).Append(current);
					}

					indexer.Arguments.Add(current);
					return current;
				}
				else if(owner is MethodExpression method)
				{
					if(Flags.IsAttaching())
					{
						if(method.Arguments.Count == 0 || method.Arguments[method.Arguments.Count - 1].ExpressionType != MemberExpressionType.Identifier)
							throw new InvalidOperationException(EXCEPTION_UNKNOWN_MESSAGE);

						return ((MemberExpression)((IdentifierExpression)method.Arguments[method.Arguments.Count - 1]).Last()).Append(current);
					}

					method.Arguments.Add(current);
					return current;
				}

				throw new InvalidOperationException(EXCEPTION_UNKNOWN_MESSAGE);
			}

			public IndexerExpression AppendIndexer()
			{
				var expression = MemberExpression.Indexer();

				if(Stack.Count == 0)
				{
					Head = Head.Append(expression);
				}
				else
				{
					var owner = Stack.Peek();

					if(owner is IndexerExpression indexer)
						indexer.Arguments.Add(expression);
					else if(owner is MethodExpression method)
						method.Arguments.Add(expression);
					else
						throw new InvalidOperationException(EXCEPTION_UNKNOWN_MESSAGE);
				}

				Stack.Push(expression);

				return expression;
			}

			public MethodExpression AppendMethod()
			{
				if(Stack == null || Stack.Count == 0)
				{
					var current = MemberExpression.Method(GetBufferContent());

					if(Head == null)
						Head = current;
					else
						Head = Head.Append(current);

					Stack.Push(current);

					return current;
				}

				var expression = MemberExpression.Method(GetBufferContent());
				var owner = Stack.Peek();

				if(owner is IndexerExpression indexer)
					indexer.Arguments.Add(expression);
				else if(owner is MethodExpression method)
					method.Arguments.Add(expression);
				else
					throw new InvalidOperationException(EXCEPTION_UNKNOWN_MESSAGE);

				Stack.Push(expression);

				return expression;
			}

			public State GetOwnerState()
			{
				if(Stack == null || Stack.Count == 0)
					return State.Gutter;

				var owner = Stack.Peek();

				if(owner is IndexerExpression)
					return State.Indexer;
				else if(owner is MethodExpression)
					return State.Method;

				return State.Gutter;
			}

			public void ResetState()
			{
				if(Stack.Count == 0 || Stack.Peek() is IdentifierExpression)
					State = State.Gutter;
				else
					State = State.Parameter;
			}

			public IMemberExpression GetResult()
			{
				switch(State)
				{
					case State.None:
						return null;
					case State.Gutter:
						return Head.First();
					case State.Identifier:
						if(Stack == null || Stack.Count == 0)
						{
							var identifier = MemberExpression.Identifier(GetBufferContent());

							if(Head == null)
								return identifier;
							else
								return Head.Append(identifier).First();
						}

						_onError?.Invoke($"");
						return null;
					default:
						_onError?.Invoke($"");
						return null;
				}
			}
			#endregion

			#region 私有方法
			private string GetBufferContent()
			{
				var content = new string(_buffer, 0, _bufferIndex);
				_bufferIndex = 0;
				return content;
			}
			#endregion
		}

		private struct StateVector
		{
			#region 常量定义
			private const int STRING_ESCAPING_FLAG = 1; //字符串是否处于转移状态(0:普通态, 1:转移态)
			private const int STRING_QUOTATION_FLAG = 2; //字符串的引号是否为单引号(0:单引号, 1:双引号)

			private const int IDENTIFIER_ATTACHING_FLAG = 4; //标识表达式是否为附加到最后一个参数的标识表达式后面(0:新增参数, 1:附加参数)
			private const int IDENTIFIER_WHITESPACE_FLAG = 8; //标识表达式中间是否出现空白字符(0:没有, 1:有)

			private const int CONSTANT_TYPE_FLAG = 0x70; //常量的类型掩码范围
			private const int CONSTANT_TYPE_INT32_FLAG = 0x10; //32位整型数常量的类型值
			private const int CONSTANT_TYPE_INT64_FLAG = 0x20; //64位整型数常量的类型值
			private const int CONSTANT_TYPE_SINGLE_FLAG = 0x30; //单精度浮点数常量的类型值
			private const int CONSTANT_TYPE_DOUBLE_FLAG = 0x40; //双精度浮点数常量的类型值
			private const int CONSTANT_TYPE_DECIMAL_FLAG = 0x50; //Decimal 数常量的类型值
			#endregion

			#region 成员变量
			private int _data;
			#endregion

			#region 公共方法
			public bool IsEscaping()
			{
				return IsMarked(STRING_ESCAPING_FLAG);
			}

			public void IsEscaping(bool enabled)
			{
				Mark(STRING_ESCAPING_FLAG, enabled);
			}

			public bool IsAttaching()
			{
				return IsMarked(IDENTIFIER_ATTACHING_FLAG);
			}

			public void IsAttaching(bool enabled)
			{
				Mark(IDENTIFIER_ATTACHING_FLAG, enabled);
			}

			public bool HasWhitespace()
			{
				return IsMarked(IDENTIFIER_WHITESPACE_FLAG);
			}

			public void HasWhitespace(bool enabled)
			{
				Mark(IDENTIFIER_WHITESPACE_FLAG, enabled);
			}

			public TypeCode GetConstantType()
			{
				switch(_data & CONSTANT_TYPE_FLAG)
				{
					case CONSTANT_TYPE_INT32_FLAG:
						return TypeCode.Int32;
					case CONSTANT_TYPE_INT64_FLAG:
						return TypeCode.Int64;
					case CONSTANT_TYPE_SINGLE_FLAG:
						return TypeCode.Single;
					case CONSTANT_TYPE_DOUBLE_FLAG:
						return TypeCode.Double;
					case CONSTANT_TYPE_DECIMAL_FLAG:
						return TypeCode.Decimal;
					default:
						return TypeCode.String;
				}
			}

			public void SetConstantType(TypeCode type)
			{
				//首先，重置数字常量类型的比特区域
				Mark(CONSTANT_TYPE_FLAG, false);

				switch(type)
				{
					case TypeCode.Int32:
						Mark(CONSTANT_TYPE_INT32_FLAG, true);
						break;
					case TypeCode.Int64:
						Mark(CONSTANT_TYPE_INT64_FLAG, true);
						break;
					case TypeCode.Single:
						Mark(CONSTANT_TYPE_SINGLE_FLAG, true);
						break;
					case TypeCode.Double:
						Mark(CONSTANT_TYPE_DOUBLE_FLAG, true);
						break;
					case TypeCode.Decimal:
						Mark(CONSTANT_TYPE_DECIMAL_FLAG, true);
						break;
				}
			}

			public char GetStringQuote()
			{
				return IsMarked(STRING_QUOTATION_FLAG) ? '"' : '\'';
			}

			public void SetStringQuote(char chr)
			{
				Mark(STRING_QUOTATION_FLAG, chr == '"');
			}
			#endregion

			#region 私有方法
			[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
			private bool IsMarked(int bit)
			{
				return (_data & bit) == bit;
			}

			[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
			private void Mark(int bit, bool value)
			{
				if(value)
					_data |= bit;
				else
					_data &= ~bit;
			}
			#endregion
		}
		#endregion
	}
}

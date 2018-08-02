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
	public class MemberPath
	{
		#region 常量定义
		private const string UNKNOWN_EXCEPTION_MESSAGE = "An unknown error occurred in the parser.";
		#endregion

		#region 公共方法
		public static bool TryParse(string text, out IMemberPathExpression expression)
		{
			return (expression = Parse(text, null)) != null;
		}

		public static IMemberPathExpression Parse(string text)
		{
			return Parse(text, message => throw new InvalidOperationException(message));
		}

		public static IMemberPathExpression Parse(string text, Action<string> error)
		{
			if(string.IsNullOrEmpty(text))
				return null;

			var state = State.None;
			var ownerState = State.None;
			var isEscaping = false;
			var isJoining = false;
			var buffer = new char[text.Length];
			var position = 0;
			var whitespaces = 0;
			var stringSymbol = '\0';
			var numberType = TypeCode.Int32;
			var stack = new Stack<MemberPathExpression>();
			MemberPathExpression head = null;

			for(int i = 0; i < text.Length; i++)
			{
				var character = text[i];

				switch(state)
				{
					case State.None:
						if((character >= 'a' && character <= 'z') ||
						   (character >= 'A' && character <= 'Z') || character == '_')
						{
							state = State.Identifier;
							buffer[position++] = character;
						}
						else if(character == '[')
						{
							state = State.Indexer;
							stack.Push(head = MemberPathExpression.Indexer());
						}
						else if(!char.IsWhiteSpace(character))
						{
							error?.Invoke($"");
							return null;
						}

						break;
					case State.Gutter:
						switch(character)
						{
							case '.':
								state = State.Separator;
								break;
							case '[':
								state = State.Indexer;

								//追加索引器表达式，并将索引器加入到递归栈中
								AppendIndexer(stack, ref head);

								break;
							default:
								if(!char.IsWhiteSpace(character))
								{
									error?.Invoke($"");
									return null;
								}

								break;
						}

						break;
					case State.Separator:
						if((character >= 'a' && character <= 'z') ||
						   (character >= 'A' && character <= 'Z') || character == '_')
						{
							state = State.Identifier;
							buffer[position++] = character;
						}
						else
						{
							if(!char.IsWhiteSpace(character))
							{
								error?.Invoke($"");
								return null;
							}
						}

						break;
					case State.Identifier:
						switch(character)
						{
							case '.':
								state = State.Separator;

								AppendIdentifier(stack.Count > 0 ? stack.Peek() : null, buffer, ref position, ref head, isJoining);

								isJoining = true;

								break;
							case '[':
								state = State.Indexer;

								AppendIdentifier(stack.Count > 0 ? stack.Peek() : null, buffer, ref position, ref head, isJoining);

								//追加索引器表达式，并将索引器加入到递归栈中
								AppendIndexer(stack, ref head);

								isJoining = false;

								break;
							case '(':
								state = State.Method;

								//追加方法表达式，并将索引器加入到递归栈中
								AppendMethod(stack, buffer, ref position, ref head);

								isJoining = false;

								break;
							case ',':
								ownerState = GetOwnerState(stack);

								if(ownerState == State.Indexer || ownerState == State.Method)
									state = ownerState;
								else
								{
									error?.Invoke($"");
									return null;
								}

								AppendIdentifier(stack.Peek(), buffer, ref position, isJoining);

								isJoining = false;

								break;
							case ']':
								ownerState = GetOwnerState(stack);

								if(ownerState != State.Indexer)
								{
									error?.Invoke($"");
									return null;
								}

								AppendIdentifier(stack.Pop(), buffer, ref position, isJoining);

								isJoining = false;

								//重置状态
								ResetState(stack, ref state);

								break;
							case ')':
								ownerState = GetOwnerState(stack);

								if(ownerState != State.Method)
								{
									error?.Invoke($"");
									return null;
								}

								AppendIdentifier(stack.Pop(), buffer, ref position, isJoining);

								isJoining = false;

								//重置状态
								ResetState(stack, ref state);

								break;
							default:
								if(char.IsLetterOrDigit(character) || character == '_')
								{
									if(whitespaces > 0)
									{
										error?.Invoke($"");
										return null;
									}

									buffer[position++] = character;
								}
								else if(!char.IsWhiteSpace(character))
								{
									error?.Invoke($"");
									return null;
								}

								break;
						}

						if(char.IsWhiteSpace(character))
							whitespaces++;
						else
							whitespaces = 0;

						break;
					case State.Method:
						if(character == '\'' || character == '\"')
						{
							stringSymbol = character;
							state = State.String;
						}
						else if(character >= '0' && character <= '9')
						{
							state = State.Number;
							numberType = TypeCode.Int32;
							buffer[position++] = character;
						}
						else if((character >= 'a' && character <= 'z') ||
								(character >= 'A' && character <= 'Z') || character == '_')
						{
							state = State.Identifier;
							buffer[position++] = character;
						}
						else if(character == '[')
						{
							//追加索引器表达式，并将索引器加入到递归栈中
							AppendIndexer(stack, ref head);
						}
						else if(character == ')')
						{
							stack.Pop();
							ResetState(stack, ref state);
						}
						else if(!char.IsWhiteSpace(character))
						{
							error?.Invoke($"");
							return null;
						}

						break;
					case State.Indexer:
						if(character == '\'' || character == '\"')
						{
							stringSymbol = character;
							state = State.String;
						}
						else if(character >= '0' && character <= '9')
						{
							state = State.Number;
							numberType = TypeCode.Int32;
							buffer[position++] = character;
						}
						else if((character >= 'a' && character <= 'z') ||
								(character >= 'A' && character <= 'Z') || character == '_')
						{
							state = State.Identifier;
							buffer[position++] = character;
						}
						else if(character == '[')
						{
							//追加索引器表达式，并将索引器加入到递归栈中
							AppendIndexer(stack, ref head);
						}
						else if(!char.IsWhiteSpace(character))
						{
							error?.Invoke($"");
							return null;
						}

						break;
					case State.Parameter:
						switch(character)
						{
							case ',':
								state = GetOwnerState(stack);
								break;
							case ']':
								ownerState = GetOwnerState(stack);

								if(ownerState != State.Indexer)
								{
									error?.Invoke($"");
									return null;
								}

								stack.Pop();

								//重置状态
								ResetState(stack, ref state);

								break;
							case ')':
								ownerState = GetOwnerState(stack);

								if(ownerState != State.Method)
								{
									error?.Invoke($"");
									return null;
								}

								stack.Pop();

								//重置状态
								ResetState(stack, ref state);

								break;
							default:
								if(!char.IsWhiteSpace(character))
								{
									error?.Invoke($"");
									return null;
								}

								break;
						}

						break;
					case State.Number:
						switch(character)
						{
							case ',':
								ownerState = GetOwnerState(stack);

								if(ownerState == State.Indexer || ownerState == State.Method)
								{
									state = ownerState;

									//将当前数字常量加入到父表达式的参数列表中
									AppendParameter(stack.Peek(), numberType, buffer, ref position);
								}
								else
								{
									error?.Invoke($"");
									return null;
								}

								break;
							case ']':
								ownerState = GetOwnerState(stack);

								if(ownerState != State.Indexer)
								{
									error?.Invoke($"");
									return null;
								}

								//将当前数字常量加入到索引器的参数列表中
								AppendParameter(stack.Pop(), numberType, buffer, ref position);

								//重置状态
								ResetState(stack, ref state);

								break;
							case ')':
								ownerState = GetOwnerState(stack);

								if(ownerState != State.Method)
								{
									error?.Invoke($"");
									return null;
								}

								//将当前数字常量加入到方法表达式的参数列表中
								AppendParameter(stack.Pop(), numberType, buffer, ref position);

								//重置状态
								ResetState(stack, ref state);

								break;
							case '.':
								if(numberType == TypeCode.Double || numberType == TypeCode.Single || numberType == TypeCode.Decimal)
								{
									error?.Invoke($"");
									return null;
								}

								numberType = TypeCode.Double;
								buffer[position++] = character;

								break;
							case 'L':
								state = State.Parameter;
								numberType = TypeCode.Int64;

								//将当前数字常量加入到父表达式的参数列表中
								AppendParameter(stack.Peek(), numberType, buffer, ref position);

								break;
							case 'f':
							case 'F':
								state = State.Parameter;
								numberType = TypeCode.Single;

								//将当前数字常量加入到父表达式的参数列表中
								AppendParameter(stack.Peek(), numberType, buffer, ref position);

								break;
							case 'm':
							case 'M':
								state = State.Parameter;
								numberType = TypeCode.Decimal;

								//将当前数字常量加入到父表达式的参数列表中
								AppendParameter(stack.Peek(), numberType, buffer, ref position);

								break;
							default:
								//至此，不是数字的字符均为非法字符（包括空白字符）
								if(character >= '0' && character <= '9')
									buffer[position++] = character;
								else if(char.IsWhiteSpace(character))
								{
									state = State.Parameter;

									//将当前数字常量加入到父表达式的参数列表中
									AppendParameter(stack.Peek(), numberType, buffer, ref position);
								}
								else
								{
									error?.Invoke($"");
									return null;
								}

								break;
						}

						break;
					case State.String:
						if(isEscaping)
						{
							buffer[position++] = Escape(character);
							isEscaping = false;
						}
						else
						{
							if(character == '\\')
								isEscaping = true;
							else if(character == stringSymbol)
							{
								state = State.Parameter;
								stringSymbol = '\0';

								//将当前字符串常量加入到父表达式的参数列表中
								AppendParameter(stack.Peek(), TypeCode.String, buffer, ref position);
							}
							else
								buffer[position++] = character;
						}

						break;
				}
			}

			if(state == State.None)
				return null;

			switch(state)
			{
				case State.None:
					return null;
				case State.Gutter:
					return head.First();
				case State.Identifier:
					if(stack == null || stack.Count == 0)
					{
						var identifier = MemberPathExpression.Identifier(GetContent(buffer, ref position));

						if(head == null)
							return identifier;
						else
							return head.Append(identifier).First();
					}

					error?.Invoke($"");
					return null;
				default:
					error?.Invoke($"");
					return null;
			}
		}
		#endregion

		#region 私有方法
		private static void AppendParameter(IMemberPathExpression owner, IMemberPathExpression parameter)
		{
			if(owner == null)
				throw new InvalidOperationException(UNKNOWN_EXCEPTION_MESSAGE);

			if(owner is IndexerExpression indexer)
				indexer.Parameters.Add(parameter);
			else if(owner is MethodExpression method)
				method.Parameters.Add(parameter);
			else
				throw new InvalidOperationException(UNKNOWN_EXCEPTION_MESSAGE);
		}

		private static void AppendParameter(IMemberPathExpression owner, TypeCode type, char[] buffer, ref int position)
		{
			AppendParameter(owner, MemberPathExpression.Constant(GetContent(buffer, ref position), type));
		}

		private static void AppendParameter(IMemberPathExpression owner, char[] buffer, ref int position)
		{
			AppendParameter(owner, MemberPathExpression.Identifier(GetContent(buffer, ref position)));
		}

		private static IdentifierExpression AppendIdentifier(IMemberPathExpression owner, char[] buffer, ref int position, ref MemberPathExpression head, bool isAttach)
		{
			if(owner == null)
			{
				var current = MemberPathExpression.Identifier(GetContent(buffer, ref position));

				if(head == null)
					head = current;
				else
					head = head.Append(current);

				return current;
			}

			return AppendIdentifier(owner, buffer, ref position, isAttach);
		}

		private static IdentifierExpression AppendIdentifier(IMemberPathExpression owner, char[] buffer, ref int position, bool isAttach)
		{
			var current = MemberPathExpression.Identifier(GetContent(buffer, ref position));

			if(owner is IndexerExpression indexer)
			{
				if(isAttach)
				{
					if(indexer.Parameters.Count == 0 || indexer.Parameters[indexer.Parameters.Count - 1].ExpressionType != MemberPathExpressionType.Identifier)
						throw new InvalidOperationException(UNKNOWN_EXCEPTION_MESSAGE);

					return ((MemberPathExpression)((IdentifierExpression)indexer.Parameters[indexer.Parameters.Count - 1]).Last()).Append(current);
				}

				indexer.Parameters.Add(current);
				return current;
			}
			else if(owner is MethodExpression method)
			{
				if(isAttach)
				{
					if(method.Parameters.Count == 0 || method.Parameters[method.Parameters.Count - 1].ExpressionType != MemberPathExpressionType.Identifier)
						throw new InvalidOperationException(UNKNOWN_EXCEPTION_MESSAGE);

					return ((MemberPathExpression)((IdentifierExpression)method.Parameters[method.Parameters.Count - 1]).Last()).Append(current);
				}

				method.Parameters.Add(current);
				return current;
			}

			throw new InvalidOperationException(UNKNOWN_EXCEPTION_MESSAGE);
		}

		private static IndexerExpression AppendIndexer(Stack<MemberPathExpression> stack, ref MemberPathExpression head)
		{
			var expression = MemberPathExpression.Indexer();

			if(stack.Count == 0)
			{
				head = head.Append(expression);
			}
			else
			{
				var owner = stack.Peek();

				if(owner is IndexerExpression indexer)
					indexer.Parameters.Add(expression);
				else if(owner is MethodExpression method)
					method.Parameters.Add(expression);
				else
					throw new InvalidOperationException(UNKNOWN_EXCEPTION_MESSAGE);
			}

			stack.Push(expression);

			return expression;
		}

		private static MethodExpression AppendMethod(Stack<MemberPathExpression> stack, char[] buffer, ref int position, ref MemberPathExpression head)
		{
			if(stack == null || stack.Count == 0)
			{
				var current = MemberPathExpression.Method(GetContent(buffer, ref position));

				if(head == null)
					head = current;
				else
					head = head.Append(current);

				//始终将方法表达式压栈（以便后续的参数处理）
				stack.Push(current);

				return current;
			}

			return AppendMethod(stack, buffer, ref position);
		}

		private static MethodExpression AppendMethod(Stack<MemberPathExpression> stack, char[] buffer, ref int position)
		{
			var expression = MemberPathExpression.Method(GetContent(buffer, ref position));
			var owner = stack.Peek();

			if(owner is IndexerExpression indexer)
				indexer.Parameters.Add(expression);
			else if(owner is MethodExpression method)
				method.Parameters.Add(expression);
			else
				throw new InvalidOperationException(UNKNOWN_EXCEPTION_MESSAGE);

			stack.Push(expression);

			return expression;
		}

		private static string GetContent(char[] buffer, ref int index)
		{
			var content = new string(buffer, 0, index);
			index = 0;
			return content;
		}

		private static void ResetState(Stack<MemberPathExpression> stack, ref State state)
		{
			if(stack.Count == 0 || stack.Peek() is IdentifierExpression)
				state = State.Gutter;
			else
				state = State.Parameter;
		}

		private static State GetOwnerState(Stack<MemberPathExpression> stack)
		{
			if(stack == null || stack.Count == 0)
				return State.Gutter;

			var owner = stack.Peek();

			if(owner is IndexerExpression)
				return State.Indexer;
			else if(owner is MethodExpression)
				return State.Method;

			return State.Gutter;
		}

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
		#endregion

		#region 私有枚举
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
		#endregion
	}
}

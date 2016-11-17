/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2016 Zongsoft Corporation <http://www.zongsoft.com>
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
using System.IO;
using System.Collections.Generic;

namespace Zongsoft.Services
{
	internal static class CommandExpressionParser
	{
		#region 私有枚举
		private enum CommandPathState
		{
			None,
			Dot,
			DoubleDot,
			Slash,
			Part,
		}
		#endregion

		#region 解析方法
		public static CommandExpression Parse(string text)
		{
			if(string.IsNullOrWhiteSpace(text))
				throw new ArgumentNullException(nameof(text));

			CommandExpression result = null;
			CommandExpression current = null;

			using(var reader = new StringReader(text))
			{
				var fullPath = ParsePath(reader);

				if(string.IsNullOrWhiteSpace(fullPath))
					return null;

				current = new CommandExpression(fullPath);

				if(result == null)
					result = current;
				else
				{
					var previous = result;

					while(previous.Next != null)
					{
						previous = previous.Next;
					}

					previous.Next = current;
				}

				KeyValuePair<string, string>? pair = null;

				while((pair = ParsePair(reader)) != null)
				{
					if(pair != null)
					{
						if(string.IsNullOrEmpty(pair.Value.Key))
							current.Arguments.Add(pair.Value.Value);
						else
							current.Options.Add(pair.Value);
					}

					if(reader.Peek() == '|')
						break;
				}
			}

			return result;
		}
		#endregion

		#region 私有方法
		private static string ParsePath(TextReader reader)
		{
			if(reader == null)
				throw new ArgumentNullException(nameof(reader));

			var result = string.Empty;
			var state = CommandPathState.None;
			var valueRead = 0;

			while((valueRead = reader.Read()) > 0)
			{
				var chr = (char)valueRead;

				if(chr == '.')
				{
					switch(state)
					{
						case CommandPathState.None:
							state = CommandPathState.Dot;
							break;
						case CommandPathState.Dot:
							state = CommandPathState.DoubleDot;
							break;
						case CommandPathState.Part:
							state = CommandPathState.Slash;
							chr = '/';
							break;
						default:
							throw new CommandExpressionException("Invalid command expression.");
					}
				}
				else if(chr == '/')
				{
					if(state == CommandPathState.Slash)
						throw new CommandExpressionException("Invalid command expression.");

					state = CommandPathState.Slash;
				}
				else if(Char.IsLetterOrDigit(chr) || chr == '_')
				{
					if(state == CommandPathState.Dot || state == CommandPathState.DoubleDot)
						throw new CommandExpressionException("Invalid command expression.");

					state = CommandPathState.Part;
				}
				else if(Char.IsWhiteSpace(chr))
				{
					return result;
				}

				result += chr;
			}

			return result;
		}

		private static IEnumerable<KeyValuePair<string, string>?> ParsePairs(string text)
		{
			if(text == null)
				yield break;

			using(var reader = new StringReader(text))
			{
				do
				{
					yield return ParsePair(reader);
				} while(reader.Peek() > 0);
			}
		}

		private static KeyValuePair<string, string>? ParsePair(TextReader reader)
		{
			if(reader == null)
				throw new ArgumentNullException(nameof(reader));

			var quote = '\0';
			var isEscaping = false;
			var isKey = false;
			var key = string.Empty;
			var value = string.Empty;
			int valueRead;

			while((valueRead = reader.Read()) > 0)
			{
				var chr = (char)valueRead;

				//如果当前位置位于引用符的外面
				if(quote == '\0')
				{
					if(Char.IsWhiteSpace(chr))
					{
						//如果结果字符串为空则表示当前空白字符位于引用符的头部，则可忽略它；
						if(string.IsNullOrEmpty(key) && string.IsNullOrEmpty(value))
							continue;
						else //否则当前空白字符位于引用符的尾部，则可直接返回。
							return new KeyValuePair<string, string>(key, value);
					}
					else if(chr == '|')
					{
						if(string.IsNullOrEmpty(key) && string.IsNullOrEmpty(value))
							return null;

						return new KeyValuePair<string, string>(key, value);
					}
				}

				if(isEscaping)
				{
					var escapedChar = IsQuote(chr) ? chr : EscapeChar(chr);

					if(escapedChar == '\0')
						value += '\\';
					else
						chr = escapedChar;
				}
				else
				{
					if(chr == '-' || chr == '/')
					{
						isKey = true;
					}
					else if(chr == ':' || chr == '=')
					{
						isKey = false;
					}
					else if(quote != '\0' && chr == quote)
					{
						if(isKey)
							key = value;
						else
							return new KeyValuePair<string, string>(key, value);
					}
					else
					{
						if(IsQuote(chr))
						{
							quote = chr;
							continue;
						}
					}
				}

				//设置转义状态：即当前字符为转义符并且当前状态不为转义状态
				isEscaping = chr == '\\' && (!isEscaping);

				if(isEscaping)
					continue;

				if(isKey)
					key += chr;
				else
					value += chr;
			}

			if(string.IsNullOrEmpty(key) && string.IsNullOrEmpty(value))
				return null;

			return new KeyValuePair<string, string>(key, value);
		}

		private static bool IsQuote(char chr)
		{
			return (chr == '"' || chr == '\'');
		}

		private static char EscapeChar(char chr)
		{
			switch(chr)
			{
				case 's':
					return ' ';
				case 't':
					return '\t';
				case '\\':
					return '\\';
				default:
					return '\0';
			}
		}
		#endregion
	}
}

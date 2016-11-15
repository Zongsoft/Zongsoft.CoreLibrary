﻿/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2011-2016 Zongsoft Corporation <http://www.zongsoft.com>
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
using System.Linq;
using System.Text;

namespace Zongsoft.Services
{
	public class CommandExpressionParser : ICommandExpressionParser
	{
		#region 私有枚举
		private enum CommandPathState
		{
			None,
			Dot,
			DoubleDot,
			Slash,
			Character,
		}

		private enum CommandExpressionState
		{
			None,
			Dot,
			DoubleDot,
			Slash,
			Character,
			Whitespace,
			Pipe,
			Option,
			OptionKey,
			OptionValue,
			OptionDelimiter,
		}
		#endregion

		#region 单例字段
		public static readonly ICommandExpressionParser Default = new CommandExpressionParser();
		#endregion

		#region 解析方法
		public CommandExpression Parse(string text)
		{
			if(string.IsNullOrWhiteSpace(text))
				throw new ArgumentNullException(nameof(text));

			var name = string.Empty;
			var value = string.Empty;
			var state = CommandExpressionState.None;
			CommandExpression expression = null;

			using(var reader = new StringReader(text))
			{
				var path = ParsePath(reader);
				char chr;

				while((chr = (char)reader.Read()) > 0)
				{
					if(chr == '-' || chr == '/')
					{
						if(state == CommandExpressionState.None)
							state = CommandExpressionState.OptionKey;
						else
							throw new CommandExpressionException("");
					}
					else if(chr == '=' || chr == ':')
					{
						if(state == CommandExpressionState.OptionKey)
							state = CommandExpressionState.OptionDelimiter;
						else
							value += chr;
					}
					else if(chr == '"' || chr == '\'')
					{
						Common.StringExtension.Escape(reader, chr);
					}
					else if(Char.IsLetterOrDigit(chr) || chr == '_')
					{
						if(state == CommandExpressionState.OptionKey)
							name += chr;
						else if(state == CommandExpressionState.OptionValue)
							value += chr;
					}
					else if(Char.IsWhiteSpace(chr))
					{
						if(string.IsNullOrEmpty(name))
							expression.Arguments.Add(value);
						else
							expression.Options.Add(name, value);

						state = CommandExpressionState.None;
					}
				}
			}

			throw new NotImplementedException();
		}
		#endregion

		private CommandPathDesciption ParsePath(TextReader reader)
		{
			if(reader == null)
				throw new ArgumentNullException(nameof(reader));

			var state = CommandPathState.None;
			var path = new CommandPathDesciption();

			char chr;

			while((chr = (char)reader.Read()) > 0)
			{
				if(chr == '.')
				{
					switch(state)
					{
						case CommandPathState.None:
							state = CommandPathState.Dot;
							path.Anchor = IO.PathAnchor.Current;
							break;
						case CommandPathState.Dot:
							state = CommandPathState.DoubleDot;
							path.Anchor = IO.PathAnchor.Parent;
							break;
						case CommandPathState.Character:
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

					state = CommandPathState.Character;
				}
				else if(Char.IsWhiteSpace(chr))
				{
					if(state == CommandPathState.Character)
						return path;
				}

				if(!Char.IsWhiteSpace(chr))
					path.FullPath += chr;
			}

			return path;
		}

		private void ParsePath(string text, int index)
		{
			var state = CommandPathState.None;
			var path = string.Empty;

			while(index < text.Length)
			{
				var chr = text[index];

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
						case CommandPathState.Character:
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

					state = CommandPathState.Character;
				}

				if(!Char.IsWhiteSpace(chr))
					path += chr;
			}
		}

		private static string EscapeString(StringReader reader, params char[] delimiters)
		{
			return EscapeString(reader, null, delimiters);
		}

		private static string EscapeString(StringReader reader, Func<char, char> escape, params char[] delimiters)
		{
			if(reader == null)
				throw new ArgumentNullException(nameof(reader));

			//如果未指定转义处理函数则设置一个默认的转义处理函数
			if(escape == null)
			{
				escape = chr =>
				{
					if(delimiters.Contains(chr))
						return chr;

					switch(chr)
					{
						case 't':
							return '\t';
						case '\\':
							return '\\';
						default:
							return chr;
					}
				};
			}

			var isEscaping = false;
			var result = string.Empty;
			char character;

			while((character = (char)reader.Read()) > 0)
			{
				if(isEscaping)
					character = escape(character);
				else if(delimiters.Contains(character))
					return result;

				//设置转义状态：即当前字符为转义符并且当前状态不为转义状态
				isEscaping = character == '\\' && (!isEscaping);

				if(isEscaping)
					continue;

				result += character;
			}

			return result;
		}

		private struct CommandPathDesciption
		{
			public Zongsoft.IO.PathAnchor Anchor;
			public string FullPath;
			public string Name;
		}
	}
}

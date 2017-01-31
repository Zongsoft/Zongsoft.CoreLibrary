/*
 * Authors:
 *   钟峰(Popeye Zhong) <9555843@qq.com>
 *
 * Copyright (C) 2017 Zongsoft Corporation <http://www.zongsoft.com>
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

namespace Zongsoft.Expressions.Tokenization
{
	public class StringTokenizer : ITokenizer
	{
		#region 公共方法
		public TokenResult Tokenize(TextReader reader)
		{
			var valueRead = reader.Read();

			if(valueRead < 0)
				return TokenResult.Fail(0);

			var chr = (char)valueRead;
			var content = string.Empty;
			var escaping = false;

			if(chr != '\'' && chr != '"')
				return TokenResult.Fail(-1);

			var quote = chr;

			while((valueRead = reader.Read()) > 0)
			{
				chr = (char)valueRead;

				if(chr == '\n' || chr == '\r')
					throw new SyntaxException("The string literal contains new-line symbol.");

				if(escaping)
				{
					if(chr != quote)
					{
						char escapedChar;

						if(EscapeChar(chr, out escapedChar))
							chr = escapedChar;
						else
							content += '\\';
					}

					content += chr;
				}
				else
				{
					if(chr == quote)
						return new TokenResult(0, new Token(TokenType.Constant, content));

					if(chr != '\\')
						content += chr;
				}

				escaping = chr == '\\' && (!escaping);
			}

			throw new SyntaxException($"Missing a closing symbol({quote}) of string.");
		}
		#endregion

		#region 私有方法
		private bool EscapeChar(char chr, out char escapedChar)
		{
			escapedChar = '\0';

			switch(chr)
			{
				case '"':
				case '\'':
				case '\\':
					escapedChar = chr;
					return true;
				case 's':
					escapedChar = ' ';
					return true;
				case 't':
					escapedChar = '\t';
					return true;
				case 'n':
					escapedChar = '\n';
					return true;
				case 'r':
					escapedChar = '\r';
					return true;
			}

			return false;
		}
		#endregion
	}
}

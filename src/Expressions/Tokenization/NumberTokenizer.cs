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
	public class NumberTokenizer : ITokenizer
	{
		#region 公共方法
		public TokenResult Tokenize(TextReader reader)
		{
			var valueRead = reader.Read();

			if(valueRead < 0)
				return TokenResult.Fail(0);

			var chr = (char)valueRead;
			var number = string.Empty;

			if(!char.IsDigit(chr))
				return TokenResult.Fail(-1);

			number += chr;

			while((valueRead = reader.Read()) > 0)
			{
				chr = (char)valueRead;

				if(char.IsDigit(chr))
					number += chr;
				else if(chr == '.')
				{
					if(number.Contains("."))
						throw new SyntaxException("Illegal numeric literal, it contains multiple dot(.) symbol.");

					number += chr;
				}
				else if(chr == 'L')
				{
					if(number.Contains("."))
						throw new SyntaxException("Illegal long integer suffix symbol(L), because it's a float numeric literal.");

					return new TokenResult(0, new Token(TokenType.Constant, long.Parse(number)));
				}
				else if(chr == 'm' || chr == 'M')
				{
					return new TokenResult(0, new Token(TokenType.Constant, decimal.Parse(number)));
				}
				else if(chr == 'f' || chr == 'F')
				{
					return new TokenResult(0, new Token(TokenType.Constant, float.Parse(number)));
				}
				else if(chr == 'd' || chr == 'D')
				{
					return new TokenResult(0, new Token(TokenType.Constant, double.Parse(number)));
				}
				else
				{
					if(number[number.Length - 1] == '.')
						throw new SyntaxException("Illegal numeric literal, cann't end with a dot(.) symbol.");

					return new TokenResult(-1, this.CreateToken(number));
				}
			}

			return new TokenResult(0, this.CreateToken(number));
		}
		#endregion

		#region 私有方法
		private Token CreateToken(string value)
		{
			if(value.Contains("."))
				return new Token(TokenType.Constant, double.Parse(value));

			return new Token(TokenType.Constant, int.Parse(value));
		}
		#endregion
	}
}

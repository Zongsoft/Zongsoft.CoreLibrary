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
	public class IdentifierTokenizer : ITokenizer
	{
		#region 公共方法
		public TokenResult Tokenize(TextReader reader)
		{
			var valueRead = reader.Read();

			if(valueRead < 0)
				return TokenResult.Fail(0);

			var chr = (char)valueRead;
			var identifier = string.Empty;

			if(!this.IsIdentifierBeginning(chr))
				return TokenResult.Fail(-1);

			identifier += chr;

			while((valueRead = reader.Read()) > 0)
			{
				chr = (char)valueRead;

				if(char.IsLetterOrDigit(chr) || chr == '_')
					identifier += chr;
				else
					return new TokenResult(-1, new Token(TokenType.Identifier, identifier));
			}

			return new TokenResult(0, new Token(TokenType.Identifier, identifier));
		}
		#endregion

		#region 私有方法
		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
		private bool IsIdentifierBeginning(char chr)
		{
			return char.IsLetter(chr) || chr == '_';
		}
		#endregion
	}
}

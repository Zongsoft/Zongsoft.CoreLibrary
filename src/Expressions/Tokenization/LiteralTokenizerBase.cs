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
using System.Linq;

namespace Zongsoft.Expressions.Tokenization
{
	public abstract class LiteralTokenizerBase : ITokenizer
	{
		#region 成员字段
		private bool _ignoreCase;
		private string[] _literals;
		#endregion

		#region 构造函数
		protected LiteralTokenizerBase(params string[] literals) : this(false, literals)
		{
		}

		protected LiteralTokenizerBase(bool ignoreCase, params string[] literals)
		{
			if(literals == null)
				throw new ArgumentNullException(nameof(literals));

			_ignoreCase = ignoreCase;
			_literals = literals;
		}
		#endregion

		#region 保护属性
		protected bool IgnoreCase
		{
			get
			{
				return _ignoreCase;
			}
		}

		protected string[] Literals
		{
			get
			{
				return _literals;
			}
			set
			{
				if(value == null)
					throw new ArgumentNullException();

				_literals = value;
			}
		}
		#endregion

		#region 公共方法
		public TokenResult Tokenize(TextReader reader)
		{
			var literal = string.Empty;
			var valueRead = 0;

			while((valueRead = reader.Read()) > 0)
			{
				var chr = (char)valueRead;

				if(char.IsWhiteSpace(chr))
				{
					if(string.IsNullOrEmpty(literal))
						return TokenResult.Fail(0);

					return new TokenResult(0, this.CreateToken(literal));
				}

				if(!this.Literals.Any(p => p.StartsWith(literal + chr, _ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal)))
				{
					if(this.Literals.Any(p => string.Equals(p, literal, _ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal)))
						return new TokenResult(-1, this.CreateToken(literal));
					else
						return TokenResult.Fail(-(literal.Length + 1));
				}

				literal += chr;
			}

			if(string.IsNullOrEmpty(literal))
				return TokenResult.Fail(0);
			else
				return new TokenResult(0, this.CreateToken(literal));
		}
		#endregion

		#region 抽象方法
		protected abstract Token CreateToken(string literal);
		#endregion
	}
}

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
using System.Collections.Generic;

namespace Zongsoft.Expressions
{
	/// <summary>
	/// 提供词法解析的类。
	/// </summary>
	public class Lexer
	{
		#region 单例字段
		public static readonly Lexer Instance = new Lexer();
		#endregion

		#region 成员字段
		private IList<ITokenizer> _tokenizers;
		#endregion

		#region 构造函数
		public Lexer()
		{
			_tokenizers = new List<ITokenizer>()
			{
				new Tokenization.NullTokenizer(),
				new Tokenization.NumberTokenizer(),
				new Tokenization.StringTokenizer(),
				new Tokenization.BooleanTokenizer(),
				new Tokenization.IdentifierTokenizer(),
				new Tokenization.SymbolTokenizer(),
			};
		}
		#endregion

		#region 公共属性
		public IList<ITokenizer> Tokenizers
		{
			get
			{
				return _tokenizers;
			}
		}
		#endregion

		#region 公共方法
		public TokenScanner GetScanner(string text)
		{
			if(string.IsNullOrEmpty(text))
				throw new ArgumentNullException(nameof(text));

			return new TokenScanner(this, text);
		}
		#endregion
	}
}

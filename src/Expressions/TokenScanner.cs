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
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace Zongsoft.Expressions
{
	/// <summary>
	/// 表示词法分析的分词扫描器。
	/// </summary>
	public class TokenScanner : IEnumerable<Token>, IDisposable
	{
		#region 成员字段
		private Lexer _lexer;
		private TokenCharacterReader _reader;
		#endregion

		#region 构造函数
		internal TokenScanner(Lexer lexer, string text)
		{
			if(lexer == null)
				throw new ArgumentNullException(nameof(lexer));

			_lexer = lexer;
			_reader = new TokenCharacterReader(text);
		}
		#endregion

		#region 公共方法
		public Token Scan()
		{
			if(_lexer == null)
				throw new ObjectDisposedException(nameof(TokenScanner));

			foreach(var tokenizer in _lexer.Tokenizers)
			{
				//跳过中间的所有空白字符
				this.SkipWhitespaces(_reader);

				//执行分词操作
				var result = tokenizer.Tokenize(_reader);

				//重新定位读取器的指针位置
				_reader.Seek(result.Offset, SeekOrigin.Current);

				if(result.Token != null)
					return result.Token;
			}

			if(_reader.Peek() > 0)
				throw new SyntaxException($"Illegal literal at {_reader.Position + 1} character in the expression.");

			return null;
		}
		#endregion

		#region 私有方法
		private void SkipWhitespaces(TokenCharacterReader reader)
		{
			int value;

			while((value = reader.Read()) > 0)
			{
				if(!char.IsWhiteSpace((char)value))
				{
					reader.Seek(-1, SeekOrigin.Current);
					return;
				}
			}
		}
		#endregion

		#region 遍历方法
		public IEnumerator<Token> GetEnumerator()
		{
			Token token;

			while((token = this.Scan()) != null)
			{
				yield return token;
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}
		#endregion

		#region 释放方法
		void IDisposable.Dispose()
		{
			_lexer = null;
			_reader.Dispose();
		}
		#endregion

		#region 嵌套子类
		private class TokenCharacterReader : TextReader
		{
			#region 成员字段
			private string _text;
			private int _position;
			#endregion

			#region 构造函数
			public TokenCharacterReader(string text)
			{
				if(string.IsNullOrEmpty(text))
					throw new ArgumentNullException(nameof(text));

				_text = text;
				_position = -1;
			}
			#endregion

			#region 公共属性
			public int Position
			{
				get
				{
					return _position;
				}
				set
				{
					if(value < -1)
						throw new ArgumentOutOfRangeException();

					if(value > _text.Length)
						throw new ArgumentOutOfRangeException();

					_position = value;
				}
			}
			#endregion

			#region 公共方法
			public int Seek(int offset, SeekOrigin origin)
			{
				var position = _position;

				switch(origin)
				{
					case SeekOrigin.Begin:
						position = offset;
						break;
					case SeekOrigin.End:
						position = (_text.Length - 1) - offset;
						break;
					case SeekOrigin.Current:
						position += offset;
						break;
				}

				return this.Position = position;
			}

			public override int Peek()
			{
				var position = _position;

				if(position >= 0 && position < _text.Length)
					return _text[position];

				return -1;
			}

			public override int Read()
			{
				var position = _position;

				while((position = _position) < _text.Length)
				{
					if(Interlocked.CompareExchange(ref _position, position + 1, position) == position)
						break;
				}

				if(position < _text.Length - 1)
					return _text[position + 1];

				return -1;
			}
			#endregion
		}
		#endregion
	}
}

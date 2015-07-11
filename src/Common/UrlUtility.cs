/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2012 Zongsoft Corporation <http://www.zongsoft.com>
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
using System.Linq;
using System.Text;

namespace Zongsoft.Common
{
	[Obsolete("Please use the 'System.Uri' class.")]
	public static class UrlUtility
	{
		#region 编码方法
		public static string UrlEncode(string text)
		{
			return UrlEncode(text, Encoding.UTF8);
		}

		public static string UrlEncode(string text, Encoding encoding)
		{
			if(text == null)
				return null;

			byte[] bytes = encoding.GetBytes(text);
			byte[] encodedBytes = UrlEncode(bytes, 0, bytes.Length);

			return Encoding.ASCII.GetString(encodedBytes);
		}

		private static byte[] UrlEncode(byte[] bytes, int offset, int count)
		{
			if(bytes == null && count == 0)
				return null;

			if(bytes == null)
				throw new ArgumentNullException("bytes");

			if(offset < 0 || offset > bytes.Length)
				throw new ArgumentOutOfRangeException("offset");

			if(count < 0 || offset + count > bytes.Length)
				throw new ArgumentOutOfRangeException("count");

			int cSpaces = 0;
			int cUnsafe = 0;

			// count them first 
			for(int i = 0; i < count; i++)
			{
				char ch = (char)bytes[offset + i];

				if(ch == ' ')
					cSpaces++;
				else if(!IsUrlSafeChar(ch))
					cUnsafe++;
			}

			// nothing to expand? 
			if(cSpaces == 0 && cUnsafe == 0)
				return bytes;

			// expand not 'safe' characters into %XX, spaces to +s
			byte[] expandedBytes = new byte[count + cUnsafe * 2];
			int pos = 0;

			for(int i = 0; i < count; i++)
			{
				byte b = bytes[offset + i];
				char ch = (char)b;

				if(IsUrlSafeChar(ch))
				{
					expandedBytes[pos++] = b;
				}
				else if(ch == ' ')
				{
					expandedBytes[pos++] = (byte)'+';
				}
				else
				{
					expandedBytes[pos++] = (byte)'%';
					expandedBytes[pos++] = (byte)IntToHex((b >> 4) & 0xf);
					expandedBytes[pos++] = (byte)IntToHex(b & 0x0f);
				}
			}

			return expandedBytes;
		}
		#endregion

		#region 解码方法
		public static string UrlDecode(string text)
		{
			return UrlDecode(text, Encoding.UTF8);
		}

		public static string UrlDecode(string text, Encoding encoding)
		{
			if(text == null)
				return null;

			int count = text.Length;
			UrlDecoder helper = new UrlDecoder(count, encoding);

			// go through the string's chars collapsing %XX and %uXXXX and
			// appending each char as char, with exception of %XX constructs 
			// that are appended as bytes

			for(int pos = 0; pos < count; pos++)
			{
				char ch = text[pos];

				if(ch == '+')
				{
					ch = ' ';
				}
				else if(ch == '%' && pos < count - 2)
				{
					if(text[pos + 1] == 'u' && pos < count - 5)
					{
						int h1 = HexToInt(text[pos + 2]);
						int h2 = HexToInt(text[pos + 3]);
						int h3 = HexToInt(text[pos + 4]);
						int h4 = HexToInt(text[pos + 5]);

						if(h1 >= 0 && h2 >= 0 && h3 >= 0 && h4 >= 0)
						{   // valid 4 hex chars
							ch = (char)((h1 << 12) | (h2 << 8) | (h3 << 4) | h4);
							pos += 5;

							// only add as char 
							helper.AddChar(ch);
							continue;
						}
					}
					else
					{
						int h1 = HexToInt(text[pos + 1]);
						int h2 = HexToInt(text[pos + 2]);

						if(h1 >= 0 && h2 >= 0)
						{     // valid 2 hex chars 
							byte b = (byte)((h1 << 4) | h2);
							pos += 2;

							// don't add as char
							helper.AddByte(b);
							continue;
						}
					}
				}

				if((ch & 0xFF80) == 0)
					helper.AddByte((byte)ch); // 7 bit have to go as bytes because of Unicode 
				else
					helper.AddChar(ch);
			}

			return helper.GetString();
		}
		#endregion

		#region 私有方法
		private static bool IsUrlSafeChar(char ch)
		{
			if((ch >= 'a' && ch <= 'z') || (ch >= 'A' && ch <= 'Z') || (ch >= '0' && ch <= '9'))
				return true;

			if(ch != '!')
			{
				switch(ch)
				{
					case '\'':
					case '(':
					case ')':
					case '*':
					case '-':
					case '.':
						return true;
					case '+':
					case ',':
						break;
					default:
						if(ch == '_')
							return true;
						break;
				}

				return false;
			}

			return true;
		}

		private static char IntToHex(int n)
		{
			if(n <= 9)
				return (char)(n + 48);

			return (char)(n - 10 + 97);
		}

		private static int HexToInt(char ch)
		{
			if(ch >= '0' && ch <= '9')
				return (int)(ch - '0');

			if(ch >= 'a' && ch <= 'f')
				return (int)(ch - 'a' + '\n');

			if(ch < 'A' || ch > 'F')
				return -1;

			return (int)(ch - 'A' + '\n');
		}
		#endregion

		#region 嵌套子类
		private class UrlDecoder
		{
			private int _bufferSize;

			// Accumulate characters in a special array 
			private int _numChars;
			private char[] _charBuffer;

			// Accumulate bytes for decoding into characters in a special array
			private int _numBytes;
			private byte[] _byteBuffer;

			// Encoding to convert chars to bytes
			private Encoding _encoding;

			private void FlushBytes()
			{
				if(_numBytes > 0)
				{
					_numChars += _encoding.GetChars(_byteBuffer, 0, _numBytes, _charBuffer, _numChars);
					_numBytes = 0;
				}
			}

			internal UrlDecoder(int bufferSize, Encoding encoding)
			{
				_bufferSize = bufferSize;
				_encoding = encoding;

				_charBuffer = new char[bufferSize];
				// byte buffer created on demand 
			}

			internal void AddChar(char ch)
			{
				if(_numBytes > 0)
					FlushBytes();

				_charBuffer[_numChars++] = ch;
			}

			internal void AddByte(byte b)
			{
				// if there are no pending bytes treat 7 bit bytes as characters
				// this optimization is temp disable as it doesn't work for some encodings
				/* 
								if (_numBytes == 0 && ((b & 0x80) == 0)) {
									AddChar((char)b); 
								} 
								else
				*/
				{
					if(_byteBuffer == null)
						_byteBuffer = new byte[_bufferSize];

					_byteBuffer[_numBytes++] = b;
				}
			}

			internal String GetString()
			{
				if(_numBytes > 0)
					FlushBytes();

				if(_numChars > 0)
					return new String(_charBuffer, 0, _numChars);
				else
					return String.Empty;
			}
		}
		#endregion
	}
}

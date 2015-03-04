/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2008-2013 Zongsoft Corporation <http://www.zongsoft.com>
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
	public static class StringExtension
	{
		public static readonly string InvalidCharacters = "`~!@#$%^&*()-+={}[]|\\/?:;'\"\t\r\n ";

		public static int GetStringHashCode(string text)
		{
			if(text == null || text.Length < 1)
				return 0;

			unsafe
			{
				fixed(char* src = text)
				{
					int hash1 = 5381;
					int hash2 = hash1;

					int c;
					char* s = src;
					while((c = s[0]) != 0)
					{
						hash1 = ((hash1 << 5) + hash1) ^ c;
						c = s[1];

						if(c == 0)
							break;

						hash2 = ((hash2 << 5) + hash2) ^ c;
						s += 2;
					}

					return hash1 + (hash2 * 1566083941);
				}
			}
		}

		public static bool ContainsCharacters(this string text, string characters)
		{
			if(string.IsNullOrEmpty(text) || string.IsNullOrEmpty(characters))
				return false;

			return ContainsCharacters(text, characters.ToArray());
		}

		public static bool ContainsCharacters(this string text, params char[] characters)
		{
			if(string.IsNullOrEmpty(text) || characters.Length < 1)
				return false;

			foreach(char character in characters)
			{
				foreach(char item in text)
				{
					if(character == item)
						return true;
				}
			}

			return false;
		}

		public static string RemoveCharacters(this string text, string invalidCharacters)
		{
			return RemoveCharacters(text, invalidCharacters, 0);
		}

		public static string RemoveCharacters(this string text, char[] invalidCharacters)
		{
			return RemoveCharacters(text, invalidCharacters, 0);
		}

		public static string RemoveCharacters(this string text, string invalidCharacters, int startIndex)
		{
			return RemoveCharacters(text, invalidCharacters, startIndex, -1);
		}

		public static string RemoveCharacters(this string text, char[] invalidCharacters, int startIndex)
		{
			return RemoveCharacters(text, invalidCharacters, startIndex, -1);
		}

		public static string RemoveCharacters(this string text, string invalidCharacters, int startIndex, int count)
		{
			return RemoveCharacters(text, invalidCharacters.ToCharArray(), startIndex, count);
		}

		public static string RemoveCharacters(this string text, char[] invalidCharacters, int startIndex, int count)
		{
			if(string.IsNullOrEmpty(text) || invalidCharacters.Length < 1)
				return text;

			if(startIndex < 0)
				throw new ArgumentOutOfRangeException("startIndex");

			if(count < 1)
				count = invalidCharacters.Length - startIndex;

			if(startIndex + count > invalidCharacters.Length)
				throw new ArgumentOutOfRangeException("count");

			string result = text;

			for(int i = startIndex; i < startIndex + count; i++)
				result = result.Replace(invalidCharacters[i].ToString(), "");

			return result;
		}

		public static string TrimString(this string text, string trimString)
		{
			return TrimString(text, trimString, StringComparison.OrdinalIgnoreCase);
		}

		public static string TrimString(this string text, string trimString, StringComparison comparisonType)
		{
			return TrimStringEnd(
				TrimStringStart(text, trimString, comparisonType),
				trimString,
				comparisonType);
		}

		public static string TrimString(this string text, string prefix, string suffix)
		{
			return TrimString(text, prefix, suffix, StringComparison.OrdinalIgnoreCase);
		}

		public static string TrimString(this string text, string prefix, string suffix, StringComparison comparisonType)
		{
			return text
					.TrimStringStart(prefix, comparisonType)
					.TrimStringEnd(suffix, comparisonType);
		}

		public static string TrimStringEnd(this string text, string trimString)
		{
			return TrimStringEnd(text, trimString, StringComparison.OrdinalIgnoreCase);
		}

		public static string TrimStringEnd(this string text, string trimString, StringComparison comparisonType)
		{
			if(string.IsNullOrEmpty(text) || string.IsNullOrEmpty(trimString))
				return text;

			while(text.EndsWith(trimString, comparisonType))
				text = text.Remove(text.Length - trimString.Length);

			return text;
		}

		public static string TrimStringStart(this string text, string trimString)
		{
			return TrimStringStart(text, trimString, StringComparison.OrdinalIgnoreCase);
		}

		public static string TrimStringStart(this string text, string trimString, StringComparison comparisonType)
		{
			if(string.IsNullOrEmpty(text) || string.IsNullOrEmpty(trimString))
				return text;

			while(text.StartsWith(trimString, comparisonType))
				text = text.Remove(0, trimString.Length);

			return text;
		}

		public static bool In(this string text, IEnumerable<string> collection, StringComparison comparisonType)
		{
			if(collection == null)
				return false;

			return collection.Any(item => string.Equals(item, text, comparisonType));
		}
	}
}

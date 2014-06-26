/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2013-2014 Zongsoft Corporation <http://www.zongsoft.com>
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
using System.Text.RegularExpressions;
using System.Text;

namespace Zongsoft.Text
{
	public class TextExpressionParser
	{
		#region 正则表达式
		//(?<express>
		//  (?'open'\$\{)
		//  \s*
		//  (
		//    (
		//      (?<name>\w+)\s*:\s*
		//    )?
		//    \s*
		//    (
		//      (
		//        (?<text>
		//          (
		//            (
		//              (?'openText'\$\{)
		//              [^\$\{\}]*
		//              (?'-openText'\})
		//            )
		//            (?(openText)(?!))
		//          )|[^:\$\{\}\s\#]+
		//        )
		//        (\#(?<format>\w+))?
		//      )
		//    )
		//    (
		//      \s+
		//      (?<args>
		//        (
		//          (
		//            (
		//              (?'openArg'\$\{)
		//              [^\$\{\}]*
		//              (?'-openArg'\})
		//            )
		//            (?(openArg)(?!))
		//          )|[^:\$\{\}\s\#]+
		//        )
		//        (\#[^:\$\{\}\s\#]+)?
		//      )
		//    )*
		//  )
		//  \s*
		//  (?'-open'\})
		//)(?(open)(?!))
		private static readonly Regex _regex = new Regex(@"
		                        (?<express>(?'open'\$\{)\s*(((?<name>\w+)\s*:\s*)?\s*(((?<text>(((?'openText'\$\{)[^\$\{\}]*(?'-openText'\}))(?(openText)(?!)))|[^:\$\{\}\s\#]+)(\#(?<format>\w+))?))(\s+(?<args>((((?'openArg'\$\{)[^\$\{\}]*(?'-openArg'\}))(?(openArg)(?!)))|[^:\$\{\}\s\#]+)(\#[^:\$\{\}\s\#]+)?))*)\s*(?'-open'\}))(?(open)(?!))",
								RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);

		//(?<value>
		//  (
		//    (
		//      (?'open'\$\{)
		//      [^\$\{\}]*
		//      (?'-open'\})
		//    )
		//    (?(open)(?!))
		//  )|[^:\$\{\}\s\#]+
		//)
		//(\#(?<format>\w+))?
		private static readonly Regex _partRegex = new Regex(@"
		                        (?<value>(((?'open'\$\{)[^\$\{\}]*(?'-open'\}))(?(open)(?!)))|[^:\$\{\}\s\#]+)(\#(?<format>\w+))?",
								RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
		#endregion

		#region 公共方法
		public static TextExpressionNodeCollection Parse(Stream stream, Encoding encoding = null)
		{
			if(stream == null)
				throw new ArgumentNullException("stream");

			using(var reader = new StreamReader(stream, encoding))
			{
				return Parse(reader.ReadToEnd());
			}
		}

		public static TextExpressionNodeCollection Parse(TextReader reader)
		{
			if(reader == null)
				throw new ArgumentNullException("reader");

			return Parse(reader.ReadToEnd());
		}

		public static TextExpressionNodeCollection Parse(string text)
		{
			if(string.IsNullOrEmpty(text))
				return TextExpressionNodeCollection.Empty;

			var index = 0;
			var nodes = new TextExpressionNodeCollection();

			//使用正则表达式解析指定的文本内容
			var matches = _regex.Matches(text);

			foreach(Match match in matches)
			{
				if(match.Index > index)
				{
					var length = match.Index - index;
					nodes.Add(new TextExpressionNode(index, length, text.Substring(index, length)));
				}

				TextExpressionArgument[] args = null;
				Group argsGroup = match.Groups["args"];

				if(argsGroup.Success)
				{
					if(argsGroup.Captures.Count == 0)
						args = new TextExpressionArgument[] { ParseArgument(argsGroup.Value) };
					else
					{
						args = new TextExpressionArgument[argsGroup.Captures.Count];

						for(int i = 0; i < argsGroup.Captures.Count; i++)
						{
							args[i] = ParseArgument(argsGroup.Captures[i].Value);
						}
					}
				}

				nodes.Add(new TextExpressionNode(match.Index,
												 match.Length,
												 match.Value,
												 new TextExpression(
													 match.Groups["name"].Value,
													 match.Groups["text"].Value,
													 match.Groups["format"].Value,
													 args)));

				index = match.Index + match.Length;
			}

			if(index < text.Length)
			{
				nodes.Add(new TextExpressionNode(index, text.Length - index, text.Substring(index, text.Length - index)));
			}

			return nodes;
		}
		#endregion

		#region 私有方法
		private static TextExpressionArgument ParseArgument(string arg)
		{
			if(string.IsNullOrWhiteSpace(arg))
				return null;

			var match = _partRegex.Match(arg);

			if(match.Success)
			{
				return new TextExpressionArgument(match.Groups["value"].Value, match.Groups["format"].Value);
			}

			return null;
		}
		#endregion
	}
}

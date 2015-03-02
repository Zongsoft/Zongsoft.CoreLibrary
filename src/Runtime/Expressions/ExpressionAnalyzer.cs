/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2014 Zongsoft Corporation <http://www.zongsoft.com>
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

namespace Zongsoft.Runtime.Expressions
{
	public class ExpressionAnalyzer : IExpressionAnalyzer
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
								RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.ExplicitCapture, TimeSpan.FromSeconds(10));

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
								RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.ExplicitCapture, TimeSpan.FromSeconds(10));
		#endregion

		#region 公共方法
		public ExpressionTree Analyze(Stream stream, Encoding encoding = null)
		{
			if(stream == null)
				throw new ArgumentNullException("stream");

			encoding = encoding ?? Encoding.UTF8;

			using(var reader = new StreamReader(stream, encoding))
			{
				return this.Analyze(reader.ReadToEnd());
			}
		}

		public ExpressionTree Analyze(TextReader reader)
		{
			if(reader == null)
				throw new ArgumentNullException("reader");

			return this.Analyze(reader.ReadToEnd());
		}

		public virtual ExpressionTree Analyze(string text)
		{
			if(string.IsNullOrEmpty(text))
				return null;

			throw new NotImplementedException();
		}
		#endregion

		#region 私有方法
		#endregion
	}
}

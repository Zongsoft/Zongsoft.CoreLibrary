/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2015 Zongsoft Corporation <http://www.zongsoft.com>
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
using System.Text.RegularExpressions;

namespace Zongsoft.Text
{
	public class TextRegular : ITextRegular
	{
		#region 成员字段
		private readonly Regex _regex;
		#endregion

		#region 构造函数
		public TextRegular(string pattern)
		{
			if(string.IsNullOrWhiteSpace(pattern))
				throw new ArgumentNullException("pattern");

			_regex = new Regex(pattern, RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.IgnorePatternWhitespace | RegexOptions.IgnoreCase, TimeSpan.FromSeconds(3));
		}
		#endregion

		#region 公共方法
		public bool IsMatch(string text)
		{
			if(text == null)
				return false;

			try
			{
				return _regex.IsMatch(text);
			}
			catch
			{
				return false;
			}
		}

		public bool IsMatch(string text, out string result)
		{
			result = null;

			if(text == null)
				return false;

			Match match;

			try
			{
				match = _regex.Match(text);
			}
			catch
			{
				return false;
			}

			if(match.Success)
			{
				var group = match.Groups["value"];

				if(group == null)
				{
					result = match.Value;
				}
				else
				{
					foreach(Capture capture in group.Captures)
					{
						result += capture.Value;
					}
				}
			}

			return match.Success;
		}
		#endregion

		#region 显式实现
		bool Zongsoft.Services.IMatchable.IsMatch(object parameter)
		{
			if(parameter == null)
				return false;

			return this.IsMatch(parameter as string);
		}
		#endregion

		#region 嵌套子类
		public static class Web
		{
			/// <summary>获取电子邮箱(Email)地址的文本验证器。</summary>
			public static TextRegular Email = new TextRegular(@"^\s*(?<value>[A-Za-z0-9]([-_\.]?[A-Za-z0-9]+)*@([A-Za-z0-9]+([-_]?[A-Za-z0-9]+)*)(\.[A-Za-z0-9]+([-_]?[A-Za-z0-9]+)*)*\.[A-Za-z]+)\s*$");
		}

		public static class Uri
		{
			#region 正则文本
			/*
^\s*(?<value>
(
	(?<schema>[A-Za-z]+)://
)?
(?<host>
	(?<domain>[A-Za-z0-9]+([-_][A-Za-z0-9]+)*)
	(\.(?<domain>[A-Za-z0-9]+([-_][A-Za-z0-9]+)*))+
)
(:(?<port>\d{1,5}))?
(?<path>
	(?<segment>
		/[^\s/:\*\?\&]*
	)*
)?
(?<query>
	\?
	(
		(?<parameter>
			(?<parameter_name>[A-Za-z0-9]+([-_][A-Za-z0-9]+)*)
			(=(?<parameter_value>[^\?\&\s]*))?
		)
		\&?
	)*
)?
(?<fragment>
	\#[^\#\s]*
)?
)\s*$
			 */
			private const string URI_PATTERN = @"^\s*(?<value>((?<schema>${schema})://)?(?<host>(?<domain>[A-Za-z0-9]+([-_][A-Za-z0-9]+)*)(\.(?<domain>[A-Za-z0-9]+([-_][A-Za-z0-9]+)*))+)(:(?<port>\d{1,5}))?(?<path>(?<segment>/[^\s/:\*\?\&]*)*)?(?<query>\?((?<parameter>(?<parameter_name>[A-Za-z0-9]+([-_][A-Za-z0-9]+)*)(=(?<parameter_value>[^\?\&\s]*))?)\&?)*)?(?<fragment>\#[^\#\s]*)?)\s*$";
			#endregion

			/// <summary>获取任意协议的URL文本验证器。</summary>
			public static TextRegular Url = new TextRegular(URI_PATTERN.Replace("${schema}", "[A-Za-z]+"));

			/// <summary>获取Http协议的URL文本验证器。</summary>
			public static TextRegular Http = new TextRegular(URI_PATTERN.Replace("${schema}", "http[s]?"));

			/// <summary>获取Ftp协议的URL文本验证器。</summary>
			public static TextRegular Ftp = new TextRegular(URI_PATTERN.Replace("${schema}", "ftp"));
		}

		public static class Chinese
		{
			/// <summary>获取中国手机号码的文本验证器。</summary>
			public static TextRegular Cellphone = new TextRegular(@"^\s*((\+|00)86\s*[-\.]?)?\s*(?<value>1\d{2})(?<separator>(\s*)|(-?))(?<value>\d{4})(?<separator>(\s*)|(-?))(?<value>\d{4})\s*$");

			/// <summary>获取中国固定电话号码的文本验证器。</summary>
			public static TextRegular Telephone = new TextRegular(@"^\s*(?<value>0\d{2,4})?(?<separator>(\s*)|(-?))(?<value>\d{4})(?<separator>(\s*)|(-?))(?<value>\d{3,4})\s*$");

			/// <summary>获取中国身份证号码的文本验证器。</summary>
			public static TextRegular IdentityNo = new TextRegular(@"^\s*(?<value>\d{6})?(?<separator>(\s*)|(-?))(?<value>\d{8})(?<separator>(\s*)|(-?))(?<value>(\d{4})|(\d{3}[A-Za-z]))\s*$");

			/// <summary>获取中国邮政编码的文本验证器。</summary>
			public static TextRegular PostalCode = new TextRegular(@"^\s*(?<value>\d{6})\s*$");
		}
		#endregion
	}
}

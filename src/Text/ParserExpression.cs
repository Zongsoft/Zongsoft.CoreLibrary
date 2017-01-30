/*
 * Authors:
 *   钟峰(Popeye Zhong) <9555843@qq.com>
 *
 * Copyright (C) 2010-2017 Zongsoft Corporation <http://www.zongsoft.com>
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

namespace Zongsoft.Text
{
	/// <summary>
	/// 表示解析器的表达式。
	/// </summary>
	public class ParserExpression
	{
		#region 成员字段
		private string _scheme;
		private string _content;
		private ParserExpression _next;
		#endregion

		#region 构造函数
		internal ParserExpression(string scheme, string content, ParserExpression next = null)
		{
			if(string.IsNullOrWhiteSpace(scheme))
				throw new ArgumentNullException(nameof(scheme));

			_scheme = scheme.ToLowerInvariant().Trim();
			_content = content;
			_next = next;
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取解析器的方案名。
		/// </summary>
		public string Scheme
		{
			get
			{
				return _scheme;
			}
		}

		/// <summary>
		/// 获取解析器表达式的内容文本。
		/// </summary>
		public string Content
		{
			get
			{
				return _content;
			}
		}

		/// <summary>
		/// 获取下位解析器表达式，即通过管道符连接的下一个解析器表达式。
		/// </summary>
		public ParserExpression Next
		{
			get
			{
				return _next;
			}
			internal protected set
			{
				_next = value;
			}
		}
		#endregion

		#region 重写方法
		public override string ToString()
		{
			if(_content == null)
				return $"${{{_scheme}}}";
			else
				return $"${{{_scheme}:{_content}}}";
		}
		#endregion
	}
}

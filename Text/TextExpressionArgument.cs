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
using System.Collections.Generic;

namespace Zongsoft.Text
{
	public class TextExpressionArgument
	{
		#region 私有变量
		private int _expressionEvaluated;
		#endregion

		#region 成员字段
		private string _format;
		private string _text;
		private TextExpressionNodeCollection _nodes;
		#endregion

		#region 构造函数
		internal TextExpressionArgument(string text, string format)
		{
			_text = text ?? string.Empty;
			_format = format ?? string.Empty;
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取表达式参数的格式字符串。
		/// </summary>
		public string Format
		{
			get
			{
				return _format;
			}
		}

		/// <summary>
		/// 获取表达式参数的文本值。
		/// </summary>
		public string Text
		{
			get
			{
				return _text;
			}
		}

		/// <summary>
		/// 获取表达式参数的文本值(即<see cref="Text"/>属性)对应的表达式节点集合(<seealso cref="TextExpressionNodeCollection"/>)，如果<see cref="Text"/>属性不是一个有效的表达式则返回空集。
		/// </summary>
		/// <remarks>
		///		<para>该属性采用延迟计算机制，如果不获取本属性则不会解析表达式参数文本值。</para>
		/// </remarks>
		public TextExpressionNodeCollection Nodes
		{
			get
			{
				if(_expressionEvaluated == 0)
				{
					var textEvaluated = System.Threading.Interlocked.CompareExchange(ref _expressionEvaluated, 1, 0);

					if(textEvaluated == 0)
						_nodes = TextExpressionParser.Parse(_text);
				}

				return _nodes;
			}
		}
		#endregion

		#region 重写方法
		public override string ToString()
		{
			return _text + (string.IsNullOrWhiteSpace(_format) ? string.Empty : "#" + _format);
		}
		#endregion
	}
}

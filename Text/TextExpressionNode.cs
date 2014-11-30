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
	/// <summary>
	/// 表示表达式节点的类。
	/// </summary>
	[Obsolete()]
	public class TextExpressionNode
	{
		#region 成员字段
		private int _index;
		private int _length;
		private string _text;
		private TextExpression _expression;
		#endregion

		#region 构造函数
		public TextExpressionNode(int index, int length, string text) : this(index, length, text, null)
		{
		}

		public TextExpressionNode(int index, int length, string text, TextExpression expression)
		{
			_text = text;
			_index = Math.Max(index, -1);
			_length = Math.Max(length, 0);
			_expression = expression;
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取表达式节点位于当前文本表达式中的起始位置。
		/// </summary>
		public int Index
		{
			get
			{
				return _index;
			}
		}

		/// <summary>
		/// 获取表达式节点位于当前文本表达式中的字符个数。
		/// </summary>
		public int Length
		{
			get
			{
				return _length;
			}
		}

		/// <summary>
		/// 获取表达式节点的原始文本值。
		/// </summary>
		public string Text
		{
			get
			{
				return _text;
			}
		}

		/// <summary>
		/// 获取表达式节点的表达式，如果本节点为常量节点(即不是表达式节点)则返回空(null)。
		/// </summary>
		public TextExpression Expression
		{
			get
			{
				return _expression;
			}
		}
		#endregion
	}
}

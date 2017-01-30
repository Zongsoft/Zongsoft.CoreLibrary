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
	/// 表示解析器解析操作的上下文对象。
	/// </summary>
	public class ParserContextBase
	{
		#region 成员字段
		private ParserExpression _expression;
		private object _host;
		private object _parameter;
		#endregion

		#region 构造函数
		protected ParserContextBase(ParserExpression expression, object host, object parameter)
		{
			if(expression == null)
				throw new ArgumentNullException(nameof(expression));

			_expression = expression;
			_host = host;
			_parameter = parameter;
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取解析上下文对应的表达式。
		/// </summary>
		public ParserExpression Expression
		{
			get
			{
				return _expression;
			}
		}

		/// <summary>
		/// 获取解析上下文所属的宿主。
		/// </summary>
		public object Host
		{
			get
			{
				return _host;
			}
		}

		/// <summary>
		/// 获取解析上下文的输入参数。
		/// </summary>
		/// <remarks>
		///		在串联解析中（即通过管道符连接的表达式中），该属性值通常表示前一个解析器的解析结果或者前一个表达式值。
		/// </remarks>
		public object Parameter
		{
			get
			{
				return _parameter;
			}
		}
		#endregion
	}
}

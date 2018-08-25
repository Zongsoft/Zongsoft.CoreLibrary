/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@qq.com>
 *
 * Copyright (C) 2010-2018 Zongsoft Corporation <http://www.zongsoft.com>
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

namespace Zongsoft.Reflection.Expressions
{
	/// <summary>
	/// 提供表达式元素的扩展方法类。
	/// </summary>
	public static class MemberExpressionExtension
	{
		/// <summary>
		/// 查找指定表达式节点位于表达式中的首个节点元素。
		/// </summary>
		/// <param name="expression">指定的要查找的表达式节点。</param>
		/// <returns>返回的首个表达式元素。</returns>
		public static IMemberExpression First(this IMemberExpression expression)
		{
			if(expression == null)
				return null;

			while(expression.Previous != null)
			{
				expression = expression.Previous;
			}

			return expression;
		}

		/// <summary>
		/// 查找指定表达式节点位于表达式中的最末节点元素。
		/// </summary>
		/// <param name="expression">指定的要查找的表达式节点。</param>
		/// <returns>返回的最末表达式元素。</returns>
		public static IMemberExpression Last(this IMemberExpression expression)
		{
			if(expression == null)
				return null;

			while(expression.Next != null)
			{
				expression = expression.Next;
			}

			return expression;
		}
	}
}

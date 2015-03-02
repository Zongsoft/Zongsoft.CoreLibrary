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
using System.Collections.Generic;

namespace Zongsoft.Runtime.Expressions
{
	public class ExpressionOperator
	{
		#region 常量定义
		internal const string ADDITION_SYMBOL = "+";
		internal const string SUBTRACT_SYMBOL = "-";
		internal const string MULTIPLE_SYMBOL = "*";
		internal const string DIVISION_SYMBOL = "/";
		internal const string MODULO_SYMBOL = "%";
		internal const string MINUS_SYMBOL = "-";
		internal const string NOT_SYMBOL = "!";
		internal const string AND_SYMBOL = "&&";
		internal const string OR_SYMBOL = "||";
		internal const string EQUAL_SYMBOL = "==";
		internal const string NOTEQUAL_SYMBOL = "!=";
		internal const string GREATERTHAN_SYMBOL = ">";
		internal const string GREATERTHANEQUAL_SYMBOL = ">=";
		internal const string LESSTHAN_SYMBOL = "<";
		internal const string LESSTHANEQUAL_SYMBOL = "<=";
		internal const string ISNULL_SYMBOL = "??";
		#endregion

		#region 公共字段
		public readonly string Symbol;
		public readonly int Priority;
		public readonly ExpressionOperatorKind Kind;
		#endregion

		#region 构造函数
		public ExpressionOperator(string symbol, ExpressionOperatorKind kind, byte priority)
		{
			if(string.IsNullOrWhiteSpace(symbol))
				throw new ArgumentNullException("symbol");

			this.Symbol = symbol.Trim();
			this.Kind = kind;
			this.Priority = priority;
		}
		#endregion

		#region 静态字段
		public static readonly ExpressionOperator Addition = new ExpressionOperator(ADDITION_SYMBOL, ExpressionOperatorKind.Two, 1);
		public static readonly ExpressionOperator Subtract = new ExpressionOperator(SUBTRACT_SYMBOL, ExpressionOperatorKind.Two, 1);
		public static readonly ExpressionOperator Multiple = new ExpressionOperator(MULTIPLE_SYMBOL, ExpressionOperatorKind.Two, 2);
		public static readonly ExpressionOperator Division = new ExpressionOperator(DIVISION_SYMBOL, ExpressionOperatorKind.Two, 2);
		public static readonly ExpressionOperator Modulo = new ExpressionOperator(MODULO_SYMBOL, ExpressionOperatorKind.Two, 2);
		public static readonly ExpressionOperator Minus = new ExpressionOperator(MINUS_SYMBOL, ExpressionOperatorKind.One, 3);
		public static readonly ExpressionOperator Not = new ExpressionOperator(NOT_SYMBOL, ExpressionOperatorKind.One, 3);
		public static readonly ExpressionOperator And = new ExpressionOperator(AND_SYMBOL, ExpressionOperatorKind.Two, 1);
		public static readonly ExpressionOperator Or = new ExpressionOperator(OR_SYMBOL, ExpressionOperatorKind.Two, 1);
		public static readonly ExpressionOperator Equal = new ExpressionOperator(EQUAL_SYMBOL, ExpressionOperatorKind.Two, 1);
		public static readonly ExpressionOperator NotEqual = new ExpressionOperator(NOTEQUAL_SYMBOL, ExpressionOperatorKind.Two, 1);
		public static readonly ExpressionOperator GreaterThan = new ExpressionOperator(GREATERTHAN_SYMBOL, ExpressionOperatorKind.Two, 1);
		public static readonly ExpressionOperator GreaterThanEqual = new ExpressionOperator(GREATERTHANEQUAL_SYMBOL, ExpressionOperatorKind.Two, 1);
		public static readonly ExpressionOperator LessThan = new ExpressionOperator(LESSTHAN_SYMBOL, ExpressionOperatorKind.Two, 1);
		public static readonly ExpressionOperator LessThanEqual = new ExpressionOperator(LESSTHANEQUAL_SYMBOL, ExpressionOperatorKind.Two, 1);
		public static readonly ExpressionOperator IsNull = new ExpressionOperator(ISNULL_SYMBOL, ExpressionOperatorKind.Two, 1);
		#endregion

		#region 静态方法
		//public static ExpressionOperator Parse(string symbol)
		//{
		//	if(string.IsNullOrWhiteSpace(symbol))
		//		throw new ArgumentNullException("symbol");

		//	switch(symbol.Trim())
		//	{
		//		case ADDITION_SYMBOL:
		//			return Addition;
		//		case SUBTRACT_SYMBOL:
		//			return Subtract;
		//		case MULTIPLE_SYMBOL:
		//			return Multiple;
		//		case DIVISION_SYMBOL:
		//			return Division;
		//		case MODULO_SYMBOL:
		//			return Modulo;
		//		case MINUS_SYMBOL:
		//			return Minus;
		//		case NOT_SYMBOL:
		//			return Not;
		//		case AND_SYMBOL:
		//			return And;
		//		case OR_SYMBOL:
		//			return Or;
		//		case EQUAL_SYMBOL:
		//			return Equal;
		//		case NOTEQUAL_SYMBOL:
		//			return NotEqual;
		//		case GREATERTHAN_SYMBOL:
		//			return GreaterThan;
		//		case GREATERTHANEQUAL_SYMBOL:
		//			return GreaterThanEqual;
		//		case LESSTHAN_SYMBOL:
		//			return LessThan;
		//		case LESSTHANEQUAL_SYMBOL:
		//			return LessThanEqual;
		//		case ISNULL_SYMBOL:
		//			return IsNull;
		//	}

		//	return null;
		//}
		#endregion
	}
}

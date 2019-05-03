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
	/// 表示成员表达式元素的基类。
	/// </summary>
	public abstract class MemberExpression : IMemberExpression
	{
		#region 成员字段
		private IMemberExpression _previous;
		private IMemberExpression _next;
		#endregion

		#region 构造函数
		protected MemberExpression()
		{
		}
		#endregion

		#region 公共属性
		public abstract MemberExpressionType ExpressionType
		{
			get;
		}

		public IMemberExpression Previous
		{
			get
			{
				return _previous;
			}
		}

		public IMemberExpression Next
		{
			get
			{
				return _next;
			}
		}
		#endregion

		#region 公共方法
		public T Append<T>(T expression) where T : MemberExpression
		{
			if(expression == null)
				throw new ArgumentNullException(nameof(expression));

			_next = expression;
			expression._previous = this;
			return expression;
		}

		public T Prepend<T>(T expression) where T : MemberExpression
		{
			if(expression == null)
				throw new ArgumentNullException(nameof(expression));

			_previous = expression;
			expression._next = this;
			return expression;
		}
		#endregion

		#region 解析方法
		public static bool TryParse(string text, out IMemberExpression expression)
		{
			return (expression = MemberExpressionParser.Parse(text, null)) != null;
		}

		public static bool TryParse(string text, int start, int count, out IMemberExpression expression)
		{
			return (expression = MemberExpressionParser.Parse(text, start, count, null)) != null;
		}

		public static IMemberExpression Parse(string text)
		{
			return MemberExpressionParser.Parse(text, message => throw new InvalidOperationException(message));
		}

		public static IMemberExpression Parse(string text, int start, int count)
		{
			return MemberExpressionParser.Parse(text, start, count, message => throw new InvalidOperationException(message));
		}

		public static IMemberExpression Parse(string text, Action<string> onError)
		{
			return MemberExpressionParser.Parse(text, onError);
		}

		public static IMemberExpression Parse(string text, int start, int count, Action<string> onError)
		{
			return MemberExpressionParser.Parse(text, start, count, onError);
		}
		#endregion

		#region 静态方法
		public static bool IsNull(object value)
		{
			return value == null || (value is ConstantExpression constant && constant.Value == null);
		}

		public static ConstantExpression Constant(object value)
		{
			return value == null ?
			       ConstantExpression.Null :
			       new ConstantExpression(value);
		}

		public static ConstantExpression Constant(string literal, TypeCode type)
		{
			if(literal == null)
				return ConstantExpression.Null;

			switch(type)
			{
				case TypeCode.Boolean:
					return new ConstantExpression(bool.Parse(literal));
				case TypeCode.Int16:
					return new ConstantExpression(short.Parse(literal));
				case TypeCode.Int32:
					return new ConstantExpression(int.Parse(literal));
				case TypeCode.Int64:
					return new ConstantExpression(long.Parse(literal));
				case TypeCode.UInt16:
					return new ConstantExpression(ushort.Parse(literal));
				case TypeCode.UInt32:
					return new ConstantExpression(uint.Parse(literal));
				case TypeCode.UInt64:
					return new ConstantExpression(ulong.Parse(literal));
				case TypeCode.Double:
					return new ConstantExpression(double.Parse(literal));
				case TypeCode.Single:
					return new ConstantExpression(float.Parse(literal));
				case TypeCode.Decimal:
					return new ConstantExpression(decimal.Parse(literal));
				case TypeCode.Byte:
					return new ConstantExpression(byte.Parse(literal));
				case TypeCode.Char:
					return new ConstantExpression(string.IsNullOrEmpty(literal) ? '\0' : literal[0]);
				case TypeCode.SByte:
					return new ConstantExpression(sbyte.Parse(literal));
				case TypeCode.DateTime:
					return new ConstantExpression(DateTime.Parse(literal));
				case TypeCode.Object:
					if(string.IsNullOrEmpty(literal) || literal.Equals("null", StringComparison.OrdinalIgnoreCase))
						return ConstantExpression.Null;
					else
						return new ConstantExpression(literal);
				default:
					return new ConstantExpression(literal);
			}
		}

		public static MethodExpression Method(string name, params IMemberExpression[] arguments)
		{
			return new MethodExpression(name, arguments);
		}

		public static IndexerExpression Indexer(params IMemberExpression[] arguments)
		{
			return new IndexerExpression(arguments);
		}

		public static IdentifierExpression Identifier(string name)
		{
			return new IdentifierExpression(name);
		}
		#endregion
	}
}

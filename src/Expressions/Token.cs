/*
 * Authors:
 *   钟峰(Popeye Zhong) <9555843@qq.com>
 *
 * Copyright (C) 2017 Zongsoft Corporation <http://www.zongsoft.com>
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

namespace Zongsoft.Expressions
{
	/// <summary>
	/// 表示词素的类。
	/// </summary>
	public class Token
	{
		#region 静态字段
		/// <summary>表示空(null)的词素。</summary>
		public static readonly Token Null = new Token(TokenType.Constant, null);

		/// <summary>表示逻辑真(true)的词素。</summary>
		public static readonly Token True = new Token(TokenType.Constant, true);

		/// <summary>表示逻辑假(false)的词素。</summary>
		public static readonly Token False = new Token(TokenType.Constant, false);
		#endregion

		#region 成员字段
		private readonly TokenType _type;
		private readonly object _value;
		#endregion

		#region 构造函数
		public Token(TokenType type, object value)
		{
			_type = type;
			_value = value;
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取词素的类型。
		/// </summary>
		public TokenType Type
		{
			get
			{
				return _type;
			}
		}

		/// <summary>
		/// 获取词素的值。
		/// </summary>
		public object Value
		{
			get
			{
				return _value;
			}
		}
		#endregion

		#region 重写方法
		public override int GetHashCode()
		{
			if(_value == null)
				return _type.GetHashCode();
			else
				return _type.GetHashCode() ^ _value.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if(obj == null || obj.GetType() != this.GetType())
				return false;

			return _type == ((Token)obj).Type && _value == ((Token)obj).Value;
		}

		public override string ToString()
		{
			return $"{_type}: {_value}";
		}
		#endregion
	}
}

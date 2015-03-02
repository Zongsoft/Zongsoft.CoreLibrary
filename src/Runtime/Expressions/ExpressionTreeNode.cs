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
	public class ExpressionTreeNode : MarshalByRefObject
	{
		#region 成员字段
		private int _index;
		private int _length;
		private string _text;
		private object _value;
		private ExpressionTreeNodeType _type;
		private ExpressionTreeNode _parent;
		private ExpressionTreeNodeCollection _children;
		#endregion

		#region 构造函数
		public ExpressionTreeNode(int index, int length, string text, object value, ExpressionTreeNodeType type, ExpressionTreeNode parent)
		{
			_index = Math.Max(index, -1);
			_length = Math.Max(length, 0);
			_text = text;
			_type = type;
			_value = value;
			_parent = parent;
			_children = new ExpressionTreeNodeCollection(this);
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取表达式节点的上级节点。
		/// </summary>
		public ExpressionTreeNode Parent
		{
			get
			{
				return _parent;
			}
			internal set
			{
				_parent = value;
			}
		}

		/// <summary>
		/// 获取表达式节点的子节点集合。
		/// </summary>
		public ExpressionTreeNodeCollection Children
		{
			get
			{
				return _children;
			}
		}

		/// <summary>
		/// 获取表达式节点位于表达式文本中的起始位置。
		/// </summary>
		public int Index
		{
			get
			{
				return _index;
			}
		}

		/// <summary>
		/// 获取表达式节点位于表达式文本中的字符个数。
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
		/// 获取表达式节点的内容值。
		/// </summary>
		public object Value
		{
			get
			{
				return _value;
			}
		}

		/// <summary>
		/// 获取表达式节点的节点类型。
		/// </summary>
		public ExpressionTreeNodeType Type
		{
			get
			{
				return _type;
			}
		}
		#endregion
	}
}

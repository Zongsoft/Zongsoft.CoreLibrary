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
	public class ExpressionTreeNodeCollection : Zongsoft.Collections.Collection<ExpressionTreeNode>
	{
		#region 成员字段
		private ExpressionTreeNode _owner;
		#endregion

		#region 构造函数
		public ExpressionTreeNodeCollection(ExpressionTreeNode owner)
		{
			_owner = owner;
		}
		#endregion

		#region 重写方法
		protected override void InsertItem(int index, ExpressionTreeNode node)
		{
			if(node == null)
				throw new ArgumentNullException("node");

			if(node.Parent != null)
				throw new InvalidOperationException();

			node.Parent = _owner;

			base.InsertItem(index, node);
		}
		#endregion
	}
}

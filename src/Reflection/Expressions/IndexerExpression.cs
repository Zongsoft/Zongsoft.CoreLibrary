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
using System.Collections.Generic;

namespace Zongsoft.Reflection.Expressions
{
	public class IndexerExpression : MemberExpression
	{
		#region 构造函数
		public IndexerExpression()
		{
			this.Arguments = new List<IMemberExpression>();
		}

		public IndexerExpression(IMemberExpression[] arguments)
		{
			if(arguments == null || arguments.Length == 0)
				this.Arguments = new List<IMemberExpression>();
			else
				this.Arguments = new List<IMemberExpression>(arguments);
		}
		#endregion

		#region 公共属性
		public override MemberExpressionType ExpressionType
		{
			get
			{
				return MemberExpressionType.Indexer;
			}
		}

		public bool HasArguments
		{
			get => this.Arguments != null && this.Arguments.Count > 0;
		}

		public IList<IMemberExpression> Arguments
		{
			get;
		}
		#endregion
	}
}

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
	public class IndexerExpression : MemberPathExpression
	{
		#region 成员字段
		private IList<IMemberPathExpression> _parameters;
		#endregion

		#region 构造函数
		public IndexerExpression()
		{
			_parameters = new List<IMemberPathExpression>();
		}

		public IndexerExpression(IMemberPathExpression[] parameters)
		{
			if(parameters == null || parameters.Length == 0)
				_parameters = new List<IMemberPathExpression>();
			else
				_parameters = new List<IMemberPathExpression>(parameters);
		}
		#endregion

		#region 公共属性
		public override MemberPathExpressionType ExpressionType
		{
			get
			{
				return MemberPathExpressionType.Indexer;
			}
		}

		public IList<IMemberPathExpression> Parameters
		{
			get
			{
				return _parameters;
			}
		}
		#endregion
	}
}

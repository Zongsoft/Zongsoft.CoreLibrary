/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2008-2013 Zongsoft Corporation <http://www.zongsoft.com>
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
using System.Collections;
using System.Collections.Generic;

namespace Zongsoft.ComponentModel
{
	[Obsolete]
	public class ConditionClauseCollection : CollectionBase
	{
		public ConditionClause this[int index]
		{
			get
			{
				return (ConditionClause)this.List[index];
			}
		}

		public ConditionClause Combine(ConditionCombine conditionCombine, object value)
		{
			if(value == null)
				throw new ArgumentNullException("value");

			ConditionClause clause = new ConditionClause(conditionCombine, value);
			this.List.Add(clause);

			return clause;
		}

		public void Remove(ConditionClause value)
		{
			if(value != null)
				this.List.Remove(value);
		}

		public void RemoveAt(int index)
		{
			this.List.RemoveAt(index);
		}
	}
}

/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2010-2013 Zongsoft Corporation <http://www.zongsoft.com>
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

namespace Zongsoft.Collections
{
	[Serializable]
	public class CategoryCollection : HierarchicalNodeCollection<Category>
	{
		#region 构造函数
		public CategoryCollection() : base(null)
		{
		}

		public CategoryCollection(Category owner) : base(owner)
		{
		}
		#endregion

		#region 公共方法
		public Category Add(string name, string title, string description)
		{
			var category = new Category(name, title, description);
			this.Add(category);
			return category;
		}

		public void AddRange(IEnumerable<Category> categories)
		{
			if(categories == null)
				return;

			foreach(var category in categories)
			{
				this.Add(category);
			}
		}

		public void AddRange(params Category[] categories)
		{
			this.AddRange((IEnumerable<Category>)categories);
		}
		#endregion
	}
}

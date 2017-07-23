/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2013-2017 Zongsoft Corporation <http://www.zongsoft.com>
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
using System.Threading;

namespace Zongsoft.ComponentModel
{
	public class SchemaCategory : Zongsoft.Collections.CategoryBase
	{
		#region 成员字段
		private SchemaCollection _schemas;
		private SchemaCategoryCollection _children;
		#endregion

		#region 构造函数
		public SchemaCategory()
		{
		}

		public SchemaCategory(string name) : this(name, name, string.Empty, true)
		{
		}

		public SchemaCategory(string name, string title, string description) : this(name, title, description, true)
		{
		}

		public SchemaCategory(string name, string title, string description, bool visible) : base(name, title, description, visible)
		{
		}
		#endregion

		#region 公共属性
		public SchemaCollection Schemas
		{
			get
			{
				if(_schemas == null)
					Interlocked.CompareExchange(ref _schemas, new SchemaCollection(), null);

				return _schemas;
			}
		}

		public SchemaCategory Parent
		{
			get
			{
				return (SchemaCategory)this.InnerParent;
			}
		}

		public SchemaCategoryCollection Children
		{
			get
			{
				if(_children == null)
					Interlocked.CompareExchange(ref _children, new SchemaCategoryCollection(this), null);

				return _children;
			}
		}
		#endregion
	}
}

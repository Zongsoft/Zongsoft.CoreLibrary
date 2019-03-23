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
using System.ComponentModel;

namespace Zongsoft.Collections
{
	[Serializable]
	[System.ComponentModel.DefaultProperty("Children")]
	public class Category : HierarchicalNode<Category>
	{
		#region 成员字段
		private bool _visible;
		private CategoryCollection _children;
		#endregion

		#region 构造函数
		public Category()
		{
			_visible = true;
		}

		public Category(string name) : this(name, name, string.Empty, true)
		{
		}

		public Category(string name, string title, string description) : this(name, title, description, true)
		{
		}

		public Category(string name, string title, string description, bool visible) : base(name, title, description)
		{
			_visible = visible;
		}
		#endregion

		#region 公共属性
		[DefaultValue(true)]
		public bool Visible
		{
			get
			{
				return _visible;
			}
			set
			{
				_visible = value;
			}
		}

		public CategoryCollection Children
		{
			get
			{
				if(_children == null)
					System.Threading.Interlocked.CompareExchange(ref _children, new CategoryCollection(this), null);

				return _children;
			}
		}
		#endregion

		#region 重写方法
		protected override HierarchicalNode GetChild(string name)
		{
			var children = _children;

			if(children != null && children.TryGet(name, out var child))
				return child;

			return null;
		}
		#endregion
	}
}

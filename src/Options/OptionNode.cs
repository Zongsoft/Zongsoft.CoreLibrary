/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2005-2008 Zongsoft Corporation <http://www.zongsoft.com>
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
using System.Reflection;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zongsoft.Options
{
	public class OptionNode : Zongsoft.Collections.HierarchicalNode
	{
		#region 成员变量
		private IOption _option;
		private OptionNodeCollection _children;
		private string _title;
		private string _description;
		#endregion

		#region 构造函数
		internal OptionNode()
		{
			_children = new OptionNodeCollection(this);
		}

		public OptionNode(string name) : this(name, name, null)
		{
		}

		public OptionNode(string name, string title, string description) : base(name)
		{
			_title = string.IsNullOrWhiteSpace(title) ? name : title;
			_description = description ?? string.Empty;
			_children = new OptionNodeCollection(this);
		}

		public OptionNode(string name, IOption option) : base(name)
		{
			if(option == null)
				throw new ArgumentNullException("option");

			_option = option;
			_children = new OptionNodeCollection(this);
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取或设置选项节点的标题文本。
		/// </summary>
		public string Title
		{
			get
			{
				return _title;
			}
			set
			{
				_title = value ?? string.Empty;
			}
		}

		/// <summary>
		/// 获取或设置选项节点的描述文本。
		/// </summary>
		public string Description
		{
			get
			{
				return _description;
			}
			set
			{
				_description = value ?? string.Empty;
			}
		}

		/// <summary>
		/// 获取或设置选项节点对应的选项对象。
		/// </summary>
		public IOption Option
		{
			get
			{
				return _option;
			}
			set
			{
				if(object.ReferenceEquals(_option, value))
					return;

				_option = value;
			}
		}

		/// <summary>
		/// 获取选项节点的父节点，根节点的父节点为空(null)。
		/// </summary>
		public OptionNode Parent
		{
			get
			{
				return (OptionNode)base.InnerParent;
			}
			internal set
			{
				base.InnerParent = value;
			}
		}

		/// <summary>
		/// 获取选项节点的子节点集合。
		/// </summary>
		public OptionNodeCollection Children
		{
			get
			{
				//确认子节点集合是否已经加载过
				this.EnsureChildren();

				return _children;
			}
		}
		#endregion

		#region 公共方法
		public OptionNode Find(string path)
		{
			return base.FindNode(path) as OptionNode;
		}

		public OptionNode Find(params string[] parts)
		{
			return base.FindNode(parts) as OptionNode;
		}
		#endregion

		#region 重写方法
		protected override Collections.HierarchicalNode GetChild(string name)
		{
			if(_children != null && _children.TryGet(name, out var child))
				return child;

			return null;
		}
		#endregion
	}
}

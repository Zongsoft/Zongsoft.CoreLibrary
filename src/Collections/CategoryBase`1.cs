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

using Zongsoft.Runtime.Serialization;

namespace Zongsoft.Collections
{
	public abstract class CategoryBase<T> : HierarchicalNode where T : HierarchicalNode
	{
		#region 成员字段
		private string _title;
		private string _description;
		private string _tags;
		private bool _visible;
		#endregion

		#region 构造函数
		protected CategoryBase()
		{
			_title = base.PathSeparatorChar.ToString();
			_visible = true;
		}

		protected CategoryBase(string name)
			: this(name, name, string.Empty, true)
		{
		}

		protected CategoryBase(string name, string title, string description)
			: this(name, title, description, true)
		{
		}

		protected CategoryBase(string name, string title, string description, bool visible)
			: base(name)
		{
			_title = string.IsNullOrEmpty(title) ? name : title;
			_description = description;
			_visible = visible;
		}
		#endregion

		#region 公共属性
		public string Title
		{
			get
			{
				return _title;
			}
			set
			{
				_title = value;
			}
		}

		public string Description
		{
			get
			{
				return _description;
			}
			set
			{
				_description = value;
			}
		}

		public string Tags
		{
			get
			{
				return _tags;
			}
			set
			{
				_tags = value;
			}
		}

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

		[SerializationMember(SerializationMemberBehavior.Ignored)]
		public T Parent
		{
			get
			{
				return (T)base.InnerParent;
			}
		}
		#endregion

		#region 公共方法
		public T Find(string path, Func<HierarchicalNodeToken, T> step = null)
		{
			if(step == null)
				return (T)base.FindNode(path, 0, 0);
			else
				return (T)base.FindNode(path, 0, 0, token => step(new HierarchicalNodeToken(token)));
		}

		public T Find(string path, int startIndex, int length = 0, Func<HierarchicalNodeToken, T> step = null)
		{
			if(step == null)
				return (T)base.FindNode(path, startIndex, length);
			else
				return (T)base.FindNode(path, startIndex, length, token => step(new HierarchicalNodeToken(token)));
		}

		public T Find(string[] parts, Func<HierarchicalNodeToken, T> step = null)
		{
			if(step == null)
				return (T)base.FindNode(parts);
			else
				return (T)base.FindNode(parts, token => step(new HierarchicalNodeToken(token)));
		}

		public T Find(string[] parts, int startIndex, int count = 0, Func<HierarchicalNodeToken, T> step = null)
		{
			if(step == null)
				return (T)base.FindNode(parts, startIndex, count);
			else
				return (T)base.FindNode(parts, startIndex, count, token => step(new HierarchicalNodeToken(token)));
		}
		#endregion

		#region 嵌套子类
		public new struct HierarchicalNodeToken
		{
			public readonly string Name;
			public readonly int Index;
			public readonly T Parent;
			public readonly T Current;

			internal HierarchicalNodeToken(HierarchicalNode.HierarchicalNodeToken token)
			{
				this.Index = token.Index;
				this.Name = token.Name;
				this.Parent = token.Parent as T;
				this.Current = token.Current as T;
			}
		}
		#endregion
	}
}

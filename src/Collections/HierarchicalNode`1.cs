/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2010-2019 Zongsoft Corporation <http://www.zongsoft.com>
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

using Zongsoft.Runtime.Serialization;

namespace Zongsoft.Collections
{
	public abstract class HierarchicalNode<T> : HierarchicalNode where T : HierarchicalNode
	{
		#region 构造函数
		protected HierarchicalNode()
		{
			this.Title = base.PathSeparatorChar.ToString();
		}

		protected HierarchicalNode(string name) : this(name, name, string.Empty)
		{
		}

		protected HierarchicalNode(string name, string title, string description) : base(name)
		{
			this.Title = string.IsNullOrEmpty(title) ? name : title;
			this.Description = description;
		}
		#endregion

		#region 公共属性
		public string Title
		{
			get; set;
		}

		public string Description
		{
			get; set;
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

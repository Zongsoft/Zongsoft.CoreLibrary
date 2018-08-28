/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
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

namespace Zongsoft.Data
{
	public class SchemaBase
	{
		#region 单例字段
		internal static readonly SchemaBase Ignores = new SchemaBase("?");
		#endregion

		#region 成员字段
		private SchemaBase _parent;
		private Sorting[] _sortingArray;
		private HashSet<Sorting> _sortings;
		private Collections.NamedCollection<SchemaBase> _children;
		#endregion

		#region 构造函数
		protected SchemaBase()
		{
		}

		protected SchemaBase(string name)
		{
			if(string.IsNullOrEmpty(name))
				throw new ArgumentNullException(nameof(name));

			this.Name = name;
		}
		#endregion

		#region 公共属性
		public virtual string Name
		{
			get;
		}

		public Paging Paging
		{
			get;
			internal set;
		}

		public Sorting[] Sortings
		{
			get
			{
				return _sortingArray;
			}
		}

		public bool HasChildren
		{
			get
			{
				return _children != null && _children.Count > 0;
			}
		}

		public Collections.IReadOnlyNamedCollection<SchemaBase> Children
		{
			get
			{
				return _children;
			}
		}
		#endregion

		#region 保护属性
		protected SchemaBase GetParent()
		{
			return _parent;
		}
		#endregion

		#region 内部方法
		internal void AddSorting(Sorting sorting)
		{
			if(_sortings == null)
				System.Threading.Interlocked.CompareExchange(ref _sortings, new HashSet<Sorting>(SortingComparer.Instance), null);

			if(_sortings.Add(sorting))
			{
				var array = new Sorting[_sortings.Count];
				_sortings.CopyTo(array);
				_sortingArray = array;
			}
		}

		internal bool TryGetChild(string name, out SchemaBase child)
		{
			child = null;

			if(_children != null)
				return _children.TryGet(name, out child);

			return false;
		}

		internal bool ContainsChild(string name)
		{
			return _children != null && _children.Contains(name);
		}

		internal void AddChild(SchemaBase child)
		{
			if(_children == null)
				System.Threading.Interlocked.CompareExchange(ref _children, new Collections.NamedCollection<SchemaBase>(segment => segment.Name), null);

			_children.Add(child);
			child._parent = this;
		}

		internal void RemoveChild(string name)
		{
			_children?.Remove(name);
		}

		internal void ClearChildren()
		{
			_children?.Clear();
		}
		#endregion

		#region 重写方法
		public bool Equals(SchemaBase other)
		{
			if(other == null)
				return false;

			return string.Equals(this.Name, other.Name, StringComparison.OrdinalIgnoreCase);
		}

		public override bool Equals(object obj)
		{
			if(obj == null || obj.GetType() != typeof(SchemaBase))
				return false;

			return this.Equals((SchemaBase)obj);
		}

		public override int GetHashCode()
		{
			return this.Name.GetHashCode();
		}

		public override string ToString()
		{
			var index = 0;
			var text = this.Name;

			if(this.Paging != null)
			{
				if(Paging.IsDisabled(this.Paging))
					text += ":*";
				else
					text += ":" + (this.Paging.PageIndex == 1 ?
								   this.Paging.PageSize.ToString() :
								   this.Paging.PageIndex.ToString() + "/" + this.Paging.PageSize.ToString());
			}

			if(this.Sortings != null && this.Sortings.Length > 0)
			{
				index = 0;
				text += "[";

				foreach(var sorting in this.Sortings)
				{
					if(index++ > 0)
						text += ", ";

					if(sorting.Mode == SortingMode.Ascending)
						text += sorting.Name;
					else
						text += "~" + sorting.Name;
				}

				text += "]";
			}

			if(_children != null && _children.Count > 0)
			{
				index = 0;
				text += "(";

				foreach(var child in _children)
				{
					if(index++ > 0)
						text += ", ";

					text += child.ToString();
				}

				text += ")";
			}

			return text;
		}
		#endregion

		#region 解析方法
		protected static bool TryParse<T>(string text, out Collections.IReadOnlyNamedCollection<T> result, Func<Token, IEnumerable<T>> mapper) where T : SchemaBase
		{
			return (result = SchemaParser.Parse(text, mapper, null)) != null;
		}

		protected static Collections.IReadOnlyNamedCollection<T> Parse<T>(string text, Func<Token, IEnumerable<T>> mapper) where T : SchemaBase
		{
			return SchemaParser.Parse(text, mapper, message => throw new InvalidOperationException(message));
		}

		protected static Collections.IReadOnlyNamedCollection<T> Parse<T>(string text, Func<Token, IEnumerable<T>> mapper, Action<string> onError) where T : SchemaBase
		{
			return SchemaParser.Parse(text, mapper, onError);
		}
		#endregion

		#region 嵌套子类
		internal protected struct Token
		{
			public readonly string Name;
			public readonly SchemaBase Parent;

			internal Token(string name, SchemaBase parent)
			{
				this.Name = name;
				this.Parent = parent;
			}
		}

		private sealed class SortingComparer : IEqualityComparer<Sorting>
		{
			public static readonly SortingComparer Instance = new SortingComparer();

			private SortingComparer()
			{
			}

			public bool Equals(Sorting x, Sorting y)
			{
				return string.Equals(x.Name, y.Name, StringComparison.OrdinalIgnoreCase);
			}

			public int GetHashCode(Sorting sorting)
			{
				return sorting.Name.ToLowerInvariant().GetHashCode();
			}
		}
		#endregion
	}
}

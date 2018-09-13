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
	public abstract class SchemaBase
	{
		#region 单例字段
		internal static readonly SchemaBase Ignores = new EmptySchema();
		#endregion

		#region 成员字段
		private Sorting[] _sortingArray;
		private HashSet<Sorting> _sortings;
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

		public string Path
		{
			get
			{
				var parent = this.GetParent();

				if(parent == null)
					return string.Empty;
				else
				{
					if(string.IsNullOrEmpty(parent.Path))
						return parent.Name;
					else
						return parent.Path + "." + parent.Name;
				}
			}
		}

		public string FullPath
		{
			get
			{
				var path = this.Path;

				if(string.IsNullOrEmpty(path))
					return this.Name;
				else
					return path + "." + this.Name;
			}
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

		public abstract bool HasChildren
		{
			get;
		}
		#endregion

		#region 抽象方法
		protected abstract SchemaBase GetParent();
		protected abstract void SetParent(SchemaBase parent);

		internal protected abstract bool TryGetChild(string name, out SchemaBase child);
		internal protected abstract void AddChild(SchemaBase child);
		internal protected abstract void RemoveChild(string name);
		internal protected abstract void ClearChildren();
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
			return this.FullPath;
		}
		#endregion

		#region 解析方法
		protected static bool TryParse<T>(string text, out Collections.IReadOnlyNamedCollection<T> result, Func<Token, IEnumerable<T>> mapper, object data = null) where T : SchemaBase
		{
			return (result = SchemaParser.Parse(text, mapper, null, data)) != null;
		}

		protected static Collections.IReadOnlyNamedCollection<T> Parse<T>(string text, Func<Token, IEnumerable<T>> mapper, object data = null) where T : SchemaBase
		{
			return SchemaParser.Parse(text, mapper, message => throw new InvalidOperationException(message), data);
		}

		protected static Collections.IReadOnlyNamedCollection<T> Parse<T>(string text, Func<Token, IEnumerable<T>> mapper, Action<string> onError, object data = null) where T : SchemaBase
		{
			return SchemaParser.Parse(text, mapper, onError, data);
		}
		#endregion

		#region 嵌套子类
		internal protected struct Token
		{
			#region 公共字段
			public readonly string Name;
			public readonly SchemaBase Parent;
			#endregion

			#region 构造函数
			internal Token(string name, SchemaBase parent)
			{
				this.Name = name;
				this.Parent = parent;
				this.Data = null;
			}
			#endregion

			#region 公共属性
			/// <summary>
			/// 获取或设置用户自定义数据。
			/// </summary>
			public object Data
			{
				get;
				set;
			}
			#endregion
		}

		private class EmptySchema : SchemaBase
		{
			public override string Name => "?";
			public override bool HasChildren => false;

			protected override SchemaBase GetParent()
			{
				return null;
			}

			protected override void SetParent(SchemaBase parent)
			{
			}

			protected internal override void AddChild(SchemaBase child)
			{
			}

			protected internal override void ClearChildren()
			{
			}

			protected internal override void RemoveChild(string name)
			{
			}

			protected internal override bool TryGetChild(string name, out SchemaBase child)
			{
				child = null;
				return false;
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

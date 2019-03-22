/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2013-2014 Zongsoft Corporation <http://www.zongsoft.com>
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
	/// <summary>
	/// 表示层次结构的节点类。
	/// </summary>
	[Serializable]
	public abstract class HierarchicalNode
	{
		#region 公共常量
		[Runtime.Serialization.SerializationMember(Runtime.Serialization.SerializationMemberBehavior.Ignored)]
		public readonly char PathSeparatorChar = '/';
		#endregion

		#region 私有变量
		private int _childrenLoaded;
		#endregion

		#region 成员变量
		private string _name;
		private string _path;
		private HierarchicalNode _parent;
		#endregion

		#region 构造函数
		protected HierarchicalNode()
		{
			_name = "/";
			PathSeparatorChar = '/';
		}

		protected HierarchicalNode(string name, HierarchicalNode parent = null, char pathSeparatorChar = '/')
		{
			if(string.IsNullOrWhiteSpace(name))
				throw new ArgumentNullException("name");

			if(name.Contains(pathSeparatorChar.ToString()))
				throw new ArgumentException("The name contains path separator char.");

			if(Zongsoft.Common.StringExtension.ContainsCharacters(name, @"./\*?!@#$%^&"))
				throw new ArgumentException("The name contains invalid character(s).");

			_name = name.Trim();
			_parent = parent;
			PathSeparatorChar = pathSeparatorChar;
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取层次结构节点的名称，名称不可为空或空字符串，根节点的名称固定为斜杠(即“/”)。
		/// </summary>
		public virtual string Name
		{
			get
			{
				return _name;
			}
		}

		/// <summary>
		/// 获取层次结构节点的路径。
		/// </summary>
		/// <remarks>
		///		<para>如果为根节点则返回空字符串("")，否则即为父节点的全路径。</para>
		/// </remarks>
		public string Path
		{
			get
			{
				if(_path == null)
				{
					if(_parent == null)
						_path = string.Empty;
					else
						_path = _parent.FullPath;
				}

				return _path;
			}
		}

		/// <summary>
		/// 获取层次结构节点的完整路径，即节点路径与名称的组合。
		/// </summary>
		public string FullPath
		{
			get
			{
				var path = this.Path;

				switch(path)
				{
					case "":
						return _name;
					default:
						if(path.Length == 1 && path[0] == PathSeparatorChar)
							return path + _name;
						else
							return path + PathSeparatorChar + _name;
				}
			}
		}

		/// <summary>
		/// 获取或设置层次结构节点的父节点，根节点的父节点为空(null)。
		/// </summary>
		internal protected HierarchicalNode InnerParent
		{
			get
			{
				return _parent;
			}
			set
			{
				if(object.ReferenceEquals(_parent, value))
					return;

				//设置父节点
				_parent = value;

				//更新层次结构节点的路径
				_path = null;
			}
		}
		#endregion

		#region 公共方法
		public virtual HierarchicalNode FindRoot()
		{
			var current = this;
			var stack = new Stack<HierarchicalNode>();

			while(current != null)
			{
				if(current._parent == null)
					return current;

				//如果当前节点是否已经在遍历的栈中，则抛出循环引用的异常
				if(stack.Contains(current))
					throw new InvalidOperationException($"Error occurred: The name as '{current.Name}' hierarchical node is circular referenced.");

				//将当前节点加入到遍历栈中
				stack.Push(current);

				//指向当前节点的父节点
				current = current._parent;
			}

			return current;
		}
		#endregion

		#region 保护方法
		internal protected HierarchicalNode FindNode(string path, Func<HierarchicalNodeToken, HierarchicalNode> onStep = null)
		{
			//注意：一定要确保空字符串路径是返回自身
			if(string.IsNullOrWhiteSpace(path))
				return this;

			return this.FindNode(path, 0, 0, onStep);
		}

		internal protected HierarchicalNode FindNode(string path, int startIndex, int length = 0, Func<HierarchicalNodeToken, HierarchicalNode> onStep = null)
		{
			if(startIndex < 0 || startIndex >= path.Length)
				throw new ArgumentOutOfRangeException(nameof(startIndex));

			if(length > 0 && length > path.Length - startIndex)
				throw new ArgumentOutOfRangeException(nameof(length));

			//注意：一定要确保空字符串路径是返回自身
			if(path == null || path.Length == 0)
				return this;

			//确保当前子节点集合已经被加载过
			this.EnsureChildren();

			//当前节点默认为本节点
			var current = this;

			int last = startIndex;
			int spaces = 0;
			int index = 0;

			for(int i = startIndex; i < (length > 0 ? length : path.Length - startIndex); i++)
			{
				if(path[i] == PathSeparatorChar)
				{
					if(index++ == 0)
					{
						if(last == i)
							current = this.FindRoot();
					}

					if(i - last > spaces)
					{
						current = this.FindStep(current, index - 1, path, i, last, spaces, onStep);

						if(current == null)
							return null;
					}

					spaces = -1;
					last = i + 1;
				}
				else if(char.IsWhiteSpace(path, i))
				{
					if(i == last)
						last = i + 1;
					else
						spaces++;
				}
				else
				{
					spaces = 0;
				}
			}

			if(last < path.Length - spaces - 1)
				current = this.FindStep(current, index, path, path.Length, last, spaces, onStep);

			return current;
		}

		internal protected HierarchicalNode FindNode(string[] paths, Func<HierarchicalNodeToken, HierarchicalNode> onStep = null)
		{
			if(paths == null || paths.Length == 0)
				return null;

			return this.FindNode(string.Join(PathSeparatorChar.ToString(), paths), onStep);
		}

		internal protected HierarchicalNode FindNode(string[] paths, int startIndex, int count = 0, Func<HierarchicalNodeToken, HierarchicalNode> onStep = null)
		{
			if(startIndex < 0 || startIndex >= paths.Length)
				throw new ArgumentOutOfRangeException(nameof(startIndex));

			if(count > 0 && count > paths.Length - startIndex)
				throw new ArgumentOutOfRangeException(nameof(count));

			if(paths == null || paths.Length == 0)
				return null;

			return this.FindNode(string.Join(PathSeparatorChar.ToString(), paths, startIndex, (count > 0 ? count : paths.Length - startIndex)), onStep);
		}
		#endregion

		#region 虚拟方法
		/// <summary>
		/// 确认子节点集合是否被加载，如果未曾被加载则加载子节点集合。
		/// </summary>
		/// <returns>如果子节点集合未曾被加载则加载当前子节点集合并返回真(true)，否则返回假(false)。</returns>
		/// <remarks>
		///		<para>在<seealso cref="LoadChildren"/>方法中会调用该方法以确保子节点被加载。</para>
		/// </remarks>
		protected bool EnsureChildren()
		{
			var childrenLoaded = System.Threading.Interlocked.Exchange(ref _childrenLoaded, 1);

			if(childrenLoaded == 0)
				this.LoadChildren();

			return childrenLoaded == 0;
		}

		/// <summary>
		/// 加载当前节点的子节点集合。
		/// </summary>
		protected virtual void LoadChildren()
		{
		}

		/// <summary>
		/// 获取指定名称的子节点对象。
		/// </summary>
		/// <param name="name">指定要查找的子节点名称。</param>
		/// <returns>如果找到指定名称的子节点则返回它，否则返回空(null)。</returns>
		protected abstract HierarchicalNode GetChild(string name);
		#endregion

		#region 私有方法
		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
		private HierarchicalNode FindStep(HierarchicalNode current, int index, string path, int i, int last, int spaces, Func<HierarchicalNodeToken, HierarchicalNode> onStep)
		{
			var part = path.Substring(last, i - last - spaces);
			HierarchicalNode parent = null;

			switch(part)
			{
				case "":
				case ".":
					return current;
				case "..":
					if(current._parent != null)
						current = current._parent;

					break;
				default:
					parent = current;
					current = parent.GetChild(part);
					break;
			}

			if(onStep != null)
				current = onStep(new HierarchicalNodeToken(index, part, current, parent));

			return current;
		}
		#endregion

		#region 嵌套子类
		public struct HierarchicalNodeToken
		{
			public readonly string Name;
			public readonly int Index;
			public readonly HierarchicalNode Parent;
			public readonly HierarchicalNode Current;

			internal HierarchicalNodeToken(int index, string name, HierarchicalNode current, HierarchicalNode parent = null)
			{
				this.Index = index;
				this.Name = name;
				this.Current = current;

				this.Parent = parent ?? (current != null ? current.InnerParent : null);
			}
		}
		#endregion
	}
}

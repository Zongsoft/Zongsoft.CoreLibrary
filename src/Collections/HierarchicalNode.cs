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
	public class HierarchicalNode : MarshalByRefObject
	{
		#region 私有常量
		private const char DefaultPathSeparatorChar = '/';
		#endregion

		#region 公共常量
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
			_name = DefaultPathSeparatorChar.ToString();
			PathSeparatorChar = DefaultPathSeparatorChar;
		}

		protected HierarchicalNode(string name) : this(name, null, DefaultPathSeparatorChar)
		{
		}

		protected HierarchicalNode(string name, HierarchicalNode parent, char pathSeparatorChar = DefaultPathSeparatorChar)
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

		#region 保护方法
		internal protected HierarchicalNode FindNode(string path, Func<HierarchicalNodeToken, HierarchicalNode> onStep = null)
		{
			if(string.IsNullOrWhiteSpace(path))
				return null;

			return this.FindNode(path.Split(PathSeparatorChar), onStep);
		}

		internal protected HierarchicalNode FindNode(string[] paths, Func<HierarchicalNodeToken, HierarchicalNode> onStep = null)
		{
			if(paths == null || paths.Length == 0)
				return null;

			//确保当前子节点集合已经被加载过
			this.EnsureChildren();

			//当前节点默认为本节点
			var current = this;

			//如果第一个部分是空字符则表示路径是以斜杠(/)打头或第一个部分是以斜杠(/)打头则从根节点开始查找
			if(string.IsNullOrWhiteSpace(paths[0]) || paths[0].Trim()[0] == PathSeparatorChar)
				current = this.FindRoot();

			int pathIndex = 0, partIndex = 0;
			string[] parts = null;

			while(current != null && pathIndex < paths.Length)
			{
				var part = string.Empty;
				HierarchicalNode parent = null;

				if(parts == null && paths[pathIndex].Contains(PathSeparatorChar.ToString()))
				{
					parts = paths[pathIndex].Split(PathSeparatorChar);
					partIndex = 0;
				}

				if(parts == null)
					part = paths[pathIndex++].Trim();
				else
				{
					if(partIndex < parts.Length)
						part = parts[partIndex++].Trim();
					else
					{
						parts = null;
						pathIndex++;
						continue;
					}
				}

				switch(part)
				{
					case "":
					case ".":
						continue;
					case "..":
						current = current._parent;
						parent = current != null ? current._parent : null;
						break;
					default:
						parent = current;
						current = current.GetChild(part);
						break;
				}

				if(onStep != null)
				{
					current = onStep(new HierarchicalNodeToken(part, current, parent));

					if(current == null)
						return null;
				}
			}

			return current;
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
		protected virtual HierarchicalNode GetChild(string name)
		{
			return null;
		}
		#endregion

		#region 私有方法
		private HierarchicalNode FindRoot()
		{
			var current = this;

			while(current != null)
			{
				if(current._parent == null)
					return current;

				current = current._parent;
			}

			return current;
		}
		#endregion

		#region 嵌套子类
		public class HierarchicalNodeToken
		{
			public readonly string Name;
			public readonly HierarchicalNode Parent;
			public readonly HierarchicalNode Current;

			internal HierarchicalNodeToken(string name, HierarchicalNode current, HierarchicalNode parent = null)
			{
				this.Name = name;
				this.Current = current;

				this.Parent = parent ?? (current != null ? current.InnerParent : null);
			}
		}
		#endregion
	}
}

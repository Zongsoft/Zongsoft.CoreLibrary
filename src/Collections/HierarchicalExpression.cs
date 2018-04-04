/*
 * Authors:
 *   钟峰(Popeye Zhong) <9555843@qq.com>
 *
 * Copyright (C) 2010-2017 Zongsoft Corporation <http://www.zongsoft.com>
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
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
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
	/// 表示层次结构的表达式类。
	/// </summary>
	/// <remarks>
	///		<para>层次结构表达式由“路径”和“成员集”两部分组成，其文本格式如下：</para>
	///		<list type="number">
	///			<item>
	///				<term>绝对路径：/root/node1/node2/node3.property1.property2 或 /Data/ConnectionStrings['Automao.SaaS'] </term>
	///				<term>相对路径：../siblingNode/node1/node2.property1.property2 或 childNode/node1/node2.property1.property2</term>
	///				<term>属性路径：../@property1.property2 或 ./@collectionProperty[index]（对于本节点的属性也可以简写成：@property1.property2）</term>
	///			</item>
	///		</list>
	/// </remarks>
	public class HierarchicalExpression
	{
		#region 成员字段
		private string _path;
		private string[] _segments;
		private Reflection.MemberPathSegment[] _members;
		private IO.PathAnchor _anchor;
		#endregion

		#region 构造函数
		private HierarchicalExpression(string[] segments, Reflection.MemberPathSegment[] members)
		{
			_segments = segments ?? new string[0];
			_members = members ?? new Reflection.MemberPathSegment[0];
			_anchor = IO.PathAnchor.None;
			_path = string.Empty;

			if(segments.Length > 0)
			{
				switch(segments[0])
				{
					case "":
						_anchor = IO.PathAnchor.Root;
						break;
					case ".":
						_anchor = IO.PathAnchor.Current;
						break;
					case "..":
						_anchor = IO.PathAnchor.Parent;
						break;
				}

				if(segments.Length == 1 && segments[0].Length == 0)
					_path = "/";
				else
					_path = string.Join("/", segments);
			}
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取层次路径的锚定点。
		/// </summary>
		public IO.PathAnchor Anchor
		{
			get
			{
				return _anchor;
			}
		}

		/// <summary>
		/// 获取层次表达式的路径。
		/// </summary>
		public string Path
		{
			get
			{
				return _path;
			}
		}

		/// <summary>
		/// 获取包含构成<see cref="Path"/>路径段的数组。
		/// </summary>
		public string[] Segments
		{
			get
			{
				return _segments;
			}
		}

		/// <summary>
		/// 获取层次表达式中的成员项数组。
		/// </summary>
		public Reflection.MemberPathSegment[] Members
		{
			get
			{
				return _members;
			}
		}
		#endregion

		#region 静态方法
		public static HierarchicalExpression Parse(string text)
		{
			return ParseCore(text, message =>
			{
				throw new ArgumentException(message);
			});
		}

		public static bool TryParse(string text, out HierarchicalExpression result)
		{
			result = null;

			if(string.IsNullOrEmpty(text))
				return false;

			result = ParseCore(text, null);
			return result != null;
		}
		#endregion

		#region 私有方法
		private static HierarchicalExpression ParseCore(string text, Action<string> onError)
		{
			if(string.IsNullOrEmpty(text))
				return null;

			var part = string.Empty;
			var parts = new List<string>();
			var spaces = 0;

			for(int i = 0; i < text.Length; i++)
			{
				var chr = text[i];

				switch(chr)
				{
					case '.':
						if(parts.Count == 0)
						{
							switch(part)
							{
								case "":
								case ".":
									part += chr;
									break;
								case "..":
									onError?.Invoke($"Invalid path anchor in the \"{text}\" path expression.");
									return null;
								default:
									parts.Add(part);
									part = string.Empty;

									return new HierarchicalExpression(parts.ToArray(), Reflection.MemberAccess.Resolve(text.Substring(i + 1), message => onError?.Invoke(message)));
							}
						}
						else
						{
							if(part.Length == 0)
							{
								onError?.Invoke($"Invalid path expression:'{text}'.");
								return null;
							}

							parts.Add(part);
							part = string.Empty;

							return new HierarchicalExpression(parts.ToArray(), Reflection.MemberAccess.Resolve(text.Substring(i + 1), message => onError?.Invoke(message)));
						}

						spaces = 0;

						break;
					case '/':
					case '\\':
						if(part.Length > 0)
						{
							parts.Add(part);
							part = string.Empty;
						}
						else
						{
							if(parts.Count > 0)
							{
								onError?.Invoke($"Contains multiple path separators in the \"{text}\" path expression.");
								return null;
							}

							parts.Add(string.Empty);
						}

						spaces = 0;

						break;
					case '[':
					case '@':
						if(part.Length > 0)
						{
							parts.Add(part);
							part = string.Empty;
						}

						return new HierarchicalExpression(parts.ToArray(), Reflection.MemberAccess.Resolve(text.Substring(i + (chr == '[' ? 0 : 1)), message => onError?.Invoke(message)));
					case ' ':
						if(part.Length > 0)
							spaces++;

						break;
					default:
						if(IsIllegalPathChars(chr))
						{
							onError?.Invoke($"Contains '{chr}' illegal character in this \"{text}\" path expression.");
							return null;
						}

						part += (spaces > 0 ? new string(' ', spaces) : "") + chr;
						break;
				}
			}

			//将最后的项加入到列表中
			if(part.Length > 0)
				parts.Add(part);

			if(parts.Count == 0)
				return null;

			return new HierarchicalExpression(parts.ToArray(), null);
		}

		private static bool IsIllegalPathChars(char chr)
		{
			foreach(var c in System.IO.Path.GetInvalidPathChars())
			{
				if(chr == c)
					return true;
			}

			return false;
		}
		#endregion
	}
}

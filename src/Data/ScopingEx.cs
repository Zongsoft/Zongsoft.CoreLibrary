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
using System.Collections;
using System.Collections.Generic;

namespace Zongsoft.Data
{
	/// <summary>
	/// 提供数据范围操作的类。
	/// </summary>
	public class ScopingEx : IEnumerable<ScopingEx.Segment>
	{
		#region 成员字段
		private Collections.INamedCollection<Segment> _segments;
		#endregion

		#region 构造函数
		private ScopingEx(Collections.INamedCollection<Segment> segments)
		{
			_segments = segments ?? throw new ArgumentNullException(nameof(segments));
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取范围包含的元素数量。
		/// </summary>
		public int Count
		{
			get
			{
				return _segments.Count;
			}
		}
		#endregion

		#region 公共方法
		public void Clear()
		{
			_segments.Clear();
		}

		public ScopingEx Append(string scope)
		{
			return ParseCore(this, scope, message => throw new InvalidOperationException(message));
		}
		#endregion

		#region 静态方法
		public static bool TryParse(string text, out ScopingEx scoping)
		{
			return (scoping = ParseCore(null, text, null)) != null;
		}

		public static ScopingEx Parse(string text, Action<string> onError = null)
		{
			return ParseCore(null, text, onError);
		}

		private static ScopingEx ParseCore(ScopingEx scoping, string text, Action<string> onError)
		{
			if(string.IsNullOrEmpty(text))
				return null;

			var context = new StateContext(scoping?._segments, text.Length, onError);

			for(int i = 0; i < text.Length; i++)
			{
				context.Character = text[i];

				switch(context.State)
				{
					case State.None:
						if(!DoNone(ref context))
							return null;

						break;
					case State.Star:
						if(!DoStar(ref context))
							return null;

						break;
					case State.Exclude:
						if(!DoExclude(ref context))
							return null;

						break;
					case State.Include:
						if(!DoInclude(ref context))
							return null;

						break;
					case State.PagingCount:
						if(!DoPagingCount(ref context))
							return null;

						break;
					case State.PagingSize:
						if(!DoPagingSize(ref context))
							return null;

						break;
					case State.SortingField:
						if(!DoSortingField(ref context))
							return null;

						break;
					case State.SortingGutter:
						if(!DoSortingGutter(ref context))
							return null;

						break;
				}
			}

			if(context.Complete(out var segments))
				return scoping ?? new ScopingEx(segments);

			return null;
		}
		#endregion

		#region 重写方法
		public override string ToString()
		{
			if(_segments == null || _segments.Count == 0)
				return string.Empty;

			var index = 0;
			var text = new System.Text.StringBuilder();

			foreach(var segment in _segments)
			{
				if(index++ > 0)
					text.Append(", ");

				text.Append(segment.ToString());
			}

			return text.ToString();
		}
		#endregion

		#region 遍历枚举
		public IEnumerator<Segment> GetEnumerator()
		{
			return _segments.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _segments.GetEnumerator();
		}
		#endregion

		#region 状态处理
		private static bool DoNone(ref StateContext context)
		{
			if(context.IsWhitespace())
				return true;

			switch(context.Character)
			{
				case '*':
					context.Include("*");
					context.State = State.Star;
					return true;
				case '!':
					context.State = State.Exclude;
					return true;
				default:
					if(context.IsLetterOrUnderscore())
					{
						context.Accept();
						context.State = State.Include;
						return true;
					}

					context.OnError("");
					return false;
			}
		}

		private static bool DoStar(ref StateContext context)
		{
			switch(context.Character)
			{
				case ',':
					context.State = State.None;
					return true;
				case ')':
					context.Pop();
					context.State = State.None;
					return true;
				default:
					context.OnError("");
					return false;
			}
		}

		private static bool DoExclude(ref StateContext context)
		{
			if(context.Character == ',')
			{
				context.Exclude();
				context.State = State.None;
				return true;
			}

			if(context.Character == ')')
			{
				context.Exclude();
				context.Pop();
				context.State = State.None;
			}

			if(context.IsLetterOrDigitOrUnderscore())
			{
				//如果首字符是数字，则激发错误
				if(char.IsDigit(context.Character) && !context.HasBuffer())
				{
					context.OnError("");
					return false;
				}

				context.Accept();
				return true;
			}

			context.OnError("");
			return false;
		}

		private static bool DoInclude(ref StateContext context)
		{
			switch(context.Character)
			{
				case ',':
					context.Include();
					context.State = State.None;
					return true;
				case ':':
					context.Include();
					context.State = State.PagingCount;
					return true;
				case '[':
					context.Include();
					context.State = State.SortingField;
					return true;
				case '(':
					context.Include();
					context.Push();
					context.State = State.None;
					return true;
				case ')':
					context.Include();
					context.Pop();
					context.State = State.None;
					return true;
				default:
					if(context.IsLetterOrDigitOrUnderscore())
					{
						context.Accept();
						return true;
					}

					context.OnError("");
					return false;
			}
		}

		private static bool DoPagingCount(ref StateContext context)
		{
			switch(context.Character)
			{
				case '?':
					if(context.HasBuffer())
					{
						context.OnError("");
						return false;
					}

					context.Current.Paging = null;
					context.State = State.Include;
					return true;
				case '*':
					if(context.HasBuffer())
					{
						context.OnError("");
						return false;
					}

					context.Current.Paging = Paging.Disable;
					context.State = State.Include;
					return true;
				case '/':
					if(!context.TryGetBuffer(out var buffer))
					{
						context.OnError("");
						return false;
					}

					context.Current.Paging = Paging.Page(1, int.Parse(buffer));
					context.State = State.PagingSize;

					return true;
				case '(':
					if(!context.HasBuffer())
					{
						context.OnError("");
						return false;
					}

					context.Push();
					context.State = State.None;
					return true;
				default:
					if(char.IsDigit(context.Character))
					{
						context.Accept();
						return true;
					}

					context.OnError("");
					return false;
			}
		}

		private static bool DoPagingSize(ref StateContext context)
		{
			switch(context.Character)
			{
				case '?':
					if(context.HasBuffer())
					{
						context.OnError("");
						return false;
					}

					context.Current.Paging.PageSize = 20;
					context.State = State.Include;
					return true;
				case ',':
					if(!context.TryGetBuffer(out var buffer))
					{
						context.OnError("");
						return false;
					}

					context.Current.Paging.PageSize = int.Parse(buffer);
					context.State = State.None;
					return true;
				case '(':
					if(!context.HasBuffer())
					{
						context.OnError("");
						return false;
					}

					context.Push();
					context.State = State.None;
					return true;
				default:
					if(char.IsDigit(context.Character))
					{
						context.Accept();
						return true;
					}

					context.OnError("");
					return false;
			}
		}

		private static bool DoSortingField(ref StateContext context)
		{
			switch(context.Character)
			{
				case '~':
				case '!':
					if(context.HasBuffer())
					{
						context.OnError("");
						return false;
					}

					context.Accept();
					return true;
				case ',':
					if(!context.HasBuffer())
					{
						context.OnError("");
						return false;
					}

					if(context.AddSorting())
					{
						context.State = State.SortingGutter;
						return true;
					}

					return false;
				case ']':
					if(context.AddSorting())
					{
						context.ResetSorting();
						context.State = State.Include;
						return true;
					}

					return false;
				default:
					if(context.IsLetterOrDigitOrUnderscore())
					{
						context.Accept();
						return true;
					}

					context.OnError("");
					return false;
			}
		}

		private static bool DoSortingGutter(ref StateContext context)
		{
			if(context.IsWhitespace())
				return true;

			if(context.IsLetterOrUnderscore())
			{
				context.Accept();
				context.State = State.SortingField;
				return true;
			}

			context.OnError("");
			return false;
		}
		#endregion

		#region 嵌套子类
		public class Segment : IEquatable<Segment>
		{
			#region 静态字段
			internal static readonly Segment All = new Segment("*");
			#endregion

			#region 成员字段
			private Collections.INamedCollection<Segment> _children;
			#endregion

			public readonly string Name;
			public readonly Segment Parent;
			public Paging Paging;
			public Sorting[] Sortings;

			#region 构造函数
			public Segment(string name, bool isExclude = false)
			{
				if(string.IsNullOrEmpty(name))
					throw new ArgumentNullException(nameof(name));

				this.Name = name;
				this.IsExclude = isExclude;
			}
			#endregion

			#region 公共属性
			public bool IsExclude
			{
				get;
			}

			public bool HasChildren
			{
				get
				{
					return _children != null && _children.Count > 0;
				}
			}

			public Collections.INamedCollection<Segment> Children
			{
				get
				{
					if(_children == null)
						_children = new Collections.NamedCollection<Segment>(child => child.Name);

					return _children;
				}
			}
			#endregion

			#region 重写方法
			public bool Equals(Segment other)
			{
				if(other == null)
					return false;

				return string.Equals(this.Name, other.Name, StringComparison.OrdinalIgnoreCase);
			}

			public override bool Equals(object obj)
			{
				if(obj == null || obj.GetType() != typeof(Segment))
					return false;

				return this.Equals((Segment)obj);
			}

			public override int GetHashCode()
			{
				return this.Name.GetHashCode();
			}

			public override string ToString()
			{
				var text = this.Name;

				if(this.Paging != null)
				{
					if(Paging.IsDisabled(this.Paging))
						text += ":*";
					else
						text += this.Paging.PageIndex == 1 ?
						        this.Paging.PageSize.ToString() :
						        this.Paging.PageIndex.ToString() + "/" + this.Paging.PageSize.ToString();
				}

				if(_children != null && _children.Count > 0)
				{
					text += "(";
					int index = 0;

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
		}

		private struct StateContext
		{
			#region 私有变量
			private int _bufferIndex;
			private readonly char[] _buffer;
			private readonly Action<string> _onError;
			private Segment _current;
			private Stack<Segment> _stack;
			private Collections.INamedCollection<Segment> _segments;
			private ISet<Sorting> _sortings;
			#endregion

			#region 公共字段
			public State State;
			public char Character;
			#endregion

			#region 构造函数
			public StateContext(Collections.INamedCollection<Segment> segments, int length, Action<string> onError)
			{
				_bufferIndex = 0;
				_buffer = new char[length];
				_current = null;
				_sortings = null;
				_onError = onError;
				_stack = new Stack<Segment>();

				this.Character = '\0';
				this.State = State.None;

				_segments = segments ?? new Collections.NamedCollection<Segment>(item => item.Name, StringComparer.OrdinalIgnoreCase);
			}
			#endregion

			#region 公共属性
			public Segment Current
			{
				get
				{
					return _current;
				}
			}
			#endregion

			#region 公共方法
			public void Accept()
			{
				_buffer[_bufferIndex++] = Character;
			}

			public void OnError(string message)
			{
				_onError?.Invoke(message);
			}

			public Segment Peek()
			{
				return _stack.Count > 0 ? _stack.Peek() : null;
			}

			public Segment Pop()
			{
				if(_stack == null || _stack.Count == 0)
				{
					_onError?.Invoke("");
					return null;
				}

				return _stack.Pop();
			}

			public void Push()
			{
				_stack.Push(_current);
			}

			public bool IsWhitespace()
			{
				return char.IsWhiteSpace(Character);
			}

			public bool IsLetterOrUnderscore()
			{
				return (Character >= 'a' && Character <= 'z') ||
					   (Character >= 'A' && Character <= 'Z') || Character == '_';
			}

			public bool IsLetterOrDigitOrUnderscore()
			{
				return (Character >= 'a' && Character <= 'z') ||
					   (Character >= 'A' && Character <= 'Z') ||
					   (Character >= '0' && Character <= '9') || Character == '_';
			}

			public void Exclude(string name = null)
			{
				var parent = this.Peek();

				if(string.IsNullOrEmpty(name))
					name = this.GetBuffer();

				if(string.IsNullOrEmpty(name) || name == "!" || name == "*")
				{
					if(parent == null)
						_segments.Clear();
					else if(parent.HasChildren)
						parent.Children.Clear();
				}
				else
				{
					if(parent == null)
					{
						_segments.Remove(name);
						_segments.Add(new Segment(name, true));
					}
					else
					{
						if(parent.HasChildren)
							parent.Children.Remove(name);

						parent.Children.Add(new Segment(name, true));
					}
				}

				_current = null;
			}

			public void Include(string name = null)
			{
				var parent = this.Peek();
				Segment current;
				Func<string, Segment> creator = identifier => identifier == "*" ? Segment.All : new Segment(identifier);

				if(string.IsNullOrEmpty(name))
				{
					if(_bufferIndex == 0)
						return;

					name = this.GetBuffer();
				}

				if(parent == null)
				{
					if(_segments.TryGet(name, out current))
						_current = current;
					else
						_segments.Add((_current = creator(name)));
				}
				else
				{
					if(parent.Children.TryGet(name, out current))
						_current = current;
					else
						parent.Children.Add((_current = creator(name)));
				}
			}

			public bool AddSorting()
			{
				if(_bufferIndex == 0)
				{
					_onError?.Invoke("");
					return false;
				}

				if(_bufferIndex == 1)
				{
					if(_buffer[0] == '~' || _buffer[0] == '!')
					{
						_onError?.Invoke("");
						return false;
					}
					else if(char.IsDigit(_buffer[0]))
					{
						_onError?.Invoke("");
						return false;
					}
				}

				if(_sortings == null)
					_sortings = new HashSet<Sorting>(SortingComparer.Instance);

				var sorting = _buffer[0] == '~' || _buffer[0] == '!' ?
				              Sorting.Descending(new string(_buffer, 1, _bufferIndex - 1)) :
				              Sorting.Ascending(new string(_buffer, 0, _bufferIndex));

				_sortings.Remove(sorting);
				_sortings.Add(sorting);

				return true;
			}

			public void ResetSorting()
			{
				_sortings?.Clear();
			}

			public bool HasBuffer()
			{
				return _bufferIndex > 0;
			}

			public string GetBuffer()
			{
				if(_bufferIndex == 0)
					return string.Empty;

				var content = new string(_buffer, 0, _bufferIndex);
				_bufferIndex = 0;
				return content;
			}

			public bool TryGetBuffer(out string buffer)
			{
				buffer = null;

				if(_bufferIndex == 0)
					return false;

				buffer = new string(_buffer, 0, _bufferIndex);
				_bufferIndex = 0;
				return true;
			}

			public bool Complete(out Collections.INamedCollection<Segment> segments)
			{
				segments = null;

				if(_stack != null && _stack.Count > 0)
				{
					_onError?.Invoke("");
					return false;
				}

				switch(State)
				{
					case State.Star:
						_segments.Add(Segment.All);
						break;
					case State.Exclude:
						this.Exclude();
						break;
					case State.Include:
						this.Include();
						break;
					case State.PagingCount:
						if(_bufferIndex == 0)
						{
							_onError?.Invoke("");
							return false;
						}

						_current.Paging = Paging.Page(1, int.Parse(new string(_buffer, 0, _bufferIndex)));
						break;
					case State.PagingSize:
						if(_bufferIndex == 0)
						{
							_onError?.Invoke("");
							return false;
						}

						_current.Paging.PageSize = int.Parse(new string(_buffer, 0, _bufferIndex));
						break;
					default:
						_onError?.Invoke("");
						return false;
				}

				segments = _segments;
				return true;
			}
			#endregion

			#region 嵌套子类
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

		private enum State
		{
			None,
			Star,
			Include,
			Exclude,
			PagingCount,
			PagingSize,
			SortingField,
			SortingGutter,
		}
		#endregion
	}
}

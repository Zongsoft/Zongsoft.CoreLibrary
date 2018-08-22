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
	public static class Scope
	{
		#region 公共方法
		public static bool TryResolve(string text, out Collections.IReadOnlyNamedCollection<Segment> result, Func<SegmentToken, IEnumerable<Segment>> mapper)
		{
			return (result = Resolve(text, mapper, null)) != null;
		}

		public static Collections.IReadOnlyNamedCollection<Segment> Resolve(string text, Func<SegmentToken, IEnumerable<Segment>> mapper)
		{
			return Resolve(text, mapper, message => throw new InvalidOperationException(message));
		}

		public static Collections.IReadOnlyNamedCollection<Segment> Resolve(string text, Func<SegmentToken, IEnumerable<Segment>> mapper, Action<string> onError)
		{
			if(string.IsNullOrEmpty(text))
				return null;

			var context = new StateContext(text.Length, mapper, onError);

			for(int i = 0; i < text.Length; i++)
			{
				context.Character = text[i];

				switch(context.State)
				{
					case State.None:
						if(!DoNone(ref context))
							return null;

						break;
					case State.Asterisk:
						if(!DoAsterisk(ref context))
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
				return (Collections.IReadOnlyNamedCollection<Segment>)segments;

			return null;
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
					//context.Accept();
					context.State = State.Asterisk;
					return true;
				case '!':
					context.State = State.Exclude;
					return true;
				case ')':
					return context.Pop() != null;
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

		private static bool DoAsterisk(ref StateContext context)
		{
			switch(context.Character)
			{
				case ',':
					context.Include("*");
					context.State = State.None;
					return true;
				case ')':
					context.Include("*");
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
			switch(context.Character)
			{
				case ',':
					context.Exclude();
					context.State = State.None;
					break;
				case ')':
					context.Exclude();
					context.Pop();
					context.State = State.None;
					break;
				default:
					if(context.IsLetterOrDigitOrUnderscore())
					{
						//如果首字符是数字，则激发错误
						if(char.IsDigit(context.Character) && !context.HasBuffer())
						{
							context.OnError("");
							return false;
						}

						//判断标识中间是否含有空白字符
						if(context.Flags.HasWhitespace())
						{
							context.OnError("");
							return false;
						}

						context.Accept();
					}
					else if(!context.IsWhitespace())
					{
						context.OnError("");
						return false;
					}

					break;
			}

			//重置是否含有空格的标记
			context.Flags.HasWhitespace(context.IsWhitespace());

			return true;
		}

		private static bool DoInclude(ref StateContext context)
		{
			switch(context.Character)
			{
				case ',':
					context.Include();
					context.State = State.None;
					break;
				case ':':
					context.Include();
					context.State = State.PagingCount;
					break;
				case '[':
					context.Include();
					context.State = State.SortingField;
					break;
				case '(':
					context.Include();
					context.Push();
					context.State = State.None;
					break;
				case ')':
					context.Include();
					context.Pop();
					context.State = State.None;
					break;
				default:
					if(context.IsLetterOrDigitOrUnderscore())
					{
						//判断标识中间是否含有空白字符
						if(context.Flags.HasWhitespace())
						{
							context.OnError("");
							return false;
						}

						context.Accept();
					}
					else if(!context.IsWhitespace())
					{
						context.OnError("");
						return false;
					}

					break;
			}

			//重置是否含有空格的标记
			context.Flags.HasWhitespace(context.IsWhitespace());

			return true;
		}

		private static bool DoPagingCount(ref StateContext context)
		{
			string buffer;

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
					if(!context.TryGetBuffer(out buffer))
					{
						context.OnError("");
						return false;
					}

					context.Current.Paging = Paging.Page(int.Parse(buffer));
					context.State = State.PagingSize;

					return true;
				case '[':
					if(!context.TryGetBuffer(out buffer))
					{
						context.OnError("");
						return false;
					}

					context.Current.Paging = Paging.Page(1, int.Parse(buffer));
					context.State = State.SortingField;
					return true;
				case '(':
					if(!context.TryGetBuffer(out buffer))
					{
						context.OnError("");
						return false;
					}

					context.Current.Paging = Paging.Page(1, int.Parse(buffer));
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
			string buffer;

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
					if(!context.TryGetBuffer(out buffer))
					{
						context.OnError("");
						return false;
					}

					context.Current.Paging.PageSize = int.Parse(buffer);
					context.State = State.None;
					return true;
				case '[':
					if(!context.TryGetBuffer(out buffer))
					{
						context.OnError("");
						return false;
					}

					if(buffer != "?")
						context.Current.Paging.PageSize = int.Parse(buffer);

					context.State = State.SortingField;
					return true;
				case '(':
					if(!context.TryGetBuffer(out buffer))
					{
						context.OnError("");
						return false;
					}

					if(buffer != "?")
						context.Current.Paging.PageSize = int.Parse(buffer);

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
					if(context.AddSorting())
					{
						context.State = State.SortingGutter;
						return true;
					}

					return false;
				case ']':
					if(context.AddSorting())
					{
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
			#region 单例字段
			internal static readonly Segment Ignore = new Segment("?");
			#endregion

			#region 成员字段
			private Segment _parent;
			private Sorting[] _sortingArray;
			private HashSet<Sorting> _sortings;
			private Collections.NamedCollection<Segment> _children;
			#endregion

			#region 构造函数
			public Segment(string name)
			{
				if(string.IsNullOrEmpty(name))
					throw new ArgumentNullException(nameof(name));

				this.Name = name;
			}
			#endregion

			#region 公共属性
			public string Name
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

			public Segment Parent
			{
				get
				{
					return _parent;
				}
			}

			public bool HasChildren
			{
				get
				{
					return _children != null && _children.Count > 0;
				}
			}

			public Collections.IReadOnlyNamedCollection<Segment> Children
			{
				get
				{
					return _children;
				}
			}

			public Segment this[string name]
			{
				get
				{
					if(_children != null && _children.TryGet(name, out var child))
						return child;

					return null;
				}
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

			internal void AddChild(Segment child)
			{
				if(_children == null)
					System.Threading.Interlocked.CompareExchange(ref _children, new Collections.NamedCollection<Segment>(segment => segment.Name), null);

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
		}

		public struct SegmentToken
		{
			public readonly string Name;
			public readonly Segment Parent;

			internal SegmentToken(string name, Segment parent)
			{
				this.Name = name;
				this.Parent = parent;
			}
		}

		private struct StateContext
		{
			#region 私有变量
			private int _bufferIndex;
			private readonly char[] _buffer;
			private readonly Action<string> _onError;
			private readonly Func<SegmentToken, IEnumerable<Segment>> _mapper;
			private Segment _current;
			private State _state;
			private Stack<Segment> _stack;
			private Collections.INamedCollection<Segment> _segments;
			#endregion

			#region 公共字段
			public State State;
			public char Character;
			public StateVector Flags;
			#endregion

			#region 构造函数
			public StateContext(int length, Func<SegmentToken, IEnumerable<Segment>> mapper, Action<string> onError)
			{
				_bufferIndex = 0;
				_buffer = new char[length];
				_state = State.None;
				_current = null;
				_mapper = mapper;
				_onError = onError;
				_stack = new Stack<Segment>();

				this.Character = '\0';
				this.State = State.None;
				this.Flags = new StateVector();

				_segments = new Collections.NamedCollection<Segment>(item => item.Name, StringComparer.OrdinalIgnoreCase);
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
				_stack.Push(_current ?? Segment.Ignore);
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
						parent.ClearChildren();
				}
				else
				{
					if(parent == null)
						_segments.Remove(name);
					else if(parent.HasChildren)
						parent.RemoveChild(name);
				}

				_current = null;
			}

			public void Include(string name = null)
			{
				var parent = this.Peek();
				Segment current;

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
						this.Map(new SegmentToken(name, null));
				}
				else
				{
					//如果是忽略段则不需要进行子集和映射处理
					if(object.ReferenceEquals(parent, Segment.Ignore))
						return;

					if(parent.HasChildren && parent.Children.TryGet(name, out current))
						_current = current;
					else
						this.Map(new SegmentToken(name, parent));
				}
			}

			public bool AddSorting()
			{
				if(_current == null)
					return true;

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

				var sorting = _buffer[0] == '~' || _buffer[0] == '!' ?
							  Sorting.Descending(new string(_buffer, 1, _bufferIndex - 1)) :
							  Sorting.Ascending(new string(_buffer, 0, _bufferIndex));

				_bufferIndex = 0;

				_current.AddSorting(sorting);
				return true;
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
					case State.None:
						if(_segments != null && _segments.Count > 0)
							segments = _segments;

						return true;
					case State.Asterisk:
						this.Include("*");
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

						var buffer = new string(_buffer, 0, _bufferIndex);

						if(buffer != "?")
							_current.Paging.PageSize = int.Parse(buffer);

						break;
					default:
						_onError?.Invoke("");
						return false;
				}

				segments = _segments;
				return true;
			}
			#endregion

			#region 私有方法
			private void Map(SegmentToken token)
			{
				//重置当前段
				_current = null;

				var segments = _mapper(token);

				if(segments == null)
					return;

				if(token.Parent == null)
				{
					foreach(var segment in segments)
					{
						if(_segments.Contains(segment.Name))
							_current = segment;
						else
							_segments.Add(_current = segment);
					}
				}
				else
				{
					foreach(var segment in segments)
					{
						if(token.Parent.HasChildren && token.Parent.Children.Contains(segment.Name))
							_current = segment;
						else
							token.Parent.AddChild(_current = segment);
					}
				}
			}
			#endregion
		}

		private struct StateVector
		{
			#region 常量定义
			private const int IDENTIFIER_WHITESPACE_FLAG = 1; //标识中间是否出现空白字符(0:没有, 1:有)
			#endregion

			#region 成员变量
			private int _data;
			#endregion

			#region 公共方法
			public bool HasWhitespace()
			{
				return this.IsMarked(IDENTIFIER_WHITESPACE_FLAG);
			}

			public void HasWhitespace(bool enabled)
			{
				this.Mark(IDENTIFIER_WHITESPACE_FLAG, enabled);
			}
			#endregion

			#region 私有方法
			[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
			private bool IsMarked(int bit)
			{
				return (_data & bit) == bit;
			}

			[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
			private void Mark(int bit, bool value)
			{
				if(value)
					_data |= bit;
				else
					_data &= ~bit;
			}
			#endregion
		}

		private enum State
		{
			None,
			Asterisk,
			Include,
			Exclude,
			PagingCount,
			PagingSize,
			SortingField,
			SortingGutter,
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

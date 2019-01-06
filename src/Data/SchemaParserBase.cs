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
	public abstract class SchemaParserBase<TEntry> : ISchemaParser, ISchemaParser<TEntry> where TEntry : SchemaEntryBase
	{
		#region 构造函数
		protected SchemaParserBase()
		{
		}
		#endregion

		#region 抽象方法
		public abstract ISchema<TEntry> Parse(string name, string expression, Type entityType);
		#endregion

		#region 保护方法
		protected bool TryParse(string expression, out Collections.INamedCollection<TEntry> result, Func<SchemaEntryToken, IEnumerable<TEntry>> mapper, object data, Collections.INamedCollection<TEntry> entries = null)
		{
			return (result = this.Parse(expression, mapper, null, data, entries)) != null;
		}

		protected Collections.INamedCollection<TEntry> Parse(string expression, Func<SchemaEntryToken, IEnumerable<TEntry>> mapper, object data, Collections.INamedCollection<TEntry> entries = null)
		{
			return this.Parse(expression, mapper, message => throw new InvalidOperationException(message), data, entries);
		}

		private Collections.INamedCollection<TEntry> Parse(string expression, Func<SchemaEntryToken, IEnumerable<TEntry>> mapper, Action<string> onError, object data, Collections.INamedCollection<TEntry> entries = null)
		{
			if(string.IsNullOrEmpty(expression))
				return null;

			var context = new StateContext(expression.Length, mapper, onError, data, entries);

			for(int i = 0; i < expression.Length; i++)
			{
				context.Character = expression[i];

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
				return segments;

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
				case '}':
					return context.Pop() != null;
				case ',':
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

		private static bool DoAsterisk(ref StateContext context)
		{
			switch(context.Character)
			{
				case ',':
					context.Include("*");
					context.State = State.None;
					return true;
				case '}':
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
				case '}':
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
				case '(':
					context.Include();
					context.State = State.SortingField;
					break;
				case '{':
					context.Include();
					context.Push();
					context.State = State.None;
					break;
				case '}':
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
				case '(':
					if(!context.TryGetBuffer(out buffer))
					{
						context.OnError("");
						return false;
					}

					context.Current.Paging = Paging.Page(1, int.Parse(buffer));
					context.State = State.SortingField;
					return true;
				case '{':
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
				case '(':
					if(!context.TryGetBuffer(out buffer))
					{
						context.OnError("");
						return false;
					}

					if(buffer != "?")
						context.Current.Paging.PageSize = int.Parse(buffer);

					context.State = State.SortingField;
					return true;
				case '{':
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
				case ')':
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

		#region 显式实现
		ISchema ISchemaParser.Parse(string name, string expression, Type entityType)
		{
			return this.Parse(name, expression, entityType);
		}
		#endregion

		#region 嵌套子类
		/// <summary>
		/// 表示数据模式解析中的元素描述类。
		/// </summary>
		protected class SchemaEntryToken
		{
			#region 构造函数
			internal SchemaEntryToken(object data)
			{
				this.Data = data;
			}
			#endregion

			#region 公共属性
			/// <summary>
			/// 获取当前元素的名称。
			/// </summary>
			public string Name
			{
				get;
				internal set;
			}

			/// <summary>
			/// 获取当前元素的父元素。
			/// </summary>
			public TEntry Parent
			{
				get;
				internal set;
			}

			/// <summary>
			/// 获取或设置用户自定义数据。
			/// </summary>
			/// <remarks>
			///		<para>设置的用户自定义数据，会传给下一个元素解析描述。</para>
			/// </remarks>
			public object Data
			{
				get;
				set;
			}
			#endregion
		}

		private struct StateContext
		{
			#region 私有变量
			private int _bufferIndex;
			private readonly char[] _buffer;
			private readonly Action<string> _onError;
			private readonly Func<SchemaEntryToken, IEnumerable<TEntry>> _mapper;
			private readonly SchemaEntryToken _token;
			private SchemaEntryBase _current;
			private Stack<SchemaEntryBase> _stack;
			private Collections.INamedCollection<TEntry> _entries;
			#endregion

			#region 公共字段
			public State State;
			public char Character;
			public StateVector Flags;
			#endregion

			#region 构造函数
			public StateContext(int length, Func<SchemaEntryToken, IEnumerable<TEntry>> mapper, Action<string> onError, object data, Collections.INamedCollection<TEntry> entries)
			{
				_bufferIndex = 0;
				_buffer = new char[length];
				_current = null;
				_mapper = mapper;
				_onError = onError;
				_token = new SchemaEntryToken(data);
				_stack = new Stack<SchemaEntryBase>();

				this.Character = '\0';
				this.State = State.None;
				this.Flags = new StateVector();

				_entries = entries ?? new Collections.NamedCollection<TEntry>(item => item.Name, StringComparer.OrdinalIgnoreCase);
			}
			#endregion

			#region 公共属性
			public SchemaEntryBase Current
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

			public SchemaEntryBase Peek()
			{
				return _stack.Count > 0 ? _stack.Peek() : null;
			}

			public SchemaEntryBase Pop()
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
				_stack.Push(_current ?? SchemaEntryBase.Ignores);
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
						_entries.Clear();
					else if(parent.HasChildren)
						parent.ClearChildren();
				}
				else
				{
					if(parent == null)
						_entries.Remove(name);
					else if(parent.HasChildren)
						parent.RemoveChild(name);
				}

				_current = null;
			}

			public void Include(string name = null)
			{
				var parent = this.Peek();
				TEntry current;

				if(string.IsNullOrEmpty(name))
				{
					if(_bufferIndex == 0)
						return;

					name = this.GetBuffer();
				}

				if(parent == null)
				{
					if(_entries.TryGet(name, out current))
						_current = current;
					else
						this.Map(name, null);
				}
				else
				{
					//如果是忽略段则不需要进行子集和映射处理
					if(object.ReferenceEquals(parent, SchemaEntryBase.Ignores))
						return;

					if(parent.TryGetChild(name, out var child))
						_current = child;
					else
						this.Map(name, parent);
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

			public bool Complete(out Collections.INamedCollection<TEntry> entries)
			{
				entries = null;

				if(_stack != null && _stack.Count > 0)
				{
					_onError?.Invoke("");
					return false;
				}

				switch(State)
				{
					case State.None:
						if(_entries != null && _entries.Count > 0)
							entries = _entries;

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

				entries = _entries;
				return true;
			}
			#endregion

			#region 私有方法
			private void Map(string name, SchemaEntryBase parent)
			{
				//重置当前段
				_current = null;

				_token.Name = name;
				_token.Parent = (TEntry)parent;

				var items = _mapper(_token);

				if(items == null)
					return;

				if(_token.Parent == null)
				{
					foreach(var item in items)
					{
						if(_entries.Contains(item.Name))
							_current = item;
						else
							_entries.Add((TEntry)(_current = item));
					}
				}
				else
				{
					foreach(var item in items)
					{
						if(_token.Parent.TryGetChild(item.Name, out _))
							_current = item;
						else
							_token.Parent.AddChild(_current = item);
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
		#endregion
	}
}

/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2018 Zongsoft Corporation <http://www.zongsoft.com>
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

namespace Zongsoft.Services
{
	/// <summary>
	/// 表示命令输出内容链的类。
	/// </summary>
	public class CommandOutletContent
	{
		#region 成员字段
		private string _text;
		private CommandOutletColor? _color;
		private CommandOutletContent _next;
		private CommandOutletContent _previous;
		#endregion

		#region 私有构造
		private CommandOutletContent(CommandOutletContent previous, string text, CommandOutletColor? color = null)
		{
			_text = text;
			_color = color;
			_previous = previous;
			_next = null;
		}

		private CommandOutletContent(CommandOutletContent previous, string text, CommandOutletContent next, CommandOutletColor? color = null)
		{
			_text = text;
			_color = color;
			_previous = previous;
			_next = next;
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取或设置内容段文本。
		/// </summary>
		public string Text
		{
			get
			{
				return _text;
			}
			set
			{
				_text = value;
			}
		}

		/// <summary>
		/// 获取或设置内容段的文本颜色。
		/// </summary>
		public CommandOutletColor? Color
		{
			get
			{
				return _color;
			}
			set
			{
				_color = value;
			}
		}

		/// <summary>
		/// 获取当前内容链的首段。
		/// </summary>
		public CommandOutletContent First
		{
			get
			{
				var current = this;

				while(current._previous != null)
				{
					current = current._previous;
				}

				return current;
			}
		}

		/// <summary>
		/// 获取当前内容链的末段。
		/// </summary>
		public CommandOutletContent Last
		{
			get
			{
				var current = this;

				while(current._next != null)
				{
					current = current._next;
				}

				return current;
			}
		}

		/// <summary>
		/// 获取当前内容链的下一段。
		/// </summary>
		public CommandOutletContent Next
		{
			get
			{
				return _next;
			}
		}

		/// <summary>
		/// 获取当前内容链的上一段。
		/// </summary>
		public CommandOutletContent Previous
		{
			get
			{
				return _previous;
			}
		}
		#endregion

		#region 公共方法
		/// <summary>
		/// 追加一个空行段。
		/// </summary>
		/// <returns>返回当前内容段。</returns>
		public CommandOutletContent AppendLine()
		{
			//获取当前内容链的最末尾段
			var last = this.Last;

			//将最末尾段的文本内容追加一个换行符
			last._text += Environment.NewLine;

			//返回当前内容段
			return this;
		}

		/// <summary>
		/// 追加一个指定文本的内容段。
		/// </summary>
		/// <param name="text">指定的内容文本。</param>
		/// <returns>返回当前内容段。</returns>
		public CommandOutletContent AppendLine(string text)
		{
			if(string.IsNullOrEmpty(text))
				return this.AppendLine();
			else
				return this.Append(text + Environment.NewLine);
		}

		/// <summary>
		/// 追加一个指定颜色和文本的内容段。
		/// </summary>
		/// <param name="color">指定的内容文本颜色。</param>
		/// <param name="text">指定的内容文本。</param>
		/// <returns>返回当前内容段。</returns>
		public CommandOutletContent AppendLine(CommandOutletColor color, string text)
		{
			if(string.IsNullOrEmpty(text))
				return this.AppendLine();
			else
				return this.Append(color, text + Environment.NewLine);
		}

		/// <summary>
		/// 追加一个指定文本的内容段。
		/// </summary>
		/// <param name="text">指定的内容文本。</param>
		/// <returns>返回当前内容段。</returns>
		public CommandOutletContent Append(string text)
		{
			if(string.IsNullOrEmpty(text))
				return this;

			//获取当前内容链的最末尾段
			var last = this.Last;

			//在最末段追加一个片段
			last.AfterCore(null, text);

			//返回当前内容段
			return this;
		}

		/// <summary>
		/// 追加一个指定颜色和文本的内容段。
		/// </summary>
		/// <param name="color">指定的内容文本颜色。</param>
		/// <param name="text">指定的内容文本。</param>
		/// <returns>返回当前内容段。</returns>
		public CommandOutletContent Append(CommandOutletColor color, string text)
		{
			if(string.IsNullOrEmpty(text))
				return this;

			//获取当前内容链的最末尾段
			var last = this.Last;

			//在最末段追加一个片段
			last.AfterCore(color, text);

			//返回当前内容段
			return this;
		}

		/// <summary>
		/// 追加一个指定的内容段。
		/// </summary>
		/// <param name="content">指定的内容段。</param>
		/// <returns>返回当前内容段。</returns>
		public CommandOutletContent Append(CommandOutletContent content)
		{
			if(content == null)
				return this;

			//获取当前内容链的最末尾段
			var last = this.Last;

			//在最末段追加一个片段
			last.AfterCore(content);

			//返回当前内容段
			return this;
		}

		/// <summary>
		/// 新增一个指定文本的内容段，并作为当前内容链的首部返回。
		/// </summary>
		/// <param name="text">指定的内容文本。</param>
		/// <returns>返回新增的首部内容段。</returns>
		public CommandOutletContent Prepend(string text)
		{
			if(string.IsNullOrEmpty(text))
				return this;

			//获取当前内容链的首段
			var first = this.First;

			//在首段插入一个片段，并返回该新的首段
			return first.BeforeCore(null, text);
		}

		/// <summary>
		/// 新增一个指定颜色和文本的内容段，并作为当前内容链的首部返回。
		/// </summary>
		/// <param name="color">指定的内容文本颜色。</param>
		/// <param name="text">指定的内容文本。</param>
		/// <returns>返回新增的首部内容段。</returns>
		public CommandOutletContent Prepend(CommandOutletColor color, string text)
		{
			if(string.IsNullOrEmpty(text))
				return this;

			//获取当前内容链的首段
			var first = this.First;

			//在首段插入一个片段，并返回该新的首段
			return first.BeforeCore(color, text);
		}

		/// <summary>
		/// 将指定内容段置为当前内容链的首部。
		/// </summary>
		/// <param name="content">指定的内容段。</param>
		/// <returns>返回当前内容链的新首部。</returns>
		public CommandOutletContent Prepend(CommandOutletContent content)
		{
			if(content == null)
				return this;

			//获取当前内容链的首段
			var first = this.First;

			//在首段插入一个片段，并返回该新的首段
			return first.BeforeCore(content);
		}

		/// <summary>
		/// 在当前内容段后面添加一个指定文本的内容段。
		/// </summary>
		/// <param name="text">指定的内容文本。</param>
		/// <returns>返回新添加的内容段。</returns>
		public CommandOutletContent After(string text)
		{
			return this.AfterCore(null, text);
		}

		/// <summary>
		/// 在当前内容段后面添加一个指定颜色和文本的内容段。
		/// </summary>
		/// <param name="color">指定的内容文本颜色。</param>
		/// <param name="text">指定的内容文本。</param>
		/// <returns>返回新添加的内容段。</returns>
		public CommandOutletContent After(CommandOutletColor color, string text)
		{
			return this.AfterCore(color, text);
		}

		/// <summary>
		/// 将指定的内容段添加到当前内容段后面。
		/// </summary>
		/// <param name="content">指定的内容段。</param>
		/// <returns>返回新添加的内容段。</returns>
		public CommandOutletContent After(CommandOutletContent content)
		{
			return this.AfterCore(content);
		}

		/// <summary>
		/// 在当前内容段前面插入一个指定文本的内容段。
		/// </summary>
		/// <param name="text">指定的内容文本。</param>
		/// <returns>返回新插入的内容段。</returns>
		public CommandOutletContent Before(string text)
		{
			return this.BeforeCore(null, text);
		}

		/// <summary>
		/// 在当前内容段前面插入一个指定颜色和文本的内容段。
		/// </summary>
		/// <param name="color">指定的内容文本颜色。</param>
		/// <param name="text">指定的内容文本。</param>
		/// <returns>返回新插入的内容段。</returns>
		public CommandOutletContent Before(CommandOutletColor color, string text)
		{
			return this.BeforeCore(color, text);
		}

		/// <summary>
		/// 将指定的内容段插入到当前内容段前面。
		/// </summary>
		/// <param name="content">指定的内容段。</param>
		/// <returns>返回新插入的内容段。</returns>
		public CommandOutletContent Before(CommandOutletContent content)
		{
			return this.BeforeCore(content);
		}
		#endregion

		#region 私有方法
		private CommandOutletContent AfterCore(CommandOutletContent content)
		{
			if(content == null)
				return this;

			if(_next == null)
			{
				content._previous = this;
				return _next = content;
			}
			else
			{
				content._previous = this;
				content._next = _next;

				return _next = _next._previous = content;
			}
		}

		private CommandOutletContent AfterCore(CommandOutletColor? color, string text)
		{
			if(string.IsNullOrEmpty(text))
				return this;

			if(_next == null)
				return _next = new CommandOutletContent(this, text, color);
			else
				return _next = _next._previous = new CommandOutletContent(this, text, _next, color);
		}

		private CommandOutletContent BeforeCore(CommandOutletContent content)
		{
			if(content == null)
				return this;

			if(_previous == null)
			{
				content._next = this;
				return _previous = content;
			}
			else
			{
				content._next = this;
				content._previous = _previous;

				return _previous._next = content;
			}
		}

		private CommandOutletContent BeforeCore(CommandOutletColor? color, string text)
		{
			if(string.IsNullOrEmpty(text))
				return this;

			if(_previous == null)
				return _previous = new CommandOutletContent(null, text, this, color);
			else
				return _previous._next = new CommandOutletContent(_previous, text, this, color);
		}
		#endregion

		#region 静态方法
		/// <summary>
		/// 创建一个指定文本的新内容段。
		/// </summary>
		/// <param name="text">指定的内容文本。</param>
		/// <returns>返回新创建的内容段。</returns>
		public static CommandOutletContent Create(string text)
		{
			return new CommandOutletContent(null, text);
		}

		/// <summary>
		/// 创建一个指定颜色和文本的新内容段。
		/// </summary>
		/// <param name="color">指定的内容文本颜色。</param>
		/// <param name="text">指定的内容文本。</param>
		/// <returns>返回新创建的内容段。</returns>
		public static CommandOutletContent Create(CommandOutletColor color, string text)
		{
			return new CommandOutletContent(null, text, color);
		}
		#endregion
	}
}

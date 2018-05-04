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
	public class CommandOutletContent
	{
		#region 成员字段
		private CommandOutletContent _next;
		private CommandOutletContent _previous;
		#endregion

		#region 构造函数
		private CommandOutletContent(CommandOutletContent previous, string text, CommandOutletColor? color = null)
		{
			this.Text = text;
			this.Color = color;
			_previous = previous;
		}

		private CommandOutletContent(CommandOutletContent previous, string text, CommandOutletContent next, CommandOutletColor? color = null)
		{
			this.Text = text;
			this.Color = color;
			_previous = previous;
			_next = next;
		}
		#endregion

		#region 公共属性
		public string Text
		{
			get;
		}

		public CommandOutletColor? Color
		{
			get;
			set;
		}

		public CommandOutletContent Next
		{
			get
			{
				return _next ?? this;
			}
		}

		public CommandOutletContent Previous
		{
			get
			{
				return _previous ?? this;
			}
		}
		#endregion

		#region 公共方法
		public CommandOutletContent Append(string text)
		{
			if(string.IsNullOrEmpty(text))
				return this;

			return _next = new CommandOutletContent(this, text);
		}

		public CommandOutletContent Append(CommandOutletColor color, string text)
		{
			if(string.IsNullOrEmpty(text))
				return this;

			return _next = new CommandOutletContent(this, text, color);
		}

		public CommandOutletContent Prepend(string text)
		{
			if(string.IsNullOrEmpty(text))
				return this;

			if(_previous != null)
				return _previous._next = new CommandOutletContent(_previous, text, this);
			else
				return new CommandOutletContent(null, text, this);
		}

		public CommandOutletContent Prepend(CommandOutletColor color, string text)
		{
			if(string.IsNullOrEmpty(text))
				return this;

			if(_previous != null)
				return _previous._next = new CommandOutletContent(_previous, text, this, color);
			else
				return new CommandOutletContent(null, text, this, color);
		}
		#endregion

		#region 静态方法
		public static CommandOutletContent Create(string text)
		{
			return new CommandOutletContent(null, text);
		}

		public static CommandOutletContent Create(CommandOutletColor color, string text)
		{
			return new CommandOutletContent(null, text, color);
		}
		#endregion
	}
}

/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2011-2013 Zongsoft Corporation <http://www.zongsoft.com>
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
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace Zongsoft.Terminals
{
	public interface ITerminal
	{
		#region 属性定义
		TerminalColor BackgroundColor
		{
			get;
			set;
		}

		TerminalColor ForegroundColor
		{
			get;
			set;
		}

		TextReader Input
		{
			get;
			set;
		}

		Stream InputStream
		{
			get;
		}

		TextWriter Output
		{
			get;
			set;
		}

		Stream OutputStream
		{
			get;
		}

		TextWriter Error
		{
			get;
			set;
		}

		Stream ErrorStream
		{
			get;
		}
		#endregion

		#region 方法定义
		void Clear();
		void Reset();
		void ResetStyles(TerminalStyles styles);

		void Write(string text);
		void Write(object value);
		void Write(string format, params object[] args);
		void Write(TerminalColor foregroundColor, string text);
		void Write(TerminalColor foregroundColor, object value);
		void Write(TerminalColor foregroundColor, string format, params object[] args);

		void WriteLine();
		void WriteLine(string text);
		void WriteLine(object value);
		void WriteLine(string format, params object[] args);
		void WriteLine(TerminalColor foregroundColor, string text);
		void WriteLine(TerminalColor foregroundColor, object value);
		void WriteLine(TerminalColor foregroundColor, string format, params object[] args);
		#endregion
	}
}

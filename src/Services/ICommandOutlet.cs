/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2016 Zongsoft Corporation <http://www.zongsoft.com>
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
using System.Text;

namespace Zongsoft.Services
{
	public interface ICommandOutlet
	{
		Encoding Encoding
		{
			get;
			set;
		}

		TextWriter Writer
		{
			get;
		}

		void Write(string text);
		void Write(object value);
		void Write(string format, params object[] args);
		void Write(CommandOutletColor color, string text);
		void Write(CommandOutletColor color, object value);
		void Write(CommandOutletColor color, string format, params object[] args);

		void WriteLine();
		void WriteLine(string text);
		void WriteLine(object value);
		void WriteLine(string format, params object[] args);
		void WriteLine(CommandOutletColor color, string text);
		void WriteLine(CommandOutletColor color, object value);
		void WriteLine(CommandOutletColor color, string format, params object[] args);
	}
}

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
	public class ConsoleTerminal : MarshalByRefObject, ITerminal
	{
		#region 同步变量
		private readonly object _syncRoot;
		#endregion

		#region 成员变量
		private TerminalCommandExecutor _executor;
		#endregion

		#region 构造函数
		public ConsoleTerminal()
		{
			_syncRoot = new object();
			_executor = new TerminalCommandExecutor(this);
		}

		public ConsoleTerminal(TerminalCommandExecutor executor)
		{
			if(executor == null)
				throw new ArgumentNullException("executor");

			_syncRoot = new object();
			_executor = executor;
		}
		#endregion

		#region 公共属性
		public TerminalCommandExecutor Executor
		{
			get
			{
				return _executor;
			}
			set
			{
				if(value == null)
					throw new ArgumentNullException();

				if(object.ReferenceEquals(_executor, value))
					return;

				_executor = value;
			}
		}

		public TerminalColor BackgroundColor
		{
			get
			{
				return this.ConvertColor(Console.BackgroundColor, TerminalColor.Black);
			}
			set
			{
				Console.BackgroundColor = ConvertColor(value, ConsoleColor.Black);
			}
		}

		public TerminalColor ForegroundColor
		{
			get
			{
				return this.ConvertColor(Console.ForegroundColor, TerminalColor.White);
			}
			set
			{
				Console.ForegroundColor = ConvertColor(value, ConsoleColor.White);
			}
		}
		#endregion

		#region 公共方法
		public void Clear()
		{
			Console.Clear();
		}

		public void Reset()
		{
			//恢复默认的颜色设置
			Console.ResetColor();

			try
			{
				if(Console.CursorLeft > 0)
					Console.WriteLine();
			}
			catch
			{
			}

			//先显示命令提示符
			if(_executor == null || _executor.Current == null || _executor.Current == _executor.Root)
				Console.Write("#>");
			else
				Console.Write("{0}>", _executor.Current.FullPath);
		}

		public void ResetStyles(TerminalStyles styles)
		{
			if((styles & TerminalStyles.Color) == TerminalStyles.Color)
				Console.ResetColor();
		}

		public void Write(string text)
		{
			Console.Write(text);
		}

		public void Write(object value)
		{
			Console.Write(value);
		}

		public void Write(TerminalColor foregroundColor, string text)
		{
			lock(_syncRoot)
			{
				var originalColor = this.ForegroundColor;
				this.ForegroundColor = foregroundColor;

				Console.Write(text);

				this.ForegroundColor = originalColor;
			}
		}

		public void Write(TerminalColor foregroundColor, object value)
		{
			lock(_syncRoot)
			{
				var originalColor = this.ForegroundColor;
				this.ForegroundColor = foregroundColor;

				Console.Write(value);

				this.ForegroundColor = originalColor;
			}
		}

		public void Write(string format, params object[] args)
		{
			Console.Write(format, args);
		}

		public void Write(TerminalColor foregroundColor, string format, params object[] args)
		{
			lock(_syncRoot)
			{
				var originalColor = this.ForegroundColor;
				this.ForegroundColor = foregroundColor;

				Console.Write(format, args);

				this.ForegroundColor = originalColor;
			}
		}

		public void WriteLine()
		{
			Console.WriteLine();
		}

		public void WriteLine(string text)
		{
			Console.WriteLine(text);
		}

		public void WriteLine(object value)
		{
			Console.WriteLine(value);
		}

		public void WriteLine(TerminalColor foregroundColor, string text)
		{
			lock(_syncRoot)
			{
				var originalColor = this.ForegroundColor;
				this.ForegroundColor = foregroundColor;

				Console.WriteLine(text);

				this.ForegroundColor = originalColor;
			}
		}

		public void WriteLine(TerminalColor foregroundColor, object value)
		{
			lock(_syncRoot)
			{
				var originalColor = this.ForegroundColor;
				this.ForegroundColor = foregroundColor;

				Console.WriteLine(value);

				this.ForegroundColor = originalColor;
			}
		}

		public void WriteLine(string format, params object[] args)
		{
			Console.WriteLine(format, args);
		}

		public void WriteLine(TerminalColor foregroundColor, string format, params object[] args)
		{
			lock(_syncRoot)
			{
				var originalColor = this.ForegroundColor;
				this.ForegroundColor = foregroundColor;

				Console.WriteLine(format, args);

				this.ForegroundColor = originalColor;
			}
		}
		#endregion

		#region 显示实现
		TextReader ITerminal.Input
		{
			get
			{
				return Console.In;
			}
			set
			{
				Console.SetIn(value);
			}
		}

		Stream ITerminal.InputStream
		{
			get
			{
				return Console.OpenStandardInput();
			}
		}

		TextWriter ITerminal.Output
		{
			get
			{
				return Console.Out;
			}
			set
			{
				Console.SetOut(value);
			}
		}

		Stream ITerminal.OutputStream
		{
			get
			{
				return Console.OpenStandardOutput();
			}
		}

		TextWriter ITerminal.Error
		{
			get
			{
				return Console.Error;
			}
			set
			{
				Console.SetError(value);
			}
		}

		Stream ITerminal.ErrorStream
		{
			get
			{
				return Console.OpenStandardError();
			}
		}
		#endregion

		#region 私有方法
		private TerminalColor ConvertColor(ConsoleColor color, TerminalColor defaultColor)
		{
			TerminalColor result;

			if(Enum.TryParse<TerminalColor>(color.ToString(), out result))
				return result;
			else
				return defaultColor;
		}

		private ConsoleColor ConvertColor(TerminalColor color, ConsoleColor defaultColor)
		{
			ConsoleColor result;

			if(Enum.TryParse<ConsoleColor>(color.ToString(), out result))
				return result;
			else
				return defaultColor;
		}
		#endregion
	}
}

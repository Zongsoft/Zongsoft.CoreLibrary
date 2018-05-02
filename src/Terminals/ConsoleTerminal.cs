/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2011-2016 Zongsoft Corporation <http://www.zongsoft.com>
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
using System.ComponentModel;

using Zongsoft.Services;

namespace Zongsoft.Terminals
{
	public class ConsoleTerminal : ITerminal
	{
		#region 单例字段
		public static readonly ConsoleTerminal Instance = new ConsoleTerminal();
		#endregion

		#region 事件定义
		public event EventHandler Resetted;
		public event EventHandler Resetting;
		public event CancelEventHandler Aborting;
		#endregion

		#region 同步变量
		private readonly object _syncRoot;
		#endregion

		#region 私有构造
		private ConsoleTerminal()
		{
			_syncRoot = new object();

			Console.TreatControlCAsInput = false;
			Console.CancelKeyPress += this.Console_CancelKeyPress;
		}
		#endregion

		#region 公共属性
		public TextReader Input
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

		public TextWriter Output
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

		public TextWriter Error
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

		public CommandOutletColor BackgroundColor
		{
			get
			{
				return this.ConvertColor(Console.BackgroundColor, CommandOutletColor.Black);
			}
			set
			{
				Console.BackgroundColor = ConvertColor(value, ConsoleColor.Black);
			}
		}

		public CommandOutletColor ForegroundColor
		{
			get
			{
				return this.ConvertColor(Console.ForegroundColor, CommandOutletColor.White);
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
			//激发“Resetting”事件
			this.OnResetting();

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

			//激发“Resetted”事件
			this.OnResetted();
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

		public void Write(CommandOutletColor foregroundColor, string text)
		{
			lock(_syncRoot)
			{
				var originalColor = this.ForegroundColor;
				this.ForegroundColor = foregroundColor;

				Console.Write(text);

				this.ForegroundColor = originalColor;
			}
		}

		public void Write(CommandOutletColor foregroundColor, object value)
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

		public void Write(CommandOutletColor foregroundColor, string format, params object[] args)
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

		public void WriteLine(CommandOutletColor foregroundColor, string text)
		{
			lock(_syncRoot)
			{
				var originalColor = this.ForegroundColor;
				this.ForegroundColor = foregroundColor;

				Console.WriteLine(text);

				this.ForegroundColor = originalColor;
			}
		}

		public void WriteLine(CommandOutletColor foregroundColor, object value)
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

		public void WriteLine(CommandOutletColor foregroundColor, string format, params object[] args)
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

		#region 显式实现
		Encoding ICommandOutlet.Encoding
		{
			get
			{
				return Console.OutputEncoding;
			}
			set
			{
				Console.OutputEncoding = value;
			}
		}

		TextWriter ICommandOutlet.Writer
		{
			get
			{
				return Console.Out;
			}
		}
		#endregion

		#region 激发事件
		protected virtual bool OnAborting()
		{
			var e = this.Aborting;

			if(e != null)
			{
				var args = new CancelEventArgs();
				e(this, args);
				return args.Cancel;
			}

			return false;
		}

		protected virtual void OnResetted()
		{
			this.Resetted?.Invoke(this, EventArgs.Empty);
		}

		protected virtual void OnResetting()
		{
			this.Resetting?.Invoke(this, EventArgs.Empty);
		}
		#endregion

		#region 中断事件
		private void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
		{
			e.Cancel = this.OnAborting();
		}
		#endregion

		#region 私有方法
		private CommandOutletColor ConvertColor(ConsoleColor color, CommandOutletColor defaultColor)
		{
			CommandOutletColor result;

			if(Enum.TryParse<CommandOutletColor>(color.ToString(), out result))
				return result;
			else
				return defaultColor;
		}

		private ConsoleColor ConvertColor(CommandOutletColor color, ConsoleColor defaultColor)
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

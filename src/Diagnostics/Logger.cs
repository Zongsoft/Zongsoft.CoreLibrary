/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2015 Zongsoft Corporation <http://www.zongsoft.com>
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
using System.Diagnostics;
using System.Collections.Generic;

namespace Zongsoft.Diagnostics
{
	public static class Logger
	{
		#region 成员字段
		private static LoggerHandlerCollection _handlers;
		#endregion

		#region 公共属性
		public static LoggerHandlerCollection Handlers
		{
			get
			{
				if(_handlers == null)
					System.Threading.Interlocked.CompareExchange(ref _handlers, new LoggerHandlerCollection(), null);

				return _handlers;
			}
		}

		public static Zongsoft.Text.TemplateEvaluatorManager TemplateManager
		{
			get
			{
				return Text.TemplateEvaluatorManager.Default;
			}
		}
		#endregion

		#region 日志方法
		public static void Trace(Exception exception, object data = null)
		{
			Log(new LogEntry(LogLevel.Trace, GetSource(), exception, data));
		}

		public static void Trace(string message, object data = null)
		{
			Log(new LogEntry(LogLevel.Trace, GetSource(), message, data));
		}

		public static void Trace(string source, Exception exception, object data = null)
		{
			source = string.IsNullOrWhiteSpace(source) ? GetSource() : source.Trim();
			Log(new LogEntry(LogLevel.Trace, source, exception, data));
		}

		public static void Trace(string source, string message, object data = null)
		{
			source = string.IsNullOrWhiteSpace(source) ? GetSource() : source.Trim();
			Log(new LogEntry(LogLevel.Trace, source, message, data));
		}

		public static void Trace(string source, string message, Exception exception, object data = null)
		{
			source = string.IsNullOrWhiteSpace(source) ? GetSource() : source.Trim();
			Log(new LogEntry(LogLevel.Trace, source, message, exception, data));
		}

		public static void Debug(Exception exception, object data = null)
		{
			Log(new LogEntry(LogLevel.Debug, GetSource(), exception, data));
		}

		public static void Debug(string message, object data = null)
		{
			Log(new LogEntry(LogLevel.Debug, GetSource(), message, data));
		}

		public static void Debug(string source, Exception exception, object data = null)
		{
			source = string.IsNullOrWhiteSpace(source) ? GetSource() : source.Trim();
			Log(new LogEntry(LogLevel.Debug, source, exception, data));
		}

		public static void Debug(string source, string message, object data = null)
		{
			source = string.IsNullOrWhiteSpace(source) ? GetSource() : source.Trim();
			Log(new LogEntry(LogLevel.Debug, source, message, data));
		}

		public static void Debug(string source, string message, Exception exception, object data = null)
		{
			source = string.IsNullOrWhiteSpace(source) ? GetSource() : source.Trim();
			Log(new LogEntry(LogLevel.Debug, source, message, exception, data));
		}

		public static void Info(Exception exception, object data = null)
		{
			Log(new LogEntry(LogLevel.Info, GetSource(), exception, data));
		}

		public static void Info(string message, object data = null)
		{
			Log(new LogEntry(LogLevel.Info, GetSource(), message, data));
		}

		public static void Info(string source, Exception exception, object data = null)
		{
			source = string.IsNullOrWhiteSpace(source) ? GetSource() : source.Trim();
			Log(new LogEntry(LogLevel.Info, source, exception, data));
		}

		public static void Info(string source, string message, object data = null)
		{
			source = string.IsNullOrWhiteSpace(source) ? GetSource() : source.Trim();
			Log(new LogEntry(LogLevel.Info, source, message, data));
		}

		public static void Info(string source, string message, Exception exception, object data = null)
		{
			source = string.IsNullOrWhiteSpace(source) ? GetSource() : source.Trim();
			Log(new LogEntry(LogLevel.Info, source, message, exception, data));
		}

		public static void Warn(Exception exception, object data = null)
		{
			Log(new LogEntry(LogLevel.Warn, GetSource(), exception, data));
		}

		public static void Warn(string message, object data = null)
		{
			Log(new LogEntry(LogLevel.Warn, GetSource(), message, data));
		}

		public static void Warn(string source, Exception exception, object data = null)
		{
			source = string.IsNullOrWhiteSpace(source) ? GetSource() : source.Trim();
			Log(new LogEntry(LogLevel.Warn, source, exception, data));
		}

		public static void Warn(string source, string message, object data = null)
		{
			source = string.IsNullOrWhiteSpace(source) ? GetSource() : source.Trim();
			Log(new LogEntry(LogLevel.Warn, source, message, data));
		}

		public static void Warn(string source, string message, Exception exception, object data = null)
		{
			source = string.IsNullOrWhiteSpace(source) ? GetSource() : source.Trim();
			Log(new LogEntry(LogLevel.Warn, source, message, exception, data));
		}

		public static void Error(Exception exception, object data = null)
		{
			Log(new LogEntry(LogLevel.Error, GetSource(), exception, data));
		}

		public static void Error(string message, object data = null)
		{
			Log(new LogEntry(LogLevel.Error, GetSource(), message, data));
		}

		public static void Error(string source, Exception exception, object data = null)
		{
			source = string.IsNullOrWhiteSpace(source) ? GetSource() : source.Trim();
			Log(new LogEntry(LogLevel.Error, source, exception, data));
		}

		public static void Error(string source, string message, object data = null)
		{
			source = string.IsNullOrWhiteSpace(source) ? GetSource() : source.Trim();
			Log(new LogEntry(LogLevel.Error, source, message, data));
		}

		public static void Error(string source, string message, Exception exception, object data = null)
		{
			source = string.IsNullOrWhiteSpace(source) ? GetSource() : source.Trim();
			Log(new LogEntry(LogLevel.Error, source, message, exception, data));
		}

		public static void Fatal(Exception exception, object data = null)
		{
			Log(new LogEntry(LogLevel.Fatal, GetSource(), exception, data));
		}

		public static void Fatal(string message, object data = null)
		{
			Log(new LogEntry(LogLevel.Fatal, GetSource(), message, data));
		}

		public static void Fatal(string source, Exception exception, object data = null)
		{
			source = string.IsNullOrWhiteSpace(source) ? GetSource() : source.Trim();
			Log(new LogEntry(LogLevel.Fatal, source, exception, data));
		}

		public static void Fatal(string source, string message, object data = null)
		{
			source = string.IsNullOrWhiteSpace(source) ? GetSource() : source.Trim();
			Log(new LogEntry(LogLevel.Fatal, source, message, data));
		}

		public static void Fatal(string source, string message, Exception exception, object data = null)
		{
			source = string.IsNullOrWhiteSpace(source) ? GetSource() : source.Trim();
			Log(new LogEntry(LogLevel.Fatal, source, message, exception, data));
		}

		public static void Log(LogLevel level, Exception exception, object data = null)
		{
			Log(new LogEntry(level, GetSource(), exception, data));
		}

		public static void Log(LogLevel level, string message, object data = null)
		{
			Log(new LogEntry(level, GetSource(), message, data));
		}

		public static void Log(LogLevel level, string source, Exception exception, object data = null)
		{
			source = string.IsNullOrWhiteSpace(source) ? GetSource() : source.Trim();
			Log(new LogEntry(level, source, exception, data));
		}

		public static void Log(LogLevel level, string source, string message, object data = null)
		{
			source = string.IsNullOrWhiteSpace(source) ? GetSource() : source.Trim();
			Log(new LogEntry(level, source, message, data));
		}

		public static void Log(LogLevel level, string source, string message, Exception exception, object data = null)
		{
			source = string.IsNullOrWhiteSpace(source) ? GetSource() : source.Trim();
			Log(new LogEntry(level, source, message, exception, data));
		}

		private static void Log(LogEntry entry)
		{
			if(entry == null || _handlers == null)
				return;

			System.Threading.Tasks.Parallel.ForEach(_handlers, handler =>
			{
				if(handler != null)
					handler.Handle(entry);
			});
		}
		#endregion

		#region 私有方法
		private static string GetSource()
		{
			var frame = new StackFrame(2, true);

			if(frame == null)
				return string.Empty;

			return frame.GetMethod().DeclaringType.Assembly.GetName().Name;
		}
		#endregion
	}
}

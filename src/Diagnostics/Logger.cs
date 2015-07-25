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
	public class Logger
	{
		#region 成员字段
		private object _parameter;
		private ILoggerSelector _selector;
		#endregion

		#region 静态方法
		public static Logger GetLogger()
		{
			return GetLogger(null);
		}

		public static Logger GetLogger(object parameter)
		{
			throw new NotImplementedException();
		}
		#endregion

		#region 日志方法
		public void Trace(Exception exception, object data = null)
		{
			this.Log(new LogEntry(LogLevel.Trace, this.GetSource(), exception, data));
		}

		public void Trace(string message, object data = null)
		{
			this.Log(new LogEntry(LogLevel.Trace, this.GetSource(), message, data));
		}

		public void Trace(string source, Exception exception, object data = null)
		{
			source = string.IsNullOrWhiteSpace(source) ? this.GetSource() : source.Trim();
			this.Log(new LogEntry(LogLevel.Trace, source, exception, data));
		}

		public void Trace(string source, string message, object data = null)
		{
			source = string.IsNullOrWhiteSpace(source) ? this.GetSource() : source.Trim();
			this.Log(new LogEntry(LogLevel.Trace, source, message, data));
		}

		public void Debug(Exception exception, object data = null)
		{
			this.Log(new LogEntry(LogLevel.Debug, this.GetSource(), exception, data));
		}

		public void Debug(string message, object data = null)
		{
			this.Log(new LogEntry(LogLevel.Debug, this.GetSource(), message, data));
		}

		public void Debug(string source, Exception exception, object data = null)
		{
			source = string.IsNullOrWhiteSpace(source) ? this.GetSource() : source.Trim();
			this.Log(new LogEntry(LogLevel.Debug, source, exception, data));
		}

		public void Debug(string source, string message, object data = null)
		{
			source = string.IsNullOrWhiteSpace(source) ? this.GetSource() : source.Trim();
			this.Log(new LogEntry(LogLevel.Debug, source, message, data));
		}

		public void Info(Exception exception, object data = null)
		{
			this.Log(new LogEntry(LogLevel.Info, this.GetSource(), exception, data));
		}

		public void Info(string message, object data = null)
		{
			this.Log(new LogEntry(LogLevel.Info, this.GetSource(), message, data));
		}

		public void Info(string source, Exception exception, object data = null)
		{
			source = string.IsNullOrWhiteSpace(source) ? this.GetSource() : source.Trim();
			this.Log(new LogEntry(LogLevel.Info, source, exception, data));
		}

		public void Info(string source, string message, object data = null)
		{
			source = string.IsNullOrWhiteSpace(source) ? this.GetSource() : source.Trim();
			this.Log(new LogEntry(LogLevel.Info, source, message, data));
		}

		public void Warn(Exception exception, object data = null)
		{
			this.Log(new LogEntry(LogLevel.Warn, this.GetSource(), exception, data));
		}

		public void Warn(string message, object data = null)
		{
			this.Log(new LogEntry(LogLevel.Warn, this.GetSource(), message, data));
		}

		public void Warn(string source, Exception exception, object data = null)
		{
			source = string.IsNullOrWhiteSpace(source) ? this.GetSource() : source.Trim();
			this.Log(new LogEntry(LogLevel.Warn, source, exception, data));
		}

		public void Warn(string source, string message, object data = null)
		{
			source = string.IsNullOrWhiteSpace(source) ? this.GetSource() : source.Trim();
			this.Log(new LogEntry(LogLevel.Warn, source, message, data));
		}

		public void Error(Exception exception, object data = null)
		{
			this.Log(new LogEntry(LogLevel.Error, this.GetSource(), exception, data));
		}

		public void Error(string message, object data = null)
		{
			this.Log(new LogEntry(LogLevel.Error, this.GetSource(), message, data));
		}

		public void Error(string source, Exception exception, object data = null)
		{
			source = string.IsNullOrWhiteSpace(source) ? this.GetSource() : source.Trim();
			this.Log(new LogEntry(LogLevel.Error, source, exception, data));
		}

		public void Error(string source, string message, object data = null)
		{
			source = string.IsNullOrWhiteSpace(source) ? this.GetSource() : source.Trim();
			this.Log(new LogEntry(LogLevel.Error, source, message, data));
		}

		public void Fatal(Exception exception, object data = null)
		{
			this.Log(new LogEntry(LogLevel.Fatal, this.GetSource(), exception, data));
		}

		public void Fatal(string message, object data = null)
		{
			this.Log(new LogEntry(LogLevel.Fatal, this.GetSource(), message, data));
		}

		public void Fatal(string source, Exception exception, object data = null)
		{
			source = string.IsNullOrWhiteSpace(source) ? this.GetSource() : source.Trim();
			this.Log(new LogEntry(LogLevel.Fatal, source, exception, data));
		}

		public void Fatal(string source, string message, object data = null)
		{
			source = string.IsNullOrWhiteSpace(source) ? this.GetSource() : source.Trim();
			this.Log(new LogEntry(LogLevel.Fatal, source, message, data));
		}

		protected virtual void Log(LogEntry entry)
		{
			if(entry == null)
				return;

			var loggers = _selector.Select(_parameter);

			System.Threading.Tasks.Parallel.ForEach(loggers, logger =>
			{
				if(logger != null)
					logger.Log(entry);
			});
		}
		#endregion

		#region 私有方法
		private string GetSource()
		{
			var frame = new StackFrame(2, true);

			if(frame == null)
				return string.Empty;

			return frame.GetMethod().DeclaringType.Assembly.GetName().Name;
		}
		#endregion
	}
}

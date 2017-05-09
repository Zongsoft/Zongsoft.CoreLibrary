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
using System.Collections.Generic;
using System.Text;

namespace Zongsoft.Diagnostics
{
	public class LogEntry : MarshalByRefObject
	{
		#region 私有变量
		private string _toString;
		#endregion

		#region 成员变量
		private LogLevel _level;
		private object _data;
		private string _source;
		private string _message;
		private string _stackTrace;
		private DateTime _timestamp;
		private Exception _exception;
		#endregion

		#region 构造函数
		public LogEntry(LogLevel level, string source, string message, object data = null) : this(level, source, message, null, data)
		{
		}

		public LogEntry(LogLevel level, string source, Exception exception, object data = null) : this(level, source, null, exception, data)
		{
		}

		public LogEntry(LogLevel level, string source, string message, Exception exception, object data = null)
		{
			if(data is Exception && exception == null)
			{
				exception = (Exception)data;
				data = null;
			}

			_level = level;
			_toString = null;
			_stackTrace = string.Empty;
			_source = string.IsNullOrEmpty(source) ? (exception == null ? string.Empty : exception.Source) : source.Trim();
			_exception = exception;
			_message = message ?? (exception == null ? string.Empty : exception.Message);
			_data = data ?? (exception != null && exception.Data != null && exception.Data.Count > 0 ? exception.Data : null);
			_timestamp = DateTime.Now;
		}
		#endregion

		#region 公共属性
		public LogLevel Level
		{
			get
			{
				return _level;
			}
		}

		public string Source
		{
			get
			{
				return _source;
			}
		}

		public Exception Exception
		{
			get
			{
				return _exception;
			}
		}

		public string Message
		{
			get
			{
				return _message;
			}
		}

		public string StackTrace
		{
			get
			{
				return _stackTrace;
			}
			internal set
			{
				_stackTrace = value;
				_toString = null;
			}
		}

		public object Data
		{
			get
			{
				return _data;
			}
		}

		public DateTime Timestamp
		{
			get
			{
				return _timestamp;
			}
		}
		#endregion

		#region 重写方法
		public override string ToString()
		{
			if(string.IsNullOrEmpty(_toString))
			{
				StringBuilder builder = new StringBuilder(512);

				builder.AppendFormat("<log level=\"{0}\" source=\"{1}\" timestamp=\"{2}\">", _level.ToString().ToUpperInvariant(), _source, _timestamp);
				builder.AppendLine();
				builder.AppendLine("\t<message><![CDATA[" + _message + "]]></message>");

				var aggregateException = _exception as AggregateException;

				if(aggregateException != null)
				{
					if(aggregateException.InnerExceptions != null && aggregateException.InnerExceptions.Count > 0)
					{
						foreach(var exception in aggregateException.InnerExceptions)
							this.WriteException(builder, exception);
					}
					else
					{
						this.WriteException(builder, _exception);
					}
				}
				else
				{
					this.WriteException(builder, _exception);
				}

				if(_data != null)
				{
					builder.AppendLine();
					builder.AppendFormat("\t<data type=\"{0}, {1}\">" + Environment.NewLine, _data.GetType().FullName, _data.GetType().Assembly.GetName().Name);
					builder.AppendLine("\t<![CDATA[");

					byte[] bytes = _data as byte[];

					if(bytes == null)
						builder.AppendLine(Zongsoft.Runtime.Serialization.Serializer.Text.Serialize(_data));
					else
						builder.AppendLine(Zongsoft.Common.Convert.ToHexString(bytes));

					builder.AppendLine("\t]]>");
					builder.AppendLine("\t</data>");
				}

				if(_stackTrace != null && _stackTrace.Length > 0)
				{
					builder.AppendLine();
					builder.AppendLine("\t<stackTrace>");
					builder.AppendLine("\t<![CDATA[");
					builder.AppendLine(_stackTrace);
					builder.AppendLine("\t]]>");
					builder.AppendLine("\t</stackTrace>");
				}

				builder.AppendLine("</log>");

				_toString = builder.ToString();
			}

			return _toString;
		}

		private void WriteException(StringBuilder builder, Exception exception)
		{
			if(builder == null || exception == null)
				return;

			builder.AppendLine();
			builder.AppendFormat("\t<exception type=\"{0}, {1}\">" + Environment.NewLine, _exception.GetType().FullName, _exception.GetType().Assembly.GetName().Name);

			if(!string.Equals(_message, exception.Message))
			{
				builder.AppendLine("\t\t<message><![CDATA[" + exception.Message + "]]></message>");
			}

			if(exception.StackTrace != null && exception.StackTrace.Length > 0)
			{
				builder.AppendLine("\t\t<stackTrace>");
				builder.AppendLine("\t\t<![CDATA[");
				builder.AppendLine(exception.StackTrace);
				builder.AppendLine("\t\t]]>");
				builder.AppendLine("\t\t</stackTrace>");
			}

			builder.AppendLine("\t</exception>");

			if(exception.InnerException != null && exception.InnerException != exception)
				WriteException(builder, exception.InnerException);
		}
		#endregion
	}
}

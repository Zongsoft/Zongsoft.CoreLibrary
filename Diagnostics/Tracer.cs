/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2010-2013 Zongsoft Corporation <http://www.zongsoft.com>
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
using System.Reflection;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zongsoft.Diagnostics
{
	public class Tracer : MarshalByRefObject
	{
		#region 事件声明
		public event EventHandler<FailureEventArgs> Failed;
		public event EventHandler<TraceEventArgs> Traced;
		public event EventHandler<TracingEventArgs> Tracing;
		#endregion

		#region 成员变量
		private TraceListenerCollection _listeners;
		#endregion

		#region 构造函数
		public Tracer()
		{
			_listeners = new TraceListenerCollection();
		}
		#endregion

		#region 单例模式
		private static Tracer _default;
		public static Tracer Default
		{
			get
			{
				if(_default == null)
					System.Threading.Interlocked.CompareExchange(ref _default, new Tracer(), null);

				return _default;
			}
		}
		#endregion

		#region 公共属性
		public TraceListenerCollection Listeners
		{
			get
			{
				return _listeners;
			}
		}
		#endregion

		#region 断言方法
		public bool Assert(bool condition, string source, string message)
		{
			return this.Assert(condition, source, message, null, TraceEntryType.Failure);
		}

		public bool Assert(bool condition, string source, string message, object data)
		{
			return this.Assert(condition, source, message, data, TraceEntryType.Failure);
		}

		public bool Assert(bool condition, string source, string message, TraceEntryType entryType)
		{
			return this.Assert(condition, source, message, null, entryType);
		}

		public bool Assert(bool condition, string source, string message, object data, TraceEntryType entryType)
		{
			if(!condition)
				this.Trace(source, message, data, entryType);

			return !condition;
		}

		public bool Assert(bool condition, TraceEntry entry)
		{
			if(entry == null)
				throw new ArgumentNullException("entry");

			if(!condition)
				this.Trace(entry);

			return !condition;
		}
		#endregion

		#region 跟踪方法
		public void Trace(string source, string message)
		{
			this.Trace(source, message, null, TraceEntryType.Tracing);
		}

		public void Trace(string source, string message, object data)
		{
			this.Trace(source, message, data, TraceEntryType.Tracing);
		}

		public void Trace(string source, string message, TraceEntryType entryType)
		{
			this.Trace(source, message, null, entryType);
		}

		public void Trace(string source, string message, object data, TraceEntryType entryType)
		{
			TraceEntry entry = this.CreateEntry(source, message, data, entryType);

			if(entry != null)
				this.Trace(entry);
		}

		public void Trace(TraceEntry entry)
		{
			if(entry == null)
				throw new ArgumentNullException("entry");

			//设置跟踪实体对象的StackTrace属性
			if(string.IsNullOrEmpty(entry.StackTrace))
				entry.StackTrace = this.GetStackTrace();

			//激发“Tracing”事件
			if(this.OnTracing(entry))
				return;

			foreach(var listener in _listeners)
			{
				if(listener != null)
					this.DoListen(listener, entry);
			}

			//激发“Traced”事件
			this.OnTraced(entry);
		}
		#endregion

		#region 虚拟方法
		protected virtual TraceEntry CreateEntry(string source, string message, object data, TraceEntryType entryType)
		{
			if(string.IsNullOrWhiteSpace(source))
				source = System.Reflection.Assembly.GetCallingAssembly().GetName().Name;

			return new TraceEntry(source, message, data, entryType);
		}
		#endregion

		#region 私有方法
		private string GetStackTrace()
		{
			StackTrace trace = new StackTrace(true);
			StringBuilder builder = new StringBuilder(0x200);

			bool detect = true;
			int baseIndex = 0;

			for(int i = 1; i <= trace.FrameCount - 1; i++)
			{
				StackFrame frame = trace.GetFrame(i);
				MethodBase method = frame.GetMethod();

				if(detect)
				{
					if(method.ReflectedType == this.GetType())
						continue;

					detect = false;
					baseIndex = i;
				}

				builder.AppendFormat("\t[{0}] ", i - baseIndex);

				if(method.ReflectedType != null)
					builder.Append(method.ReflectedType.Name);
				else
					builder.Append("<Module>");

				builder.Append(".");
				builder.Append(method.Name);
				builder.Append("(");

				ParameterInfo[] parameters = method.GetParameters();
				for(int j = 0; j < parameters.Length; j++)
				{
					ParameterInfo info = parameters[j];
					if(j > 0)
						builder.Append(", ");

					builder.Append(info.ParameterType.Name);
					builder.Append(" ");
					builder.Append(info.Name);
				}

				builder.Append(")  ");
				builder.Append(frame.GetFileName());

				int fileLineNumber = frame.GetFileLineNumber();
				if(fileLineNumber > 0)
				{
					builder.Append("(");
					builder.Append(fileLineNumber.ToString(System.Globalization.CultureInfo.InvariantCulture));
					builder.Append(")");
				}

				builder.AppendLine();
			}

			return builder.ToString();
		}

		private void DoListen(ITraceListener listener, TraceEntry entry)
		{
			if(listener == null)
				return;

			bool shouldTrace = true;

			try
			{
				if(listener.Filter != null)
					shouldTrace = listener.Filter.ShouldTrace(entry);
			}
			catch(Exception ex)
			{
				this.OnFailed(new FailureEventArgs(ex, listener.Filter));
			}

			try
			{
				if(shouldTrace)
					listener.OnTrace(entry);
			}
			catch(Exception ex)
			{
				this.OnFailed(new FailureEventArgs(ex, listener));
			}
		}
		#endregion

		#region 激发事件
		protected virtual void OnFailed(FailureEventArgs args)
		{
			if(this.Failed != null)
				this.Failed(this, args);
		}

		protected virtual void OnTraced(TraceEventArgs args)
		{
			if(this.Traced != null)
				this.Traced(this, args);
		}

		protected void OnTraced(TraceEntry entry)
		{
			try
			{
				TraceEventArgs args = new TraceEventArgs(entry);
				this.OnTraced(args);
			}
			catch(Exception ex)
			{
				this.OnFailed(new FailureEventArgs(ex, entry));
			}
		}

		protected virtual void OnTracing(TracingEventArgs args)
		{
			if(this.Tracing != null)
				this.Tracing(this, args);
		}

		protected bool OnTracing(TraceEntry entry)
		{
			try
			{
				TracingEventArgs args = new TracingEventArgs(entry);
				this.OnTracing(args);
				return args.Cancel;
			}
			catch(Exception ex)
			{
				this.OnFailed(new FailureEventArgs(ex, entry));
				return false;
			}
		}
		#endregion
	}
}

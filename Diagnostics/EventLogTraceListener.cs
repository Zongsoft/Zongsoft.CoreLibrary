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
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zongsoft.Diagnostics
{
	public class EventLogTraceListener : TraceListenerBase
	{
		#region 构造函数
		public EventLogTraceListener() : base("EventLogTraceListener")
		{
		}

		public EventLogTraceListener(string name) : base(name)
		{
		}
		#endregion

		#region 重写方法
		public override void OnTrace(TraceEntry entry)
		{
			if(entry == null)
				return;

			System.Diagnostics.EventLog.WriteEntry(entry.Source, entry.ToString(), GetEventLogEntryType(entry.EntryType));
		}
		#endregion

		#region 私有方法
		private static System.Diagnostics.EventLogEntryType GetEventLogEntryType(TraceEntryType entryType)
		{
			switch(entryType)
			{
				case TraceEntryType.Error:
					return System.Diagnostics.EventLogEntryType.Error;
				case TraceEntryType.Failure:
				case TraceEntryType.Warning:
					return System.Diagnostics.EventLogEntryType.Warning;
				case TraceEntryType.Tracing:
					return System.Diagnostics.EventLogEntryType.Information;
			}

			return System.Diagnostics.EventLogEntryType.Information;
		}
		#endregion
	}
}

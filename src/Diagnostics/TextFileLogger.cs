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
using System.IO;
using System.Collections.Generic;

namespace Zongsoft.Diagnostics
{
	public class TextFileLogger : FileLogger
	{
		#region 常量定义
		private const string FORMATSTRING = @"[{log.level}] {log.source} ({log.timestamp}@{environment.machineName}){newline}{log.message}{newline}{log.stackTrace}";
		#endregion

		#region 成员字段
		private string _formatString;
		#endregion

		#region 公共属性
		public string FormatString
		{
			get
			{
				return _formatString;
			}
			set
			{
				_formatString = value;
			}
		}
		#endregion

		#region 重写方法
		protected override void WriteLog(LogEntry entry, Stream stream)
		{
			using(var writer = new StreamWriter(stream, System.Text.Encoding.UTF8))
			{
				writer.WriteLine(entry.ToString());
			}
		}
		#endregion

		#region 虚拟方法
		protected virtual string GetFormattedContent(LogEntry entry)
		{
			var formatString = this.FormatString;

			if(string.IsNullOrWhiteSpace(formatString))
				formatString = FORMATSTRING;

			throw new NotImplementedException();
		}

		protected virtual string Format(LogEntry entry, string formatString)
		{
			throw new NotImplementedException();
		}
		#endregion
	}
}

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
	public abstract class FileLogger : ILogger
	{
		#region 成员字段
		private string _filePath;
		#endregion

		#region 构造函数
		protected FileLogger()
		{
		}

		protected FileLogger(string filePath)
		{
			if(string.IsNullOrWhiteSpace(filePath))
				throw new ArgumentNullException("filePath");

			_filePath = filePath.Trim();
		}
		#endregion

		#region 公共属性
		public string FilePath
		{
			get
			{
				return _filePath;
			}
			set
			{
				_filePath = value;
			}
		}
		#endregion

		#region 日志方法
		public void Log(LogEntry entry)
		{
			if(entry == null)
				return;

			var filePath = this.GetFilePath(entry);

			if(string.IsNullOrWhiteSpace(filePath))
				throw new InvalidOperationException("Unspecified path of the log file.");

			using(var stream = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.Read))
			{
				this.WriteLog(entry, stream);
			}
		}
		#endregion

		#region 抽象方法
		protected abstract void WriteLog(LogEntry entry, Stream output);
		#endregion

		#region 虚拟方法
		protected virtual string GetFilePath(LogEntry entry)
		{
			var filePath = string.Empty;

			if(string.IsNullOrWhiteSpace(_filePath))
				filePath = string.IsNullOrWhiteSpace(entry.Source) ? string.Empty : entry.Source + ".log";
			else
				filePath = _filePath.Trim();

			if(!string.IsNullOrWhiteSpace(filePath))
			{
				var applicationContext = Zongsoft.ComponentModel.ApplicationContextBase.GetApplicationContext();

				if(applicationContext != null)
					filePath = applicationContext.EnsureDirectory(System.IO.Path.GetDirectoryName(filePath));
			}

			return filePath;
		}
		#endregion
	}
}

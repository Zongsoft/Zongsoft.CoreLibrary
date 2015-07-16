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
using System.Reflection;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;

namespace Zongsoft.Diagnostics
{
	public class FileLogTraceListener : TraceListenerBase
	{
		#region 常量定义
		private const int DefaultBufferSize = 1024 * 10;
		#endregion

		#region 成员变量
		private int _maximumFileSize;
		private string _logsDirectory;
		private Zongsoft.Runtime.Serialization.ISerializer _serializer;
		#endregion

		#region 构造函数
		public FileLogTraceListener() : this("FileLogTraceListener")
		{
		}

		public FileLogTraceListener(string name) : base(name)
		{
			_maximumFileSize = 1024 * 1024;
			_logsDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取或设置日子文件的最大大小，单位为字节(Byte)。默认值为1M。
		/// </summary>
		public int MaximumFileSize
		{
			get
			{
				return _maximumFileSize;
			}
			set
			{
				_maximumFileSize = Math.Min(value, 1024 * 1024);
			}
		}

		public string LogsDirectory
		{
			get
			{
				return _logsDirectory;
			}
			set
			{
				if(string.IsNullOrWhiteSpace(value))
					throw new ArgumentNullException();

				_logsDirectory = value;
			}
		}

		public Zongsoft.Runtime.Serialization.ISerializer Serializer
		{
			get
			{
				if(_serializer == null)
				{
					_serializer = new Zongsoft.Runtime.Serialization.Serializer(
						new Runtime.Serialization.JsonSerializationWriter(),
						new Runtime.Serialization.SerializationSettings(3, Runtime.Serialization.SerializationMembers.Properties));
				}

				return _serializer;
			}
			set
			{
				_serializer = value;
			}
		}
		#endregion

		#region 重写方法
		public override void OnTrace(TraceEntry entry)
		{
			if(entry == null)
				return;

			if(!Directory.Exists(_logsDirectory))
				Directory.CreateDirectory(_logsDirectory);

			string fileName = (string.IsNullOrWhiteSpace(entry.Source) ? "nosource" : entry.Source) + ".log";

			char[] invalidChars = System.IO.Path.GetInvalidFileNameChars();
			foreach(char invalidChar in invalidChars)
				fileName.Replace(invalidChar.ToString(), string.Empty);

			this.WriteEntry(Path.Combine(_logsDirectory, fileName), entry);
		}
		#endregion

		/// <summary>
		/// 将日志记录项追加到指定的日志文件中。
		/// </summary>
		/// <param name="filePath">要写入的日志文件，如果文件不存在则新建它。</param>
		/// <param name="entry">要写入的日志记录项。</param>
		/// <exception cref="System.ArgumentNullException">filePath 是空(null)或空字符串("")，entry 是空(null)。</exception>
		public void WriteEntry(string filePath, TraceEntry entry)
		{
			if(string.IsNullOrEmpty(filePath))
				throw new ArgumentNullException("filePath");

			if(entry == null)
				throw new ArgumentNullException("entry");

			filePath = this.GetFilePath(filePath);

			using(FileStream stream = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.Read, DefaultBufferSize, false))
			{
				using(var writer = new StreamWriter(stream, Encoding.UTF8, DefaultBufferSize))
				{
					writer.WriteLine("[{0}] {1}, {2}@{3}", entry.EntryType.ToString(), entry.Timestamp.ToString(), Environment.UserName, Environment.MachineName);
					writer.WriteLine("Message: {0}", entry.Message);

					if(!string.IsNullOrEmpty(entry.StackTrace))
					{
						writer.WriteLine("StackTrace:");
						writer.WriteLine(entry.StackTrace);
					}

					if(entry.Data != null)
					{
						writer.WriteLine(this.GetTypeName(entry.Data.GetType()));
						writer.WriteLine("{");
						writer.Flush();

						//创建文本序列化器
						var serializer = this.Serializer;

						//将实体的数据对象序列化到日志文件中
						if(serializer != null)
							serializer.Serialize(stream, entry.Data);

						writer.WriteLine("}");
					}
				}
			}
		}

		private string GetTypeName(Type type)
		{
			if(type.IsPrimitive || type == typeof(string))
				return type.FullName;
			else
				return type.AssemblyQualifiedName;
		}

		private string GetFilePath(string filePath)
		{
			return Zongsoft.IO.PathUtility.GetCurrentFilePathWithSerialNo(filePath, currentFilePath =>
			{
				if(!File.Exists(currentFilePath))
					return false;

				FileInfo fileInfo = new FileInfo(currentFilePath);
				return fileInfo.Length > _maximumFileSize;
			});
		}
	}
}

/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2014-2015 Zongsoft Corporation <http://www.zongsoft.com>
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

namespace Zongsoft.IO
{
	[Zongsoft.Services.Matcher(typeof(FileSystem.Matcher))]
	public class LocalFileSystem : IFileSystem
	{
		#region 单例字段
		public static readonly LocalFileSystem Instance = new LocalFileSystem();
		#endregion

		#region 构造函数
		private LocalFileSystem()
		{
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取本地文件目录系统的模式，始终返回“zfs.local”。
		/// </summary>
		public string Schema
		{
			get
			{
				return "zfs.local";
			}
		}

		public IFile File
		{
			get
			{
				return LocalFileService.Instance;
			}
		}

		public IDirectory Directory
		{
			get
			{
				return LocalDirectoryService.Instance;
			}
		}
		#endregion

		#region 公共方法
		public string GetUrl(string path)
		{
			var fullPath = LocalPath.ToLocalPath(path);
			return @"file:///" + fullPath.Replace('\\', '/').Trim();
		}
		#endregion

		#region 嵌套子类
		private sealed class LocalDirectoryService : IDirectory
		{
			#region 单例字段
			public static readonly LocalDirectoryService Instance = new LocalDirectoryService();
			#endregion

			#region 私有构造
			private LocalDirectoryService()
			{
			}
			#endregion

			#region 公共方法
			public bool Create(string path, IDictionary<string, string> properties = null)
			{
				var fullPath = LocalPath.ToLocalPath(path);

				if(System.IO.Directory.Exists(fullPath))
					return false;

				return System.IO.Directory.CreateDirectory(fullPath) != null;
			}

			public bool Delete(string path)
			{
				var fullPath = LocalPath.ToLocalPath(path);

				try
				{
					System.IO.Directory.Delete(fullPath);
					return true;
				}
				catch(DirectoryNotFoundException)
				{
					return false;
				}
			}

			public bool Delete(string path, bool recursive)
			{
				var fullPath = LocalPath.ToLocalPath(path);

				try
				{
					System.IO.Directory.Delete(fullPath, recursive);
					return true;
				}
				catch(DirectoryNotFoundException)
				{
					return false;
				}
			}

			public void Move(string source, string destination)
			{
				var sourcePath = LocalPath.ToLocalPath(source);
				var destinationPath = LocalPath.ToLocalPath(destination);

				System.IO.Directory.Move(sourcePath, destinationPath);
			}

			public bool Exists(string path)
			{
				var fullPath = LocalPath.ToLocalPath(path);
				return System.IO.Directory.Exists(fullPath);
			}

			public DirectoryInfo GetInfo(string path)
			{
				var fullPath = LocalPath.ToLocalPath(path);
				var info = new System.IO.DirectoryInfo(fullPath);

				if(info == null || !info.Exists)
					return null;

				return new DirectoryInfo(info.FullName, name: info.Name, createdTime: info.CreationTime, modifiedTime: info.LastWriteTime);
			}

			public IEnumerable<string> GetChildren(string path)
			{
				return this.GetChildren(path, null, false);
			}

			public IEnumerable<string> GetChildren(string path, string pattern, bool recursive = false)
			{
				var fullPath = LocalPath.ToLocalPath(path);
				return System.IO.Directory.GetFileSystemEntries(fullPath, pattern, recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
			}

			public IEnumerable<string> GetDirectories(string path)
			{
				return this.GetDirectories(path, null, false);
			}

			public IEnumerable<string> GetDirectories(string path, string pattern, bool recursive = false)
			{
				var fullPath = LocalPath.ToLocalPath(path);
				return System.IO.Directory.GetDirectories(fullPath, pattern, recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
			}

			public IEnumerable<string> GetFiles(string path)
			{
				return this.GetFiles(path, null, false);
			}

			public IEnumerable<string> GetFiles(string path, string pattern, bool recursive = false)
			{
				var fullPath = LocalPath.ToLocalPath(path);
				return System.IO.Directory.GetFiles(fullPath, pattern, recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
			}
			#endregion
		}

		private sealed class LocalFileService : IFile
		{
			#region 单例字段
			public static readonly LocalFileService Instance = new LocalFileService();
			#endregion

			#region 私有构造
			private LocalFileService()
			{
			}
			#endregion

			#region 公共方法
			public bool Delete(string path)
			{
				var fullPath = LocalPath.ToLocalPath(path);

				try
				{
					System.IO.File.Delete(fullPath);
					return true;
				}
				catch(FileNotFoundException)
				{
					return false;
				}
			}

			public bool Exists(string path)
			{
				var fullPath = LocalPath.ToLocalPath(path);
				return System.IO.File.Exists(fullPath);
			}

			public void Copy(string source, string destination)
			{
				this.Copy(source, destination, true);
			}

			public void Copy(string source, string destination, bool overwrite)
			{
				var sourcePath = LocalPath.ToLocalPath(source);
				var destinationPath = LocalPath.ToLocalPath(destination);

				System.IO.File.Copy(sourcePath, destinationPath, overwrite);
			}

			public void Move(string source, string destination)
			{
				var sourcePath = LocalPath.ToLocalPath(source);
				var destinationPath = LocalPath.ToLocalPath(destination);

				System.IO.File.Move(sourcePath, destinationPath);
			}

			public FileInfo GetInfo(string path)
			{
				var fullPath = LocalPath.ToLocalPath(path);
				var info = new System.IO.FileInfo(fullPath);

				if(info == null || !info.Exists)
					return null;

				return new FileInfo(info.FullName, name: info.Name, size: info.Length, createdTime: info.CreationTime, modifiedTime: info.LastWriteTime);
			}

			public Stream Open(string path, IDictionary<string, string> properties = null)
			{
				var fullPath = LocalPath.ToLocalPath(path);
				return System.IO.File.Open(fullPath, FileMode.Open);
			}

			public Stream Open(string path, FileMode mode, IDictionary<string, string> properties = null)
			{
				var fullPath = LocalPath.ToLocalPath(path);
				return System.IO.File.Open(fullPath, mode);
			}

			public Stream Open(string path, FileMode mode, FileAccess access, IDictionary<string, string> properties = null)
			{
				var fullPath = LocalPath.ToLocalPath(path);
				return System.IO.File.Open(fullPath, mode, access);
			}

			public Stream Open(string path, FileMode mode, FileAccess access, FileShare share, IDictionary<string, string> properties = null)
			{
				var fullPath = LocalPath.ToLocalPath(path);
				return System.IO.File.Open(fullPath, mode, access, share);
			}
			#endregion
		}
		#endregion
	}
}

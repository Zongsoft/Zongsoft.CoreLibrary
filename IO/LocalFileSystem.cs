/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2008-2011 Zongsoft Corporation <http://www.zongsoft.com>
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
	public class LocalFileSystem
	{
		#region 单例模式
		public static readonly IFile File = new LocalFile();
		public static readonly IDirectory Directory = new LocalDirectory();
		#endregion

		#region 本地文件
		private sealed class LocalFile : IFile
		{
			#region 私有构造
			internal LocalFile()
			{
			}
			#endregion

			public void Delete(string path)
			{
				System.IO.File.Delete(path);
			}

			public bool Exists(string path)
			{
				return System.IO.File.Exists(path);
			}

			public void Copy(string source, string destination)
			{
				System.IO.File.Copy(source, destination);
			}

			public void Copy(string source, string destination, bool overwrite)
			{
				System.IO.File.Copy(source, destination, overwrite);
			}

			public void Move(string source, string destination)
			{
				System.IO.File.Move(source, destination);
			}

			public Stream Open(string path)
			{
				return System.IO.File.Open(path, FileMode.Open);
			}

			public Stream Open(string path, FileMode mode)
			{
				return System.IO.File.Open(path, mode);
			}

			public Stream Open(string path, FileMode mode, FileAccess access)
			{
				return System.IO.File.Open(path, mode, access);
			}

			public Stream Open(string path, FileMode mode, FileAccess access, FileShare share)
			{
				return System.IO.File.Open(path, mode, access, share);
			}
		}
		#endregion

		#region 本地目录
		private sealed class LocalDirectory : IDirectory
		{
			#region 私有构造
			internal LocalDirectory()
			{
			}
			#endregion

			public void Create(string path)
			{
				System.IO.Directory.CreateDirectory(path);
			}

			public void Delete(string path)
			{
				System.IO.Directory.Delete(path);
			}

			public void Delete(string path, bool recursive)
			{
				System.IO.Directory.Delete(path, recursive);
			}

			public void Move(string source, string destination)
			{
				System.IO.Directory.Move(source, destination);
			}

			public bool Exists(string path)
			{
				return System.IO.Directory.Exists(path);
			}

			public IEnumerable<string> GetChildren(string path)
			{
				return System.IO.Directory.GetFileSystemEntries(path);
			}

			public IEnumerable<string> GetChildren(string path, string pattern, bool recursive)
			{
				return System.IO.Directory.GetFileSystemEntries(path, pattern, recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
			}

			public IEnumerable<string> GetDirectories(string path)
			{
				return System.IO.Directory.GetDirectories(path);
			}

			public IEnumerable<string> GetDirectories(string path, string pattern, bool recursive)
			{
				return System.IO.Directory.GetDirectories(path, pattern, recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
			}

			public IEnumerable<string> GetFiles(string path)
			{
				return System.IO.Directory.GetFiles(path);
			}

			public IEnumerable<string> GetFiles(string path, string pattern, bool recursive)
			{
				return System.IO.Directory.GetFiles(path, pattern, recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
			}
		}
		#endregion
	}
}

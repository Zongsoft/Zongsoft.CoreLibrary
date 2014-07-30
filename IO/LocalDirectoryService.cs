/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2010-2014 Zongsoft Corporation <http://www.zongsoft.com>
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
	public sealed class LocalDirectoryService : IDirectoryService
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
		public bool Create(string path)
		{
			var fullPath = LocalPath.ToLocalPath(path);

			if(System.IO.Directory.Exists(fullPath))
				return false;

			return System.IO.Directory.CreateDirectory(fullPath) != null;
		}

		public void Delete(string path)
		{
			var fullPath = LocalPath.ToLocalPath(path);
			System.IO.Directory.Delete(fullPath);
		}

		public void Delete(string path, bool recursive)
		{
			var fullPath = LocalPath.ToLocalPath(path);
			System.IO.Directory.Delete(fullPath, recursive);
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

		public IEnumerable<string> GetChildren(string path)
		{
			return this.GetChildren(path, null, false);
		}

		public IEnumerable<string> GetChildren(string path, string pattern, bool recursive)
		{
			var fullPath = LocalPath.ToLocalPath(path);
			return System.IO.Directory.GetFileSystemEntries(fullPath, pattern, recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
		}

		public IEnumerable<string> GetDirectories(string path)
		{
			return this.GetDirectories(path, null, false);
		}

		public IEnumerable<string> GetDirectories(string path, string pattern, bool recursive)
		{
			var fullPath = LocalPath.ToLocalPath(path);
			return System.IO.Directory.GetDirectories(fullPath, pattern, recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
		}

		public IEnumerable<string> GetFiles(string path)
		{
			return this.GetFiles(path, null, false);
		}

		public IEnumerable<string> GetFiles(string path, string pattern, bool recursive)
		{
			var fullPath = LocalPath.ToLocalPath(path);
			return System.IO.Directory.GetFiles(fullPath, pattern, recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
		}
		#endregion
	}
}

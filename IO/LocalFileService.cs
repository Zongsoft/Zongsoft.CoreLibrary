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
	public sealed class LocalFileService : IFileService
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
		public void Delete(string path)
		{
			var fullPath = LocalPath.ToLocalPath(path);
			System.IO.File.Delete(fullPath);
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

		public Stream Open(string path)
		{
			var fullPath = LocalPath.ToLocalPath(path);
			return System.IO.File.Open(fullPath, FileMode.Open);
		}

		public Stream Open(string path, FileMode mode)
		{
			var fullPath = LocalPath.ToLocalPath(path);
			return System.IO.File.Open(fullPath, mode);
		}

		public Stream Open(string path, FileMode mode, FileAccess access)
		{
			var fullPath = LocalPath.ToLocalPath(path);
			return System.IO.File.Open(fullPath, mode, access);
		}

		public Stream Open(string path, FileMode mode, FileAccess access, FileShare share)
		{
			var fullPath = LocalPath.ToLocalPath(path);
			return System.IO.File.Open(fullPath, mode, access, share);
		}
		#endregion
	}
}

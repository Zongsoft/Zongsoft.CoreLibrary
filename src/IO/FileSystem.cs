﻿/*
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
	public static class FileSystem
	{
		#region 成员字段
		private static readonly IFile _file;
		private static readonly IDirectory _directory;
		private static Zongsoft.Services.IServiceProvider _providers;
		#endregion

		#region 构造函数
		static FileSystem()
		{
			_file = new FileService();
			_directory = new DirectoryService();
		}
		#endregion

		#region 公共属性
		public static IFile File
		{
			get
			{
				return _file;
			}
		}

		public static IDirectory Directory
		{
			get
			{
				return _directory;
			}
		}

		public static Zongsoft.Services.IServiceProvider Providers
		{
			get
			{
				if(_providers == null)
					System.Threading.Interlocked.CompareExchange(ref _providers, new Zongsoft.Services.ServiceProvider(), null);

				return _providers;
			}
			set
			{
				if(value == null)
					throw new ArgumentNullException();

				_providers = value;
			}
		}
		#endregion

		#region 公共方法
		public static string GetUrl(string virtualPath)
		{
			Path path;
			var fs = GetFileSystem(virtualPath, out path);
			return fs.GetUrl(path.FullPath);
		}
		#endregion

		#region 私有方法
		private static IFileSystem GetFileSystem(string text, out Path path)
		{
			if(string.IsNullOrWhiteSpace(text))
				throw new ArgumentNullException("text");

			if(!Path.TryParse(text, out path))
				throw new PathException(text);

			if(_providers == null)
				throw new InvalidOperationException("The value of 'Services' property is null.");

			var fileSystem = _providers.Resolve<IFileSystem>(path.Schema);

			if(fileSystem == null)
				throw new InvalidOperationException(string.Format("Can not obtain the File or Directory provider by the '{0}' path.", path));

			return fileSystem;
		}

		private static IFile GetFileService(string text, out Path path)
		{
			return GetFileSystem(text, out path).File;
		}

		private static IDirectory GetDirectoryService(string text, out Path path)
		{
			return GetFileSystem(text, out path).Directory;
		}

		private static IFile[] GetFileServices(string[] texts, out Path[] paths)
		{
			if(texts == null || texts.Length == 0)
				throw new ArgumentNullException("texts");

			paths = new Path[texts.Length];
			var result = new IFile[texts.Length];

			for(int i = 0; i < texts.Length; i++)
			{
				result[i] = GetFileService(texts[i], out paths[i]);
			}

			return result;
		}

		private static IDirectory[] GetDirectoryServices(string[] texts, out Path[] paths)
		{
			if(texts == null || texts.Length == 0)
				throw new ArgumentNullException("texts");

			paths = new Path[texts.Length];
			var result = new IDirectory[texts.Length];

			for(int i = 0; i < texts.Length; i++)
			{
				result[i] = GetDirectoryService(texts[i], out paths[i]);
			}

			return result;
		}
		#endregion

		#region 匹配器类
		public class Matcher : Zongsoft.Services.IMatcher<string>
		{
			public bool Match(object target, string parameter)
			{
				var fileSystem = target as IFileSystem;
				return fileSystem != null && string.Equals(fileSystem.Schema, parameter, StringComparison.OrdinalIgnoreCase);
			}

			bool Zongsoft.Services.IMatcher.Match(object target, object parameter)
			{
				var fileSystem = target as IFileSystem;
				return fileSystem != null && string.Equals(fileSystem.Schema, (parameter as string), StringComparison.OrdinalIgnoreCase);
			}
		}
		#endregion

		#region 嵌套子类
		private class DirectoryService : IDirectory
		{
			public bool Create(string virtualPath, IDictionary<string, string> properties = null)
			{
				Path path;
				var service = FileSystem.GetDirectoryService(virtualPath, out path);

				return service.Create(path.FullPath, properties);
			}

			public bool Delete(string virtualPath)
			{
				return this.Delete(virtualPath, false);
			}

			public bool Delete(string virtualPath, bool recursive)
			{
				Path path;
				var service = FileSystem.GetDirectoryService(virtualPath, out path);

				return service.Delete(path.FullPath, recursive);
			}

			public void Move(string source, string destination)
			{
				Path[] paths;
				var services = FileSystem.GetDirectoryServices(new string[] { source, destination }, out paths);

				if(!string.Equals(paths[0].Schema, paths[1].Schema, StringComparison.OrdinalIgnoreCase))
					throw new InvalidOperationException();

				services[0].Move(paths[0].FullPath, paths[1].FullPath);
			}

			public bool Exists(string virtualPath)
			{
				Path path;
				var service = FileSystem.GetDirectoryService(virtualPath, out path);

				return service.Exists(path.FullPath);
			}

			public DirectoryInfo GetInfo(string virtualPath)
			{
				Path path;
				var service = FileSystem.GetDirectoryService(virtualPath, out path);

				return service.GetInfo(path.FullPath);
			}

			public IEnumerable<string> GetChildren(string virtualPath)
			{
				return this.GetChildren(virtualPath, null, false);
			}

			public IEnumerable<string> GetChildren(string virtualPath, string pattern, bool recursive)
			{
				Path path;
				var service = FileSystem.GetDirectoryService(virtualPath, out path);

				return service.GetChildren(path.FullPath, pattern, recursive);
			}

			public IEnumerable<string> GetDirectories(string virtualPath)
			{
				return this.GetDirectories(virtualPath, null, false);
			}

			public IEnumerable<string> GetDirectories(string virtualPath, string pattern, bool recursive)
			{
				Path path;
				var service = FileSystem.GetDirectoryService(virtualPath, out path);

				return service.GetDirectories(path.FullPath, pattern, recursive);
			}

			public IEnumerable<string> GetFiles(string virtualPath)
			{
				return this.GetFiles(virtualPath, null, false);
			}

			public IEnumerable<string> GetFiles(string virtualPath, string pattern, bool recursive)
			{
				Path path;
				var service = FileSystem.GetDirectoryService(virtualPath, out path);

				return service.GetFiles(path.FullPath, pattern, recursive);
			}
		}

		private class FileService : IFile
		{
			public bool Delete(string virtualPath)
			{
				Path path;
				var service = FileSystem.GetFileService(virtualPath, out path);

				return service.Delete(path.FullPath);
			}

			public bool Exists(string virtualPath)
			{
				Path path;
				var service = FileSystem.GetFileService(virtualPath, out path);

				return service.Exists(path.FullPath);
			}

			public void Copy(string source, string destination)
			{
				this.Copy(source, destination, true);
			}

			public void Copy(string source, string destination, bool overwrite)
			{
				Path[] paths;
				var services = FileSystem.GetFileServices(new string[] { source, destination }, out paths);

				if(!string.Equals(paths[0].Schema, paths[1].Schema, StringComparison.OrdinalIgnoreCase))
					throw new InvalidOperationException();

				services[0].Copy(paths[0].FullPath, paths[1].FullPath, overwrite);
			}

			public void Move(string source, string destination)
			{
				Path[] paths;
				var services = FileSystem.GetFileServices(new string[] { source, destination }, out paths);

				if(!string.Equals(paths[0].Schema, paths[1].Schema, StringComparison.OrdinalIgnoreCase))
					throw new InvalidOperationException();

				services[0].Move(paths[0].FullPath, paths[1].FullPath);
			}

			public FileInfo GetInfo(string virtualPath)
			{
				Path path;
				var service = FileSystem.GetFileService(virtualPath, out path);

				return service.GetInfo(path.FullPath);
			}

			public Stream Open(string virtualPath, IDictionary<string, string> properties = null)
			{
				Path path;
				var service = FileSystem.GetFileService(virtualPath, out path);

				return service.Open(path.FullPath, properties);
			}

			public Stream Open(string virtualPath, FileMode mode, IDictionary<string, string> properties = null)
			{
				Path path;
				var service = FileSystem.GetFileService(virtualPath, out path);

				return service.Open(path.FullPath, mode, properties);
			}

			public Stream Open(string virtualPath, FileMode mode, FileAccess access, IDictionary<string, string> properties = null)
			{
				Path path;
				var service = FileSystem.GetFileService(virtualPath, out path);

				return service.Open(path.FullPath, mode, access, properties);
			}

			public Stream Open(string virtualPath, FileMode mode, FileAccess access, System.IO.FileShare share, IDictionary<string, string> properties = null)
			{
				Path path;
				var service = FileSystem.GetFileService(virtualPath, out path);

				return service.Open(path.FullPath, mode, access, share, properties);
			}
		}
		#endregion
	}
}

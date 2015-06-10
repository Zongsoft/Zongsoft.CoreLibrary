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
	public class FileSystem : IFileSystem
	{
		#region 常量定义
		public const string Schema = "zfs";
		#endregion

		#region 单例字段
		//public static readonly FileSystem Default = new FileSystem(Zongsoft.Services.ServiceProvider);
		#endregion

		#region 成员字段
		private Zongsoft.Services.IServiceProvider _serviceProvider;
		private IFile _file;
		private IDirectory _directory;
		#endregion

		#region 构造函数
		public FileSystem()
		{
		}

		public FileSystem(Zongsoft.Services.IServiceProvider serviceProvider)
		{
			if(serviceProvider == null)
				throw new ArgumentNullException("serviceProvider");

			_serviceProvider = serviceProvider;
		}
		#endregion

		#region 公共属性
		public Zongsoft.Services.IServiceProvider ServiceProvider
		{
			get
			{
				return _serviceProvider;
			}
			set
			{
				if(value == null)
					throw new ArgumentNullException();

				_serviceProvider = value;
			}
		}

		public IFile File
		{
			get
			{
				if(_file == null)
					System.Threading.Interlocked.CompareExchange(ref _file, new FileService(this), null);

				return _file;
			}
		}

		public IDirectory Directory
		{
			get
			{
				if(_directory == null)
					System.Threading.Interlocked.CompareExchange(ref _directory, new DirectoryService(this), null);

				return _directory;
			}
		}
		#endregion

		#region 内部方法
		internal T GetService<T>(string text, out Path path) where T : class
		{
			if(string.IsNullOrWhiteSpace(text))
				throw new ArgumentNullException("text");

			var serviceProvider = this.ServiceProvider;

			if(serviceProvider == null)
				throw new InvalidOperationException();

			if(!Path.TryParse(text, out path))
				throw new PathException(text);

			var service = serviceProvider.Resolve<T>(path.Schema) as T;

			if(service == null)
				throw new InvalidOperationException(string.Format("Can not obtain the File or Directory provider by the '{0}' path.", path));

			return service;
		}

		internal T[] GetServices<T>(string[] texts, out Path[] paths) where T : class
		{
			if(texts == null || texts.Length == 0)
				throw new ArgumentNullException("texts");

			paths = new Path[texts.Length];
			var result = new T[texts.Length];

			for(int i = 0; i < texts.Length; i++)
			{
				result[i] = this.GetService<T>(texts[i], out paths[i]);
			}

			return result;
		}
		#endregion

		#region 嵌套子类
		private class FileService : IFile
		{
			#region 成员字段
			private readonly FileSystem _fileSystem;
			#endregion

			#region 构造函数
			internal FileService(FileSystem fileSystem)
			{
				if(fileSystem == null)
					throw new ArgumentNullException("fileSystem");

				_fileSystem = fileSystem;
			}
			#endregion

			public void Delete(string virtualPath)
			{
				Path path;
				var service = _fileSystem.GetService<IFile>(virtualPath, out path);

				service.Delete(path.FullPath);
			}

			public bool Exists(string virtualPath)
			{
				Path path;
				var service = _fileSystem.GetService<IFile>(virtualPath, out path);

				return service.Exists(path.FullPath);
			}

			public void Copy(string source, string destination)
			{
				this.Copy(source, destination, true);
			}

			public void Copy(string source, string destination, bool overwrite)
			{
				Path[] paths;
				var services = _fileSystem.GetServices<IFile>(new string[] { source, destination }, out paths);

				if(!string.Equals(paths[0].Schema, paths[1].Schema, StringComparison.OrdinalIgnoreCase))
					throw new InvalidOperationException();

				services[0].Copy(paths[0].FullPath, paths[1].FullPath, overwrite);
			}

			public void Move(string source, string destination)
			{
				Path[] paths;
				var services = _fileSystem.GetServices<IFile>(new string[] { source, destination }, out paths);

				if(!string.Equals(paths[0].Schema, paths[1].Schema, StringComparison.OrdinalIgnoreCase))
					throw new InvalidOperationException();

				services[0].Move(paths[0].FullPath, paths[1].FullPath);
			}

			public FileInfo GetInfo(string virtualPath)
			{
				Path path;
				var service = _fileSystem.GetService<IFile>(virtualPath, out path);

				return service.GetInfo(path.FullPath);
			}

			public System.IO.Stream Open(string virtualPath)
			{
				Path path;
				var service = _fileSystem.GetService<IFile>(virtualPath, out path);

				return service.Open(path.FullPath);
			}

			public System.IO.Stream Open(string virtualPath, System.IO.FileMode mode)
			{
				Path path;
				var service = _fileSystem.GetService<IFile>(virtualPath, out path);

				return service.Open(path.FullPath, mode);
			}

			public System.IO.Stream Open(string virtualPath, System.IO.FileMode mode, System.IO.FileAccess access)
			{
				Path path;
				var service = _fileSystem.GetService<IFile>(virtualPath, out path);

				return service.Open(path.FullPath, mode, access);
			}

			public System.IO.Stream Open(string virtualPath, System.IO.FileMode mode, System.IO.FileAccess access, System.IO.FileShare share)
			{
				Path path;
				var service = _fileSystem.GetService<IFile>(virtualPath, out path);

				return service.Open(path.FullPath, mode, access, share);
			}
		}

		private class DirectoryService : IDirectory
		{
			#region 成员字段
			private readonly FileSystem _fileSystem;
			#endregion

			#region 构造函数
			internal DirectoryService(FileSystem fileSystem)
			{
				if(fileSystem == null)
					throw new ArgumentNullException("fileSystem");

				_fileSystem = fileSystem;
			}
			#endregion

			public bool Create(string virtualPath)
			{
				Path path;
				var service = _fileSystem.GetService<IDirectory>(virtualPath, out path);

				return service.Create(path.FullPath);
			}

			public void Delete(string virtualPath)
			{
				this.Delete(virtualPath, false);
			}

			public void Delete(string virtualPath, bool recursive)
			{
				Path path;
				var service = _fileSystem.GetService<IDirectory>(virtualPath, out path);

				service.Delete(path.FullPath, recursive);
			}

			public void Move(string source, string destination)
			{
				Path[] paths;
				var services = _fileSystem.GetServices<IDirectory>(new string[] { source, destination }, out paths);

				if(!string.Equals(paths[0].Schema, paths[1].Schema, StringComparison.OrdinalIgnoreCase))
					throw new InvalidOperationException();

				services[0].Move(paths[0].FullPath, paths[1].FullPath);
			}

			public bool Exists(string virtualPath)
			{
				Path path;
				var service = _fileSystem.GetService<IDirectory>(virtualPath, out path);

				return service.Exists(path.FullPath);
			}

			public DirectoryInfo GetInfo(string virtualPath)
			{
				Path path;
				var service = _fileSystem.GetService<IDirectory>(virtualPath, out path);

				return service.GetInfo(path.FullPath);
			}

			public IEnumerable<string> GetChildren(string virtualPath)
			{
				return this.GetChildren(virtualPath, null, false);
			}

			public IEnumerable<string> GetChildren(string virtualPath, string pattern, bool recursive)
			{
				Path path;
				var service = _fileSystem.GetService<IDirectory>(virtualPath, out path);

				return service.GetChildren(path.FullPath, pattern, recursive);
			}

			public IEnumerable<string> GetDirectories(string virtualPath)
			{
				return this.GetDirectories(virtualPath, null, false);
			}

			public IEnumerable<string> GetDirectories(string virtualPath, string pattern, bool recursive)
			{
				Path path;
				var service = _fileSystem.GetService<IDirectory>(virtualPath, out path);

				return service.GetDirectories(path.FullPath, pattern, recursive);
			}

			public IEnumerable<string> GetFiles(string virtualPath)
			{
				return this.GetFiles(virtualPath, null, false);
			}

			public IEnumerable<string> GetFiles(string virtualPath, string pattern, bool recursive)
			{
				Path path;
				var service = _fileSystem.GetService<IDirectory>(virtualPath, out path);

				return service.GetFiles(path.FullPath, pattern, recursive);
			}
		}
		#endregion

		#region 显式实现
		string IFileSystem.Schema
		{
			get
			{
				return FileSystem.Schema;
			}
		}
		#endregion
	}
}

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
using System.Threading.Tasks;

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
			_file = new FileProvider();
			_directory = new DirectoryProvider();
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
			if(_providers == null)
				throw new InvalidOperationException("The value of 'Services' property is null.");

			if(string.IsNullOrWhiteSpace(text))
				throw new ArgumentNullException("text");

			path = Path.Parse(text);

			var fileSystem = _providers.Resolve<IFileSystem>(path.Schema);

			if(fileSystem == null)
				throw new InvalidOperationException(string.Format("Can not obtain the File or Directory provider by the '{0}' path.", path));

			return fileSystem;
		}

		private static IFile GetFileProvider(string text, out Path path)
		{
			return GetFileSystem(text, out path).File;
		}

		private static IDirectory GetDirectoryProvider(string text, out Path path)
		{
			return GetFileSystem(text, out path).Directory;
		}

		private static IFile[] GetFileProviders(string[] texts, out Path[] paths)
		{
			if(texts == null || texts.Length == 0)
				throw new ArgumentNullException("texts");

			paths = new Path[texts.Length];
			var result = new IFile[texts.Length];

			for(int i = 0; i < texts.Length; i++)
			{
				result[i] = GetFileProvider(texts[i], out paths[i]);
			}

			return result;
		}

		private static IDirectory[] GetDirectoryProviders(string[] texts, out Path[] paths)
		{
			if(texts == null || texts.Length == 0)
				throw new ArgumentNullException("texts");

			paths = new Path[texts.Length];
			var result = new IDirectory[texts.Length];

			for(int i = 0; i < texts.Length; i++)
			{
				result[i] = GetDirectoryProvider(texts[i], out paths[i]);
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
		private class DirectoryProvider : IDirectory
		{
			public bool Create(string virtualPath, IDictionary<string, string> properties = null)
			{
				Path path;
				var service = FileSystem.GetDirectoryProvider(virtualPath, out path);

				return service.Create(path.FullPath, properties);
			}

			public Task<bool> CreateAsync(string virtualPath, IDictionary<string, string> properties = null)
			{
				Path path;
				var service = FileSystem.GetDirectoryProvider(virtualPath, out path);

				return service.CreateAsync(path.FullPath, properties);
			}

			public bool Delete(string virtualPath, bool recursive = false)
			{
				Path path;
				var service = FileSystem.GetDirectoryProvider(virtualPath, out path);

				return service.Delete(path.FullPath, recursive);
			}

			public Task<bool> DeleteAsync(string virtualPath, bool recursive)
			{
				Path path;
				var service = FileSystem.GetDirectoryProvider(virtualPath, out path);

				return service.DeleteAsync(path.FullPath, recursive);
			}

			public void Move(string source, string destination)
			{
				Path[] paths;
				var services = FileSystem.GetDirectoryProviders(new string[] { source, destination }, out paths);

				if(!string.Equals(paths[0].Schema, paths[1].Schema, StringComparison.OrdinalIgnoreCase))
					throw new InvalidOperationException();

				services[0].Move(paths[0].FullPath, paths[1].FullPath);
			}

			public Task MoveAsync(string source, string destination)
			{
				Path[] paths;
				var services = FileSystem.GetDirectoryProviders(new string[] { source, destination }, out paths);

				if(!string.Equals(paths[0].Schema, paths[1].Schema, StringComparison.OrdinalIgnoreCase))
					throw new InvalidOperationException();

				return services[0].MoveAsync(paths[0].FullPath, paths[1].FullPath);
			}

			public bool Exists(string virtualPath)
			{
				Path path;
				var service = FileSystem.GetDirectoryProvider(virtualPath, out path);

				return service.Exists(path.FullPath);
			}

			public Task<bool> ExistsAsync(string virtualPath)
			{
				Path path;
				var service = FileSystem.GetDirectoryProvider(virtualPath, out path);

				return service.ExistsAsync(path.FullPath);
			}

			public DirectoryInfo GetInfo(string virtualPath)
			{
				Path path;
				var service = FileSystem.GetDirectoryProvider(virtualPath, out path);

				return service.GetInfo(path.FullPath);
			}

			public Task<DirectoryInfo> GetInfoAsync(string virtualPath)
			{
				Path path;
				var service = FileSystem.GetDirectoryProvider(virtualPath, out path);

				return service.GetInfoAsync(path.FullPath);
			}

			public bool SetInfo(string virtualPath, IDictionary<string, string> properties)
			{
				Path path;
				var service = FileSystem.GetDirectoryProvider(virtualPath, out path);

				return service.SetInfo(path.FullPath, properties);
			}

			public Task<bool> SetInfoAsync(string virtualPath, IDictionary<string, string> properties)
			{
				Path path;
				var service = FileSystem.GetDirectoryProvider(virtualPath, out path);

				return service.SetInfoAsync(path.FullPath, properties);
			}

			public IEnumerable<PathInfo> GetChildren(string virtualPath)
			{
				return this.GetChildren(virtualPath, null, false);
			}

			public IEnumerable<PathInfo> GetChildren(string virtualPath, string pattern, bool recursive = false)
			{
				Path path;
				var service = FileSystem.GetDirectoryProvider(virtualPath, out path);

				return service.GetChildren(path.FullPath, pattern, recursive);
			}

			public Task<IEnumerable<PathInfo>> GetChildrenAsync(string virtualPath)
			{
				return this.GetChildrenAsync(virtualPath, null, false);
			}

			public Task<IEnumerable<PathInfo>> GetChildrenAsync(string virtualPath, string pattern, bool recursive = false)
			{
				Path path;
				var service = FileSystem.GetDirectoryProvider(virtualPath, out path);

				return service.GetChildrenAsync(path.FullPath, pattern, recursive);
			}

			public IEnumerable<DirectoryInfo> GetDirectories(string virtualPath)
			{
				return this.GetDirectories(virtualPath, null, false);
			}

			public IEnumerable<DirectoryInfo> GetDirectories(string virtualPath, string pattern, bool recursive = false)
			{
				Path path;
				var service = FileSystem.GetDirectoryProvider(virtualPath, out path);

				return service.GetDirectories(path.FullPath, pattern, recursive);
			}

			public Task<IEnumerable<DirectoryInfo>> GetDirectoriesAsync(string virtualPath)
			{
				return this.GetDirectoriesAsync(virtualPath, null, false);
			}

			public Task<IEnumerable<DirectoryInfo>> GetDirectoriesAsync(string virtualPath, string pattern, bool recursive = false)
			{
				Path path;
				var service = FileSystem.GetDirectoryProvider(virtualPath, out path);

				return service.GetDirectoriesAsync(path.FullPath, pattern, recursive);
			}

			public IEnumerable<FileInfo> GetFiles(string virtualPath)
			{
				return this.GetFiles(virtualPath, null, false);
			}

			public IEnumerable<FileInfo> GetFiles(string virtualPath, string pattern, bool recursive = false)
			{
				Path path;
				var service = FileSystem.GetDirectoryProvider(virtualPath, out path);

				return service.GetFiles(path.FullPath, pattern, recursive);
			}

			public Task<IEnumerable<FileInfo>> GetFilesAsync(string virtualPath)
			{
				return this.GetFilesAsync(virtualPath, null, false);
			}

			public Task<IEnumerable<FileInfo>> GetFilesAsync(string virtualPath, string pattern, bool recursive = false)
			{
				Path path;
				var service = FileSystem.GetDirectoryProvider(virtualPath, out path);

				return service.GetFilesAsync(path.FullPath, pattern, recursive);
			}
		}

		private class FileProvider : IFile
		{
			public bool Delete(string virtualPath)
			{
				Path path;
				var service = FileSystem.GetFileProvider(virtualPath, out path);

				return service.Delete(path.FullPath);
			}

			public Task<bool> DeleteAsync(string virtualPath)
			{
				Path path;
				var service = FileSystem.GetFileProvider(virtualPath, out path);

				return service.DeleteAsync(path.FullPath);
			}

			public bool Exists(string virtualPath)
			{
				Path path;
				var service = FileSystem.GetFileProvider(virtualPath, out path);

				return service.Exists(path.FullPath);
			}

			public Task<bool> ExistsAsync(string virtualPath)
			{
				Path path;
				var service = FileSystem.GetFileProvider(virtualPath, out path);

				return service.ExistsAsync(path.FullPath);
			}

			public void Copy(string source, string destination)
			{
				this.Copy(source, destination, true);
			}

			public void Copy(string source, string destination, bool overwrite)
			{
				Path[] paths;
				var services = FileSystem.GetFileProviders(new string[] { source, destination }, out paths);

				if(!string.Equals(paths[0].Schema, paths[1].Schema, StringComparison.OrdinalIgnoreCase))
					throw new InvalidOperationException();

				services[0].Copy(paths[0].FullPath, paths[1].FullPath, overwrite);
			}

			public Task CopyAsync(string source, string destination)
			{
				return this.CopyAsync(source, destination, true);
			}

			public Task CopyAsync(string source, string destination, bool overwrite)
			{
				Path[] paths;
				var services = FileSystem.GetFileProviders(new string[] { source, destination }, out paths);

				if(!string.Equals(paths[0].Schema, paths[1].Schema, StringComparison.OrdinalIgnoreCase))
					throw new InvalidOperationException();

				return services[0].CopyAsync(paths[0].FullPath, paths[1].FullPath, overwrite);
			}

			public void Move(string source, string destination)
			{
				Path[] paths;
				var services = FileSystem.GetFileProviders(new string[] { source, destination }, out paths);

				if(!string.Equals(paths[0].Schema, paths[1].Schema, StringComparison.OrdinalIgnoreCase))
					throw new InvalidOperationException();

				services[0].Move(paths[0].FullPath, paths[1].FullPath);
			}

			public Task MoveAsync(string source, string destination)
			{
				Path[] paths;
				var services = FileSystem.GetFileProviders(new string[] { source, destination }, out paths);

				if(!string.Equals(paths[0].Schema, paths[1].Schema, StringComparison.OrdinalIgnoreCase))
					throw new InvalidOperationException();

				return services[0].MoveAsync(paths[0].FullPath, paths[1].FullPath);
			}

			public FileInfo GetInfo(string virtualPath)
			{
				Path path;
				var service = FileSystem.GetFileProvider(virtualPath, out path);

				return service.GetInfo(path.FullPath);
			}

			public Task<FileInfo> GetInfoAsync(string virtualPath)
			{
				Path path;
				var service = FileSystem.GetFileProvider(virtualPath, out path);

				return service.GetInfoAsync(path.FullPath);
			}

			public bool SetInfo(string virtualPath, IDictionary<string, string> properties)
			{
				Path path;
				var service = FileSystem.GetFileProvider(virtualPath, out path);

				return service.SetInfo(path.FullPath, properties);
			}

			public Task<bool> SetInfoAsync(string virtualPath, IDictionary<string, string> properties)
			{
				Path path;
				var service = FileSystem.GetFileProvider(virtualPath, out path);

				return service.SetInfoAsync(path.FullPath, properties);
			}

			public Stream Open(string virtualPath, IDictionary<string, string> properties = null)
			{
				Path path;
				var service = FileSystem.GetFileProvider(virtualPath, out path);

				return service.Open(path.FullPath, properties);
			}

			public Stream Open(string virtualPath, FileMode mode, IDictionary<string, string> properties = null)
			{
				Path path;
				var service = FileSystem.GetFileProvider(virtualPath, out path);

				return service.Open(path.FullPath, mode, properties);
			}

			public Stream Open(string virtualPath, FileMode mode, FileAccess access, IDictionary<string, string> properties = null)
			{
				Path path;
				var service = FileSystem.GetFileProvider(virtualPath, out path);

				return service.Open(path.FullPath, mode, access, properties);
			}

			public Stream Open(string virtualPath, FileMode mode, FileAccess access, System.IO.FileShare share, IDictionary<string, string> properties = null)
			{
				Path path;
				var service = FileSystem.GetFileProvider(virtualPath, out path);

				return service.Open(path.FullPath, mode, access, share, properties);
			}
		}
		#endregion
	}
}

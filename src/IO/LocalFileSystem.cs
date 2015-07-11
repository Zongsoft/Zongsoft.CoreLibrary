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
				return LocalFileProvider.Instance;
			}
		}

		public IDirectory Directory
		{
			get
			{
				return LocalDirectoryProvider.Instance;
			}
		}
		#endregion

		#region 公共方法
		public string GetUrl(string path)
		{
			return @"file:///" + GetLocalPath(path);
		}
		#endregion

		#region 路径解析
		public static string GetLocalPath(string text)
		{
			if(string.IsNullOrWhiteSpace(text))
				throw new ArgumentNullException("text");

			var path = Path.Parse(text);

			switch(Environment.OSVersion.Platform)
			{
				case PlatformID.MacOSX:
				case PlatformID.Unix:
					return path.FullPath;
			}

			var driveName = path.Schema;
			var fullPath = path.FullPath;

			if(string.IsNullOrEmpty(driveName))
			{
				var index = fullPath.IndexOf('/', (fullPath[0] == '/' ? 1 : 0));
				var parts = fullPath.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

				if(parts != null && parts.Length > 0)
				{
					driveName = parts[0];
					fullPath = "/" + string.Join("/", parts, 1, parts.Length - 1);
				}

				if(string.IsNullOrWhiteSpace(driveName))
					throw new PathException(string.Format("The '{0}' path not cantians drive.", text));
			}

			if(driveName.Length > 1)
			{
				var drives = System.IO.DriveInfo.GetDrives();
				var matched = false;

				foreach(var drive in drives)
				{
					matched = MatchDriver(drive, driveName);

					if(matched)
					{
						driveName = drive.Name[0].ToString();
						break;
					}
				}

				if(!matched)
					throw new PathException(string.Format("Not matched drive for '{0}' path.", text));
			}

			return driveName + ":" + fullPath;
		}

		private static bool MatchDriver(System.IO.DriveInfo drive, string driveName)
		{
			if(drive == null)
				return false;

			try
			{
				return string.Equals(drive.VolumeLabel, driveName, StringComparison.OrdinalIgnoreCase);
			}
			catch
			{
				return false;
			}
		}
		#endregion

		#region 嵌套子类
		private sealed class LocalDirectoryProvider : IDirectory
		{
			#region 单例字段
			public static readonly LocalDirectoryProvider Instance = new LocalDirectoryProvider();
			#endregion

			#region 私有构造
			private LocalDirectoryProvider()
			{
			}
			#endregion

			#region 公共方法
			public bool Create(string path, IDictionary<string, string> properties = null)
			{
				var fullPath = GetLocalPath(path);

				if(System.IO.Directory.Exists(fullPath))
					return false;

				return System.IO.Directory.CreateDirectory(fullPath) != null;
			}

			public async Task<bool> CreateAsync(string path, IDictionary<string, string> properties = null)
			{
				var fullPath = GetLocalPath(path);

				if(await Task.Run(() => System.IO.Directory.Exists(fullPath)))
					return false;

				var result = await Task.Run(() => System.IO.Directory.CreateDirectory(fullPath));

				return result != null;
			}

			public bool Delete(string path, bool recursive = false)
			{
				var fullPath = GetLocalPath(path);

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

			public async Task<bool> DeleteAsync(string path, bool recursive = false)
			{
				var fullPath = GetLocalPath(path);

				try
				{
					await Task.Run(() => System.IO.Directory.Delete(fullPath, recursive));
					return true;
				}
				catch(DirectoryNotFoundException)
				{
					return false;
				}
			}

			public void Move(string source, string destination)
			{
				var sourcePath = GetLocalPath(source);
				var destinationPath = GetLocalPath(destination);

				System.IO.Directory.Move(sourcePath, destinationPath);
			}

			public async Task MoveAsync(string source, string destination)
			{
				var sourcePath = GetLocalPath(source);
				var destinationPath = GetLocalPath(destination);

				await Task.Run(() => System.IO.Directory.Move(sourcePath, destinationPath));
			}

			public bool Exists(string path)
			{
				var fullPath = GetLocalPath(path);
				return System.IO.Directory.Exists(fullPath);
			}

			public async Task<bool> ExistsAsync(string path)
			{
				var fullPath = GetLocalPath(path);
				return await Task.Run(() => System.IO.Directory.Exists(fullPath));
			}

			public DirectoryInfo GetInfo(string path)
			{
				var fullPath = GetLocalPath(path);
				var info = new System.IO.DirectoryInfo(fullPath);

				if(info == null || !info.Exists)
					return null;

				return new DirectoryInfo(info.FullName, info.CreationTime, info.LastWriteTime, LocalFileSystem.Instance.GetUrl(path));
			}

			public async Task<DirectoryInfo> GetInfoAsync(string path)
			{
				var fullPath = GetLocalPath(path);
				var info = await Task.Run(() => new System.IO.DirectoryInfo(fullPath));

				if(info == null || !info.Exists)
					return null;

				return new DirectoryInfo(info.FullName, info.CreationTime, info.LastWriteTime, LocalFileSystem.Instance.GetUrl(path));
			}

			public bool SetInfo(string path, IDictionary<string, string> properties)
			{
				throw new NotSupportedException();
			}

			public Task<bool> SetInfoAsync(string path, IDictionary<string, string> properties)
			{
				throw new NotSupportedException();
			}

			public IEnumerable<PathInfo> GetChildren(string path)
			{
				return this.GetChildren(path, null, false);
			}

			public IEnumerable<PathInfo> GetChildren(string path, string pattern, bool recursive = false)
			{
				var fullPath = GetLocalPath(path);
				return new InfoEnumerator<PathInfo>(System.IO.Directory.GetFileSystemEntries(fullPath, pattern, recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly));
			}

			public Task<IEnumerable<PathInfo>> GetChildrenAsync(string path)
			{
				return this.GetChildrenAsync(path, null, false);
			}

			public async Task<IEnumerable<PathInfo>> GetChildrenAsync(string path, string pattern, bool recursive = false)
			{
				var fullPath = GetLocalPath(path);
				return await Task.Run(() => new InfoEnumerator<PathInfo>(System.IO.Directory.GetFileSystemEntries(fullPath, pattern, recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly)));
			}

			public IEnumerable<DirectoryInfo> GetDirectories(string path)
			{
				return this.GetDirectories(path, null, false);
			}

			public IEnumerable<DirectoryInfo> GetDirectories(string path, string pattern, bool recursive = false)
			{
				var fullPath = GetLocalPath(path);
				return new InfoEnumerator<DirectoryInfo>(System.IO.Directory.GetDirectories(fullPath, pattern, recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly));
			}

			public Task<IEnumerable<DirectoryInfo>> GetDirectoriesAsync(string path)
			{
				return this.GetDirectoriesAsync(path, null, false);
			}

			public async Task<IEnumerable<DirectoryInfo>> GetDirectoriesAsync(string path, string pattern, bool recursive = false)
			{
				var fullPath = GetLocalPath(path);
				return await Task.Run(() => new InfoEnumerator<DirectoryInfo>(System.IO.Directory.GetDirectories(fullPath, pattern, recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly)));
			}

			public IEnumerable<FileInfo> GetFiles(string path)
			{
				return this.GetFiles(path, null, false);
			}

			public IEnumerable<FileInfo> GetFiles(string path, string pattern, bool recursive = false)
			{
				var fullPath = GetLocalPath(path);
				return new InfoEnumerator<FileInfo>(System.IO.Directory.GetFiles(fullPath, pattern, recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly));
			}

			public Task<IEnumerable<FileInfo>> GetFilesAsync(string path)
			{
				return this.GetFilesAsync(path, null, false);
			}

			public async Task<IEnumerable<FileInfo>> GetFilesAsync(string path, string pattern, bool recursive = false)
			{
				var fullPath = GetLocalPath(path);
				return await Task.Run(() => new InfoEnumerator<FileInfo>(System.IO.Directory.GetFiles(fullPath, pattern, recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly)));
			}
			#endregion

			#region 嵌套子类
			private class InfoEnumerator<T> : IEnumerable<T>, IEnumerator<T> where T : PathInfo
			{
				#region 私有字段
				private int _index;
				private string[] _items;
				#endregion

				#region 构造函数
				public InfoEnumerator(string[] items)
				{
					_items = items;
				}
				#endregion

				#region 公共成员
				public T Current
				{
					get
					{
						if(_items == null || _index < 0 || _index >= _items.Length)
							return null;

						if(typeof(T) == typeof(FileInfo))
							return LocalFileSystem.Instance.File.GetInfo(_items[_index]) as T;
						else if(typeof(T) == typeof(DirectoryInfo))
							return LocalFileSystem.Instance.Directory.GetInfo(_items[_index]) as T;
						else if(typeof(T) == typeof(PathInfo))
							return (T)new PathInfo(_items[_index], null);

						throw new InvalidOperationException();
					}
				}

				public bool MoveNext()
				{
					if(_items != null && ++_index < _items.Length)
						return true;

					return false;
				}

				public void Reset()
				{
					_index = 0;
				}
				#endregion

				#region 显式实现
				object System.Collections.IEnumerator.Current
				{
					get
					{
						return this.Current;
					}
				}

				void IDisposable.Dispose()
				{
					_index = -1;
					_items = null;
				}
				#endregion

				#region 枚举遍历
				public IEnumerator<T> GetEnumerator()
				{
					return this;
				}

				System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
				{
					return this;
				}
				#endregion
			}
			#endregion
		}

		private sealed class LocalFileProvider : IFile
		{
			#region 单例字段
			public static readonly LocalFileProvider Instance = new LocalFileProvider();
			#endregion

			#region 私有构造
			private LocalFileProvider()
			{
			}
			#endregion

			#region 公共方法
			public bool Delete(string path)
			{
				var fullPath = GetLocalPath(path);

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

			public async Task<bool> DeleteAsync(string path)
			{
				var fullPath = GetLocalPath(path);

				try
				{
					await Task.Run(() => System.IO.File.Delete(fullPath));
					return true;
				}
				catch(FileNotFoundException)
				{
					return false;
				}
			}

			public bool Exists(string path)
			{
				var fullPath = GetLocalPath(path);
				return System.IO.File.Exists(fullPath);
			}

			public async Task<bool> ExistsAsync(string path)
			{
				var fullPath = GetLocalPath(path);
				return await Task.Run(() => System.IO.File.Exists(fullPath));
			}

			public void Copy(string source, string destination)
			{
				this.Copy(source, destination, true);
			}

			public void Copy(string source, string destination, bool overwrite)
			{
				var sourcePath = GetLocalPath(source);
				var destinationPath = GetLocalPath(destination);

				System.IO.File.Copy(sourcePath, destinationPath, overwrite);
			}

			public Task CopyAsync(string source, string destination)
			{
				return this.CopyAsync(source, destination, true);
			}

			public async Task CopyAsync(string source, string destination, bool overwrite)
			{
				var sourcePath = GetLocalPath(source);
				var destinationPath = GetLocalPath(destination);

				await Task.Run(() => System.IO.File.Copy(sourcePath, destinationPath, overwrite));
			}

			public void Move(string source, string destination)
			{
				var sourcePath = GetLocalPath(source);
				var destinationPath = GetLocalPath(destination);

				System.IO.File.Move(sourcePath, destinationPath);
			}

			public async Task MoveAsync(string source, string destination)
			{
				var sourcePath = GetLocalPath(source);
				var destinationPath = GetLocalPath(destination);

				await Task.Run(() => System.IO.File.Move(sourcePath, destinationPath));
			}

			public FileInfo GetInfo(string path)
			{
				var fullPath = GetLocalPath(path);
				var info = new System.IO.FileInfo(fullPath);

				if(info == null || !info.Exists)
					return null;

				return new FileInfo(info.FullName, info.Length, info.CreationTime, info.LastWriteTime, LocalFileSystem.Instance.GetUrl(path));
			}

			public async Task<FileInfo> GetInfoAsync(string path)
			{
				var fullPath = GetLocalPath(path);
				var info = await Task.Run(() => new System.IO.FileInfo(fullPath));

				if(info == null || !info.Exists)
					return null;

				return new FileInfo(info.FullName, info.Length, info.CreationTime, info.LastWriteTime, LocalFileSystem.Instance.GetUrl(path));
			}

			public bool SetInfo(string path, IDictionary<string, string> properties)
			{
				throw new NotSupportedException();
			}

			public Task<bool> SetInfoAsync(string path, IDictionary<string, string> properties)
			{
				throw new NotSupportedException();
			}

			public Stream Open(string path, IDictionary<string, string> properties = null)
			{
				var fullPath = GetLocalPath(path);
				return System.IO.File.Open(fullPath, FileMode.Open);
			}

			public Stream Open(string path, FileMode mode, IDictionary<string, string> properties = null)
			{
				var fullPath = GetLocalPath(path);
				return System.IO.File.Open(fullPath, mode);
			}

			public Stream Open(string path, FileMode mode, FileAccess access, IDictionary<string, string> properties = null)
			{
				var fullPath = GetLocalPath(path);
				return System.IO.File.Open(fullPath, mode, access);
			}

			public Stream Open(string path, FileMode mode, FileAccess access, FileShare share, IDictionary<string, string> properties = null)
			{
				var fullPath = GetLocalPath(path);
				return System.IO.File.Open(fullPath, mode, access, share);
			}
			#endregion
		}
		#endregion
	}
}

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
using System.Text.RegularExpressions;

namespace Zongsoft.IO
{
	[Zongsoft.Services.Matcher(typeof(FileSystem.Matcher))]
	public class LocalFileSystem : IFileSystem
	{
		#region 单例字段
		public static readonly LocalFileSystem Instance = new LocalFileSystem();
		#endregion

		#region 私有构造
		private LocalFileSystem()
		{
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取本地文件目录系统的方案，始终返回“zfs.local”。
		/// </summary>
		public string Scheme
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
			if(string.IsNullOrEmpty(path))
				return null;

			return @"file:///" + GetLocalPath(path);
		}

		public string GetUrl(Path path)
		{
			if(path == null)
				return null;

			return @"file:///" + GetLocalPath(path);
		}
		#endregion

		#region 路径解析
		public static string GetLocalPath(string text)
		{
			return GetLocalPath(Path.Parse(text));
		}

		private static string GetLocalPath(Path path)
		{
			if(path == null)
				throw new ArgumentNullException(nameof(path));

			switch(Environment.OSVersion.Platform)
			{
				case PlatformID.MacOSX:
				case PlatformID.Unix:
					return path.FullPath;
			}

			var driveName = path.Scheme;
			var fullPath = path.FullPath;

			if(string.IsNullOrEmpty(driveName))
			{
				var parts = fullPath.Split('/');

				if(parts != null && parts.Length > 0)
				{
					var index = (string.IsNullOrEmpty(parts[0]) && parts.Length > 1) ? 1 : 0;
					driveName = parts[index];
					fullPath = "/" + string.Join("/", parts, index + 1, parts.Length - (index + 1));
				}

				if(string.IsNullOrWhiteSpace(driveName))
					throw new PathException(string.Format("The '{0}' path not cantians drive.", path.Url));
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
					throw new PathException(string.Format("Not matched drive for '{0}' path.", path.Url));
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
		internal sealed class LocalDirectoryProvider : IDirectory
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
			public bool Create(string path, IDictionary<string, object> properties = null)
			{
				var fullPath = GetLocalPath(path);

				if(System.IO.Directory.Exists(fullPath))
					return false;

				return System.IO.Directory.CreateDirectory(fullPath) != null;
			}

			public async Task<bool> CreateAsync(string path, IDictionary<string, object> properties = null)
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

			public bool SetInfo(string path, IDictionary<string, object> properties)
			{
				throw new NotSupportedException();
			}

			public Task<bool> SetInfoAsync(string path, IDictionary<string, object> properties)
			{
				throw new NotSupportedException();
			}

			public IEnumerable<PathInfo> GetChildren(string path)
			{
				return this.GetChildren(path, null, false);
			}

			/// <summary>
			/// 获取指定路径中与搜索模式匹配的所有文件名称和目录信息的可枚举集合，还可以搜索子目录。
			/// </summary>
			/// <param name="path">要搜索的目录。</param>
			/// <param name="pattern">用于搜索匹配的所有文件或子目录的字符串。
			///		<para>默认模式为空(null)，如果为空(null)或空字符串(“”)或“*”，即表示返回指定范围内的所有文件和目录。</para>
			///		<para>如果<paramref name="pattern"/>参数以反斜杠(“\”)或正斜杠(“/”)或竖线符(“|”)字符起始和结尾，则表示搜索模式为正则表达式，即进行正则匹配搜索；否则即为本地文件系统的匹配模式。</para>
			/// </param>
			/// <param name="recursive">指定搜索操作的范围是应仅包含当前目录还是应包含所有子目录，默认是仅包含当前目录。</param>
			/// <returns>指定搜索条件匹配的<seealso cref="PathInfo"/>集合。</returns>
			public IEnumerable<PathInfo> GetChildren(string path, string pattern, bool recursive = false)
			{
				var fullPath = GetLocalPath(path);
				var entries = this.Search(pattern, p => System.IO.Directory.EnumerateFileSystemEntries(fullPath, p, recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly));

				return new InfoEnumerator<PathInfo>(entries);
			}

			public Task<IEnumerable<PathInfo>> GetChildrenAsync(string path)
			{
				return this.GetChildrenAsync(path, null, false);
			}

			public async Task<IEnumerable<PathInfo>> GetChildrenAsync(string path, string pattern, bool recursive = false)
			{
				var fullPath = GetLocalPath(path);

				return await Task.Run(() =>
				{
					var entries = this.Search(pattern, p => System.IO.Directory.EnumerateFileSystemEntries(fullPath, p, recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly));
					return new InfoEnumerator<PathInfo>(entries);
				});
			}

			public IEnumerable<DirectoryInfo> GetDirectories(string path)
			{
				return this.GetDirectories(path, null, false);
			}

			/// <summary>
			/// 返回指定路径中与搜索模式匹配的目录信息的可枚举集合，还可以搜索子目录。
			/// </summary>
			/// <param name="path">要搜索的目录。</param>
			/// <param name="pattern">用于搜索匹配的所有子目录的字符串。
			///		<para>默认模式为空(null)，如果为空(null)或空字符串(“”)或“*”，即表示返回指定范围内的所有目录。</para>
			///		<para>如果<paramref name="pattern"/>参数以反斜杠(“\”)或正斜杠(“/”)或竖线符(“|”)字符起始和结尾，则表示搜索模式为正则表达式，即进行正则匹配搜索；否则即为本地文件系统的匹配模式。</para>
			/// </param>
			/// <param name="recursive">指定搜索操作的范围是应仅包含当前目录还是应包含所有子目录，默认是仅包含当前目录。</param>
			/// <returns>指定搜索条件匹配的<seealso cref="DirectoryInfo"/>集合。</returns>
			public IEnumerable<DirectoryInfo> GetDirectories(string path, string pattern, bool recursive = false)
			{
				var fullPath = GetLocalPath(path);
				var entries = this.Search(pattern, p => System.IO.Directory.EnumerateDirectories(fullPath, p, recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly));

				return new InfoEnumerator<DirectoryInfo>(entries);
			}

			public Task<IEnumerable<DirectoryInfo>> GetDirectoriesAsync(string path)
			{
				return this.GetDirectoriesAsync(path, null, false);
			}

			public async Task<IEnumerable<DirectoryInfo>> GetDirectoriesAsync(string path, string pattern, bool recursive = false)
			{
				var fullPath = GetLocalPath(path);

				return await Task.Run(() =>
				{
					var entries = this.Search(pattern, p => System.IO.Directory.EnumerateDirectories(fullPath, p, recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly));
					return new InfoEnumerator<DirectoryInfo>(entries);
				});
			}

			public IEnumerable<FileInfo> GetFiles(string path)
			{
				return this.GetFiles(path, null, false);
			}

			/// <summary>
			/// 返回指定路径中与搜索模式匹配的文件信息的可枚举集合，还可以搜索子目录。
			/// </summary>
			/// <param name="path">要搜索的目录。</param>
			/// <param name="pattern">用于搜索匹配的所有文件的字符串。
			///		<para>默认模式为空(null)，如果为空(null)或空字符串(“”)或“*”，即表示返回指定范围内的所有文件。</para>
			///		<para>如果<paramref name="pattern"/>参数以反斜杠(“\”)或正斜杠(“/”)或竖线符(“|”)字符起始和结尾，则表示搜索模式为正则表达式，即进行正则匹配搜索；否则即为本地文件系统的匹配模式。</para>
			/// </param>
			/// <param name="recursive">指定搜索操作的范围是应仅包含当前目录还是应包含所有子目录，默认是仅包含当前目录。</param>
			/// <returns>指定搜索条件匹配的<seealso cref="FileInfo"/>集合。</returns>
			public IEnumerable<FileInfo> GetFiles(string path, string pattern, bool recursive = false)
			{
				var fullPath = GetLocalPath(path);
				var entries = this.Search(pattern, p => System.IO.Directory.EnumerateFiles(fullPath, p, recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly));

				return new InfoEnumerator<FileInfo>(entries);
			}

			public Task<IEnumerable<FileInfo>> GetFilesAsync(string path)
			{
				return this.GetFilesAsync(path, null, false);
			}

			public async Task<IEnumerable<FileInfo>> GetFilesAsync(string path, string pattern, bool recursive = false)
			{
				var fullPath = GetLocalPath(path);

				return await Task.Run(() =>
				{
					var entries = this.Search(pattern, p => System.IO.Directory.EnumerateFiles(fullPath, p, recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly));
					return new InfoEnumerator<FileInfo>(entries);
				});
			}
			#endregion

			#region 私有方法
			private static readonly Regex _regex = new Regex(@"(?<delimiter>[/\|\\]).+\k<delimiter>", RegexOptions.Compiled | RegexOptions.ExplicitCapture);

			private IEnumerable<string> Search(string pattern, Func<string, IEnumerable<string>> searcher, Predicate<string> filter = null)
			{
				var regularPattern = string.Empty;
				IEnumerable<string> result = null;

				if(string.IsNullOrEmpty(pattern))
				{
					result = searcher("*");
				}
				else
				{
					var matches = _regex.Matches(pattern);

					if(matches == null || matches.Count <= 0)
					{
						result = searcher(pattern);
					}
					else
					{
						var position = 0;

						foreach(Match match in matches)
						{
							if(match.Index > 0)
								regularPattern += EscapePattern(pattern.Substring(position, match.Index - position));

							regularPattern += match.Value.Substring(1, match.Value.Length - 2);

							//移动当前指针位置
							position = match.Index + match.Length;
						}

						if(position < pattern.Length)
							regularPattern += EscapePattern(pattern.Substring(position));

						result = searcher("*");

						//设置正则匹配模式为完整匹配
						if(regularPattern != null && regularPattern.Length > 0)
							regularPattern = "^" + regularPattern + "$";
					}
				}

				if(result == null)
					yield break;

				foreach(var item in result)
				{
					if(string.IsNullOrEmpty(regularPattern))
					{
						if(filter == null || filter(item))
							yield return item;
					}
					else
					{
						var fileName = System.IO.Path.GetFileName(item);

						if(Regex.IsMatch(fileName, regularPattern, RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture) && (filter == null || filter(item)))
							yield return item;
					}
				}
			}

			internal static string EscapePattern(string pattern)
			{
				if(string.IsNullOrWhiteSpace(pattern))
					return pattern;

				string result = string.Empty;

				foreach(var chr in pattern)
				{
					switch(chr)
					{
						case '.':
						case '^':
						case '*':
						case '?':
						case '+':
						case '-':
						case '|':
						case '\\':
						case '(':
						case ')':
						case '[':
						case ']':
						case '{':
						case '}':
						case '<':
						case '>':
							result += ("\\" + chr);
							break;
						default:
							result += chr;
							break;
					}
				}

				return result;
			}
			#endregion

			#region 嵌套子类
			private class InfoEnumerator<T> : IEnumerable<T>, IEnumerator<T> where T : PathInfo
			{
				#region 私有字段
				private IEnumerator<string> _source;
				#endregion

				#region 构造函数
				public InfoEnumerator(IEnumerable<string> source)
				{
					_source = source == null ? null : source.GetEnumerator();
				}
				#endregion

				#region 公共成员
				public T Current
				{
					get
					{
						if(_source == null)
							return null;

						var item = _source.Current;

						if(string.IsNullOrEmpty(item))
							return null;

						if(typeof(T) == typeof(FileInfo))
							return LocalFileSystem.Instance.File.GetInfo(item) as T;
						else if(typeof(T) == typeof(DirectoryInfo))
							return LocalFileSystem.Instance.Directory.GetInfo(item) as T;
						else if(typeof(T) == typeof(PathInfo))
							return (T)new PathInfo(item);

						throw new InvalidOperationException();
					}
				}

				public bool MoveNext()
				{
					if(_source != null)
						return _source.MoveNext();

					return false;
				}

				public void Reset()
				{
					if(_source != null)
						_source.Reset();
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
					_source = null;
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

			[Obsolete]
			private class InfoEnumeratorObsoleted<T> : IEnumerable<T>, IEnumerator<T> where T : PathInfo
			{
				#region 私有字段
				private int _index;
				private string[] _items;
				#endregion

				#region 构造函数
				public InfoEnumeratorObsoleted(string[] items)
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

			public bool SetInfo(string path, IDictionary<string, object> properties)
			{
				throw new NotSupportedException();
			}

			public Task<bool> SetInfoAsync(string path, IDictionary<string, object> properties)
			{
				throw new NotSupportedException();
			}

			public Stream Open(string path, IDictionary<string, object> properties = null)
			{
				var fullPath = GetLocalPath(path);
				return System.IO.File.Open(fullPath, FileMode.Open);
			}

			public Stream Open(string path, FileMode mode, IDictionary<string, object> properties = null)
			{
				var fullPath = GetLocalPath(path);
				return System.IO.File.Open(fullPath, mode);
			}

			public Stream Open(string path, FileMode mode, FileAccess access, IDictionary<string, object> properties = null)
			{
				var fullPath = GetLocalPath(path);
				return System.IO.File.Open(fullPath, mode, access);
			}

			public Stream Open(string path, FileMode mode, FileAccess access, FileShare share, IDictionary<string, object> properties = null)
			{
				var fullPath = GetLocalPath(path);
				return System.IO.File.Open(fullPath, mode, access, share);
			}
			#endregion
		}
		#endregion
	}
}

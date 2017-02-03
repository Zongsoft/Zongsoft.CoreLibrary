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
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Zongsoft.IO
{
	/// <summary>
	/// 表示不依赖操作系统的路径。
	/// </summary>
	/// <remarks>
	///		<para>路径格式分为<seealso cref="Path.Scheme"/>和<seealso cref="Path.FullPath"/>这两个部分，中间使用冒号(:)分隔，路径各层级间使用正斜杠(/)进行分隔。如果是目录路径则以正斜杠(/)结尾。</para>
	///		<para>其中<seealso cref="Path.Scheme"/>可以省略，如果为目录路径，则<see cref="Path.FileName"/>属性为空或空字符串("")。常用路径示例如下：</para>
	///		<list type="bullet">
	///			<item>
	///				<term>某个文件的<see cref="Url"/>：zfs:/data/attachments/2014/07/file-name.ext</term>
	///			</item>
	///			<item>
	///				<term>某个本地文件的<see cref="Url"/>：zfs.local:/data/attachments/2014/07/file-name.ext</term>
	///			</item>
	///			<item>
	///				<term>某个分布式文件的<see cref="Url"/>：zfs.distributed:/data/attachments/file-name.ext</term>
	///			</item>
	///			<item>
	///				<term>某个目录的<see cref="Url"/>：zfs:/data/attachments/2014/07/</term>
	///			</item>
	///			<item>
	///				<term>未指定模式(Scheme)的<see cref="Url"/>：/data/attachements/images/</term>
	///			</item>
	///		</list>
	/// </remarks>
	public sealed class Path
	{
		#region 常量定义
		private const string SCHEME_REGEX = @"\s*((?<scheme>[A-Za-z]+(\.[A-Za-z_\-]+)?):)?";
		private const string PATH_REGEX = @"(?<path>(?<part>[/\\][^/\\\*\?:]+)*(?<part>[/\\])?)\s*";
		#endregion

		#region 私有变量
		private static readonly Regex _regex = new Regex("^" + SCHEME_REGEX + PATH_REGEX + "$", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
		#endregion

		#region 成员字段
		private string _originalString;
		private string _scheme;
		private string _fullPath;
		private string _directoryName;
		private string _fileName;
		#endregion

		#region 构造函数
		private Path(string originalString, string scheme, string fullPath)
		{
			if(string.IsNullOrWhiteSpace(originalString))
				throw new ArgumentNullException("originalString");

			_originalString = originalString.Trim();
			_scheme = string.IsNullOrWhiteSpace(scheme) ? null : scheme.Trim().ToLowerInvariant();

			if(string.IsNullOrWhiteSpace(fullPath))
			{
				_fullPath = "/";
				_directoryName = "/";
				_fileName = string.Empty;

				return;
			}

			_fullPath = fullPath.Trim();

			var parts = _fullPath.Split('/', '\\');
			_fileName = parts[parts.Length - 1];
			_directoryName = string.Join("/", parts, 0, parts.Length - 1) + "/";
		}
		#endregion

		#region 公共属性
		public string Scheme
		{
			get
			{
				return _scheme;
			}
		}

		public string DirectoryName
		{
			get
			{
				return _directoryName;
			}
		}

		public string FileName
		{
			get
			{
				return _fileName;
			}
		}

		public string FullPath
		{
			get
			{
				return _fullPath;
			}
		}

		public string Url
		{
			get
			{
				return string.IsNullOrEmpty(_scheme) ? _fullPath : (_scheme + ":" + _fullPath);
			}
		}

		public bool IsFile
		{
			get
			{
				return !string.IsNullOrEmpty(_fileName);
			}
		}

		public bool IsDirectory
		{
			get
			{
				return string.IsNullOrEmpty(_fileName);
			}
		}
		#endregion

		#region 重写方法
		public override string ToString()
		{
			if(string.IsNullOrWhiteSpace(_scheme))
				return _fullPath;
			else
				return _scheme + ":" + _fullPath;
		}

		public override int GetHashCode()
		{
			switch(Environment.OSVersion.Platform)
			{
				case PlatformID.MacOSX:
				case PlatformID.Unix:
					return (_scheme + _fullPath).GetHashCode();
				default:
					return (_scheme + _fullPath.ToLowerInvariant()).GetHashCode();
			}
		}

		public override bool Equals(object obj)
		{
			if(obj == null || obj.GetType() != this.GetType())
				return false;

			var other = (Path)obj;

			if(!string.Equals(_scheme, other._scheme))
				return false;

			switch(Environment.OSVersion.Platform)
			{
				case PlatformID.MacOSX:
				case PlatformID.Unix:
					return string.Equals(_fullPath, other._fullPath, StringComparison.Ordinal);
				default:
					return string.Equals(_fullPath, other._fullPath, StringComparison.OrdinalIgnoreCase);
			}
		}
		#endregion

		#region 静态方法
		/// <summary>
		/// 解析文本格式的路径。
		/// </summary>
		/// <param name="text">要解析的路径文本。</param>
		/// <returns>返回解析成功的<see cref="Path"/>路径对象。</returns>
		/// <exception cref="ArgumentNullException">当<paramref name="text"/>参数为空或空白字符串。</exception>
		/// <exception cref="PathException">当<paramref name="text"/>参数为无效的路径格式。</exception>
		public static Path Parse(string text)
		{
			if(string.IsNullOrWhiteSpace(text))
				throw new ArgumentNullException("text");

			string scheme, path;

			if(TryParse(text, out scheme, out path))
				return new Path(text, scheme, path);

			throw new PathException(text);
		}

		/// <summary>
		/// 尝试解析文本格式的路径。
		/// </summary>
		/// <param name="text">要解析的路径文本。</param>
		/// <param name="result">解析成功的<see cref="Path"/>路径对象。</param>
		/// <returns>如果解析成功则返回真(True)，否则返回假(False)。</returns>
		public static bool TryParse(string text, out Path result)
		{
			result = null;
			string scheme, path;

			if(TryParse(text, out scheme, out path))
			{
				result = new Path(text, scheme, path);
				return true;
			}

			return false;
		}

		public static bool TryParse(string text, out string scheme, out string path)
		{
			scheme = null;
			path = null;

			if(string.IsNullOrWhiteSpace(text))
				return false;

			var match = _regex.Match(text);

			if(match.Success)
			{
				scheme = string.IsNullOrWhiteSpace(match.Groups["scheme"].Value) ? null : match.Groups["scheme"].Value;
				path = match.Groups["path"].Value.Replace('\\', '/');
			}

			return match.Success;
		}

		/// <summary>
		/// 解析路径文本中指定的方案名。
		/// </summary>
		/// <param name="text">指定的路径文本。</param>
		/// <returns>返回的路径方案名，如果解析失败或者路径文本未包含方案则返回空。</returns>
		public static string GetScheme(string text)
		{
			if(string.IsNullOrWhiteSpace(text))
				return null;

			var match = _regex.Match(text);

			if(match.Success)
				return string.IsNullOrWhiteSpace(match.Groups["scheme"].Value) ? null : match.Groups["scheme"].Value;

			return null;
		}

		public static string GetPath(string text)
		{
			if(string.IsNullOrWhiteSpace(text))
				return null;

			var match = _regex.Match(text);

			if(match.Success)
				return match.Groups["path"].Value.Replace('\\', '/');

			return null;
		}

		/// <summary>
		/// 将字符串数组组合成一个路径。
		/// </summary>
		/// <param name="parts">由路径的各部分构成的数组。</param>
		/// <returns>组合后的路径。</returns>
		/// <remarks>
		///		<para>该方法支持连接字符串中相对路径的解析处理，并自动忽略每个路径节两边的空白字符。如下代码所示：</para>
		///		<code><![CDATA[
		///		Path.Combine(@"D:\data\images\", "avatars/001.jpg");                            // D:/data/images/avatars/001.jpg
		///		Path.Combine(@"D:\data\images\", "./avatars / 001.jpg");                        // D:/data/images/avatars/001.jpg
		///		Path.Combine(@"D:\data\images\", ".. /avatars / 001.jpg");                      // D:/data/avatars/001.jpg
		///		Path.Combine(@"D:\data\images\", "/avatars/001.jpg");                           // /avatars/001.jpg
		///		Path.Combine(@"D:\data\images\", "avatars / 001.jpg", " / final.ext");          // /final.ext
		///		Path.Combine(@"D:\data\images\", "avatars / 001.jpg", " / final.ext", " tail")  // /final.ext/tail
		///		Path.Combine(@"zfs.local:/data/images/", "./bin");                              // zfs.local:/data/images/bin
		///		Path.Combine(@"zfs.local:/data/images/", "../bin/Debug");                       // zfs.local:/data/bin/Debug
		///		Path.Combine(@"zfs.local:/data/images/", "./bin", "../bin/Debug");              // zfs.local:/data/images/bin/Debug
		///		Path.Combine(@"zfs.local:/data/images/", "/root");                              // /root
		///		Path.Combine(@"zfs.local:/data/images/", "./bin", "../bin/Debug", "/root/");    // /root/
		///		]]></code>
		/// </remarks>
		public static string Combine(params string[] parts)
		{
			if(parts == null)
				throw new ArgumentNullException(nameof(parts));

			var slashed = false;
			var segments = new List<string>();

			for(int i = 0; i < parts.Length; i++)
			{
				if(string.IsNullOrEmpty(parts[i]))
					continue;

				var part = string.Empty;
				var spaces = 0;

				foreach(var chr in parts[i])
				{
					switch(chr)
					{
						case ' ':
							if(part.Length > 0)
								spaces++;
							break;
						case '\t':
						case '\n':
						case '\r':
							break;
						case '/':
						case '\\':
							spaces = 0;
							slashed = true;

							switch(part)
							{
								case "":
									segments.Clear();
									segments.Add("");
									break;
								case ".":
									break;
								case "..":
									if(segments.Count > 0)
										segments.RemoveAt(segments.Count - 1);
									break;
								default:
									if(part.Contains(":"))
										segments.Clear();

									segments.Add(part);
									break;
							}

							part = string.Empty;
							break;
						//注意：忽略对“?”、“*”字符的检验处理，因为需要支持对通配符模式路径的链接。
						//case '?':
						//case '*':
						case '"':
						case '|':
						case '<':
						case '>':
							throw new ArgumentException("Invalid path, it contains a illegal character.");
						default:
							if(spaces > 0)
							{
								part += new string(' ', spaces);
								spaces = 0;
							}

							part += chr;
							slashed = false;
							break;
					}
				}

				if(part.Length > 0)
				{
					switch(part)
					{
						case ".":
							break;
						case "..":
							if(segments.Count > 0)
								segments.RemoveAt(segments.Count - 1);
							break;
						default:
							if(part.Contains(":"))
								segments.Clear();

							segments.Add(part);
							break;
					}
				}
			}

			return (segments.Count == 0 ? string.Empty : string.Join("/", segments)) + (slashed ? "/" : "");
		}
		#endregion
	}
}

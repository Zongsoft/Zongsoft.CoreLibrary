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
	///		<para>路径格式分为<seealso cref="DirectoryInfo.Schema"/>和<seealso cref="DirectoryInfo.FullPath"/>这两个部分，其中 Schema 与 Path 中间使用冒号(:)分隔，路径分层使用正斜杠(/)分隔。如果是目录的话则应该以正斜杠结尾。路径示例如下：</para>
	///		<list type="bullet">
	///			<item>
	///				<term>某个文件：zfs:/data/attachments/2014/07/file-name.ext</term>
	///			</item>
	///			<item>
	///				<term>某个本地文件：zfs.local:/data/attachments/2014/07/file-name.ext</term>
	///			</item>
	///			<item>
	///				<term>某个分布式文件：zfs.distributed:/data/attachments/file-name.ext</term>
	///			</item>
	///			<item>
	///				<term>某个目录：zfs:/data/attachments/2014/07/</term>
	///			</item>
	///		</list>
	/// </remarks>
	public class Path
	{
		#region 私有变量
		private static readonly Regex _regex = new Regex(@"(?<schema>\w+(\.\w+)?)\:(?<path>(?<part>/[^/\\\*\?]+)+/?)", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
		#endregion

		#region 成员字段
		private string _originalString;
		private string _schema;
		private string _fullPath;
		private string _directoryName;
		private string _fileName;
		#endregion

		#region 构造函数
		private Path(string originalString, string schema, string fullPath)
		{
			if(string.IsNullOrWhiteSpace(originalString))
				throw new ArgumentNullException("originalString");

			if(string.IsNullOrWhiteSpace(schema))
				throw new ArgumentNullException("schema");

			if(string.IsNullOrWhiteSpace(fullPath))
				throw new ArgumentNullException("fullPath");

			_originalString = originalString.Trim();
			_schema = schema.Trim().ToLowerInvariant();
			_fullPath = fullPath.Trim();

			var parts = fullPath.Split('/');

			if(fullPath.EndsWith("/"))
			{
				_fileName = string.Empty;
				_directoryName = _fullPath;
			}
			else
			{
				_fileName = parts[parts.Length - 1].Trim();
				_directoryName = string.Join("/", parts, 0, parts.Length - 1) + "/";
			}
		}
		#endregion

		#region 公共属性
		public string Schema
		{
			get
			{
				return _schema;
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
		#endregion

		#region 重写方法
		public override string ToString()
		{
			if(string.IsNullOrWhiteSpace(_schema))
				return _fullPath;
			else
				return _schema + ":" + _originalString;
		}

		public override int GetHashCode()
		{
			switch(Environment.OSVersion.Platform)
			{
				case PlatformID.MacOSX:
				case PlatformID.Unix:
					return (_schema + _fullPath).GetHashCode();
				default:
					return (_schema + _fullPath.ToLowerInvariant()).GetHashCode();
			}
		}

		public override bool Equals(object obj)
		{
			if(obj == null || obj.GetType() != this.GetType())
				return false;

			var other = (Path)obj;

			if(!string.Equals(_schema, other._schema))
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
		/// <param name="path">要解析的路径文本。</param>
		/// <returns>返回解析成功的<see cref="DirectoryName"/>路径对象。</returns>
		/// <exception cref="ArgumentNullException">当<paramref name="path"/>参数为空或空白字符串。</exception>
		/// <exception cref="PathException">当<paramref name="path"/>参数为无效的路径格式。</exception>
		public static Path Parse(string path)
		{
			if(string.IsNullOrWhiteSpace(path))
				throw new ArgumentNullException("path");

			var match = _regex.Match(path);

			if(match.Success)
				return new Path(path, match.Groups["schema"].Value, match.Groups["path"].Value);

			throw new PathException(path);
		}

		/// <summary>
		/// 尝试解析文本格式的路径。
		/// </summary>
		/// <param name="path">要解析的路径文本。</param>
		/// <param name="result">解析成功的<see cref="DirectoryName"/>路径对象。</param>
		/// <returns>如果解析成功则返回真(True)，否则返回假(False)。</returns>
		public static bool TryParse(string path, out Path result)
		{
			result = null;

			if(string.IsNullOrWhiteSpace(path))
				return false;

			var match = _regex.Match(path);

			if(match.Success)
				result = new Path(path, match.Groups["schema"].Value, match.Groups["path"].Value);

			return match.Success;
		}

		public static bool TryParse(string text, out string schema, out string path)
		{
			schema = string.Empty;
			path = string.Empty;

			if(string.IsNullOrWhiteSpace(text))
				return false;

			var match = _regex.Match(text);

			if(match.Success)
			{
				schema = match.Groups["schema"].Value;
				path = match.Groups["path"].Value;
			}

			return match.Success;
		}

		public static string GetSchema(string path)
		{
			if(string.IsNullOrWhiteSpace(path))
				return string.Empty;

			var match = _regex.Match(path);

			if(match.Success)
				return match.Groups["schema"].Value;

			return string.Empty;
		}

		public static string Combine(params string[] parts)
		{
			if(parts == null || parts.Length == 0)
				throw new ArgumentNullException("parts");

			string result = string.Empty;

			for(int i = 0; i < parts.Length; i++)
			{
				if(string.IsNullOrWhiteSpace(parts[i]))
					continue;

				var part = parts[i].Replace('\\', '/');

				if(result.Length == 0)
					result = part;
				else
				{
					if(result.EndsWith("/"))
						result += part.TrimStart('/');
					else
						result += "/" + part.TrimStart('/');
				}
			}

			return result;
		}
		#endregion
	}
}

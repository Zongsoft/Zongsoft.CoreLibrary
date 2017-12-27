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
		#region 成员字段
		private string _scheme;
		private string _fullPath;
		private string _directoryName;
		private string _fileName;
		private string[] _segments;
		private PathAnchor _anchor;
		#endregion

		#region 私有构造
		private Path(string scheme, string[] segments, PathAnchor anchor)
		{
			_scheme = scheme;
			_segments = segments;
			_anchor = anchor;

			//计算并保存路径全称
			_fullPath = GetAnchorString(anchor, true) + string.Join("/", segments);

			if(segments.Length == 0)
			{
				_fileName = string.Empty;
				_directoryName = GetAnchorString(anchor, true);
			}
			else
			{
				if(string.IsNullOrEmpty(segments[segments.Length - 1]))
				{
					_fileName = string.Empty;
					_directoryName = _fullPath;
				}
				else
				{
					_fileName = segments[segments.Length - 1];

					if(segments.Length == 1)
						_directoryName = GetAnchorString(anchor, true);
					else
						_directoryName = GetAnchorString(anchor, true) + string.Join("/", segments, 0, segments.Length - 1) + "/";
				}
			}
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取路径的文件系统<see cref="IFileSystem.Scheme"/>方案。
		/// </summary>
		public string Scheme
		{
			get
			{
				return _scheme;
			}
		}

		/// <summary>
		/// 获取路径的锚点，即路径的起始点。
		/// </summary>
		public PathAnchor Anchor
		{
			get
			{
				return _anchor;
			}
		}

		/// <summary>
		/// 获取路径中的文件名，有关路径中文件名的定义请参考备注说明。
		/// </summary>
		/// <remarks>
		///		<para>路径如果以斜杠(/)结尾，则表示该路径为「目录路径」，即<see cref="FileName"/>属性为空(null)或空字符串("")；否则文件名则为<see cref="Segments"/>路径节数组中的最后一个节的内容。</para>
		/// </remarks>
		public string FileName
		{
			get
			{
				return _fileName;
			}
		}

		/// <summary>
		/// 获取路径的完整路径（注：不含<see cref="Scheme"/>部分）。
		/// </summary>
		public string FullPath
		{
			get
			{
				return _fullPath;
			}
		}

		/// <summary>
		/// 获取路径中的目录名，目录名不含<see cref="Scheme"/>部分。
		/// </summary>
		public string DirectoryName
		{
			get
			{
				return _directoryName;
			}
		}

		/// <summary>
		/// 获取路径中的目录URL，该属性值包含<see cref="Scheme"/>和<see cref="DirectoryName"/>。
		/// </summary>
		/// <remarks>
		///		<para>如果<see cref="Scheme"/>为空(null)或空字符串("")，则<see cref="DirectoryUrl"/>与<see cref="DirectoryName"/>属性值相同。</para>
		/// </remarks>
		public string DirectoryUrl
		{
			get
			{
				return string.IsNullOrEmpty(_scheme) ? _directoryName : (_scheme + ":" + _directoryName);
			}
		}

		/// <summary>
		/// 获取路径的完整URL，该属性值包含<see cref="Scheme"/>和<see cref="FullPath"/>。
		/// </summary>
		/// <remarks>
		///		<para>如果<see cref="Scheme"/>为空(null)或空字符串("")，则<see cref="Url"/>与<see cref="FullPath"/>属性值相同。</para>
		/// </remarks>
		public string Url
		{
			get
			{
				return string.IsNullOrEmpty(_scheme) ? _fullPath : (_scheme + ":" + _fullPath);
			}
		}

		/// <summary>
		/// 获取路径中各节点数组，更多内容请参考备注说明。
		/// </summary>
		/// <remarks>
		///		<para>如果当前路径是一个「文件路径」，即<see cref="IsFile"/>属性为真(True)，则该数组的最后一个元素内容就是<see cref="FileName"/>的值，亦文件路径的<see cref="Segments"/>不可能为空数组，因为它至少包含一个为文件名的元素。</para>
		///		<para>如果当前路径是一个「目录路径」，即<see cref="IsDirectory"/>属性为真(True)，并且不是空目录，则该数组的最后一个元素值为空(null)或空字符串("")。所谓“空目录”的示例如下：</para>
		///		<list type="bullet">
		///			<item>空目录：scheme:/</item>
		///			<item>空目录：scheme:./</item>
		///			<item>空目录：scheme:../</item>
		///			<item>非空目录：scheme:root/</item>
		///			<item>非空目录：scheme:root/directory/</item>
		///			<item>非空目录：scheme:/root/</item>
		///			<item>非空目录：scheme:/root/directory/</item>
		///			<item>非空目录：scheme:./root/</item>
		///			<item>非空目录：scheme:./root/directory/</item>
		///			<item>非空目录：scheme:../root/</item>
		///			<item>非空目录：scheme:../root/directory/</item>
		///		</list>
		/// </remarks>
		public string[] Segments
		{
			get
			{
				return _segments;
			}
		}

		/// <summary>
		/// 获取一个值，指示当前路径是否为文件路径。如果返回真(True)，即表示<see cref="FileName"/>有值。
		/// </summary>
		/// <remarks>
		///		<para>路径如果不是以斜杠(/)结尾，则表示该路径为「文件路径」，文件路径中的<see cref="FileName"/>即为<see cref="Segments"/>数组中最后一个元素的值。</para>
		/// </remarks>
		public bool IsFile
		{
			get
			{
				return !string.IsNullOrEmpty(_fileName);
			}
		}

		/// <summary>
		/// 获取一个值，指示当前路径是否为目录路径。有关「目录路径」定义请参考备注说明。
		/// </summary>
		/// <remarks>
		///		<para>路径如果以斜杠(/)结尾，则表示该路径为「目录路径」，即<see cref="FileName"/>属性为空(null)或空字符串("")；否则文件名则为<see cref="Segments"/>路径节数组中的最后一个节的内容。</para>
		/// </remarks>
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
			return this.Url;
		}

		public override int GetHashCode()
		{
			return this.Url.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if(obj == null || obj.GetType() != this.GetType())
				return false;

			return string.Equals(this.Url, ((Path)obj).Url);
		}
		#endregion

		#region 静态方法
		/// <summary>
		/// 解析路径。
		/// </summary>
		/// <param name="text">要解析的路径文本。</param>
		/// <returns>返回解析成功的<see cref="Path"/>路径对象。</returns>
		/// <exception cref="PathException">当<paramref name="text"/>参数为无效的路径格式。</exception>
		public static Path Parse(string text)
		{
			string scheme;
			string[] segments;
			PathAnchor anchor;

			//解析路径文本，并确保无效的路径文本格式会触发异常
			ParseCore(text, true, out scheme, out segments, out anchor);

			//返回解析成功后的路径对象
			return new Path(scheme, segments, anchor);
		}

		/// <summary>
		/// 尝试解析路径。
		/// </summary>
		/// <param name="text">要解析的路径文本。</param>
		/// <param name="path">解析成功的<see cref="Path"/>路径对象。</param>
		/// <returns>如果解析成功则返回真(True)，否则返回假(False)。</returns>
		public static bool TryParse(string text, out Path path)
		{
			//设置输出参数的默认值
			path = null;

			string scheme;
			string[] segments;
			PathAnchor anchor;

			if(ParseCore(text, false, out scheme, out segments, out anchor))
			{
				path = new Path(scheme, segments, anchor);
				return true;
			}

			return false;
		}

		/// <summary>
		/// 尝试解析路径。
		/// </summary>
		/// <param name="text">指定要解析的路径文本。</param>
		/// <param name="scheme">返回解析成功的路径对应的文件系统<see cref="IFileSystem.Scheme"/>方案。</param>
		/// <param name="path">返回解析成功的完整路径，更多信息请参考<see cref="Path.FullPath"/>属性文档。</param>
		/// <returns>如果解析成功则返回真(True)，否则返回假(False)。</returns>
		public static bool TryParse(string text, out string scheme, out string path)
		{
			PathAnchor anchor;
			return TryParse(text, out scheme, out path, out anchor);
		}

		/// <summary>
		/// 尝试解析路径。
		/// </summary>
		/// <param name="text">指定要解析的路径文本。</param>
		/// <param name="scheme">返回解析成功的路径对应的文件系统<see cref="IFileSystem.Scheme"/>方案。</param>
		/// <param name="path">返回解析成功的完整路径，更多信息请参考<see cref="Path.FullPath"/>属性文档。</param>
		/// <param name="anchor">返回解析成功的路径锚点。</param>
		/// <returns>如果解析成功则返回真(True)，否则返回假(False)。</returns>
		public static bool TryParse(string text, out string scheme, out string path, out PathAnchor anchor)
		{
			path = null;
			string[] segments;

			if(ParseCore(text, false, out scheme, out segments, out anchor))
			{
				path = GetAnchorString(anchor, true) + string.Join("/", segments);
				return true;
			}

			return false;
		}

		/// <summary>
		/// 尝试解析路径。
		/// </summary>
		/// <param name="text">指定要解析的路径文本。</param>
		/// <param name="scheme">返回解析成功的路径对应的文件系统<see cref="IFileSystem.Scheme"/>方案。</param>
		/// <param name="segments">返回解析成功的路径节点数组，更多信息请参考<see cref="Path.Segments"/>属性文档。</param>
		/// <param name="anchor">返回解析成功的路径锚点。</param>
		/// <returns>如果解析成功则返回真(True)，否则返回假(False)。</returns>
		public static bool TryParse(string text, out string scheme, out string[] segments, out PathAnchor anchor)
		{
			return ParseCore(text, false, out scheme, out segments, out anchor);
		}

		/// <summary>
		/// 解析路径。
		/// </summary>
		/// <param name="text">指定要解析的路径文本。</param>
		/// <param name="throwException">指定无效的路径文本是否激发异常。</param>
		/// <param name="scheme">返回解析成功的路径对应的文件系统<see cref="IFileSystem.Scheme"/>方案。</param>
		/// <param name="segments">返回解析成功的路径节点数组，更多信息请参考<see cref="Path.Segments"/>属性文档。</param>
		/// <param name="anchor">返回解析成功的路径锚点。</param>
		/// <returns>如果解析成功则返回真(True)，否则返回假(False)。</returns>
		private static bool ParseCore(string text, bool throwException, out string scheme, out string[] segments, out PathAnchor anchor)
		{
			const int PATH_NONE_STATE = 0;      //状态机：初始态
			const int PATH_SLASH_STATE = 1;     //状态机：斜杠态（路径分隔符）
			const int PATH_ANCHOR_STATE = 2;    //状态机：锚点态
			const int PATH_SEGMENT_STATE = 3;   //状态机：内容态

			scheme = null;
			segments = null;
			anchor = PathAnchor.None;

			if(string.IsNullOrEmpty(text))
			{
				if(throwException)
					throw new PathException("The path text is null or empty.");

				return false;
			}

			var state = 0;
			var spaces = 0;
			var part = string.Empty;
			var parts = new List<string>();

			for(int i=0; i<text.Length; i++)
			{
				var chr = text[i];

				switch(chr)
				{
					case ' ':
						if(state == PATH_ANCHOR_STATE && anchor == PathAnchor.Current)
						{
							if(throwException)
								throw new PathException("");

							return false;
						}

						if(part.Length > 0)
							spaces++;

						break;
					case '\t':
					case '\n':
					case '\r':
						break;
					case ':':
						//注意：当首次遇到冒号时，其为Scheme定语；否则即为普通字符
						if(parts.Count == 0)
						{
							if(string.IsNullOrEmpty(part))
							{
								if(throwException)
									throw new PathException("The scheme of path is empty.");

								return false;
							}

							//设置路径方案
							scheme = part;

							//重置空格计数器
							spaces = 0;

							//重置内容文本
							part = string.Empty;

							//设置当前状态为初始态
							state = PATH_NONE_STATE;
						}
						else
						{
							//跳转到默认分支，即做普通字符处理
							goto default;
						}

						break;
					case '.':
						switch(state)
						{
							case PATH_NONE_STATE:
								anchor = PathAnchor.Current;
								break;
							case PATH_ANCHOR_STATE:
								if(anchor == PathAnchor.Current)
								{
									anchor = PathAnchor.Parent;
								}
								else
								{
									if(throwException)
										throw new PathException("Invalid anchor of path.");

									return false;
								}

								break;
							default:
								goto TEXT_LABEL;
						}

						state = PATH_ANCHOR_STATE;

						break;
					case '/':
					case '\\':
						switch(state)
						{
							case PATH_NONE_STATE:
								anchor = PathAnchor.Root;
								break;
							case PATH_SLASH_STATE:
								if(throwException)
									throw new PathException("Invalid path text, it contains repeated slash character.");

								return false;
							case PATH_SEGMENT_STATE:
								if(string.IsNullOrEmpty(part))
								{
									if(throwException)
										throw new PathException("Error occurred, The path parser internal error.");

									return false;
								}

								parts.Add(part);

								break;
						}

						spaces = 0;
						part = string.Empty;
						state = PATH_SLASH_STATE;

						break;
					//注意：忽略对“?”、“*”字符的检验处理，因为需要支持对通配符模式路径的链接。
					//case '?':
					//case '*':
					case '"':
					case '|':
					case '<':
					case '>':
						if(throwException)
							throw new ArgumentException(string.Format("Invalid path, it contains '{0}' illegal character(s).", chr));

						return false;
					default:
TEXT_LABEL:
						if(spaces > 0)
						{
							part += new string(' ', spaces);
							spaces = 0;
						}

						part += chr;
						state = PATH_SEGMENT_STATE;

						break;
				}
			}

			if(parts.Count == 0 && anchor == PathAnchor.None)
			{
				if(throwException)
					throw new PathException("The path text is all whitespaces.");

				return false;
			}

			if(state == PATH_SEGMENT_STATE && part.Length > 0)
				parts.Add(part);

			segments = new string[parts.Count + (state == PATH_SLASH_STATE && parts.Count > 0 ? 1 : 0)];

			for(var i = 0; i < parts.Count; i++)
			{
				segments[i] = parts[i];
			}

			return true;
		}

		/// <summary>
		/// 将字符串数组组合成一个路径。
		/// </summary>
		/// <param name="paths">由路径的各部分构成的数组。</param>
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
		public static string Combine(params string[] paths)
		{
			if(paths == null)
				throw new ArgumentNullException(nameof(paths));

			var slashed = false;
			var segments = new List<string>();

			for(int i = 0; i < paths.Length; i++)
			{
				if(string.IsNullOrEmpty(paths[i]))
					continue;

				var segment = string.Empty;
				var spaces = 0;

				foreach(var chr in paths[i])
				{
					switch(chr)
					{
						case ' ':
							if(segment.Length > 0)
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

							switch(segment)
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
									if(segment.Contains(":"))
										segments.Clear();

									segments.Add(segment);
									break;
							}

							segment = string.Empty;
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
								segment += new string(' ', spaces);
								spaces = 0;
							}

							segment += chr;
							slashed = false;
							break;
					}
				}

				if(segment.Length > 0)
				{
					switch(segment)
					{
						case ".":
							break;
						case "..":
							if(segments.Count > 0)
								segments.RemoveAt(segments.Count - 1);
							break;
						default:
							if(segment.Contains(":"))
								segments.Clear();

							segments.Add(segment);
							break;
					}
				}
			}

			return (segments.Count == 0 ? string.Empty : string.Join("/", segments)) + (slashed ? "/" : "");
		}
		#endregion

		#region 私有方法
		private static string GetAnchorString(PathAnchor anchor, bool slashed)
		{
			switch(anchor)
			{
				case PathAnchor.Root:
					return "/";
				case PathAnchor.Current:
					return slashed ? "./" : ".";
				case PathAnchor.Parent:
					return slashed ? "../" : "..";
				default:
					return string.Empty;
			}
		}
		#endregion
	}
}

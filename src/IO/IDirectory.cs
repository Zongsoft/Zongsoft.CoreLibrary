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
using System.Threading.Tasks;

namespace Zongsoft.IO
{
	/// <summary>
	/// 公开用于创建、移动和遍历目录和子目录等功能的抽象接口，该接口将提供不同文件系统的目录支持。
	/// </summary>
	public interface IDirectory
	{
		/// <summary>
		/// 获取指定目录路径对应的<see cref="DirectoryInfo"/>描述信息。
		/// </summary>
		/// <param name="path">指定的目录路径。</param>
		/// <returns>如果指定的路径是存在的则返回对应的<see cref="DirectoryInfo"/>，否则返回空(null)。</returns>
		DirectoryInfo GetInfo(string path);
		Task<DirectoryInfo> GetInfoAsync(string path);

		bool SetInfo(string path, IDictionary<string, string> properties);
		Task<bool> SetInfoAsync(string path, IDictionary<string, string> properties);

		/// <summary>
		/// 创建一个指定路径的目录。
		/// </summary>
		/// <param name="path">指定要创建的目录路径。</param>
		/// <param name="properties">目录的扩展属性集，默认为空(null)。</param>
		/// <returns>如果创建成功则返回真(True)，否则返回假(False)。</returns>
		/// <remarks>
		///		<para>如果<paramref name="path"/>参数指定的路径不存在并且创建成功则返回真；如果指定的路径已存在则返回假。</para>
		/// </remarks>
		bool Create(string path, IDictionary<string, string> properties = null);
		Task<bool> CreateAsync(string path, IDictionary<string, string> properties = null);

		bool Delete(string path, bool recursive = false);
		Task<bool> DeleteAsync(string path, bool recursive = false);

		void Move(string source, string destination);
		Task MoveAsync(string source, string destination);

		bool Exists(string path);
		Task<bool> ExistsAsync(string path);

		/// <summary>
		/// 获取指定路径中的所有文件和目录信息的可枚举集合。
		/// </summary>
		/// <param name="path">要搜索的目录。</param>
		/// <returns>指定路径中的所有文件和目录的<seealso cref="PathInfo"/>集合。</returns>
		/// <remarks>更多功能搜索功能请参考<see cref="GetChildren(string, string, bool)"/>方法。</remarks>
		IEnumerable<PathInfo> GetChildren(string path);

		/// <summary>
		/// 获取指定路径中与搜索模式匹配的所有文件和目录信息的可枚举集合，还可以搜索子目录。
		/// </summary>
		/// <param name="path">要搜索的目录。</param>
		/// <param name="pattern">搜索模式文本，即用于搜索匹配的所有文件或子目录名称的字符串。
		///		<para>注意：不同文件目录系统可能支持的搜索模式能力是不一样的。</para>
		///		<para>搜索模式文本为空(null)或空字符串(“”)或“*”，表示返回指定范围内的所有文件和目录。文件系统的标准搜索模式包括：多字匹配模式(“*”)和单字匹配模式(“?”)，大部分文件目录系统均支持标准搜索模式。</para>
		///		<para>如果搜索模式文本中包含以反斜杠(“\”)或正斜杠(“/”)或竖线符(“|”)字符对括的文本，则表示搜索模式为正则匹配搜索模式；否则为文件系统的标准搜索模式。</para>
		/// </param>
		/// <param name="recursive">指定搜索操作的范围是应仅包含当前目录还是应包含所有子目录，默认是仅包含当前目录。</param>
		/// <returns>匹配指定搜索条件的<seealso cref="PathInfo"/>集合。</returns>
		/// <remarks>
		///		<list type="bullet">
		///			<item>
		///				<term><c>prefix-/\d+/.log</c></term>
		///				<description>正则匹配搜索模式：两个正斜杠(“/”)字符括起来的部分为正则表达式(“\d+”)，表示查找指定目录范围内的以“prefix-”打头并接一个或多个数字，扩展名为“.log”的所有文件或目录。</description>
		///			</item>
		///			<item>
		///				<term><c>prefix-*.log</c></term>
		///				<description>标准搜索模式：字符“*”表示匹配零个或多个字符，表示查找指定目录范围内的以“prefix-”打头并接零个或多个字符，扩展名为“.log”的所有文件或目录。</description>
		///			</item>
		///		</list>
		/// </remarks>
		IEnumerable<PathInfo> GetChildren(string path, string pattern, bool recursive = false);

		Task<IEnumerable<PathInfo>> GetChildrenAsync(string path);
		Task<IEnumerable<PathInfo>> GetChildrenAsync(string path, string pattern, bool recursive = false);

		/// <summary>
		/// 获取指定路径中的所有目录信息的可枚举集合。
		/// </summary>
		/// <param name="path">要搜索的目录。</param>
		/// <returns>指定路径中的所有目录的<seealso cref="DirectoryInfo"/>集合。</returns>
		/// <remarks>更多功能搜索功能请参考<see cref="GetDirectories(string, string, bool)"/>方法。</remarks>
		IEnumerable<DirectoryInfo> GetDirectories(string path);

		/// <summary>
		/// 获取指定路径中与搜索模式匹配的所有目录信息的可枚举集合，还可以搜索子目录。
		/// </summary>
		/// <param name="path">要搜索的目录。</param>
		/// <param name="pattern">搜索模式文本，即用于搜索匹配的所有子目录名称的字符串。更多详细功能请参考：<see cref="GetChildren(string, string, bool)"/>方法。</param>
		/// <param name="recursive">指定搜索操作的范围是应仅包含当前目录还是应包含所有子目录，默认是仅包含当前目录。</param>
		/// <returns>匹配指定搜索条件的<seealso cref="DirectoryInfo"/>集合。</returns>
		IEnumerable<DirectoryInfo> GetDirectories(string path, string pattern, bool recursive = false);

		Task<IEnumerable<DirectoryInfo>> GetDirectoriesAsync(string path);
		Task<IEnumerable<DirectoryInfo>> GetDirectoriesAsync(string path, string pattern, bool recursive = false);

		/// <summary>
		/// 获取指定路径中的所有文件信息的可枚举集合。
		/// </summary>
		/// <param name="path">要搜索的目录。</param>
		/// <returns>指定路径中的所有目录的<seealso cref="FileInfo"/>集合。</returns>
		/// <remarks>更多功能搜索功能请参考<see cref="GetFiles(string, string, bool)"/>方法。</remarks>
		IEnumerable<FileInfo> GetFiles(string path);

		/// <summary>
		/// 获取指定路径中与搜索模式匹配的所有文件信息的可枚举集合，还可以搜索子目录。
		/// </summary>
		/// <param name="path">要搜索的目录。</param>
		/// <param name="pattern">搜索模式文本，即用于搜索匹配的所有文件名的字符串。更多详细功能请参考：<see cref="GetChildren(string, string, bool)"/>方法。</param>
		/// <param name="recursive">指定搜索操作的范围是应仅包含当前目录还是应包含所有子目录，默认是仅包含当前目录。</param>
		/// <returns>匹配指定搜索条件的<seealso cref="FileInfo"/>集合。</returns>
		IEnumerable<FileInfo> GetFiles(string path, string pattern, bool recursive = false);

		Task<IEnumerable<FileInfo>> GetFilesAsync(string path);
		Task<IEnumerable<FileInfo>> GetFilesAsync(string path, string pattern, bool recursive = false);
	}
}

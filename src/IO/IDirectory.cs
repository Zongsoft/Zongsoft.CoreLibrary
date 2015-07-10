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

		IEnumerable<PathInfo> GetChildren(string path);
		IEnumerable<PathInfo> GetChildren(string path, string pattern, bool recursive = false);

		Task<IEnumerable<PathInfo>> GetChildrenAsync(string path);
		Task<IEnumerable<PathInfo>> GetChildrenAsync(string path, string pattern, bool recursive = false);

		IEnumerable<DirectoryInfo> GetDirectories(string path);
		IEnumerable<DirectoryInfo> GetDirectories(string path, string pattern, bool recursive = false);

		Task<IEnumerable<DirectoryInfo>> GetDirectoriesAsync(string path);
		Task<IEnumerable<DirectoryInfo>> GetDirectoriesAsync(string path, string pattern, bool recursive = false);

		IEnumerable<FileInfo> GetFiles(string path);
		IEnumerable<FileInfo> GetFiles(string path, string pattern, bool recursive = false);

		Task<IEnumerable<FileInfo>> GetFilesAsync(string path);
		Task<IEnumerable<FileInfo>> GetFilesAsync(string path, string pattern, bool recursive = false);
	}
}

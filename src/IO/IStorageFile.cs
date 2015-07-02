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
	/// <summary>
	/// 表示<see cref="IStorageFile">存储文件</see>的操作接口。
	/// </summary>
	[Obsolete("Please use Aliyun-OSS providr of filesystem.")]
	public interface IStorageFile
	{
		/// <summary>
		/// 根据指定的文件存储信息和内容，将其保存到文件存储容器中。
		/// </summary>
		/// <param name="file">指定要创建的文件存储信息。</param>
		/// <param name="content">指定要创建的文件内容。</param>
		void Create(StorageFileInfo file, Stream content);

		/// <summary>
		/// 打开指定编号的文件，并获取其内容。
		/// </summary>
		/// <param name="fileId">指定要打开的文件编号。</param>
		/// <returns>返回打开的文件内容，如果指定的文件编号不存在则返回空(null)。</returns>
		Stream Open(Guid fileId);

		/// <summary>
		/// 打开指定编号的文件，并获取其内容。
		/// </summary>
		/// <param name="fileId">指定要打开的文件编号。</param>
		/// <param name="info">输出参数：指定编号对应的文件描述信息，如果指定的文件编号不存在则输出空(null)。</param>
		/// <returns>返回打开的文件内容，如果指定的文件编号不存在则返回空(null)。</returns>
		Stream Open(Guid fileId, out StorageFileInfo info);

		/// <summary>
		/// 获取指定编号的文件信息。
		/// </summary>
		/// <param name="fileId">指定要获取信息的文件编号。</param>
		/// <returns>返回指定编号的文件信息，如果指定的文件编号不存在则返回空(null)。</returns>
		StorageFileInfo GetInfo(Guid fileId);

		/// <summary>
		/// 获取指定编号的文件路径。
		/// </summary>
		/// <param name="fileId">指定要查找的文件编号</param>
		/// <returns>返回的文件路径，如果指定编号的文件不存在则返回空(null)。</returns>
		string GetPath(Guid fileId);

		/// <summary>
		/// 删除指定编号的文件。
		/// </summary>
		/// <param name="fileId">指定要删除的文件编号。</param>
		/// <returns>如果删除成功则返回真(True)，否则返回假(False)。</returns>
		bool Delete(Guid fileId);

		/// <summary>
		/// 复制指定编号的文件到指定的目标存储容器中。
		/// </summary>
		/// <param name="fileId">指定要复制的文件编号。</param>
		/// <param name="bucketId">要复制文件的目的存储容器编号。</param>
		/// <returns>如果复制成功则返回目标文件的编号，否则返回空(null)。</returns>
		/// <remarks>
		///		<para>如果指定的目的存储容器即为指定文件的容器则返回原文件编号；如果指定的文件编号不存在则返回空(null)。</para>
		///		<para>注意：复制操作确保会复制一份文件信息到目的容器中，但是文件的内容是否被物理复制则取决于具体的实现者。</para>
		/// </remarks>
		Guid? Copy(Guid fileId, int bucketId);

		/// <summary>
		/// 移动指定编号的文件到指定的目标存储容器中。
		/// </summary>
		/// <param name="fileId">指定要移动的文件编号。</param>
		/// <param name="bucketId">要移动文件的目的存储容器编号。</param>
		/// <returns>如果移动成功则返回真(True)，否则返回假(False)。</returns>
		/// <remarks>
		///		<para>如果指定的目的存储容器即为指定文件的容器则返回假(False)；如果指定的文件编号不存在则返回假(False)。</para>
		/// </remarks>
		bool Move(Guid fileId, int bucketId);

		bool SetExtendedProperties(Guid fileId, IDictionary<string, object> extendedProperties);
	}
}

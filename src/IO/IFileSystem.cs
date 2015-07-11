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
using System.Collections.Generic;

namespace Zongsoft.IO
{
	/// <summary>
	/// 表示文件目录系统的接口。
	/// </summary>
	public interface IFileSystem
	{
		/// <summary>
		/// 获取文件目录系统的模式名。
		/// </summary>
		string Schema
		{
			get;
		}

		/// <summary>
		/// 获取文件操作服务。
		/// </summary>
		IFile File
		{
			get;
		}

		/// <summary>
		/// 获取目录操作服务。
		/// </summary>
		IDirectory Directory
		{
			get;
		}

		/// <summary>
		/// 获取本地路径对应的外部访问Url地址。
		/// </summary>
		/// <param name="path">要获取的本地路径。</param>
		/// <returns>返回对应的Url地址。</returns>
		/// <remarks>
		///		<para>本地路径：是指特定的<see cref="IFileSystem"/>文件目录系统的路径格式。</para>
		///		<para>外部访问Url地址：是指可通过Web方式访问某个文件或目录的URL。</para>
		/// </remarks>
		string GetUrl(string path);
	}
}

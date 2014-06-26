/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2008-2011 Zongsoft Corporation <http://www.zongsoft.com>
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
	/// 公开用于创建、移动和遍历目录和子目录等功能的抽象接口，该接口将提供不同文件系统的目录支持。
	/// </summary>
	public interface IDirectory
	{
		void Create(string path);

		void Delete(string path);
		void Delete(string path, bool recursive);

		void Move(string source, string destination);
		bool Exists(string path);

		IEnumerable<string> GetChildren(string path);
		IEnumerable<string> GetChildren(string path, string pattern, bool recursive);

		IEnumerable<string> GetDirectories(string path);
		IEnumerable<string> GetDirectories(string path, string pattern, bool recursive);

		IEnumerable<string> GetFiles(string path);
		IEnumerable<string> GetFiles(string path, string pattern, bool recursive);
	}
}

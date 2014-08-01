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
using System.Collections.Concurrent;
using System.Linq;

namespace Zongsoft.IO
{
	public class FileProvider : Zongsoft.Services.ServiceProviderBase, IFileService
	{
		#region 单例字段
		public static readonly FileProvider Default = new FileProvider();
		#endregion

		#region 构造函数
		public FileProvider() : this(new Zongsoft.Services.ServiceStorage(), null)
		{
		}

		public FileProvider(Zongsoft.Services.IServiceBuilder builder) : this(new Zongsoft.Services.ServiceStorage(), builder)
		{
		}

		public FileProvider(Zongsoft.Services.IServiceStorage storage, Zongsoft.Services.IServiceBuilder builder) : base(storage, builder)
		{
			this.Register("zfs.local", LocalFileService.Instance, typeof(IFileService));
		}
		#endregion

		#region 公共方法
		/// <summary>
		/// 获取指定虚拟路径的指定的文件服务。
		/// </summary>
		/// <param name="text">指定的虚拟路径文本。</param>
		/// <param name="path">返回指定虚拟路径文本对应的路径对象。</param>
		/// <returns>如果获取成功则返回对应的文件服务，否则返回空(null)。</returns>
		public IFileService GetService(string text, out Path path)
		{
			if(string.IsNullOrWhiteSpace(text))
				throw new ArgumentNullException("text");

			if(!Path.TryParse(text, out path))
				throw new PathException(text);

			var service = this.Resolve(path.Schema) as IFileService;

			if(service == null)
				throw new InvalidOperationException(string.Format("Can not obtain the FileService object by the '{0}' path.", path));

			return service;
		}

		public IFileService[] GetServices(string[] texts, out Path[] paths)
		{
			if(texts == null || texts.Length == 0)
				throw new ArgumentNullException("texts");

			paths = new Path[texts.Length];
			var result = new IFileService[texts.Length];

			for(int i = 0; i < texts.Length; i++)
			{
				result[i] = this.GetService(texts[i], out paths[i]);
			}

			return result;
		}
		#endregion

		#region 文件操作
		public void Delete(string virtualPath)
		{
			Path path;
			var service = this.GetService(virtualPath, out path);

			service.Delete(path.FullPath);
		}

		public bool Exists(string virtualPath)
		{
			Path path;
			var service = this.GetService(virtualPath, out path);

			return service.Exists(path.FullPath);
		}

		public void Copy(string source, string destination)
		{
			this.Copy(source, destination, true);
		}

		public void Copy(string source, string destination, bool overwrite)
		{
			Path[] paths;
			var services = this.GetServices(new string[] { source, destination }, out paths);

			if(!string.Equals(paths[0].Schema, paths[1].Schema, StringComparison.OrdinalIgnoreCase))
				throw new InvalidOperationException();

			services[0].Copy(paths[0].FullPath, paths[1].FullPath, overwrite);
		}

		public void Move(string source, string destination)
		{
			Path[] paths;
			var services = this.GetServices(new string[] { source, destination }, out paths);

			if(!string.Equals(paths[0].Schema, paths[1].Schema, StringComparison.OrdinalIgnoreCase))
				throw new InvalidOperationException();

			services[0].Move(paths[0].FullPath, paths[1].FullPath);
		}

		public FileInfo GetInfo(string virtualPath)
		{
			Path path;
			var service = this.GetService(virtualPath, out path);

			return service.GetInfo(path.FullPath);
		}

		public System.IO.Stream Open(string virtualPath)
		{
			Path path;
			var service = this.GetService(virtualPath, out path);

			return service.Open(path.FullPath);
		}

		public System.IO.Stream Open(string virtualPath, System.IO.FileMode mode)
		{
			Path path;
			var service = this.GetService(virtualPath, out path);

			return service.Open(path.FullPath, mode);
		}

		public System.IO.Stream Open(string virtualPath, System.IO.FileMode mode, System.IO.FileAccess access)
		{
			Path path;
			var service = this.GetService(virtualPath, out path);

			return service.Open(path.FullPath, mode, access);
		}

		public System.IO.Stream Open(string virtualPath, System.IO.FileMode mode, System.IO.FileAccess access, System.IO.FileShare share)
		{
			Path path;
			var service = this.GetService(virtualPath, out path);

			return service.Open(path.FullPath, mode, access, share);
		}
		#endregion
	}
}

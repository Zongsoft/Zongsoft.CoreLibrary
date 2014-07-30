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
	public class DirectoryProvider : Zongsoft.Services.ServiceProviderBase, IDirectoryService
	{
		#region 单例字段
		public static readonly DirectoryProvider Default = new DirectoryProvider();
		#endregion

		#region 构造函数
		public DirectoryProvider() : this(new Zongsoft.Services.ServiceStorage(), null)
		{
		}

		public DirectoryProvider(Zongsoft.Services.IServiceBuilder builder) : this(new Zongsoft.Services.ServiceStorage(), builder)
		{
		}

		public DirectoryProvider(Zongsoft.Services.IServiceStorage storage, Zongsoft.Services.IServiceBuilder builder) : base(storage, builder)
		{
			this.Register("zfs.local", LocalDirectoryService.Instance, typeof(IDirectoryService));
		}
		#endregion

		#region 公共方法
		/// <summary>
		/// 获取指定虚拟路径的指定的目录服务。
		/// </summary>
		/// <param name="text">指定的虚拟路径文本。</param>
		/// <param name="path">返回指定虚拟路径文本对应的路径对象。</param>
		/// <returns>如果获取成功则返回对应的目录服务，否则返回空(null)。</returns>
		public IDirectoryService GetService(string text, out Path path)
		{
			if(string.IsNullOrWhiteSpace(text))
				throw new ArgumentNullException("text");

			if(!Path.TryParse(text, out path))
				throw new PathException(text);

			var service = this.Resolve(path.Schema) as IDirectoryService;

			if(service == null)
				throw new InvalidOperationException(string.Format("Can not obtain the DirectoryService object by the '{0}' path.", path));

			return service;
		}

		public IDirectoryService[] GetServices(string[] texts, out Path[] paths)
		{
			if(texts == null || texts.Length == 0)
				throw new ArgumentNullException("texts");

			paths = new Path[texts.Length];
			var result = new IDirectoryService[texts.Length];

			for(int i = 0; i < texts.Length; i++)
			{
				result[i] = this.GetService(texts[i], out paths[i]);
			}

			return result;
		}
		#endregion

		#region 目录操作
		public bool Create(string virtualPath)
		{
			Path path;
			var service = this.GetService(virtualPath, out path);

			return service.Create(path.FullPath);
		}

		public void Delete(string virtualPath)
		{
			this.Delete(virtualPath, false);
		}

		public void Delete(string virtualPath, bool recursive)
		{
			Path path;
			var service = this.GetService(virtualPath, out path);

			service.Delete(path.FullPath, recursive);
		}

		public void Move(string source, string destination)
		{
			Path[] paths;
			var services = this.GetServices(new string[] { source, destination }, out paths);

			if(!string.Equals(paths[0].Schema, paths[1].Schema, StringComparison.OrdinalIgnoreCase))
				throw new InvalidOperationException();

			services[0].Move(paths[0].FullPath, paths[1].FullPath);
		}

		public bool Exists(string virtualPath)
		{
			Path path;
			var service = this.GetService(virtualPath, out path);

			return service.Exists(path.FullPath);
		}

		public IEnumerable<string> GetChildren(string virtualPath)
		{
			return this.GetChildren(virtualPath, null, false);
		}

		public IEnumerable<string> GetChildren(string virtualPath, string pattern, bool recursive)
		{
			Path path;
			var service = this.GetService(virtualPath, out path);

			return service.GetChildren(path.FullPath, pattern, recursive);
		}

		public IEnumerable<string> GetDirectories(string virtualPath)
		{
			return this.GetDirectories(virtualPath, null, false);
		}

		public IEnumerable<string> GetDirectories(string virtualPath, string pattern, bool recursive)
		{
			Path path;
			var service = this.GetService(virtualPath, out path);

			return service.GetDirectories(path.FullPath, pattern, recursive);
		}

		public IEnumerable<string> GetFiles(string virtualPath)
		{
			return this.GetFiles(virtualPath, null, false);
		}

		public IEnumerable<string> GetFiles(string virtualPath, string pattern, bool recursive)
		{
			Path path;
			var service = this.GetService(virtualPath, out path);

			return service.GetFiles(path.FullPath, pattern, recursive);
		}
		#endregion
	}
}

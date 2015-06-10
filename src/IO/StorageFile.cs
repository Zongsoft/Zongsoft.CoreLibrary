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
using System.Collections;
using System.Collections.Generic;

using Zongsoft.Collections;

namespace Zongsoft.IO
{
	public class StorageFile : IStorageFile
	{
		#region 成员字段
		private Zongsoft.Runtime.Caching.ICache _storage;
		private IFileSystem _fileSystem;
		#endregion

		#region 构造函数
		public StorageFile()
		{
		}

		public StorageFile(Zongsoft.Services.IServiceProvider serviceProvider)
		{
			if(serviceProvider != null)
			{
				_storage = serviceProvider.Resolve<Zongsoft.Runtime.Caching.ICache>();
				_fileSystem = serviceProvider.Resolve<IFileSystem>(Zongsoft.IO.FileSystem.Schema);
			}
		}

		public StorageFile(Zongsoft.Runtime.Caching.ICache storage, IFileSystem fileSystem)
		{
			if(storage == null)
				throw new ArgumentNullException("storage");

			if(fileSystem == null)
				throw new ArgumentNullException("fileSystem");

			_storage = storage;
			_fileSystem = fileSystem;
		}
		#endregion

		#region 公共属性
		public Zongsoft.Runtime.Caching.ICache Storage
		{
			get
			{
				return _storage;
			}
			set
			{
				if(value == null)
					throw new ArgumentNullException();

				_storage = value;
			}
		}

		public IFileSystem FileSystem
		{
			get
			{
				return _fileSystem;
			}
			set
			{
				if(value == null)
					throw new ArgumentNullException();

				_fileSystem = value;
			}
		}
		#endregion

		#region 公共方法
		public void Create(StorageFileInfo file, Stream content)
		{
			if(file == null)
				throw new ArgumentNullException("file");

			var storage = this.Storage;

			if(storage == null)
				throw new InvalidOperationException("The Storage is null.");

			storage.SetValue(GetFileKey(file.FileId), file.ToDictionary());

			var collection = storage.GetValue(GetFileCollectionKey(file.BucketId)) as ICollection<string>;

			if(collection != null)
				collection.Add(file.FileId.ToString("n"));
			else
				storage.SetValue(GetFileCollectionKey(file.BucketId), new string[] { file.FileId.ToString("n") });

			if(!string.IsNullOrWhiteSpace(file.Path) && content != null)
			{
				var path = Zongsoft.IO.Path.Parse(file.Path);
				var fileSystem = this.EnsureFileSystem();

				//确认文件的所在目录是存在的，如果不存在则创建相应的目录
				fileSystem.Directory.Create(path.DirectoryName);

				//创建或打开指定路径的文件流
				var stream = fileSystem.File.Open(file.Path, FileMode.Create, FileAccess.Write);

				//将文件内容写入到创建或打开的文件流中
				StreamUtility.Copy(content, stream);
			}
		}

		public Stream Open(Guid fileId)
		{
			var storage = this.Storage;

			if(storage == null)
				throw new InvalidOperationException("The Storage is null.");

			var dictionary = storage.GetValue(GetFileKey(fileId)) as IDictionary;

			if(dictionary == null || dictionary.Count < 1)
				return null;

			var path = string.Empty;

			if(dictionary.TryGetValue<string>("Path", out path) && string.IsNullOrWhiteSpace(path))
				return null;

			if(dictionary is Zongsoft.Common.IAccumulator)
				((Zongsoft.Common.IAccumulator)dictionary).Increment("TotalVisits");
			else
			{
				long totalVisits = 0;
				dictionary.TryGetValue<long>("TotalVisits", out totalVisits);
				dictionary["TotalVisits"] = totalVisits + 1;
			}

			dictionary["VisitedTime"] = DateTime.Now;

			return this.EnsureFileSystem().File.Open(path, FileMode.Open, FileAccess.ReadWrite, FileShare.Read);
		}

		public Stream Open(Guid fileId, out StorageFileInfo info)
		{
			var storage = this.Storage;

			if(storage == null)
				throw new InvalidOperationException("The Storage is null.");

			info = null;

			var dictionary = storage.GetValue(GetFileKey(fileId)) as IDictionary;

			if(dictionary == null || dictionary.Count < 1)
				return null;

			info = StorageFileInfo.FromDictionary(dictionary);

			if(dictionary is Zongsoft.Common.IAccumulator)
				((Zongsoft.Common.IAccumulator)dictionary).Increment("TotalVisits");
			else
			{
				long totalVisits = 0;
				dictionary.TryGetValue<long>("TotalVisits", out totalVisits);
				dictionary["TotalVisits"] = totalVisits + 1;
			}

			dictionary["VisitedTime"] = DateTime.Now;

			if(string.IsNullOrWhiteSpace(info.Path))
				return null;

			return this.EnsureFileSystem().File.Open(info.Path, FileMode.Open, FileAccess.ReadWrite, FileShare.Read);
		}

		public StorageFileInfo GetInfo(Guid fileId)
		{
			var storage = this.Storage;

			if(storage == null)
				throw new InvalidOperationException("The Storage is null.");

			var dictionary = storage.GetValue(GetFileKey(fileId)) as IDictionary;

			if(dictionary != null)
				return StorageFileInfo.FromDictionary(dictionary);

			return null;
		}

		public string GetPath(Guid fileId)
		{
			var storage = this.Storage;

			if(storage == null)
				throw new InvalidOperationException("The Storage is null.");

			var dictionary = storage.GetValue(GetFileKey(fileId)) as IDictionary;

			if(dictionary != null)
			{
				string path = null;

				if(dictionary.TryGetValue<string>("Path", out path))
					return path;
			}

			return null;
		}

		public bool Delete(Guid fileId)
		{
			var storage = this.Storage;

			if(storage == null)
				throw new InvalidOperationException("The Storage is null.");

			var dictionary = storage.GetValue(GetFileKey(fileId)) as IDictionary;
			var filePath = string.Empty;
			var bucketId = 0;

			if(dictionary == null || dictionary.Count < 1)
				return false;

			dictionary.TryGetValue<int>("BucketId", out bucketId);
			dictionary.TryGetValue<string>("Path", out filePath);

			storage.Remove(GetFileKey(fileId));

			var collection = storage.GetValue(GetFileCollectionKey(bucketId)) as ICollection<string>;

			if(collection != null)
				collection.Remove(fileId.ToString("n"));

			if(!string.IsNullOrWhiteSpace(filePath))
				this.EnsureFileSystem().File.Delete(filePath);

			return true;
		}

		public Guid? Copy(Guid fileId, int bucketId)
		{
			var storage = this.Storage;

			if(storage == null)
				throw new InvalidOperationException("The Storage is null.");

			var info = this.GetInfo(fileId);

			if(info == null)
				return null;

			if(info.BucketId == bucketId)
				return fileId;

			var newId = Guid.NewGuid();

			this.Create(new StorageFileInfo(bucketId, newId)
			{
				Name = info.Name,
				Type = info.Type,
				Size = info.Size,
				Path = info.Path,
				Title = info.Title,
			}, null);

			return newId;
		}

		public bool Move(Guid fileId, int bucketId)
		{
			var storage = this.Storage;

			if(storage == null)
				throw new InvalidOperationException("The Storage is null.");

			var dictionary = storage.GetValue(GetFileKey(fileId)) as IDictionary;

			if(dictionary == null)
				return false;

			int currentBucketId;

			if(dictionary.TryGetValue("BucketId", out currentBucketId))
			{
				//将当前文件的所在存储容器编号更改为目标容器编号
				dictionary["BucketId"] = bucketId.ToString();

				//获取当前文件容器列表集合
				var collection = storage.GetValue(GetFileCollectionKey(currentBucketId)) as ICollection<string>;

				if(collection != null)
					collection.Remove(currentBucketId.ToString());

				//获取目标文件容器列表集合
				collection = storage.GetValue(GetFileCollectionKey(bucketId)) as ICollection<string>;

				if(collection != null)
					collection.Add(bucketId.ToString());
				else
					storage.SetValue(GetFileCollectionKey(bucketId), new string[] { bucketId.ToString() });

				return true;
			}

			return false;
		}
		#endregion

		#region 私有方法
		private IFileSystem EnsureFileSystem()
		{
			var fileSystem = this.FileSystem;

			if(fileSystem == null)
				throw new InvalidOperationException("The value of 'FileSystem' property is null.");

			return fileSystem;
		}

		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
		internal static string GetFileCollectionKey(int bucketId)
		{
			return "Zongsoft.IO.StorageFiles:" + bucketId.ToString();
		}

		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
		internal static string GetFileKey(Guid fileId)
		{
			return "Zongsoft.IO.StorageFile:" + fileId.ToString("n");
		}
		#endregion
	}
}

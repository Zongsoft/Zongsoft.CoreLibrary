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

namespace Zongsoft.IO
{
	public class StorageFileService : IStorageFileService
	{
		#region 成员字段
		private Zongsoft.Runtime.Caching.ICache _storage;
		private Zongsoft.IO.IFileService _fileService;
		#endregion

		#region 构造函数
		public StorageFileService()
		{
		}

		public StorageFileService(Zongsoft.Runtime.Caching.ICache storage)
		{
			if(storage == null)
				throw new ArgumentNullException("storage");

			_storage = storage;
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

		public Zongsoft.IO.IFileService FileService
		{
			get
			{
				return _fileService ?? Zongsoft.IO.FileProvider.Default;
			}
			set
			{
				_fileService = value;
			}
		}
		#endregion

		#region 公共方法
		public void Create(StorageFile file, Stream content)
		{
			
		}

		public Stream Open(Guid fileId)
		{
			throw new NotImplementedException();
		}

		public StorageFile GetFileInfo(Guid fileId)
		{
			var storage = this.Storage;

			if(storage == null)
				throw new InvalidOperationException("The Storage is null.");

			var dictionary = storage.GetValue("Zongsoft.IO.StorageFile:" + fileId.ToString("n")) as IDictionary;

			if(dictionary != null)
				return StorageFile.FromDictionary(dictionary);

			return null;
		}

		public bool Delete(Guid fileId)
		{
			var storage = this.Storage;

			if(storage == null)
				throw new InvalidOperationException("The Storage is null.");

			var dictionary = storage.GetValue("Zongsoft.IO.StorageFile:" + fileId.ToString("n")) as IDictionary;
			var filePath = string.Empty;

			Zongsoft.Collections.DictionaryExtension.TryGetValue(dictionary, "Path", out filePath);

			storage.Remove("Zongsoft.IO.StorageFile:" + fileId.ToString("n"));

			var collection = storage.GetValue("Zongsoft.IO.StorageFiles") as IList;

			if(collection != null)
				collection.Remove(fileId.ToString("n"));

			if(!string.IsNullOrWhiteSpace(filePath))
				this.FileService.Delete(filePath);

			return true;
		}

		public bool Copy(Guid fileId, int bucketId)
		{
			throw new NotImplementedException();
		}

		public bool Move(Guid fileId, int bucketId)
		{
			throw new NotImplementedException();
		}
		#endregion
	}
}

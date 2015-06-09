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
using System.Collections;
using System.Collections.Generic;

namespace Zongsoft.IO
{
	public class StorageBucketService : IStorageBucketService
	{
		#region 成员字段
		private Zongsoft.Runtime.Caching.ICache _storage;
		#endregion

		#region 构造函数
		public StorageBucketService()
		{
		}

		public StorageBucketService(Zongsoft.Runtime.Caching.ICache storage)
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
		#endregion

		#region 公共方法
		public StorageBucket Create(int bucketId, string name, string title, string path)
		{
			if(string.IsNullOrWhiteSpace(name))
				throw new ArgumentNullException("name");

			var storage = this.Storage;

			if(storage == null)
				throw new InvalidOperationException("The Storage is null.");

			var bucket = new StorageBucket(bucketId)
			{
				Name = name,
				Path = path,
				Title = title,
			};

			storage.SetValue("Zongsoft.IO.StorageBucket:" + bucketId.ToString(), bucket.ToDictionary());

			var collection = storage.GetValue("Zongsoft.IO.StorageBuckets", key => new Tuple<object, TimeSpan>(new int[] { bucketId }, TimeSpan.Zero)) as IList;

			if(collection != null)
				collection.Add(bucketId);

			return bucket;
		}

		public StorageBucket GetBucketInfo(int bucketId)
		{
			var storage = this.Storage;

			if(storage == null)
				throw new InvalidOperationException("The Storage is null.");

			var dictionary = storage.GetValue("Zongsoft.IO.StorageBucket:" + bucketId) as IDictionary;

			if(dictionary != null)
				return StorageBucket.FromDictionary(dictionary);

			return null;
		}

		public bool Delete(int bucketId)
		{
			var storage = this.Storage;

			if(storage == null)
				throw new InvalidOperationException("The Storage is null.");

			var result = storage.Remove("Zongsoft.IO.StorageBucket:" + bucketId);

			var collection = storage.GetValue("Zongsoft.IO.StorageBuckets") as IList;

			if(collection != null)
				collection.Remove(bucketId);

			return result;
		}

		public void Modify(int bucketId, string name, string title, string path, DateTime? modifiedTime)
		{
			var storage = this.Storage;

			if(storage == null)
				throw new InvalidOperationException("The Storage is null.");

			var dictionary = storage.GetValue("Zongsoft.IO.StorageBucket:" + bucketId) as IDictionary;

			if(dictionary != null)
			{
				if(name != null)
					dictionary["Name"] = name.Trim();

				if(title != null)
					dictionary["Title"] = title;

				if(path != null)
					dictionary["Path"] = path.Trim();

				if(modifiedTime.HasValue)
					dictionary["ModifiedTime"] = modifiedTime.Value;
			}
		}

		public int GetBucketCount()
		{
			var storage = this.Storage;

			if(storage == null)
				throw new InvalidOperationException("The Storage is null.");

			var collection = storage.GetValue("Zongsoft.IO.StorageBuckets") as ICollection;

			if(collection == null)
				return 0;

			return collection.Count;
		}

		public int GetFileCount(int bucketId)
		{
			var storage = this.Storage;

			if(storage == null)
				throw new InvalidOperationException("The Storage is null.");

			var collection = storage.GetValue("Zongsoft.IO.StorageFiles:" + bucketId.ToString()) as ICollection;

			if(collection == null)
				return 0;

			return collection.Count;
		}
		#endregion
	}
}

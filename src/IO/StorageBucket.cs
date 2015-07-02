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

using Zongsoft.Collections;

namespace Zongsoft.IO
{
	[Obsolete("Please use Aliyun-OSS providr of filesystem.")]
	public class StorageBucket : IStorageBucket
	{
		#region 成员字段
		private Zongsoft.Runtime.Caching.ICache _storage;
		#endregion

		#region 构造函数
		public StorageBucket()
		{
		}

		public StorageBucket(Zongsoft.Runtime.Caching.ICache storage)
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
		public StorageBucketInfo Create(int bucketId, string name, string title, string path)
		{
			if(string.IsNullOrWhiteSpace(name))
				throw new ArgumentNullException("name");

			var storage = this.Storage;

			if(storage == null)
				throw new InvalidOperationException("The Storage is null.");

			var bucket = new StorageBucketInfo(bucketId)
			{
				Name = name,
				Path = path,
				Title = title,
			};

			storage.SetValue(GetBucketKey(bucketId), bucket.ToDictionary());

			var collection = storage.GetValue(GetBucketCollectionKey()) as ICollection<string>;

			if(collection != null)
				collection.Add(bucketId.ToString());
			else
				storage.SetValue(GetBucketCollectionKey(), new string[] { bucketId.ToString() });

			return bucket;
		}

		public StorageBucketInfo GetInfo(int bucketId)
		{
			var storage = this.Storage;

			if(storage == null)
				throw new InvalidOperationException("The Storage is null.");

			var dictionary = storage.GetValue(GetBucketKey(bucketId)) as IDictionary;

			if(dictionary != null)
				return StorageBucketInfo.FromDictionary(dictionary);

			return null;
		}

		public string GetPath(int bucketId)
		{
			var storage = this.Storage;

			if(storage == null)
				throw new InvalidOperationException("The Storage is null.");

			var dictionary = storage.GetValue(GetBucketKey(bucketId)) as IDictionary;

			if(dictionary != null)
			{
				string path = null;

				if(dictionary.TryGetValue<string>("Path", out path))
					return path;
			}

			return null;
		}

		public bool Delete(int bucketId)
		{
			var storage = this.Storage;

			if(storage == null)
				throw new InvalidOperationException("The Storage is null.");

			var result = storage.Remove(GetBucketKey(bucketId));

			var collection = storage.GetValue(GetBucketCollectionKey()) as ICollection<string>;

			if(collection != null)
				collection.Remove(bucketId.ToString());

			return result;
		}

		public void Modify(int bucketId, string name, string title, string path, DateTime? modifiedTime)
		{
			var storage = this.Storage;

			if(storage == null)
				throw new InvalidOperationException("The Storage is null.");

			var dictionary = storage.GetValue(GetBucketKey(bucketId)) as IDictionary;

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

			var collection = storage.GetValue(GetBucketCollectionKey()) as ICollection;

			if(collection == null)
				return 0;

			return collection.Count;
		}

		public int GetFileCount(int bucketId)
		{
			var storage = this.Storage;

			if(storage == null)
				throw new InvalidOperationException("The Storage is null.");

			var collection = storage.GetValue(StorageFile.GetFileCollectionKey(bucketId)) as ICollection;

			if(collection == null)
				return 0;

			return collection.Count;
		}
		#endregion

		#region 私有方法
		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
		internal static string GetBucketCollectionKey()
		{
			return "Zongsoft.IO.StorageBuckets";
		}

		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
		internal static string GetBucketKey(int bucketId)
		{
			return "Zongsoft.IO.StorageBucket:" + bucketId.ToString();
		}
		#endregion
	}
}

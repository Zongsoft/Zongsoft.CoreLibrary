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
using System.Text.RegularExpressions;

using Zongsoft.Collections;

namespace Zongsoft.IO
{
	[Obsolete("Please use Aliyun-OSS providr of filesystem.")]
	public class StorageFile : IStorageFile
	{
		#region 成员字段
		private Zongsoft.Runtime.Caching.ICache _storage;
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
			}
		}

		public StorageFile(Zongsoft.Runtime.Caching.ICache storage)
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
		public void Create(StorageFileInfo file, Stream content)
		{
			if(file == null)
				throw new ArgumentNullException("file");

			var storage = this.Storage;

			if(storage == null)
				throw new InvalidOperationException("The Storage is null.");

			//如果文件路径为空则为它设置一个有效的文件路径
			if(string.IsNullOrWhiteSpace(file.Path))
				file.Path = this.GetFilePath(file);

			storage.SetValue(GetFileKey(file.FileId), file.ToDictionary());

			var collection = storage.GetValue(GetFileCollectionKey(file.BucketId)) as ICollection<string>;

			if(collection != null)
				collection.Add(file.FileId.ToString("n"));
			else
				storage.SetValue(GetFileCollectionKey(file.BucketId), new string[] { file.FileId.ToString("n") });

			if(content != null)
			{
				var path = Zongsoft.IO.Path.Parse(file.Path);

				//确认文件的所在目录是存在的，如果不存在则创建相应的目录
				FileSystem.Directory.Create(path.Schema + ":" + path.DirectoryName);

				//创建或打开指定路径的文件流
				using(var stream = FileSystem.File.Open(file.Path, FileMode.Create, FileAccess.Write))
				{
					//将文件内容写入到创建或打开的文件流中
					content.CopyTo(stream);
				}
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

			return FileSystem.File.Open(path, FileMode.Open, FileAccess.ReadWrite, FileShare.Read);
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

			return FileSystem.File.Open(info.Path, FileMode.Open, FileAccess.ReadWrite, FileShare.Read);
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
				FileSystem.File.Delete(filePath);

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

			var newInfo = new StorageFileInfo(bucketId, Guid.NewGuid())
			{
				Name = info.Name,
				Type = info.Type,
				Size = info.Size,
				Path = info.Path,
			};

			if(info.HasExtendedProperties)
			{
				foreach(var entry in info.ExtendedProperties)
				{
					newInfo.ExtendedProperties.Add(entry.Key, entry.Value);
				}
			}

			this.Create(newInfo, null);

			return newInfo.FileId;
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

		public bool SetExtendedProperties(Guid fileId, IDictionary<string, object> extendedProperties)
		{
			if(extendedProperties == null)
				throw new ArgumentNullException("extendedProperties");

			var storage = this.Storage;

			if(storage == null)
				throw new InvalidOperationException("The Storage is null.");

			var dictionary = storage.GetValue(GetFileKey(fileId)) as IDictionary;

			if(dictionary == null)
				return false;

			foreach(var extendedProperty in extendedProperties)
			{
				dictionary[StorageFileInfo.EXTENDEDPROPERTIESPREFIX + extendedProperty.Key] = extendedProperty.Value;
			}

			return true;
		}
		#endregion

		#region 私有方法
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

		private string GetBucketPath(int bucketId)
		{
			var storage = this.Storage;

			if(storage == null)
				throw new InvalidOperationException("The Storage is null.");

			var dictionary = storage.GetValue(StorageBucket.GetBucketKey(bucketId)) as IDictionary;

			if(dictionary != null)
			{
				string path = null;

				if(dictionary.TryGetValue<string>("Path", out path))
					return path;
			}

			return null;
		}

		private string GetFilePath(StorageFileInfo fileInfo)
		{
			var timestamp = fileInfo.CreatedTime;
			var parameters = Zongsoft.Collections.DictionaryExtension.ToDictionary<string, string>(Environment.GetEnvironmentVariables());

			parameters.Add("date", timestamp.ToString("yyyyMMdd"));
			parameters.Add("time", timestamp.ToString("HHmmss"));
			parameters.Add("year", timestamp.Year.ToString("0000"));
			parameters.Add("month", timestamp.Month.ToString("00"));
			parameters.Add("day", timestamp.Day.ToString("00"));

			var resolvedPath = this.ResolveTextWithParameters(this.GetBucketPath(fileInfo.BucketId), parameters);

			//返回当前文件的完整虚拟路径
			return Zongsoft.IO.Path.Combine(resolvedPath, string.Format("[{0}]{1:n}{2}", fileInfo.BucketId, fileInfo.FileId, System.IO.Path.GetExtension(fileInfo.Name).ToLowerInvariant()));
		}

		private string ResolveTextWithParameters(string text, IDictionary<string, string> parameters)
		{
			if(string.IsNullOrWhiteSpace(text))
				return string.Empty;

			if(parameters == null || parameters.Count < 1)
				return text;

			var result = text;

			foreach(var parameter in parameters)
			{
				if(string.IsNullOrWhiteSpace(parameter.Key))
					continue;

				result = Regex.Replace(result, @"\$\(" + parameter.Key + @"\)", parameter.Value, RegexOptions.IgnorePatternWhitespace | RegexOptions.IgnoreCase);
			}

			return result;
		}
		#endregion
	}
}

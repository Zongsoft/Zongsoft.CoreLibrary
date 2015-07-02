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
using System.ComponentModel;

using Zongsoft.Collections;

namespace Zongsoft.IO
{
	/// <summary>
	/// 表示存储文件的信息类。
	/// </summary>
	[Serializable]
	[Obsolete("Please use Aliyun-OSS providr of filesystem.")]
	public class StorageFileInfo : MarshalByRefObject
	{
		#region 常量定义
		internal const string EXTENDEDPROPERTIESPREFIX = "ExtendedProperties.";
		#endregion

		#region 成员字段
		private Guid _fileId;
		private int _bucketId;
		private string _name;
		private string _type;
		private long _size;
		private string _path;
		private DateTime _createdTime;
		private DateTime? _modifiedTime;
		private DateTime? _visitedTime;
		private long _totalVisits;
		private IDictionary<string, object> _extendedProperties;
		#endregion

		#region 构造函数
		public StorageFileInfo() : this(0)
		{
		}

		public StorageFileInfo(int bucketId) : this(bucketId, Guid.NewGuid())
		{
		}

		public StorageFileInfo(int bucketId, Guid fileId)
		{
			_bucketId = bucketId;
			_fileId = fileId;
			_createdTime = DateTime.Now;
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取或设置文件的编号。
		/// </summary>
		public Guid FileId
		{
			get
			{
				return _fileId;
			}
			set
			{
				_fileId = value;
			}
		}

		/// <summary>
		/// 获取或设置文件所在的容器编号。
		/// </summary>
		public int BucketId
		{
			get
			{
				return _bucketId;
			}
			set
			{
				_bucketId = value;
			}
		}

		/// <summary>
		/// 获取或设置文件的名称。
		/// </summary>
		public string Name
		{
			get
			{
				return _name;
			}
			set
			{
				_name = value;
			}
		}

		/// <summary>
		/// 获取或设置文件的MIME类型名。
		/// </summary>
		public string Type
		{
			get
			{
				return _type;
			}
			set
			{
				_type = value;
			}
		}

		/// <summary>
		/// 获取或设置文件的存储路径。
		/// </summary>
		public string Path
		{
			get
			{
				return _path;
			}
			set
			{
				_path = value;
			}
		}

		/// <summary>
		/// 获取或设置文件的大小(单位：字节)。
		/// </summary>
		public long Size
		{
			get
			{
				return _size;
			}
			set
			{
				_size = value;
			}
		}

		/// <summary>
		/// 获取或设置文件的下载次数。
		/// </summary>
		public long TotalVisits
		{
			get
			{
				return _totalVisits;
			}
			set
			{
				_totalVisits = value;
			}
		}

		/// <summary>
		/// 获取或设置文件的创建时间。
		/// </summary>
		public DateTime CreatedTime
		{
			get
			{
				return _createdTime;
			}
			set
			{
				_createdTime = value;
			}
		}

		/// <summary>
		/// 获取或设置文件的最后修改时间。
		/// </summary>
		public DateTime? ModifiedTime
		{
			get
			{
				return _modifiedTime;
			}
			set
			{
				_modifiedTime = value;
			}
		}

		/// <summary>
		/// 获取或设置文件的最后访问时间。
		/// </summary>
		public DateTime? VisitedTime
		{
			get
			{
				return _visitedTime;
			}
			set
			{
				_visitedTime = value;
			}
		}

		/// <summary>
		/// 获取一个值，指示扩展属性集是否存在并且有值。
		/// </summary>
		public bool HasExtendedProperties
		{
			get
			{
				return _extendedProperties != null && _extendedProperties.Count > 0;
			}
		}

		/// <summary>
		/// 获取扩展属性集。
		/// </summary>
		public IDictionary<string, object> ExtendedProperties
		{
			get
			{
				if(_extendedProperties == null)
					System.Threading.Interlocked.CompareExchange(ref _extendedProperties, new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase), null);

				return _extendedProperties;
			}
		}
		#endregion

		#region 公共方法
		public IDictionary<string, object> ToDictionary()
		{
			var result = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
			{
				{ "BucketId", _bucketId },
				{ "FileId", _fileId },
				{ "Name", _name },
				{ "Type", _type },
				{ "Size", _size },
				{ "Path", _path },
				{ "TotalVisits", _totalVisits },
				{ "CreatedTime", _createdTime },
				{ "ModifiedTime", _modifiedTime },
				{ "VisitedTime", _visitedTime },
			};

			var extendedProperties = _extendedProperties;

			if(extendedProperties != null && extendedProperties.Count > 0)
			{
				foreach(var extendedProperty in extendedProperties)
				{
					result.Add(EXTENDEDPROPERTIESPREFIX + extendedProperty.Key, extendedProperty.Value);
				}
			}

			return result;
		}

		public static StorageFileInfo FromDictionary(IDictionary dictionary)
		{
			if(dictionary == null || dictionary.Count < 1)
				return null;

			object bucketId, fileId, name, type, size, path, totalVisits, createdTime, modifiedTime, visitedTime;

			if(!dictionary.TryGetValue("BucketId", out bucketId))
				return null;

			if(!dictionary.TryGetValue("FileId", out fileId))
				return null;

			dictionary.TryGetValue("Name", out name);
			dictionary.TryGetValue("Type", out type);
			dictionary.TryGetValue("Size", out size);
			dictionary.TryGetValue("Path", out path);
			dictionary.TryGetValue("TotalVisits", out totalVisits);
			dictionary.TryGetValue("CreatedTime", out createdTime);
			dictionary.TryGetValue("ModifiedTime", out modifiedTime);
			dictionary.TryGetValue("VisitedTime", out visitedTime);

			var result = new StorageFileInfo(Zongsoft.Common.Convert.ConvertValue<int>(bucketId), Zongsoft.Common.Convert.ConvertValue<Guid>(fileId))
			{
				Name = Zongsoft.Common.Convert.ConvertValue<string>(name),
				Type = Zongsoft.Common.Convert.ConvertValue<string>(type),
				Size = Zongsoft.Common.Convert.ConvertValue<long>(size),
				Path = Zongsoft.Common.Convert.ConvertValue<string>(path),
				TotalVisits = Zongsoft.Common.Convert.ConvertValue<int>(totalVisits),
				CreatedTime = Zongsoft.Common.Convert.ConvertValue<DateTime>(createdTime),
				ModifiedTime = Zongsoft.Common.Convert.ConvertValue<DateTime?>(modifiedTime),
				VisitedTime = Zongsoft.Common.Convert.ConvertValue<DateTime?>(visitedTime),
			};

			foreach(var key in dictionary.Keys)
			{
				if(key == null)
					continue;

				if(key.ToString().StartsWith(EXTENDEDPROPERTIESPREFIX))
					result.ExtendedProperties[key.ToString().Substring(EXTENDEDPROPERTIESPREFIX.Length)] = dictionary[key];
			}

			return result;
		}
		#endregion

		#region 重写方法
		public override int GetHashCode()
		{
			var bytes = _fileId.ToByteArray();
			int result = bytes[0];

			for(int i = 1; i < bytes.Length; i++)
			{
				result ^= bytes[i];
			}

			return result;
		}

		public override bool Equals(object obj)
		{
			if(obj == null || obj.GetType() != this.GetType())
				return false;

			return ((StorageFileInfo)obj).FileId == this.FileId;
		}

		public override string ToString()
		{
			if(string.IsNullOrWhiteSpace(_path))
				return string.Format("#{0} {1}", _bucketId.ToString(), _name);
			else
				return string.Format("#{0} {1} ({2})", _bucketId.ToString(), _name, _path);
		}
		#endregion
	}
}

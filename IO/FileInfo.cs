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

namespace Zongsoft.IO
{
	public class FileInfo : MarshalByRefObject
	{
		#region 成员字段
		private string _fullPath;
		private string _name;
		private string _directoryName;
		private long _size;
		private DateTime _createdTime;
		private DateTime? _modifiedTime;
		private Dictionary<string, object> _properties;
		#endregion

		#region 构造函数
		public FileInfo()
		{
		}

		public FileInfo(string fullPath, string name, long size = 0, DateTime? createdTime = null, DateTime? modifiedTime = null)
		{
			if(string.IsNullOrWhiteSpace(fullPath))
				throw new ArgumentNullException("fullPath");

			_fullPath = fullPath;
			_name = name;
			_size = size;
			_modifiedTime = modifiedTime;

			if(createdTime.HasValue)
				_createdTime = createdTime.Value;
		}
		#endregion

		#region 公共属性
		public virtual string Name
		{
			get
			{
				return _name;
			}
			protected set
			{
				_name = value;
			}
		}

		public virtual string DirectoryName
		{
			get
			{
				return _directoryName;
			}
			protected set
			{
				_directoryName = value;
			}
		}

		public string FullPath
		{
			get
			{
				return _fullPath;
			}
			protected set
			{
				_fullPath = value;
			}
		}

		public long Size
		{
			get
			{
				return _size;
			}
			protected set
			{
				_size = value;
			}
		}

		public DateTime CreatedTime
		{
			get
			{
				return _createdTime;
			}
			protected set
			{
				_createdTime = value;
			}
		}

		public DateTime? ModifiedTime
		{
			get
			{
				return _modifiedTime;
			}
			protected set
			{
				_modifiedTime = value;
			}
		}

		public IDictionary<string, object> Properties
		{
			get
			{
				if(_properties == null)
					System.Threading.Interlocked.CompareExchange(ref _properties, new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase), null);

				return _properties;
			}
		}
		#endregion
	}
}

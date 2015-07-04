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
	public class DirectoryInfo : MarshalByRefObject
	{
		#region 成员字段
		private string _fullPath;
		private string _name;
		private DateTime _createdTime;
		private DateTime _modifiedTime;
		private Dictionary<string, string> _properties;
		#endregion

		#region 构造函数
		protected DirectoryInfo()
		{
		}

		public DirectoryInfo(string fullPath, string name, DateTime? createdTime = null, DateTime? modifiedTime = null)
		{
			if(string.IsNullOrWhiteSpace(fullPath))
				throw new ArgumentNullException("fullPath");

			_fullPath = fullPath;
			_name = name;

			if(createdTime.HasValue)
				_createdTime = createdTime.Value;

			if(modifiedTime.HasValue)
				_modifiedTime = modifiedTime.Value;
			else
				_modifiedTime = _createdTime;
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

		public DateTime ModifiedTime
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

		public bool HasProperties
		{
			get
			{
				return _properties != null && _properties.Count > 0;
			}
		}

		public IDictionary<string, string> Properties
		{
			get
			{
				if(_properties == null)
					System.Threading.Interlocked.CompareExchange(ref _properties, new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase), null);

				return _properties;
			}
		}
		#endregion
	}
}

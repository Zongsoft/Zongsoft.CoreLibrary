/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2010-2015 Zongsoft Corporation <http://www.zongsoft.com>
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
	[Serializable]
	public class FileInfo : PathInfo
	{
		#region 成员字段
		private long _size;
		private byte[] _checksum;
		#endregion

		#region 构造函数
		protected FileInfo()
		{
		}

		public FileInfo(string path, long size, DateTime? createdTime = null, DateTime? modifiedTime = null, string url = null) : base(path, createdTime, modifiedTime, url)
		{
			_size = size;
		}

		public FileInfo(Path path, long size, DateTime? createdTime = null, DateTime? modifiedTime = null, string url = null) : base(path, createdTime, modifiedTime, url)
		{
			_size = size;
		}

		public FileInfo(string path, long size, byte[] checksum, DateTime? createdTime = null, DateTime? modifiedTime = null, string url = null)
			: base(path, createdTime, modifiedTime, url)
		{
			_size = size;
			_checksum = checksum;
		}

		public FileInfo(Path path, long size, byte[] checksum, DateTime? createdTime = null, DateTime? modifiedTime = null, string url = null)
			: base(path, createdTime, modifiedTime, url)
		{
			_size = size;
			_checksum = checksum;
		}
		#endregion

		#region 公共属性
		public byte[] Checksum
		{
			get
			{
				return _checksum;
			}
			set
			{
				_checksum = value;
			}
		}

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

		public override bool IsFile
		{
			get
			{
				return true;
			}
		}

		public override bool IsDirectory
		{
			get
			{
				return false;
			}
		}
		#endregion

		#region 重写方法
		public override int GetHashCode()
		{
			var path = this.Path;
			var text = _size.ToString() + Zongsoft.Common.Convert.ToHexString(_checksum);

			if(path == null)
				return text.GetHashCode();
			else
				return (path.Url + text).GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if(obj == null || obj.GetType() != this.GetType())
				return false;

			var other = (FileInfo)obj;

			if(_size != other._size || !Zongsoft.Collections.BinaryComparer.Default.Equals(_checksum, other._checksum))
				return false;

			var path = this.Path;
			return path == null ? true : path.Equals(other.Path);
		}
		#endregion
	}
}

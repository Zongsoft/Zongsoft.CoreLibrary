/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2011-2013 Zongsoft Corporation <http://www.zongsoft.com>
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
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zongsoft.Communication
{
	public class Package : Zongsoft.Runtime.Serialization.ISerializable, IDisposable
	{
		#region 成员字段
		private string _url;
		private PackageHeaderCollection<Package> _headers;
		private PackageContentCollection _contents;
		#endregion

		#region 构造函数
		public Package(string url)
		{
			if(string.IsNullOrWhiteSpace(url))
				throw new ArgumentNullException("url");

			_url = url.Trim();
			_headers = new PackageHeaderCollection<Package>(this);
			_contents = new PackageContentCollection(this);
		}
		#endregion

		#region 公共属性
		public string Url
		{
			get
			{
				return _url;
			}
			set
			{
				if(string.IsNullOrWhiteSpace(value))
					throw new ArgumentNullException();

				_url = value.Trim();
			}
		}

		public PackageHeaderCollection<Package> Headers
		{
			get
			{
				return _headers;
			}
		}

		public PackageContentCollection Contents
		{
			get
			{
				return _contents;
			}
		}
		#endregion

		#region 序列方法
		public void Serialize(Stream serializationStream)
		{
			PackageSerializer.Default.Serialize(serializationStream, this);
		}
		#endregion

		#region 处置方法
		public void Dispose()
		{
			if(_contents == null)
				return;

			foreach(var content in _contents)
			{
				if(content != null)
				{
					if(content.ContentStream != null)
						content.ContentStream.Dispose();

					content.ContentBuffer = null;
					content.ContentStream = null;
				}
			}
		}
		#endregion
	}
}

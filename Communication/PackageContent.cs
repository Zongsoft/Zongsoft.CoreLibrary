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
using System.Text;

namespace Zongsoft.Communication
{
	public class PackageContent : Zongsoft.Runtime.Serialization.ISerializable
	{
		#region 私有变量
		private readonly object _syncRoot;
		#endregion

		#region 成员字段
		private PackageHeaderCollection<PackageContent> _headers;
		private byte[] _contentBuffer;
		private Stream _contentStream;
		private int _contentLength;
		#endregion

		#region 构造函数
		public PackageContent()
		{
			_syncRoot = new object();
		}
		#endregion

		#region 公共属性
		public PackageHeaderCollection<PackageContent> Headers
		{
			get
			{
				if(_headers == null)
					System.Threading.Interlocked.CompareExchange(ref _headers, new PackageHeaderCollection<PackageContent>(this), null);

				return _headers;
			}
		}

		public int ContentLength
		{
			get
			{
				if(_contentLength <= 0)
				{
					if(_contentBuffer != null)
						return _contentBuffer.Length;
					if(_contentStream != null && _contentStream.CanSeek)
						return (int)_contentStream.Length;
				}

				return _contentLength;
			}
			set
			{
				_contentLength = value;
			}
		}

		public byte[] ContentBuffer
		{
			get
			{
				return _contentBuffer;
			}
			set
			{
				_contentBuffer = value;
			}
		}

		public Stream ContentStream
		{
			get
			{
				return _contentStream;
			}
			set
			{
				_contentStream = value;
			}
		}
		#endregion

		public virtual void Serialize(Stream serializationStream)
		{
			if(_headers == null || _headers.Count == 0)
			{
				serializationStream.WriteByte(0);
			}
			else
			{
				serializationStream.WriteByte((byte)_headers.Count);

				foreach(var header in _headers)
				{
					header.Serialize(serializationStream);
				}
			}

			if(_contentBuffer != null)
			{
				//写入内容数组的实际长度
				serializationStream.Write(BitConverter.GetBytes(this.ContentLength), 0, 4);
				//将内容数组全部写入序列化流
				serializationStream.Write(_contentBuffer, 0, this.ContentLength);
			}
			else if(_contentStream != null)
			{
				//写入内容流的实际长度
				serializationStream.Write(BitConverter.GetBytes(this.ContentLength), 0, 4);

				byte[] buffer = new byte[1024];
				int bytesRead = 0;
				int availableLength = this.ContentLength;

				while(availableLength > 0 && (bytesRead = _contentStream.Read(buffer, 0, Math.Min(buffer.Length, availableLength))) > 0)
				{
					serializationStream.Write(buffer, 0, bytesRead);
					availableLength -= bytesRead;
				}
			}
		}
	}
}

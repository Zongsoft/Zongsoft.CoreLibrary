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

using Zongsoft.Runtime.Caching;
using Zongsoft.Runtime.Serialization;

namespace Zongsoft.Communication
{
	public class PackageSerializer : Zongsoft.Runtime.Serialization.ISerializer
	{
		#region 成员字段
		private IBufferManager _bufferManager;
		#endregion

		#region 构造函数
		public PackageSerializer()
		{
		}

		public PackageSerializer(IBufferManager bufferManager)
		{
			if(bufferManager == null)
				throw new ArgumentNullException("bufferManager");

			_bufferManager = bufferManager;
		}
		#endregion

		#region 公共属性
		public IBufferManager BufferManager
		{
			get
			{
				if(_bufferManager == null)
				{
					string filePath = Path.Combine(Path.GetTempPath(), this.GetType().FullName + ".cache");
					System.Threading.Interlocked.CompareExchange(ref _bufferManager, new BufferManager(filePath), null);
				}

				return _bufferManager;
			}
			set
			{
				if(value == null)
					throw new ArgumentNullException();

				_bufferManager = value;
			}
		}
		#endregion

		#region 反序列化
		public object Deserialize(Stream serializationStream)
		{
			if(serializationStream == null)
				throw new ArgumentNullException("serializationStream");

			if(this.BufferManager == null)
				throw new InvalidOperationException("No available BufferManager object.");

			int lower = serializationStream.ReadByte();
			int upper = serializationStream.ReadByte();

			if(lower < 0 || upper < 0)
				return null;

			var temp = new byte[lower + (upper << 8)];
			if(serializationStream.Read(temp, 0, temp.Length) != temp.Length)
				return null;

			var package = new Package(Zongsoft.Common.UrlUtility.UrlDecode(Encoding.ASCII.GetString(temp)));

			int headerCount = serializationStream.ReadByte();
			int contentCount = serializationStream.ReadByte();

			if(headerCount > 0)
				this.DeserializeHeaders(serializationStream, package.Headers, headerCount);
			if(contentCount > 0)
				this.DeserializeContents(serializationStream, package, contentCount);

			return package;
		}

		private void DeserializeHeaders(Stream serializationStream, ICollection<PackageHeader> headers, int count)
		{
			int nameLength, valueLength;
			string name = string.Empty;
			string value = null;
			byte[] temp = new byte[0xFF];

			for(int i = 0; i < count; i++)
			{
				nameLength = serializationStream.ReadByte();
				valueLength = serializationStream.ReadByte();

				if(nameLength > 0 && serializationStream.Read(temp, 0, nameLength) == nameLength)
					name = Encoding.UTF8.GetString(temp, 0, nameLength);

				if(valueLength > 0 && serializationStream.Read(temp, 0, valueLength) == valueLength)
					value = Encoding.UTF8.GetString(temp, 0, valueLength);

				if(!string.IsNullOrEmpty(name))
					headers.Add(new PackageHeader(name, value));
			}
		}

		private void DeserializeContents(Stream serializationStream, Package package, int count)
		{
			int headerCount;
			byte[] temp = new byte[4];

			for(int i = 0; i < count; i++)
			{
				PackageContent content = new PackageContent();
				headerCount = serializationStream.ReadByte();
				this.DeserializeHeaders(serializationStream, content.Headers, headerCount);

				if(serializationStream.Read(temp, 0, 4) == 4)
				{
					int contentLength = BitConverter.ToInt32(temp, 0);
					int id = _bufferManager.Allocate(contentLength);
					_bufferManager.Write(id, serializationStream, contentLength);
					content.ContentStream = _bufferManager.GetStream(id);
				}

				package.Contents.Add(content);
			}
		}
		#endregion

		#region 序列化
		public void Serialize(Stream serializationStream, object graph)
		{
			if(graph == null)
				return;

			if(serializationStream == null)
				throw new ArgumentNullException("serializationStream");

			Package package = graph as Package;

			if(package == null)
				throw new NotSupportedException();

			package.Serialize(serializationStream);
		}
		#endregion
	}
}

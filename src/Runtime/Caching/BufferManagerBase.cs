/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2013 Zongsoft Corporation <http://www.zongsoft.com>
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

namespace Zongsoft.Runtime.Caching
{
	public abstract class BufferManagerBase : IBufferManager
	{
		#region 成员字段
		private int _blockSize;
		#endregion

		#region 构造函数
		/// <summary>
		/// 默认构造函数，默认<see cref="BlockSize"/>为32KB。
		/// </summary>
		protected BufferManagerBase() : this(32 * BufferUtility.KB)
		{
		}

		/// <summary>
		/// 以指定的<paramref name="blockSize"/>构建一个实例。
		/// </summary>
		/// <param name="blockSize">指定的存储块大小，单位为字节。</param>
		/// <exception cref="ArgumentOutOfRangeException">当<paramref name="blockSize"/>参数值大于4MB。</exception>
		/// <remarks>如果指定的<paramref name="blockSize"/>参数值小于1024，则强制更新为1024。</remarks>
		protected BufferManagerBase(int blockSize)
		{
			if(blockSize > 4 * BufferUtility.MB)
				throw new ArgumentOutOfRangeException("blockSize");

			_blockSize = Math.Max(blockSize, BufferUtility.KB);
		}
		#endregion

		#region 公共属性
		public int BlockSize
		{
			get
			{
				return _blockSize;
			}
		}
		#endregion

		#region 抽象方法
		public abstract int Allocate(long size);
		public abstract void Release(int id);
		public abstract Stream GetStream(int id);
		#endregion

		#region 读取方法
		public int Read(int id, Stream stream, int count)
		{
			return this.Read(id, -1, stream, count);
		}

		public virtual int Read(int id, long position, Stream stream, int count)
		{
			if(stream == null)
				throw new ArgumentNullException("stream");

			if(count < 0)
				throw new ArgumentOutOfRangeException("count");

			var bufferStream = this.GetStream(id);

			if(bufferStream == null)
				throw new InvalidOperationException("Invalid id of buffer.");

			if(position >= 0)
				bufferStream.Position = position;

			int length = count;
			int bytesRead, totalRead = 0;
			var buffer = new byte[Math.Min(_blockSize, count)];

			while(length > 0 && (bytesRead = bufferStream.Read(buffer, 0, Math.Min(buffer.Length, length))) > 0)
			{
				stream.Write(buffer, 0, bytesRead);

				totalRead += bytesRead;
				length -= bytesRead;
			}

			return totalRead;
		}

		public int Read(int id, byte[] buffer, int offset, int count)
		{
			return this.Read(id, -1, buffer, offset, count);
		}

		public virtual int Read(int id, long position, byte[] buffer, int offset, int count)
		{
			if(buffer == null)
				throw new ArgumentNullException("buffer");

			if(offset < 0)
				throw new ArgumentOutOfRangeException("offset");

			if(count < 0)
				throw new ArgumentOutOfRangeException("count");

			if(offset + count > buffer.Length)
				throw new ArgumentException();

			var bufferStream = this.GetStream(id);

			if(bufferStream == null)
				throw new InvalidOperationException("Invalid id of buffer.");

			if(position >= 0)
				bufferStream.Position = position;

			return bufferStream.Read(buffer, offset, count);
		}
		#endregion

		#region 写入方法
		public void Write(int id, Stream stream, int count)
		{
			this.Write(id, -1, stream, count);
		}

		public virtual void Write(int id, long position, Stream stream, int count)
		{
			if(stream == null)
				throw new ArgumentNullException("stream");

			if(count < 0)
				throw new ArgumentOutOfRangeException("count");

			var bufferStream = this.GetStream(id);

			if(bufferStream == null)
				throw new InvalidOperationException("Invalid id of buffer.");

			if(position >= 0)
				bufferStream.Position = position;

			int bytesRead, length = count;
			var buffer = new byte[Math.Min(_blockSize, count)];

			while(length > 0 && (bytesRead = stream.Read(buffer, 0, Math.Min(buffer.Length, length))) > 0)
			{
				bufferStream.Write(buffer, 0, bytesRead);
				length -= bytesRead;
			}
		}

		public void Write(int id, byte[] buffer, int offset, int count)
		{
			this.Write(id, -1, buffer, offset, count);
		}

		public virtual void Write(int id, long position, byte[] buffer, int offset, int count)
		{
			if(buffer == null)
				throw new ArgumentNullException("buffer");

			if(offset < 0)
				throw new ArgumentOutOfRangeException("offset");

			if(count < 0)
				throw new ArgumentOutOfRangeException("count");

			if(offset + count > buffer.Length)
				throw new ArgumentException();

			var bufferStream = this.GetStream(id);

			if(bufferStream == null)
				throw new InvalidOperationException("Invalid id of buffer.");

			if(position >= 0)
				bufferStream.Position = position;

			bufferStream.Write(buffer, offset, count);
		}
		#endregion
	}
}

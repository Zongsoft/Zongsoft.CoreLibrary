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

namespace Zongsoft.Common
{
	public class Buffer
	{
		#region 成员字段
		private byte[] _value;
		private int _offset;
		private int _count;
		private int _position;
		#endregion

		#region 构造函数
		public Buffer(byte[] value) : this(value, 0, -1)
		{
		}

		public Buffer(byte[] value, int offset) : this(value, offset, -1)
		{
		}

		public Buffer(byte[] value, int offset, int count)
		{
			if(value == null)
				throw new ArgumentNullException("value");

			if(offset < 0 || offset >= value.Length)
				throw new ArgumentOutOfRangeException("offset");

			if(count < 0)
				count = value.Length - offset;

			if(offset + count > value.Length)
				throw new ArgumentOutOfRangeException("count");

			_value = value;
			_offset = offset;
			_count = count;
		}
		#endregion

		#region 公共属性
		public byte[] Value
		{
			get
			{
				return _value;
			}
		}

		public int Offset
		{
			get
			{
				return _offset;
			}
		}

		public int Count
		{
			get
			{
				return _count;
			}
		}

		public int Position
		{
			get
			{
				return _position;
			}
		}
		#endregion

		#region 公共方法
		public bool CanRead()
		{
			return _position < _count;
		}

		public int Read(Stream stream, int count)
		{
			if(stream == null)
				throw new ArgumentNullException("stream");

			if(count < 1)
				return 0;

			int position = _position;
			int availableLength = Math.Min(count, _count - position);

			if(availableLength > 0)
			{
				stream.Write(_value, _offset + position, availableLength);
				_position = position + availableLength;
			}

			return availableLength;
		}

		public int Read(byte[] buffer, int offset, int count)
		{
			if(buffer == null)
				throw new ArgumentNullException("buffer");

			if(offset < 0)
				throw new ArgumentOutOfRangeException("offset");

			if(count < 1 || offset + count > buffer.Length)
				return 0;

			int position = _position;
			int availableLength = Math.Min(count, _count - position);

			if(availableLength > 0)
			{
				System.Buffer.BlockCopy(_value, _offset + position, buffer, offset, availableLength);
				_position = position + availableLength;
			}

			return availableLength;
		}
		#endregion
	}
}

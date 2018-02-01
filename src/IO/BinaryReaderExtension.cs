/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2010-2018 Zongsoft Corporation <http://www.zongsoft.com>
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
using System.Text;
using System.Threading.Tasks;

namespace Zongsoft.IO
{
	public static class BinaryReaderExtension
	{
		#region 常量定义
		private const int BUFFER_SIZE = 1024;
		#endregion

		public static void CopyTo(this BinaryReader reader, Stream destination, int bufferSize = BUFFER_SIZE)
		{
			if(reader == null)
				throw new ArgumentNullException(nameof(reader));
			if(destination == null)
				throw new ArgumentNullException(nameof(destination));

			if(!destination.CanWrite)
				throw new NotSupportedException("The destination stream does not support writing.");

			var buffer = new byte[Math.Max(bufferSize, BUFFER_SIZE)];
			var bufferRead = 0;

			while((bufferRead = reader.Read(buffer, 0, buffer.Length)) > 0)
			{
				destination.Write(buffer, 0, bufferRead);
			}
		}

		public static async Task CopyToAsync(this BinaryReader reader, Stream destination, int bufferSize = BUFFER_SIZE)
		{
			if(reader == null)
				throw new ArgumentNullException(nameof(reader));
			if(destination == null)
				throw new ArgumentNullException(nameof(destination));

			if(!destination.CanWrite)
				throw new NotSupportedException("The destination stream does not support writing.");

			var buffer = new byte[Math.Max(bufferSize, BUFFER_SIZE)];
			var bufferRead = 0;

			while((bufferRead = reader.Read(buffer, 0, buffer.Length)) > 0)
			{
				await destination.WriteAsync(buffer, 0, bufferRead);
			}
		}
	}
}

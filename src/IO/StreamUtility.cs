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
using System.IO;

namespace Zongsoft.IO
{
	public static class StreamUtility
	{
		public static long Copy(Stream source, Stream destination, int bufferSize = 4096)
		{
			if(source == null)
				throw new ArgumentNullException("source");

			if(destination == null)
				throw new ArgumentNullException("destination");

			var buffer = new byte[Math.Max(bufferSize, 16)];
			int readedLength = 0;
			long totalLength = 0;

			while((readedLength = source.Read(buffer, 0, buffer.Length)) > 0)
			{
				destination.Write(buffer, 0, readedLength);
				totalLength += (long)readedLength;
			}

			return totalLength;
		}
	}
}

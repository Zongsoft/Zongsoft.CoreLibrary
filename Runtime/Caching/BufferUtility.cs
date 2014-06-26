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
using System.Collections.Generic;

namespace Zongsoft.Runtime.Caching
{
	internal static class BufferUtility
	{
		#region 常量定义
		public const int KB = 1024;
		public const int MB = 1024 * KB;
		public const int GB = 1024 * MB;

		public const int Kilobytes = KB;
		public const int Megabytes = MB;
		public const int Gigabytes = GB;
		#endregion

		private static readonly DateTime BaseTimestamp = new DateTime(2000, 1, 1);

		public static uint GetTimestamp()
		{
			return (uint)(DateTime.Now - BaseTimestamp).TotalSeconds;
		}

		public static TimeSpan GetTimestampSpan(uint timestamp)
		{
			return TimeSpan.FromSeconds(GetTimestamp() - timestamp);
		}
	}
}

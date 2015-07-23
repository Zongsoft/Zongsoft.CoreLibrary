/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2012 Zongsoft Corporation <http://www.zongsoft.com>
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
using System.Collections;
using System.Collections.Generic;

namespace Zongsoft.Collections
{
	public class BinaryComparer : IEqualityComparer, IEqualityComparer<byte[]>
	{
		public static readonly BinaryComparer Default = new BinaryComparer();

		public bool Equals(byte[] x, byte[] y)
		{
			if(x == null && y == null)
				return true;

			if(x == null || y == null || x.Length != y.Length)
				return false;

			for(int i = 0; i < x.Length; i++)
			{
				if(x[i] != y[i])
					return false;
			}

			return true;
		}

		public int GetHashCode(byte[] obj)
		{
			if(obj == null || obj.Length == 0)
				return 0;

			if(obj.Length == 4)
				return BitConverter.ToInt32(obj, 0);

			int result = 0;

			for(int i = 0; i < obj.Length; i++)
			{
				result ^= obj[i] << (i % 4) * 8;
			}

			return result;
		}

		public new bool Equals(object x, object y)
		{
			if(x == null && y == null)
				return true;

			if(x == null || y == null || x.GetType() != typeof(byte[]) || y.GetType() != typeof(byte[]))
				return false;

			return this.Equals((byte[])x, (byte[])y);
		}

		public int GetHashCode(object obj)
		{
			if(obj == null || obj.GetType() != typeof(byte[]))
				return 0;

			return this.GetHashCode((byte[])obj);
		}
	}
}

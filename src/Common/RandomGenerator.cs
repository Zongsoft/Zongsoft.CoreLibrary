/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *   喻星(Xing Yu) <yx@automao.cn>
 *
 * Copyright (C) 2015 Zongsoft Corporation <http://www.zongsoft.com>
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
using System.Security.Cryptography;

namespace Zongsoft.Common
{
	public static class RandomGenerator
	{
		#region 常量定义
		private const string Digits = "0123456789ABCDEFGHJKMNPRSTUVWXYZ";
		private static readonly System.Security.Cryptography.RandomNumberGenerator _random = System.Security.Cryptography.RandomNumberGenerator.Create();
		#endregion

		#region 公共方法
		public static byte[] Generate(int length)
		{
			if(length < 1)
				throw new ArgumentOutOfRangeException("length");

			var bytes = new byte[length];
			_random.GetBytes(bytes);
			return bytes;
		}

		public static long GenerateInt64()
		{
			var bytes = new byte[8];
			_random.GetBytes(bytes);
			return BitConverter.ToInt64(bytes, 0);
		}

		public static int GenerateInt32()
		{
			var bytes = new byte[4];
			_random.GetBytes(bytes);
			return BitConverter.ToInt32(bytes, 0);
		}

		public static string GenerateString(int length = 8)
		{
			if(length < 1 || length > 128)
				throw new ArgumentOutOfRangeException("length");

			var result = new char[length];
			var data = new byte[length];

			_random.GetBytes(data);

			//确保首位字符始终为数字字符
			result[0] = Digits[data[0] %10];

			for(int i = 1; i < length; i++)
			{
				result[i] = Digits[data[i] % 32];
			}

			return new string(result);
		}

		[Obsolete]
		public static string GenerateStringEx(int length = 8)
		{
			if(length < 1 || length > 128)
				throw new ArgumentOutOfRangeException("length");

			var result = new char[length];
			var data = new byte[(int)Math.Ceiling((length * 5) / 8.0)];

			_random.GetBytes(data);

			int value;

			for(int i = 0; i < length; i++)
			{
				int index = i * 5 / 8;
				var bitCount = i * 5 % 8;//当前字节中已获取的位数
				var takeCount = 8 - bitCount;

				if(takeCount < 5)
				{
					value = (((byte)(255 << bitCount)) & data[index]) >> bitCount;
					var count = 8 - (5 - takeCount);
					value += ((byte)(data[index + 1] << count) >> (count - takeCount));
				}
				else
					value = data[index] & (((255 >> (takeCount - 5)) - (255 >> takeCount)) >> bitCount);

				result[i] = Digits[value % 32];
			}

			return new string(result);
		}
		#endregion
	}
}

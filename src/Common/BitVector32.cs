/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2011-2015 Zongsoft Corporation <http://www.zongsoft.com>
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
using System.Threading;

namespace Zongsoft.Common
{
	public struct BitVector32
	{
		#region 成员字段
		private int _data;
		#endregion

		#region 构造函数
		public BitVector32(int data)
		{
			_data = data;
		}
		#endregion

		#region 公共属性
		public int Data
		{
			get
			{
				return _data;
			}
		}

		public bool this[int bit]
		{
			get
			{
				var data = _data;
				return (data & bit) == bit;
			}
			set
			{
				while(true)
				{
					var oldData = _data;
					int newData;

					if(value)
						newData = oldData | bit;
					else
						newData = oldData & ~bit;

					var result = Interlocked.CompareExchange(ref _data, newData, oldData);

					if(result == oldData)
						break;
				}
			}
		}
		#endregion

		#region 类型转换
		public static implicit operator int(BitVector32 vector)
		{
			return vector._data;
		}

		public static implicit operator BitVector32(int data)
		{
			return new BitVector32(data);
		}
		#endregion

		#region 重写方法
		public override string ToString()
		{
			return _data.ToString();
		}
		#endregion
	}
}

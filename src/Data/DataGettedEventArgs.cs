/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2016-2017 Zongsoft Corporation <http://www.zongsoft.com>
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

namespace Zongsoft.Data
{
	/// <summary>
	/// 为数据服务的获取事件提供数据。
	/// </summary>
	public class DataGettedEventArgs<T> : DataAccessEventArgs<DataSelectContext>
	{
		#region 构造函数
		public DataGettedEventArgs(DataSelectContext context) : base(context)
		{
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取查询操作的首条数据。
		/// </summary>
		public T Result
		{
			get
			{
				var items = this.Context.Result;

				if(items != null)
				{
					var iterator = items.GetEnumerator();

					if(iterator != null && iterator.MoveNext())
						return (T)iterator.Current;
				}

				return default(T);
			}
		}
		#endregion
	}
}

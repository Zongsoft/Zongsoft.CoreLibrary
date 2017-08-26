/*
 * Authors:
 *   钟峰(Popeye Zhong) <9555843@qq.com>
 *
 * Copyright (C) 2017 Zongsoft Corporation <http://www.zongsoft.com>
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

namespace Zongsoft.Transitions
{
	public struct State<TKey, TValue> where TKey : struct where TValue : struct
	{
		#region 构造函数
		public State(TKey key, TValue value)
		{
			this.Key = key;
			this.Value = value;
			this.Timestamp = DateTime.Now;
			this.Description = null;
		}

		public State(TKey key, TValue value, string description)
		{
			this.Key = key;
			this.Value = value;
			this.Timestamp = DateTime.Now;
			this.Description = description;
		}

		public State(TKey key, TValue value, DateTime timestamp, string description)
		{
			this.Key = key;
			this.Value = value;
			this.Timestamp = timestamp;
			this.Description = description;
		}
		#endregion

		#region 公共属性
		public TKey Key;
		public TValue Value;
		public DateTime Timestamp;
		public string Description;
		#endregion
	}
}

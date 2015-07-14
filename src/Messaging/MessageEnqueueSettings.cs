/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
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
using System.Collections.Generic;

namespace Zongsoft.Messaging
{
	public class MessageEnqueueSettings
	{
		#region 成员字段
		private TimeSpan _delayTimeout;
		private byte _priority;
		#endregion

		#region 构造函数
		public MessageEnqueueSettings() : this(TimeSpan.Zero)
		{
		}

		public MessageEnqueueSettings(byte priority) : this(TimeSpan.Zero)
		{
		}

		public MessageEnqueueSettings(TimeSpan delayTimeout, byte priority = 6)
		{
			_delayTimeout = delayTimeout;
			_priority = priority;
		}
		#endregion

		#region 公共属性
		public TimeSpan DelayTimeout
		{
			get
			{
				return _delayTimeout;
			}
			set
			{
				_delayTimeout = value;
			}
		}

		public byte Priority
		{
			get
			{
				return _priority;
			}
			set
			{
				_priority = value;
			}
		}
		#endregion
	}
}

/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2010-2013 Zongsoft Corporation <http://www.zongsoft.com>
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
using System.Text;

namespace Zongsoft.Diagnostics
{
	[Serializable]
	public class FailureHandleEventArgs : FailureEventArgs
	{
		#region 成员字段
		private bool _handled;
		#endregion

		#region 构造函数
		public FailureHandleEventArgs(Exception exception) : this(exception, null, false)
		{
		}

		public FailureHandleEventArgs(Exception exception, object state) : this(exception, state, false)
		{
		}

		public FailureHandleEventArgs(Exception exception, object state, bool handled) : base(exception, state)
		{
			_handled = handled;
		}

		public FailureHandleEventArgs(string message) : this(message, null, false)
		{
		}

		public FailureHandleEventArgs(string message, object state) : this(message, state, false)
		{
		}

		public FailureHandleEventArgs(string message, object state, bool handled) : base(message, state)
		{
			_handled = handled;
		}
		#endregion

		#region 公共属性
		public bool Handled
		{
			get
			{
				return _handled;
			}
			set
			{
				_handled = value;
			}
		}
		#endregion
	}
}

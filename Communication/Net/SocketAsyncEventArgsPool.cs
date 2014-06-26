/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2011-2013 Zongsoft Corporation <http://www.zongsoft.com>
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
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Threading;
using System.Text;

namespace Zongsoft.Communication.Net
{
	internal class SocketAsyncEventArgsPool : Zongsoft.Common.ObjectPool<SocketAsyncEventArgs>
	{
		#region 成员变量
		private Action<object, SocketAsyncEventArgs> _completedCallback;
		#endregion

		#region 构造函数
		internal SocketAsyncEventArgsPool(Action<object, SocketAsyncEventArgs> completed) : base(100)
		{
			if(completed == null)
				throw new ArgumentNullException("completed");

			_completedCallback = completed;
		}
		#endregion

		#region 重写方法
		protected override SocketAsyncEventArgs OnCreate()
		{
			return new SocketAsyncEventArgs()
			{
				SocketFlags = SocketFlags.Partial,
			};
		}

		protected override void OnTakeout(SocketAsyncEventArgs value)
		{
			value.Completed += AsyncArgs_Completed;
		}

		protected override void OnTakein(SocketAsyncEventArgs value)
		{
			value.AcceptSocket = null;
			value.UserToken = null;
			value.SetBuffer(null, 0, 0);

			value.Completed -= AsyncArgs_Completed;
		}

		protected override void OnRemove(SocketAsyncEventArgs value)
		{
			value.Completed -= AsyncArgs_Completed;
			value.Dispose();
		}
		#endregion

		#region 私有方法
		private void AsyncArgs_Completed(object sender, SocketAsyncEventArgs args)
		{
			if(_completedCallback != null)
				_completedCallback(sender, args);
		}
		#endregion
	}
}

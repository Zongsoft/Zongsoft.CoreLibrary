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
using System.IO;
using System.Collections.Generic;
using System.Text;

using Zongsoft.Services;
using Zongsoft.Services.Composition;

namespace Zongsoft.Communication
{
	public class RequestContext : Zongsoft.Services.Composition.ExecutorContext
	{
		#region 成员变量
		private IChannel _channel;
		#endregion

		#region 构造函数
		internal RequestContext(Executor executor, ReceivedEventArgs args) : base(executor, args)
		{
			if(args == null)
				throw new ArgumentNullException("args");

			_channel = args.Channel;
			this.Parameter = args.ReceivedObject;
		}

		public RequestContext(Executor executor, IChannel channel, object receivedObject) : base(executor, receivedObject)
		{
			_channel = channel;
		}
		#endregion

		#region 公共属性
		public IChannel Channel
		{
			get
			{
				return _channel;
			}
		}

		public object ReceivedObject
		{
			get
			{
				return this.Parameter;
			}
		}
		#endregion
	}
}

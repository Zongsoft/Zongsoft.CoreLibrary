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
using System.IO;
using System.Collections.Generic;

using Zongsoft.Services;
using Zongsoft.Services.Composition;

namespace Zongsoft.Communication
{
	internal static class Utility
	{
		#region 公共方法
		public static void ProcessReceive(IExecutor executor, ReceivedEventArgs args)
		{
			if(args == null)
				throw new ArgumentNullException("args");

			//如果执行器参数为空，不抛出异常，直接退出
			if(executor == null)
				return;

			//通过执行器执行当前请求
			executor.Execute(args);
		}
		#endregion

		#region 嵌套子类
		public class CommunicationExecutor : Zongsoft.Services.Composition.Executor
		{
			internal CommunicationExecutor(object host) : base(host)
			{
			}

			protected override ExecutionContext CreateContext(object parameter)
			{
				var args = parameter as ReceivedEventArgs;

				if(args != null)
					return new ChannelContext(this, args.ReceivedObject, args.Channel);

				return base.CreateContext(parameter);
			}

			protected override ExecutionPipelineContext CreatePipelineContext(ExecutionContext context, ExecutionPipeline pipeline, object parameter)
			{
				var channelContext = context as IChannelContext;

				if(channelContext != null)
					return new ChannelPipelineContext(context, pipeline, parameter, channelContext.Channel);

				return base.CreatePipelineContext(context, pipeline, parameter);
			}
		}
		#endregion
	}
}

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

namespace Zongsoft.Diagnostics
{
	public class TracerModule : Zongsoft.ComponentModel.IModule, IDisposable
	{
		#region 公共属性
		public string Name
		{
			get
			{
				return this.GetType().Name;
			}
		}
		#endregion

		#region 初始化器
		public virtual void Initialize(Zongsoft.ComponentModel.ApplicationContextBase context)
		{
			//判断当前应用上下文中是否有主配置对象
			if(context.Configuration == null)
				return;

			//获取住配置中的跟踪配置节
			var element = context.Configuration.GetOptionObject("diagnostics/tracer") as Configuration.TracerElement;

			if(element != null && element.Enabled)
			{
				foreach(Configuration.TraceListenerElement listenerElement in element.Listeners)
				{
					var listener = listenerElement.Listener;

					if(listener != null)
						context.Tracer.Listeners.Add(listener);
				}
			}
		}

		void Zongsoft.ComponentModel.IModule.Initialize(object context)
		{
			this.Initialize(context as Zongsoft.ComponentModel.ApplicationContextBase);
		}
		#endregion

		#region 释放资源
		void IDisposable.Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
		}
		#endregion
	}
}

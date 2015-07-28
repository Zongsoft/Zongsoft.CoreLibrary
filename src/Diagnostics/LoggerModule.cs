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

namespace Zongsoft.Diagnostics
{
	public class LoggerModule : Zongsoft.ComponentModel.IModule, IDisposable
	{
		#region 公共属性
		public virtual string Name
		{
			get
			{
				return this.GetType().Name;
			}
		}
		#endregion

		#region 公共方法
		public virtual void Initialize(Zongsoft.ComponentModel.ApplicationContextBase context)
		{
			if(context == null)
				return;

			var loggerElement = context.Configuration.GetOptionObject(@"/Diagnostics/Logger") as Configuration.LoggerElement;

			if(loggerElement != null)
			{
				foreach(Configuration.LoggerHandlerElement handlerElement in loggerElement.Handlers)
				{
					var type = Type.GetType(handlerElement.Type, true, true);

					if(!typeof(ILogger).IsAssignableFrom(type))
						throw new Options.Configuration.OptionConfigurationException();

					var instance = (ILogger)Activator.CreateInstance(type);

					if(instance == null)
						throw new Options.Configuration.OptionConfigurationException();

					if(handlerElement.Properties != null && handlerElement.Properties.Count > 0)
					{
						foreach(Zongsoft.Options.Configuration.SettingElement property in handlerElement.Properties)
						{
							Zongsoft.Common.Convert.SetValue(instance, property.Name, property.Value);
						}
					}

					LoggerHandlerPredication predication = null;

					if(handlerElement.Predication != null)
					{
						predication = new LoggerHandlerPredication()
						{
							Source = handlerElement.Predication.Source,
							ExceptionType = handlerElement.Predication.ExceptionType,
							MaxLevel = handlerElement.Predication.MaxLevel,
							MinLevel = handlerElement.Predication.MinLevel,
						};
					}

					Logger.Handlers.Add(new LoggerHandler(handlerElement.Name, instance, predication));
				}
			}
		}
		#endregion

		#region 显式实现
		void Zongsoft.ComponentModel.IModule.Initialize(object context)
		{
			this.Initialize(context as Zongsoft.ComponentModel.ApplicationContextBase);
		}
		#endregion

		#region 处置方法
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

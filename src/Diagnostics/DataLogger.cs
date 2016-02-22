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
	public class DataLogger : ILogger
	{
		#region 成员字段
		private Zongsoft.Data.IDataAccess _dataAccess;
		private Zongsoft.Runtime.Caching.ICache _storage;
		private Configuration.LoggerHandlerElement _configuration;
		#endregion

		#region 公共属性
		public Zongsoft.Data.IDataAccess DataAccess
		{
			get
			{
				return _dataAccess;
			}
			set
			{
				if(value == null)
					throw new ArgumentNullException();

				_dataAccess = value;
			}
		}

		public Zongsoft.Runtime.Caching.ICache Storage
		{
			get
			{
				return _storage;
			}
			set
			{
				if(value == null)
					throw new ArgumentNullException();

				_storage = value;
			}
		}

		public Configuration.LoggerHandlerElement Configuration
		{
			get
			{
				return _configuration;
			}
			set
			{
				if(value == null)
					throw new ArgumentNullException();

				_configuration = value;
			}
		}
		#endregion

		#region 日志方法
		public void Log(LogEntry entry)
		{
			var dataAccess = this.DataAccess;

			if(dataAccess == null)
				return;

			var storage = this.Storage;

			foreach(Zongsoft.Options.Configuration.SettingElement parameter in _configuration.Parameters)
			{
				Logger.TemplateManager.Evaluate<string>(parameter.Value, entry);
			}

			throw new NotImplementedException();
		}
		#endregion
	}
}

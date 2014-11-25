/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2013-2014 Zongsoft Corporation <http://www.zongsoft.com>
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

namespace Zongsoft.Options.Configuration
{
	public static class OptionConfigurationManager
	{
		#region 私有变量
		private readonly static Zongsoft.Collections.ObjectCache<OptionConfiguration> _cache = new Collections.ObjectCache<OptionConfiguration>(0);
		#endregion

		#region 公共方法
		public static OptionConfiguration Open(string filePath, bool createNotExists = false)
		{
			if(string.IsNullOrWhiteSpace(filePath))
				throw new ArgumentNullException("filePath");

			return _cache.Get(filePath.Trim(), key =>
			{
				if(File.Exists(key))
					return OptionConfiguration.Load(key);

				if(createNotExists)
					return new OptionConfiguration(key);

				return null;
			});
		}

		public static void Close(string filePath)
		{
			if(string.IsNullOrWhiteSpace(filePath))
				return;

			_cache.Remove(filePath.Trim());
		}
		#endregion
	}
}

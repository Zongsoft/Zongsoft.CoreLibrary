/*
 *   _____                                ______
 *  /_   /  ____  ____  ____  _________  / __/ /_
 *    / /  / __ \/ __ \/ __ \/ ___/ __ \/ /_/ __/
 *   / /__/ /_/ / / / / /_/ /\_ \/ /_/ / __/ /_
 *  /____/\____/_/ /_/\__  /____/\____/_/  \__/
 *                   /____/
 *
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@qq.com>
 *
 * Copyright (C) 2010-2019 Zongsoft Corporation <http://www.zongsoft.com>
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

namespace Zongsoft.Common
{
	public static class UriExtension
	{
		public static bool TryGetQueryString(this Uri url, string key, out string value)
		{
			bool TryGetPart(string part, out KeyValuePair<string, string> pair)
			{
				if(string.IsNullOrEmpty(part))
				{
					pair = default(KeyValuePair<string, string>);
					return false;
				}

				var index = part.IndexOf('=');

				if(index < 0)
					pair = new KeyValuePair<string, string>(part, string.Empty);
				else
					pair = new KeyValuePair<string, string>(part.Substring(0, index), part.Substring(index + 1));

				return true;
			}

			var parts = StringExtension.Slice<KeyValuePair<string, string>>(url.Query, '&', TryGetPart);

			foreach(var part in parts)
			{
				if(string.Equals(key, part.Key, StringComparison.OrdinalIgnoreCase))
				{
					value = part.Value;
					return true;
				}
			}

			value = null;
			return false;
		}
	}
}

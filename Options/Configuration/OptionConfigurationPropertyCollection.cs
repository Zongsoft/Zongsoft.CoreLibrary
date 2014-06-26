/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2013 Zongsoft Corporation <http://www.zongsoft.com>
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
using System.Collections.ObjectModel;

namespace Zongsoft.Options.Configuration
{
	public class OptionConfigurationPropertyCollection : KeyedCollection<string, OptionConfigurationProperty>
	{
		public OptionConfigurationPropertyCollection() : base(StringComparer.OrdinalIgnoreCase)
		{
		}

		protected override string GetKeyForItem(OptionConfigurationProperty item)
		{
			return item.Name;
		}

		public bool TryGetValue(string name, out OptionConfigurationProperty value)
		{
			value = null;

			if(name == null)
				name = string.Empty;

			if(Contains(name))
			{
				value = this[name];
				return true;
			}

			return false;
		}
	}
}

/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2014 Zongsoft Corporation <http://www.zongsoft.com>
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

namespace Zongsoft.Options.Profiles
{
	public class ProfileSectionCollection : Zongsoft.Collections.NamedCollection<ProfileSection>
	{
		#region 构造函数
		public ProfileSectionCollection(ProfileItemCollection items) : base(items)
		{
		}
		#endregion

		#region 公共方法
		public ProfileSection Add(string name, int lineNumber = -1)
		{
			var item = new ProfileSection(name, lineNumber);
			base.Add(item);
			return item;
		}
		#endregion

		#region 重写方法
		protected override string GetKeyForItem(ProfileSection item)
		{
			return item.Name;
		}

		protected override bool OnItemMatch(object item)
		{
			return ((ProfileItem)item).ItemType == ProfileItemType.Section;
		}
		#endregion
	}
}

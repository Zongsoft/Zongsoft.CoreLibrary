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
	internal class ProfileItemCollection : System.Collections.ObjectModel.ObservableCollection<ProfileItem>
	{
		#region 成员字段
		private object _owner;
		#endregion

		#region 构造函数
		public ProfileItemCollection(object owner)
		{
			_owner = owner ?? throw new ArgumentNullException(nameof(owner));
		}
		#endregion

		#region 内部属性
		internal object Owner
		{
			get
			{
				return _owner;
			}
			set
			{
				if(object.ReferenceEquals(_owner, value))
					return;

				foreach(var item in this.Items)
					item.Owner = value;
			}
		}
		#endregion

		#region 重写方法
		protected override void InsertItem(int index, ProfileItem item)
		{
			if(item != null)
				item.Owner = _owner;

			base.InsertItem(index, item);
		}

		protected override void SetItem(int index, ProfileItem item)
		{
			if(item != null)
				item.Owner = _owner;

			base.SetItem(index, item);
		}
		#endregion
	}
}

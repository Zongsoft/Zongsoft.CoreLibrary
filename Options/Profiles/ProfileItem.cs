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
	public abstract class ProfileItem
	{
		#region 成员字段
		private object _owner;
		private int _lineNumber;
		#endregion

		#region 构造函数
		protected ProfileItem()
		{
			_lineNumber = -1;
		}

		protected ProfileItem(int lineNumber)
		{
			_lineNumber = Math.Max(lineNumber, -1);
		}

		protected ProfileItem(Profile owner, int lineNumber)
		{
			if(owner == null)
				throw new ArgumentNullException("owner");

			_owner = owner;
			_lineNumber = Math.Max(lineNumber, -1);
		}

		protected ProfileItem(ProfileSection owner, int lineNumber)
		{
			if(owner == null)
				throw new ArgumentNullException("owner");

			_owner = owner;
			_lineNumber = Math.Max(lineNumber, -1);
		}
		#endregion

		#region 公共属性
		public virtual Profile Profile
		{
			get
			{
				return _owner as Profile;
			}
		}

		public abstract ProfileItemType ItemType
		{
			get;
		}

		public int LineNumber
		{
			get
			{
				return _lineNumber;
			}
		}
		#endregion

		#region 保护属性
		internal protected object Owner
		{
			get
			{
				return _owner;
			}
			internal set
			{
				if(value == null)
					throw new ArgumentNullException();

				if(object.ReferenceEquals(_owner, value))
					return;

				_owner = value;
				this.OnOwnerChanged(value);
			}
		}
		#endregion

		#region 虚拟方法
		protected virtual void OnOwnerChanged(object owner)
		{
		}
		#endregion
	}
}

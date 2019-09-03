﻿/*
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
 * Copyright (C) 2015-2019 Zongsoft Corporation <http://www.zongsoft.com>
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

namespace Zongsoft.Security.Membership
{
	public class ChangedEventArgs : EventArgs
	{
		#region 构造函数
		public ChangedEventArgs(uint id, string propertyName, object propertyValue)
		{
			this.Id = id;
			this.PropertyName = propertyName;
			this.PropertyValue = PropertyValue;
		}
		#endregion

		#region 公共属性
		public uint Id
		{
			get;
		}

		public string PropertyName
		{
			get;
		}

		public object PropertyValue
		{
			get;
		}
		#endregion

		#region 重写方法
		public override string ToString()
		{
			if(this.PropertyValue == null)
				return $"[{this.Id.ToString()}] {this.PropertyName}=NULL";
			else
				return $"[{this.Id.ToString()}] {this.PropertyName}={this.PropertyValue}";
		}
		#endregion
	}
}

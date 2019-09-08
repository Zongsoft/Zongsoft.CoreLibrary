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

namespace Zongsoft.Data.Metadata
{
	/// <summary>
	/// 表示数据实体关联约束的元数据类。
	/// </summary>
	public struct DataAssociationConstraint
	{
		#region 构造函数
		public DataAssociationConstraint(string name, DataAssociationConstraintActor actor, object value)
		{
			this.Name = name;
			this.Actor = actor;
			this.Value = value;
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取关联约束的目标成员名。
		/// </summary>
		public string Name
		{
			get;
		}

		/// <summary>
		/// 获取关联约束的主体(即约束目标)。
		/// </summary>
		public DataAssociationConstraintActor Actor
		{
			get;
		}

		/// <summary>
		/// 获取关联约束的目标值。
		/// </summary>
		public object Value
		{
			get;
		}
		#endregion

		#region 重写方法
		public override string ToString()
		{
			if(this.Value == null)
				return this.Actor.ToString() + ":" + this.Name + "=NULL";
			else
				return this.Actor.ToString() + ":" + this.Name + "=" + this.Value.ToString();
		}
		#endregion
	}
}

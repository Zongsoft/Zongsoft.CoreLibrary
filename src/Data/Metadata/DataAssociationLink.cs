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
using System.Collections.Generic;

namespace Zongsoft.Data.Metadata
{
	/// <summary>
	/// 表示数据实体关联成员的元数据类。
	/// </summary>
	public class DataAssociationLink
	{
		#region 成员字段
		private IDataEntityComplexProperty _owner;
		private IDataEntitySimplexProperty _principal;
		private IDataEntitySimplexProperty _foreign;
		private readonly string _name;
		private readonly string _role;
		#endregion

		#region 构造函数
		public DataAssociationLink(IDataEntityComplexProperty owner, string name, string role)
		{
			_owner = owner;
			_name = name;
			_role = role;
			_principal = _foreign = null;
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取关联元素的主属性。
		/// </summary>
		public IDataEntitySimplexProperty Principal
		{
			get
			{
				if(_principal == null)
					_principal = (IDataEntitySimplexProperty)_owner.Entity.Properties.Get(_name);

				return _principal;
			}
		}

		/// <summary>
		/// 获取关联元素的外链属性。
		/// </summary>
		public IDataEntitySimplexProperty Foreign
		{
			get
			{
				if(_foreign == null)
					_foreign = (IDataEntitySimplexProperty)_owner.Foreign.Properties.Get(_role);

				return _foreign;
			}
		}

		/// <summary>
		/// 获取关联元素的主属性名。
		/// </summary>
		public string Name
		{
			get => _name;
		}

		/// <summary>
		/// 获取关联元素的外链属性名。
		/// </summary>
		public string Role
		{
			get => _role;
		}
		#endregion

		#region 重写方法
		public override string ToString()
		{
			return _name + "=" + _role;
		}
		#endregion
	}
}

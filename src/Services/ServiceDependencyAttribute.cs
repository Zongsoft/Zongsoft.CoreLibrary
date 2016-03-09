/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2015-2016 Zongsoft Corporation <http://www.zongsoft.com>
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
using System.ComponentModel;

namespace Zongsoft.Services
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = true)]
	public class ServiceDependencyAttribute : Attribute
	{
		#region 成员字段
		private string _name;
		private Type _contract;
		#endregion

		#region 构造函数
		public ServiceDependencyAttribute()
		{
		}

		public ServiceDependencyAttribute(string name)
		{
			this.Name = name;
		}

		public ServiceDependencyAttribute(Type contract)
		{
			this.Contract = contract;
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取或设置服务的名称。
		/// </summary>
		public string Name
		{
			get
			{
				return _name;
			}
			set
			{
				_name = string.IsNullOrWhiteSpace(value) ? null : value.Trim();
			}
		}

		/// <summary>
		/// 获取或设置服务的契约类型。
		/// </summary>
		public Type Contract
		{
			get
			{
				return _contract;
			}
			set
			{
				_contract = value;
			}
		}
		#endregion
	}
}

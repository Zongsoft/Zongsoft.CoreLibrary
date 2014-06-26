/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2003-2010 Zongsoft Corporation <http://www.zongsoft.com>
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
using System.Data;
using System.Collections.Generic;

namespace Zongsoft.Data
{
	[Serializable]
	public class Parameter
	{
		#region 静态字段
		/// <summary>提供单例的关于查询准则的参数对象。该参数名称为：__Criteria__，默认值：AND，方向：输入参数。</summary>
		public static readonly Parameter Criteria = new Parameter("__Criteria__", "AND", ParameterDirection.Input);

		/// <summary>提供单例的关于分页大小的参数对象。该参数名称为：__PageSize__，默认值：20，方向：输入参数。</summary>
		public static readonly Parameter PageSize = new Parameter("__PageSize__", 20, ParameterDirection.Input);

		/// <summary>提供单例的关于分页页号的参数对象。该参数名称为：__PageIndex__，默认值：0，方向：输入参数。</summary>
		public static readonly Parameter PageIndex = new Parameter("__PageIndex__", 0, ParameterDirection.Input);

		/// <summary>提供单例的关于分页页数的参数对象。该参数名称为：__PageCount__，默认值：-1，方向：输出参数。</summary>
		public static readonly Parameter PageCount = new Parameter("__PageCount__", -1, ParameterDirection.Output);

		/// <summary>提供单例的关于记录总数的参数对象。该参数名称为：__TotalCount__，默认值：-1，方向：输出参数。</summary>
		public static readonly Parameter TotalCount = new Parameter("__TotalCount__", -1, ParameterDirection.Output);
		#endregion

		#region 构造函数
		internal Parameter(string name, object value) : this(name, value, ParameterDirection.Input)
		{
		}

		internal Parameter(string name, object value, ParameterDirection direction)
		{
			if(string.IsNullOrEmpty(name))
				throw new ArgumentNullException("name");

			this.Name = name;
			this.Value = value;
			this.Direction = direction;
		}
		#endregion

		#region 公共属性
		public ParameterDirection Direction
		{
			get;
			private set;
		}

		public string Name
		{
			get;
			private set;
		}

		public object Value
		{
			get;
			set;
		}
		#endregion
	}
}

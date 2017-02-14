/*
 * Authors:
 *   钟峰(Popeye Zhong) <9555843@qq.com>
 *
 * Copyright (C) 2010-2017 Zongsoft Corporation <http://www.zongsoft.com>
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

namespace Zongsoft.Reflection
{
	public class MemberToken
	{
		#region 公共字段
		public readonly string Name;
		public readonly object[] Parameters;
		#endregion

		#region 构造函数
		public MemberToken(string name)
		{
			if(string.IsNullOrWhiteSpace(name))
				throw new ArgumentNullException(nameof(name));

			this.Name = name;
			this.Parameters = null;
		}

		public MemberToken(object[] parameters)
		{
			if(parameters == null || parameters.Length == 0)
				throw new ArgumentNullException(nameof(parameters));

			this.Name = string.Empty;
			this.Parameters = parameters;
		}
		#endregion

		#region 公共属性
		public bool IsIndexer
		{
			get
			{
				return this.Parameters != null && this.Parameters.Length > 0;
			}
		}
		#endregion

		#region 重写方法
		public override string ToString()
		{
			if(!string.IsNullOrEmpty(this.Name))
				return this.Name;

			var result = new System.Text.StringBuilder();

			foreach(var parameter in this.Parameters)
			{
				if(result.Length > 0)
					result.Append(", ");

				if(parameter == null)
					result.Append("null");
				else
				{
					if(parameter is string)
						result.Append("\"" + parameter + "\"");
					else
						result.Append(parameter.ToString());
				}
			}

			return "[" + result.ToString() + "]";
		}
		#endregion
	}
}

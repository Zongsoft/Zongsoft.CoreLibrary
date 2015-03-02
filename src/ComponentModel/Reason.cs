/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2008-2010 Zongsoft Corporation <http://www.zongsoft.com>
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
using System.Text;

namespace Zongsoft.ComponentModel
{
	[Serializable]
	public class Reason
	{
		#region 成员变量
		private string _name;
		#endregion

		#region 构造函数
		public Reason(string name)
		{
			this.Name = name;
		}
		#endregion

		#region 公共属性
		public string Name
		{
			get
			{
				return _name;
			}
			protected set
			{
				if(string.IsNullOrEmpty(value))
					throw new ArgumentNullException();

				_name = value;
			}
		}
		#endregion

		#region 重写方法
		public override bool Equals(object obj)
		{
			if(obj == null)
				return false;

			if(obj.GetType() != this.GetType())
				return false;

			if(Object.ReferenceEquals(this, obj))
				return true;

			if(string.Equals(((Reason)obj).Name, _name, StringComparison.OrdinalIgnoreCase))
				return true;

			return false;
		}

		public override int GetHashCode()
		{
			return System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(_name);
		}

		public override string ToString()
		{
			return _name;
		}
		#endregion

		#region 操作符重载
		public static bool operator == (Reason a, Reason b)
		{
			if(object.ReferenceEquals(a, b))
				return true;

			if(((object)a == null) || ((object)b == null))
				return false;

			return string.Equals(a.Name, b.Name, StringComparison.OrdinalIgnoreCase);
		}

		public static bool operator !=(Reason a, Reason b)
		{
			return !(a == b);
		}
		#endregion
	}
}

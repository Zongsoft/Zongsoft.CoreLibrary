/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2015 Zongsoft Corporation <http://www.zongsoft.com>
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

namespace Zongsoft.Services
{
	public abstract class PredicationBase<T> : IPredication<T>, IMatchable<string>
	{
		#region 成员字段
		private string _name;
		#endregion

		#region 构造函数
		protected PredicationBase(string name)
		{
			if(string.IsNullOrWhiteSpace(name))
				throw new ArgumentNullException("name");

			_name = name.Trim();
		}
		#endregion

		#region 公共属性
		public virtual string Name
		{
			get
			{
				return _name;
			}
		}
		#endregion

		#region 断言方法
		public abstract bool Predicate(T parameter);

		bool IPredication.Predicate(object parameter)
		{
			return this.Predicate(this.ConvertParameter(parameter));
		}
		#endregion

		#region 虚拟方法
		protected virtual T ConvertParameter(object parameter)
		{
			return Zongsoft.Common.Convert.ConvertValue<T>(parameter);
		}
		#endregion

		#region 服务匹配
		public virtual bool IsMatch(string parameter)
		{
			return string.Equals(this.Name, parameter, StringComparison.OrdinalIgnoreCase);
		}

		bool IMatchable.IsMatch(object parameter)
		{
			return this.IsMatch(parameter as string);
		}
		#endregion

		#region 重写方法
		public override string ToString()
		{
			return string.Format("{0} ({1})", this.Name, this.GetType());
		}
		#endregion
	}
}

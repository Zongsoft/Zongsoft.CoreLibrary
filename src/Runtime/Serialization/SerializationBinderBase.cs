/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2015-2017 Zongsoft Corporation <http://www.zongsoft.com>
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

namespace Zongsoft.Runtime.Serialization
{
	public abstract class SerializationBinderBase<T> : ISerializationBinder
	{
		#region 构造函数
		protected SerializationBinderBase()
		{
		}
		#endregion

		#region 公共属性
		public virtual bool GetMemberValueSupported
		{
			get
			{
				return false;
			}
		}
		#endregion

		#region 保护方法
		protected abstract Type GetMemberType(string name, T container);

		protected virtual object GetMemberValue(string name, T container, object value)
		{
			return value;
		}
		#endregion

		#region 显式实现
		Type ISerializationBinder.GetMemberType(string name, object container)
		{
			if(container is T)
				return this.GetMemberType(name, (T)container);

			return null;
		}

		object ISerializationBinder.GetMemberValue(string name, object container, object value)
		{
			if(container is T)
				return this.GetMemberValue(name, (T)container, value);

			return value;
		}
		#endregion
	}
}

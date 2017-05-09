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
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = true)]
	public class SerializationBinderAttribute : Attribute
	{
		#region 成员字段
		private Type _binderType;
		#endregion

		#region 构造函数
		public SerializationBinderAttribute(Type binderType)
		{
			if(binderType == null)
				throw new ArgumentNullException(nameof(binderType));

			if(!typeof(ISerializationBinder).IsAssignableFrom(binderType))
				throw new ArgumentException("The binder type no implement ISerializationBinder interface.");

			_binderType = binderType;
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取或设置绑定器的类型。
		/// </summary>
		public Type BinderType
		{
			get
			{
				return _binderType;
			}
			set
			{
				_binderType = value;
			}
		}
		#endregion
	}
}

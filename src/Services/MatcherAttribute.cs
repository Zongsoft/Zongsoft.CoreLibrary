/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2010-2013 Zongsoft Corporation <http://www.zongsoft.com>
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

namespace Zongsoft.Services
{
	[AttributeUsage(AttributeTargets.Class)]
	public class MatcherAttribute : Attribute
	{
		#region 成员字段
		private Type _type;
		#endregion

		#region 构造函数
		public MatcherAttribute(Type type)
		{
			if(type == null)
				throw new ArgumentNullException("type");

			if(!typeof(IMatcher).IsAssignableFrom(type))
				throw new ArgumentException("The type is not a IMatcher.");

			_type = type;
		}

		public MatcherAttribute(string typeName)
		{
			if(string.IsNullOrWhiteSpace(typeName))
				throw new ArgumentNullException("typeName");

			var type = Type.GetType(typeName, false);

			if(type == null || !typeof(IMatcher).IsAssignableFrom(type))
				throw new ArgumentException("The type is not a IMatcher.");

			_type = type;
		}
		#endregion

		#region 公共属性
		public Type Type
		{
			get
			{
				return _type;
			}
		}

		public IMatcher Matcher
		{
			get
			{
				if(_type == null)
					return null;

				return Activator.CreateInstance(_type) as IMatcher;
			}
		}
		#endregion
	}
}

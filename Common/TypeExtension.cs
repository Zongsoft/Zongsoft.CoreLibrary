/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2013 Zongsoft Corporation <http://www.zongsoft.com>
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
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

namespace Zongsoft.Common
{
	public static class TypeExtension
	{
		public static bool IsImplements(this Type instanceType, Type interfaceType)
		{
			if(instanceType == null || interfaceType == null)
				return false;

			var result = instanceType.FindInterfaces((type, criteria) =>
			{
				if(interfaceType.IsGenericType)
				{
					if(interfaceType.IsGenericTypeDefinition)
					{
						if(type.IsGenericType)
						{
							if(type.IsGenericTypeDefinition)
								return type == interfaceType;
							else
								return (type.GetGenericTypeDefinition() == interfaceType);
						}
						else
						{
							return false;
						}
					}
					else
					{
						if(type.IsGenericType)
						{
							var implementArguments = type.GetGenericArguments();
							var interfaceArguments = interfaceType.GetGenericArguments();

							if(implementArguments.Length != interfaceArguments.Length)
								return false;

							for(int i = 0; i < implementArguments.Length; i++)
							{
								if(!interfaceArguments[i].IsAssignableFrom(implementArguments[i]))
									return false;
							}

							return true;
						}
					}
				}

				return interfaceType.IsAssignableFrom(instanceType);
			}, interfaceType);

			return result != null && result.Length > 0;
		}
	}
}

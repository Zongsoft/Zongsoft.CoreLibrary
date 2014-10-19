/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2013-2014 Zongsoft Corporation <http://www.zongsoft.com>
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

namespace Zongsoft.Common
{
	public static class TypeExtension
	{
		/// <summary>
		/// 提供比<see cref="System.Type.IsAssignableFrom"/>加强的功能，支持对泛型定义接口或类的匹配。
		/// </summary>
		/// <param name="instanceType">指定的实例类型。</param>
		/// <param name="type">指定的接口或基类的类型。</param>
		/// <returns>如果当满足如下条件之一则返回真(true)：
		/// <list type="bullet">
		///		<item>
		///			<term>如果 <paramref name="type"/> 为泛型定义类型，则 <paramref name="instanceType"/> 实现的接口或基类中有从 <paramref name="type"/> 指定的泛型定义中泛化的版本。</term>
		///		</item>
		///		<item>
		///			<term>如果 <paramref name="type"/> 和当前 <paramref name="instanceType"/> 表示同一类型；</term>
		///		</item>
		///		<item>
		///			<term>当前 <paramref name="instanceType"/> 位于 <paramref name="type"/> 的继承层次结构中；</term>
		///		</item>
		///		<item>
		///			<term>当前 <paramref name="instanceType"/> 是 <paramref name="type"/> 实现的接口；</term>
		///		</item>
		///		<item>
		///			<term><paramref name="type"/> 是泛型类型参数且当前 <paramref name="instanceType"/> 表示 <paramref name="type"/> 的约束之一。</term>
		///		</item>
		/// </list>
		/// </returns>
		/// <remarks>
		///		<para>除了 <see cref="System.Type.IsAssignableFrom"/> 支持的特性外，增加了如下特性：</para>
		///		<code>
		///		TypeExtension.IsAssignableFrom(typeof(IDictionary<,>), typeof(Dictionary<string, object>));	// true
		///		TypeExtension.IsAssignableFrom(typeof(Dictionary<,>), typeof(Dictioanry<string, int>));		//true
		///		
		///		public class MyNamedCollection<T> : Collection<T>, IDictionary<string, T>
		///		{
		///		}
		///		
		///		TypeExtension.IsAssignableFrom(typeof(IDictionary<,>), typeof(MyNamedCollection<string, object>)); //true
		///		</code>
		/// </remarks>
		public static bool IsAssignableFrom(this Type type, Type instanceType)
		{
			if(type == null)
				throw new ArgumentNullException("type");

			if(instanceType == null)
				throw new ArgumentNullException("instanceType");

			if(type.IsGenericType && type.IsGenericTypeDefinition)
			{
				IEnumerable<Type> baseTypes = null;

				if(type.IsInterface)
					baseTypes = instanceType.GetInterfaces();
				else
				{
					baseTypes = new List<Type>();

					var currentType = instanceType;

					while(currentType != typeof(object) &&
						  currentType != typeof(Enum) &&
						  currentType != typeof(Delegate) &&
						  currentType != typeof(ValueType))
					{
						((List<Type>)baseTypes).Add(currentType);
						currentType = currentType.BaseType;
					}
				}

				foreach(var baseType in baseTypes)
				{
					if(baseType.IsGenericType && baseType.GetGenericTypeDefinition() == type)
					{
						return true;
					}
				}
			}

			return type.IsAssignableFrom(instanceType);
		}
	}
}

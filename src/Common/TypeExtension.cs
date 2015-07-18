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
		/// <param name="type">指定的接口或基类的类型。</param>
		/// <param name="instanceType">指定的实例类型。</param>
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
		///		<example>
		///		<code>
		///		TypeExtension.IsAssignableFrom(typeof(IDictionary&lt;,&gt;), typeof(IDictionary&lt;string, object&gt;));	// true
		///		TypeExtension.IsAssignableFrom(typeof(IDictionary&lt;,&gt;), typeof(Dictionary&lt;string, object&gt;));	// true
		///		TypeExtension.IsAssignableFrom(typeof(Dictionary&lt;,&gt;), typeof(Dictioanry&lt;string, int&gt;));		//true
		///		
		///		public class MyNamedCollection&lt;T&gt; : Collection&lt;T&gt;, IDictionary&lt;string, T&gt;
		///		{
		///		}
		///		
		///		TypeExtension.IsAssignableFrom(typeof(IDictionary&lt;,&gt;), typeof(MyNamedCollection&lt;string, object&gt;)); //true
		///		</code>
		///		</example>
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
				{
					if(instanceType.IsInterface)
					{
						baseTypes = new List<Type>(new Type[] { instanceType });
						((List<Type>)baseTypes).AddRange(instanceType.GetInterfaces());
					}
					else
					{
						baseTypes = instanceType.GetInterfaces();
					}
				}
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

		public static bool IsScalarType(this Type type)
		{
			if(type == null)
				throw new ArgumentNullException("type");

			if(type.IsArray)
				return IsScalarType(type.GetElementType());

			if(type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
				return IsScalarType(type.GetGenericArguments()[0]);

			return type.IsPrimitive || type.IsEnum ||
			       type == typeof(string) || type == typeof(decimal) ||
			       type == typeof(DateTime) || type == typeof(TimeSpan) ||
			       type == typeof(DateTimeOffset) || type == typeof(Guid);
		}

		public static Type GetType(string typeName, bool throwOnError = false, bool ignoreCase = true)
		{
			if(string.IsNullOrWhiteSpace(typeName))
				return null;

			typeName = typeName.Replace(" ", "");

			switch(typeName.ToLowerInvariant())
			{
				case "string":
					return typeof(string);
				case "string[]":
					return typeof(string[]);

				case "int":
					return typeof(int);
				case "int?":
					return typeof(int?);
				case "int[]":
					return typeof(int[]);

				case "long":
					return typeof(long);
				case "long?":
					return typeof(long?);
				case "long[]":
					return typeof(long[]);

				case "short":
					return typeof(short);
				case "short?":
					return typeof(short?);
				case "short[]":
					return typeof(short[]);

				case "byte":
					return typeof(byte);
				case "byte?":
					return typeof(byte?);
				case "binary":
				case "byte[]":
					return typeof(byte[]);

				case "bool":
				case "boolean":
					return typeof(bool);
				case "bool?":
				case "boolean?":
					return typeof(bool?);
				case "bool[]":
				case "boolean[]":
					return typeof(bool[]);

				case "money":
				case "currency":
				case "decimal":
					return typeof(decimal);
				case "money?":
				case "currency?":
				case "decimal?":
					return typeof(decimal?);
				case "money[]":
				case "currency[]":
				case "decimal[]":
					return typeof(decimal[]);

				case "float":
				case "single":
					return typeof(float);
				case "float?":
				case "single?":
					return typeof(float?);
				case "float[]":
				case "single[]":
					return typeof(float[]);

				case "double":
				case "number":
					return typeof(double);
				case "double?":
				case "number?":
					return typeof(double?);
				case "double[]":
				case "number[]":
					return typeof(double[]);

				case "uint":
					return typeof(uint);
				case "uint?":
					return typeof(uint?);
				case "uint[]":
					return typeof(uint[]);

				case "ulong":
					return typeof(ulong);
				case "ulong?":
					return typeof(ulong?);
				case "ulong[]":
					return typeof(ulong[]);

				case "ushort":
					return typeof(ushort);
				case "ushort?":
					return typeof(ushort?);
				case "ushort[]":
					return typeof(ushort[]);

				case "sbyte":
					return typeof(sbyte);
				case "sbyte?":
					return typeof(sbyte?);
				case "sbyte[]":
					return typeof(sbyte[]);

				case "char":
					return typeof(char);
				case "char?":
					return typeof(char?);
				case "char[]":
					return typeof(char[]);

				case "date":
				case "time":
				case "datetime":
					return typeof(DateTime);
				case "date?":
				case "time?":
				case "datetime?":
					return typeof(DateTime?);
				case "date[]":
				case "time[]":
				case "datetime[]":
					return typeof(DateTime[]);

				case "timespan":
					return typeof(TimeSpan);
				case "timespan?":
					return typeof(TimeSpan?);
				case "timespan[]":
					return typeof(TimeSpan[]);

				case "guid":
					return typeof(Guid);
				case "guid?":
					return typeof(Guid?);
				case "guid[]":
					return typeof(Guid[]);

				case "object":
					return typeof(object);
				case "void":
					return typeof(void);
			}

			if(!typeName.Contains("."))
				typeName = "System." + typeName;

			return Type.GetType(typeName, throwOnError, ignoreCase);
		}
	}
}

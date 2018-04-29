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
using System.Linq;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;

namespace Zongsoft.Common
{
	public static class TypeExtension
	{
		public static bool IsAssignableFrom(this Type type, Type instanceType)
		{
			return IsAssignableFrom(type, instanceType, null);
		}

		public static bool IsAssignableFrom(this Type type, Type instanceType, out IEnumerable<Type> genericTypes)
		{
			var list = new List<Type>();

			if(IsAssignableFrom(type, instanceType, t =>
			{
				list.Add(t);
				return false;
			}))
			{
				genericTypes = list;
				return true;
			}
			else
			{
				genericTypes = list;
				return false;
			}
		}

		/// <summary>
		/// 提供比<see cref="System.Type.IsAssignableFrom"/>加强的功能，支持对泛型定义接口或类的匹配。
		/// </summary>
		/// <param name="type">指定的接口或基类的类型。</param>
		/// <param name="instanceType">指定的实例类型。</param>
		/// <param name="genericMatch">当<paramref name="type"/>参数为泛型原型，则该委托表示找到的实现者的泛化类型，返回空表示继续后续匹配，否则结束匹配并将该委托的返回作为方法的结果。</param>
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
		public static bool IsAssignableFrom(this Type type, Type instanceType, Func<Type, bool?> genericMatch)
		{
			if(type == null)
				throw new ArgumentNullException(nameof(type));

			if(instanceType == null)
				throw new ArgumentNullException(nameof(instanceType));

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
						//如果没有指定泛型匹配回调则返回成功
						if(genericMatch == null)
							return true;

						//调用匹配委托
						var match = genericMatch(baseType);

						//如果匹配回调有确定的结果，则返回该结果，否则忽略该次回调并等待下一轮匹配
						if(match.HasValue)
							return match.Value;
					}
				}

				return false;
			}

			return type.IsAssignableFrom(instanceType);
		}

		public static string GetExplicitImplementationName(this Type interfaceType, string memberName)
		{
			if(interfaceType == null)
				throw new ArgumentNullException(nameof(interfaceType));

			if(string.IsNullOrEmpty(memberName))
				throw new ArgumentNullException(nameof(memberName));

			if(interfaceType.IsGenericTypeDefinition)
				return null;

			if(!interfaceType.IsInterface)
				return memberName;

			if(interfaceType.IsGenericType)
			{
				var text = new System.Text.StringBuilder(interfaceType.Namespace + "." + interfaceType.Name.Substring(0, interfaceType.Name.Length - 2) + "<", 128);
				var argumentTypes = interfaceType.GetGenericArguments();

				for(int i = 0; i < argumentTypes.Length; i++)
				{
					text.Append(argumentTypes[i].FullName);

					if(i < argumentTypes.Length - 1)
						text.Append(",");
				}

				text.Append(">." + memberName);
				return text.ToString();
			}

			return interfaceType.Namespace + "." + interfaceType.Name + "." + memberName;
		}

		public static bool IsEnumerable(this Type type)
		{
			return typeof(IEnumerable).IsAssignableFrom(type) || IsAssignableFrom(typeof(IEnumerable<>), type);
		}

		public static bool IsCollection(this Type type)
		{
			return typeof(ICollection).IsAssignableFrom(type) || IsAssignableFrom(typeof(ICollection<>), type);
		}

		public static bool IsList(this Type type)
		{
			return typeof(IList).IsAssignableFrom(type) || IsAssignableFrom(typeof(IList<>), type);
		}

		public static bool IsDictionary(this Type type)
		{
			return typeof(IDictionary).IsAssignableFrom(type) || IsAssignableFrom(typeof(IDictionary<,>), type);
		}

		public static bool IsScalarType(this Type type)
		{
			if(type == null)
				throw new ArgumentNullException(nameof(type));

			if(type.IsArray)
				return IsScalarType(type.GetElementType());

			if(type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
				return IsScalarType(type.GetGenericArguments()[0]);

			var result = type.IsPrimitive || type.IsEnum ||
			             type == typeof(string) || type == typeof(decimal) ||
			             type == typeof(DateTime) || type == typeof(TimeSpan) ||
			             type == typeof(DateTimeOffset) || type == typeof(Guid) || type == typeof(DBNull);

			if(result)
				return result;

			var converter = TypeDescriptor.GetConverter(type);
			return (converter != null && converter.CanConvertFrom(typeof(string)) && converter.CanConvertTo(typeof(string)));
		}

		public static bool IsInteger(this Type type)
		{
			if(type == null)
				throw new ArgumentNullException("type");

			var code = Type.GetTypeCode(type);

			return code == TypeCode.Byte || code == TypeCode.SByte ||
			       code == TypeCode.Int16 || code == TypeCode.UInt16 ||
			       code == TypeCode.Int32 || code == TypeCode.UInt32 ||
			       code == TypeCode.Int64 || code == TypeCode.UInt64;
		}

		public static bool IsNumeric(this Type type)
		{
			if(type == null)
				throw new ArgumentNullException("type");

			var code = Type.GetTypeCode(type);

			return TypeExtension.IsInteger(type) ||
				   code == TypeCode.Single || code == TypeCode.Double ||
				   code == TypeCode.Decimal || code == TypeCode.Char;
		}

		public static Type GetListElementType(this Type type)
		{
			return GetElementType(type, typeof(IList<>), typeof(IList));
		}

		public static Type GetCollectionElementType(this Type type)
		{
			return GetElementType(type, typeof(ICollection<>), typeof(ICollection));
		}

		public static Type GetElementType(this Type type)
		{
			return GetElementType(type, typeof(IEnumerable<>), typeof(IEnumerable));
		}

		private static Type GetElementType(Type type, Type genericDefinitionInterface, Type collectionInterface)
		{
			if(type == null)
				throw new ArgumentNullException(nameof(type));

			if(type.IsArray)
				return type.GetElementType();

			if(type.IsInterface && type.IsGenericType && type.GetGenericTypeDefinition() == genericDefinitionInterface)
				return type.GetGenericArguments()[0];

			var collectionType = type.GetInterfaces().FirstOrDefault(t => t.IsGenericType && t.GetGenericTypeDefinition() == genericDefinitionInterface);

			if(collectionType != null)
				return collectionType.GetGenericArguments()[0];

			//如果指定的类型是一个非泛型集合接口的实现则返回其元素类型为object类型
			if(collectionInterface.IsAssignableFrom(type))
				return typeof(object);

			return null;
		}

		public static object GetDefaultValue(this Type type)
		{
			if(type == typeof(DBNull))
				return DBNull.Value;

			if(type == null || type.IsClass || type.IsInterface || type == typeof(Nullable<>) || TypeExtension.IsAssignableFrom(typeof(Nullable<>), type))
				return null;

			if(type.IsEnum)
			{
				var attribute = (DefaultValueAttribute)Attribute.GetCustomAttribute(type, typeof(DefaultValueAttribute), true);

				if(attribute != null && attribute.Value != null)
				{
					if(attribute.Value is string)
						return Enum.Parse(type, attribute.Value.ToString(), true);
					else
						return Enum.ToObject(type, attribute.Value);
				}

				Array values = Enum.GetValues(type);

				if(values.Length > 0)
					return System.Convert.ChangeType(values.GetValue(0), type);
			}

			return System.Activator.CreateInstance(type);
		}

		public static string GetTypeAlias<T>()
		{
			return GetTypeAlias(typeof(T));
		}

		public static string GetTypeAlias(this Type type)
		{
			if(type == null)
				return null;

			var elementType = type.IsArray ? type.GetElementType() : type;
			var code = Type.GetTypeCode(elementType);
			var alias = string.Empty;

			if(code != TypeCode.Object)
			{
				alias = code.ToString().ToLowerInvariant();
			}
			else
			{
				if(type == typeof(object))
					alias = "object";
				else if(type == typeof(Guid))
					alias = "guid";
				else
					return type.AssemblyQualifiedName;
			}

			if(type.IsArray)
				alias += "[]";

			return alias;
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

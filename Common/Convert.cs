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
using System.Reflection;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;

namespace Zongsoft.Common
{
	public static class Convert
	{
		#region 对象转换
		public static T ConvertValue<T>(object value)
		{
			return (T)ConvertValue(value, typeof(T), () => default(T));
		}

		public static T ConvertValue<T>(object value, T defaultValue)
		{
			return (T)ConvertValue(value, typeof(T), () => defaultValue);
		}

		public static T ConvertValue<T>(object value, Func<object> getDefaultValue)
		{
			return (T)ConvertValue(value, typeof(T), getDefaultValue);
		}

		public static object ConvertValue(object value, Type conversionType)
		{
			return ConvertValue(value, conversionType, () => GetDefaultValue(conversionType));
		}

		public static object ConvertValue(object value, Type conversionType, object defaultValue)
		{
			return ConvertValue(value, conversionType, () => defaultValue);
		}

		public static object ConvertValue(object value, Type conversionType, Func<object> getDefaultValue)
		{
			if(getDefaultValue == null)
				throw new ArgumentNullException("getDefaultValue");

			if(conversionType == null)
				return value;

			if(value == null || System.Convert.IsDBNull(value))
			{
				if(conversionType == typeof(DBNull))
					return DBNull.Value;
				else
					return getDefaultValue();
			}

			Type type = conversionType;

			if(conversionType.IsGenericType && (!conversionType.IsGenericTypeDefinition) && conversionType.GetGenericTypeDefinition() == typeof(Nullable<>))
				type = conversionType.GetGenericArguments()[0];

			if(type == value.GetType() || type.IsAssignableFrom(value.GetType()))
				return value;

			try
			{
				if(type == typeof(Encoding))
				{
					if(value == null)
						return getDefaultValue();

					if(value.GetType() == typeof(string))
					{
						switch(((string)value).ToLowerInvariant())
						{
							case "utf8":
							case "utf-8":
								return Encoding.UTF8;
							case "utf7":
							case "utf-7":
								return Encoding.UTF7;
							case "utf32":
								return Encoding.UTF32;
							case "unicode":
								return Encoding.Unicode;
							case "ascii":
								return Encoding.ASCII;
							case "bigend":
							case "bigendian":
								return Encoding.BigEndianUnicode;
							default:
								try
								{
									return Encoding.GetEncoding((string)value);
								}
								catch
								{
									return getDefaultValue();
								}
						}
					}
					else
					{
						switch(Type.GetTypeCode(value.GetType()))
						{
							case TypeCode.Byte:
							case TypeCode.Decimal:
							case TypeCode.Double:
							case TypeCode.Int16:
							case TypeCode.Int32:
							case TypeCode.Int64:
							case TypeCode.SByte:
							case TypeCode.Single:
							case TypeCode.UInt16:
							case TypeCode.UInt32:
							case TypeCode.UInt64:
								return Encoding.GetEncoding((int)System.Convert.ChangeType(value, typeof(int)));
						}
					}
				}

				//初始化特定类型转换器的映射
				InitializeTypeConverters();

				TypeConverter converter = TypeDescriptor.GetConverter(type);

				if(converter != null && converter.CanConvertFrom(value.GetType()))
					return converter.ConvertFrom(value);

				return System.Convert.ChangeType(value, type);
			}
			catch
			{
				return getDefaultValue();
			}
		}

		public static bool TryConvertValue<T>(object value, out T result)
		{
			bool b = true;

			result = (T)ConvertValue(value, typeof(T), () =>
			{
				b = false;
				return default(T);
			});

			return b;
		}

		public static bool TryConvertValue(object value, Type conversionType, out object result)
		{
			result = ConvertValue(value, conversionType, () => typeof(Convert));

			if(result == typeof(Convert))
			{
				result = null;
				return false;
			}

			return true;
		}
		#endregion

		#region 转换映射
		private static int _initialized;
		private static void InitializeTypeConverters()
		{
			var initialized = System.Threading.Interlocked.CompareExchange(ref _initialized, 1, 0);

			if(initialized == 0)
			{
				TypeDescriptor.AddAttributes(typeof(System.Enum), new Attribute[] { new TypeConverterAttribute(typeof(Zongsoft.ComponentModel.EnumConverter)) });
				TypeDescriptor.AddAttributes(typeof(System.Guid), new Attribute[] { new TypeConverterAttribute(typeof(Zongsoft.ComponentModel.GuidConverter)) });
				TypeDescriptor.AddAttributes(typeof(System.Net.IPEndPoint), new Attribute[] { new TypeConverterAttribute(typeof(Zongsoft.Communication.IPEndPointConverter)) });
			}
		}
		#endregion

		#region 取默认值
		public static object GetDefaultValue(Type type)
		{
			if(type == typeof(DBNull))
				return DBNull.Value;

			if(type == null || type.IsClass || type.IsInterface)
				return null;

			if(type.IsEnum)
			{
				var attributes = type.GetCustomAttributes(typeof(DefaultValueAttribute), true);

				if(attributes.Length > 0)
				{
					return ((DefaultValueAttribute)attributes[0]).Value;
				}
				else
				{
					Array values = Enum.GetValues(type);

					if(values.Length > 0)
						return values.GetValue(0);
				}
			}

			return Activator.CreateInstance(type);
		}
		#endregion

		#region 空值判断
		/// <summary>
		/// 判断指定的值是否为空或者DBNull，如果是则返回真(True)，否则返回假(False)。
		/// </summary>
		/// <param name="value">要判断的值。</param>
		/// <returns>返回的结果。</returns>
		public static bool IsNullOrDBNull(object value)
		{
			return (value == null || System.Convert.IsDBNull(value));
		}

		/// <summary>
		/// 判断指定的值是否为空或者DBNull，如果是则返回指定泛型类型的默认值，否则返回参数本身。
		/// </summary>
		/// <typeparam name="T">指定的参数的泛型。</typeparam>
		/// <param name="value">指定的参数值。</param>
		/// <returns>返回的结果。</returns>
		public static T IsNullOrDBNull<T>(object value)
		{
			return IsNullOrDBNull<T>(value, default(T));
		}

		/// <summary>
		/// 判断指定的值是否为空或者DBNull，如果是则返回指定的默认值，否则返回参数本身。
		/// </summary>
		/// <typeparam name="T">指定的参数的泛型。</typeparam>
		/// <param name="value">指定的参数值。</param>
		/// <param name="defaultValue">默认值。</param>
		/// <returns>参数值或默认值。</returns>
		public static T IsNullOrDBNull<T>(object value, T defaultValue)
		{
			if(value == null || System.Convert.IsDBNull(value))
				return defaultValue;

			try
			{
				return (T)value;
			}
			catch
			{
			}

			try
			{
				return (T)System.Convert.ChangeType(value, typeof(T));
			}
			catch
			{
			}

			return defaultValue;
		}
		#endregion

		#region 字节文本
		/// <summary>
		/// 将指定的字节数组转换为其用十六进制数字编码的等效字符串表示形式。
		/// </summary>
		/// <param name="buffer">一个 8 位无符号字节数组。</param>
		/// <returns>参数中元素的字符串表示形式，以十六进制文本表示。</returns>
		public static string ToHexString(byte[] buffer)
		{
			return ToHexString(buffer, '\0');
		}

		/// <summary>
		/// 将指定的字节数组转换为其用十六进制数字编码的等效字符串表示形式。参数指定是否在返回值中插入分隔符。
		/// </summary>
		/// <param name="buffer">一个 8 位无符号字节数组。</param>
		/// <param name="separator">每字节对应的十六进制文本中间的分隔符。</param>
		/// <returns>参数中元素的字符串表示形式，以十六进制文本表示。</returns>
		public static string ToHexString(byte[] buffer, char separator)
		{
			if(buffer == null || buffer.Length < 1)
				return string.Empty;

			StringBuilder builder = new StringBuilder(buffer.Length * 2);

			for(int i = 0; i < buffer.Length; i++)
			{
				builder.AppendFormat("{0:X2}", buffer[i]);

				if(separator != '\0' && i < buffer.Length - 1)
					builder.Append(separator);
			}

			return builder.ToString();
		}

		/// <summary>
		/// 将指定的十六进制格式的字符串转换为等效的字节数组。
		/// </summary>
		/// <param name="text">要转换的十六进制格式的字符串。</param>
		/// <returns>与<paramref name="text"/>等效的字节数组。</returns>
		/// <exception cref="System.FormatException"><paramref name="text"/>参数中含有非空白字符。</exception>
		/// <remarks>该方法的实现始终忽略<paramref name="text"/>参数中的空白字符。</remarks>
		public static byte[] FromHexString(string text)
		{
			return FromHexString(text, '\0', true);
		}

		/// <summary>
		/// 将指定的十六进制格式的字符串转换为等效的字节数组。
		/// </summary>
		/// <param name="text">要转换的十六进制格式的字符串。</param>
		/// <param name="separator">要过滤掉的分隔符字符。</param>
		/// <returns>与<paramref name="text"/>等效的字节数组。</returns>
		/// <exception cref="System.FormatException"><paramref name="text"/>参数中含有非空白字符或非指定的分隔符。</exception>
		/// <remarks>该方法的实现始终忽略<paramref name="text"/>参数中的空白字符。</remarks>
		public static byte[] FromHexString(string text, char separator)
		{
			return FromHexString(text, separator, true);
		}

		/// <summary>
		/// 将指定的十六进制格式的字符串转换为等效的字节数组。
		/// </summary>
		/// <param name="text">要转换的十六进制格式的字符串。</param>
		/// <param name="separator">要过滤掉的分隔符字符。</param>
		/// <param name="throwExceptionOnFormat">指定当输入文本中含有非法字符时是否抛出<seealso cref="System.FormatException"/>异常。</param>
		/// <returns>与<paramref name="text"/>等效的字节数组。</returns>
		/// <exception cref="System.FormatException">当<param name="throwExceptionOnFormat"参数为真，并且<paramref name="text"/>参数中含有非空白字符或非指定的分隔符。</exception>
		/// <remarks>该方法的实现始终忽略<paramref name="text"/>参数中的空白字符。</remarks>
		public static byte[] FromHexString(string text, char separator, bool throwExceptionOnFormat)
		{
			if(string.IsNullOrEmpty(text))
				return new byte[0];

			int index = 0;
			char[] buffer = new char[2];
			List<byte> result = new List<byte>();

			foreach(char character in text)
			{
				if(char.IsWhiteSpace(character) || character == separator)
					continue;

				buffer[index++] = character;
				if(index == buffer.Length)
				{
					index = 0;
					byte value = 0;

					if(TryParseHex(buffer, out value))
						result.Add(value);
					else
					{
						if(throwExceptionOnFormat)
							throw new FormatException();
						else
							return new byte[0];
					}
				}
			}

			return result.ToArray();
		}

		public static bool TryParseHex(char[] characters, out byte value)
		{
			long number;

			if(TryParseHex(characters, out number))
			{
				if(number >= byte.MinValue && number <= byte.MaxValue)
				{
					value = (byte)number;
					return true;
				}
			}

			value = 0;
			return false;
		}

		public static bool TryParseHex(char[] characters, out short value)
		{
			long number;

			if(TryParseHex(characters, out number))
			{
				if(number >= short.MinValue && number <= short.MaxValue)
				{
					value = (short)number;
					return true;
				}
			}

			value = 0;
			return false;
		}

		public static bool TryParseHex(char[] characters, out int value)
		{
			long number;

			if(TryParseHex(characters, out number))
			{
				if(number >= int.MinValue && number <= int.MaxValue)
				{
					value = (int)number;
					return true;
				}
			}

			value = 0;
			return false;
		}

		public static bool TryParseHex(char[] characters, out long value)
		{
			value = 0;

			if(characters == null)
				return false;

			int count = 0;
			byte[] digits = new byte[characters.Length];

			foreach(char character in characters)
			{
				if(char.IsWhiteSpace(character))
					continue;

				if(character >= '0' && character <= '9')
					digits[count++] = (byte)(character - '0');
				else if(character >= 'A' && character <= 'F')
					digits[count++] = (byte)((character - 'A') + 10);
				else if(character >= 'a' && character <= 'f')
					digits[count++] = (byte)((character - 'a') + 10);
				else
					return false;
			}

			long number = 0;

			if(count > 0)
			{
				for(int i = 0; i < count; i++)
				{
					number += digits[i] * (long)Math.Pow(16, count - i - 1);
				}
			}

			value = number;
			return true;
		}
		#endregion

		#region 对象解析

		#region 默认解析
		private static readonly Action<ObjectResolvingContext> DefaultResolve = (ctx) =>
		{
			if(ctx.Container == null)
				return;

			if(ctx.Direction == ObjectResolvingDirection.Get)
			{
				var member = GetMember(ctx.Container.GetType(), ctx.Name, (BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetField | BindingFlags.GetProperty), true);

				if(member == null)
					throw new ArgumentException("Invalid path of target object.");

				switch(member.MemberType)
				{
					case MemberTypes.Field:
						ctx.Value = ((FieldInfo)member).GetValue(ctx.Container);
						break;
					case MemberTypes.Property:
						ctx.Value = ((PropertyInfo)member).GetValue(ctx.Container, null);
						break;
				}

				ctx.Handled = true;
			}
			else if(ctx.Direction == ObjectResolvingDirection.Set)
			{
				var value = ctx.Value;
				var member = GetMember(ctx.Container.GetType(), ctx.Name, (BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetField | BindingFlags.SetProperty), true);

				if(member == null)
					throw new ArgumentException("Invalid path of target object.");

				switch(member.MemberType)
				{
					case MemberTypes.Field:
						value = Zongsoft.Common.Convert.ConvertValue(ctx.Value, ((FieldInfo)member).FieldType);
						((FieldInfo)member).SetValue(ctx.Container, value);
						break;
					case MemberTypes.Property:
						value = Zongsoft.Common.Convert.ConvertValue(ctx.Value, ((PropertyInfo)member).PropertyType);
						((PropertyInfo)member).SetValue(ctx.Container, value, null);
						break;
				}

				ctx.Handled = true;
			}
		};
		#endregion

		#region 获取方法
		public static object GetValue(object target, string path)
		{
			if(target == null || path == null || path.Length < 1)
				return target;

			return GetValue(target, path.Split('.'), null);
		}

		public static object GetValue(object target, string path, Action<ObjectResolvingContext> resolve)
		{
			if(target == null || path == null || path.Length < 1)
				return target;

			return GetValue(target, path.Split('.'), resolve);
		}

		public static object GetValue(object target, string[] memberNames)
		{
			if(target == null || memberNames == null || memberNames.Length < 1)
				return target;

			return GetValue(target, memberNames, 0, memberNames.Length, null);
		}

		public static object GetValue(object target, string[] memberNames, Action<ObjectResolvingContext> resolve)
		{
			if(target == null || memberNames == null || memberNames.Length < 1)
				return target;

			return GetValue(target, memberNames, 0, memberNames.Length, resolve);
		}

		public static object GetValue(object target, string[] memberNames, int start, int length, Action<ObjectResolvingContext> resolve)
		{
			if(target == null || memberNames == null || memberNames.Length < 1)
				return target;

			if(start < 0 || start >= memberNames.Length)
				throw new ArgumentOutOfRangeException("start");

			//创建解析上下文对象
			ObjectResolvingContext context = new ObjectResolvingContext(target, string.Join(".", memberNames));

			for(int i = 0; i < Math.Min(memberNames.Length - start, length); i++)
			{
				string memberName = memberNames[start + i];

				if(memberName == null || memberName.Trim().Length < 1)
					continue;

				if(context.Value == null)
					continue;

				context.Handled = false;
				context.Name = memberName;
				context.Container = context.Value;

				//解析对象成员
				if(resolve == null)
					DefaultResolve(context);
				else
				{
					resolve(context);

					if(!context.Handled)
						DefaultResolve(context);
				}
			}

			return context.Value;
		}
		#endregion

		#region 设置方法
		public static void SetValue(object target, string path, object value)
		{
			SetValue(target, path, value, null);
		}

		public static void SetValue(object target, string path, object value, Action<ObjectResolvingContext> resolve)
		{
			if(target == null || path == null || path.Length < 1)
				return;

			SetValue(target, path.Split('.'), value, resolve);
		}

		public static void SetValue(object target, string[] memberNames, object value)
		{
			SetValue(target, memberNames, value, null);
		}

		public static void SetValue(object target, string[] memberNames, object value, Action<ObjectResolvingContext> resolve)
		{
			if(target == null || memberNames == null || memberNames.Length < 1)
				return;

			object container = target;

			if(memberNames.Length > 1)
			{
				container = GetValue(target, memberNames, 0, memberNames.Length - 1, resolve);

				if(container == null)
					throw new InvalidOperationException(string.Format("The '{0}' member is not exists in object of '{1}' type.", string.Join(".", memberNames), target.GetType().FullName));
			}

			//创建构件解析上下文对象
			var context = new ObjectResolvingContext(target, container, memberNames[memberNames.Length - 1], value, string.Join(".", memberNames));

			//调用解析回调方法
			if(resolve == null)
				DefaultResolve(context);
			else
			{
				resolve(context);

				if(!context.Handled)
					DefaultResolve(context);
			}
		}
		#endregion

		#region 私有方法
		private static MemberInfo GetMember(Type type, string name, BindingFlags binding, bool ignoreCase)
		{
			if(type == null || string.IsNullOrWhiteSpace(name))
				return null;

			var members = type.FindMembers((MemberTypes.Field | MemberTypes.Property),
								(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetField | BindingFlags.GetProperty),
								(member, criteria) =>
								{
									return string.Equals((string)criteria, member.Name,
														 (ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal));
								},
								name);

			if(members != null && members.Length > 0)
				return members[0];

			return null;
		}
		#endregion

		#region 嵌套子类
		/// <summary>
		/// 表示对象成员的解析方向。
		/// </summary>
		public enum ObjectResolvingDirection
		{
			/// <summary>获取对象的成员值。</summary>
			Get,
			/// <summary>设置对象的成员值。</summary>
			Set,
		}

		/// <summary>
		/// 表示在对象成员解析程序中的操作上下文。
		/// </summary>
		public class ObjectResolvingContext : MarshalByRefObject
		{
			#region 成员字段
			private ObjectResolvingDirection _direction;
			private object _target;
			private object _container;
			private string _path;
			private object _value;
			private string _name;
			private bool _handled;
			#endregion

			#region 构造函数
			internal ObjectResolvingContext(object target, string path)
			{
				if(target == null)
					throw new ArgumentNullException("target");

				if(string.IsNullOrWhiteSpace(path))
					throw new ArgumentNullException("path");

				_direction = ObjectResolvingDirection.Get;
				_target = target;
				_container = target;
				_value = target;
				_path = path;
			}

			internal ObjectResolvingContext(object target, object container, string name, object value, string path)
			{
				if(target == null)
					throw new ArgumentNullException("target");

				if(string.IsNullOrWhiteSpace(path))
					throw new ArgumentNullException("path");

				_direction = ObjectResolvingDirection.Set;
				_target = target;
				_container = container;
				_name = name;
				_value = value;
				_path = path;
			}
			#endregion

			#region 公共属性
			/// <summary>
			/// 获取解析过程中当前处理的方向。
			/// </summary>
			public ObjectResolvingDirection Direction
			{
				get
				{
					return _direction;
				}
			}

			/// <summary>
			/// 获取解析程序的目标根对象。
			/// </summary>
			public object Target
			{
				get
				{
					return _target;
				}
			}

			/// <summary>
			/// 获取解析过程中当前成员的容器对象。
			/// </summary>
			public object Container
			{
				get
				{
					return _container;
				}
				internal set
				{
					_container = value;
				}
			}

			/// <summary>
			/// 获取解析的完整成员路径，返回一个以“.”分隔的字符串。
			/// </summary>
			public string Path
			{
				get
				{
					return _path;
				}
			}

			/// <summary>
			/// 获取或设置一个操作的值，该属性在不同场景中所表示的含义和可设置性均不同。详情请参考备注。
			/// </summary>
			/// <remarks>
			///		<para>当<see cref="Direction"/>属性值等于<seealso cref="ObjectResolvingDirection.Get"/>时，该属性可设置，表示处理程序所解析出来的成员值。</para>
			///		<para>当<see cref="Direction"/>属性值等于<seealso cref="ObjectResolvingDirection.Set"/>时，该属性不可设置，表示是由用户指定要设置的目标值。</para>
			/// </remarks>
			/// <exception cref="System.InvalidOperationException">当<see cref="Direction"/>属性值不等于<seealso cref="ObjectResolvingDirection.Get"/>时激发。</exception>
			public object Value
			{
				get
				{
					return _value;
				}
				set
				{
					if(_direction != ObjectResolvingDirection.Get)
						throw new InvalidOperationException();

					_value = value;
				}
			}

			/// <summary>
			/// 获取当前解析的成员名称。
			/// </summary>
			public string Name
			{
				get
				{
					return _name;
				}
				internal set
				{
					_name = value;
				}
			}

			/// <summary>
			/// 获取或设置处理完成标记。
			/// </summary>
			/// <remarks>
			///		<para>如果设置该属性为真(true)，表示自定义解析程序已经完成对当前成员的解析，则表示告知系统不要再对当前成员的进行解析处理了；</para>
			///		<para>如果设置该属性为假(false)，即默认值。表示自定义自定义解析程序未对当前成员进行解析，则意味将由系统对当前成员进行解析处理。</para>
			/// </remarks>
			public bool Handled
			{
				get
				{
					return _handled;
				}
				set
				{
					_handled = value;
				}
			}
			#endregion
		}
		#endregion

		#endregion
	}
}

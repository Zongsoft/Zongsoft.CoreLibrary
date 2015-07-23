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
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.ComponentModel;
using System.Text;

namespace Zongsoft.Common
{
	public static class Convert
	{
		#region 类型转换
		public static T ConvertValue<T>(object value)
		{
			return (T)ConvertValue(value, typeof(T), () => default(T));
		}

		public static T ConvertValue<T>(object value, T defaultValue)
		{
			return (T)ConvertValue(value, typeof(T), () => defaultValue);
		}

		public static T ConvertValue<T>(object value, Func<object> defaultValueThunk)
		{
			return (T)ConvertValue(value, typeof(T), defaultValueThunk);
		}

		public static object ConvertValue(object value, Type conversionType)
		{
			return ConvertValue(value, conversionType, () => GetDefaultValue(conversionType));
		}

		public static object ConvertValue(object value, Type conversionType, object defaultValue)
		{
			return ConvertValue(value, conversionType, () => defaultValue);
		}

		public static object ConvertValue(object value, Type conversionType, Func<object> defaultValueThunk)
		{
			if(defaultValueThunk == null)
				throw new ArgumentNullException("defaultValueThunk");

			if(conversionType == null)
				return value;

			if(value == null || System.Convert.IsDBNull(value))
			{
				if(conversionType == typeof(DBNull))
					return DBNull.Value;
				else
					return defaultValueThunk();
			}

			Type type = conversionType;

			if(conversionType.IsGenericType && (!conversionType.IsGenericTypeDefinition) && conversionType.GetGenericTypeDefinition() == typeof(Nullable<>))
				type = conversionType.GetGenericArguments()[0];

			if(type == value.GetType() || type.IsAssignableFrom(value.GetType()))
				return value;

			try
			{
				//获取指定的类型转换器
				var converter = GetTypeConverter(type);

				if(converter != null && converter.CanConvertFrom(value.GetType()))
					return converter.ConvertFrom(value);

				return System.Convert.ChangeType(value, type);
			}
			catch
			{
				return defaultValueThunk();
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

		#region 获取转换器
		private static int _initialized;
		private static TypeConverter GetTypeConverter(Type type)
		{
			if(_initialized == 0)
			{
				var initialized = System.Threading.Interlocked.CompareExchange(ref _initialized, 1, 0);

				if(initialized == 0)
				{
					TypeDescriptor.AddAttributes(typeof(System.Enum), new Attribute[] { new TypeConverterAttribute(typeof(Zongsoft.ComponentModel.EnumConverter)) });
					TypeDescriptor.AddAttributes(typeof(System.Guid), new Attribute[] { new TypeConverterAttribute(typeof(Zongsoft.ComponentModel.GuidConverter)) });
					TypeDescriptor.AddAttributes(typeof(Encoding), new Attribute[] { new TypeConverterAttribute(typeof(Zongsoft.ComponentModel.EncodingConverter)) });
					TypeDescriptor.AddAttributes(typeof(System.Net.IPEndPoint), new Attribute[] { new TypeConverterAttribute(typeof(Zongsoft.Communication.IPEndPointConverter)) });
				}
			}

			return TypeDescriptor.GetConverter(type);
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
				//var attributes = type.GetCustomAttributes(typeof(DefaultValueAttribute), true);
				//if(attributes.Length > 0)
				//	return ((DefaultValueAttribute)attributes[0]).Value;

				var attribute = Attribute.GetCustomAttribute(type, typeof(DefaultValueAttribute), true);
				if(attribute != null)
					return ((DefaultValueAttribute)attribute).Value;

				Array values = Enum.GetValues(type);

				if(values.Length > 0)
					return values.GetValue(0);
			}

			return Activator.CreateInstance(type);
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
		/// <exception cref="System.FormatException">当<paramref name="throwExceptionOnFormat"/>参数为真，并且<paramref name="text"/>参数中含有非空白字符或非指定的分隔符。</exception>
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

		#region 对象组装
		[Obsolete("Please use Zongsoft.Runtime.Serialization.DictionarySerializer class.")]
		public static T Populate<T>(IEnumerable<KeyValuePair<string, object>> dictionary, Func<T> creator = null, Action<ObjectResolvingContext> resolve = null)
		{
			if(dictionary == null)
				return default(T);

			var result = creator != null ? creator() : Activator.CreateInstance<T>();

			if(resolve == null)
			{
				resolve = ctx =>
				{
					if(ctx.Direction == ObjectResolvingDirection.Get)
					{
						ctx.Value = ctx.GetMemberValue();

						if(ctx.Value == null)
						{
							ctx.Value = Activator.CreateInstance(ctx.MemberType);

							switch(ctx.Member.MemberType)
							{
								case MemberTypes.Field:
									((FieldInfo)ctx.Member).SetValue(ctx.Container, ctx.Value);
									break;
								case MemberTypes.Property:
									((PropertyInfo)ctx.Member).SetValue(ctx.Container, ctx.Value);
									break;
							}
						}
					}
				};
			}

			foreach(KeyValuePair<string, object> entry in dictionary)
			{
				if(string.IsNullOrWhiteSpace(entry.Key))
					continue;

				SetValue(result, entry.Key, entry.Value, resolve);
			}

			return result;
		}

		[Obsolete("Please use Zongsoft.Runtime.Serialization.DictionarySerializer class.")]
		public static T Populate<T>(IDictionary dictionary, Func<T> creator = null, Action<ObjectResolvingContext> resolve = null)
		{
			if(dictionary == null || dictionary.Count < 1)
				return default(T);

			return Populate<T>(Zongsoft.Collections.DictionaryExtension.ToDictionary<string, object>(dictionary), creator, resolve);
		}

		[Obsolete("Please use Zongsoft.Runtime.Serialization.DictionarySerializer class.")]
		public static object Populate(IEnumerable<KeyValuePair<string, object>> dictionary, Type type, Func<object> creator = null, Action<ObjectResolvingContext> resolve = null)
		{
			if(dictionary == null)
				return null;

			var result = creator != null ? creator() : Activator.CreateInstance(type);

			if(resolve == null)
			{
				resolve = ctx =>
				{
					if(ctx.Direction == ObjectResolvingDirection.Get)
					{
						ctx.Value = ctx.GetMemberValue();

						if(ctx.Value == null)
						{
							ctx.Value = Activator.CreateInstance(ctx.MemberType);

							switch(ctx.Member.MemberType)
							{
								case MemberTypes.Field:
									((FieldInfo)ctx.Member).SetValue(ctx.Container, ctx.Value);
									break;
								case MemberTypes.Property:
									((PropertyInfo)ctx.Member).SetValue(ctx.Container, ctx.Value);
									break;
							}
						}
					}
				};
			}

			foreach(KeyValuePair<string, object> entry in dictionary)
			{
				if(string.IsNullOrWhiteSpace(entry.Key))
					continue;

				SetValue(result, entry.Key, entry.Value, resolve);
			}

			return result;
		}

		[Obsolete("Please use Zongsoft.Runtime.Serialization.DictionarySerializer class.")]
		public static object Populate(IDictionary dictionary, Type type, Func<object> creator = null, Action<ObjectResolvingContext> resolve = null)
		{
			if(dictionary == null || dictionary.Count < 1)
				return null;

			return Populate(Zongsoft.Collections.DictionaryExtension.ToDictionary<string, object>(dictionary), creator, resolve);
		}
		#endregion

		#region 获取方法
		public static Type GetMemberType(object target, string text)
		{
			if(target == null)
				return null;

			if(string.IsNullOrWhiteSpace(text))
				return target.GetType();

			var type = target.GetType();
			var parts = text.Split(new char[]{ '.' }, StringSplitOptions.RemoveEmptyEntries);

			foreach(var part in parts)
			{
				var member = GetMember(type, part, (BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static), true);

				if(member == null)
					throw new ArgumentException(string.Format("The '{0}' member is not existed in the '{1}' type, the original text is '{2}'.", part, type.FullName, text));

				switch(member.MemberType)
				{
					case MemberTypes.Field:
						type = ((FieldInfo)member).FieldType;
						break;
					case MemberTypes.Property:
						type = ((PropertyInfo)member).PropertyType;
						break;
				}
			}

			return type;
		}

		public static bool TryGetMemberType(object target, string text, out Type memberType)
		{
			memberType = null;

			if(target == null)
				return false;

			memberType = target.GetType();

			if(string.IsNullOrWhiteSpace(text))
				return true;

			var parts = text.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);

			foreach(var part in parts)
			{
				var member = GetMember(memberType, part, (BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static), true);

				if(member == null)
					return false;

				switch(member.MemberType)
				{
					case MemberTypes.Field:
						memberType = ((FieldInfo)member).FieldType;
						break;
					case MemberTypes.Property:
						memberType = ((PropertyInfo)member).PropertyType;
						break;
				}
			}

			return true;
		}

		public static object GetValue(object target, string text)
		{
			if(target == null || text == null || text.Length < 1)
				return target;

			return GetValue(target, text.Split('.'), null);
		}

		public static object GetValue(object target, string text, Action<ObjectResolvingContext> resolve)
		{
			if(target == null || text == null || text.Length < 1)
				return target;

			return GetValue(target, text.Split('.'), resolve);
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

				context.Handled = true;
				context.MemberName = memberName;
				context.Container = context.Value;
				context.Value = null;

				//解析对象成员
				if(resolve == null)
					DefaultResolve(context);
				else
				{
					resolve(context);

					if(!context.Handled)
						DefaultResolve(context);
				}

				//如果解析被终止则退出后续解析
				if(context.IsTerminated)
					break;
			}

			return context.Value;
		}
		#endregion

		#region 设置方法
		public static void SetValue(object target, string text, object value)
		{
			SetValue(target, text, value, null);
		}

		public static void SetValue(object target, string text, object value, Action<ObjectResolvingContext> resolve)
		{
			if(target == null || text == null || text.Length < 1)
				return;

			SetValue(target, text.Split('.'), value, resolve);
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
					throw new InvalidOperationException(string.Format("The '{0}' member value is null of '{1}' type.", string.Join(".", memberNames, 0, memberNames.Length - 1), target.GetType().FullName));
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

			var member = context.Member;

			if(member == null)
				throw new ArgumentException(string.Format("The '{0}' member is not exists in the '{1}' type.", context.MemberName, context.Container.GetType().FullName));

			switch(member.MemberType)
			{
				case MemberTypes.Field:
					((FieldInfo)member).SetValue(context.Container, Zongsoft.Common.Convert.ConvertValue(context.Value, ((FieldInfo)member).FieldType));
					break;
				case MemberTypes.Property:
					((PropertyInfo)member).SetValue(context.Container, Zongsoft.Common.Convert.ConvertValue(context.Value, ((PropertyInfo)member).PropertyType), null);
					break;
			}
		}
		#endregion

		#region 私有方法
		private static readonly Action<ObjectResolvingContext> DefaultResolve = (ctx) =>
		{
			if(ctx.Direction == ObjectResolvingDirection.Get)
				ctx.Value = ctx.GetMemberValue();
		};

		private static MemberInfo GetMember(Type type, string name, BindingFlags? binding = null, bool ignoreCase = true)
		{
			if(type == null || string.IsNullOrWhiteSpace(name))
				return null;

			if(!binding.HasValue)
				binding = (BindingFlags.Public | BindingFlags.Instance);

			var members = type.FindMembers((MemberTypes.Field | MemberTypes.Property),
								binding.Value,
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
			private MemberInfo _member;
			private string _text;
			private string _memberName;
			private object _value;
			private bool _handled;
			private bool _isTerminated;
			#endregion

			#region 构造函数
			internal ObjectResolvingContext(object target, string text)
			{
				if(target == null)
					throw new ArgumentNullException("target");

				if(string.IsNullOrWhiteSpace(text))
					throw new ArgumentNullException("text");

				_direction = ObjectResolvingDirection.Get;
				_target = target;
				_container = target;
				_value = target;
				_text = text;
				_handled = true;
			}

			internal ObjectResolvingContext(object target, object container, string memberName, object value, string text)
			{
				if(target == null)
					throw new ArgumentNullException("target");

				if(string.IsNullOrWhiteSpace(text))
					throw new ArgumentNullException("text");

				_direction = ObjectResolvingDirection.Set;
				_target = target;
				_container = container;
				_memberName = memberName;
				_value = value;
				_text = text;
				_handled = true;
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

					//必须重置当前的解析成员
					_member = null;
				}
			}

			/// <summary>
			/// 获取解析的文本参数值。
			/// </summary>
			public string Text
			{
				get
				{
					return _text;
				}
			}

			/// <summary>
			/// 获取或设置一个操作的值，该属性在不同场景中所表示的含义和可设置性均不同。详情请参考备注。
			/// </summary>
			/// <remarks>
			///		<para>当<see cref="Direction"/>属性值等于<seealso cref="ObjectResolvingDirection.Get"/>时，表示处理程序所解析出来的成员值。</para>
			///		<para>当<see cref="Direction"/>属性值等于<seealso cref="ObjectResolvingDirection.Set"/>时，表示是由用户指定要设置的目标值。</para>
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
					_value = value;
				}
			}

			/// <summary>
			/// 获取当前解析的成员名称。
			/// </summary>
			public string MemberName
			{
				get
				{
					return _memberName;
				}
				internal set
				{
					_memberName = value;

					//必须重置当前的解析成员
					_member = null;
				}
			}

			/// <summary>
			/// 获取当前解析的成员信息。
			/// </summary>
			public MemberInfo Member
			{
				get
				{
					if(_member == null)
					{
						if(_container == null || string.IsNullOrWhiteSpace(_memberName))
							return null;

						_member = GetMember(_container.GetType(), _memberName);
					}

					return _member;
				}
			}

			/// <summary>
			/// 获取当前解析成员的类型。
			/// </summary>
			public Type MemberType
			{
				get
				{
					var member = this.Member;

					if(member == null)
						return null;

					switch(member.MemberType)
					{
						case MemberTypes.Field:
							return ((FieldInfo)member).FieldType;
						case MemberTypes.Property:
							return ((PropertyInfo)member).PropertyType;
						case MemberTypes.Method:
							return ((MethodInfo)member).ReturnType;
					}

					return null;
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

			/// <summary>
			/// 获取或设置是否终止标记。
			/// </summary>
			public bool IsTerminated
			{
				get
				{
					return _isTerminated;
				}
				set
				{
					_isTerminated = value;
				}
			}
			#endregion

			#region 公共方法
			public object GetMemberValue()
			{
				return this.GetMemberValue(this.Container);
			}

			public object GetMemberValue(object container)
			{
				var member = this.Member;

				if(member == null)
				{
					if(container == null)
						throw new ArgumentException(string.Format("The '{0}' member is not exists for '{1}' text.", this.MemberName, this.Text));
					else
						throw new ArgumentException(string.Format("The '{0}' member is not exists in the '{1}' type.", this.MemberName, container.GetType().FullName));
				}

				switch(member.MemberType)
				{
					case MemberTypes.Field:
						return ((FieldInfo)member).GetValue(container);
					case MemberTypes.Property:
						return ((PropertyInfo)member).GetValue(container, null);
					case MemberTypes.Method:
						return ((MethodInfo)member).Invoke(container, null);
				}

				throw new InvalidOperationException(string.Format("Invalid '{0}' member for '{1}' text.", this.MemberName, this.Text));
			}
			#endregion
		}
		#endregion

		#endregion
	}
}

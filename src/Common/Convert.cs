/*
 *   _____                                ______
 *  /_   /  ____  ____  ____  _________  / __/ /_
 *    / /  / __ \/ __ \/ __ \/ ___/ __ \/ /_/ __/
 *   / /__/ /_/ / / / / /_/ /\_ \/ /_/ / __/ /_
 *  /____/\____/_/ /_/\__  /____/\____/_/  \__/
 *                   /____/
 *
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@qq.com>
 *
 * Copyright (C) 2010-2019 Zongsoft Corporation <http://www.zongsoft.com>
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
using System.Text;
using System.Collections.Generic;
using System.ComponentModel;

namespace Zongsoft.Common
{
	public static class Convert
	{
		#region 初始化器
		static Convert()
		{
			TypeDescriptor.AddAttributes(typeof(Enum), new Attribute[] { new TypeConverterAttribute(typeof(Zongsoft.ComponentModel.EnumConverter)) });
			TypeDescriptor.AddAttributes(typeof(Guid), new Attribute[] { new TypeConverterAttribute(typeof(Zongsoft.ComponentModel.GuidConverter)) });
			TypeDescriptor.AddAttributes(typeof(Encoding), new Attribute[] { new TypeConverterAttribute(typeof(Zongsoft.ComponentModel.EncodingConverter)) });
			TypeDescriptor.AddAttributes(typeof(System.Net.IPEndPoint), new Attribute[] { new TypeConverterAttribute(typeof(Zongsoft.Communication.IPEndPointConverter)) });
		}
		#endregion

		#region 类型转换
		public static T ConvertValue<T>(object value)
		{
			return (T)ConvertValue(value, typeof(T));
		}

		public static T ConvertValue<T>(object value, T defaultValue)
		{
			return (T)ConvertValue(value, typeof(T), defaultValue);
		}

		public static T ConvertValue<T>(object value, Func<T> defaultValueThunk)
		{
			if(TryConvertValue(value, typeof(T), out var result))
				return (T)result;

			if(defaultValueThunk != null)
				return defaultValueThunk();

			throw new InvalidOperationException($"Unable to convert {value} to {typeof(T)} type.");
		}

		public static T ConvertValue<T>(object value, Func<TypeConverter> converterFactory)
		{
			return (T)ConvertValue(value, typeof(T), converterFactory);
		}

		public static T ConvertValue<T>(object value, Func<TypeConverter> converterFactory, T defaultValue)
		{
			return (T)ConvertValue(value, typeof(T), converterFactory, defaultValue);
		}

		public static T ConvertValue<T>(object value, Func<TypeConverter> converterFactory, Func<T> defaultValueThunk)
		{
			if(TryConvertValue(value, typeof(T), converterFactory, out var result))
				return (T)result;

			if(defaultValueThunk != null)
				return defaultValueThunk();

			throw new InvalidOperationException($"Unable to convert {value} to {typeof(T)} type.");
		}

		public static object ConvertValue(object value, Type conversionType)
		{
			if(TryConvertValue(value, conversionType, out var result))
				return result;

			throw new InvalidOperationException($"Unable to convert {value} to {conversionType} type.");
		}

		public static object ConvertValue(object value, Type conversionType, object defaultValue)
		{
			return TryConvertValue(value, conversionType, out var result) ? result : defaultValue;
		}

		public static object ConvertValue(object value, Type conversionType, Func<object> defaultValueThunk)
		{
			if(TryConvertValue(value, conversionType, out var result))
				return result;

			if(defaultValueThunk != null)
				return defaultValueThunk();

			throw new InvalidOperationException($"Unable to convert {value} to {conversionType} type.");
		}

		public static object ConvertValue(object value, Type conversionType, Func<TypeConverter> converterFactory)
		{
			if(TryConvertValue(value, conversionType, converterFactory, out var result))
				return result;

			throw new InvalidOperationException($"Unable to convert {value} to {conversionType} type.");
		}

		public static object ConvertValue(object value, Type conversionType, Func<TypeConverter> converterFactory, object defaultValue)
		{
			return TryConvertValue(value, conversionType, converterFactory, out var result) ? result : defaultValue;
		}

		public static object ConvertValue(object value, Type conversionType, Func<TypeConverter> converterFactory, Func<object> defaultValueThunk)
		{
			if(TryConvertValue(value, conversionType, converterFactory, out var result))
				return result;

			if(defaultValueThunk != null)
				return defaultValueThunk();

			throw new InvalidOperationException($"Unable to convert {value} to {conversionType} type.");
		}

		public static bool TryConvertValue<T>(object value, out T result)
		{
			return TryConvertValue<T>(value, null, out result);
		}

		public static bool TryConvertValue(object value, Type conversionType, out object result)
		{
			return TryConvertValue(value, conversionType, null, out result);
		}

		public static bool TryConvertValue<T>(object value, Func<TypeConverter> converterFactory, out T result)
		{
			if(TryConvertValue(value, typeof(T), converterFactory, out var obj))
			{
				result = (T)obj;
				return true;
			}

			result = default(T);
			return false;
		}

		public static bool TryConvertValue(object value, Type conversionType, Func<TypeConverter> converterFactory, out object result)
		{
			result = null;

			//如果转换类型为空，则不进行类型转换处理
			if(conversionType == null)
			{
				result = value;
				return true;
			}

			//处理待转换值为空的情况
			if(value == null || System.Convert.IsDBNull(value))
			{
				if(conversionType == typeof(DBNull))
					result = DBNull.Value;
				else if(conversionType.IsGenericType && conversionType.GetGenericTypeDefinition() == typeof(Nullable<>))
					result = null;
				else
					result = conversionType.IsValueType ? Activator.CreateInstance(conversionType) : null;

				return true;
			}

			Type type = conversionType;

			if(conversionType.IsGenericType && (!conversionType.IsGenericTypeDefinition) && conversionType.GetGenericTypeDefinition() == typeof(Nullable<>))
				type = conversionType.GetGenericArguments()[0];

			//处理类型兼容的情况
			if(type == value.GetType() || type.IsAssignableFrom(value.GetType()))
			{
				result = value;
				return true;
			}

			try
			{
				//获取目标类型的转换器
				var converter = converterFactory?.Invoke() ?? TypeDescriptor.GetConverter(type);

				if(converter != null)
				{
					if(converter.CanConvertFrom(value.GetType())) //尝试从源类型进行转换
					{
						result = converter.ConvertFrom(value);
						return true;
					}

					if(converter.CanConvertTo(type)) //尝试从目标类型进行转换
					{
						result = converter.ConvertTo(value, type);
						return true;
					}
				}

				if(value is string)
				{
					var method = type.GetMethod("TryParse", new Type[] { typeof(string), type });

					if(method != null && method.IsStatic)
					{
						var args = new object[] { value, null };
						var succeed = method.Invoke(null, args);

						if(succeed.GetType() == typeof(bool))
						{
							if((bool)succeed)
								result = args[1];

							return (bool)succeed;
						}
					}
					else
					{
						method = type.GetMethod("Parse", new Type[] { typeof(string) });

						if(method != null && method.IsStatic)
						{
							result = method.Invoke(null, new object[] { value });
							return true;
						}
					}
				}

				result = System.Convert.ChangeType(value, type);
				return true;
			}
			catch
			{
				return false;
			}
		}
		#endregion

		#region 字节文本
		/// <summary>
		/// 将指定的字节数组转换为其用十六进制数字编码的等效字符串表示形式。
		/// </summary>
		/// <param name="bytes">一个 8 位无符号字节数组。</param>
		/// <param name="lowerCase">返回的十六进制字符串中是否使用小写字符，默认为大写。</param>
		/// <returns>参数中元素的字符串表示形式，以十六进制文本表示。</returns>
		public static string ToHexString(byte[] bytes, bool lowerCase = false)
		{
			return ToHexString(bytes, 0, 0, '\0', lowerCase);
		}

		/// <summary>
		/// 将指定的字节数组转换为其用十六进制数字编码的等效字符串表示形式。
		/// </summary>
		/// <param name="bytes">一个 8 位无符号字节数组。</param>
		/// <param name="offset">指定字节数组的起始下标。</param>
		/// <param name="count">指定字节数组的元素个数。</param>
		/// <param name="lowerCase">返回的十六进制字符串中是否使用小写字符，默认为大写。</param>
		/// <returns>参数中元素的字符串表示形式，以十六进制文本表示。</returns>
		public static string ToHexString(byte[] bytes, int offset, int count, bool lowerCase = false)
		{
			return ToHexString(bytes, offset, count, '\0', lowerCase);
		}

		/// <summary>
		/// 将指定的字节数组转换为其用十六进制数字编码的等效字符串表示形式。
		/// </summary>
		/// <param name="bytes">一个 8 位无符号字节数组。</param>
		/// <param name="separator">每字节对应的十六进制文本中间的分隔符。</param>
		/// <param name="lowerCase">返回的十六进制字符串中是否使用小写字符，默认为大写。</param>
		/// <returns>参数中元素的字符串表示形式，以十六进制文本表示。</returns>
		public static string ToHexString(byte[] bytes, char separator, bool lowerCase = false)
		{
			return ToHexString(bytes, 0, 0, separator, lowerCase);
		}

		/// <summary>
		/// 将指定的字节数组转换为其用十六进制数字编码的等效字符串表示形式。
		/// </summary>
		/// <param name="bytes">一个 8 位无符号字节数组。</param>
		/// <param name="offset">指定字节数组的起始下标。</param>
		/// <param name="count">指定字节数组的元素个数。</param>
		/// <param name="separator">每字节对应的十六进制文本中间的分隔符。</param>
		/// <param name="lowerCase">返回的十六进制字符串中是否使用小写字符，默认为大写。</param>
		/// <returns>参数中元素的字符串表示形式，以十六进制文本表示。</returns>
		public static string ToHexString(byte[] bytes, int offset, int count, char separator, bool lowerCase = false)
		{
			if(bytes == null || bytes.Length == 0)
				return string.Empty;

			if(offset < 0 || offset >= bytes.Length)
				throw new ArgumentOutOfRangeException(nameof(offset));

			if(count < 1)
				count = (bytes.Length - offset);
			else
				count = Math.Min(bytes.Length - offset, count);

			var alpha = lowerCase ? 'a' : 'A';
			var rank = separator == '\0' ? 2 : 3;
			var characters = new char[count * rank - (rank - 2)];

			for(int i = 0; i < count; i++)
			{
				characters[i * rank] = GetDigit((byte)(bytes[offset + i] / 16), alpha);
				characters[i * rank + 1] = GetDigit((byte)(bytes[offset + i] % 16), alpha);

				if(rank == 3 && i < count - 1)
					characters[i * rank + 2] = separator;
			}

			return new string(characters);
		}

		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
		private static char GetDigit(byte value, char alpha)
		{
			if(value < 10)
				return (char)('0' + value);

			return (char)(alpha + (value - 10));
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

			var index = 0;
			var buffer = new char[2];
			var result = new List<byte>();

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
	}
}

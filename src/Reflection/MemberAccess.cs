/*
 * Authors:
 *   钟峰(Popeye Zhong) <9555843@qq.com>
 *
 * Copyright (C) 2010-2017 Zongsoft Corporation <http://www.zongsoft.com>
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

namespace Zongsoft.Reflection
{
	public class MemberAccess
	{
		#region 路径操作
		public static MemberPathSegment[] Resolve(string path)
		{
			if(string.IsNullOrWhiteSpace(path))
				return null;

			return Resolve(path, message =>
			{
				throw new ArgumentException(message);
			});
		}

		public static bool TryResolve(string path, out MemberPathSegment[] members)
		{
			members = null;

			if(string.IsNullOrWhiteSpace(path))
				return false;

			var succeed = true;

			members = Resolve(path, _ =>
			{
				succeed = false;
			});

			return succeed;
		}

		public static MemberPathSegment[] Resolve(string path, Action<string> onError)
		{
			if(string.IsNullOrWhiteSpace(path))
				return null;

			var part = string.Empty;
			var parts = new List<MemberPathSegment>();
			var quote = '\0';
			var escaping = false;
			var spaces = 0;
			var isString = false;
			var parameterIndex = -1;
			var parameters = new List<object>();

			for(int i = 0; i < path.Length; i++)
			{
				var chr = path[i];

				if(quote != '\0')
				{
					if(escaping)
					{
						char escapedChar;

						if(EscapeChar(chr, out escapedChar))
							part += escapedChar;
						else
							part += '\\' + chr;
					}
					else
					{
						if(chr == quote)
							quote = '\0';
						else if(chr != '\\')
							part += chr;
					}

					//转义状态只有在引号里面才可能发生
					escaping = chr == '\\' && !escaping;
				}
				else
				{
					switch(chr)
					{
						case '.':
							if(parameterIndex >= 0)
							{
								if(string.IsNullOrEmpty(part))
								{
									onError?.Invoke($"Invalid float numeric format in the \"{path}\" path expression.");
									return null;
								}

								if(part.Contains("."))
								{
									onError?.Invoke($"Reduplicated dot(.) in the \"{path}\" path expression.");
									return null;
								}

								int temp;
								if(!int.TryParse(part, out temp))
								{
									onError?.Invoke($"Invalid numeric format in the \"{path}\" path expression.");
									return null;
								}

								part += chr;
								break;
							}

							if(part.Length == 0)
							{
								if(parts.Count > 0 && parts[parts.Count - 1].IsIndexer)
									break;

								if(parts.Count == 0)
									onError?.Invoke($"The first character can not be dot(.) in the \"{path}\" path expression.");
								else
									onError?.Invoke($"Reduplicated dot(.) in the \"{path}\" path expression.");

								return null;
							}

							spaces = 0;

							parts.Add(new MemberPathSegment(part));
							part = string.Empty;

							break;
						case '"':
						case '\'':
							if(parameterIndex < 0)
							{
								onError?.Invoke($"The quotation mark must be in indexer in the \"{path}\" path expression.");
								return null;
							}

							spaces = 0;
							quote = chr;
							part = string.Empty;
							isString = true;

							break;
						case ',':
							if(parameterIndex < 0)
							{
								onError?.Invoke($"The comma(,) bust be in indexer in the \"{path}\" path expression.");
								return null;
							}

							parameters.Add(GetParameterValue(part, isString));
							spaces = 0;
							parameterIndex++;
							isString = false;

							break;
						case '[':
							if(parameterIndex >= 0)
							{
								onError?.Invoke($"Missing matched ']' symbol in the \"{path}\" path expression.");
								return null;
							}

							if(part.Length > 0)
								parts.Add(new MemberPathSegment(part));

							spaces = 0;
							part = null;
							parameterIndex++;
							isString = false;

							break;
						case ']':
							if(parameterIndex < 0)
							{
								onError?.Invoke($"Missing matched '[' symbol in the \"{path}\" path expresion.");
								return null;
							}

							if(part == null)
							{
								onError?.Invoke($"Missing parameters of indexer in the \"{path}\" path expression.");
								return null;
							}

							parameters.Add(GetParameterValue(part, isString));
							parts.Add(new MemberPathSegment(parameters.ToArray()));

							spaces = 0;
							part = string.Empty;
							parameterIndex = -1;
							parameters.Clear();

							break;
						default:
							if(char.IsWhiteSpace(chr))
							{
								//忽略成员前面的空白字符
								if(string.IsNullOrEmpty(part))
									break;

								spaces++;
							}
							else if(char.IsLetter(chr) || chr == '_')
							{
								if(spaces > 0)
								{
									onError?.Invoke($"The member part contains one or many whitespaces in the \"{path}\" path expression.");
									return null;
								}

								part += chr;
							}
							else if(char.IsDigit(chr))
							{
								if(parameterIndex < 0 && part.Length == 0)
								{
									onError?.Invoke($"The first character of member part must be a letter or underscore(_) in the \"{path}\" path expression.");
									return null;
								}

								if(spaces > 0)
								{
									onError?.Invoke($"The member part can not contains whitespaces in the \"{path}\" path expression.");
									return null;
								}

								part += chr;
							}
							else
							{
								onError?.Invoke($"Contains a '{chr}' illegal character in the \"{path}\" path expression.");
								return null;
							}
							break;
					}
				}
			}

			//如果还处在引号里面，则激发错误
			if(quote != '\0')
			{
				onError?.Invoke($"Missing closing quotation mark in the \"{path}\" path expression.");
				return null;
			}

			if(part.Length > 0)
			{
				if(parameterIndex >= 0)
				{
					onError?.Invoke($"Missing matched ']' symbol in the \"{path}\" path expression.");
					return null;
				}

				parts.Add(new MemberPathSegment(part));
			}

			return parts.ToArray();
		}

		private static object GetParameterValue(string value, bool isString)
		{
			if(value == null)
				return null;

			if(isString)
				return value;

			switch(value)
			{
				case "null":
					return null;
				case "true":
					return true;
				case "false":
					return false;
			}

			if(value.Contains("."))
			{
				double result;

				if(double.TryParse(value, out result))
					return result;
			}
			else
			{
				int result;

				if(int.TryParse(value, out result))
					return result;
			}

			return value;
		}
		#endregion

		#region 成员类型
		public static Type GetMemberType(object origin, string path)
		{
			if(origin == null)
				throw new ArgumentNullException(nameof(origin));
			if(string.IsNullOrWhiteSpace(path))
				throw new ArgumentNullException(nameof(path));

			return GetMemberType(origin, Resolve(path));
		}

		public static Type GetMemberType(object origin, MemberPathSegment[] members)
		{
			if(origin == null)
				throw new ArgumentNullException(nameof(origin));
			if(members == null)
				throw new ArgumentNullException(nameof(members));

			MemberInfo memberInfo = origin is Type ? (Type)origin : origin.GetType();

			foreach(var member in members)
			{
				memberInfo = GetMemberInfo(GetMemberType(memberInfo), member);

				if(memberInfo == null)
					throw new ArgumentException($"The '{member}' member is not exists in the '{GetMemberType(memberInfo)}' type.");
			}

			return GetMemberType(memberInfo);
		}

		public static bool TryGetMemberType(object origin, string path, out Type memberType)
		{
			if(origin == null)
				throw new ArgumentNullException(nameof(origin));
			if(string.IsNullOrWhiteSpace(path))
				throw new ArgumentNullException(nameof(path));

			return TryGetMemberType(origin, Resolve(path), out memberType);
		}

		public static bool TryGetMemberType(object origin, MemberPathSegment[] members, out Type memberType)
		{
			if(origin == null)
				throw new ArgumentNullException(nameof(origin));
			if(members == null)
				throw new ArgumentNullException(nameof(members));

			//设置返回参数的默认值
			memberType = null;

			MemberInfo memberInfo = origin is Type ? (Type)origin : origin.GetType();

			foreach(var member in members)
			{
				memberType = GetMemberType(memberInfo);
				memberInfo = GetMemberInfo(memberType, member);

				if(memberInfo == null)
				{
					//将输出参数置空
					memberType = null;

					//返回失败
					return false;
				}
			}

			//获取最后一个成员的成员类型
			memberType = GetMemberType(memberInfo);

			//返回执行结果
			return memberType != null;
		}
		#endregion

		#region 获取成员
		public static T GetMemberValue<T>(object origin, string path, Func<MemberGettingContext, MemberGettingResult> resolve = null)
		{
			if(origin == null)
				throw new ArgumentNullException(nameof(origin));
			if(string.IsNullOrWhiteSpace(path))
				throw new ArgumentNullException(nameof(path));

			return GetMemberValue<T>(origin, Resolve(path), resolve);
		}

		public static T GetMemberValue<T>(object origin, MemberPathSegment[] members, Func<MemberGettingContext, MemberGettingResult> resolve = null)
		{
			if(origin == null)
				throw new ArgumentNullException(nameof(origin));
			if(members == null)
				throw new ArgumentNullException(nameof(members));

			return GetMemberValueCore<T>(origin, members, resolve, (owner, member) =>
			{
				throw new ArgumentException(string.Format("The '{0}' member is not exists in the '{1}' type.", member, (owner is Type ? ((Type)owner).FullName : owner.GetType().FullName)));
			});
		}

		public static bool TryGetMemberValue<T>(object origin, string path, out T value, Func<MemberGettingContext, MemberGettingResult> resolve = null)
		{
			if(origin == null)
				throw new ArgumentNullException(nameof(origin));
			if(string.IsNullOrWhiteSpace(path))
				throw new ArgumentNullException(nameof(path));

			return TryGetMemberValue<T>(origin, Resolve(path), out value, resolve);
		}

		public static bool TryGetMemberValue<T>(object origin, MemberPathSegment[] members, out T value, Func<MemberGettingContext, MemberGettingResult> resolve = null)
		{
			if(origin == null)
				throw new ArgumentNullException(nameof(origin));
			if(members == null)
				throw new ArgumentNullException(nameof(members));

			var succeed = true;
			value = GetMemberValueCore<T>(origin, members, resolve, (_, __) => succeed = false);
			return succeed;
		}
		#endregion

		#region 设置成员
		public static void SetMemberValue<T>(object origin, string path, T value, Func<MemberGettingContext, object> getter = null, Action<MemberSettingContext<T>> setter = null)
		{
			if(origin == null)
				throw new ArgumentNullException(nameof(origin));
			if(string.IsNullOrWhiteSpace(path))
				throw new ArgumentNullException(nameof(path));

			SetMemberValue<T>(origin, Resolve(path), () => value, getter, setter);
		}

		public static void SetMemberValue<T>(object origin, string path, Func<T> valueFactory, Func<MemberGettingContext, object> getter = null, Action<MemberSettingContext<T>> setter = null)
		{
			if(origin == null)
				throw new ArgumentNullException(nameof(origin));
			if(string.IsNullOrWhiteSpace(path))
				throw new ArgumentNullException(nameof(path));
			if(valueFactory == null)
				throw new ArgumentNullException(nameof(valueFactory));

			SetMemberValue(origin, Resolve(path), valueFactory, getter, setter);
		}

		public static void SetMemberValue<T>(object origin, MemberPathSegment[] members, T value, Func<MemberGettingContext, object> getter = null, Action<MemberSettingContext<T>> setter = null)
		{
			if(origin == null)
				throw new ArgumentNullException(nameof(origin));
			if(members == null)
				throw new ArgumentNullException(nameof(members));

			SetMemberValue(origin, members, () => value, getter, setter);
		}

		public static void SetMemberValue<T>(object origin, MemberPathSegment[] members, Func<T> valueFactory, Func<MemberGettingContext, object> getter = null, Action<MemberSettingContext<T>> setter = null)
		{
			if(origin == null)
				throw new ArgumentNullException(nameof(origin));
			if(members == null)
				throw new ArgumentNullException(nameof(members));
			if(valueFactory == null)
				throw new ArgumentNullException(nameof(valueFactory));

			if(members.Length == 0)
				return;

			var owner = origin;
			MemberGettingContext context = null;

			for(int i = 0; i < members.Length - 1; i++)
			{
				if(getter == null)
				{
					if(!TryGetMemberValueCore(owner, members[i], out owner))
						throw new ArgumentException(string.Format("The '{0}' member is not exists in the '{1}' type.", members[i], (owner is Type ? ((Type)owner).FullName : owner.GetType().FullName)));
				}
				else
				{
					//创建获取操作上下文对象
					context = new MemberGettingContext(owner, members[i], context);

					//执行获取操作
					owner = getter(context);

					if(owner == null)
						throw new InvalidOperationException($"The result of getter is null.");
				}
			}

			if(setter == null)
				SetMemberValueCore(owner, members[members.Length - 1], valueFactory, true);
			else
				setter(new MemberSettingContext<T>(owner, members[members.Length - 1], valueFactory, context));
		}

		public static bool TrySetMemberValue<T>(object origin, string path, T value, Func<MemberGettingContext, object> getter = null, Action<MemberSettingContext<T>> setter = null)
		{
			return TrySetMemberValue(origin, Resolve(path), () => value, getter, setter);
		}

		public static bool TrySetMemberValue<T>(object origin, string path, Func<T> valueFactory, Func<MemberGettingContext, object> getter = null, Action<MemberSettingContext<T>> setter = null)
		{
			return TrySetMemberValue(origin, Resolve(path), valueFactory, getter, setter);
		}

		public static bool TrySetMemberValue<T>(object origin, MemberPathSegment[] members, T value, Func<MemberGettingContext, object> getter = null, Action<MemberSettingContext<T>> setter = null)
		{
			return TrySetMemberValue(origin, members, () => value, getter, setter);
		}

		public static bool TrySetMemberValue<T>(object origin, MemberPathSegment[] members, Func<T> valueFactory, Func<MemberGettingContext, object> getter = null, Action<MemberSettingContext<T>> setter = null)
		{
			if(origin == null || members == null || valueFactory == null || members.Length == 0)
				return false;

			var owner = origin;
			MemberGettingContext context = null;

			for(int i = 0; i < members.Length - 1; i++)
			{
				if(getter == null)
				{
					if(!TryGetMemberValueCore(owner, members[i], out owner))
						return false;
				}
				else
				{
					//创建获取操作上下文对象
					context = new MemberGettingContext(owner, members[i], context);

					//执行获取操作
					owner = getter(context);

					if(owner == null)
						return false;
				}
			}

			if(setter == null)
				SetMemberValueCore(owner, members[members.Length - 1], valueFactory, false);
			else
				setter(new MemberSettingContext<T>(owner, members[members.Length - 1], valueFactory, context));

			return true;
		}
		#endregion

		#region 私有方法
		private static bool EscapeChar(char chr, out char escapedChar)
		{
			switch(chr)
			{
				case '"':
				case '\'':
				case '\\':
				case '[':
				case ']':
					escapedChar = chr;
					return true;
				case 's':
					escapedChar = ' ';
					return true;
				case 't':
					escapedChar = '\t';
					return true;
				case 'n':
					escapedChar = '\n';
					return true;
				case 'r':
					escapedChar = '\r';
					return true;
			}

			escapedChar = chr;
			return false;
		}

		internal static MemberInfo GetMemberInfo(object owner, MemberPathSegment segment)
		{
			if(owner == null)
				return null;

			var type = owner is Type ? (Type)owner : owner.GetType();

			var members = type.FindMembers(MemberTypes.Field | MemberTypes.Property, BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static, (member, criteria) =>
			{
				MemberPathSegment memberSegment = (MemberPathSegment)criteria;

				if(memberSegment.IsIndexer && member.MemberType == MemberTypes.Property)
				{
					var parameters = ((PropertyInfo)member).GetIndexParameters();

					if(parameters.Length != memberSegment.Parameters.Length)
						return false;

					for(int i = 0; i < parameters.Length; i++)
					{
						if(parameters[i].ParameterType != memberSegment.Parameters[i].GetType())
							return false;
					}

					return true;
				}

				return string.Equals(member.Name, memberSegment.Name, StringComparison.OrdinalIgnoreCase);
			}, segment);

			if(members != null && members.Length > 0)
				return members[0];

			return null;
		}

		internal static Type GetMemberType(MemberInfo member)
		{
			if(member == null)
				return null;

			if(member is Type)
				return (Type)member;

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

		internal static bool TryGetMemberValueCore(object owner, MemberPathSegment token, out object value)
		{
			value = null;

			var memberInfo = GetMemberInfo(owner, token);

			if(memberInfo == null)
				return false;

			switch(memberInfo.MemberType)
			{
				case MemberTypes.Field:
					value = ((FieldInfo)memberInfo).GetValue(owner);
					return true;
				case MemberTypes.Property:
					value = ((PropertyInfo)memberInfo).GetValue(owner, token.Parameters);
					return true;
				case MemberTypes.Method:
					value = ((MethodInfo)memberInfo).Invoke(owner, token.Parameters);
					return true;
			}

			return false;
		}

		internal static void SetMemberValueCore<T>(object owner, MemberPathSegment member, Func<T> valueFactory, bool throwsOnError)
		{
			if(owner == null)
				throw new ArgumentNullException(nameof(owner));

			var type = owner is Type ? (Type)owner : owner.GetType();
			var memberInfo = GetMemberInfo(type, member);

			//如果没有找到指定的成员
			if(memberInfo == null)
			{
				if(throwsOnError)
					throw new ArgumentException(string.Format("The '{0}' member is not exists in the '{1}' type.", member, (owner is Type ? ((Type)owner).FullName : owner.GetType().FullName)));

				return;
			}

			try
			{
				switch(memberInfo.MemberType)
				{
					case MemberTypes.Field:
						((FieldInfo)memberInfo).SetValue(owner, valueFactory());
						break;
					case MemberTypes.Property:
						((PropertyInfo)memberInfo).SetValue(owner, valueFactory());
						break;
					default:
						if(throwsOnError)
							throw new InvalidOperationException($"Dont support set value of '{member}' member in the '{type}' type.");
						return;
				}
			}
			catch
			{
				if(throwsOnError)
					throw;
			}
		}

		private static T GetMemberValueCore<T>(object origin, MemberPathSegment[] members, Func<MemberGettingContext, MemberGettingResult> resolve, Action<object, MemberPathSegment> onError)
		{
			var owner = origin;
			MemberGettingContext context = null;

			foreach(var member in members)
			{
				if(resolve == null)
				{
					object memberValue;

					if(!TryGetMemberValueCore(owner, member, out memberValue))
					{
						onError(owner, member);
						return default(T);
					}

					owner = memberValue;
				}
				else
				{
					context = new MemberGettingContext(owner, member, context);
					MemberGettingResult result;

					if((result = resolve(context)).Cancel)
						return (T)result.Value;

					owner = result.Value;
				}
			}

			return (T)owner;
		}
		#endregion
	}
}

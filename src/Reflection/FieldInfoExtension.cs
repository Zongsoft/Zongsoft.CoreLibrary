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
using System.Reflection;
using System.Reflection.Emit;
using System.Collections.Concurrent;

namespace Zongsoft.Reflection
{
	public static class FieldInfoExtension
	{
		#region 委托定义
		public delegate object Getter(ref object target);
		public delegate void Setter(ref object target, object value);
		#endregion

		#region 缓存变量
		private static readonly ConcurrentDictionary<FieldInfo, FieldToken> _fields = new ConcurrentDictionary<FieldInfo, FieldToken>();
		#endregion

		#region 公共方法
		public static Getter GetGetter(this FieldInfo field)
		{
			if(field == null)
				throw new ArgumentNullException(nameof(field));

			return _fields.GetOrAdd(field, info => new FieldToken(info, GenerateGetter(info))).Getter;
		}

		public static Setter GetSetter(this FieldInfo field)
		{
			if(field == null)
				throw new ArgumentNullException(nameof(field));

			return _fields.GetOrAdd(field, info => new FieldToken(info, GenerateSetter(info))).Setter;
		}

		public static Getter GenerateGetter(this FieldInfo field)
		{
			if(field == null)
				throw new ArgumentNullException(nameof(field));

			var method = new DynamicMethod("dynamic:" + field.DeclaringType.FullName + "!Get" + field.Name,
				typeof(object),
				new Type[] { typeof(object).MakeByRefType() },
				typeof(FieldInfoExtension),
				true);

			var generator = method.GetILGenerator();

			generator.DeclareLocal(field.DeclaringType);

			generator.Emit(OpCodes.Ldarg_0);
			generator.Emit(OpCodes.Ldind_Ref);
			if(field.DeclaringType.IsValueType)
				generator.Emit(OpCodes.Unbox_Any, field.DeclaringType);
			else
				generator.Emit(OpCodes.Castclass, field.DeclaringType);

			generator.Emit(OpCodes.Ldfld, field);

			if(field.FieldType.IsValueType)
				generator.Emit(OpCodes.Box, field.FieldType);

			generator.Emit(OpCodes.Ret);

			return (Getter)method.CreateDelegate(typeof(Getter));
		}

		public static Setter GenerateSetter(this FieldInfo field)
		{
			if(field == null)
				throw new ArgumentNullException(nameof(field));

			//如果字段为只读则返回空
			if(field.IsInitOnly)
				return null;

			var method = new DynamicMethod("dynamic:" + field.DeclaringType.FullName + "!Set" + field.Name,
				null,
				new Type[] { typeof(object).MakeByRefType(), typeof(object) },
				typeof(FieldInfoExtension),
				true);

			var generator = method.GetILGenerator();

			generator.DeclareLocal(field.DeclaringType);

			generator.Emit(OpCodes.Ldarg_0);
			generator.Emit(OpCodes.Ldind_Ref);
			if(field.DeclaringType.IsValueType)
				generator.Emit(OpCodes.Unbox_Any, field.DeclaringType);
			else
				generator.Emit(OpCodes.Castclass, field.DeclaringType);
			generator.Emit(OpCodes.Stloc_0);

			if(field.DeclaringType.IsValueType)
				generator.Emit(OpCodes.Ldloca_S, 0);
			else
				generator.Emit(OpCodes.Ldloc_0);

			generator.Emit(OpCodes.Ldarg_1);
			if(field.FieldType.IsValueType)
			{
				var underlyingType = Nullable.GetUnderlyingType(field.FieldType) ?? field.FieldType;

				generator.Emit(OpCodes.Ldtoken, underlyingType);
				generator.Emit(OpCodes.Call, typeof(Type).GetMethod(nameof(Type.GetTypeFromHandle), BindingFlags.Public | BindingFlags.Static));
				generator.Emit(OpCodes.Call, typeof(Convert).GetMethod(nameof(Convert.ChangeType), new[] { typeof(object), typeof(Type) }));
				generator.Emit(OpCodes.Unbox_Any, field.FieldType);
			}
			else
				generator.Emit(OpCodes.Castclass, field.FieldType);

			generator.Emit(OpCodes.Stfld, field);

			if(field.DeclaringType.IsValueType)
			{
				generator.Emit(OpCodes.Ldarg_0);
				generator.Emit(OpCodes.Ldloc_0);
				generator.Emit(OpCodes.Box, field.DeclaringType);
				generator.Emit(OpCodes.Stind_Ref);
			}

			generator.Emit(OpCodes.Ret);

			return (Setter)method.CreateDelegate(typeof(Setter));
		}
		#endregion

		#region 嵌套子类
		private class FieldToken
		{
			#region 成员字段
			private FieldInfo _field;
			private Getter _getter;
			private Setter _setter;
			#endregion

			#region 构造函数
			public FieldToken(FieldInfo field, Getter getter)
			{
				_field = field;
				_getter = getter;
			}

			public FieldToken(FieldInfo field, Setter setter)
			{
				_field = field;
				_setter = setter;
			}
			#endregion

			#region 公共属性
			public Getter Getter
			{
				get
				{
					if(_getter == null)
					{
						lock(this)
						{
							if(_getter == null)
								_getter = GenerateGetter(_field);
						}
					}

					return _getter;
				}
			}

			public Setter Setter
			{
				get
				{
					if(_setter == null)
					{
						lock(this)
						{
							if(_setter == null)
								_setter = GenerateSetter(_field);
						}
					}

					return _setter;
				}
			}
			#endregion
		}
		#endregion
	}
}

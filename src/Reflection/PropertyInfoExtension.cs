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
	public static class PropertyInfoExtension
	{
		#region 委托定义
		public delegate object Getter(ref object target, params object[] parameters);
		public delegate void Setter(ref object target, object value, params object[] parameters);
		#endregion

		#region 缓存变量
		private static readonly ConcurrentDictionary<PropertyInfo, PropertyToken> _properties = new ConcurrentDictionary<PropertyInfo, PropertyToken>();
		#endregion

		#region 公共方法
		public static Getter GetGetter(this PropertyInfo property)
		{
			if(property == null)
				throw new ArgumentNullException(nameof(property));

			return _properties.GetOrAdd(property, info => new PropertyToken(info, GenerateGetter(info))).Getter;
		}

		public static Setter GetSetter(this PropertyInfo property)
		{
			if(property == null)
				throw new ArgumentNullException(nameof(property));

			return _properties.GetOrAdd(property, info => new PropertyToken(info, GenerateSetter(info))).Setter;
		}

		public static Getter GenerateGetter(this PropertyInfo property)
		{
			if(property == null)
				throw new ArgumentNullException(nameof(property));

			//如果属性不可读则返回空
			if(!property.CanRead)
				return null;

			var method = new DynamicMethod("dynamic:" + property.DeclaringType.FullName + "!Get" + property.Name, typeof(object), new Type[] { typeof(object).MakeByRefType(), typeof(object[]) }, typeof(PropertyInfoExtension), true);
			var generator = method.GetILGenerator();

			if(!property.GetMethod.IsStatic)
			{
				generator.DeclareLocal(property.DeclaringType);

				generator.Emit(OpCodes.Ldarg_0);
				generator.Emit(OpCodes.Ldind_Ref);
				if(property.DeclaringType.IsValueType)
					generator.Emit(OpCodes.Unbox_Any, property.DeclaringType);
				else
					generator.Emit(OpCodes.Castclass, property.DeclaringType);
				generator.Emit(OpCodes.Stloc_0);

				if(property.DeclaringType.IsValueType)
					generator.Emit(OpCodes.Ldloca_S, 0);
				else
					generator.Emit(OpCodes.Ldloc_0);
			}

			//获取属性的索引器参数
			var parameters = property.GetIndexParameters();
			if(parameters != null && parameters.Length > 0)
			{
				var ERROR_LABEL = generator.DefineLabel();
				var NORMAL_LABEL = generator.DefineLabel();

				//if(parameters == null || ...)
				generator.Emit(OpCodes.Ldarg_1);
				generator.Emit(OpCodes.Brfalse_S, ERROR_LABEL);

				//if(... || parameters.Length < x)
				generator.Emit(OpCodes.Ldarg_1);
				generator.Emit(OpCodes.Ldlen);
				generator.Emit(OpCodes.Conv_I4);
				generator.Emit(OpCodes.Ldc_I4, parameters.Length);
				generator.Emit(OpCodes.Bge_S, NORMAL_LABEL);

				//throw new ArgumentException("...");
				generator.MarkLabel(ERROR_LABEL);
				generator.Emit(OpCodes.Ldstr, $"The count of {property.Name} property indexer parameters is not enough.");
				generator.Emit(OpCodes.Newobj, typeof(ArgumentException).GetConstructor(new Type[] { typeof(string) }));
				generator.Emit(OpCodes.Throw);

				generator.MarkLabel(NORMAL_LABEL);

				//属性索引器获取方法的调用参数
				for(int i = 0; i < parameters.Length; i++)
				{
					generator.Emit(OpCodes.Ldarg_1);
					generator.Emit(OpCodes.Ldc_I4, i);
					generator.Emit(OpCodes.Ldelem_Ref);

					if(parameters[i].ParameterType.IsValueType)
						generator.Emit(OpCodes.Unbox_Any, parameters[i].ParameterType);
					else
						generator.Emit(OpCodes.Castclass, parameters[i].ParameterType);
				}
			}

			//调用属性的获取方法
			if(property.DeclaringType.IsValueType || property.GetMethod.IsStatic)
				generator.Emit(OpCodes.Call, property.GetMethod);
			else
				generator.Emit(OpCodes.Callvirt, property.GetMethod);

			if(property.PropertyType.IsValueType)
				generator.Emit(OpCodes.Box, property.PropertyType);

			generator.Emit(OpCodes.Ret);

			return (Getter)method.CreateDelegate(typeof(Getter));
		}

		public static Setter GenerateSetter(this PropertyInfo property)
		{
			if(property == null)
				throw new ArgumentNullException(nameof(property));

			//如果属性不可写则返回空
			if(!property.CanWrite)
				return null;

			var method = new DynamicMethod("dynamic:" + property.DeclaringType.FullName + "!Set" + property.Name, null, new Type[] { typeof(object).MakeByRefType(), typeof(object), typeof(object[]) }, typeof(PropertyInfoExtension), true);
			var generator = method.GetILGenerator();

			if(!property.SetMethod.IsStatic)
			{
				generator.DeclareLocal(property.DeclaringType);

				generator.Emit(OpCodes.Ldarg_0);
				generator.Emit(OpCodes.Ldind_Ref);
				if(property.DeclaringType.IsValueType)
					generator.Emit(OpCodes.Unbox_Any, property.DeclaringType);
				else
					generator.Emit(OpCodes.Castclass, property.DeclaringType);
				generator.Emit(OpCodes.Stloc_0);

				if(property.DeclaringType.IsValueType)
					generator.Emit(OpCodes.Ldloca_S, 0);
				else
					generator.Emit(OpCodes.Ldloc_0);
			}

			//获取属性的索引器参数
			var parameters = property.GetIndexParameters();
			if(parameters != null && parameters.Length > 0)
			{
				var ERROR_LABEL = generator.DefineLabel();
				var NORMAL_LABEL = generator.DefineLabel();

				//if(parameters == null || ...)
				generator.Emit(OpCodes.Ldarg_2);
				generator.Emit(OpCodes.Brfalse_S, ERROR_LABEL);

				//if(... || parameters.Length < x)
				generator.Emit(OpCodes.Ldarg_2);
				generator.Emit(OpCodes.Ldlen);
				generator.Emit(OpCodes.Conv_I4);
				generator.Emit(OpCodes.Ldc_I4, parameters.Length);
				generator.Emit(OpCodes.Bge_S, NORMAL_LABEL);

				//throw new ArgumentException("...");
				generator.MarkLabel(ERROR_LABEL);
				generator.Emit(OpCodes.Ldstr, $"The count of {property.Name} property indexer parameters is not enough.");
				generator.Emit(OpCodes.Newobj, typeof(ArgumentException).GetConstructor(new Type[] { typeof(string) }));
				generator.Emit(OpCodes.Throw);

				generator.MarkLabel(NORMAL_LABEL);

				//属性索引器获取方法的调用参数
				for(int i = 0; i < parameters.Length; i++)
				{
					generator.Emit(OpCodes.Ldarg_2);
					generator.Emit(OpCodes.Ldc_I4, i);
					generator.Emit(OpCodes.Ldelem_Ref);

					if(parameters[i].ParameterType.IsValueType)
						generator.Emit(OpCodes.Unbox_Any, parameters[i].ParameterType);
					else
						generator.Emit(OpCodes.Castclass, parameters[i].ParameterType);
				}
			}

			generator.Emit(OpCodes.Ldarg_1);
			if(property.PropertyType.IsValueType)
			{
				var underlyingType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;

				generator.Emit(OpCodes.Ldtoken, underlyingType);
				generator.Emit(OpCodes.Call, typeof(Type).GetMethod(nameof(Type.GetTypeFromHandle), BindingFlags.Public | BindingFlags.Static));
				generator.Emit(OpCodes.Call, typeof(Convert).GetMethod(nameof(Convert.ChangeType), new[] { typeof(object), typeof(Type) }));
				generator.Emit(OpCodes.Unbox_Any, property.PropertyType);
			}
			else
				generator.Emit(OpCodes.Castclass, property.PropertyType);

			//调用属性的设置方法
			if(property.DeclaringType.IsValueType || property.SetMethod.IsStatic)
				generator.Emit(OpCodes.Call, property.SetMethod);
			else
				generator.Emit(OpCodes.Callvirt, property.SetMethod);

			if(property.DeclaringType.IsValueType && (!property.SetMethod.IsStatic))
			{
				generator.Emit(OpCodes.Ldarg_0);
				generator.Emit(OpCodes.Ldloc_0);
				generator.Emit(OpCodes.Box, property.DeclaringType);
				generator.Emit(OpCodes.Stind_Ref);
			}

			generator.Emit(OpCodes.Ret);

			return (Setter)method.CreateDelegate(typeof(Setter));
		}
		#endregion

		#region 嵌套子类
		private class PropertyToken
		{
			#region 成员字段
			private PropertyInfo _property;
			private Getter _getter;
			private Setter _setter;
			#endregion

			#region 构造函数
			public PropertyToken(PropertyInfo property, Getter getter)
			{
				_property = property;
				_getter = getter;
			}

			public PropertyToken(PropertyInfo property, Setter setter)
			{
				_property = property;
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
								_getter = GenerateGetter(_property);
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
								_setter = GenerateSetter(_property);
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

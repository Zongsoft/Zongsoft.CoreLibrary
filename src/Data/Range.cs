/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2016-2018 Zongsoft Corporation <http://www.zongsoft.com>
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
using System.Linq.Expressions;
using System.Collections.Concurrent;

namespace Zongsoft.Data
{
	public static class Range
	{
		#region 委托定义
		private delegate void Getter(object target, out object minimum, out object maximum);
		#endregion

		#region 委托缓存
		private static readonly ConcurrentDictionary<Type, Getter> _methods = new ConcurrentDictionary<Type, Getter>();
		#endregion

		#region 公共方法
		public static Range<T> Create<T>(T minimum, T maximum) where T : struct, IComparable<T>
		{
			return new Range<T>(minimum, maximum);
		}

		public static bool IsRange(object target)
		{
			if(target == null)
				return false;

			return IsRange(target.GetType());
		}

		public static bool IsRange(Type type)
		{
			if(type == null)
				throw new ArgumentNullException(nameof(type));

			if(type.IsArray)
			{
				var elementType = type.GetElementType();

				if(typeof(IComparable).IsAssignableFrom(elementType) ||
				   typeof(IComparable<>).MakeGenericType(elementType).IsAssignableFrom(elementType))
					return true;

				return false;
			}

			return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Range<>);
		}

		public static void GetRange(object target, out object minimum, out object maximum)
		{
			if(target == null)
				throw new ArgumentNullException(nameof(target));

			if(!TryGetRange(target, out minimum, out maximum))
				throw new ArgumentException("The specified target is not a range.");
		}

		public static bool TryGetRange(object target, out object minimum, out object maximum)
		{
			minimum = null;
			maximum = null;

			if(target == null)
				return false;

			var type = target.GetType();

			if(!IsRange(type))
				return false;

			if(type.IsArray)
			{
				Array array = (Array)target;

				if(array.Length >= 2)
				{
					minimum = array.GetValue(0);
					maximum = array.GetValue(1);

					return true;
				}

				return false;
			}

			var elementType = type.GetGenericArguments()[0];
			_methods.GetOrAdd(elementType, Compile(elementType)).Invoke(target, out minimum, out maximum);

			return minimum != null || maximum != null;
		}
		#endregion

		#region 私有方法
		private static Getter Compile(Type type)
		{
			var rangeType = typeof(Range<>).MakeGenericType(type);
			var nullableType = typeof(Nullable<>).MakeGenericType(type);

			var method = new DynamicMethod("GetRange_" + type.Name, null, new Type[] { typeof(object), typeof(object).MakeByRefType(), typeof(object).MakeByRefType() }, typeof(Range), true);

			method.DefineParameter(1, ParameterAttributes.None, "target");
			method.DefineParameter(2, ParameterAttributes.Out, "minimum");
			method.DefineParameter(3, ParameterAttributes.Out, "maximum");

			var generator = method.GetILGenerator();

			//local_0 : Range<T>
			generator.DeclareLocal(rangeType);
			//local_1 : Nullable<T>
			generator.DeclareLocal(nullableType);

			var minimumElseLabel = generator.DefineLabel();
			var maximumIfLabel = generator.DefineLabel();
			var maximumElseLabel = generator.DefineLabel();

			//local_0 = (Range<T>)target;
			generator.Emit(OpCodes.Ldarg_0);
			generator.Emit(OpCodes.Unbox_Any, rangeType);
			generator.Emit(OpCodes.Stloc_0);

			//if(!range.Minimum.HasValue)
			generator.Emit(OpCodes.Ldloca_S, 0);
			generator.Emit(OpCodes.Call, rangeType.GetProperty("Minimum").GetMethod);
			generator.Emit(OpCodes.Stloc_1);
			generator.Emit(OpCodes.Ldloca_S, 1);
			generator.Emit(OpCodes.Call, nullableType.GetProperty("HasValue").GetMethod);
			generator.Emit(OpCodes.Brtrue_S, minimumElseLabel);

			//minimum = null;
			generator.Emit(OpCodes.Ldarg_1);
			generator.Emit(OpCodes.Ldnull);
			generator.Emit(OpCodes.Stind_Ref);
			generator.Emit(OpCodes.Br_S, maximumIfLabel);

			generator.MarkLabel(minimumElseLabel);

			//minimum = range.Minimum.Value
			generator.Emit(OpCodes.Ldarg_1);

			generator.Emit(OpCodes.Ldloca_S, 1);
			generator.Emit(OpCodes.Call, nullableType.GetProperty("Value").GetMethod);
			generator.Emit(OpCodes.Box, type);
			generator.Emit(OpCodes.Stind_Ref);

			generator.MarkLabel(maximumIfLabel);

			//if(!range.Maximum.HasValue)
			generator.Emit(OpCodes.Ldloca_S, 0);
			generator.Emit(OpCodes.Call, rangeType.GetProperty("Maximum").GetMethod);
			generator.Emit(OpCodes.Stloc_1);
			generator.Emit(OpCodes.Ldloca_S, 1);
			generator.Emit(OpCodes.Call, nullableType.GetProperty("HasValue").GetMethod);
			generator.Emit(OpCodes.Brtrue_S, maximumElseLabel);

			//maximum = null;
			generator.Emit(OpCodes.Ldarg_2);
			generator.Emit(OpCodes.Ldnull);
			generator.Emit(OpCodes.Stind_Ref);
			generator.Emit(OpCodes.Ret);

			generator.MarkLabel(maximumElseLabel);

			//maximum = range.Maximum.Value
			generator.Emit(OpCodes.Ldarg_2);

			generator.Emit(OpCodes.Ldloca_S, 1);
			generator.Emit(OpCodes.Call, nullableType.GetProperty("Value").GetMethod);
			generator.Emit(OpCodes.Box, type);
			generator.Emit(OpCodes.Stind_Ref);
			generator.Emit(OpCodes.Ret);

			return (Getter)method.CreateDelegate(typeof(Getter));
		}

		private static Getter CompileWithExpression(Type type)
		{
			var target = Expression.Parameter(typeof(object), "target");
			var minimum = Expression.Parameter(typeof(object).MakeByRefType(), "minimum");
			var maximum = Expression.Parameter(typeof(object).MakeByRefType(), "maximum");

			var variables = new []
			{
				Expression.Variable(typeof(Range<>).MakeGenericType(type), "range"),
			};

			var minimumProperty = Expression.Property(variables[0], variables[0].Type.GetProperty("Minimum"));
			var maximumProperty = Expression.Property(variables[0], variables[0].Type.GetProperty("Maximum"));

			var statements = new Expression[]
			{
				Expression.Assign(variables[0], Expression.Convert(target, variables[0].Type)),

				Expression.IfThenElse(
					Expression.Equal(minimumProperty, Expression.Constant(null)),
					Expression.Assign(minimum, Expression.Constant(null)),
					Expression.Assign(minimum, Expression.Convert(Expression.Property(minimumProperty, typeof(Nullable<>).MakeGenericType(type).GetProperty("Value")), typeof(object)))),

				Expression.IfThenElse(
					Expression.Equal(maximumProperty, Expression.Constant(null)),
					Expression.Assign(maximum, Expression.Constant(null)),
					Expression.Assign(maximum, Expression.Convert(Expression.Property(maximumProperty, typeof(Nullable<>).MakeGenericType(type).GetProperty("Value")), typeof(object))))
			};

			return Expression.Lambda<Getter>(Expression.Block(variables, statements), target, minimum, maximum).Compile();
		}

		/// <summary>
		/// 示例：动态编译后的代码。
		/// </summary>
		/// <param name="target">指定的待解析的范围对象。</param>
		/// <param name="minimum">返回参数，指定的范围对象的最小值。</param>
		/// <param name="maximum">返回参数，指定的范围对象的最大值。</param>
		private static void GetRange_XXX(object target, out object minimum, out object maximum)
		{
			var range = (Range<int>)target;

			if(range.Minimum == null)
				minimum = null;
			else
				minimum = range.Minimum.Value;

			if(range.Maximum == null)
				maximum = null;
			else
				maximum = range.Maximum.Value;
		}
		#endregion
	}
}

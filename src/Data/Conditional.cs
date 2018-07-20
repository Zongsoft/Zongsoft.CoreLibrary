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
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace Zongsoft.Data
{
	/// <summary>
	/// 提供条件转换的静态类。
	/// </summary>
	public static class Conditional
	{
		#region 静态变量
		private static readonly ConcurrentDictionary<Type, ConditionalDescriptor> _cache = new ConcurrentDictionary<Type, ConditionalDescriptor>();
		#endregion

		#region 公共方法
		public static ICondition ToCondition(this IConditional conditional)
		{
			if(conditional == null)
				return null;

			var changes = conditional.GetChanges();

			if(changes == null || changes.Count == 0)
				return null;

			return conditional.ToConditions() & condition;
		}

		public static ConditionCollection operator &(Conditional left, Conditional right)
		{
			if(left == null)
				return right;

			if(right == null)
				return left;

			return left.ToConditions() & right.ToConditions();
		}

		public static ConditionCollection operator |(Condition condition, Conditional conditional)
		{
			if(conditional == null)
				return null;

			var descriptor = _cache.GetOrAdd(conditional.GetType(), type => new ConditionalDescriptor(type));
			return GenerateCondition(conditional, descriptor.Properties[changes.First().Key]);
		}

		public static ConditionCollection ToConditions(this IConditional conditional)
		{
			if(conditional == null)
				return null;

			return ToConditions(conditional, conditional.GetChanges());
		}
		#endregion

		#region 私有方法
		private static ConditionCollection ToConditions(IConditional conditional, IDictionary<string, object> changes)
		{
			if(changes == null || changes.Count == 0)
				return null;

			ConditionCollection conditions = null;
			var descriptor = _cache.GetOrAdd(conditional.GetType(), type => new ConditionalDescriptor(type));

			//处理已经被更改过的属性
			foreach(var change in changes)
			{
				var condition = GenerateCondition(conditional, descriptor.Properties[change.Key]);

				if(condition != null)
				{
					if(conditions == null)
						conditions = new ConditionCollection(conditional.Combination);

					conditions.Add(condition);
				}
			}

			return conditions;
		}

		private static ICondition GenerateCondition(IConditional conditional, ConditionalPropertyDescripor property)
		{
			//如果当前属性值为默认值，则忽略它
			if(property == null)
				return null;

			//获取当前属性对应的条件命列表
			var names = GetConditionNames(property);

			//创建转换器上下文
			var context = new ConditionalConverterContext(conditional,
				property.Attribute == null ? ConditionalBehaviors.None : property.Attribute.Behaviors,
				names,
				property.PropertyType,
				property.GetValue(conditional),
				property.Operator);

			//如果当前属性指定了特定的转换器，则使用该转换器来处理
			if(property.Converter != null)
				return property.Converter.Convert(context);

			//使用默认转换器进行转换处理
			return ConditionalConverter.Default.Convert(context);
		}

		private static string[] GetConditionNames(ConditionalPropertyDescripor property)
		{
			if(property.Attribute != null && property.Attribute.Names != null && property.Attribute.Names.Length > 0)
				return property.Attribute.Names;

			return new string[] { property.Name };
		}
		#endregion

		#region 嵌套子类
		private class ConditionalDescriptor
		{
			public readonly Type Type;
			public readonly IDictionary<string, ConditionalPropertyDescripor> Properties;

			public ConditionalDescriptor(Type type)
			{
				this.Type = type;

				var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
				this.Properties = new Dictionary<string, ConditionalPropertyDescripor>();

				foreach(var property in properties)
				{
					if(!property.CanRead)
						continue;

					var attribute = property.GetCustomAttribute<ConditionalAttribute>(true);

					if(attribute != null && attribute.Ignored)
						continue;

					this.Properties.Add(property.Name, new ConditionalPropertyDescripor(property, attribute));
				}
			}
		}

		private class ConditionalPropertyDescripor
		{
			public readonly string Name;
			public readonly Type PropertyType;
			public readonly PropertyInfo PropertyInfo;
			public readonly ConditionalAttribute Attribute;
			public readonly IConditionalConverter Converter;

			public ConditionalPropertyDescripor(PropertyInfo property, ConditionalAttribute attribute)
			{
				this.PropertyInfo = property;
				this.Attribute = attribute;
				this.Name = property.Name;
				this.PropertyType = property.PropertyType;

				if(attribute != null && attribute.ConverterType != null)
					this.Converter = Activator.CreateInstance(attribute.ConverterType) as IConditionalConverter;
			}

			public ConditionOperator? Operator
			{
				get
				{
					return this.Attribute != null ? this.Attribute.Operator : null;
				}
			}

			public object GetValue(object target)
			{
				return this.PropertyInfo.GetValue(target);
			}
		}
		#endregion
	}
}

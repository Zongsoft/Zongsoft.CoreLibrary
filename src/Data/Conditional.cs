/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2016 Zongsoft Corporation <http://www.zongsoft.com>
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
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace Zongsoft.Data
{
	/// <summary>
	/// 表示数据过滤条件的组合实体。
	/// </summary>
	public abstract class Conditional : Zongsoft.Common.ModelBase, IConditional
	{
		#region 静态变量
		private static readonly ConcurrentDictionary<Type, ConditionalDescriptor> _cache = new ConcurrentDictionary<Type, ConditionalDescriptor>();
		#endregion

		#region 成员字段
		private ConditionCombination _conditionCombination;
		private ConditionalBehaviors _defaultBehaviors;
		#endregion

		#region 构造函数
		protected Conditional()
		{
			_defaultBehaviors = ConditionalBehaviors.None;
			_conditionCombination = ConditionCombination.And;
		}

		protected Conditional(ConditionalBehaviors defaultBehaviors)
		{
			_defaultBehaviors = defaultBehaviors;
			_conditionCombination = ConditionCombination.And;
		}
		#endregion

		#region 保护属性
		protected ConditionCombination ConditionCombination
		{
			get
			{
				return _conditionCombination;
			}
			set
			{
				_conditionCombination = value;
			}
		}

		protected ConditionalBehaviors DefaultBehaviors
		{
			get
			{
				return _defaultBehaviors;
			}
			set
			{
				_defaultBehaviors = value;
			}
		}
		#endregion

		#region 符号重写
		public static implicit operator ConditionCollection(Conditional conditional)
		{
			if(conditional == null)
				return null;

			return conditional.ToConditions();
		}

		public static ConditionCollection operator &(Condition condition, Conditional conditional)
		{
			if(conditional == null)
				return null;

			return condition & conditional.ToConditions();
		}

		public static ConditionCollection operator &(Conditional conditional, Condition condition)
		{
			if(conditional == null)
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

			return condition | conditional.ToConditions();
		}

		public static ConditionCollection operator |(Conditional conditional, Condition condition)
		{
			if(conditional == null)
				return null;

			return conditional.ToConditions() | condition;
		}

		public static ConditionCollection operator |(Conditional left, Conditional right)
		{
			if(left == null)
				return right;

			if(right == null)
				return left;

			return left.ToConditions() | right.ToConditions();
		}
		#endregion

		#region 公共方法
		public bool Contains(string name)
		{
			return this.HasChanges(name);
		}

		public ICondition[] Find(string name)
		{
			if(string.IsNullOrWhiteSpace(name))
				return null;

			var conditions = this.ToConditions();

			if(conditions != null && conditions.Count > 0)
				return conditions.Find(name);

			return null;
		}

		public virtual ConditionCollection ToConditions()
		{
			ConditionCollection conditions = null;
			var descriptor = _cache.GetOrAdd(this.GetType(), type => new ConditionalDescriptor(type));

			//只遍历基类属性字典中的属性（即显式设置过的属性）
			foreach(var property in this.GetChangedProperties())
			{
				var condition = this.GenerateCondition(descriptor.Properties[property.Key]);

				if(condition != null)
				{
					if(conditions == null)
						conditions = new ConditionCollection(this.ConditionCombination);

					conditions.Add(condition);
				}
			}

			return conditions;
		}
		#endregion

		#region 重写方法
		public override string ToString()
		{
			var conditions = this.ToConditions();

			if(conditions == null || conditions.Count == 0)
				return string.Empty;

			return conditions.ToString();
		}
		#endregion

		#region 私有方法
		private ICondition GenerateCondition(ConditionalPropertyDescripor property)
		{
			//如果当前属性值为默认值，则忽略它
			if(property == null)
				return null;

			//获取当前属性对应的条件命列表
			var names = this.GetConditionNames(property);

			//创建转换器上下文
			var context = new ConditionalConverterContext(this, property.Attribute == null ? _defaultBehaviors : property.Attribute.Behaviors, names, property.PropertyType, property.GetValue(this), property.Operator);

			//如果当前属性指定了特定的转换器，则使用该转换器来处理
			if(property.Converter != null)
				return property.Converter.Convert(context);

			//使用默认转换器进行转换处理
			return ConditionalConverter.Default.Convert(context);
		}

		private string[] GetConditionNames(ConditionalPropertyDescripor property)
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
				this.Properties = new ConcurrentDictionary<string, ConditionalPropertyDescripor>();

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
			public readonly PropertyInfo Info;
			public readonly ConditionalAttribute Attribute;
			public readonly IConditionalConverter Converter;

			public ConditionalPropertyDescripor(PropertyInfo property, ConditionalAttribute attribute)
			{
				this.Info = property;
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
				return this.Info.GetValue(target);
			}
		}
		#endregion
	}
}

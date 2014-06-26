/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2008-2013 Zongsoft Corporation <http://www.zongsoft.com>
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

namespace Zongsoft.ComponentModel
{
	[Obsolete]
	public class ConditionResolver
	{
		#region 静态字段
		public static readonly ConditionResolver Default = new ConditionResolver();
		#endregion

		#region 事件定义
		public event EventHandler AffixWildcardChanged;
		public event EventHandler IgnoreConditionCombineChanged;
		public event EventHandler IgnorePagingChanged;
		#endregion

		#region 成员变量
		private bool _affixWildcard;
		private bool _ignoreConditionCombine;
		private bool _ignorePaging;
		#endregion

		#region 构造函数
		public ConditionResolver()
		{
			_affixWildcard = false;
			_ignoreConditionCombine = false;
			_ignorePaging = false;
		}
		#endregion

		#region 公共属性
		[DefaultValue(false)]
		public bool AffixWildcard
		{
			get
			{
				return _affixWildcard;
			}
			set
			{
				if(_affixWildcard == value)
					return;

				_affixWildcard = value;
				this.OnAffixWildcardChanged(EventArgs.Empty);
			}
		}

		[DefaultValue(false)]
		public bool IgnoreConditionCombine
		{
			get
			{
				return _ignoreConditionCombine;
			}
			set
			{
				if(_ignoreConditionCombine == value)
					return;

				_ignoreConditionCombine = value;
				this.OnIgnoreConditionCombineChanged(EventArgs.Empty);
			}
		}

		[DefaultValue(false)]
		public bool IgnorePaging
		{
			get
			{
				return _ignorePaging;
			}
			set
			{
				if(_ignorePaging == value)
					return;

				_ignorePaging = value;
				this.OnIgnorePagingChanged(EventArgs.Empty);
			}
		}
		#endregion

		#region 虚拟属性
		public virtual string Wildcard
		{
			get
			{
				return "%";
			}
		}
		#endregion

		#region 虚拟成员
		public virtual void Reset(object target)
		{
			if(target == null)
				return;

			foreach(PropertyDescriptor property in TypeDescriptor.GetProperties(target))
			{
				if(property.IsReadOnly)
					continue;

				if(property.CanResetValue(target))
					property.ResetValue(target);
			}
		}

		public virtual IDictionary<string, object> Resolve(ICondition condition)
		{
			Dictionary<string, object> parameters = new Dictionary<string, object>();

			if(condition == null)
				return parameters;

			//如果不忽略条件组合
			if(!this.IgnoreConditionCombine)
			{
				if(condition.ConditionCombine == ConditionCombine.And)
					parameters.Add(Zongsoft.Data.Parameter.Criteria.Name, "AND");
				else
					parameters.Add(Zongsoft.Data.Parameter.Criteria.Name, "OR");
			}

			//如果不忽略分页
			if(!this.IgnorePaging)
			{
				parameters.Add(Zongsoft.Data.Parameter.PageSize.Name, condition.PageSize);
				parameters.Add(Zongsoft.Data.Parameter.PageIndex.Name, condition.PageIndex);
			}

			object value = null;

			foreach(PropertyDescriptor property in TypeDescriptor.GetProperties(condition))
			{
				if(property.Attributes[typeof(IgnoreConditionAttribute)] != null)
					continue;

				if(property.PropertyType == typeof(string))
					value = this.GetStringParameterValue((string)property.GetValue(condition));
				else
					value = property.GetValue(condition);

				DefaultValueAttribute defaultAttribute = (DefaultValueAttribute)property.Attributes[typeof(DefaultValueAttribute)];

				if(defaultAttribute != null)
				{
					if(value == defaultAttribute.Value)
						continue;
				}
				else
				{
					if(property.PropertyType.IsClass && value == null)
						continue;
				}

				//如果属性值类型为枚举，则将其值转换为枚举的基础类型
				if(value != null && value.GetType().IsEnum)
				{
					Type underlyingType = Enum.GetUnderlyingType(value.GetType());
					value = Convert.ChangeType(value, underlyingType);
				}

				AliasAttribute alias = (AliasAttribute)property.Attributes[typeof(AliasAttribute)];

				if(alias == null)
					parameters[property.Name] = value;
				else
					parameters[alias.Alias] = value;
			}

			return parameters;
		}

		public virtual ICondition Resolve(IDictionary<string, object> parameters)
		{
			if(parameters == null || parameters.Count < 1)
				return null;

			ICondition condition = new Condition();

			if(parameters.ContainsKey(Zongsoft.Data.Parameter.Criteria.Name))
			{
			}

			if(parameters.ContainsKey(Zongsoft.Data.Parameter.PageSize.Name))
				condition.PageSize = (int)parameters[Zongsoft.Data.Parameter.PageSize.Name];
			else
				condition.PageSize = -1;

			return condition;
		}

		protected virtual void OnAffixWildcardChanged(EventArgs e)
		{
			if(this.AffixWildcardChanged != null)
				this.AffixWildcardChanged(this, e);
		}

		protected virtual void OnIgnoreConditionCombineChanged(EventArgs e)
		{
			if(this.IgnoreConditionCombineChanged != null)
				this.IgnoreConditionCombineChanged(this, e);
		}

		protected virtual void OnIgnorePagingChanged(EventArgs e)
		{
			if(this.IgnorePagingChanged != null)
				this.IgnorePagingChanged(this, e);
		}
		#endregion

		#region 私有方法
		private string GetStringParameterValue(string parameterValue)
		{
			if(string.IsNullOrEmpty(parameterValue))
				return null;

			string result = parameterValue;
			string wildcard = this.Wildcard;

			if(this.AffixWildcard && (!string.IsNullOrEmpty(wildcard)))
			{
				if(!parameterValue.StartsWith(wildcard))
					result = wildcard + result;
				if(!parameterValue.EndsWith(wildcard))
					result = result + wildcard;
			}

			return result;
		}
		#endregion
	}
}

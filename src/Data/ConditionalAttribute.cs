/*
 *   _____                                ______
 *  /_   /  ____  ____  ____  _________  / __/ /_
 *    / /  / __ \/ __ \/ __ \/ ___/ __ \/ /_/ __/
 *   / /__/ /_/ / / / / /_/ /\_ \/ /_/ / __/ /_
 *  /____/\____/_/ /_/\__  /____/\____/_/  \__/
 *                   /____/
 *
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
using System.Collections.Generic;

namespace Zongsoft.Data
{
	/// <summary>
	/// 表示条件组合成员的描述特性。
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	public class ConditionalAttribute : Attribute
	{
		#region 成员字段
		private string[] _names;
		private bool _ignored;
		private ConditionalBehaviors _behaviors;
		private ConditionOperator? _operator;
		private Type _converterType;
		#endregion

		#region 构造函数
		public ConditionalAttribute(bool ignored)
		{
			_ignored = ignored;
			_behaviors = ConditionalBehaviors.IgnoreNullOrEmpty;
		}

		public ConditionalAttribute(params string[] names)
		{
			_names = EnsureNames(names);
			_behaviors = ConditionalBehaviors.IgnoreNullOrEmpty;
		}

		public ConditionalAttribute(ConditionOperator @operator, params string[] names) : this(ConditionalBehaviors.IgnoreNullOrEmpty, @operator, names)
		{
		}

		public ConditionalAttribute(ConditionalBehaviors behaviors, params string[] names)
		{
			_behaviors = behaviors;
			_names = EnsureNames(names);
		}

		public ConditionalAttribute(ConditionalBehaviors behaviors, ConditionOperator @operator, params string[] names)
		{
			_behaviors = behaviors;
			_operator = @operator;
			_names = EnsureNames(names);
		}

		public ConditionalAttribute(Type converterType, params string[] names) : this(converterType, ConditionalBehaviors.IgnoreNullOrEmpty, names)
		{
		}

		public ConditionalAttribute(Type converterType, ConditionOperator @operator, params string[] names) : this(converterType, ConditionalBehaviors.IgnoreNullOrEmpty, @operator, names)
		{
		}

		public ConditionalAttribute(Type converterType, ConditionalBehaviors behaviors, params string[] names)
		{
			this.ConverterType = converterType;
			_behaviors = behaviors;
			_names = EnsureNames(names);
		}

		public ConditionalAttribute(Type converterType, ConditionalBehaviors behaviors, ConditionOperator @operator, params string[] names)
		{
			this.ConverterType = converterType;
			_behaviors = behaviors;
			_operator = @operator;
			_names = EnsureNames(names);
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取或设置条件成员对应的条件名数组。
		/// </summary>
		public string[] Names
		{
			get
			{
				return _names;
			}
			set
			{
				_names = EnsureNames(value);
			}
		}

		/// <summary>
		/// 获取或设置一个值，指示是否忽略当前条件成员。
		/// </summary>
		public bool Ignored
		{
			get
			{
				return _ignored;
			}
			set
			{
				_ignored = value;
			}
		}

		/// <summary>
		/// 获取或设置查询条件的行为。
		/// </summary>
		public ConditionalBehaviors Behaviors
		{
			get
			{
				return _behaviors;
			}
			set
			{
				_behaviors = value;
			}
		}

		/// <summary>
		/// 获取或设置条件成员的运算符，如果为空则表示自动匹配一个合适的运算符。
		/// </summary>
		public ConditionOperator? Operator
		{
			get
			{
				return _operator;
			}
			set
			{
				_operator = value;
			}
		}

		/// <summary>
		/// 获取或设置条件转换器类型。
		/// </summary>
		public Type ConverterType
		{
			get
			{
				return _converterType;
			}
			set
			{
				if(value != null && !typeof(IConditionalConverter).IsAssignableFrom(value))
					throw new ArgumentException();

				_converterType = value;
			}
		}
		#endregion

		#region 私有方法
		private static string[] EnsureNames(string[] names)
		{
			if(names == null || names.Length < 1)
				return null;

			var map = new HashSet<string>();

			foreach(var name in names)
			{
				if(string.IsNullOrWhiteSpace(name))
					continue;

				foreach(var part in name.Split(','))
				{
					if(!string.IsNullOrWhiteSpace(part))
						map.Add(part.Trim());
				}
			}

			if(map.Count > 0)
			{
				var result = new string[map.Count];
				map.CopyTo(result);
				return result;
			}

			return null;
		}
		#endregion
	}
}

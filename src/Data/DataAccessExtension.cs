/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2010-2015 Zongsoft Corporation <http://www.zongsoft.com>
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
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace Zongsoft.Data
{
	public static class DataAccessExtension
	{
		#region 扩展方法
		public static void Execute(this IDataAccess dataAccess, string name, object inParameters)
		{
			if(dataAccess == null)
				throw new ArgumentNullException("dataAccess");

			dataAccess.Execute(name, ConvertToDictionary(inParameters));
		}

		public static void Execute(this IDataAccess dataAccess, string name, object inParameters, out IDictionary<string, object> outParameters)
		{
			if(dataAccess == null)
				throw new ArgumentNullException("dataAccess");

			dataAccess.Execute(name, ConvertToDictionary(inParameters), out outParameters);
		}

		public static int Delete(this IDataAccess dataAccess, string name, IDictionary<string, object> condition, string scope = null)
		{
			if(dataAccess == null)
				throw new ArgumentNullException("dataAccess");

			return dataAccess.Delete(name, ResolveCondition(condition), scope);
		}

		public static int Delete(this IDataAccess dataAccess, string name, object condition, string scope = null)
		{
			if(dataAccess == null)
				throw new ArgumentNullException("dataAccess");

			return dataAccess.Delete(name, ResolveCondition(condition), scope);
		}

		public static int Update(this IDataAccess dataAccess, string name, object entity, IDictionary<string, object> condition, string scope = null)
		{
			if(dataAccess == null)
				throw new ArgumentNullException("dataAccess");

			return dataAccess.Update(name, entity, ResolveCondition(condition), scope);
		}

		public static int Update(this IDataAccess dataAccess, string name, object entity, object condition, string scope = null)
		{
			if(dataAccess == null)
				throw new ArgumentNullException("dataAccess");

			return dataAccess.Update(name, entity, ResolveCondition(condition), scope);
		}

		public static int Update<T>(this IDataAccess dataAccess, string name, IEnumerable<T> entities, IDictionary<string, object> condition, string scope = null)
		{
			if(dataAccess == null)
				throw new ArgumentNullException("dataAccess");

			return dataAccess.Update(name, entities, ResolveCondition(condition), scope);
		}

		public static int Update<T>(this IDataAccess dataAccess, string name, IEnumerable<T> entities, object condition, string scope = null)
		{
			if(dataAccess == null)
				throw new ArgumentNullException("dataAccess");

			return dataAccess.Update(name, entities, ResolveCondition(condition), scope);
		}
		#endregion

		public static void ResolveScopeString(string scope, out string[] includes, out string[] excludes)
		{
			//设置输出参数的默认返回值
			includes = excludes = new string[0];

			if(string.IsNullOrWhiteSpace(scope))
				return;

			var includeSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			var excludeSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

			var parts = scope.Split(',', ';');

			for(int i = 0; i < parts.Length; i++)
			{
				var part = parts[i].Trim();

				if(part.Length == 0)
					continue;

				switch(part[0])
				{
					case '-':
					case '!':
						if(part.Length > 1)
							excludeSet.Add(part.Substring(1));
						else
							excludeSet.Add("!");

						break;
					case '*':
						if(part.Length != 1)
							throw new ArgumentException("scope");

						includeSet.Add("*");
						break;
					default:
						if((part[0] >= 'A' && part[0] <= 'Z') || (part[0] >= 'a' && part[0] <= 'z') || part[0] == '_')
							includeSet.Add(part);
						else
							throw new ArgumentException("scope");

						break;
				}
			}
		}

		#region 私有方法
		private static ConditionCollection ResolveCondition(object condition)
		{
			if(condition == null)
				return null;

			var properties = TypeDescriptor.GetProperties(condition);

			if(properties == null || properties.Count < 1)
				return null;

			var result = new ConditionCollection(ConditionCombine.Or);

			foreach(PropertyDescriptor property in properties)
			{
				result.Add(ConvertToCondition(property.Name, property.GetValue(condition)));
			}

			return result;
		}

		private static ConditionCollection ResolveCondition(IDictionary<string, object> condition)
		{
			if(condition == null || condition.Count < 1)
				return null;

			var result = new ConditionCollection(ConditionCombine.Or);

			foreach(var parameter in condition)
			{
				result.Add(ConvertToCondition(parameter.Key, parameter.Value));
			}

			return result;
		}

		private static ICondition ConvertToCondition(string name, object value)
		{
			if(value == null)
				return new Condition(name, null);

			Type valueType = value.GetType();

			if(valueType.IsEnum)
				return new Condition(name, Zongsoft.Common.Convert.ConvertValue(value, Enum.GetUnderlyingType(valueType)));

			if(valueType.IsArray || typeof(ICollection).IsAssignableFrom(valueType))
				return new Condition(name, value, ConditionOperator.In);

			if(typeof(IDictionary).IsAssignableFrom(valueType))
			{
				var clauses = new ConditionCollection(ConditionCombine.Or);

				foreach(DictionaryEntry entry in (IDictionary)value)
				{
					if(entry.Key != null)
						clauses.Add(ConvertToCondition(entry.Key.ToString(), entry.Value));
				}

				return clauses;
			}

			if(typeof(IDictionary<string, object>).IsAssignableFrom(valueType))
			{
				var clauses = new ConditionCollection(ConditionCombine.Or);

				foreach(var entry in (IDictionary<string, object>)value)
				{
					if(entry.Key != null)
						clauses.Add(ConvertToCondition(entry.Key, entry.Value));
				}

				return clauses;
			}

			return new Condition(name, value);
		}

		private static IDictionary<string, object> ConvertToDictionary(object condition)
		{
			if(condition == null)
				return null;

			if(condition.GetType().IsValueType)
				throw new ArgumentException();

			var dictionary = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
			var properties = System.ComponentModel.TypeDescriptor.GetProperties(condition);

			foreach(PropertyDescriptor property in properties)
			{
				dictionary.Add(property.Name, property.GetValue(condition));
			}

			return dictionary;
		}
		#endregion
	}
}

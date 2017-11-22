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
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Zongsoft.Data
{
	public static class DataAccessExtension
	{
		#region 扩展方法
		public static IEnumerable<T> Execute<T>(this IDataAccess dataAccess, string name, object inParameters)
		{
			if(dataAccess == null)
				throw new ArgumentNullException("dataAccess");

			return dataAccess.Execute<T>(name, ConvertToDictionary(inParameters));
		}

		public static IEnumerable<T> Execute<T>(this IDataAccess dataAccess, string name, object inParameters, out IDictionary<string, object> outParameters)
		{
			if(dataAccess == null)
				throw new ArgumentNullException("dataAccess");

			return dataAccess.Execute<T>(name, ConvertToDictionary(inParameters), out outParameters);
		}

		public static int Delete<T>(this IDataAccess dataAccess, string name, ICondition condition, Expression<Func<T, object>> cascades)
		{
			if(dataAccess == null)
				throw new ArgumentNullException("dataAccess");

			if(cascades == null)
				return dataAccess.Delete(name, condition);

			return dataAccess.Delete(name, condition, ResolveScopeExpression(name, cascades, null, type => IsScalarType(type, dataAccess)));
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

		public static int Update<T>(this IDataAccess dataAccess, string name, T data, ICondition condition, Expression<Func<T, object>> includes, Expression<Func<T, object>> excludes = null)
		{
			if(dataAccess == null)
				throw new ArgumentNullException("dataAccess");

			if(data == null)
				throw new ArgumentNullException("data");

			return dataAccess.Update(name, data, condition, ResolveScopeExpression(name, includes, excludes, type => IsScalarType(type, dataAccess)));
		}

		public static int UpdateMany<T>(this IDataAccess dataAccess, string name, IEnumerable<T> data, Expression<Func<T, object>> includes, Expression<Func<T, object>> excludes = null)
		{
			if(dataAccess == null)
				throw new ArgumentNullException("dataAccess");

			return dataAccess.UpdateMany(name, data, ResolveScopeExpression(name, includes, excludes, type => IsScalarType(type, dataAccess)));
		}

		public static int Insert<T>(this IDataAccess dataAccess, string name, T data, Expression<Func<T, object>> includes, Expression<Func<T, object>> excludes = null)
		{
			return dataAccess.Insert(name, data, ResolveScopeExpression(name, includes, excludes, type => IsScalarType(type, dataAccess)));
		}

		public static int InsertMany<T>(this IDataAccess dataAccess, string name, IEnumerable<T> data, Expression<Func<T, object>> includes, Expression<Func<T, object>> excludes = null)
		{
			return dataAccess.InsertMany(name, data, ResolveScopeExpression(name, includes, excludes, type => IsScalarType(type, dataAccess)));
		}
		#endregion

		#region 私有方法
		private static ConditionCollection ResolveCondition(object condition)
		{
			if(condition == null)
				return null;

			var properties = TypeDescriptor.GetProperties(condition);

			if(properties == null || properties.Count < 1)
				return null;

			var result = new ConditionCollection(ConditionCombination.Or);

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

			var result = new ConditionCollection(ConditionCombination.Or);

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
				var clauses = new ConditionCollection(ConditionCombination.Or);

				foreach(DictionaryEntry entry in (IDictionary)value)
				{
					if(entry.Key != null)
						clauses.Add(ConvertToCondition(entry.Key.ToString(), entry.Value));
				}

				return clauses;
			}

			if(typeof(IDictionary<string, object>).IsAssignableFrom(valueType))
			{
				var clauses = new ConditionCollection(ConditionCombination.Or);

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

		private static string ResolveScopeExpression<T>(string entityName, Expression<Func<T, object>> includes, Expression<Func<T, object>> excludes, Func<Type, bool> isScalarType)
		{
			var members = new HashSet<string>();

			if(includes != null)
				members.UnionWith(ResolveExpression(includes, isScalarType));

			if(excludes != null)
				members.ExceptWith(ResolveExpression(excludes, isScalarType));

			return string.Join(",", members.ToArray());
		}

		private static IEnumerable<string> ResolveExpression<T>(Expression<Func<T, object>> expression, Func<Type, bool> isScalarType)
		{
			return ResolveExpression(expression.Body, expression.Parameters[0], isScalarType);
		}

		private static string[] ResolveExpression(Expression expression, ParameterExpression parameter, Func<Type, bool> isScalarType)
		{
			if(expression == parameter)
				return GetMembers(expression.Type, null, isScalarType);

			if(expression.NodeType == ExpressionType.MemberAccess)
			{
				if(isScalarType(expression.Type))
					return new string[] { ExpressionToString(expression, parameter) };

				var memberName = GetMemberName(expression, parameter);
				return GetMembers(expression.Type, memberName, isScalarType);
			}

			if(expression is NewExpression)
			{
				HashSet<string> list = new HashSet<string>();
				var ne = (NewExpression)expression;
				foreach(var argument in ne.Arguments)
				{
					list.UnionWith(ResolveExpression(argument, parameter, isScalarType));
				}
				return list.ToArray();
			}

			if(expression.NodeType == ExpressionType.Convert && expression.Type == typeof(object))
				return ResolveExpression(((UnaryExpression)expression).Operand, parameter, isScalarType);

			throw new NotSupportedException();
		}

		private static string ExpressionToString(Expression expression, Expression stop)
		{
			if(expression == stop)
				return string.Empty;

			dynamic propertyExpression = expression;

			var str = ExpressionToString(propertyExpression.Expression, stop);

			if(string.IsNullOrEmpty(str))
				return propertyExpression.Member.Name;

			return str + "." + propertyExpression.Member.Name;
		}

		private static string GetMemberName(Expression expression, ParameterExpression parameter)
		{
			dynamic propertyExpression = expression;

			var temp = (Expression)propertyExpression.Expression;

			if(temp == parameter)
				return propertyExpression.Member.Name;

			return GetMemberName(temp, parameter) + "." + propertyExpression.Member.Name;
		}

		private static string[] GetMembers(Type type, string prev, Func<Type, bool> isScalarType)
		{
			if(prev == null)
				prev = string.Empty;
			else
				prev += ".";

			return type.GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(p => isScalarType(p.PropertyType)).Select(p => prev + p.Name).ToArray();
		}

		private static bool IsScalarType(Type type, IDataAccess dataAccess)
		{
			return Zongsoft.Common.TypeExtension.IsScalarType(type) || typeof(Expression).IsAssignableFrom(type);
		}
		#endregion
	}
}

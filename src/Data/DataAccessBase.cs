/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2010-2016 Zongsoft Corporation <http://www.zongsoft.com>
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
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Reflection;
using System.Linq;
using System.Linq.Expressions;

namespace Zongsoft.Data
{
	/// <summary>
	/// 表示数据访问的抽象基类。
	/// </summary>
	public abstract class DataAccessBase : MarshalByRefObject, IDataAccess
	{
		#region 事件定义
		public event EventHandler<DataCountedEventArgs> Counted;
		public event EventHandler<DataCountingEventArgs> Counting;
		public event EventHandler<DataExecutedEventArgs> Executed;
		public event EventHandler<DataExecutingEventArgs> Executing;
		public event EventHandler<DataSelectedEventArgs> Selected;
		public event EventHandler<DataSelectingEventArgs> Selecting;
		public event EventHandler<DataDeletedEventArgs> Deleted;
		public event EventHandler<DataDeletingEventArgs> Deleting;
		public event EventHandler<DataInsertedEventArgs> Inserted;
		public event EventHandler<DataInsertingEventArgs> Inserting;
		public event EventHandler<DataUpdatedEventArgs> Updated;
		public event EventHandler<DataUpdatingEventArgs> Updating;
		#endregion

		#region 私有字段
		private ConcurrentDictionary<Type, EntityDesciptior> _entityCache;
		#endregion

		#region 构造函数
		protected DataAccessBase()
		{
			_entityCache = new ConcurrentDictionary<Type, EntityDesciptior>();
		}
		#endregion

		#region 执行方法
		public object Execute(string name, IDictionary<string, object> inParameters)
		{
			IDictionary<string, object> outParameters;
			return this.Execute(name, inParameters, out outParameters);
		}

		public abstract object Execute(string name, IDictionary<string, object> inParameters, out IDictionary<string, object> outParameters);
		#endregion

		#region 存在方法
		public abstract bool Exists(string name, ICondition condition);
		#endregion

		#region 计数方法
		public int Count(string name, ICondition condition, string includes = null)
		{
			if(string.IsNullOrWhiteSpace(includes))
				return this.Count(name, condition, (string[])null);

			return this.Count(name, condition, includes.Split(',', ';'));
		}

		protected abstract int Count(string name, ICondition condition, string[] includes);
		#endregion

		#region 查询方法
		public IEnumerable<T> Select<T>(string name, ICondition condition = null, params Sorting[] sortings)
		{
			return this.Select<T>(name, condition, this.ResolveScope(name, null, typeof(T)), null, null, null);
		}

		public IEnumerable<T> Select<T>(string name, ICondition condition, string scope, params Sorting[] sortings)
		{
			return this.Select<T>(name, condition, this.ResolveScope(name, scope, typeof(T)), null, null, sortings);
		}

		public IEnumerable<T> Select<T>(string name, ICondition condition, string scope, Paging paging, params Sorting[] sortings)
		{
			return this.Select<T>(name, condition, this.ResolveScope(name, scope, typeof(T)), paging, null, sortings);
		}

		public IEnumerable<T> Select<T>(string name, ICondition condition, Paging paging, params Sorting[] sortings)
		{
			return this.Select<T>(name, condition, this.ResolveScope(name, null, typeof(T)), paging, null, sortings);
		}

		public IEnumerable<T> Select<T>(string name, ICondition condition, Paging paging, string scope, params Sorting[] sortings)
		{
			return this.Select<T>(name, condition, this.ResolveScope(name, scope, typeof(T)), paging, null, sortings);
		}

		public IEnumerable<T> Select<T>(string name, ICondition condition, Grouping grouping, params Sorting[] sortings)
		{
			return this.Select<T>(name, condition, this.ResolveScope(name, null, typeof(T)), null, grouping, sortings);
		}

		public IEnumerable<T> Select<T>(string name, ICondition condition, Grouping grouping, string scope, params Sorting[] sortings)
		{
			return this.Select<T>(name, condition, this.ResolveScope(name, scope, typeof(T)), null, grouping, sortings);
		}

		public IEnumerable<T> Select<T>(string name, ICondition condition, Grouping grouping, string scope, Paging paging, params Sorting[] sortings)
		{
			return this.Select<T>(name, condition, this.ResolveScope(name, scope, typeof(T)), paging, grouping, sortings);
		}

		public IEnumerable<T> Select<T>(string name, ICondition condition, Grouping grouping, Paging paging, params Sorting[] sortings)
		{
			return this.Select<T>(name, condition, this.ResolveScope(name, null, typeof(T)), paging, grouping, sortings);
		}

		public IEnumerable<T> Select<T>(string name, ICondition condition, Grouping grouping, Paging paging, string scope, params Sorting[] sortings)
		{
			return this.Select<T>(name, condition, this.ResolveScope(name, scope, typeof(T)), paging, grouping, sortings);
		}

		[Obsolete]
		public IEnumerable<T> Select<T>(string name,
		                                ICondition condition = null,
		                                Expression<Func<T, object>> includes = null,
		                                Expression<Func<T, object>> excludes = null,
		                                Paging paging = null,
		                                Grouping grouping = null,
		                                params Sorting[] sortings)
		{
			return this.Select<T>(name, condition, this.ResolveScopeExpression(name, includes, excludes), paging, grouping, sortings);
		}

		protected abstract IEnumerable<T> Select<T>(string name,
		                                            ICondition condition,
		                                            string[] members,
		                                            Paging paging,
		                                            Grouping grouping,
		                                            Sorting[] sortings);
		#endregion

		#region 删除方法
		public int Delete(string name, ICondition condition, string cascades = null)
		{
			if(string.IsNullOrWhiteSpace(cascades))
				return this.Delete(name, condition, (string[])null);

			return this.Delete(name, condition, cascades.Split(',', ';'));
		}

		public int Delete<T>(string name, ICondition condition, Expression<Func<T, object>> cascades)
		{
			if(cascades == null)
				return this.Delete(name, condition, (string[])null);

			return this.Delete(name, condition, this.ResolveScopeExpression(name, cascades, null));
		}

		protected abstract int Delete(string name, ICondition condition, string[] cascades);
		#endregion

		#region 插入方法
		public int Insert(string name, object data, string scope = null)
		{
			if(data == null)
				throw new ArgumentNullException("entity");

			return this.Insert(name, data, this.ResolveScope(name, scope, data.GetType()));
		}

		public int Insert<T>(string name, T data, Expression<Func<T, object>> includes, Expression<Func<T, object>> excludes = null)
		{
			return this.Insert(name, data, this.ResolveScopeExpression(name, includes, excludes));
		}

		protected virtual int Insert(string name, object data, string[] members)
		{
			return this.InsertMany(name, new object[] { data }, members);
		}

		public int InsertMany<T>(string name, IEnumerable<T> data, string scope = null)
		{
			return this.InsertMany(name, data, this.ResolveScope(name, scope, typeof(T)));
		}

		public int InsertMany<T>(string name, IEnumerable<T> data, Expression<Func<T, object>> includes, Expression<Func<T, object>> excludes = null)
		{
			return this.InsertMany(name, data, this.ResolveScopeExpression(name, includes, excludes));
		}

		protected abstract int InsertMany(string name, IEnumerable data, string[] members);
		#endregion

		#region 更新方法
		/// <summary>
		/// 根据指定的条件将指定的实体更新到数据源。
		/// </summary>
		/// <param name="name">指定的实体映射名。</param>
		/// <param name="data">要更新的实体对象。</param>
		/// <param name="condition">要更新的条件子句，如果为空(null)则根据实体的主键进行更新。</param>
		/// <param name="scope">指定的要更新的和排除更新的属性名列表，如果指定的是多个属性则属性名之间使用逗号(,)分隔；要排除的属性以减号(-)打头，星号(*)表示所有属性，感叹号(!)表示排除所有属性；如果未指定该参数则默认只会更新所有单值属性而不会更新导航属性。</param>
		/// <returns>返回受影响的记录行数，执行成功返回大于零的整数，失败则返回负数。</returns>
		public int Update(string name, object data, ICondition condition = null, string scope = null)
		{
			if(data == null)
				throw new ArgumentNullException("data");

			return this.Update(name, data, condition, this.ResolveScope(name, scope, data.GetType()));
		}

		public int Update(string name, object data, string scope, ICondition condition = null)
		{
			if(data == null)
				throw new ArgumentNullException("data");

			return this.Update(name, data, condition, this.ResolveScope(name, scope, data.GetType()));
		}

		public int Update<T>(string name, T data, ICondition condition, Expression<Func<T, object>> includes, Expression<Func<T, object>> excludes = null)
		{
			if(data == null)
				throw new ArgumentNullException("data");

			return this.Update(name, data, condition, this.ResolveScopeExpression(name, includes, excludes));
		}

		protected virtual int Update(string name, object data, ICondition condition, string[] members)
		{
			return this.UpdateMany(name, new object[] { data }, condition, members);
		}

		/// <summary>
		/// 根据指定的条件将指定的实体集更新到数据源。
		/// </summary>
		/// <typeparam name="T">指定的实体集中的实体的类型。</typeparam>
		/// <param name="name">指定的实体映射名。</param>
		/// <param name="data">要更新的实体集。</param>
		/// <param name="condition">要更新的条件子句，如果为空(null)则根据实体的主键进行更新。</param>
		/// <param name="scope">指定的要更新的和排除更新的属性名列表，如果指定的是多个属性则属性名之间使用逗号(,)分隔；要排除的属性以减号(-)打头，星号(*)表示所有属性，感叹号(!)表示排除所有属性；如果未指定该参数则默认只会更新所有单值属性而不会更新导航属性。</param>
		/// <returns>返回受影响的记录行数，执行成功返回大于零的整数，失败则返回负数。</returns>
		public int UpdateMany<T>(string name, IEnumerable<T> data, ICondition condition = null, string scope = null)
		{
			return this.UpdateMany(name, data, condition, this.ResolveScope(name, scope, typeof(T)));
		}

		public int UpdateMany<T>(string name, IEnumerable<T> data, string scope, ICondition condition = null)
		{
			return this.UpdateMany(name, data, condition, this.ResolveScope(name, scope, typeof(T)));
		}

		public int UpdateMany<T>(string name, IEnumerable<T> data, ICondition condition, Expression<Func<T, object>> includes, Expression<Func<T, object>> excludes = null)
		{
			return this.UpdateMany(name, data, condition, this.ResolveScopeExpression(name, includes, excludes));
		}

		protected abstract int UpdateMany(string name, IEnumerable data, ICondition condition, string[] members);
		#endregion

		#region 递增方法
		public abstract long Increment(string name, string member, ICondition condition, int interval = 1);

		public long Decrement(string name, string member, ICondition condition, int interval = 1)
		{
			return this.Increment(name, member, condition, -interval);
		}
		#endregion

		#region 保护方法
		protected abstract Type GetEntityType(string name);

		protected virtual bool IsScalarType(Type type)
		{
			return Zongsoft.Common.TypeExtension.IsScalarType(type) || typeof(Expression).IsAssignableFrom(type);
		}
		#endregion

		#region 激发事件
		protected virtual void OnCounted(DataCountedEventArgs args)
		{
			var e = this.Counted;

			if(e != null)
				e(this, args);
		}

		protected virtual void OnCounting(DataCountingEventArgs args)
		{
			var e = this.Counting;

			if(e != null)
				e(this, args);
		}

		protected virtual void OnExecuted(DataExecutedEventArgs args)
		{
			var e = this.Executed;

			if(e != null)
				e(this, args);
		}

		protected virtual void OnExecuting(DataExecutingEventArgs args)
		{
			var e = this.Executing;

			if(e != null)
				e(this, args);
		}

		protected virtual void OnSelected(DataSelectedEventArgs args)
		{
			var e = this.Selected;

			if(e != null)
				e(this, args);
		}

		protected virtual void OnSelecting(DataSelectingEventArgs args)
		{
			var e = this.Selecting;

			if(e != null)
				e(this, args);
		}

		protected virtual void OnDeleted(DataDeletedEventArgs args)
		{
			var e = this.Deleted;

			if(e != null)
				e(this, args);
		}

		protected virtual void OnDeleting(DataDeletingEventArgs args)
		{
			var e = this.Deleting;

			if(e != null)
				e(this, args);
		}

		protected virtual void OnInserted(DataInsertedEventArgs args)
		{
			var e = this.Inserted;

			if(e != null)
				e(this, args);
		}

		protected virtual void OnInserting(DataInsertingEventArgs args)
		{
			var e = this.Inserting;

			if(e != null)
				e(this, args);
		}

		protected virtual void OnUpdated(DataUpdatedEventArgs args)
		{
			var e = this.Updated;

			if(e != null)
				e(this, args);
		}

		protected virtual void OnUpdating(DataUpdatingEventArgs args)
		{
			var e = this.Updating;

			if(e != null)
				e(this, args);
		}
		#endregion

		#region 私有方法
		private string[] ResolveScope(string entityName, string scope, Type entityType)
		{
			if(string.IsNullOrWhiteSpace(entityName))
				throw new ArgumentNullException("entityName");

			var isWeakType = entityType != null && (typeof(IDictionary).IsAssignableFrom(entityType) || Zongsoft.Common.TypeExtension.IsAssignableFrom(typeof(IDictionary<,>), entityType));

			if(entityType == null || isWeakType)
				entityType = this.GetEntityType(entityName);

			var entityDescriptor = _entityCache.GetOrAdd(entityType, type => new EntityDesciptior(this, entityName, type));
			return this.ResolveScope(entityDescriptor, scope, isWeakType).ToArray();
		}

		private HashSet<string> ResolveScope(EntityDesciptior entity, string scope, bool isWeakType)
		{
			var result = new HashSet<string>(entity.Properties.Where(p => p.IsScalarType).Select(p => p.PropertyName), StringComparer.OrdinalIgnoreCase);

			if(string.IsNullOrWhiteSpace(scope))
				return result;

			var members = scope.Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);

			for(int i = 0; i < members.Length; i++)
			{
				var member = members[i].Trim();

				if(member.Length == 0)
					continue;

				switch(member[0])
				{
					case '-':
					case '!':
						if(member.Length > 1)
							result.Remove(member.Substring(1));
						else
							result.Clear();

						break;
					case '*':
						if(member.Length != 1)
							throw new ArgumentException("scope");

						result.UnionWith(entity.Properties.SelectMany(p =>
						{
							if(p.IsScalarType)
								return new string[] { p.PropertyName };

							var list = new List<string>();
							this.GetComplexPropertyMembers(entity.EntityName, p.PropertyName, p.PropertyType, list, new HashSet<Type>(new Type[] { p.PropertyType }));
							return list.ToArray();
						}));

						break;
					default:
						if((member[0] >= 'A' && member[0] <= 'Z') || (member[0] >= 'a' && member[0] <= 'z') || member[0] == '_')
						{
							EntityPropertyDescriptor property = null;

							if(member.Contains("."))
							{
								var navigationProperty = GetNavigationProperty(member, entity.EntityType, isWeakType);

								if(navigationProperty != null)
									property = new EntityPropertyDescriptor(member, navigationProperty.PropertyType, this.IsScalarType(navigationProperty.PropertyType));
							}
							else
							{
								property = entity.Properties.FirstOrDefault(p => string.Equals(p.PropertyName, member, StringComparison.OrdinalIgnoreCase));
							}

							if(property == null)
							{
								if(isWeakType)
								{
									result.Add(member);
									continue;
								}

								throw new ArgumentException(string.Format("The '{0}' property is not exists in the '{1}' entity.", member, entity.EntityName));
							}

							if(property.IsScalarType)
								result.Add(member);
							else
							{
								var list = new List<string>();
								this.GetComplexPropertyMembers(entity.EntityName, property.PropertyName, property.PropertyType, list, null);
								result.UnionWith(list);
							}
						}
						else
						{
							throw new ArgumentException(string.Format("Invalid '{0}' member in the '{1}' scope.", member, scope));
						}

						break;
				}
			}

			return result;
		}

		private PropertyDescriptor GetNavigationProperty(string path, Type type, bool isWeakType)
		{
			if(string.IsNullOrWhiteSpace(path))
				return null;

			var parts = path.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
			PropertyDescriptor property = null;

			foreach(var part in parts)
			{
				property = TypeDescriptor.GetProperties(type).Find(part, true);

				if(property == null)
					throw new ArgumentException(string.Format("The '{0}' member is not existed in the '{1}' type, the original text is '{2}'.", part, type.FullName, path));

				type = property.PropertyType;
			}

			return property;
		}

		private void GetComplexPropertyMembers(string entityName, string memberPrefix, Type memberType, ICollection<string> collection, HashSet<Type> recursiveStack)
		{
			foreach(PropertyDescriptor property in TypeDescriptor.GetProperties(memberType))
			{
				if(this.IsScalarType(property.PropertyType))
					collection.Add(memberPrefix + "." + property.Name);
				else if(recursiveStack != null && !recursiveStack.Contains(property.PropertyType))
				{
					recursiveStack.Add(property.PropertyType);
					GetComplexPropertyMembers(entityName, memberPrefix + "." + property.Name, property.PropertyType, collection, recursiveStack);
				}
			}
		}

		private string[] ResolveScopeExpression<T>(string entityName, Expression<Func<T, object>> includes, Expression<Func<T, object>> excludes)
		{
			var members = new HashSet<string>();

			if(includes == null)
				members.UnionWith(this.ResolveScope(entityName, null, typeof(T)));
			else
				members.UnionWith(this.ResolveExpression(includes));

			if(excludes != null)
				members.ExceptWith(this.ResolveExpression(excludes));

			return members.ToArray();
		}

		private IEnumerable<string> ResolveExpression<T>(Expression<Func<T, object>> expression)
		{
			return this.ResolveExpression(expression.Body, expression.Parameters[0]);
		}

		private string[] ResolveExpression(Expression expression, ParameterExpression parameter)
		{
			if(expression == parameter)
				return GetMembers(expression.Type);

			//if(expression.GetType().FullName == "System.Linq.Expressions.PropertyExpression")
			if(expression.NodeType == ExpressionType.MemberAccess)
			{
				if(IsScalarType(expression.Type))
					return new string[] { ExpressionToString(expression, parameter) };

				var memberName = GetMemberName(expression, parameter);
				return GetMembers(expression.Type, memberName);
			}

			if(expression is NewExpression)
			{
				HashSet<string> list = new HashSet<string>();
				var ne = (NewExpression)expression;
				foreach(var argument in ne.Arguments)
				{
					list.UnionWith(ResolveExpression(argument, parameter));
				}
				return list.ToArray();
			}

			if(expression.NodeType == ExpressionType.Convert && expression.Type == typeof(object))
				return ResolveExpression(((UnaryExpression)expression).Operand, parameter);

			throw new NotSupportedException();
		}

		private string ExpressionToString(Expression expression, Expression stop)
		{
			if(expression == stop)
				return string.Empty;

			dynamic propertyExpression = expression;

			var str = ExpressionToString(propertyExpression.Expression, stop);

			if(string.IsNullOrEmpty(str))
				return propertyExpression.Member.Name;

			return str + "." + propertyExpression.Member.Name;
		}

		private string GetMemberName(Expression expression, ParameterExpression parameter)
		{
			dynamic propertyExpression = expression;

			var temp = (Expression)propertyExpression.Expression;

			if(temp == parameter)
				return propertyExpression.Member.Name;

			return GetMemberName(temp, parameter) + "." + propertyExpression.Member.Name;
		}

		private string[] GetMembers(Type type, string prev = null)
		{
			if(prev == null)
				prev = string.Empty;
			else
				prev += ".";

			return type.GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(p => IsScalarType(p.PropertyType)).Select(p => prev + p.Name).ToArray();
		}
		#endregion

		#region 嵌套子类
		private class EntityDesciptior
		{
			public readonly string EntityName;
			public readonly Type EntityType;

			private readonly DataAccessBase _dataAccess;
			private EntityPropertyDescriptor[] _properties;

			public EntityDesciptior(DataAccessBase dataAccess, string entityName, Type entityType)
			{
				_dataAccess = dataAccess;

				if(string.IsNullOrWhiteSpace(entityName))
					throw new ArgumentNullException("entityName");

				this.EntityName = entityName;
				this.EntityType = entityType;
			}

			public EntityPropertyDescriptor[] Properties
			{
				get
				{
					if(_properties == null)
					{
						lock(this)
						{
							if(_properties == null)
								this.InitializeProperties(this.EntityType);
						}
					}

					return _properties;
				}
			}

			private void InitializeProperties(Type type)
			{
				var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);

				_properties = new EntityPropertyDescriptor[properties.Length];

				for(int i=0; i< properties.Length; i++)
				{
					_properties[i] = new EntityPropertyDescriptor(properties[i].Name, properties[i].PropertyType, _dataAccess.IsScalarType(properties[i].PropertyType));
				}
			}
		}

		private class EntityPropertyDescriptor
		{
			public readonly string PropertyName;
			public readonly Type PropertyType;
			public readonly bool IsScalarType;

			public EntityPropertyDescriptor(string propertyName, Type propertyType, bool isScalarType)
			{
				this.PropertyName = propertyName;
				this.PropertyType = propertyType;
				this.IsScalarType = isScalarType;
			}
		}
		#endregion
	}
}

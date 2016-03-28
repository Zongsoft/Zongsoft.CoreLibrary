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

		#region 构造函数
		protected DataAccessBase()
		{
		}
		#endregion

		#region 执行方法
		public IEnumerable<T> Execute<T>(string name, IDictionary<string, object> inParameters)
		{
			IDictionary<string, object> outParameters;
			return this.Execute<T>(name, inParameters, out outParameters);
		}

		public IEnumerable<T> Execute<T>(string name, IDictionary<string, object> inParameters, out IDictionary<string, object> outParameters)
		{
			//激发“Executing”事件
			var args = this.OnExecuting(name, typeof(T), inParameters, out outParameters);

			if(args.Cancel)
				return args.Result as IEnumerable<T>;

			//执行数据操作方法
			args.Result = this.OnExecute<T>(name, inParameters, out outParameters);

			//激发“Executed”事件
			return this.OnExecuted(name, typeof(T), args.InParameters, ref outParameters, args.Result) as IEnumerable<T>;
		}

		protected abstract IEnumerable<T> OnExecute<T>(string name, IDictionary<string, object> inParameters, out IDictionary<string, object> outParameters);

		public object ExecuteScalar(string name, IDictionary<string, object> inParameters)
		{
			IDictionary<string, object> outParameters;
			return this.ExecuteScalar(name, inParameters, out outParameters);
		}

		public object ExecuteScalar(string name, IDictionary<string, object> inParameters, out IDictionary<string, object> outParameters)
		{
			//激发“Executing”事件
			var args = this.OnExecuting(name, typeof(object), inParameters, out outParameters);

			if(args.Cancel)
				return args.Result;

			//执行数据操作方法
			args.Result = this.OnExecuteScalar(name, inParameters, out outParameters);

			//激发“Executed”事件
			return this.OnExecuted(name, typeof(object), args.InParameters, ref outParameters, args.Result);
		}

		protected abstract object OnExecuteScalar(string name, IDictionary<string, object> inParameters, out IDictionary<string, object> outParameters);
		#endregion

		#region 存在方法
		public abstract bool Exists(string name, ICondition condition);
		#endregion

		#region 计数方法
		public int Count(string name, ICondition condition, string includes = null)
		{
			//激发“Counting”事件
			var args = this.OnCounting(name, condition, includes);

			if(args.Cancel)
				return args.Result;

			//执行计数操作方法
			args.Result = this.OnCount(name, condition, string.IsNullOrWhiteSpace(includes) ? (string[])null : includes.Split(',', ';'));

			//激发“Counted”事件
			return this.OnCounted(name, args.Condition, args.Includes, args.Result);
		}

		protected abstract int OnCount(string name, ICondition condition, string[] includes);
		#endregion

		#region 查询方法
		public IEnumerable<T> Select<T>(string name, ICondition condition = null, params Sorting[] sortings)
		{
			return this.Select<T>(name, condition, null, string.Empty, null, sortings);
		}

		public IEnumerable<T> Select<T>(string name, ICondition condition, string scope, params Sorting[] sortings)
		{
			return this.Select<T>(name, condition, null, scope, null, sortings);
		}

		public IEnumerable<T> Select<T>(string name, ICondition condition, string scope, Paging paging, params Sorting[] sortings)
		{
			return this.Select<T>(name, condition, null, scope, paging, sortings);
		}

		public IEnumerable<T> Select<T>(string name, ICondition condition, Paging paging, params Sorting[] sortings)
		{
			return this.Select<T>(name, condition, null, string.Empty, paging, sortings);
		}

		public IEnumerable<T> Select<T>(string name, ICondition condition, Paging paging, string scope, params Sorting[] sortings)
		{
			return this.Select<T>(name, condition, null, scope, paging, sortings);
		}

		public IEnumerable<T> Select<T>(string name, ICondition condition, Grouping grouping, params Sorting[] sortings)
		{
			return this.Select<T>(name, condition, grouping, string.Empty, null, sortings);
		}

		public IEnumerable<T> Select<T>(string name, ICondition condition, Grouping grouping, string scope, params Sorting[] sortings)
		{
			return this.Select<T>(name, condition, grouping, scope, null, sortings);
		}

		public IEnumerable<T> Select<T>(string name, ICondition condition, Grouping grouping, string scope, Paging paging, params Sorting[] sortings)
		{
			//激发“Selecting”事件
			var args = this.OnSelecting(name, typeof(T), condition, grouping, scope, paging, sortings);

			if(args.Cancel)
				return args.Result as IEnumerable<T>;

			//执行数据查询操作
			args.Result = this.OnSelect<T>(name, condition, grouping, scope, paging, sortings);

			//激发“Selected”事件
			return this.OnSelected(name, typeof(T), args.Condition, args.Grouping, args.Scope, args.Paging, args.Sortings, (IEnumerable<T>)args.Result);
		}

		public IEnumerable<T> Select<T>(string name, ICondition condition, Grouping grouping, Paging paging, params Sorting[] sortings)
		{
			return this.Select<T>(name, condition, grouping, string.Empty, paging, sortings);
		}

		public IEnumerable<T> Select<T>(string name, ICondition condition, Grouping grouping, Paging paging, string scope, params Sorting[] sortings)
		{
			return this.Select<T>(name, condition, grouping, scope, paging, sortings);
		}

		protected abstract IEnumerable<T> OnSelect<T>(string name,
													  ICondition condition,
													  Grouping grouping,
													  string scope,
													  Paging paging,
													  Sorting[] sortings);
		#endregion

		#region 删除方法
		public int Delete(string name, ICondition condition, params string[] cascades)
		{
			if(cascades != null && cascades.Length == 1)
				cascades = cascades[0].Split(',', ';');

			//激发“Deleting”事件
			var args = this.OnDeleting(name, condition, cascades);

			if(args.Cancel)
				return args.Result;

			//执行数据删除操作
			args.Result = this.OnDelete(name, condition, cascades);

			//激发“Deleted”事件
			return this.OnDeleted(name, args.Condition, args.Cascades, args.Result);
		}

		protected abstract int OnDelete(string name, ICondition condition, string[] cascades);
		#endregion

		#region 插入方法
		public int Insert(string name, object data, string scope = null)
		{
			if(data == null)
				return 0;

			//激发“Inserting”事件
			var args = this.OnInserting(name, data, scope);

			if(args.Cancel)
				return args.Result;

			//执行数据插入操作
			args.Result = this.OnInsert(name, data, scope);

			//激发“Inserted”事件
			return this.OnInserted(name, args.Data, args.Scope, args.Result);
		}

		protected virtual int OnInsert(string name, object data, string scope)
		{
			return this.OnInsertMany(name, new object[] { data }, scope);
		}

		public int InsertMany(string name, IEnumerable data, string scope = null)
		{
			if(data == null)
				return 0;

			//激发“Inserting”事件
			var args = this.OnInserting(name, data, scope);

			if(args.Cancel)
				return args.Result;

			//执行数据插入操作
			args.Result = this.OnInsertMany(name, data, scope);

			//激发“Inserted”事件
			return this.OnInserted(name, args.Data, args.Scope, args.Result);
		}

		protected abstract int OnInsertMany(string name, IEnumerable data, string scope);
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
				return 0;

			//激发“Updating”事件
			var args = this.OnUpdating(name, data, condition, scope);

			if(args.Cancel)
				return args.Result;

			//执行数据更新操作
			args.Result = this.OnUpdate(name, data, condition, scope);

			//激发“Updated”事件
			return this.OnUpdated(name, args.Data, args.Condition, args.Scope, args.Result);
		}

		public int Update(string name, object data, string scope, ICondition condition = null)
		{
			return this.Update(name, data, condition, scope);
		}

		protected virtual int OnUpdate(string name, object data, ICondition condition, string scope)
		{
			if(data == null)
				throw new ArgumentNullException("data");

			return this.OnUpdateMany(name, new object[] { data }, condition, scope);
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
		public int UpdateMany(string name, IEnumerable data, ICondition condition = null, string scope = null)
		{
			if(data == null)
				return 0;

			//激发“Updating”事件
			var args = this.OnUpdating(name, data, condition, scope);

			if(args.Cancel)
				return args.Result;

			//执行数据更新操作
			args.Result = this.OnUpdateMany(name, data, condition, scope);

			//激发“Updated”事件
			return this.OnUpdated(name, args.Data, args.Condition, args.Scope, args.Result);
		}

		public int UpdateMany(string name, IEnumerable data, string scope, ICondition condition = null)
		{
			return this.UpdateMany(name, data, condition, scope);
		}

		protected abstract int OnUpdateMany(string name, IEnumerable data, ICondition condition, string scope);
		#endregion

		#region 递增方法
		public abstract long Increment(string name, string member, ICondition condition, int interval = 1);

		public long Decrement(string name, string member, ICondition condition, int interval = 1)
		{
			return this.Increment(name, member, condition, -interval);
		}
		#endregion

		#region 激发事件
		protected int OnCounted(string name, ICondition condition, string includes, int result)
		{
			var args = new DataCountedEventArgs(name, condition, includes, result);
			this.OnCounted(args);
			return args.Result;
		}

		protected DataCountingEventArgs OnCounting(string name, ICondition condition, string includes)
		{
			var args = new DataCountingEventArgs(name, condition, includes);
			this.OnCounting(args);
			return args;
		}

		protected object OnExecuted(string name, Type resultType, IDictionary<string, object> inParameters, ref IDictionary<string, object> outParameters, object result)
		{
			var args = new DataExecutedEventArgs(name, resultType, inParameters, null, result);
			this.OnExecuted(args);
			outParameters = args.OutParameters;
			return args.Result;
		}

		protected DataExecutingEventArgs OnExecuting(string name, Type resultType, IDictionary<string, object> inParameters, out IDictionary<string, object> outParameters)
		{
			var args = new DataExecutingEventArgs(name, resultType, inParameters);
			this.OnExecuting(args);
			outParameters = args.OutParameters;
			return args;
		}

		protected IEnumerable<TEntity> OnSelected<TEntity>(string name, Type entityType, ICondition condition, Grouping grouping, string scope, Paging paging, Sorting[] sortings, IEnumerable<TEntity> result)
		{
			var args = new DataSelectedEventArgs(name, entityType, condition, grouping, scope, paging, sortings, result);
			this.OnSelected(args);
			return args.Result as IEnumerable<TEntity>;
		}

		protected DataSelectingEventArgs OnSelecting(string name, Type entityType, ICondition condition, Grouping grouping, string scope, Paging paging, Sorting[] sortings)
		{
			var args = new DataSelectingEventArgs(name, entityType, condition, grouping, scope, paging, sortings);
			this.OnSelecting(args);
			return args;
		}

		protected int OnDeleted(string name, ICondition condition, string[] cascades, int result)
		{
			var args = new DataDeletedEventArgs(name, condition, cascades, result);
			this.OnDeleted(args);
			return args.Result;
		}

		protected DataDeletingEventArgs OnDeleting(string name, ICondition condition, string[] cascades)
		{
			var args = new DataDeletingEventArgs(name, condition, cascades);
			this.OnDeleting(args);
			return args;
		}

		protected int OnInserted(string name, object data, string scope, int result)
		{
			var args = new DataInsertedEventArgs(name, data, scope, result);
			this.OnInserted(args);
			return args.Result;
		}

		protected DataInsertingEventArgs OnInserting(string name, object data, string scope)
		{
			var args = new DataInsertingEventArgs(name, data, scope);
			this.OnInserting(args);
			return args;
		}

		protected int OnUpdated(string name, object data, ICondition condition, string scope, int result)
		{
			var args = new DataUpdatedEventArgs(name, data, condition, scope, result);
			this.OnUpdated(args);
			return args.Result;
		}

		protected DataUpdatingEventArgs OnUpdating(string name, object data, ICondition condition, string scope)
		{
			var args = new DataUpdatingEventArgs(name, data, condition, scope);
			this.OnUpdating(args);
			return args;
		}

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
	}
}

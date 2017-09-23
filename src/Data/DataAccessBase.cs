/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2010-2017 Zongsoft Corporation <http://www.zongsoft.com>
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
using System.Collections;
using System.Collections.Generic;

namespace Zongsoft.Data
{
	/// <summary>
	/// 表示数据访问的抽象基类。
	/// </summary>
	public abstract class DataAccessBase : IDataAccess
	{
		#region 事件定义
		public event EventHandler<DataCountedEventArgs> Counted;
		public event EventHandler<DataCountingEventArgs> Counting;
		public event EventHandler<DataExecutedEventArgs> Executed;
		public event EventHandler<DataExecutingEventArgs> Executing;
		public event EventHandler<DataExistedEventArgs> Existed;
		public event EventHandler<DataExistingEventArgs> Existing;
		public event EventHandler<DataIncrementedEventArgs> Incremented;
		public event EventHandler<DataIncrementingEventArgs> Incrementing;
		public event EventHandler<DataDecrementedEventArgs> Decremented;
		public event EventHandler<DataDecrementingEventArgs> Decrementing;
		public event EventHandler<DataSelectedEventArgs> Selected;
		public event EventHandler<DataSelectingEventArgs> Selecting;
		public event EventHandler<DataDeletedEventArgs> Deleted;
		public event EventHandler<DataDeletingEventArgs> Deleting;
		public event EventHandler<DataInsertedEventArgs> Inserted;
		public event EventHandler<DataInsertingEventArgs> Inserting;
		public event EventHandler<DataManyInsertedEventArgs> ManyInserted;
		public event EventHandler<DataManyInsertingEventArgs> ManyInserting;
		public event EventHandler<DataUpdatedEventArgs> Updated;
		public event EventHandler<DataUpdatingEventArgs> Updating;
		public event EventHandler<DataManyUpdatedEventArgs> ManyUpdated;
		public event EventHandler<DataManyUpdatingEventArgs> ManyUpdating;
		#endregion

		#region 成员字段
		private IDataAccessNaming _naming;
		private ICollection<IDataAccessFilter> _filters;
		#endregion

		#region 构造函数
		protected DataAccessBase()
		{
			_naming = new DataAccessNaming();
			_filters = new List<IDataAccessFilter>();
		}

		protected DataAccessBase(IDataAccessNaming naming)
		{
			if(naming == null)
				throw new ArgumentNullException(nameof(naming));

			_naming = naming;
			_filters = new List<IDataAccessFilter>();
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取数据访问名称映射器。
		/// </summary>
		public IDataAccessNaming Naming
		{
			get
			{
				return _naming;
			}
		}

		/// <summary>
		/// 获取数据访问过滤器集合。
		/// </summary>
		public ICollection<IDataAccessFilter> Filters
		{
			get
			{
				return _filters;
			}
		}
		#endregion

		#region 获取主键
		public string[] GetKey<T>()
		{
			return this.GetKey(this.GetName<T>());
		}

		public abstract string[] GetKey(string name);
		#endregion

		#region 执行方法
		public IEnumerable<T> Execute<T>(string name, IDictionary<string, object> inParameters)
		{
			IDictionary<string, object> outParameters;
			return this.Execute<T>(name, inParameters, out outParameters);
		}

		public IEnumerable<T> Execute<T>(string name, IDictionary<string, object> inParameters, out IDictionary<string, object> outParameters)
		{
			if(string.IsNullOrWhiteSpace(name))
				throw new ArgumentNullException(nameof(name));

			//激发“Executing”事件
			var args = this.OnExecuting(name, typeof(T), inParameters, out outParameters);

			if(args.Cancel)
				return args.Result as IEnumerable<T>;

			//定义数据访问过滤器上下文
			var filterContext = new DataAccessFilterContext(this, DataAccessMethod.Execute, args);

			//调用数据访问过滤器前事件
			this.InvokeFilters(filterContext, (filter, ctx) => filter.OnExecuting(ctx));

			//执行数据操作方法
			args.Result = this.OnExecute<T>(name, inParameters, out outParameters);

			//调用数据访问过滤器后事件
			this.InvokeFilters(filterContext, (filter, ctx) => filter.OnExecuted(ctx));

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
			if(string.IsNullOrWhiteSpace(name))
				throw new ArgumentNullException(nameof(name));

			//激发“Executing”事件
			var args = this.OnExecuting(name, typeof(object), inParameters, out outParameters);

			if(args.Cancel)
				return args.Result;

			//定义数据访问过滤器上下文
			var filterContext = new DataAccessFilterContext(this, DataAccessMethod.ExecuteScalar, args);

			//调用数据访问过滤器前事件
			this.InvokeFilters(filterContext, (filter, ctx) => filter.OnExecuting(ctx));

			//执行数据操作方法
			args.Result = this.OnExecuteScalar(name, inParameters, out outParameters);

			//调用数据访问过滤器后事件
			this.InvokeFilters(filterContext, (filter, ctx) => filter.OnExecuted(ctx));

			//激发“Executed”事件
			return this.OnExecuted(name, typeof(object), args.InParameters, ref outParameters, args.Result);
		}

		protected abstract object OnExecuteScalar(string name, IDictionary<string, object> inParameters, out IDictionary<string, object> outParameters);
		#endregion

		#region 存在方法
		public bool Exists<T>(ICondition condition)
		{
			return this.Exists(this.GetName<T>(), condition);
		}

		public bool Exists(string name, ICondition condition)
		{
			if(string.IsNullOrWhiteSpace(name))
				throw new ArgumentNullException(nameof(name));

			//激发“Existing”事件
			var args = this.OnExisting(name, condition, false);

			if(args.Cancel)
				return args.Result;

			//定义数据访问过滤器上下文
			var filterContext = new DataAccessFilterContext(this, DataAccessMethod.Exists, args);

			//调用数据访问过滤器前事件
			this.InvokeFilters(filterContext, (filter, ctx) => filter.OnExecuting(ctx));

			//执行存在操作方法
			args.Result = this.OnExists(name, condition);

			//调用数据访问过滤器后事件
			this.InvokeFilters(filterContext, (filter, ctx) => filter.OnExecuted(ctx));

			//激发“Existed”事件
			return this.OnExisted(name, args.Condition, args.Result);
		}

		protected abstract bool OnExists(string name, ICondition condition);
		#endregion

		#region 计数方法
		public int Count<T>(ICondition condition, string includes = null)
		{
			return this.Count(this.GetName<T>(), condition, includes);
		}

		public int Count(string name, ICondition condition, string includes = null)
		{
			if(string.IsNullOrWhiteSpace(name))
				throw new ArgumentNullException(nameof(name));

			//激发“Counting”事件
			var args = this.OnCounting(name, condition, includes);

			if(args.Cancel)
				return args.Result;

			//定义数据访问过滤器上下文
			var filterContext = new DataAccessFilterContext(this, DataAccessMethod.Count, args);

			//调用数据访问过滤器前事件
			this.InvokeFilters(filterContext, (filter, ctx) => filter.OnExecuting(ctx));

			//执行计数操作方法
			args.Result = this.OnCount(name, condition, string.IsNullOrWhiteSpace(includes) ? (string[])null : includes.Split(',', ';'));

			//调用数据访问过滤器后事件
			this.InvokeFilters(filterContext, (filter, ctx) => filter.OnExecuted(ctx));

			//激发“Counted”事件
			return this.OnCounted(name, args.Condition, args.Includes, args.Result);
		}

		protected abstract int OnCount(string name, ICondition condition, string[] includes);
		#endregion

		#region 查询方法
		public IEnumerable<T> Select<T>(ICondition condition = null, params Sorting[] sortings)
		{
			return this.Select<T>(this.GetName<T>(), condition, null, string.Empty, null, sortings);
		}

		public IEnumerable<T> Select<T>(ICondition condition, string scope, params Sorting[] sortings)
		{
			return this.Select<T>(this.GetName<T>(), condition, null, scope, null, sortings);
		}

		public IEnumerable<T> Select<T>(ICondition condition, string scope, Paging paging, params Sorting[] sortings)
		{
			return this.Select<T>(this.GetName<T>(), condition, null, scope, paging, sortings);
		}

		public IEnumerable<T> Select<T>(ICondition condition, Paging paging, params Sorting[] sortings)
		{
			return this.Select<T>(this.GetName<T>(), condition, null, string.Empty, paging, sortings);
		}

		public IEnumerable<T> Select<T>(ICondition condition, Paging paging, string scope, params Sorting[] sortings)
		{
			return this.Select<T>(this.GetName<T>(), condition, null, scope, paging, sortings);
		}

		public IEnumerable<T> Select<T>(ICondition condition, Grouping grouping, params Sorting[] sortings)
		{
			return this.Select<T>(this.GetName<T>(), condition, grouping, string.Empty, null, sortings);
		}

		public IEnumerable<T> Select<T>(ICondition condition, Grouping grouping, string scope, params Sorting[] sortings)
		{
			return this.Select<T>(this.GetName<T>(), condition, grouping, scope, null, sortings);
		}

		public IEnumerable<T> Select<T>(ICondition condition, Grouping grouping, string scope, Paging paging, params Sorting[] sortings)
		{
			return this.Select<T>(this.GetName<T>(), condition, grouping, scope, paging, sortings);
		}

		public IEnumerable<T> Select<T>(ICondition condition, Grouping grouping, Paging paging, params Sorting[] sortings)
		{
			return this.Select<T>(this.GetName<T>(), condition, grouping, string.Empty, paging, sortings);
		}

		public IEnumerable<T> Select<T>(ICondition condition, Grouping grouping, Paging paging, string scope, params Sorting[] sortings)
		{
			return this.Select<T>(this.GetName<T>(), condition, grouping, scope, paging, sortings);
		}

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
			if(string.IsNullOrWhiteSpace(name))
				throw new ArgumentNullException(nameof(name));

			//激发“Selecting”事件
			var args = this.OnSelecting(name, typeof(T), condition, grouping, scope, paging, sortings);

			if(args.Cancel)
				return (args.Result as IEnumerable<T>) ?? Enumerable.Empty<T>();

			//定义数据访问过滤器上下文
			var filterContext = new DataAccessFilterContext(this, DataAccessMethod.Select, args);

			//调用数据访问过滤器前事件
			this.InvokeFilters(filterContext, (filter, ctx) => filter.OnExecuting(ctx));

			//执行数据查询操作
			args.Result = this.OnSelect<T>(name, condition, grouping, scope, paging, sortings);

			//调用数据访问过滤器后事件
			this.InvokeFilters(filterContext, (filter, ctx) => filter.OnExecuted(ctx));

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
		public int Delete<T>(ICondition condition, params string[] cascades)
		{
			return this.Delete(this.GetName<T>(), condition, cascades);
		}

		public int Delete(string name, ICondition condition, params string[] cascades)
		{
			if(string.IsNullOrWhiteSpace(name))
				throw new ArgumentNullException(nameof(name));

			if(cascades != null && cascades.Length == 1)
				cascades = cascades[0].Split(',', ';');

			//激发“Deleting”事件
			var args = this.OnDeleting(name, condition, cascades);

			if(args.Cancel)
				return args.Count;

			//定义数据访问过滤器上下文
			var filterContext = new DataAccessFilterContext(this, DataAccessMethod.Delete, args);

			//调用数据访问过滤器前事件
			this.InvokeFilters(filterContext, (filter, ctx) => filter.OnExecuting(ctx));

			//执行数据删除操作
			args.Count = this.OnDelete(name, condition, cascades);

			//调用数据访问过滤器后事件
			this.InvokeFilters(filterContext, (filter, ctx) => filter.OnExecuted(ctx));

			//激发“Deleted”事件
			return this.OnDeleted(name, args.Condition, args.Cascades, args.Count);
		}

		protected abstract int OnDelete(string name, ICondition condition, string[] cascades);
		#endregion

		#region 插入方法
		public int Insert<T>(T data, string scope = null)
		{
			if(data == null)
				return 0;

			return this.Insert(this.GetName(data.GetType()), data, scope);
		}

		public int Insert(string name, object data, string scope = null)
		{
			if(string.IsNullOrWhiteSpace(name))
				throw new ArgumentNullException(nameof(name));

			if(data == null)
				return 0;

			//激发“Inserting”事件
			var args = this.OnInserting(name, data, scope);

			if(args.Cancel)
				return args.Count;

			//定义数据访问过滤器上下文
			var filterContext = new DataAccessFilterContext(this, DataAccessMethod.Insert, args);

			//调用数据访问过滤器前事件
			this.InvokeFilters(filterContext, (filter, ctx) => filter.OnExecuting(ctx));

			//执行数据插入操作
			args.Count = this.OnInsert(name, args.Data, scope);

			//调用数据访问过滤器后事件
			this.InvokeFilters(filterContext, (filter, ctx) => filter.OnExecuted(ctx));

			//激发“Inserted”事件
			return this.OnInserted(name, args.Data, args.Scope, args.Count);
		}

		protected virtual int OnInsert(string name, DataDictionary data, string scope)
		{
			return this.OnInsertMany(name, new[] { data }, scope);
		}

		public int InsertMany<T>(IEnumerable<T> items, string scope = null)
		{
			return this.InsertMany(this.GetName<T>(), items, scope);
		}

		public int InsertMany(string name, IEnumerable items, string scope = null)
		{
			if(string.IsNullOrWhiteSpace(name))
				throw new ArgumentNullException(nameof(name));

			if(items == null)
				return 0;

			//激发“ManyInserting”事件
			var args = this.OnManyInserting(name, items, scope);

			if(args.Cancel)
				return args.Count;

			//定义数据访问过滤器上下文
			var filterContext = new DataAccessFilterContext(this, DataAccessMethod.InsertMany, args);

			//调用数据访问过滤器前事件
			this.InvokeFilters(filterContext, (filter, ctx) => filter.OnExecuting(ctx));

			//执行数据插入操作
			args.Count = this.OnInsertMany(name, args.Data, scope);

			//调用数据访问过滤器后事件
			this.InvokeFilters(filterContext, (filter, ctx) => filter.OnExecuted(ctx));

			//激发“ManyInserted”事件
			return this.OnManyInserted(name, args.Data, args.Scope, args.Count);
		}

		protected abstract int OnInsertMany(string name, IEnumerable<DataDictionary> items, string scope);
		#endregion

		#region 更新方法
		public int Update<T>(T data, ICondition condition = null, string scope = null)
		{
			if(data == null)
				return 0;

			return this.Update(this.GetName(data.GetType()), data, condition, scope);
		}

		public int Update<T>(T data, string scope, ICondition condition = null)
		{
			if(data == null)
				return 0;

			return this.Update(this.GetName(data.GetType()), data, condition, scope);
		}

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
			if(string.IsNullOrWhiteSpace(name))
				throw new ArgumentNullException(nameof(name));

			if(data == null)
				return 0;

			//激发“Updating”事件
			var args = this.OnUpdating(name, data, condition, scope);

			if(args.Cancel)
				return args.Count;

			//定义数据访问过滤器上下文
			var filterContext = new DataAccessFilterContext(this, DataAccessMethod.Update, args);

			//调用数据访问过滤器前事件
			this.InvokeFilters(filterContext, (filter, ctx) => filter.OnExecuting(ctx));

			//执行数据更新操作
			args.Count = this.OnUpdate(name, args.Data, condition, scope);

			//调用数据访问过滤器后事件
			this.InvokeFilters(filterContext, (filter, ctx) => filter.OnExecuted(ctx));

			//激发“Updated”事件
			return this.OnUpdated(name, args.Data, args.Condition, args.Scope, args.Count);
		}

		public int Update(string name, object data, string scope, ICondition condition = null)
		{
			return this.Update(name, data, condition, scope);
		}

		protected virtual int OnUpdate(string name, DataDictionary data, ICondition condition, string scope)
		{
			if(data == null)
				return 0;

			return this.OnUpdateMany(name, new[] { data }, condition, scope);
		}

		public int UpdateMany<T>(IEnumerable<T> items, ICondition condition = null, string scope = null)
		{
			return this.UpdateMany(this.GetName<T>(), items, condition, scope);
		}

		public int UpdateMany<T>(IEnumerable<T> items, string scope, ICondition condition = null)
		{
			return this.UpdateMany(this.GetName<T>(), items, condition, scope);
		}

		/// <summary>
		/// 根据指定的条件将指定的实体集更新到数据源。
		/// </summary>
		/// <param name="name">指定的实体映射名。</param>
		/// <param name="items">要更新的数据集。</param>
		/// <param name="condition">要更新的条件子句，如果为空(null)则根据实体的主键进行更新。</param>
		/// <param name="scope">指定的要更新的和排除更新的属性名列表，如果指定的是多个属性则属性名之间使用逗号(,)分隔；要排除的属性以减号(-)打头，星号(*)表示所有属性，感叹号(!)表示排除所有属性；如果未指定该参数则默认只会更新所有单值属性而不会更新导航属性。</param>
		/// <returns>返回受影响的记录行数，执行成功返回大于零的整数，失败则返回负数。</returns>
		public int UpdateMany(string name, IEnumerable items, ICondition condition = null, string scope = null)
		{
			if(string.IsNullOrWhiteSpace(name))
				throw new ArgumentNullException(nameof(name));

			if(items == null)
				return 0;

			//激发“ManyUpdating”事件
			var args = this.OnManyUpdating(name, items, condition, scope);

			if(args.Cancel)
				return args.Count;

			//定义数据访问过滤器上下文
			var filterContext = new DataAccessFilterContext(this, DataAccessMethod.UpdateMany, args);

			//调用数据访问过滤器前事件
			this.InvokeFilters(filterContext, (filter, ctx) => filter.OnExecuting(ctx));

			//执行数据更新操作
			args.Count = this.OnUpdateMany(name, args.Data, condition, scope);

			//调用数据访问过滤器后事件
			this.InvokeFilters(filterContext, (filter, ctx) => filter.OnExecuted(ctx));

			//激发“ManyUpdated”事件
			return this.OnManyUpdated(name, args.Data, args.Condition, args.Scope, args.Count);
		}

		public int UpdateMany(string name, IEnumerable items, string scope, ICondition condition = null)
		{
			return this.UpdateMany(name, items, condition, scope);
		}

		protected abstract int OnUpdateMany(string name, IEnumerable<DataDictionary> items, ICondition condition, string scope);
		#endregion

		#region 递增方法
		public long Increment<T>(string member, ICondition condition, int interval = 1)
		{
			return this.Increment(this.GetName<T>(), member, condition, interval);
		}

		public long Increment(string name, string member, ICondition condition, int interval = 1)
		{
			if(string.IsNullOrWhiteSpace(name))
				throw new ArgumentNullException(nameof(name));
			if(string.IsNullOrWhiteSpace(member))
				throw new ArgumentNullException(nameof(member));

			//激发“Incrementing”事件
			var args = this.OnIncrementing(name, member, condition, interval);

			if(args.Cancel)
				return args.Result;

			//定义数据访问过滤器上下文
			var filterContext = new DataAccessFilterContext(this, DataAccessMethod.Increment, args);

			//调用数据访问过滤器前事件
			this.InvokeFilters(filterContext, (filter, ctx) => filter.OnExecuting(ctx));

			//执行递增操作方法
			args.Result = this.OnIncrement(name, member, condition, interval);

			//调用数据访问过滤器后事件
			this.InvokeFilters(filterContext, (filter, ctx) => filter.OnExecuted(ctx));

			//激发“Incremented”事件
			return this.OnIncremented(name, args.Member, args.Condition, args.Interval, args.Result);
		}

		protected abstract long OnIncrement(string name, string member, ICondition condition, int interval);

		public long Decrement<T>(string member, ICondition condition, int interval = 1)
		{
			return this.Decrement(this.GetName<T>(), member, condition, interval);
		}

		public long Decrement(string name, string member, ICondition condition, int interval = 1)
		{
			if(string.IsNullOrWhiteSpace(name))
				throw new ArgumentNullException(nameof(name));
			if(string.IsNullOrWhiteSpace(member))
				throw new ArgumentNullException(nameof(member));

			//激发“Decrementing”事件
			var args = this.OnDecrementing(name, member, condition, interval);

			if(args.Cancel)
				return args.Result;

			//定义数据访问过滤器上下文
			var filterContext = new DataAccessFilterContext(this, DataAccessMethod.Decrement, args);

			//调用数据访问过滤器前事件
			this.InvokeFilters(filterContext, (filter, ctx) => filter.OnExecuting(ctx));

			//执行递减操作方法
			args.Result = this.OnDecrement(name, member, condition, interval);

			//调用数据访问过滤器后事件
			this.InvokeFilters(filterContext, (filter, ctx) => filter.OnExecuted(ctx));

			//激发“Decremented”事件
			return this.OnIncremented(name, args.Member, args.Condition, args.Interval, args.Result);
		}

		protected virtual long OnDecrement(string name, string member, ICondition condition, int interval)
		{
			return this.OnIncrement(name, member, condition, -interval);
		}
		#endregion

		#region 虚拟方法
		protected virtual string GetName(Type type)
		{
			var name = _naming.Get(type);

			if(string.IsNullOrEmpty(name))
				throw new InvalidOperationException($"Missing data access name mapping of the '{type.FullName}' type.");

			return name;
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
			var args = new DataExecutedEventArgs(name, resultType, inParameters, outParameters, result);
			this.OnExecuted(args);
			return args.Result;
		}

		protected DataExecutingEventArgs OnExecuting(string name, Type resultType, IDictionary<string, object> inParameters, out IDictionary<string, object> outParameters)
		{
			var args = new DataExecutingEventArgs(name, resultType, inParameters);
			this.OnExecuting(args);
			outParameters = args.OutParameters;
			return args;
		}

		protected bool OnExisted(string name, ICondition condition, bool result)
		{
			var args = new DataExistedEventArgs(name, condition, result);
			this.OnExisted(args);
			return args.Result;
		}

		protected DataExistingEventArgs OnExisting(string name, ICondition condition, bool cancel)
		{
			var args = new DataExistingEventArgs(name, condition, cancel);
			this.OnExisting(args);
			return args;
		}

		protected long OnIncremented(string name, string member, ICondition condition, int interval, long result)
		{
			var args = new DataIncrementedEventArgs(name, member, condition, interval, result);
			this.OnIncremented(args);
			return args.Result;
		}

		protected DataIncrementingEventArgs OnIncrementing(string name, string member, ICondition condition, int interval = 1, bool cancel = false)
		{
			var args = new DataIncrementingEventArgs(name, member, condition, interval, cancel);
			this.OnIncrementing(args);
			return args;
		}

		protected long OnDecremented(string name, string member, ICondition condition, int interval, long result)
		{
			var args = new DataDecrementedEventArgs(name, member, condition, interval, result);
			this.OnDecremented(args);
			return args.Result;
		}

		protected DataDecrementingEventArgs OnDecrementing(string name, string member, ICondition condition, int interval = 1, bool cancel = false)
		{
			var args = new DataDecrementingEventArgs(name, member, condition, interval, cancel);
			this.OnDecrementing(args);
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
			return args.Count;
		}

		protected DataDeletingEventArgs OnDeleting(string name, ICondition condition, string[] cascades)
		{
			var args = new DataDeletingEventArgs(name, condition, cascades);
			this.OnDeleting(args);
			return args;
		}

		protected int OnInserted(string name, DataDictionary data, string scope, int result)
		{
			var args = new DataInsertedEventArgs(name, data, scope, result);
			this.OnInserted(args);
			return args.Count;
		}

		protected DataInsertingEventArgs OnInserting(string name, object data, string scope)
		{
			var args = new DataInsertingEventArgs(name, GetDataDictionary(data), scope);
			this.OnInserting(args);
			return args;
		}

		protected int OnManyInserted(string name, IEnumerable<DataDictionary> data, string scope, int result)
		{
			var args = new DataManyInsertedEventArgs(name, data, scope, result);
			this.OnManyInserted(args);
			return args.Count;
		}

		protected DataManyInsertingEventArgs OnManyInserting(string name, object data, string scope)
		{
			var args = new DataManyInsertingEventArgs(name, GetDataDictionaries(data), scope);
			this.OnManyInserting(args);
			return args;
		}

		protected int OnUpdated(string name, DataDictionary data, ICondition condition, string scope, int result)
		{
			var args = new DataUpdatedEventArgs(name, data, condition, scope, result);
			this.OnUpdated(args);
			return args.Count;
		}

		protected DataUpdatingEventArgs OnUpdating(string name, object data, ICondition condition, string scope)
		{
			var args = new DataUpdatingEventArgs(name, GetDataDictionary(data), condition, scope);
			this.OnUpdating(args);
			return args;
		}

		protected int OnManyUpdated(string name, IEnumerable<DataDictionary> data, ICondition condition, string scope, int result)
		{
			var args = new DataManyUpdatedEventArgs(name, data, condition, scope, result);
			this.OnManyUpdated(args);
			return args.Count;
		}

		protected DataManyUpdatingEventArgs OnManyUpdating(string name, object data, ICondition condition, string scope)
		{
			var args = new DataManyUpdatingEventArgs(name, GetDataDictionaries(data), condition, scope);
			this.OnManyUpdating(args);
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

		protected virtual void OnExisted(DataExistedEventArgs args)
		{
			var e = this.Existed;

			if(e != null)
				e(this, args);
		}

		protected virtual void OnExisting(DataExistingEventArgs args)
		{
			var e = this.Existing;

			if(e != null)
				e(this, args);
		}

		protected virtual void OnIncremented(DataIncrementedEventArgs args)
		{
			var e = this.Incremented;

			if(e != null)
				e(this, args);
		}

		protected virtual void OnIncrementing(DataIncrementingEventArgs args)
		{
			var e = this.Incrementing;

			if(e != null)
				e(this, args);
		}

		protected virtual void OnDecremented(DataDecrementedEventArgs args)
		{
			var e = this.Decremented;

			if(e != null)
				e(this, args);
		}

		protected virtual void OnDecrementing(DataDecrementingEventArgs args)
		{
			var e = this.Decrementing;

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

		protected virtual void OnManyInserted(DataManyInsertedEventArgs args)
		{
			var e = this.ManyInserted;

			if(e != null)
				e(this, args);
		}

		protected virtual void OnManyInserting(DataManyInsertingEventArgs args)
		{
			var e = this.ManyInserting;

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

		protected virtual void OnManyUpdated(DataManyUpdatedEventArgs args)
		{
			var e = this.ManyUpdated;

			if(e != null)
				e(this, args);
		}

		protected virtual void OnManyUpdating(DataManyUpdatingEventArgs args)
		{
			var e = this.ManyUpdating;

			if(e != null)
				e(this, args);
		}
		#endregion

		#region 私有方法
		private string GetName<T>()
		{
			return this.GetName(typeof(T));
		}

		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.Synchronized)]
		private void InvokeFilters(DataAccessFilterContext context, Action<IDataAccessFilter, DataAccessFilterContext> invoke)
		{
			foreach(var filter in _filters)
			{
				if(filter == null)
					continue;

				var predication = filter as Zongsoft.Services.IPredication;

				if(predication == null || predication.Predicate(context))
				{
					invoke(filter, context);
				}
			}
		}

		private static DataDictionary GetDataDictionary(object data)
		{
			if(data == null)
				throw new ArgumentNullException("data");

			return (data as DataDictionary) ?? new DataDictionary(data);
		}

		private static IEnumerable<DataDictionary> GetDataDictionaries(object data)
		{
			if(data == null)
				throw new ArgumentNullException("data");

			var items = data as IEnumerable;

			if(items == null)
				yield return (data as DataDictionary) ?? new DataDictionary(data);
			else
			{
				foreach(var item in items)
				{
					if(item != null)
						yield return (item as DataDictionary) ?? new DataDictionary(item);
				}
			}
		}
		#endregion
	}
}

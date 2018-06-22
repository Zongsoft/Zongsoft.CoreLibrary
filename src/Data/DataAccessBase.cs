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
		public event EventHandler<DataDeletedEventArgs> Deleted;
		public event EventHandler<DataDeletingEventArgs> Deleting;
		public event EventHandler<DataInsertedEventArgs> Inserted;
		public event EventHandler<DataInsertingEventArgs> Inserting;
		public event EventHandler<DataUpdatedEventArgs> Updated;
		public event EventHandler<DataUpdatingEventArgs> Updating;
		public event EventHandler<DataSelectedEventArgs> Selected;
		public event EventHandler<DataSelectingEventArgs> Selecting;
		#endregion

		#region 成员字段
		private string _name;
		private IDataAccessNaming _naming;
		private ICollection<IDataAccessFilter> _filters;
		#endregion

		#region 构造函数
		protected DataAccessBase(string name)
		{
			_name = name ?? string.Empty;
			_naming = new DataAccessNaming(name);
			_filters = new List<IDataAccessFilter>();
		}

		protected DataAccessBase(string name, IDataAccessNaming naming)
		{
			_name = name ?? string.Empty;
			_naming = naming ?? throw new ArgumentNullException(nameof(naming));
			_filters = new List<IDataAccessFilter>();
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取数据访问的应用名。
		/// </summary>
		public string Name
		{
			get
			{
				return _name;
			}
			set
			{
				_name = value ?? string.Empty;
			}
		}

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
		public IEnumerable<T> Execute<T>(string name, IDictionary<string, object> inParameters, object state = null, Func<DataExecuteContext, bool> executing = null, Action<DataExecuteContext> executed = null)
		{
			IDictionary<string, object> outParameters;
			return this.Execute<T>(name, inParameters, out outParameters, state, executing, executed);
		}

		public IEnumerable<T> Execute<T>(string name, IDictionary<string, object> inParameters, out IDictionary<string, object> outParameters, object state = null, Func<DataExecuteContext, bool> executing = null, Action<DataExecuteContext> executed = null)
		{
			if(string.IsNullOrEmpty(name))
				throw new ArgumentNullException(nameof(name));

			//创建数据访问上下文对象
			var context = this.CreateExecuteContext(name, false, typeof(T), inParameters, state);

			//处理数据访问操作前的回调
			if(executing != null && executing(context))
			{
				//设置默认的返回参数值
				outParameters = context.OutParameters;

				//返回委托回调的结果
				return context.Result as IEnumerable<T>;
			}

			//激发“Executing”事件，如果被中断则返回
			if(this.OnExecuting(context))
			{
				//设置默认的返回参数值
				outParameters = context.OutParameters;

				//返回事件执行后的结果
				return context.Result as IEnumerable<T>;
			}

			//调用数据访问过滤器前事件
			this.InvokeFilters(context, (filter, ctx) => filter.OnFiltering(ctx));

			//执行数据操作方法
			this.OnExecute(context);

			//调用数据访问过滤器后事件
			this.InvokeFilters(context, (filter, ctx) => filter.OnFiltered(ctx));

			//激发“Executed”事件
			this.OnExecuted(context);

			//处理数据访问操作后的回调
			if(executed != null)
				executed(context);

			//再次更新返回参数值
			outParameters = context.OutParameters;

			//返回最终的结果
			return context.Result as IEnumerable<T>;
		}

		public object ExecuteScalar(string name, IDictionary<string, object> inParameters, object state = null, Func<DataExecuteContext, bool> executing = null, Action<DataExecuteContext> executed = null)
		{
			IDictionary<string, object> outParameters;
			return this.ExecuteScalar(name, inParameters, out outParameters, state, executing, executed);
		}

		public object ExecuteScalar(string name, IDictionary<string, object> inParameters, out IDictionary<string, object> outParameters, object state = null, Func<DataExecuteContext, bool> executing = null, Action<DataExecuteContext> executed = null)
		{
			if(string.IsNullOrEmpty(name))
				throw new ArgumentNullException(nameof(name));

			//创建数据访问上下文对象
			var context = this.CreateExecuteContext(name, true, typeof(object), inParameters, state);

			//处理数据访问操作前的回调
			if(executing != null && executing(context))
			{
				//设置默认的返回参数值
				outParameters = context.OutParameters;

				//返回委托回调的结果
				return context.Result;
			}

			//激发“Executing”事件，如果被中断则返回
			if(this.OnExecuting(context))
			{
				//设置默认的返回参数值
				outParameters = context.OutParameters;

				//返回事件执行后的结果
				return context.Result;
			}

			//调用数据访问过滤器前事件
			this.InvokeFilters(context, (filter, ctx) => filter.OnFiltering(ctx));

			//执行数据操作方法
			this.OnExecute(context);

			//调用数据访问过滤器后事件
			this.InvokeFilters(context, (filter, ctx) => filter.OnFiltered(ctx));

			//激发“Executed”事件
			this.OnExecuted(context);

			//处理数据访问操作后的回调
			if(executed != null)
				executed(context);

			//再次更新返回参数值
			outParameters = context.OutParameters;

			//返回最终的结果
			return context.Result;
		}

		protected abstract void OnExecute(DataExecuteContext context);
		#endregion

		#region 存在方法
		public bool Exists<T>(ICondition condition, object state = null, Func<DataExistContext, bool> existing = null, Action<DataExistContext> existed = null)
		{
			return this.Exists(this.GetName<T>(), condition, state, existing, existed);
		}

		public bool Exists(string name, ICondition condition, object state = null, Func<DataExistContext, bool> existing = null, Action<DataExistContext> existed = null)
		{
			if(string.IsNullOrEmpty(name))
				throw new ArgumentNullException(nameof(name));

			//创建数据访问上下文对象
			var context = this.CreateExistContext(name, condition, state);

			//处理数据访问操作前的回调
			if(existing != null && existing(context))
				return context.Result;

			//激发“Existing”事件，如果被中断则返回
			if(this.OnExisting(context))
				return context.Result;

			//调用数据访问过滤器前事件
			this.InvokeFilters(context, (filter, ctx) => filter.OnFiltering(ctx));

			//执行存在操作方法
			this.OnExists(context);

			//调用数据访问过滤器后事件
			this.InvokeFilters(context, (filter, ctx) => filter.OnFiltered(ctx));

			//激发“Existed”事件
			this.OnExisted(context);

			//处理数据访问操作后的回调
			if(existed != null)
				existed(context);

			//返回最终的结果
			return context.Result;
		}

		protected abstract void OnExists(DataExistContext context);
		#endregion

		#region 计数方法
		public int Count<T>(ICondition condition)
		{
			return this.Count(this.GetName<T>(), condition, string.Empty, null, null, null);
		}

		public int Count<T>(ICondition condition, object state)
		{
			return this.Count(this.GetName<T>(), condition, string.Empty, state, null, null);
		}

		public int Count<T>(ICondition condition, string includes)
		{
			return this.Count(this.GetName<T>(), condition, includes, null, null, null);
		}

		public int Count<T>(ICondition condition, string includes, object state, Func<DataCountContext, bool> counting = null, Action<DataCountContext> counted = null)
		{
			return this.Count(this.GetName<T>(), condition, includes, state, counting, counted);
		}

		public int Count(string name, ICondition condition)
		{
			return this.Count(name, condition, string.Empty, null, null, null);
		}

		public int Count(string name, ICondition condition, object state)
		{
			return this.Count(name, condition, string.Empty, state, null, null);
		}

		public int Count(string name, ICondition condition, string includes)
		{
			return this.Count(name, condition, includes, null, null, null);
		}

		public int Count(string name, ICondition condition, string includes, object state, Func<DataCountContext, bool> counting = null, Action<DataCountContext> counted = null)
		{
			if(string.IsNullOrEmpty(name))
				throw new ArgumentNullException(nameof(name));

			//创建数据访问上下文对象
			var context = this.CreateCountContext(name, condition, includes, state);

			//处理数据访问操作前的回调
			if(counting != null && counting(context))
				return context.Result;

			//激发“Counting”事件，如果被中断则返回
			if(this.OnCounting(context))
				return context.Result;

			//调用数据访问过滤器前事件
			this.InvokeFilters(context, (filter, ctx) => filter.OnFiltering(ctx));

			//执行计数操作方法
			this.OnCount(context);

			//调用数据访问过滤器后事件
			this.InvokeFilters(context, (filter, ctx) => filter.OnFiltered(ctx));

			//激发“Counted”事件
			this.OnCounted(context);

			//处理数据访问操作后的回调
			if(counted != null)
				counted(context);

			//返回最终的结果
			return context.Result;
		}

		protected abstract void OnCount(DataCountContext context);
		#endregion

		#region 递增方法
		public long Increment<T>(string member, ICondition condition)
		{
			return this.Increment(this.GetName<T>(), member, condition, 1, null, null, null);
		}

		public long Increment<T>(string member, ICondition condition, object state)
		{
			return this.Increment(this.GetName<T>(), member, condition, 1, state, null, null);
		}

		public long Increment<T>(string member, ICondition condition, int interval)
		{
			return this.Increment(this.GetName<T>(), member, condition, interval, null, null, null);
		}

		public long Increment<T>(string member, ICondition condition, int interval, object state, Func<DataIncrementContext, bool> incrementing = null, Action<DataIncrementContext> incremented = null)
		{
			return this.Increment(this.GetName<T>(), member, condition, interval, state, incrementing, incremented);
		}

		public long Increment(string name, string member, ICondition condition)
		{
			return this.Increment(name, member, condition, 1, null, null, null);
		}

		public long Increment(string name, string member, ICondition condition, object state)
		{
			return this.Increment(name, member, condition, 1, state, null, null);
		}

		public long Increment(string name, string member, ICondition condition, int interval)
		{
			return this.Increment(name, member, condition, interval, null, null, null);
		}

		public long Increment(string name, string member, ICondition condition, int interval, object state, Func<DataIncrementContext, bool> incrementing = null, Action<DataIncrementContext> incremented = null)
		{
			if(string.IsNullOrEmpty(name))
				throw new ArgumentNullException(nameof(name));

			if(string.IsNullOrEmpty(member))
				throw new ArgumentNullException(nameof(member));

			//创建数据访问上下文对象
			var context = this.CreateIncrementContext(name, member, condition, interval, state);

			//处理数据访问操作前的回调
			if(incrementing != null && incrementing(context))
				return context.Result;

			//激发“Incrementing”事件
			if(this.OnIncrementing(context))
				return context.Result;

			//调用数据访问过滤器前事件
			this.InvokeFilters(context, (filter, ctx) => filter.OnFiltering(ctx));

			//执行递增操作方法
			this.OnIncrement(context);

			//调用数据访问过滤器后事件
			this.InvokeFilters(context, (filter, ctx) => filter.OnFiltered(ctx));

			//激发“Incremented”事件
			this.OnIncremented(context);

			//处理数据访问操作后的回调
			if(incremented != null)
				incremented(context);

			//返回最终的结果
			return context.Result;
		}

		public long Decrement<T>(string member, ICondition condition)
		{
			return this.Decrement(this.GetName<T>(), member, condition, 1, null, null, null);
		}

		public long Decrement<T>(string member, ICondition condition, object state)
		{
			return this.Decrement(this.GetName<T>(), member, condition, 1, state, null, null);
		}

		public long Decrement<T>(string member, ICondition condition, int interval)
		{
			return this.Decrement(this.GetName<T>(), member, condition, interval, null, null, null);
		}

		public long Decrement<T>(string member, ICondition condition, int interval, object state, Func<DataIncrementContext, bool> decrementing = null, Action<DataIncrementContext> decremented = null)
		{
			return this.Increment(this.GetName<T>(), member, condition, -interval, state, decrementing, decremented);
		}

		public long Decrement(string name, string member, ICondition condition)
		{
			return this.Decrement(name, member, condition, 1, null, null, null);
		}

		public long Decrement(string name, string member, ICondition condition, object state)
		{
			return this.Decrement(name, member, condition, 1, state, null, null);
		}

		public long Decrement(string name, string member, ICondition condition, int interval)
		{
			return this.Decrement(name, member, condition, interval, null, null, null);
		}

		public long Decrement(string name, string member, ICondition condition, int interval, object state, Func<DataIncrementContext, bool> decrementing = null, Action<DataIncrementContext> decremented = null)
		{
			return this.Increment(name, member, condition, -interval, state, decrementing, decremented);
		}

		protected abstract void OnIncrement(DataIncrementContext context);
		#endregion

		#region 删除方法
		public int Delete<T>(ICondition condition, params string[] cascades)
		{
			return this.Delete(this.GetName<T>(), condition, null, cascades, null, null);
		}

		public int Delete<T>(ICondition condition, object state, params string[] cascades)
		{
			return this.Delete(this.GetName<T>(), condition, state, cascades, null, null);
		}

		public int Delete<T>(ICondition condition, object state, string[] cascades, Func<DataDeleteContext, bool> deleting = null, Action<DataDeleteContext> deleted = null)
		{
			return this.Delete(this.GetName<T>(), condition, state, cascades, deleting, deleted);
		}

		public int Delete(string name, ICondition condition, params string[] cascades)
		{
			return this.Delete(name, condition, null, cascades, null, null);
		}

		public int Delete(string name, ICondition condition, object state, params string[] cascades)
		{
			return this.Delete(name, condition, state, cascades, null, null);
		}

		public int Delete(string name, ICondition condition, object state, string[] cascades, Func<DataDeleteContext, bool> deleting = null, Action<DataDeleteContext> deleted = null)
		{
			if(string.IsNullOrEmpty(name))
				throw new ArgumentNullException(nameof(name));

			if(cascades != null && cascades.Length == 1)
				cascades = cascades[0].Split(',', ';');

			//创建数据访问上下文对象
			var context = this.CreateDeleteContext(name, condition, cascades, state);

			//处理数据访问操作前的回调
			if(deleting != null && deleting(context))
				return context.Count;

			//激发“Deleting”事件，如果被中断则返回
			if(this.OnDeleting(context))
				return context.Count;

			//调用数据访问过滤器前事件
			this.InvokeFilters(context, (filter, ctx) => filter.OnFiltering(ctx));

			//执行数据删除操作
			this.OnDelete(context);

			//调用数据访问过滤器后事件
			this.InvokeFilters(context, (filter, ctx) => filter.OnFiltered(ctx));

			//激发“Deleted”事件
			this.OnDeleted(context);

			//处理数据访问操作后的回调
			if(deleted != null)
				deleted(context);

			//返回最终结果
			return context.Count;
		}

		protected abstract void OnDelete(DataDeleteContext context);
		#endregion

		#region 插入方法
		public int Insert<T>(T data)
		{
			if(data == null)
				return 0;

			return this.Insert(this.GetName(data.GetType()), data, null, null, null, null);
		}

		public int Insert<T>(T data, object state)
		{
			if(data == null)
				return 0;

			return this.Insert(this.GetName(data.GetType()), data, null, state, null, null);
		}

		public int Insert<T>(T data, string scope)
		{
			if(data == null)
				return 0;

			return this.Insert(this.GetName(data.GetType()), data, scope, null, null, null);
		}

		public int Insert<T>(T data, string scope, object state, Func<DataInsertContext, bool> inserting = null, Action<DataInsertContext> inserted = null)
		{
			if(data == null)
				return 0;

			return this.Insert(this.GetName(data.GetType()), data, scope, state, inserting, inserted);
		}

		public int Insert(string name, object data)
		{
			return this.Insert(name, data, null, null, null, null);
		}

		public int Insert(string name, object data, object state)
		{
			return this.Insert(name, data, null, state, null, null);
		}

		public int Insert(string name, object data, string scope)
		{
			return this.Insert(name, data, scope, null, null, null);
		}

		public int Insert(string name, object data, string scope, object state, Func<DataInsertContext, bool> inserting = null, Action<DataInsertContext> inserted = null)
		{
			if(string.IsNullOrEmpty(name))
				throw new ArgumentNullException(nameof(name));

			if(data == null)
				return 0;

			//创建数据访问上下文对象
			var context = this.CreateInsertContext(name, false, data, scope, state);

			//处理数据访问操作前的回调
			if(inserting != null && inserting(context))
				return context.Count;

			//激发“Inserting”事件，如果被中断则返回
			if(this.OnInserting(context))
				return context.Count;

			//调用数据访问过滤器前事件
			this.InvokeFilters(context, (filter, ctx) => filter.OnFiltering(ctx));

			//执行数据插入操作
			this.OnInsert(context);

			//调用数据访问过滤器后事件
			this.InvokeFilters(context, (filter, ctx) => filter.OnFiltered(ctx));

			//激发“Inserted”事件
			this.OnInserted(context);

			//处理数据访问操作后的回调
			if(inserted != null)
				inserted(context);

			//返回最终的结果
			return context.Count;
		}

		public int InsertMany<T>(IEnumerable<T> items)
		{
			if(items == null)
				return 0;

			return this.InsertMany(this.GetName<T>(), items, null, null, null, null);
		}

		public int InsertMany<T>(IEnumerable<T> items, object state)
		{
			if(items == null)
				return 0;

			return this.InsertMany(this.GetName<T>(), items, null, state, null, null);
		}

		public int InsertMany<T>(IEnumerable<T> items, string scope)
		{
			if(items == null)
				return 0;

			return this.InsertMany(this.GetName<T>(), items, scope, null, null, null);
		}

		public int InsertMany<T>(IEnumerable<T> items, string scope, object state, Func<DataInsertContext, bool> inserting = null, Action<DataInsertContext> inserted = null)
		{
			if(items == null)
				return 0;

			return this.InsertMany(this.GetName<T>(), items, scope, state, inserting, inserted);
		}

		public int InsertMany(string name, IEnumerable items)
		{
			return this.InsertMany(name, items, null, null, null, null);
		}

		public int InsertMany(string name, IEnumerable items, object state)
		{
			return this.InsertMany(name, items, null, state, null, null);
		}

		public int InsertMany(string name, IEnumerable items, string scope)
		{
			return this.InsertMany(name, items, scope, null, null, null);
		}

		public int InsertMany(string name, IEnumerable items, string scope, object state, Func<DataInsertContext, bool> inserting = null, Action<DataInsertContext> inserted = null)
		{
			if(string.IsNullOrEmpty(name))
				throw new ArgumentNullException(nameof(name));

			if(items == null)
				return 0;

			//创建数据访问上下文对象
			var context = this.CreateInsertContext(name, true, items, scope, state);

			//处理数据访问操作前的回调
			if(inserting != null && inserting(context))
				return context.Count;

			//激发“Inserting”事件，如果被中断则返回
			if(this.OnInserting(context))
				return context.Count;

			//调用数据访问过滤器前事件
			this.InvokeFilters(context, (filter, ctx) => filter.OnFiltering(ctx));

			//执行数据插入操作
			this.OnInsert(context);

			//调用数据访问过滤器后事件
			this.InvokeFilters(context, (filter, ctx) => filter.OnFiltered(ctx));

			//激发“Inserted”事件
			this.OnInserted(context);

			//处理数据访问操作后的回调
			if(inserted != null)
				inserted(context);

			//返回最终的结果
			return context.Count;
		}

		protected abstract void OnInsert(DataInsertContext context);
		#endregion

		#region 更新方法
		public int Update<T>(T data)
		{
			if(data == null)
				return 0;

			return this.Update(this.GetName(data.GetType()), data, null, string.Empty, null, null, null);
		}

		public int Update<T>(T data, object state)
		{
			if(data == null)
				return 0;

			return this.Update(this.GetName(data.GetType()), data, null, string.Empty, state, null, null);
		}

		public int Update<T>(T data, string scope)
		{
			if(data == null)
				return 0;

			return this.Update(this.GetName(data.GetType()), data, null, scope, null, null, null);
		}

		public int Update<T>(T data, string scope, object state)
		{
			if(data == null)
				return 0;

			return this.Update(this.GetName(data.GetType()), data, null, scope, state, null, null);
		}

		public int Update<T>(T data, ICondition condition)
		{
			if(data == null)
				return 0;

			return this.Update(this.GetName(data.GetType()), data, condition, string.Empty, null, null, null);
		}

		public int Update<T>(T data, ICondition condition, object state)
		{
			if(data == null)
				return 0;

			return this.Update(this.GetName(data.GetType()), data, condition, string.Empty, state, null, null);
		}

		public int Update<T>(T data, ICondition condition, string scope)
		{
			if(data == null)
				return 0;

			return this.Update(this.GetName(data.GetType()), data, condition, scope, null, null, null);
		}

		public int Update<T>(T data, ICondition condition, string scope, object state)
		{
			if(data == null)
				return 0;

			return this.Update(this.GetName(data.GetType()), data, condition, scope, state, null, null);
		}

		public int Update<T>(T data, ICondition condition, string scope, object state, Func<DataUpdateContext, bool> updating = null, Action<DataUpdateContext> updated = null)
		{
			if(data == null)
				return 0;

			return this.Update(this.GetName(data.GetType()), data, condition, scope, state, updating, updated);
		}

		public int Update(string name, object data)
		{
			return this.Update(name, data, null, string.Empty, null, null, null);
		}

		public int Update(string name, object data, object state)
		{
			return this.Update(name, data, null, string.Empty, state, null, null);
		}

		public int Update(string name, object data, string scope)
		{
			return this.Update(name, data, null, scope, null, null, null);
		}

		public int Update(string name, object data, string scope, object state)
		{
			return this.Update(name, data, null, scope, state, null, null);
		}

		public int Update(string name, object data, ICondition condition)
		{
			return this.Update(name, data, condition, string.Empty, null, null, null);
		}

		public int Update(string name, object data, ICondition condition, object state)
		{
			return this.Update(name, data, condition, string.Empty, state, null, null);
		}

		public int Update(string name, object data, ICondition condition, string scope)
		{
			return this.Update(name, data, condition, scope, null, null, null);
		}

		/// <summary>
		/// 根据指定的条件将指定的实体更新到数据源。
		/// </summary>
		/// <param name="name">指定的实体映射名。</param>
		/// <param name="data">要更新的实体对象。</param>
		/// <param name="condition">要更新的条件子句，如果为空(null)则根据实体的主键进行更新。</param>
		/// <param name="scope">指定的要更新的和排除更新的属性名列表，如果指定的是多个属性则属性名之间使用逗号(,)分隔；要排除的属性以感叹号(!)打头，星号(*)表示所有属性，感叹号(!)表示排除所有属性；如果未指定该参数则默认只会更新所有单值属性而不会更新导航属性。</param>
		/// <param name="state">指定要传入的状态数据。</param>
		/// <param name="updating">指定的更新前回调函数。</param>
		/// <param name="updated">指定的更新后回调函数。</param>
		/// <returns>返回受影响的记录行数，执行成功返回大于零的整数，失败则返回负数。</returns>
		public int Update(string name, object data, ICondition condition, string scope, object state, Func<DataUpdateContext, bool> updating = null, Action<DataUpdateContext> updated = null)
		{
			if(string.IsNullOrEmpty(name))
				throw new ArgumentNullException(nameof(name));

			if(data == null)
				return 0;

			//创建数据访问上下文对象
			var context = this.CreateUpdateContext(name, false, data, condition, scope, state);

			//处理数据访问操作前的回调
			if(updating != null && updating(context))
				return context.Count;

			//激发“Updating”事件，如果被中断则返回
			if(this.OnUpdating(context))
				return context.Count;

			//调用数据访问过滤器前事件
			this.InvokeFilters(context, (filter, ctx) => filter.OnFiltering(ctx));

			//执行数据更新操作
			this.OnUpdate(context);

			//调用数据访问过滤器后事件
			this.InvokeFilters(context, (filter, ctx) => filter.OnFiltered(ctx));

			//激发“Updated”事件
			this.OnUpdated(context);

			//处理数据访问操作后的回调
			if(updated != null)
				updated(context);

			//返回最终的结果
			return context.Count;
		}

		public int UpdateMany<T>(IEnumerable<T> items)
		{
			return this.UpdateMany(this.GetName<T>(), items, string.Empty, null, null, null);
		}

		public int UpdateMany<T>(IEnumerable<T> items, object state)
		{
			return this.UpdateMany(this.GetName<T>(), items, string.Empty, state, null, null);
		}

		public int UpdateMany<T>(IEnumerable<T> items, string scope)
		{
			return this.UpdateMany(this.GetName<T>(), items, scope, null, null, null);
		}

		public int UpdateMany<T>(IEnumerable<T> items, string scope, object state, Func<DataUpdateContext, bool> updating = null, Action<DataUpdateContext> updated = null)
		{
			return this.UpdateMany(this.GetName<T>(), items, scope, state, updating, updated);
		}

		public int UpdateMany(string name, IEnumerable items)
		{
			return this.UpdateMany(name, items, string.Empty, null, null, null);
		}

		public int UpdateMany(string name, IEnumerable items, object state)
		{
			return this.UpdateMany(name, items, string.Empty, state, null, null);
		}

		public int UpdateMany(string name, IEnumerable items, string scope)
		{
			return this.UpdateMany(name, items, scope, null, null, null);
		}

		/// <summary>
		/// 根据指定的条件将指定的实体集更新到数据源。
		/// </summary>
		/// <param name="name">指定的实体映射名。</param>
		/// <param name="items">要更新的数据集。</param>
		/// <param name="scope">指定的要更新的和排除更新的属性名列表，如果指定的是多个属性则属性名之间使用逗号(,)分隔；要排除的属性以感叹号(!)打头，星号(*)表示所有属性，感叹号(!)表示排除所有属性；如果未指定该参数则默认只会更新所有单值属性而不会更新导航属性。</param>
		/// <param name="state">指定要传入的状态数据。</param>
		/// <param name="updating">可选的更新前回调委托参数。</param>
		/// <param name="updated">可选的更新后回调委托参数。</param>
		/// <returns>返回受影响的记录行数，执行成功返回大于零的整数，失败则返回负数。</returns>
		public int UpdateMany(string name, IEnumerable items, string scope, object state, Func<DataUpdateContext, bool> updating = null, Action<DataUpdateContext> updated = null)
		{
			if(string.IsNullOrEmpty(name))
				throw new ArgumentNullException(nameof(name));

			if(items == null)
				return 0;

			//创建数据访问上下文对象
			var context = this.CreateUpdateContext(name, true, items, null, scope, state);

			//处理数据访问操作前的回调
			if(updating != null && updating(context))
				return context.Count;

			//激发“Updating”事件，如果被中断则返回
			if(this.OnUpdating(context))
				return context.Count;

			//调用数据访问过滤器前事件
			this.InvokeFilters(context, (filter, ctx) => filter.OnFiltering(ctx));

			//执行数据更新操作
			this.OnUpdate(context);

			//调用数据访问过滤器后事件
			this.InvokeFilters(context, (filter, ctx) => filter.OnFiltered(ctx));

			//激发“Updated”事件
			this.OnUpdated(context);

			//处理数据访问操作后的回调
			if(updated != null)
				updated(context);

			//返回最终的结果
			return context.Count;
		}

		protected abstract void OnUpdate(DataUpdateContext context);
		#endregion

		#region 查询方法
		public IEnumerable<T> Select<T>(object state = null, params Sorting[] sortings)
		{
			return this.Select<T>(this.GetName<T>(), null, string.Empty, null, state, sortings, null, null);
		}

		public IEnumerable<T> Select<T>(ICondition condition, params Sorting[] sortings)
		{
			return this.Select<T>(this.GetName<T>(), condition, string.Empty, null, null, sortings, null, null);
		}

		public IEnumerable<T> Select<T>(ICondition condition, object state, params Sorting[] sortings)
		{
			return this.Select<T>(this.GetName<T>(), condition, string.Empty, null, state, sortings, null, null);
		}

		public IEnumerable<T> Select<T>(ICondition condition, Paging paging, params Sorting[] sortings)
		{
			return this.Select<T>(this.GetName<T>(), condition, null, paging, null, sortings, null, null);
		}

		public IEnumerable<T> Select<T>(ICondition condition, Paging paging, object state, params Sorting[] sortings)
		{
			return this.Select<T>(this.GetName<T>(), condition, null, paging, state, sortings, null, null);
		}

		public IEnumerable<T> Select<T>(ICondition condition, Paging paging, string scope, params Sorting[] sortings)
		{
			return this.Select<T>(this.GetName<T>(), condition, scope, paging, null, sortings, null, null);
		}

		public IEnumerable<T> Select<T>(ICondition condition, Paging paging, string scope, object state, params Sorting[] sortings)
		{
			return this.Select<T>(this.GetName<T>(), condition, scope, paging, state, sortings, null, null);
		}

		public IEnumerable<T> Select<T>(ICondition condition, string scope, params Sorting[] sortings)
		{
			return this.Select<T>(this.GetName<T>(), condition, scope, null, null, sortings, null, null);
		}

		public IEnumerable<T> Select<T>(ICondition condition, string scope, object state, params Sorting[] sortings)
		{
			return this.Select<T>(this.GetName<T>(), condition, scope, null, state, sortings, null, null);
		}

		public IEnumerable<T> Select<T>(ICondition condition, string scope, Paging paging, params Sorting[] sortings)
		{
			return this.Select<T>(this.GetName<T>(), condition, scope, paging, null, sortings, null, null);
		}

		public IEnumerable<T> Select<T>(ICondition condition, string scope, Paging paging, object state, params Sorting[] sortings)
		{
			return this.Select<T>(this.GetName<T>(), condition, scope, paging, state, sortings, null, null);
		}

		public IEnumerable<T> Select<T>(ICondition condition, string scope, Paging paging, object state, Sorting[] sortings, Func<DataSelectContext, bool> selecting, Action<DataSelectContext> selected)
		{
			return this.Select<T>(this.GetName<T>(), condition, scope, paging, state, sortings, selecting, selected);
		}

		public IEnumerable<T> Select<T>(string name, object state = null, params Sorting[] sortings)
		{
			return this.Select<T>(name, null, string.Empty, null, state, sortings, null, null);
		}

		public IEnumerable<T> Select<T>(string name, ICondition condition, params Sorting[] sortings)
		{
			return this.Select<T>(name, condition, string.Empty, null, null, sortings, null, null);
		}

		public IEnumerable<T> Select<T>(string name, ICondition condition, object state, params Sorting[] sortings)
		{
			return this.Select<T>(name, condition, string.Empty, null, state, sortings, null, null);
		}

		public IEnumerable<T> Select<T>(string name, ICondition condition, Paging paging, params Sorting[] sortings)
		{
			return this.Select<T>(name, condition, string.Empty, paging, null, sortings, null, null);
		}

		public IEnumerable<T> Select<T>(string name, ICondition condition, Paging paging, object state, params Sorting[] sortings)
		{
			return this.Select<T>(name, condition, string.Empty, paging, state, sortings, null, null);
		}

		public IEnumerable<T> Select<T>(string name, ICondition condition, Paging paging, string scope, params Sorting[] sortings)
		{
			return this.Select<T>(name, condition, scope, paging, null, sortings, null, null);
		}

		public IEnumerable<T> Select<T>(string name, ICondition condition, Paging paging, string scope, object state, params Sorting[] sortings)
		{
			return this.Select<T>(name, condition, scope, paging, state, sortings, null, null);
		}

		public IEnumerable<T> Select<T>(string name, ICondition condition, string scope, params Sorting[] sortings)
		{
			return this.Select<T>(name, condition, scope, null, null, sortings, null, null);
		}

		public IEnumerable<T> Select<T>(string name, ICondition condition, string scope, object state, params Sorting[] sortings)
		{
			return this.Select<T>(name, condition, scope, null, state, sortings, null, null);
		}

		public IEnumerable<T> Select<T>(string name, ICondition condition, string scope, Paging paging, params Sorting[] sortings)
		{
			return this.Select<T>(name, condition, scope, paging, null, sortings, null, null);
		}

		public IEnumerable<T> Select<T>(string name, ICondition condition, string scope, Paging paging, object state, params Sorting[] sortings)
		{
			return this.Select<T>(name, condition, scope, paging, state, sortings, null, null);
		}

		public IEnumerable<T> Select<T>(string name, ICondition condition, string scope, Paging paging, object state, Sorting[] sortings, Func<DataSelectContext, bool> selecting, Action<DataSelectContext> selected)
		{
			if(string.IsNullOrEmpty(name))
				throw new ArgumentNullException(nameof(name));

			//创建数据访问上下文对象
			var context = this.CreateSelectContext(name, typeof(T), condition, null, scope, paging, sortings, state);

			//执行查询方法
			return this.Select<T>(context, selecting, selected);
		}

		public IEnumerable<T> Select<T>(string name, Grouping grouping, params Sorting[] sortings)
		{
			return this.Select<T>(name, grouping, null, string.Empty, null, null, sortings, null, null);
		}

		public IEnumerable<T> Select<T>(string name, Grouping grouping, object state, params Sorting[] sortings)
		{
			return this.Select<T>(name, grouping, null, string.Empty, null, state, sortings, null, null);
		}

		public IEnumerable<T> Select<T>(string name, Grouping grouping, string scope, object state = null, params Sorting[] sortings)
		{
			return this.Select<T>(name, grouping, null, scope, null, state, sortings, null, null);
		}

		public IEnumerable<T> Select<T>(string name, Grouping grouping, Paging paging, object state = null, params Sorting[] sortings)
		{
			return this.Select<T>(name, grouping, null, string.Empty, paging, state, sortings, null, null);
		}

		public IEnumerable<T> Select<T>(string name, Grouping grouping, string scope, Paging paging, object state = null, params Sorting[] sortings)
		{
			return this.Select<T>(name, grouping, null, scope, paging, state, sortings, null, null);
		}

		public IEnumerable<T> Select<T>(string name, Grouping grouping, ICondition condition, params Sorting[] sortings)
		{
			return this.Select<T>(name, grouping, condition, string.Empty, null, null, sortings, null, null);
		}

		public IEnumerable<T> Select<T>(string name, Grouping grouping, ICondition condition, object state, params Sorting[] sortings)
		{
			return this.Select<T>(name, grouping, condition, string.Empty, null, state, sortings, null, null);
		}

		public IEnumerable<T> Select<T>(string name, Grouping grouping, ICondition condition, string scope, object state = null, params Sorting[] sortings)
		{
			return this.Select<T>(name, grouping, condition, scope, null, state, sortings, null, null);
		}

		public IEnumerable<T> Select<T>(string name, Grouping grouping, ICondition condition, Paging paging, object state = null, params Sorting[] sortings)
		{
			return this.Select<T>(name, grouping, condition, string.Empty, paging, state, sortings, null, null);
		}

		public IEnumerable<T> Select<T>(string name, Grouping grouping, ICondition condition, string scope, Paging paging, object state = null, params Sorting[] sortings)
		{
			return this.Select<T>(name, grouping, condition, scope, paging, state, sortings, null, null);
		}

		public IEnumerable<T> Select<T>(string name, Grouping grouping, ICondition condition, string scope, Paging paging, object state, Sorting[] sortings, Func<DataSelectContext, bool> selecting, Action<DataSelectContext> selected)
		{
			if(string.IsNullOrEmpty(name))
				throw new ArgumentNullException(nameof(name));

			//创建数据访问上下文对象
			var context = this.CreateSelectContext(name, typeof(T), condition, grouping, scope, paging, sortings, state);

			//执行查询方法
			return this.Select<T>(context, selecting, selected);
		}

		private IEnumerable<T> Select<T>(DataSelectContext context, Func<DataSelectContext, bool> selecting, Action<DataSelectContext> selected)
		{
			//处理数据访问操作前的回调
			if(selecting != null && selecting(context))
				return context.Result as IEnumerable<T>;

			//激发“Selecting”事件，如果被中断则返回
			if(this.OnSelecting(context))
				return context.Result as IEnumerable<T>;

			//调用数据访问过滤器前事件
			this.InvokeFilters(context, (filter, ctx) => filter.OnFiltering(ctx));

			//执行数据查询操作
			this.OnSelect<T>(context);

			//调用数据访问过滤器后事件
			this.InvokeFilters(context, (filter, ctx) => filter.OnFiltered(ctx));

			//激发“Selected”事件
			this.OnSelected(context);

			//处理数据访问操作后的回调
			if(selected != null)
				selected(context);

			//返回最终的结果
			return context.Result as IEnumerable<T>;
		}

		protected abstract void OnSelect<T>(DataSelectContext context);
		#endregion

		#region 虚拟方法
		protected virtual string GetName(Type type)
		{
			var name = _naming.Get(type);

			if(string.IsNullOrEmpty(name))
				throw new InvalidOperationException($"Missing data access name mapping of the '{type.FullName}' type.");

			return name;
		}

		protected virtual DataCountContext CreateCountContext(string name, ICondition condition, string includes, object state)
		{
			return new DataCountContext(this, name, condition, includes, state);
		}

		protected virtual DataExistContext CreateExistContext(string name, ICondition condition, object state)
		{
			return new DataExistContext(this, name, condition, state);
		}

		protected virtual DataExecuteContext CreateExecuteContext(string name, bool isScalar, Type resultType, IDictionary<string, object> inParameters, object state)
		{
			return new DataExecuteContext(this, name, isScalar, resultType, inParameters, null, state);
		}

		protected virtual DataIncrementContext CreateIncrementContext(string name, string member, ICondition condition, int interval, object state)
		{
			return new DataIncrementContext(this, name, member, condition, interval, state);
		}

		protected virtual DataDeleteContext CreateDeleteContext(string name, ICondition condition, string[] cascades, object state)
		{
			return new DataDeleteContext(this, name, condition, cascades, state);
		}

		protected virtual DataInsertContext CreateInsertContext(string name, bool isMultiple, object data, string scope, object state)
		{
			if(isMultiple)
				data = GetDataDictionaries(data);
			else
				data = GetDataDictionary(data);

			return new DataInsertContext(this, name, isMultiple, data, scope, state);
		}

		protected virtual DataUpsertContext CreateUpsertContext(string name, bool isMultiple, object data, string scope, object state)
		{
			if(isMultiple)
				data = GetDataDictionaries(data);
			else
				data = GetDataDictionary(data);

			return new DataUpsertContext(this, name, isMultiple, data, scope, state);
		}

		protected virtual DataUpdateContext CreateUpdateContext(string name, bool isMultiple, object data, ICondition condition, string scope, object state)
		{
			if(isMultiple)
				data = GetDataDictionaries(data);
			else
				data = GetDataDictionary(data);

			return new DataUpdateContext(this, name, isMultiple, data, condition, scope, state);
		}

		protected virtual DataSelectContext CreateSelectContext(string name, Type elementType, ICondition condition, Grouping grouping, string scope, Paging paging, Sorting[] sortings, object state)
		{
			return new DataSelectContext(this, name, elementType, grouping, condition, scope, paging, sortings, state);
		}
		#endregion

		#region 激发事件
		protected virtual void OnCounted(DataCountContext context)
		{
			var e = this.Counted;

			if(e != null)
				e(this, new DataCountedEventArgs(context));
		}

		protected virtual bool OnCounting(DataCountContext context)
		{
			var e = this.Counting;

			if(e == null)
				return false;

			var args = new DataCountingEventArgs(context);
			e(this, args);
			return args.Cancel;
		}

		protected virtual void OnExecuted(DataExecuteContext context)
		{
			var e = this.Executed;

			if(e != null)
				e(this, new DataExecutedEventArgs(context));
		}

		protected virtual bool OnExecuting(DataExecuteContext context)
		{
			var e = this.Executing;

			if(e == null)
				return false;

			var args = new DataExecutingEventArgs(context);
			e(this, args);
			return args.Cancel;
		}

		protected virtual void OnExisted(DataExistContext context)
		{
			var e = this.Existed;

			if(e != null)
				e(this, new DataExistedEventArgs(context));
		}

		protected virtual bool OnExisting(DataExistContext context)
		{
			var e = this.Existing;

			if(e == null)
				return false;

			var args = new DataExistingEventArgs(context);
			e(this, args);
			return args.Cancel;
		}

		protected virtual void OnIncremented(DataIncrementContext context)
		{
			var e = this.Incremented;

			if(e != null)
				e(this, new DataIncrementedEventArgs(context));
		}

		protected virtual bool OnIncrementing(DataIncrementContext context)
		{
			var e = this.Incrementing;

			if(e == null)
				return false;

			var args = new DataIncrementingEventArgs(context);
			e(this, args);
			return args.Cancel;
		}

		protected virtual void OnDeleted(DataDeleteContext context)
		{
			var e = this.Deleted;

			if(e != null)
				e(this, new DataDeletedEventArgs(context));
		}

		protected virtual bool OnDeleting(DataDeleteContext context)
		{
			var e = this.Deleting;

			if(e == null)
				return false;

			var args = new DataDeletingEventArgs(context);
			e(this, args);
			return args.Cancel;
		}

		protected virtual void OnInserted(DataInsertContext context)
		{
			var e = this.Inserted;

			if(e != null)
				e(this, new DataInsertedEventArgs(context));
		}

		protected virtual bool OnInserting(DataInsertContext context)
		{
			var e = this.Inserting;

			if(e == null)
				return false;

			var args = new DataInsertingEventArgs(context);
			e(this, args);
			return args.Cancel;
		}

		protected virtual void OnUpdated(DataUpdateContext context)
		{
			var e = this.Updated;

			if(e != null)
				e(this, new DataUpdatedEventArgs(context));
		}

		protected virtual bool OnUpdating(DataUpdateContext context)
		{
			var e = this.Updating;

			if(e == null)
				return false;

			var args = new DataUpdatingEventArgs(context);
			e(this, args);
			return args.Cancel;
		}

		protected virtual void OnSelected(DataSelectContext context)
		{
			var e = this.Selected;

			if(e != null)
				e(this, new DataSelectedEventArgs(context));
		}

		protected virtual bool OnSelecting(DataSelectContext context)
		{
			var e = this.Selecting;

			if(e == null)
				return false;

			var args = new DataSelectingEventArgs(context);
			e(this, args);
			return args.Cancel;
		}
		#endregion

		#region 私有方法
		private string GetName<T>()
		{
			return this.GetName(typeof(T));
		}

		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.Synchronized)]
		private void InvokeFilters(DataAccessContextBase context, Action<IDataAccessFilter, DataAccessContextBase> invoke)
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

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
 * Copyright (C) 2010-2019 Zongsoft Corporation <http://www.zongsoft.com>
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
	[System.Reflection.DefaultMember(nameof(Filters))]
	public abstract class DataAccessBase : IDataAccess
	{
		#region 事件定义
		public event EventHandler<DataAccessErrorEventArgs> Error;
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
		public event EventHandler<DataUpsertedEventArgs> Upserted;
		public event EventHandler<DataUpsertingEventArgs> Upserting;
		public event EventHandler<DataUpdatedEventArgs> Updated;
		public event EventHandler<DataUpdatingEventArgs> Updating;
		public event EventHandler<DataSelectedEventArgs> Selected;
		public event EventHandler<DataSelectingEventArgs> Selecting;
		#endregion

		#region 成员字段
		private string _name;
		private ISchemaParser _schema;
		private Common.ISequence _sequence;
		private IDataAccessNaming _naming;
		private FilterCollection _filters;
		private Metadata.IDataValueProviderBinder _binder;
		#endregion

		#region 构造函数
		protected DataAccessBase(string name)
		{
			_name = name ?? string.Empty;
			_naming = new DataAccessNaming();
			_filters = new FilterCollection();
		}

		protected DataAccessBase(string name, IDataAccessNaming naming)
		{
			_name = name ?? string.Empty;
			_naming = naming ?? throw new ArgumentNullException(nameof(naming));
			_filters = new FilterCollection();
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
		/// 获取数据模式解析器。
		/// </summary>
		public ISchemaParser Schema
		{
			get
			{
				if(_schema == null)
					_schema = this.CreateSchema();

				return _schema;
			}
		}

		/// <summary>
		/// 获取或设置一个数据序号提供程序。
		/// </summary>
		public Common.ISequence Sequence
		{
			get
			{
				return _sequence;
			}
			set
			{
				_sequence = this.CreateSequence(value);
			}
		}

		/// <summary>
		/// 获取数据访问器的元数据容器。
		/// </summary>
		public abstract Metadata.IDataMetadataContainer Metadata
		{
			get;
		}

		/// <summary>
		/// 获取或设置数据提供程序绑定器。
		/// </summary>
		public Metadata.IDataValueProviderBinder Binder
		{
			get
			{
				return _binder;
			}
			set
			{
				if(_binder != null)
					_binder.Unbind(this.Metadata);

				if(value != null)
					value.Bind(this.Metadata);

				_binder = value;
			}
		}

		/// <summary>
		/// 获取数据访问过滤器集合。
		/// </summary>
		public ICollection<IDataAccessFilter> Filters
		{
			get => _filters;
		}
		#endregion

		#region 执行方法
		public IEnumerable<T> Execute<T>(string name, IDictionary<string, object> inParameters, object state = null, Func<DataExecuteContextBase, bool> executing = null, Action<DataExecuteContextBase> executed = null)
		{
			IDictionary<string, object> outParameters;
			return this.Execute<T>(name, inParameters, out outParameters, state, executing, executed);
		}

		public IEnumerable<T> Execute<T>(string name, IDictionary<string, object> inParameters, out IDictionary<string, object> outParameters, object state = null, Func<DataExecuteContextBase, bool> executing = null, Action<DataExecuteContextBase> executed = null)
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
			this.OnFiltering(context);

			//执行数据操作方法
			this.OnExecute(context);

			//调用数据访问过滤器后事件
			this.OnFiltered(context);

			//激发“Executed”事件
			this.OnExecuted(context);

			//处理数据访问操作后的回调
			if(executed != null)
				executed(context);

			//再次更新返回参数值
			outParameters = context.OutParameters;

			var result = this.ToEnumerable<T>(context.Result);

			//处置上下文资源
			context.Dispose();

			//返回最终的结果
			return result;
		}

		public object ExecuteScalar(string name, IDictionary<string, object> inParameters, object state = null, Func<DataExecuteContextBase, bool> executing = null, Action<DataExecuteContextBase> executed = null)
		{
			IDictionary<string, object> outParameters;
			return this.ExecuteScalar(name, inParameters, out outParameters, state, executing, executed);
		}

		public object ExecuteScalar(string name, IDictionary<string, object> inParameters, out IDictionary<string, object> outParameters, object state = null, Func<DataExecuteContextBase, bool> executing = null, Action<DataExecuteContextBase> executed = null)
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
			this.OnFiltering(context);

			//执行数据操作方法
			this.OnExecute(context);

			//调用数据访问过滤器后事件
			this.OnFiltered(context);

			//激发“Executed”事件
			this.OnExecuted(context);

			//处理数据访问操作后的回调
			if(executed != null)
				executed(context);

			//再次更新返回参数值
			outParameters = context.OutParameters;

			var result = context.Result;

			//处置上下文资源
			context.Dispose();

			//返回最终的结果
			return result;
		}

		protected abstract void OnExecute(DataExecuteContextBase context);
		#endregion

		#region 存在方法
		public bool Exists<T>(ICondition condition, object state = null, Func<DataExistContextBase, bool> existing = null, Action<DataExistContextBase> existed = null)
		{
			return this.Exists(this.GetName<T>(), condition, state, existing, existed);
		}

		public bool Exists(string name, ICondition condition, object state = null, Func<DataExistContextBase, bool> existing = null, Action<DataExistContextBase> existed = null)
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
			this.OnFiltering(context);

			//执行存在操作方法
			this.OnExists(context);

			//调用数据访问过滤器后事件
			this.OnFiltered(context);

			//激发“Existed”事件
			this.OnExisted(context);

			//处理数据访问操作后的回调
			if(existed != null)
				existed(context);

			var result = context.Result;

			//处置上下文资源
			context.Dispose();

			//返回最终的结果
			return result;
		}

		protected abstract void OnExists(DataExistContextBase context);
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

		public int Count<T>(ICondition condition, string member)
		{
			return this.Count(this.GetName<T>(), condition, member, null, null, null);
		}

		public int Count<T>(ICondition condition, string member, object state, Func<DataCountContextBase, bool> counting = null, Action<DataCountContextBase> counted = null)
		{
			return this.Count(this.GetName<T>(), condition, member, state, counting, counted);
		}

		public int Count(string name, ICondition condition)
		{
			return this.Count(name, condition, string.Empty, null, null, null);
		}

		public int Count(string name, ICondition condition, object state)
		{
			return this.Count(name, condition, string.Empty, state, null, null);
		}

		public int Count(string name, ICondition condition, string member)
		{
			return this.Count(name, condition, member, null, null, null);
		}

		public int Count(string name, ICondition condition, string member, object state, Func<DataCountContextBase, bool> counting = null, Action<DataCountContextBase> counted = null)
		{
			if(string.IsNullOrEmpty(name))
				throw new ArgumentNullException(nameof(name));

			//创建数据访问上下文对象
			var context = this.CreateCountContext(name, condition, member, state);

			//处理数据访问操作前的回调
			if(counting != null && counting(context))
				return context.Result;

			//激发“Counting”事件，如果被中断则返回
			if(this.OnCounting(context))
				return context.Result;

			//调用数据访问过滤器前事件
			this.OnFiltering(context);

			//执行计数操作方法
			this.OnCount(context);

			//调用数据访问过滤器后事件
			this.OnFiltered(context);

			//激发“Counted”事件
			this.OnCounted(context);

			//处理数据访问操作后的回调
			if(counted != null)
				counted(context);

			var result = context.Result;

			//处置上下文资源
			context.Dispose();

			//返回最终的结果
			return result;
		}

		protected abstract void OnCount(DataCountContextBase context);
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

		public long Increment<T>(string member, ICondition condition, int interval, object state, Func<DataIncrementContextBase, bool> incrementing = null, Action<DataIncrementContextBase> incremented = null)
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

		public long Increment(string name, string member, ICondition condition, int interval, object state, Func<DataIncrementContextBase, bool> incrementing = null, Action<DataIncrementContextBase> incremented = null)
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
			this.OnFiltering(context);

			//执行递增操作方法
			this.OnIncrement(context);

			//调用数据访问过滤器后事件
			this.OnFiltered(context);

			//激发“Incremented”事件
			this.OnIncremented(context);

			//处理数据访问操作后的回调
			if(incremented != null)
				incremented(context);

			var result = context.Result;

			//处置上下文资源
			context.Dispose();

			//返回最终的结果
			return result;
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

		public long Decrement<T>(string member, ICondition condition, int interval, object state, Func<DataIncrementContextBase, bool> decrementing = null, Action<DataIncrementContextBase> decremented = null)
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

		public long Decrement(string name, string member, ICondition condition, int interval, object state, Func<DataIncrementContextBase, bool> decrementing = null, Action<DataIncrementContextBase> decremented = null)
		{
			return this.Increment(name, member, condition, -interval, state, decrementing, decremented);
		}

		protected abstract void OnIncrement(DataIncrementContextBase context);
		#endregion

		#region 删除方法
		public int Delete<T>(ICondition condition, string schema = null)
		{
			return this.Delete(this.GetName<T>(), condition, schema, null, null, null);
		}

		public int Delete<T>(ICondition condition, object state)
		{
			return this.Delete(this.GetName<T>(), condition, string.Empty, state, null, null);
		}

		public int Delete<T>(ICondition condition, string schema, object state, Func<DataDeleteContextBase, bool> deleting = null, Action<DataDeleteContextBase> deleted = null)
		{
			return this.Delete(this.GetName<T>(), condition, schema, state, deleting, deleted);
		}

		public int Delete(string name, ICondition condition, string schema = null)
		{
			return this.Delete(name, condition, schema, null, null, null);
		}

		public int Delete(string name, ICondition condition, object state)
		{
			return this.Delete(name, condition, string.Empty, state, null, null);
		}

		public int Delete(string name, ICondition condition, string schema, object state, Func<DataDeleteContextBase, bool> deleting = null, Action<DataDeleteContextBase> deleted = null)
		{
			return this.Delete(name, condition, this.Schema.Parse(name, schema), state, deleting, deleted);
		}

		public int Delete(string name, ICondition condition, ISchema schema, object state, Func<DataDeleteContextBase, bool> deleting = null, Action<DataDeleteContextBase> deleted = null)
		{
			if(string.IsNullOrEmpty(name))
				throw new ArgumentNullException(nameof(name));

			//创建数据访问上下文对象
			var context = this.CreateDeleteContext(name, condition, schema, state);

			//处理数据访问操作前的回调
			if(deleting != null && deleting(context))
				return context.Count;

			//激发“Deleting”事件，如果被中断则返回
			if(this.OnDeleting(context))
				return context.Count;

			//调用数据访问过滤器前事件
			this.OnFiltering(context);

			//执行数据删除操作
			this.OnDelete(context);

			//调用数据访问过滤器后事件
			this.OnFiltered(context);

			//激发“Deleted”事件
			this.OnDeleted(context);

			//处理数据访问操作后的回调
			if(deleted != null)
				deleted(context);

			var result = context.Count;

			//处置上下文资源
			context.Dispose();

			//返回最终的结果
			return result;
		}

		protected abstract void OnDelete(DataDeleteContextBase context);
		#endregion

		#region 插入方法
		public int Insert<T>(T data)
		{
			if(data == null)
				return 0;

			return this.Insert(this.GetName(data.GetType()), data, string.Empty, null, null, null);
		}

		public int Insert<T>(T data, object state)
		{
			if(data == null)
				return 0;

			return this.Insert(this.GetName(data.GetType()), data, string.Empty, state, null, null);
		}

		public int Insert<T>(T data, string schema)
		{
			if(data == null)
				return 0;

			return this.Insert(this.GetName(data.GetType()), data, schema, null, null, null);
		}

		public int Insert<T>(T data, string schema, object state, Func<DataInsertContextBase, bool> inserting = null, Action<DataInsertContextBase> inserted = null)
		{
			if(data == null)
				return 0;

			return this.Insert(this.GetName(data.GetType()), data, schema, state, inserting, inserted);
		}

		public int Insert<T>(object data)
		{
			if(data == null)
				return 0;

			return this.Insert(this.GetName(typeof(T)), data, string.Empty, null, null, null);
		}

		public int Insert<T>(object data, object state)
		{
			if(data == null)
				return 0;

			return this.Insert(this.GetName(typeof(T)), data, string.Empty, state, null, null);
		}

		public int Insert<T>(object data, string schema)
		{
			if(data == null)
				return 0;

			return this.Insert(this.GetName(typeof(T)), data, schema, null, null, null);
		}

		public int Insert<T>(object data, string schema, object state, Func<DataInsertContextBase, bool> inserting = null, Action<DataInsertContextBase> inserted = null)
		{
			if(data == null)
				return 0;

			return this.Insert(this.GetName(typeof(T)), data, schema, state, inserting, inserted);
		}

		public int Insert(string name, object data)
		{
			return this.Insert(name, data, string.Empty, null, null, null);
		}

		public int Insert(string name, object data, object state)
		{
			return this.Insert(name, data, string.Empty, state, null, null);
		}

		public int Insert(string name, object data, string schema)
		{
			return this.Insert(name, data, schema, null, null, null);
		}

		public int Insert(string name, object data, string schema, object state, Func<DataInsertContextBase, bool> inserting = null, Action<DataInsertContextBase> inserted = null)
		{
			return this.Insert(name, data, this.Schema.Parse(name, schema, data.GetType()), state, inserting, inserted);
		}

		public int Insert(string name, object data, ISchema schema, object state, Func<DataInsertContextBase, bool> inserting = null, Action<DataInsertContextBase> inserted = null)
		{
			if(string.IsNullOrEmpty(name))
				throw new ArgumentNullException(nameof(name));

			if(data == null)
				return 0;

			//创建数据访问上下文对象
			var context = this.CreateInsertContext(name, false, data, schema, state);

			//处理数据访问操作前的回调
			if(inserting != null && inserting(context))
				return context.Count;

			//激发“Inserting”事件，如果被中断则返回
			if(this.OnInserting(context))
				return context.Count;

			//调用数据访问过滤器前事件
			this.OnFiltering(context);

			//执行数据插入操作
			this.OnInsert(context);

			//调用数据访问过滤器后事件
			this.OnFiltered(context);

			//激发“Inserted”事件
			this.OnInserted(context);

			//处理数据访问操作后的回调
			if(inserted != null)
				inserted(context);

			var result = context.Count;

			//处置上下文资源
			context.Dispose();

			//返回最终的结果
			return result;
		}

		public int InsertMany<T>(IEnumerable<T> items)
		{
			if(items == null)
				return 0;

			return this.InsertMany(this.GetName<T>(), items, string.Empty, null, null, null);
		}

		public int InsertMany<T>(IEnumerable<T> items, object state)
		{
			if(items == null)
				return 0;

			return this.InsertMany(this.GetName<T>(), items, string.Empty, state, null, null);
		}

		public int InsertMany<T>(IEnumerable<T> items, string schema)
		{
			if(items == null)
				return 0;

			return this.InsertMany(this.GetName<T>(), items, schema, null, null, null);
		}

		public int InsertMany<T>(IEnumerable<T> items, string schema, object state, Func<DataInsertContextBase, bool> inserting = null, Action<DataInsertContextBase> inserted = null)
		{
			if(items == null)
				return 0;

			return this.InsertMany(this.GetName<T>(), items, schema, state, inserting, inserted);
		}

		public int InsertMany<T>(IEnumerable items)
		{
			if(items == null)
				return 0;

			return this.InsertMany(this.GetName<T>(), items, string.Empty, null, null, null);
		}

		public int InsertMany<T>(IEnumerable items, object state)
		{
			if(items == null)
				return 0;

			return this.InsertMany(this.GetName<T>(), items, string.Empty, state, null, null);
		}

		public int InsertMany<T>(IEnumerable items, string schema)
		{
			if(items == null)
				return 0;

			return this.InsertMany(this.GetName<T>(), items, schema, null, null, null);
		}

		public int InsertMany<T>(IEnumerable items, string schema, object state, Func<DataInsertContextBase, bool> inserting = null, Action<DataInsertContextBase> inserted = null)
		{
			if(items == null)
				return 0;

			return this.InsertMany(this.GetName<T>(), items, schema, state, inserting, inserted);
		}

		public int InsertMany(string name, IEnumerable items)
		{
			return this.InsertMany(name, items, string.Empty, null, null, null);
		}

		public int InsertMany(string name, IEnumerable items, object state)
		{
			return this.InsertMany(name, items, string.Empty, state, null, null);
		}

		public int InsertMany(string name, IEnumerable items, string schema)
		{
			return this.InsertMany(name, items, schema, null, null, null);
		}

		public int InsertMany(string name, IEnumerable items, string schema, object state, Func<DataInsertContextBase, bool> inserting = null, Action<DataInsertContextBase> inserted = null)
		{
			return this.InsertMany(name, items, this.Schema.Parse(name, schema, Common.TypeExtension.GetElementType(items.GetType())), state, inserting, inserted);
		}

		public int InsertMany(string name, IEnumerable items, ISchema schema, object state, Func<DataInsertContextBase, bool> inserting = null, Action<DataInsertContextBase> inserted = null)
		{
			if(string.IsNullOrEmpty(name))
				throw new ArgumentNullException(nameof(name));

			if(items == null)
				return 0;

			//创建数据访问上下文对象
			var context = this.CreateInsertContext(name, true, items, schema, state);

			//处理数据访问操作前的回调
			if(inserting != null && inserting(context))
				return context.Count;

			//激发“Inserting”事件，如果被中断则返回
			if(this.OnInserting(context))
				return context.Count;

			//调用数据访问过滤器前事件
			this.OnFiltering(context);

			//执行数据插入操作
			this.OnInsert(context);

			//调用数据访问过滤器后事件
			this.OnFiltered(context);

			//激发“Inserted”事件
			this.OnInserted(context);

			//处理数据访问操作后的回调
			if(inserted != null)
				inserted(context);

			var result = context.Count;

			//处置上下文资源
			context.Dispose();

			//返回最终的结果
			return result;
		}

		protected abstract void OnInsert(DataInsertContextBase context);
		#endregion

		#region 复写方法
		public int Upsert<T>(T data)
		{
			if(data == null)
				return 0;

			return this.Upsert(this.GetName(data.GetType()), data, string.Empty, null, null, null);
		}

		public int Upsert<T>(T data, object state)
		{
			if(data == null)
				return 0;

			return this.Upsert(this.GetName(data.GetType()), data, string.Empty, state, null, null);
		}

		public int Upsert<T>(T data, string schema)
		{
			if(data == null)
				return 0;

			return this.Upsert(this.GetName(data.GetType()), data, schema, null, null, null);
		}

		public int Upsert<T>(T data, string schema, object state, Func<DataUpsertContextBase, bool> upserting = null, Action<DataUpsertContextBase> upserted = null)
		{
			if(data == null)
				return 0;

			return this.Upsert(this.GetName(data.GetType()), data, schema, state, upserting, upserted);
		}

		public int Upsert<T>(object data)
		{
			if(data == null)
				return 0;

			return this.Upsert(this.GetName(typeof(T)), data, string.Empty, null, null, null);
		}

		public int Upsert<T>(object data, object state)
		{
			if(data == null)
				return 0;

			return this.Upsert(this.GetName(typeof(T)), data, string.Empty, state, null, null);
		}

		public int Upsert<T>(object data, string schema)
		{
			if(data == null)
				return 0;

			return this.Upsert(this.GetName(typeof(T)), data, schema, null, null, null);
		}

		public int Upsert<T>(object data, string schema, object state, Func<DataUpsertContextBase, bool> upserting = null, Action<DataUpsertContextBase> upserted = null)
		{
			if(data == null)
				return 0;

			return this.Upsert(this.GetName(typeof(T)), data, schema, state, upserting, upserted);
		}

		public int Upsert(string name, object data)
		{
			return this.Upsert(name, data, string.Empty, null, null, null);
		}

		public int Upsert(string name, object data, object state)
		{
			return this.Upsert(name, data, string.Empty, state, null, null);
		}

		public int Upsert(string name, object data, string schema)
		{
			return this.Upsert(name, data, schema, null, null, null);
		}

		public int Upsert(string name, object data, string schema, object state, Func<DataUpsertContextBase, bool> upserting = null, Action<DataUpsertContextBase> upserted = null)
		{
			return this.Upsert(name, data, this.Schema.Parse(name, schema, data.GetType()), state, upserting, upserted);
		}

		public int Upsert(string name, object data, ISchema schema, object state, Func<DataUpsertContextBase, bool> upserting = null, Action<DataUpsertContextBase> upserted = null)
		{
			if(string.IsNullOrEmpty(name))
				throw new ArgumentNullException(nameof(name));

			if(data == null)
				return 0;

			//创建数据访问上下文对象
			var context = this.CreateUpsertContext(name, false, data, schema, state);

			//处理数据访问操作前的回调
			if(upserting != null && upserting(context))
				return context.Count;

			//激发“Upserting”事件，如果被中断则返回
			if(this.OnUpserting(context))
				return context.Count;

			//调用数据访问过滤器前事件
			this.OnFiltering(context);

			//执行数据插入操作
			this.OnUpsert(context);

			//调用数据访问过滤器后事件
			this.OnFiltered(context);

			//激发“Upserted”事件
			this.OnUpserted(context);

			//处理数据访问操作后的回调
			if(upserted != null)
				upserted(context);

			var result = context.Count;

			//处置上下文资源
			context.Dispose();

			//返回最终的结果
			return result;
		}

		public int UpsertMany<T>(IEnumerable<T> items)
		{
			if(items == null)
				return 0;

			return this.UpsertMany(this.GetName<T>(), items, string.Empty, null, null, null);
		}

		public int UpsertMany<T>(IEnumerable<T> items, object state)
		{
			if(items == null)
				return 0;

			return this.UpsertMany(this.GetName<T>(), items, string.Empty, state, null, null);
		}

		public int UpsertMany<T>(IEnumerable<T> items, string schema)
		{
			if(items == null)
				return 0;

			return this.UpsertMany(this.GetName<T>(), items, schema, null, null, null);
		}

		public int UpsertMany<T>(IEnumerable<T> items, string schema, object state, Func<DataUpsertContextBase, bool> upserting = null, Action<DataUpsertContextBase> upserted = null)
		{
			if(items == null)
				return 0;

			return this.UpsertMany(this.GetName<T>(), items, schema, state, upserting, upserted);
		}

		public int UpsertMany<T>(IEnumerable items)
		{
			if(items == null)
				return 0;

			return this.UpsertMany(this.GetName<T>(), items, string.Empty, null, null, null);
		}

		public int UpsertMany<T>(IEnumerable items, object state)
		{
			if(items == null)
				return 0;

			return this.UpsertMany(this.GetName<T>(), items, string.Empty, state, null, null);
		}

		public int UpsertMany<T>(IEnumerable items, string schema)
		{
			if(items == null)
				return 0;

			return this.UpsertMany(this.GetName<T>(), items, schema, null, null, null);
		}

		public int UpsertMany<T>(IEnumerable items, string schema, object state, Func<DataUpsertContextBase, bool> upserting = null, Action<DataUpsertContextBase> upserted = null)
		{
			if(items == null)
				return 0;

			return this.UpsertMany(this.GetName<T>(), items, schema, state, upserting, upserted);
		}

		public int UpsertMany(string name, IEnumerable items)
		{
			return this.UpsertMany(name, items, string.Empty, null, null, null);
		}

		public int UpsertMany(string name, IEnumerable items, object state)
		{
			return this.UpsertMany(name, items, string.Empty, state, null, null);
		}

		public int UpsertMany(string name, IEnumerable items, string schema)
		{
			return this.UpsertMany(name, items, schema, null, null, null);
		}

		public int UpsertMany(string name, IEnumerable items, string schema, object state, Func<DataUpsertContextBase, bool> upserting = null, Action<DataUpsertContextBase> upserted = null)
		{
			return this.UpsertMany(name, items, this.Schema.Parse(name, schema, Common.TypeExtension.GetElementType(items.GetType())), state, upserting, upserted);
		}

		public int UpsertMany(string name, IEnumerable items, ISchema schema, object state, Func<DataUpsertContextBase, bool> upserting = null, Action<DataUpsertContextBase> upserted = null)
		{
			if(string.IsNullOrEmpty(name))
				throw new ArgumentNullException(nameof(name));

			if(items == null)
				return 0;

			//创建数据访问上下文对象
			var context = this.CreateUpsertContext(name, true, items, schema, state);

			//处理数据访问操作前的回调
			if(upserting != null && upserting(context))
				return context.Count;

			//激发“Upserting”事件，如果被中断则返回
			if(this.OnUpserting(context))
				return context.Count;

			//调用数据访问过滤器前事件
			this.OnFiltering(context);

			//执行数据插入操作
			this.OnUpsert(context);

			//调用数据访问过滤器后事件
			this.OnFiltered(context);

			//激发“Upserted”事件
			this.OnUpserted(context);

			//处理数据访问操作后的回调
			if(upserted != null)
				upserted(context);

			var result = context.Count;

			//处置上下文资源
			context.Dispose();

			//返回最终的结果
			return result;
		}

		protected abstract void OnUpsert(DataUpsertContextBase context);
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

		public int Update<T>(T data, string schema)
		{
			if(data == null)
				return 0;

			return this.Update(this.GetName(data.GetType()), data, null, schema, null, null, null);
		}

		public int Update<T>(T data, string schema, object state)
		{
			if(data == null)
				return 0;

			return this.Update(this.GetName(data.GetType()), data, null, schema, state, null, null);
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

		public int Update<T>(T data, ICondition condition, string schema)
		{
			if(data == null)
				return 0;

			return this.Update(this.GetName(data.GetType()), data, condition, schema, null, null, null);
		}

		public int Update<T>(T data, ICondition condition, string schema, object state, Func<DataUpdateContextBase, bool> updating = null, Action<DataUpdateContextBase> updated = null)
		{
			if(data == null)
				return 0;

			return this.Update(this.GetName(data.GetType()), data, condition, schema, state, updating, updated);
		}

		public int Update<T>(object data)
		{
			if(data == null)
				return 0;

			return this.Update(this.GetName(typeof(T)), data, null, string.Empty, null, null, null);
		}

		public int Update<T>(object data, object state)
		{
			if(data == null)
				return 0;

			return this.Update(this.GetName(typeof(T)), data, null, string.Empty, state, null, null);
		}

		public int Update<T>(object data, string schema)
		{
			if(data == null)
				return 0;

			return this.Update(this.GetName(typeof(T)), data, null, schema, null, null, null);
		}

		public int Update<T>(object data, string schema, object state)
		{
			if(data == null)
				return 0;

			return this.Update(this.GetName(typeof(T)), data, null, schema, state, null, null);
		}

		public int Update<T>(object data, ICondition condition)
		{
			if(data == null)
				return 0;

			return this.Update(this.GetName(typeof(T)), data, condition, string.Empty, null, null, null);
		}

		public int Update<T>(object data, ICondition condition, object state)
		{
			if(data == null)
				return 0;

			return this.Update(this.GetName(typeof(T)), data, condition, string.Empty, state, null, null);
		}

		public int Update<T>(object data, ICondition condition, string schema)
		{
			if(data == null)
				return 0;

			return this.Update(this.GetName(typeof(T)), data, condition, schema, null, null, null);
		}

		public int Update<T>(object data, ICondition condition, string schema, object state, Func<DataUpdateContextBase, bool> updating = null, Action<DataUpdateContextBase> updated = null)
		{
			if(data == null)
				return 0;

			return this.Update(this.GetName(typeof(T)), data, condition, schema, state, updating, updated);
		}

		public int Update(string name, object data)
		{
			return this.Update(name, data, null, string.Empty, null, null, null);
		}

		public int Update(string name, object data, object state)
		{
			return this.Update(name, data, null, string.Empty, state, null, null);
		}

		public int Update(string name, object data, string schema)
		{
			return this.Update(name, data, null, schema, null, null, null);
		}

		public int Update(string name, object data, string schema, object state)
		{
			return this.Update(name, data, null, schema, state, null, null);
		}

		public int Update(string name, object data, ICondition condition)
		{
			return this.Update(name, data, condition, string.Empty, null, null, null);
		}

		public int Update(string name, object data, ICondition condition, object state)
		{
			return this.Update(name, data, condition, string.Empty, state, null, null);
		}

		public int Update(string name, object data, ICondition condition, string schema)
		{
			return this.Update(name, data, condition, schema, null, null, null);
		}

		public int Update(string name, object data, ICondition condition, string schema, object state, Func<DataUpdateContextBase, bool> updating = null, Action<DataUpdateContextBase> updated = null)
		{
			return this.Update(name, data, condition, this.Schema.Parse(name, schema, data.GetType()), state, updating, updated);
		}

		public int Update(string name, object data, ICondition condition, ISchema schema, object state, Func<DataUpdateContextBase, bool> updating = null, Action<DataUpdateContextBase> updated = null)
		{
			if(string.IsNullOrEmpty(name))
				throw new ArgumentNullException(nameof(name));

			if(data == null)
				return 0;

			//创建数据访问上下文对象
			var context = this.CreateUpdateContext(name, false, data, condition, schema, state);

			//处理数据访问操作前的回调
			if(updating != null && updating(context))
				return context.Count;

			//激发“Updating”事件，如果被中断则返回
			if(this.OnUpdating(context))
				return context.Count;

			//调用数据访问过滤器前事件
			this.OnFiltering(context);

			//执行数据更新操作
			this.OnUpdate(context);

			//调用数据访问过滤器后事件
			this.OnFiltered(context);

			//激发“Updated”事件
			this.OnUpdated(context);

			//处理数据访问操作后的回调
			if(updated != null)
				updated(context);

			var result = context.Count;

			//处置上下文资源
			context.Dispose();

			//返回最终的结果
			return result;
		}

		public int UpdateMany<T>(IEnumerable<T> items)
		{
			return this.UpdateMany(this.GetName<T>(), items, string.Empty, null, null, null);
		}

		public int UpdateMany<T>(IEnumerable<T> items, object state)
		{
			return this.UpdateMany(this.GetName<T>(), items, string.Empty, state, null, null);
		}

		public int UpdateMany<T>(IEnumerable<T> items, string schema)
		{
			return this.UpdateMany(this.GetName<T>(), items, schema, null, null, null);
		}

		public int UpdateMany<T>(IEnumerable<T> items, string schema, object state, Func<DataUpdateContextBase, bool> updating = null, Action<DataUpdateContextBase> updated = null)
		{
			return this.UpdateMany(this.GetName<T>(), items, schema, state, updating, updated);
		}

		public int UpdateMany<T>(IEnumerable items)
		{
			return this.UpdateMany(this.GetName<T>(), items, string.Empty, null, null, null);
		}

		public int UpdateMany<T>(IEnumerable items, object state)
		{
			return this.UpdateMany(this.GetName<T>(), items, string.Empty, state, null, null);
		}

		public int UpdateMany<T>(IEnumerable items, string schema)
		{
			return this.UpdateMany(this.GetName<T>(), items, schema, null, null, null);
		}

		public int UpdateMany<T>(IEnumerable items, string schema, object state, Func<DataUpdateContextBase, bool> updating = null, Action<DataUpdateContextBase> updated = null)
		{
			return this.UpdateMany(this.GetName<T>(), items, schema, state, updating, updated);
		}

		public int UpdateMany(string name, IEnumerable items)
		{
			return this.UpdateMany(name, items, string.Empty, null, null, null);
		}

		public int UpdateMany(string name, IEnumerable items, object state)
		{
			return this.UpdateMany(name, items, string.Empty, state, null, null);
		}

		public int UpdateMany(string name, IEnumerable items, string schema)
		{
			return this.UpdateMany(name, items, schema, null, null, null);
		}

		public int UpdateMany(string name, IEnumerable items, string schema, object state, Func<DataUpdateContextBase, bool> updating = null, Action<DataUpdateContextBase> updated = null)
		{
			return this.UpdateMany(name, items, this.Schema.Parse(name, schema, Common.TypeExtension.GetElementType(items.GetType())), state, updating, updated);
		}

		public int UpdateMany(string name, IEnumerable items, ISchema schema, object state, Func<DataUpdateContextBase, bool> updating = null, Action<DataUpdateContextBase> updated = null)
		{
			if(string.IsNullOrEmpty(name))
				throw new ArgumentNullException(nameof(name));

			if(items == null)
				return 0;

			//创建数据访问上下文对象
			var context = this.CreateUpdateContext(name, true, items, null, schema, state);

			//处理数据访问操作前的回调
			if(updating != null && updating(context))
				return context.Count;

			//激发“Updating”事件，如果被中断则返回
			if(this.OnUpdating(context))
				return context.Count;

			//调用数据访问过滤器前事件
			this.OnFiltering(context);

			//执行数据更新操作
			this.OnUpdate(context);

			//调用数据访问过滤器后事件
			this.OnFiltered(context);

			//激发“Updated”事件
			this.OnUpdated(context);

			//处理数据访问操作后的回调
			if(updated != null)
				updated(context);

			var result = context.Count;

			//处置上下文资源
			context.Dispose();

			//返回最终的结果
			return result;
		}

		protected abstract void OnUpdate(DataUpdateContextBase context);
		#endregion

		#region 普通查询
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
			return this.Select<T>(this.GetName<T>(), condition, string.Empty, paging, null, sortings, null, null);
		}

		public IEnumerable<T> Select<T>(ICondition condition, Paging paging, object state, params Sorting[] sortings)
		{
			return this.Select<T>(this.GetName<T>(), condition, string.Empty, paging, state, sortings, null, null);
		}

		public IEnumerable<T> Select<T>(ICondition condition, Paging paging, string schema, params Sorting[] sortings)
		{
			return this.Select<T>(this.GetName<T>(), condition, schema, paging, null, sortings, null, null);
		}

		public IEnumerable<T> Select<T>(ICondition condition, Paging paging, string schema, object state, params Sorting[] sortings)
		{
			return this.Select<T>(this.GetName<T>(), condition, schema, paging, state, sortings, null, null);
		}

		public IEnumerable<T> Select<T>(ICondition condition, string schema, params Sorting[] sortings)
		{
			return this.Select<T>(this.GetName<T>(), condition, schema, null, null, sortings, null, null);
		}

		public IEnumerable<T> Select<T>(ICondition condition, string schema, object state, params Sorting[] sortings)
		{
			return this.Select<T>(this.GetName<T>(), condition, schema, null, state, sortings, null, null);
		}

		public IEnumerable<T> Select<T>(ICondition condition, string schema, Paging paging, params Sorting[] sortings)
		{
			return this.Select<T>(this.GetName<T>(), condition, schema, paging, null, sortings, null, null);
		}

		public IEnumerable<T> Select<T>(ICondition condition, string schema, Paging paging, object state, params Sorting[] sortings)
		{
			return this.Select<T>(this.GetName<T>(), condition, schema, paging, state, sortings, null, null);
		}

		public IEnumerable<T> Select<T>(ICondition condition, string schema, Paging paging, object state, Sorting[] sortings, Func<DataSelectContextBase, bool> selecting, Action<DataSelectContextBase> selected)
		{
			return this.Select<T>(this.GetName<T>(), condition, schema, paging, state, sortings, selecting, selected);
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

		public IEnumerable<T> Select<T>(string name, ICondition condition, Paging paging, string schema, params Sorting[] sortings)
		{
			return this.Select<T>(name, condition, schema, paging, null, sortings, null, null);
		}

		public IEnumerable<T> Select<T>(string name, ICondition condition, Paging paging, string schema, object state, params Sorting[] sortings)
		{
			return this.Select<T>(name, condition, schema, paging, state, sortings, null, null);
		}

		public IEnumerable<T> Select<T>(string name, ICondition condition, string schema, params Sorting[] sortings)
		{
			return this.Select<T>(name, condition, schema, null, null, sortings, null, null);
		}

		public IEnumerable<T> Select<T>(string name, ICondition condition, string schema, object state, params Sorting[] sortings)
		{
			return this.Select<T>(name, condition, schema, null, state, sortings, null, null);
		}

		public IEnumerable<T> Select<T>(string name, ICondition condition, string schema, Paging paging, params Sorting[] sortings)
		{
			return this.Select<T>(name, condition, schema, paging, null, sortings, null, null);
		}

		public IEnumerable<T> Select<T>(string name, ICondition condition, string schema, Paging paging, object state, params Sorting[] sortings)
		{
			return this.Select<T>(name, condition, schema, paging, state, sortings, null, null);
		}

		public IEnumerable<T> Select<T>(string name, ICondition condition, string schema, Paging paging, object state, Sorting[] sortings, Func<DataSelectContextBase, bool> selecting, Action<DataSelectContextBase> selected)
		{
			return this.Select<T>(name, condition, this.Schema.Parse(name, schema, typeof(T)), paging, state, sortings, selecting, selected);
		}

		public IEnumerable<T> Select<T>(string name, ICondition condition, ISchema schema, Paging paging, object state, Sorting[] sortings, Func<DataSelectContextBase, bool> selecting, Action<DataSelectContextBase> selected)
		{
			if(string.IsNullOrEmpty(name))
				throw new ArgumentNullException(nameof(name));

			//创建数据访问上下文对象
			var context = this.CreateSelectContext(name, typeof(T), condition, null, schema, paging, sortings, state);

			//执行查询方法
			return this.Select<T>(context, selecting, selected);
		}
		#endregion

		#region 分组查询
		public IEnumerable<T> Select<T>(string name, Grouping grouping, params Sorting[] sortings)
		{
			return this.Select<T>(name, grouping, null, string.Empty, null, null, sortings, null, null);
		}

		public IEnumerable<T> Select<T>(string name, Grouping grouping, object state, params Sorting[] sortings)
		{
			return this.Select<T>(name, grouping, null, string.Empty, null, state, sortings, null, null);
		}

		public IEnumerable<T> Select<T>(string name, Grouping grouping, Paging paging, object state = null, params Sorting[] sortings)
		{
			return this.Select<T>(name, grouping, null, string.Empty, paging, state, sortings, null, null);
		}

		public IEnumerable<T> Select<T>(string name, Grouping grouping, string schema, params Sorting[] sortings)
		{
			return this.Select<T>(name, grouping, null, schema, null, null, sortings, null, null);
		}

		public IEnumerable<T> Select<T>(string name, Grouping grouping, string schema, object state, params Sorting[] sortings)
		{
			return this.Select<T>(name, grouping, null, schema, null, state, sortings, null, null);
		}

		public IEnumerable<T> Select<T>(string name, Grouping grouping, string schema, Paging paging, object state = null, params Sorting[] sortings)
		{
			return this.Select<T>(name, grouping, null, schema, paging, state, sortings, null, null);
		}

		public IEnumerable<T> Select<T>(string name, Grouping grouping, ICondition condition, string schema, params Sorting[] sortings)
		{
			return this.Select<T>(name, grouping, condition, schema, null, null, sortings, null, null);
		}

		public IEnumerable<T> Select<T>(string name, Grouping grouping, ICondition condition, string schema, object state, params Sorting[] sortings)
		{
			return this.Select<T>(name, grouping, condition, schema, null, state, sortings, null, null);
		}

		public IEnumerable<T> Select<T>(string name, Grouping grouping, ICondition condition, string schema, Paging paging, object state = null, params Sorting[] sortings)
		{
			return this.Select<T>(name, grouping, condition, schema, paging, state, sortings, null, null);
		}

		public IEnumerable<T> Select<T>(string name, Grouping grouping, ICondition condition, string schema, Paging paging, object state, Sorting[] sortings, Func<DataSelectContextBase, bool> selecting, Action<DataSelectContextBase> selected)
		{
			return this.Select<T>(name, grouping, condition, string.IsNullOrWhiteSpace(schema) ? null : this.Schema.Parse(name, schema, typeof(T)), paging, state, sortings, selecting, selected);
		}

		public IEnumerable<T> Select<T>(string name, Grouping grouping, ICondition condition, ISchema schema, Paging paging, object state, Sorting[] sortings, Func<DataSelectContextBase, bool> selecting, Action<DataSelectContextBase> selected)
		{
			if(string.IsNullOrEmpty(name))
				throw new ArgumentNullException(nameof(name));

			//创建数据访问上下文对象
			var context = this.CreateSelectContext(name, typeof(T), condition, grouping, schema, paging, sortings, state);

			//执行查询方法
			return this.Select<T>(context, selecting, selected);
		}

		private IEnumerable<T> Select<T>(DataSelectContextBase context, Func<DataSelectContextBase, bool> selecting, Action<DataSelectContextBase> selected)
		{
			//处理数据访问操作前的回调
			if(selecting != null && selecting(context))
				return context.Result as IEnumerable<T>;

			//激发“Selecting”事件，如果被中断则返回
			if(this.OnSelecting(context))
				return context.Result as IEnumerable<T>;

			//调用数据访问过滤器前事件
			this.OnFiltering(context);

			//执行数据查询操作
			this.OnSelect(context);

			//调用数据访问过滤器后事件
			this.OnFiltered(context);

			//激发“Selected”事件
			this.OnSelected(context);

			//处理数据访问操作后的回调
			if(selected != null)
				selected(context);

			var result = this.ToEnumerable<T>(context.Result);

			//处置上下文资源
			context.Dispose();

			//返回最终的结果
			return result;
		}

		protected abstract void OnSelect(DataSelectContextBase context);
		#endregion

		#region 虚拟方法
		protected virtual string GetName(Type type)
		{
			var name = _naming.Get(type);

			if(string.IsNullOrEmpty(name))
				throw new InvalidOperationException($"Missing data access name mapping of the '{type.FullName}' type.");

			return name;
		}

		protected virtual Common.ISequence CreateSequence(Common.ISequence sequence)
		{
			return sequence;
		}

		protected virtual void OnFiltering(IDataAccessContextBase context)
		{
			_filters.InvokeFiltering(context);
		}

		protected virtual void OnFiltered(IDataAccessContextBase context)
		{
			_filters.InvokeFiltered(context);
		}
		#endregion

		#region 抽象方法
		protected abstract ISchemaParser CreateSchema();
		protected abstract DataCountContextBase CreateCountContext(string name, ICondition condition, string member, object state);
		protected abstract DataExistContextBase CreateExistContext(string name, ICondition condition, object state);
		protected abstract DataExecuteContextBase CreateExecuteContext(string name, bool isScalar, Type resultType, IDictionary<string, object> inParameters, object state);
		protected abstract DataIncrementContextBase CreateIncrementContext(string name, string member, ICondition condition, int interval, object state);
		protected abstract DataDeleteContextBase CreateDeleteContext(string name, ICondition condition, ISchema schema, object state);
		protected abstract DataInsertContextBase CreateInsertContext(string name, bool isMultiple, object data, ISchema schema, object state);
		protected abstract DataUpsertContextBase CreateUpsertContext(string name, bool isMultiple, object data, ISchema schema, object state);
		protected abstract DataUpdateContextBase CreateUpdateContext(string name, bool isMultiple, object data, ICondition condition, ISchema schema, object state);
		protected abstract DataSelectContextBase CreateSelectContext(string name, Type entityType, ICondition condition, Grouping grouping, ISchema schema, Paging paging, Sorting[] sortings, object state);
		#endregion

		#region 激发事件
		protected virtual void OnError(DataAccessErrorEventArgs args)
		{
			this.Error?.Invoke(this, args);
		}

		protected virtual void OnCounted(DataCountContextBase context)
		{
			this.Counted?.Invoke(this, new DataCountedEventArgs(context));
		}

		protected virtual bool OnCounting(DataCountContextBase context)
		{
			var e = this.Counting;

			if(e == null)
				return false;

			var args = new DataCountingEventArgs(context);
			e(this, args);
			return args.Cancel;
		}

		protected virtual void OnExecuted(DataExecuteContextBase context)
		{
			this.Executed?.Invoke(this, new DataExecutedEventArgs(context));
		}

		protected virtual bool OnExecuting(DataExecuteContextBase context)
		{
			var e = this.Executing;

			if(e == null)
				return false;

			var args = new DataExecutingEventArgs(context);
			e(this, args);
			return args.Cancel;
		}

		protected virtual void OnExisted(DataExistContextBase context)
		{
			this.Existed?.Invoke(this, new DataExistedEventArgs(context));
		}

		protected virtual bool OnExisting(DataExistContextBase context)
		{
			var e = this.Existing;

			if(e == null)
				return false;

			var args = new DataExistingEventArgs(context);
			e(this, args);
			return args.Cancel;
		}

		protected virtual void OnIncremented(DataIncrementContextBase context)
		{
			this.Incremented?.Invoke(this, new DataIncrementedEventArgs(context));
		}

		protected virtual bool OnIncrementing(DataIncrementContextBase context)
		{
			var e = this.Incrementing;

			if(e == null)
				return false;

			var args = new DataIncrementingEventArgs(context);
			e(this, args);
			return args.Cancel;
		}

		protected virtual void OnDeleted(DataDeleteContextBase context)
		{
			this.Deleted?.Invoke(this, new DataDeletedEventArgs(context));
		}

		protected virtual bool OnDeleting(DataDeleteContextBase context)
		{
			var e = this.Deleting;

			if(e == null)
				return false;

			var args = new DataDeletingEventArgs(context);
			e(this, args);
			return args.Cancel;
		}

		protected virtual void OnInserted(DataInsertContextBase context)
		{
			this.Inserted?.Invoke(this, new DataInsertedEventArgs(context));
		}

		protected virtual bool OnInserting(DataInsertContextBase context)
		{
			var e = this.Inserting;

			if(e == null)
				return false;

			var args = new DataInsertingEventArgs(context);
			e(this, args);
			return args.Cancel;
		}

		protected virtual void OnUpserted(DataUpsertContextBase context)
		{
			this.Upserted?.Invoke(this, new DataUpsertedEventArgs(context));
		}

		protected virtual bool OnUpserting(DataUpsertContextBase context)
		{
			var e = this.Upserting;

			if(e == null)
				return false;

			var args = new DataUpsertingEventArgs(context);
			e(this, args);
			return args.Cancel;
		}

		protected virtual void OnUpdated(DataUpdateContextBase context)
		{
			this.Updated?.Invoke(this, new DataUpdatedEventArgs(context));
		}

		protected virtual bool OnUpdating(DataUpdateContextBase context)
		{
			var e = this.Updating;

			if(e == null)
				return false;

			var args = new DataUpdatingEventArgs(context);
			e(this, args);
			return args.Cancel;
		}

		protected virtual void OnSelected(DataSelectContextBase context)
		{
			this.Selected?.Invoke(this, new DataSelectedEventArgs(context));
		}

		protected virtual bool OnSelecting(DataSelectContextBase context)
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
		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
		private string GetName<T>()
		{
			return this.GetName(typeof(T));
		}

		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
		private IEnumerable<T> ToEnumerable<T>(object result)
		{
			return Collections.Enumerable.Enumerate<T>(result);
		}
		#endregion

		#region 嵌套子类
		private class FilterCollection : ICollection<IDataAccessFilter>
		{
			#region 私有变量
			private readonly ISet<IDataAccessFilter> _items;
			private readonly IDictionary<DataAccessMethod, ICollection<IDataAccessFilter>> _globals;
			private readonly IDictionary<string, FilterToken> _tokens;
			#endregion

			#region 构造函数
			public FilterCollection()
			{
				_items = new HashSet<IDataAccessFilter>();
				_globals = new Dictionary<DataAccessMethod, ICollection<IDataAccessFilter>>();
				_tokens = new Dictionary<string, FilterToken>(StringComparer.OrdinalIgnoreCase);
			}
			#endregion

			#region 公共属性
			public int Count
			{
				get => _items.Count;
			}

			public bool IsReadOnly
			{
				get => false;
			}
			#endregion

			#region 公共方法
			public void InvokeFiltering(IDataAccessContextBase context)
			{
				if(_globals != null && _globals.Count > 0)
				{
					if(_globals.TryGetValue(context.Method, out var filters) && filters != null && filters.Count > 0)
					{
						foreach(var filter in filters)
							filter.OnFiltering(context);
					}
				}

				if(_tokens != null && _tokens.Count > 0)
				{
					if(_tokens.TryGetValue(context.Name, out var token) && token.TryGetFilters(context.Method, out var filters))
					{
						foreach(var filter in filters)
							filter.OnFiltering(context);
					}
				}
			}

			public void InvokeFiltered(IDataAccessContextBase context)
			{
				if(_tokens != null && _tokens.Count > 0)
				{
					if(_tokens.TryGetValue(context.Name, out var token) && token.TryGetFilters(context.Method, out var filters))
					{
						foreach(var filter in filters)
							filter.OnFiltered(context);
					}
				}

				if(_globals != null && _globals.Count > 0)
				{
					if(_globals.TryGetValue(context.Method, out var filters) && filters != null && filters.Count > 0)
					{
						foreach(var filter in filters)
							filter.OnFiltered(context);
					}
				}
			}

			public void Add(IDataAccessFilter item)
			{
				if(item == null)
					throw new ArgumentNullException(nameof(item));

				if(!_items.Add(item))
					return;

				if(string.IsNullOrWhiteSpace(item.Name))
				{
					foreach(var method in item.Methods)
					{
						if(_globals.TryGetValue(method, out var filters))
							filters.Add(item);
						else
							_globals.Add(method, new List<IDataAccessFilter>(new[] { item }));
					}
				}
				else
				{
					if(_tokens.TryGetValue(item.Name, out var token))
						token.Add(item);
					else
						_tokens.Add(item.Name, new FilterToken(item));
				}
			}

			public void Clear()
			{
				_items.Clear();
				_globals.Clear();
				_tokens.Clear();
			}

			public bool Remove(IDataAccessFilter item)
			{
				if(item == null)
					return false;

				if(!_items.Remove(item))
					return false;

				if(string.IsNullOrWhiteSpace(item.Name))
				{
					if(item.Methods == null || item.Methods.Length == 0)
					{
						foreach(var global in _globals.Values)
						{
							global.Remove(item);
						}
					}
					else
					{
						for(int i = 0; i < item.Methods.Length; i++)
						{
							if(_globals.TryGetValue(item.Methods[i], out var global))
							{
								global.Remove(item);
							}
						}
					}
				}
				else
				{
					if(_tokens.TryGetValue(item.Name, out var token))
					{
						token.Remove(item);
						return true;
					}
				}

				return false;
			}

			public bool Contains(IDataAccessFilter item)
			{
				if(item == null)
					return false;

				return _items.Contains(item);
			}

			public void CopyTo(IDataAccessFilter[] array, int arrayIndex)
			{
				if(array == null)
					throw new ArgumentNullException(nameof(array));

				_items.CopyTo(array, arrayIndex);
			}
			#endregion

			#region 遍历枚举
			public IEnumerator<IDataAccessFilter> GetEnumerator()
			{
				return _items.GetEnumerator();
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return _items.GetEnumerator();
			}
			#endregion

			private class FilterToken
			{
				#region 成员字段
				private ICollection<IDataAccessFilter> _counts;
				private ICollection<IDataAccessFilter> _exists;
				private ICollection<IDataAccessFilter> _selects;
				private ICollection<IDataAccessFilter> _deletes;
				private ICollection<IDataAccessFilter> _inserts;
				private ICollection<IDataAccessFilter> _updates;
				private ICollection<IDataAccessFilter> _upserts;
				private ICollection<IDataAccessFilter> _executes;
				private ICollection<IDataAccessFilter> _increments;
				#endregion

				#region 构造函数
				public FilterToken(IDataAccessFilter filter)
				{
					this.Add(filter);
				}
				#endregion

				#region 私有属性
				private ICollection<IDataAccessFilter> Counts
				{
					get => this.EnsureFilters(ref _counts);
				}

				private ICollection<IDataAccessFilter> Exists
				{
					get => this.EnsureFilters(ref _exists);
				}

				private ICollection<IDataAccessFilter> Selects
				{
					get => this.EnsureFilters(ref _selects);
				}

				private ICollection<IDataAccessFilter> Deletes
				{
					get => this.EnsureFilters(ref _deletes);
				}

				private ICollection<IDataAccessFilter> Inserts
				{
					get => this.EnsureFilters(ref _inserts);
				}

				private ICollection<IDataAccessFilter> Updates
				{
					get => this.EnsureFilters(ref _updates);
				}

				private ICollection<IDataAccessFilter> Upserts
				{
					get => this.EnsureFilters(ref _upserts);
				}

				private ICollection<IDataAccessFilter> Executes
				{
					get => this.EnsureFilters(ref _executes);
				}

				private ICollection<IDataAccessFilter> Increments
				{
					get => this.EnsureFilters(ref _increments);
				}
				#endregion

				#region 公共方法
				public void Add(IDataAccessFilter filter)
				{
					if(filter.Methods == null || filter.Methods.Length == 0)
					{
						this.Counts.Add(filter);
						this.Exists.Add(filter);
						this.Selects.Add(filter);
						this.Deletes.Add(filter);
						this.Inserts.Add(filter);
						this.Updates.Add(filter);
						this.Upserts.Add(filter);
						this.Executes.Add(filter);
						this.Increments.Add(filter);
					}
					else
					{
						for(int i = 0; i < filter.Methods.Length; i++)
						{
							switch(filter.Methods[i])
							{
								case DataAccessMethod.Count:
									this.Counts.Add(filter);
									break;
								case DataAccessMethod.Exists:
									this.Exists.Add(filter);
									break;
								case DataAccessMethod.Select:
									this.Selects.Add(filter);
									break;
								case DataAccessMethod.Delete:
									this.Deletes.Add(filter);
									break;
								case DataAccessMethod.Insert:
									this.Inserts.Add(filter);
									break;
								case DataAccessMethod.Update:
									this.Updates.Add(filter);
									break;
								case DataAccessMethod.Upsert:
									this.Upserts.Add(filter);
									break;
								case DataAccessMethod.Execute:
									this.Executes.Add(filter);
									break;
								case DataAccessMethod.Increment:
									this.Increments.Add(filter);
									break;
							}
						}
					}
				}

				public void Remove(IDataAccessFilter filter)
				{
					if(filter.Methods == null || filter.Methods.Length == 0)
					{
						_counts?.Remove(filter);
						_exists?.Remove(filter);
						_selects?.Remove(filter);
						_deletes?.Remove(filter);
						_inserts?.Remove(filter);
						_updates?.Remove(filter);
						_upserts?.Remove(filter);
						_executes?.Remove(filter);
						_increments?.Remove(filter);
					}
					else
					{
						for(int i = 0; i < filter.Methods.Length; i++)
						{
							switch(filter.Methods[i])
							{
								case DataAccessMethod.Count:
									_counts?.Remove(filter);
									break;
								case DataAccessMethod.Exists:
									_exists?.Remove(filter);
									break;
								case DataAccessMethod.Select:
									_selects?.Remove(filter);
									break;
								case DataAccessMethod.Delete:
									_deletes?.Remove(filter);
									break;
								case DataAccessMethod.Insert:
									_inserts?.Remove(filter);
									break;
								case DataAccessMethod.Update:
									_updates?.Remove(filter);
									break;
								case DataAccessMethod.Upsert:
									_upserts?.Remove(filter);
									break;
								case DataAccessMethod.Execute:
									_executes?.Remove(filter);
									break;
								case DataAccessMethod.Increment:
									_increments?.Remove(filter);
									break;
							}
						}
					}
				}

				public bool TryGetFilters(DataAccessMethod method, out ICollection<IDataAccessFilter> filters)
				{
					filters = null;

					switch(method)
					{
						case DataAccessMethod.Count:
							filters = _counts;
							break;
						case DataAccessMethod.Exists:
							filters = _exists;
							break;
						case DataAccessMethod.Select:
							filters = _selects;
							break;
						case DataAccessMethod.Delete:
							filters = _deletes;
							break;
						case DataAccessMethod.Insert:
							filters = _inserts;
							break;
						case DataAccessMethod.Update:
							filters = _updates;
							break;
						case DataAccessMethod.Upsert:
							filters = _upserts;
							break;
						case DataAccessMethod.Execute:
							filters = _executes;
							break;
						case DataAccessMethod.Increment:
							filters = _increments;
							break;
					}

					return filters != null && filters.Count > 0;
				}
				#endregion

				#region 私有方法
				[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
				private ICollection<IDataAccessFilter> EnsureFilters(ref ICollection<IDataAccessFilter> filters)
				{
					if(filters == null)
					{
						lock(this)
						{
							if(filters == null)
								filters = new List<IDataAccessFilter>();
						}
					}

					return filters;
				}
				#endregion
			}
		}
		#endregion
	}
}

/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2016-2017 Zongsoft Corporation <http://www.zongsoft.com>
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
using System.Collections.Concurrent;

namespace Zongsoft.Data
{
	public class DataService<TEntity> : IDataService<TEntity>
	{
		#region 事件定义
		public event EventHandler<DataGettedEventArgs<TEntity>> Getted;
		public event EventHandler<DataGettingEventArgs<TEntity>> Getting;

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
		private IDataAccess _dataAccess;
		private Zongsoft.Services.IServiceProvider _serviceProvider;
		private DataSearchKeyAttribute _searchKey;
		#endregion

		#region 构造函数
		public DataService(Zongsoft.Services.IServiceProvider serviceProvider)
		{
			if(serviceProvider == null)
				throw new ArgumentNullException("serviceProvider");

			_serviceProvider = serviceProvider;
			_dataAccess = serviceProvider.ResolveRequired<IDataAccess>();

			//获取当前数据搜索键
			_searchKey = (DataSearchKeyAttribute)Attribute.GetCustomAttribute(this.GetType(), typeof(DataSearchKeyAttribute), true);

			//注册数据递增键序列号
			DataSequence.Register(this);
		}

		public DataService(string name, Zongsoft.Services.IServiceProvider serviceProvider)
		{
			if(string.IsNullOrWhiteSpace(name))
				throw new ArgumentNullException("name");
			if(serviceProvider == null)
				throw new ArgumentNullException("serviceProvider");

			_name = name.Trim();
			_serviceProvider = serviceProvider;
			_dataAccess = serviceProvider.ResolveRequired<IDataAccess>();

			//获取当前数据搜索键
			_searchKey = (DataSearchKeyAttribute)Attribute.GetCustomAttribute(this.GetType(), typeof(DataSearchKeyAttribute), true);

			//注册数据递增键序列号
			DataSequence.Register(this);
		}
		#endregion

		#region 公共属性
		public string Name
		{
			get
			{
				if(string.IsNullOrWhiteSpace(_name))
				{
					var dataAccess = this.DataAccess;

					if(dataAccess != null && dataAccess.Naming != null)
						_name = dataAccess.Naming.Get<TEntity>();
				}

				return _name;
			}
			protected set
			{
				if(string.IsNullOrWhiteSpace(value))
					throw new ArgumentNullException();

				_name = value.Trim();
			}
		}

		public IDataAccess DataAccess
		{
			get
			{
				return _dataAccess;
			}
			set
			{
				if(value == null)
					throw new ArgumentNullException();

				_dataAccess = value;
			}
		}

		public Zongsoft.Services.IServiceProvider ServiceProvider
		{
			get
			{
				return _serviceProvider;
			}
			set
			{
				if(value == null)
					throw new ArgumentNullException();

				_serviceProvider = value;
			}
		}
		#endregion

		#region 保护属性
		protected virtual Zongsoft.Security.CredentialPrincipal Principal
		{
			get
			{
				return Zongsoft.ComponentModel.ApplicationContextBase.Current.Principal as Zongsoft.Security.CredentialPrincipal;
			}
		}
		#endregion

		#region 执行方法
		public IEnumerable<T> Execute<T>(string name, IDictionary<string, object> inParameters)
		{
			IDictionary<string, object> outParameters;
			return this.Execute<T>(name, inParameters, out outParameters);
		}

		public virtual IEnumerable<T> Execute<T>(string name, IDictionary<string, object> inParameters, out IDictionary<string, object> outParameters)
		{
			return this.DataAccess.Execute<T>(name, inParameters, out outParameters, ctx => this.OnExecuting(ctx), ctx => this.OnExecuted(ctx));
		}

		public object ExecuteScalar(string name, IDictionary<string, object> inParameters)
		{
			IDictionary<string, object> outParameters;
			return this.ExecuteScalar(name, inParameters, out outParameters);
		}

		public virtual object ExecuteScalar(string name, IDictionary<string, object> inParameters, out IDictionary<string, object> outParameters)
		{
			return this.DataAccess.ExecuteScalar(name, inParameters, out outParameters, ctx => this.OnExecuting(ctx), ctx => this.OnExecuted(ctx));
		}
		#endregion

		#region 存在方法
		public virtual bool Exists(ICondition condition)
		{
			return this.DataAccess.Exists(this.Name, condition, ctx => this.OnExisting(ctx), ctx => this.OnExisted(ctx));
		}

		public virtual bool Exists<TKey>(TKey key)
		{
			bool singleton;
			return this.Exists(this.ConvertKey(key, out singleton));
		}

		public virtual bool Exists<TKey1, TKey2>(TKey1 key1, TKey2 key2)
		{
			bool singleton;
			return this.Exists(this.ConvertKey(key1, key2, out singleton));
		}

		public virtual bool Exists<TKey1, TKey2, TKey3>(TKey1 key1, TKey2 key2, TKey3 key3)
		{
			bool singleton;
			return this.Exists(this.ConvertKey(key1, key2, key3, out singleton));
		}
		#endregion

		#region 计数方法
		public virtual int Count(ICondition condition, string includes = null)
		{
			return this.DataAccess.Count(this.Name, condition, includes, ctx => this.OnCounting(ctx), ctx => this.OnCounted(ctx));
		}
		#endregion

		#region 递增方法
		public virtual long Increment(string member, ICondition condition, int interval = 1)
		{
			return this.DataAccess.Increment(this.Name, member, condition, interval, ctx => this.OnIncrementing(ctx), ctx => this.OnIncremented(ctx));
		}

		public long Decrement(string member, ICondition condition, int interval = 1)
		{
			return this.DataAccess.Decrement(this.Name, member, condition, interval, ctx => this.OnIncrementing(ctx), ctx => this.OnIncremented(ctx));
		}
		#endregion

		#region 删除方法
		public virtual int Delete<TKey>(TKey key, params string[] cascades)
		{
			bool singleton;
			return this.Delete(this.ConvertKey(key, out singleton), cascades);
		}

		public virtual int Delete<TKey1, TKey2>(TKey1 key1, TKey2 key2, params string[] cascades)
		{
			bool singleton;
			return this.Delete(this.ConvertKey(key1, key2, out singleton), cascades);
		}

		public virtual int Delete<TKey1, TKey2, TKey3>(TKey1 key1, TKey2 key2, TKey3 key3, params string[] cascades)
		{
			bool singleton;
			return this.Delete(this.ConvertKey(key1, key2, key3, out singleton), cascades);
		}

		public int Delete(ICondition condition, params string[] cascades)
		{
			return this.OnDelete(condition, cascades);
		}

		protected virtual int OnDelete(ICondition condition, string[] cascades)
		{
			if(condition == null)
				throw new NotSupportedException("The condition cann't is null on delete operation.");

			return this.DataAccess.Delete(this.Name, condition, cascades, ctx => this.OnDeleting(ctx), ctx => this.OnDeleted(ctx));
		}
		#endregion

		#region 插入方法
		public int Insert(object data, string scope = null)
		{
			if(data == null)
				return 0;

			return this.OnInsert(DataDictionary<TEntity>.GetDataDictionary(data), scope);
		}

		public int InsertMany(IEnumerable data, string scope = null)
		{
			if(data == null)
				return 0;

			return this.OnInsertMany(DataDictionary<TEntity>.GetDataDictionaries(data), scope);
		}

		protected virtual int OnInsert(DataDictionary<TEntity> data, string scope)
		{
			if(data == null || data.Data == null)
				return 0;

			//执行数据引擎的插入操作
			return this.DataAccess.Insert(this.Name, data, scope, ctx => this.OnInserting(ctx), ctx => this.OnInserted(ctx));
		}

		protected virtual int OnInsertMany(IEnumerable<DataDictionary<TEntity>> items, string scope)
		{
			if(items == null)
				return 0;

			//执行数据引擎的插入操作
			return this.DataAccess.InsertMany(this.Name, items, scope, ctx => this.OnInserting(ctx), ctx => this.OnInserted(ctx));
		}
		#endregion

		#region 更新方法
		public virtual int Update<TKey>(object data, TKey key, string scope = null)
		{
			bool singleton;
			return this.Update(data, this.ConvertKey(key, out singleton), scope);
		}

		public virtual int Update<TKey1, TKey2>(object data, TKey1 key1, TKey2 key2, string scope = null)
		{
			bool singleton;
			return this.Update(data, this.ConvertKey(key1, key2, out singleton), scope);
		}

		public virtual int Update<TKey1, TKey2, TKey3>(object data, TKey1 key1, TKey2 key2, TKey3 key3, string scope = null)
		{
			bool singleton;
			return this.Update(data, this.ConvertKey(key1, key2, key3, out singleton), scope);
		}

		public virtual int UpdateMany<TKey>(IEnumerable data, TKey key, string scope = null)
		{
			bool singleton;
			return this.UpdateMany(data, this.ConvertKey(key, out singleton), scope);
		}

		public virtual int UpdateMany<TKey1, TKey2>(IEnumerable data, TKey1 key1, TKey2 key2, string scope = null)
		{
			bool singleton;
			return this.UpdateMany(data, this.ConvertKey(key1, key2, out singleton), scope);
		}

		public virtual int UpdateMany<TKey1, TKey2, TKey3>(IEnumerable data, TKey1 key1, TKey2 key2, TKey3 key3, string scope = null)
		{
			bool singleton;
			return this.UpdateMany(data, this.ConvertKey(key1, key2, key3, out singleton), scope);
		}

		public int Update(object data, ICondition condition = null, string scope = null)
		{
			if(data == null)
				return 0;

			return this.OnUpdate(DataDictionary<TEntity>.GetDataDictionary(data), condition, scope);
		}

		public int Update(object data, string scope, ICondition condition = null)
		{
			return this.Update(data, condition, scope);
		}

		public int UpdateMany(IEnumerable data, ICondition condition = null, string scope = null)
		{
			if(data == null)
				return 0;

			return this.OnUpdateMany(DataDictionary<TEntity>.GetDataDictionaries(data), condition, scope);
		}

		public int UpdateMany(IEnumerable data, string scope, ICondition condition = null)
		{
			return this.UpdateMany(data, condition, scope);
		}

		protected virtual int OnUpdate(DataDictionary<TEntity> data, ICondition condition, string scope)
		{
			if(data == null || data.Data == null)
				return 0;

			return this.DataAccess.Update(this.Name, data, condition, scope, ctx => this.OnUpdating(ctx), ctx => this.OnUpdated(ctx));
		}

		protected virtual int OnUpdateMany(IEnumerable<DataDictionary<TEntity>> items, ICondition condition, string scope)
		{
			if(items == null)
				return 0;

			return this.DataAccess.UpdateMany(this.Name, items, condition, scope, ctx => this.OnUpdating(ctx), ctx => this.OnUpdated(ctx));
		}
		#endregion

		#region 查询方法
		public object Search(string keyword, params Sorting[] sortings)
		{
			return this.Search(keyword, string.Empty, null, sortings);
		}

		public virtual object Search(string keyword, string scope, Paging paging = null, params Sorting[] sortings)
		{
			bool singleton;

			//获取搜索条件和搜索结果是否为单条数据
			var condition = this.GetKey(keyword, out singleton);

			if(condition == null)
				throw new ArgumentException($"The {this.Name} service does not supportd search operation or specified search key is invalid.");

			if(singleton)
				return this.GetSingle(condition, scope);
			else
				return this.Select(condition, scope, paging, sortings);
		}

		public object Search(string keyword, Paging paging, string scope = null, params Sorting[] sortings)
		{
			return this.Search(keyword, scope, paging, sortings);
		}

		public object Get<TKey>(TKey key, params Sorting[] sortings)
		{
			return this.Get(key, string.Empty, null, sortings);
		}

		public virtual object Get<TKey>(TKey key, string scope, Paging paging = null, params Sorting[] sortings)
		{
			bool singleton;
			var condition = this.ConvertKey(key, out singleton);

			if(singleton)
				return this.GetSingle(condition, scope);
			else
				return this.Select(condition, scope, paging, sortings);
		}

		public object Get<TKey>(TKey key, Paging paging, string scope = null, params Sorting[] sortings)
		{
			return this.Get(key, scope, paging, sortings);
		}

		public object Get<TKey1, TKey2>(TKey1 key1, TKey2 key2, params Sorting[] sortings)
		{
			return this.Get(key1, key2, string.Empty, null, sortings);
		}

		public virtual object Get<TKey1, TKey2>(TKey1 key1, TKey2 key2, string scope, Paging paging = null, params Sorting[] sortings)
		{
			bool singleton;
			var condition = this.ConvertKey(key1, key2, out singleton);

			if(singleton)
				return this.GetSingle(condition, scope);
			else
				return this.Select(condition, scope, paging, sortings);
		}

		public object Get<TKey1, TKey2>(TKey1 key1, TKey2 key2, Paging paging, string scope = null, params Sorting[] sortings)
		{
			return this.Get(key1, key2, scope, paging, sortings);
		}

		public object Get<TKey1, TKey2, TKey3>(TKey1 key1, TKey2 key2, TKey3 key3, params Sorting[] sortings)
		{
			return this.Get(key1, key2, key3, string.Empty, null, sortings);
		}

		public virtual object Get<TKey1, TKey2, TKey3>(TKey1 key1, TKey2 key2, TKey3 key3, string scope, Paging paging = null, params Sorting[] sortings)
		{
			bool singleton;
			var condition = this.ConvertKey(key1, key2, key3, out singleton);

			if(singleton)
				return this.GetSingle(condition, scope);
			else
				return this.Select(condition, scope, paging, sortings);
		}

		public object Get<TKey1, TKey2, TKey3>(TKey1 key1, TKey2 key2, TKey3 key3, Paging paging, string scope = null, params Sorting[] sortings)
		{
			return this.Get(key1, key2, key3, scope, paging, sortings);
		}

		private TEntity GetSingle(ICondition condition, string scope)
		{
			return this.OnGet(condition, scope);
		}

		protected virtual TEntity OnGet(ICondition condition, string scope)
		{
			return this.DataAccess.Select<TEntity>(this.Name, condition, null, scope, null, null, ctx => this.OnGetting(ctx), ctx => this.OnGetted(ctx)).FirstOrDefault();
		}

		public IEnumerable<TEntity> Select(ICondition condition = null, params Sorting[] sortings)
		{
			return this.Select(condition, null, string.Empty, null, sortings);
		}

		public IEnumerable<TEntity> Select(ICondition condition, Paging paging, params Sorting[] sortings)
		{
			return this.Select(condition, null, string.Empty, paging, sortings);
		}

		public IEnumerable<TEntity> Select(ICondition condition, Paging paging, string scope, params Sorting[] sortings)
		{
			return this.Select(condition, null, scope, paging, sortings);
		}

		public IEnumerable<TEntity> Select(ICondition condition, string scope, params Sorting[] sortings)
		{
			return this.Select(condition, null, scope, null, sortings);
		}

		public IEnumerable<TEntity> Select(ICondition condition, string scope, Paging paging, params Sorting[] sortings)
		{
			return this.Select(condition, null, scope, paging, sortings);
		}

		public IEnumerable<TEntity> Select(ICondition condition, Grouping grouping, params Sorting[] sortings)
		{
			return this.Select(condition, grouping, string.Empty, null, sortings);
		}

		public IEnumerable<TEntity> Select(ICondition condition, Grouping grouping, Paging paging, params Sorting[] sortings)
		{
			return this.Select(condition, grouping, string.Empty, paging, sortings);
		}

		public IEnumerable<TEntity> Select(ICondition condition, Grouping grouping, Paging paging, string scope, params Sorting[] sortings)
		{
			return this.Select(condition, grouping, scope, paging, sortings);
		}

		public IEnumerable<TEntity> Select(ICondition condition, Grouping grouping, string scope, params Sorting[] sortings)
		{
			return this.Select(condition, grouping, scope, null, sortings);
		}

		public IEnumerable<TEntity> Select(ICondition condition, Grouping grouping, string scope, Paging paging, params Sorting[] sortings)
		{
			return this.OnSelect(condition, grouping, scope, paging, sortings);
		}

		protected virtual IEnumerable<TEntity> OnSelect(ICondition condition, Grouping grouping, string scope, Paging paging, params Sorting[] sortings)
		{
			return this.DataAccess.Select<TEntity>(this.Name, condition, grouping, scope, paging, sortings, ctx => this.OnSelecting(ctx), ctx => this.OnSelected(ctx));
		}

		#region 显式实现
		IEnumerable IDataService.Select(ICondition condition, params Sorting[] sortings)
		{
			return this.Select(condition, sortings);
		}

		IEnumerable IDataService.Select(ICondition condition, string scope, params Sorting[] sortings)
		{
			return this.Select(condition, scope, sortings);
		}

		IEnumerable IDataService.Select(ICondition condition, string scope, Paging paging, params Sorting[] sortings)
		{
			return this.Select(condition, scope, paging, sortings);
		}

		IEnumerable IDataService.Select(ICondition condition, Paging paging, params Sorting[] sortings)
		{
			return this.Select(condition, paging, sortings);
		}

		IEnumerable IDataService.Select(ICondition condition, Paging paging, string scope, params Sorting[] sortings)
		{
			return this.Select(condition, paging, scope, sortings);
		}

		IEnumerable IDataService.Select(ICondition condition, Grouping grouping, params Sorting[] sortings)
		{
			return this.Select(condition, grouping, sortings);
		}

		IEnumerable IDataService.Select(ICondition condition, Grouping grouping, string scope, params Sorting[] sortings)
		{
			return this.Select(condition, grouping, scope, sortings);
		}

		IEnumerable IDataService.Select(ICondition condition, Grouping grouping, string scope, Paging paging, params Sorting[] sortings)
		{
			return this.Select(condition, grouping, scope, paging, sortings);
		}

		IEnumerable IDataService.Select(ICondition condition, Grouping grouping, Paging paging, params Sorting[] sortings)
		{
			return this.Select(condition, grouping, paging, sortings);
		}

		IEnumerable IDataService.Select(ICondition condition, Grouping grouping, Paging paging, string scope, params Sorting[] sortings)
		{
			return this.Select(condition, grouping, paging, scope, sortings);
		}
		#endregion

		#endregion

		#region 激发事件
		protected virtual void OnGetted(DataSelectionContext context)
		{
			var e = this.Getted;

			if(e != null)
				e(this, new DataGettedEventArgs<TEntity>(context));
		}

		protected virtual bool OnGetting(DataSelectionContext context)
		{
			var e = this.Getting;

			if(e == null)
				return false;

			var args = new DataGettingEventArgs<TEntity>(context);
			e(this, args);
			return args.Cancel;
		}

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

		protected virtual void OnExecuted(DataExecutionContext context)
		{
			var e = this.Executed;

			if(e != null)
				e(this, new DataExecutedEventArgs(context));
		}

		protected virtual bool OnExecuting(DataExecutionContext context)
		{
			var e = this.Executing;

			if(e == null)
				return false;

			var args = new DataExecutingEventArgs(context);
			e(this, args);
			return args.Cancel;
		}

		protected virtual void OnExisted(DataExistenceContext context)
		{
			var e = this.Existed;

			if(e != null)
				e(this, new DataExistedEventArgs(context));
		}

		protected virtual bool OnExisting(DataExistenceContext context)
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

		protected virtual void OnDeleted(DataDeletionContext context)
		{
			var e = this.Deleted;

			if(e != null)
				e(this, new DataDeletedEventArgs(context));
		}

		protected virtual bool OnDeleting(DataDeletionContext context)
		{
			var e = this.Deleting;

			if(e == null)
				return false;

			var args = new DataDeletingEventArgs(context);
			e(this, args);
			return args.Cancel;
		}

		protected virtual void OnInserted(DataInsertionContext context)
		{
			var e = this.Inserted;

			if(e != null)
				e(this, new DataInsertedEventArgs(context));
		}

		protected virtual bool OnInserting(DataInsertionContext context)
		{
			var e = this.Inserting;

			if(e == null)
				return false;

			var args = new DataInsertingEventArgs(context);
			e(this, args);
			return args.Cancel;
		}

		protected virtual void OnUpdated(DataUpdationContext context)
		{
			var e = this.Updated;

			if(e != null)
				e(this, new DataUpdatedEventArgs(context));
		}

		protected virtual bool OnUpdating(DataUpdationContext context)
		{
			var e = this.Updating;

			if(e == null)
				return false;

			var args = new DataUpdatingEventArgs(context);
			e(this, args);
			return args.Cancel;
		}

		protected virtual void OnSelected(DataSelectionContext context)
		{
			var e = this.Selected;

			if(e != null)
				e(this, new DataSelectedEventArgs(context));
		}

		protected virtual bool OnSelecting(DataSelectionContext context)
		{
			var e = this.Selecting;

			if(e == null)
				return false;

			var args = new DataSelectingEventArgs(context);
			e(this, args);
			return args.Cancel;
		}
		#endregion

		#region 键值操作
		/// <summary>
		/// 根据指定的搜索关键字获取对应的<see cref="ICondition"/>条件。
		/// </summary>
		/// <param name="keyword">指定的搜索关键字。</param>
		/// <param name="singleton">输出一个值，指示返回的搜索条件执行后的结果是否为单个对象。</param>
		/// <returns>返回对应的搜索<see cref="ICondition"/>条件。</returns>
		protected virtual ICondition GetKey(string keyword, out bool singleton)
		{
			singleton = false;

			if(_searchKey == null || string.IsNullOrWhiteSpace(keyword))
				return null;

			var index = keyword.IndexOf(':');

			if(index < 1)
				return _searchKey.GetSearchKey(keyword, null, out singleton);

			var tag = keyword.Substring(0, index);
			var value = index < keyword.Length - 1 ? keyword.Substring(index + 1) : null;

			return _searchKey.GetSearchKey(value, new string[] { tag }, out singleton);
		}

		/// <summary>
		/// 根据指定的查询参数值获取对应的查询<see cref="ICondition"/>条件。
		/// </summary>
		/// <param name="values">指定的查询值数组。</param>
		/// <param name="singleton">输出一个值，指示返回的查询条件执行后的结果是否为单个对象。</param>
		/// <returns>返回对应的查询<see cref="ICondition"/>条件。</returns>
		/// <remarks>
		///		<para>基类的实现始终返回当前数据服务对应的主键的键值对数组。</para>
		/// </remarks>
		protected virtual ICondition GetKey(object[] values, out bool singleton)
		{
			//设置输出参数默认值
			singleton = false;

			if(values == null || values.Length == 0)
				return null;

			//获取当前数据服务对应的主键
			var primaryKey = this.DataAccess.GetKey(this.Name);

			//如果主键获取失败或主键未定义或主键项数量不等于传入的数组元素个数则返回空
			if(primaryKey == null || primaryKey.Length == 0 || primaryKey.Length != values.Length)
				return null;

			//匹配主键，故输出参数值为真
			singleton = true;

			//如果主键成员只有一个则返回单个条件
			if(primaryKey.Length == 1)
				return Condition.Equal(primaryKey[0], values[0]);

			//创建返回的条件集（AND组合）
			var conditions = ConditionCollection.And();

			for(int i = 0; i < primaryKey.Length; i++)
			{
				conditions.Add(Condition.Equal(primaryKey[i], values[i]));
			}

			return conditions;
		}
		#endregion

		#region 私有方法
		private ICondition ConvertKey<TKey>(TKey key, out bool singleton)
		{
			return this.EnsureInquiryKey(new object[] { key }, out singleton);
		}

		private ICondition ConvertKey<TKey1, TKey2>(TKey1 key1, TKey2 key2, out bool singleton)
		{
			return this.EnsureInquiryKey(new object[] { key1, key2 }, out singleton);
		}

		private ICondition ConvertKey<TKey1, TKey2, TKey3>(TKey1 key1, TKey2 key2, TKey3 key3, out bool singleton)
		{
			return this.EnsureInquiryKey(new object[] { key1, key2, key3 }, out singleton);
		}

		private ICondition EnsureInquiryKey(object[] values, out bool singleton)
		{
			if(values != null && values.Length > 3)
				throw new NotSupportedException("Too many the keys.");

			//获取查询键值对数组
			var condition = this.GetKey(values ?? new object[0], out singleton);

			if(condition == null)
				throw new ArgumentException($"The specified key is invalid of the {this.Name} service.");

			if(condition != null && condition is ConditionCollection)
			{
				if(((ConditionCollection)condition).Count == 1)
					return ((ConditionCollection)condition)[0];
			}

			return condition;
		}
		#endregion

		#region 嵌套子类
		private static class DataSequence
		{
			#region 常量定义
			private const string SEQUENCE_KEY_PREFIX = "Zongsoft.Data.Sequence";
			#endregion

			#region 静态缓存
			private static readonly ConcurrentDictionary<DataService<TEntity>, DataSequenceToken[]> _cache = new ConcurrentDictionary<DataService<TEntity>, DataSequenceToken[]>();
			#endregion

			#region 公共方法
			public static bool Register(DataService<TEntity> dataService)
			{
				var attributes = (DataSequenceAttribute[])Attribute.GetCustomAttributes(dataService.GetType(), typeof(DataSequenceAttribute), true);

				if(attributes == null || attributes.Length == 0)
					return false;

				var tokens = new DataSequenceToken[attributes.Length];

				for(int i = 0; i < attributes.Length; i++)
				{
					var attribute = attributes[i];
					var sequence = string.IsNullOrWhiteSpace(attribute.SequenceName) ? dataService.ServiceProvider.ResolveRequired<Common.ISequence>() : dataService.ServiceProvider.ResolveRequired(attribute.SequenceName) as Common.ISequence;

					if(sequence == null)
						throw new InvalidOperationException($"Not found '{attribute.SequenceName}' sequence for the '{dataService.Name}' data service.");

					var property = typeof(TEntity).GetProperty(attribute.Keys[attribute.Keys.Length - 1], System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

					if(property == null)
						throw new InvalidOperationException($"The '{attribute.Keys[attribute.Keys.Length - 1]}' sequence property is not existed.");

					if(!Zongsoft.Common.TypeExtension.IsNumeric(property.PropertyType))
						throw new InvalidOperationException($"The '{property.Name}' sequence property type not is numeric.");

					tokens[i] = new DataSequenceToken(sequence, attribute, property.PropertyType);
				}

				return _cache.TryAdd(dataService, tokens);
			}

			public static void Increments(DataService<TEntity> dataService, DataDictionary<TEntity> data)
			{
				DataSequenceToken[] tokens;

				if(_cache.TryGetValue(dataService, out tokens))
				{
					foreach(var token in tokens)
					{
						var sequenceKey = GetSequenceKey(data, token.Attribute);

						if(sequenceKey != null && sequenceKey.Length > 0)
						{
							//确保只有当序号字段未指定值或值为零时，才使用增量的序号值
							data.Set(token.Attribute.Keys[token.Attribute.Keys.Length - 1],
									 () => token.Sequence.Increment(sequenceKey, 1, token.Attribute.Seed),
									 value => value == null || (ulong)System.Convert.ChangeType(value, typeof(ulong)) == 0);
						}
					}
				}
			}
			#endregion

			#region 私有方法
			private static string GetSequenceKey(DataDictionary<TEntity> data, DataSequenceAttribute attribute)
			{
				var result = SEQUENCE_KEY_PREFIX;

				if(!string.IsNullOrWhiteSpace(attribute.Prefix))
					result += ":" + attribute.Prefix;

				for(int i = 0; i < attribute.Keys.Length - 1; i++)
				{
					var value = data.Get(attribute.Keys[i]);

					if(value != null)
						result += ":" + value.ToString().ToLowerInvariant();
				}

				return result += ":" + attribute.Keys[attribute.Keys.Length - 1].ToLowerInvariant();
			}
			#endregion

			#region 嵌套子类
			private class DataSequenceToken
			{
				public readonly Common.ISequence Sequence;
				public readonly DataSequenceAttribute Attribute;
				public readonly Type Type;

				public DataSequenceToken(Common.ISequence sequence, DataSequenceAttribute attribute, Type type)
				{
					this.Sequence = sequence;
					this.Attribute = attribute;
					this.Type = type;
				}
			}
			#endregion
		}
		#endregion
	}
}

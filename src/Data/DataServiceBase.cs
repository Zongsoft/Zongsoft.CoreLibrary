/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2016-2018 Zongsoft Corporation <http://www.zongsoft.com>
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
	public class DataServiceBase<TEntity> : IDataService<TEntity>
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
		protected DataServiceBase(Zongsoft.Services.IServiceProvider serviceProvider)
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

		protected DataServiceBase(string name, Zongsoft.Services.IServiceProvider serviceProvider)
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
				if(string.IsNullOrEmpty(_name))
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
				if(_dataAccess == null)
					_dataAccess = _serviceProvider.Resolve<IDataAccess>();

				return _dataAccess;
			}
			set
			{
				_dataAccess = value ?? throw new ArgumentNullException();
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
		protected virtual Zongsoft.Security.Credential Credential
		{
			get
			{
				if(Services.ApplicationContext.Current?.Principal is Zongsoft.Security.CredentialPrincipal principal && principal.Identity.IsAuthenticated)
					return principal.Identity.Credential;

				return null;
			}
		}

		protected virtual bool CanDelete
		{
			get => true;
		}

		protected virtual bool CanInsert
		{
			get => true;
		}

		protected virtual bool CanUpdate
		{
			get => true;
		}

		protected virtual bool CanUpsert
		{
			get => true;
		}
		#endregion

		#region 执行方法
		public IEnumerable<T> Execute<T>(string name, IDictionary<string, object> inParameters, object state = null)
		{
			IDictionary<string, object> outParameters;
			return this.Execute<T>(name, inParameters, out outParameters, state);
		}

		public virtual IEnumerable<T> Execute<T>(string name, IDictionary<string, object> inParameters, out IDictionary<string, object> outParameters, object state = null)
		{
			return this.DataAccess.Execute<T>(name, inParameters, out outParameters, state, ctx => this.OnExecuting(ctx), ctx => this.OnExecuted(ctx));
		}

		public object ExecuteScalar(string name, IDictionary<string, object> inParameters, object state = null)
		{
			IDictionary<string, object> outParameters;
			return this.ExecuteScalar(name, inParameters, out outParameters, state);
		}

		public virtual object ExecuteScalar(string name, IDictionary<string, object> inParameters, out IDictionary<string, object> outParameters, object state = null)
		{
			return this.DataAccess.ExecuteScalar(name, inParameters, out outParameters, state, ctx => this.OnExecuting(ctx), ctx => this.OnExecuted(ctx));
		}
		#endregion

		#region 存在方法
		public virtual bool Exists(ICondition condition, object state = null)
		{
			//修整查询条件
			condition = this.OnValidate(DataAccessMethod.Exists, condition);

			return this.DataAccess.Exists(this.Name, condition, state, ctx => this.OnExisting(ctx), ctx => this.OnExisted(ctx));
		}

		public virtual bool Exists<TKey>(TKey key, object state = null)
		{
			return this.Exists(this.ConvertKey(key, out _), state);
		}

		public virtual bool Exists<TKey1, TKey2>(TKey1 key1, TKey2 key2, object state = null)
		{
			return this.Exists(this.ConvertKey(key1, key2, out _), state);
		}

		public virtual bool Exists<TKey1, TKey2, TKey3>(TKey1 key1, TKey2 key2, TKey3 key3, object state = null)
		{
			return this.Exists(this.ConvertKey(key1, key2, key3, out _), state);
		}
		#endregion

		#region 计数方法
		public int Count(ICondition condition, object state)
		{
			return this.Count(condition, string.Empty, state);
		}

		public int Count(ICondition condition, string member)
		{
			return this.Count(condition, member, null);
		}

		public virtual int Count(ICondition condition, string member = null, object state = null)
		{
			//修整查询条件
			condition = this.OnValidate(DataAccessMethod.Count, condition);

			return this.DataAccess.Count(this.Name, condition, member, state, ctx => this.OnCounting(ctx), ctx => this.OnCounted(ctx));
		}
		#endregion

		#region 递增方法
		public long Increment(string member, ICondition condition, object state)
		{
			return this.Increment(member, condition, 1, state);
		}

		public long Increment(string member, ICondition condition, int interval)
		{
			return this.Increment(member, condition, interval, null);
		}

		public virtual long Increment(string member, ICondition condition, int interval = 1, object state = null)
		{
			//修整查询条件
			condition = this.OnValidate(DataAccessMethod.Increment, condition);

			return this.DataAccess.Increment(this.Name, member, condition, interval, state, ctx => this.OnIncrementing(ctx), ctx => this.OnIncremented(ctx));
		}

		public long Decrement(string member, ICondition condition, object state)
		{
			return this.Decrement(member, condition, 1, state);
		}

		public long Decrement(string member, ICondition condition, int interval)
		{
			return this.Decrement(member, condition, interval, null);
		}

		public virtual long Decrement(string member, ICondition condition, int interval = 1, object state = null)
		{
			//修整查询条件
			condition = this.OnValidate(DataAccessMethod.Increment, condition);

			return this.DataAccess.Decrement(this.Name, member, condition, interval, state, ctx => this.OnIncrementing(ctx), ctx => this.OnIncremented(ctx));
		}
		#endregion

		#region 删除方法
		public virtual int Delete<TKey>(TKey key, string schema = null)
		{
			return this.Delete<TKey>(key, schema, null);
		}

		public virtual int Delete<TKey>(TKey key, string schema, object state)
		{
			//确认是否可以执行该操作
			this.EnsureDelete();

			//将删除键转换成条件对象，并进行修整
			var condition = this.OnValidate(DataAccessMethod.Delete, this.ConvertKey(key, out _));

			//执行删除操作
			return this.OnDelete(condition, this.GetSchema(schema), state);
		}

		public virtual int Delete<TKey1, TKey2>(TKey1 key1, TKey2 key2, string schema = null)
		{
			return this.Delete<TKey1, TKey2>(key1, key2, schema, null);
		}

		public virtual int Delete<TKey1, TKey2>(TKey1 key1, TKey2 key2, string schema, object state)
		{
			//确认是否可以执行该操作
			this.EnsureDelete();

			//将删除键转换成条件对象，并进行修整
			var condition = this.OnValidate(DataAccessMethod.Delete, this.ConvertKey(key1, key2, out _));

			//执行删除操作
			return this.OnDelete(condition, this.GetSchema(schema), state);
		}

		public virtual int Delete<TKey1, TKey2, TKey3>(TKey1 key1, TKey2 key2, TKey3 key3, string schema = null)
		{
			return this.Delete<TKey1, TKey2, TKey3>(key1, key2, key3, schema, null);
		}

		public virtual int Delete<TKey1, TKey2, TKey3>(TKey1 key1, TKey2 key2, TKey3 key3, string schema, object state)
		{
			//确认是否可以执行该操作
			this.EnsureDelete();

			//将删除键转换成条件对象，并进行修整
			var condition = this.OnValidate(DataAccessMethod.Delete, this.ConvertKey(key1, key2, key3, out _));

			//执行删除操作
			return this.OnDelete(condition, this.GetSchema(schema), state);
		}

		public int Delete(ICondition condition, string schema = null)
		{
			return this.Delete(condition, schema, null);
		}

		public int Delete(ICondition condition, string schema, object state)
		{
			//确认是否可以执行该操作
			this.EnsureDelete();

			//修整删除条件
			condition = this.OnValidate(DataAccessMethod.Delete, condition);

			//执行删除操作
			return this.OnDelete(condition, this.GetSchema(schema), state);
		}

		protected virtual int OnDelete(ICondition condition, ISchema schema, object state)
		{
			if(condition == null)
				throw new NotSupportedException("The condition cann't is null on delete operation.");

			return this.DataAccess.Delete(this.Name, condition, schema, state, ctx => this.OnDeleting(ctx), ctx => this.OnDeleted(ctx));
		}
		#endregion

		#region 插入方法
		public int Insert(object data)
		{
			return this.Insert(data, null, null);
		}

		public int Insert(object data, object state)
		{
			return this.Insert(data, null, state);
		}

		public int Insert(object data, string schema)
		{
			return this.Insert(data, schema, null);
		}

		public int Insert(object data, string schema, object state)
		{
			//确认是否可以执行该操作
			this.EnsureInsert();

			if(data == null)
				return 0;

			//将当前插入数据对象转换成数据字典
			var dictionary = DataDictionary.GetDictionary<TEntity>(data);

			//验证待新增的数据
			this.OnValidate(DataAccessMethod.Insert, dictionary);

			//尝试递增注册的递增键值
			DataSequence.Increments(this, dictionary);

			return this.OnInsert(dictionary, this.GetSchema(schema, data.GetType()), state);
		}

		public int InsertMany(IEnumerable items)
		{
			return this.InsertMany(items, null, null);
		}

		public int InsertMany(IEnumerable items, object state)
		{
			return this.InsertMany(items, null, state);
		}

		public int InsertMany(IEnumerable items, string schema)
		{
			return this.InsertMany(items, schema, null);
		}

		public int InsertMany(IEnumerable items, string schema, object state)
		{
			//确认是否可以执行该操作
			this.EnsureInsert();

			if(items == null)
				return 0;

			//将当前插入数据集合对象转换成数据字典集合
			var dictionares = DataDictionary.GetDictionaries<TEntity>(items);

			foreach(var dictionary in dictionares)
			{
				//验证待新增的数据
				this.OnValidate(DataAccessMethod.Insert, dictionary);

				//尝试递增注册的递增键值
				DataSequence.Increments(this, dictionary);
			}

			return this.OnInsertMany(dictionares, this.GetSchema(schema, Common.TypeExtension.GetElementType(items.GetType())), state);
		}

		protected virtual int OnInsert(IDataDictionary<TEntity> data, ISchema schema, object state)
		{
			if(data == null || data.Data == null)
				return 0;

			//执行数据引擎的插入操作
			return this.DataAccess.Insert(this.Name, data, schema, state, ctx => this.OnInserting(ctx), ctx => this.OnInserted(ctx));
		}

		protected virtual int OnInsertMany(IEnumerable<IDataDictionary<TEntity>> items, ISchema schema, object state)
		{
			if(items == null)
				return 0;

			//执行数据引擎的插入操作
			return this.DataAccess.InsertMany(this.Name, items, schema, state, ctx => this.OnInserting(ctx), ctx => this.OnInserted(ctx));
		}
		#endregion

		#region 更新方法
		public int Update<TKey>(object data, TKey key, object state = null)
		{
			return this.Update<TKey>(data, key, null, state);
		}

		public virtual int Update<TKey>(object data, TKey key, string schema, object state = null)
		{
			return this.Update(data, this.ConvertKey(key, out _), schema, state);
		}

		public int Update<TKey1, TKey2>(object data, TKey1 key1, TKey2 key2, object state = null)
		{
			return this.Update<TKey1, TKey2>(data, key1, key2, null, state);
		}

		public virtual int Update<TKey1, TKey2>(object data, TKey1 key1, TKey2 key2, string schema, object state = null)
		{
			return this.Update(data, this.ConvertKey(key1, key2, out _), schema, state);
		}

		public int Update<TKey1, TKey2, TKey3>(object data, TKey1 key1, TKey2 key2, TKey3 key3, object state = null)
		{
			return this.Update<TKey1, TKey2, TKey3>(data, key1, key2, key3, null, state);
		}

		public virtual int Update<TKey1, TKey2, TKey3>(object data, TKey1 key1, TKey2 key2, TKey3 key3, string schema, object state = null)
		{
			return this.Update(data, this.ConvertKey(key1, key2, key3, out _), schema, state);
		}

		public int Update(object data, object state = null)
		{
			return this.Update(data, null, null, state);
		}

		public int Update(object data, string schema, object state = null)
		{
			return this.Update(data, null, schema, state);
		}

		public int Update(object data, ICondition condition, object state = null)
		{
			return this.Update(data, condition, null, state);
		}

		public int Update(object data, ICondition condition, string schema, object state = null)
		{
			//确认是否可以执行该操作
			this.EnsureUpdate();

			//修整过滤条件
			condition = this.OnValidate(DataAccessMethod.Update, condition);

			//将当前更新数据对象转换成数据字典
			var dictionary = DataDictionary.GetDictionary<TEntity>(data);

			//验证待更新的数据
			this.OnValidate(DataAccessMethod.Update, dictionary);

			//执行更新操作
			return this.OnUpdate(dictionary, condition, this.GetSchema(schema, data.GetType()), state);
		}

		public int UpdateMany(IEnumerable items, object state = null)
		{
			return this.UpdateMany(items, null, state);
		}

		public int UpdateMany(IEnumerable items, string schema, object state = null)
		{
			//确认是否可以执行该操作
			this.EnsureUpdate();

			if(items == null)
				return 0;

			//将当前更新数据集合对象转换成数据字典集合
			var dictionares = DataDictionary.GetDictionaries<TEntity>(items);

			foreach(var dictionary in dictionares)
			{
				//验证待更新的数据
				this.OnValidate(DataAccessMethod.Update, dictionary);
			}

			return this.OnUpdateMany(dictionares, this.GetSchema(schema, Common.TypeExtension.GetElementType(items.GetType())), state);
		}

		protected virtual int OnUpdate(IDataDictionary<TEntity> data, ICondition condition, ISchema schema, object state)
		{
			if(data == null || data.Data == null)
				return 0;

			return this.DataAccess.Update(this.Name, data, condition, schema, state, ctx => this.OnUpdating(ctx), ctx => this.OnUpdated(ctx));
		}

		protected virtual int OnUpdateMany(IEnumerable<IDataDictionary<TEntity>> items, ISchema schema, object state)
		{
			if(items == null)
				return 0;

			return this.DataAccess.UpdateMany(this.Name, items, schema, state, ctx => this.OnUpdating(ctx), ctx => this.OnUpdated(ctx));
		}
		#endregion

		#region 查询方法

		#region 搜索方法
		public IEnumerable Search(string keyword, params Sorting[] sortings)
		{
			return this.Search(keyword, string.Empty, null, null, sortings);
		}

		public IEnumerable Search(string keyword, object state, params Sorting[] sortings)
		{
			return this.Search(keyword, string.Empty, null, state, sortings);
		}

		public IEnumerable Search(string keyword, Paging paging, params Sorting[] sortings)
		{
			return this.Search(keyword, string.Empty, paging, null, sortings);
		}

		public IEnumerable Search(string keyword, string schema, params Sorting[] sortings)
		{
			return this.Search(keyword, schema, null, null, sortings);
		}

		public IEnumerable Search(string keyword, string schema, object state, params Sorting[] sortings)
		{
			return this.Search(keyword, schema, null, state, sortings);
		}

		public IEnumerable Search(string keyword, string schema, Paging paging, params Sorting[] sortings)
		{
			return this.Search(keyword, schema, paging, null, sortings);
		}

		public virtual IEnumerable Search(string keyword, string schema, Paging paging, object state, params Sorting[] sortings)
		{
			//获取搜索条件和搜索结果是否为单条数据
			var condition = this.GetKey(keyword);

			if(condition == null)
				throw new ArgumentException($"The {this.Name} service does not supportd search operation or specified search key is invalid.");

			return this.Select(condition, schema, paging, state, sortings);
		}
		#endregion

		#region 单键查询
		public object Get<TKey>(TKey key, params Sorting[] sortings)
		{
			return this.Get<TKey>(key, string.Empty, null, null, out _, sortings);
		}

		public object Get<TKey>(TKey key, object state, params Sorting[] sortings)
		{
			return this.Get<TKey>(key, string.Empty, null, state, out _, sortings);
		}

		public object Get<TKey>(TKey key, Paging paging, params Sorting[] sortings)
		{
			return this.Get<TKey>(key, string.Empty, paging, null, out _, sortings);
		}

		public object Get<TKey>(TKey key, string schema, params Sorting[] sortings)
		{
			return this.Get<TKey>(key, schema, null, null, out _, sortings);
		}

		public object Get<TKey>(TKey key, string schema, object state, params Sorting[] sortings)
		{
			return this.Get<TKey>(key, schema, null, state, out _, sortings);
		}

		public object Get<TKey>(TKey key, string schema, Paging paging, params Sorting[] sortings)
		{
			return this.Get<TKey>(key, schema, paging, null, out _, sortings);
		}

		public object Get<TKey>(TKey key, string schema, Paging paging, object state, params Sorting[] sortings)
		{
			return this.Get(key, schema, paging, state, out _, sortings);
		}

		public virtual object Get<TKey>(TKey key, string schema, Paging paging, object state, out IPaginator paginator, params Sorting[] sortings)
		{
			var condition = this.ConvertKey(key, out var singleton);

			if(singleton)
			{
				//修整查询条件
				condition = this.OnValidate(DataAccessMethod.Select, condition);

				//执行单条查询方法
				return this.OnGet(condition, this.GetSchema(schema), state, out paginator);
			}

			var result = this.Select(condition, schema, paging, state, sortings);
			paginator = result as IPaginator;
			return result;
		}
		#endregion

		#region 双键查询
		public object Get<TKey1, TKey2>(TKey1 key1, TKey2 key2, params Sorting[] sortings)
		{
			return this.Get<TKey1, TKey2>(key1, key2, string.Empty, null, null, out _, sortings);
		}

		public object Get<TKey1, TKey2>(TKey1 key1, TKey2 key2, object state, params Sorting[] sortings)
		{
			return this.Get<TKey1, TKey2>(key1, key2, string.Empty, null, state, out _, sortings);
		}

		public object Get<TKey1, TKey2>(TKey1 key1, TKey2 key2, Paging paging, params Sorting[] sortings)
		{
			return this.Get<TKey1, TKey2>(key1, key2, string.Empty, paging, null, out _, sortings);
		}

		public object Get<TKey1, TKey2>(TKey1 key1, TKey2 key2, string schema, params Sorting[] sortings)
		{
			return this.Get<TKey1, TKey2>(key1, key2, schema, null, null, out _, sortings);
		}

		public object Get<TKey1, TKey2>(TKey1 key1, TKey2 key2, string schema, object state, params Sorting[] sortings)
		{
			return this.Get<TKey1, TKey2>(key1, key2, schema, null, state, out _, sortings);
		}

		public object Get<TKey1, TKey2>(TKey1 key1, TKey2 key2, string schema, Paging paging, params Sorting[] sortings)
		{
			return this.Get<TKey1, TKey2>(key1, key2, schema, paging, null, out _, sortings);
		}

		public object Get<TKey1, TKey2>(TKey1 key1, TKey2 key2, string schema, Paging paging, object state, params Sorting[] sortings)
		{
			return this.Get(key1, key2, schema, paging, state, out _, sortings);
		}

		public virtual object Get<TKey1, TKey2>(TKey1 key1, TKey2 key2, string schema, Paging paging, object state, out IPaginator paginator, params Sorting[] sortings)
		{
			var condition = this.ConvertKey(key1, key2, out var singleton);

			if(singleton)
			{
				//修整查询条件
				condition = this.OnValidate(DataAccessMethod.Select, condition);

				//执行单条查询方法
				return this.OnGet(condition, this.GetSchema(schema), state, out paginator);
			}

			var result = this.Select(condition, schema, paging, state, sortings);
			paginator = result as IPaginator;
			return result;
		}
		#endregion

		#region 三键查询
		public object Get<TKey1, TKey2, TKey3>(TKey1 key1, TKey2 key2, TKey3 key3, params Sorting[] sortings)
		{
			return this.Get(key1, key2, key3, string.Empty, null, null, out _, sortings);
		}

		public object Get<TKey1, TKey2, TKey3>(TKey1 key1, TKey2 key2, TKey3 key3, object state, params Sorting[] sortings)
		{
			return this.Get(key1, key2, key3, string.Empty, null, state, out _, sortings);
		}

		public object Get<TKey1, TKey2, TKey3>(TKey1 key1, TKey2 key2, TKey3 key3, Paging paging, params Sorting[] sortings)
		{
			return this.Get(key1, key2, key3, string.Empty, paging, null, out _, sortings);
		}

		public object Get<TKey1, TKey2, TKey3>(TKey1 key1, TKey2 key2, TKey3 key3, string schema, params Sorting[] sortings)
		{
			return this.Get(key1, key2, key3, schema, null, null, out _, sortings);
		}

		public object Get<TKey1, TKey2, TKey3>(TKey1 key1, TKey2 key2, TKey3 key3, string schema, object state, params Sorting[] sortings)
		{
			return this.Get(key1, key2, key3, schema, null, state, out _, sortings);
		}

		public object Get<TKey1, TKey2, TKey3>(TKey1 key1, TKey2 key2, TKey3 key3, string schema, Paging paging, params Sorting[] sortings)
		{
			return this.Get(key1, key2, key3, schema, paging, null, out _, sortings);
		}

		public object Get<TKey1, TKey2, TKey3>(TKey1 key1, TKey2 key2, TKey3 key3, string schema, Paging paging, object state, params Sorting[] sortings)
		{
			return this.Get(key1, key2, key3, schema, paging, state, out _, sortings);
		}

		public virtual object Get<TKey1, TKey2, TKey3>(TKey1 key1, TKey2 key2, TKey3 key3, string schema, Paging paging, object state, out IPaginator paginator, params Sorting[] sortings)
		{
			var condition = this.ConvertKey(key1, key2, key3, out var singleton);

			if(singleton)
			{
				//修整查询条件
				condition = this.OnValidate(DataAccessMethod.Select, condition);

				//执行单条查询方法
				return this.OnGet(condition, this.GetSchema(schema), state, out paginator);
			}

			var result = this.Select(condition, schema, paging, state, sortings);
			paginator = result as IPaginator;
			return result;
		}

		protected virtual object OnGet(ICondition condition, ISchema schema, object state, out IPaginator paginator)
		{
			var result = this.DataAccess.Select<TEntity>(this.Name, condition, schema, null, state, null, ctx => this.OnGetting(ctx), ctx => this.OnGetted(ctx));
			paginator = result as IPaginator;

			using(var iterator = result.GetEnumerator())
			{
				if(iterator.MoveNext())
					return iterator.Current;
				else
					return null;
			}
		}
		#endregion

		#region 常规查询
		public IEnumerable<TEntity> Select(object state = null, params Sorting[] sortings)
		{
			return this.Select(null, string.Empty, null, state, sortings);
		}

		public IEnumerable<TEntity> Select(ICondition condition, params Sorting[] sortings)
		{
			return this.Select(condition, string.Empty, null, null, sortings);
		}

		public IEnumerable<TEntity> Select(ICondition condition, object state, params Sorting[] sortings)
		{
			return this.Select(condition, string.Empty, null, state, sortings);
		}

		public IEnumerable<TEntity> Select(ICondition condition, Paging paging, params Sorting[] sortings)
		{
			return this.Select(condition, string.Empty, paging, null, sortings);
		}

		public IEnumerable<TEntity> Select(ICondition condition, Paging paging, object state, params Sorting[] sortings)
		{
			return this.Select(condition, string.Empty, paging, state, sortings);
		}

		public IEnumerable<TEntity> Select(ICondition condition, Paging paging, string schema, params Sorting[] sortings)
		{
			return this.Select(condition, schema, paging, null, sortings);
		}

		public IEnumerable<TEntity> Select(ICondition condition, Paging paging, string schema, object state, params Sorting[] sortings)
		{
			return this.Select(condition, schema, paging, state, sortings);
		}

		public IEnumerable<TEntity> Select(ICondition condition, string schema, params Sorting[] sortings)
		{
			return this.Select(condition, schema, null, null, sortings);
		}

		public IEnumerable<TEntity> Select(ICondition condition, string schema, object state, params Sorting[] sortings)
		{
			return this.Select(condition, schema, null, state, sortings);
		}

		public IEnumerable<TEntity> Select(ICondition condition, string schema, Paging paging, params Sorting[] sortings)
		{
			return this.Select(condition, schema, paging, null, sortings);
		}

		public IEnumerable<TEntity> Select(ICondition condition, string schema, Paging paging, object state, params Sorting[] sortings)
		{
			//修整查询条件
			condition = this.OnValidate(DataAccessMethod.Select, condition);

			//执行查询方法
			return this.OnSelect(condition, this.GetSchema(schema, typeof(TEntity)), paging, sortings, state);
		}

		protected virtual IEnumerable<TEntity> OnSelect(ICondition condition, ISchema schema, Paging paging, Sorting[] sortings, object state)
		{
			return this.DataAccess.Select<TEntity>(this.Name, condition, schema, paging, state, sortings, ctx => this.OnSelecting(ctx), ctx => this.OnSelected(ctx));
		}

		public IEnumerable<T> Select<T>(Grouping grouping, params Sorting[] sortings)
		{
			return this.Select<T>(grouping, null, null, null, sortings);
		}

		public IEnumerable<T> Select<T>(Grouping grouping, object state, params Sorting[] sortings)
		{
			return this.Select<T>(grouping, null, null, state, sortings);
		}

		public IEnumerable<T> Select<T>(Grouping grouping, Paging paging, object state = null, params Sorting[] sortings)
		{
			return this.Select<T>(grouping, null, paging, state, sortings);
		}

		public IEnumerable<T> Select<T>(Grouping grouping, ICondition condition, params Sorting[] sortings)
		{
			return this.Select<T>(grouping, condition, null, null, sortings);
		}

		public IEnumerable<T> Select<T>(Grouping grouping, ICondition condition, object state, params Sorting[] sortings)
		{
			return this.Select<T>(grouping, condition, null, state, sortings);
		}

		public IEnumerable<T> Select<T>(Grouping grouping, ICondition condition, Paging paging, object state = null, params Sorting[] sortings)
		{
			return this.OnSelect<T>(grouping, condition, paging, sortings, state);
		}

		protected virtual IEnumerable<T> OnSelect<T>(Grouping grouping, ICondition condition, Paging paging, Sorting[] sortings, object state)
		{
			return this.DataAccess.Select<T>(this.Name, grouping, condition, paging, state, sortings, ctx => this.OnSelecting(ctx), ctx => this.OnSelected(ctx));
		}
		#endregion

		#region 显式实现
		IEnumerable IDataService.Select(object state, params Sorting[] sortings)
		{
			return this.Select(state, sortings);
		}

		IEnumerable IDataService.Select(ICondition condition, params Sorting[] sortings)
		{
			return this.Select(condition, sortings);
		}

		IEnumerable IDataService.Select(ICondition condition, object state, params Sorting[] sortings)
		{
			return this.Select(condition, state, sortings);
		}

		IEnumerable IDataService.Select(ICondition condition, string schema, params Sorting[] sortings)
		{
			return this.Select(condition, schema, sortings);
		}

		IEnumerable IDataService.Select(ICondition condition, string schema, object state, params Sorting[] sortings)
		{
			return this.Select(condition, schema, state, sortings);
		}

		IEnumerable IDataService.Select(ICondition condition, string schema, Paging paging, params Sorting[] sortings)
		{
			return this.Select(condition, schema, paging, sortings);
		}

		IEnumerable IDataService.Select(ICondition condition, string schema, Paging paging, object state, params Sorting[] sortings)
		{
			return this.Select(condition, schema, paging, state, sortings);
		}

		IEnumerable IDataService.Select(ICondition condition, Paging paging, params Sorting[] sortings)
		{
			return this.Select(condition, paging, sortings);
		}

		IEnumerable IDataService.Select(ICondition condition, Paging paging, object state, params Sorting[] sortings)
		{
			return this.Select(condition, paging, state, sortings);
		}

		IEnumerable IDataService.Select(ICondition condition, Paging paging, string schema, params Sorting[] sortings)
		{
			return this.Select(condition, paging, schema, sortings);
		}

		IEnumerable IDataService.Select(ICondition condition, Paging paging, string schema, object state, params Sorting[] sortings)
		{
			return this.Select(condition, paging, schema, state, sortings);
		}
		#endregion

		#endregion

		#region 校验方法
		protected virtual ICondition OnValidate(DataAccessMethod method, ICondition condition)
		{
			return condition;
		}

		protected virtual void OnValidate(DataAccessMethod method, IDataDictionary<TEntity> data)
		{
		}
		#endregion

		#region 激发事件
		protected virtual void OnGetted(DataSelectContextBase context)
		{
			var e = this.Getted;

			if(e != null)
				e(this, new DataGettedEventArgs<TEntity>(context));
		}

		protected virtual bool OnGetting(DataSelectContextBase context)
		{
			var e = this.Getting;

			if(e == null)
				return false;

			var args = new DataGettingEventArgs<TEntity>(context);
			e(this, args);
			return args.Cancel;
		}

		protected virtual void OnCounted(DataCountContextBase context)
		{
			var e = this.Counted;

			if(e != null)
				e(this, new DataCountedEventArgs(context));
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
			var e = this.Executed;

			if(e != null)
				e(this, new DataExecutedEventArgs(context));
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
			var e = this.Existed;

			if(e != null)
				e(this, new DataExistedEventArgs(context));
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
			var e = this.Incremented;

			if(e != null)
				e(this, new DataIncrementedEventArgs(context));
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
			var e = this.Deleted;

			if(e != null)
				e(this, new DataDeletedEventArgs(context));
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
			var e = this.Inserted;

			if(e != null)
				e(this, new DataInsertedEventArgs(context));
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

		protected virtual void OnUpdated(DataUpdateContextBase context)
		{
			var e = this.Updated;

			if(e != null)
				e(this, new DataUpdatedEventArgs(context));
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
			var e = this.Selected;

			if(e != null)
				e(this, new DataSelectedEventArgs(context));
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

		#region 键值操作
		/// <summary>
		/// 根据指定的搜索关键字获取对应的<see cref="ICondition"/>条件。
		/// </summary>
		/// <param name="keyword">指定的搜索关键字。</param>
		/// <returns>返回对应的搜索<see cref="ICondition"/>条件。</returns>
		protected virtual ICondition GetKey(string keyword)
		{
			if(_searchKey == null || string.IsNullOrWhiteSpace(keyword))
				return null;

			var index = keyword.IndexOf(':');

			if(index < 1)
				return _searchKey.GetSearchKey(keyword, null);

			var tag = keyword.Substring(0, index);
			var value = index < keyword.Length - 1 ? keyword.Substring(index + 1) : null;

			return _searchKey.GetSearchKey(value, new string[] { tag });
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
				return Data.Condition.Equal(primaryKey[0], values[0]);

			//创建返回的条件集（AND组合）
			var conditions = ConditionCollection.And();

			for(int i = 0; i < primaryKey.Length; i++)
			{
				conditions.Add(Data.Condition.Equal(primaryKey[i], values[i]));
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

		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
		private ISchema GetSchema(string expression, Type type = null)
		{
			return this.DataAccess.Schema.Parse(this.Name, expression, type ?? typeof(TEntity));
		}

		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
		private void EnsureDelete()
		{
			if(!this.CanDelete)
				throw new InvalidOperationException("The delete operation is not allowed.");
		}

		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
		private void EnsureInsert()
		{
			if(!this.CanInsert)
				throw new InvalidOperationException("The insert operation is not allowed.");
		}

		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
		private void EnsureUpdate()
		{
			if(!this.CanUpdate)
				throw new InvalidOperationException("The update operation is not allowed.");
		}

		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
		private void EnsureUpsert()
		{
			if(!this.CanUpsert)
				throw new InvalidOperationException("The upsert operation is not allowed.");
		}
		#endregion

		#region 嵌套子类
		public sealed class Condition : Zongsoft.Data.Condition.Builder<TEntity>
		{
			#region 私有构造
			private Condition()
			{
			}
			#endregion
		}

		private static class DataSequence
		{
			#region 常量定义
			private const string SEQUENCE_KEY_PREFIX = "Zongsoft.Data.Sequence";
			#endregion

			#region 静态缓存
			private static readonly ConcurrentDictionary<DataServiceBase<TEntity>, DataSequenceToken[]> _cache = new ConcurrentDictionary<DataServiceBase<TEntity>, DataSequenceToken[]>();
			#endregion

			#region 公共方法
			public static bool Register(DataServiceBase<TEntity> dataService)
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

			public static void Increments(DataServiceBase<TEntity> dataService, IDataDictionary data)
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
							data.SetValue(token.Attribute.Keys[token.Attribute.Keys.Length - 1],
									 () => token.Sequence.Increment(sequenceKey, 1, token.Attribute.Seed),
									 value => value == null || (ulong)System.Convert.ChangeType(value, typeof(ulong)) == 0);
						}
					}
				}
			}
			#endregion

			#region 私有方法
			private static string GetSequenceKey(IDataDictionary data, DataSequenceAttribute attribute)
			{
				var result = SEQUENCE_KEY_PREFIX;

				if(!string.IsNullOrWhiteSpace(attribute.Prefix))
					result += ":" + attribute.Prefix;

				for(int i = 0; i < attribute.Keys.Length - 1; i++)
				{
					var value = data.GetValue(attribute.Keys[i]);

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

/*
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
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Zongsoft.Data
{
	public class DataService<TEntity> : IDataService<TEntity>
	{
		#region 事件定义
		public event EventHandler<DataCountedEventArgs> Counted;
		public event EventHandler<DataCountingEventArgs> Counting;
		public event EventHandler<DataExecutedEventArgs> Executed;
		public event EventHandler<DataExecutingEventArgs> Executing;
		public event EventHandler<DataGettedEventArgs> Getted;
		public event EventHandler<DataGettingEventArgs> Getting;
		public event EventHandler<DataSelectedEventArgs> Selected;
		public event EventHandler<DataSelectingEventArgs> Selecting;
		public event EventHandler<DataDeletedEventArgs> Deleted;
		public event EventHandler<DataDeletingEventArgs> Deleting;
		public event EventHandler<DataInsertedEventArgs> Inserted;
		public event EventHandler<DataInsertingEventArgs> Inserting;
		public event EventHandler<DataUpdatedEventArgs> Updated;
		public event EventHandler<DataUpdatingEventArgs> Updating;
		#endregion

		#region 成员字段
		private string _name;
		private IDataAccess _dataAccess;
		private Zongsoft.Services.IServiceProvider _serviceProvider;
		#endregion

		#region 构造函数
		public DataService(Zongsoft.Services.IServiceProvider serviceProvider)
		{
			if(serviceProvider == null)
				throw new ArgumentNullException("serviceProvider");

			_serviceProvider = serviceProvider;
		}

		public DataService(string name, Zongsoft.Services.IServiceProvider serviceProvider)
		{
			if(string.IsNullOrWhiteSpace(name))
				throw new ArgumentNullException("name");

			if(serviceProvider == null)
				throw new ArgumentNullException("serviceProvider");

			_name = name.Trim();
			_serviceProvider = serviceProvider;
		}
		#endregion

		#region 公共属性
		public string Name
		{
			get
			{
				if(string.IsNullOrWhiteSpace(_name))
				{
					var attribute = (Metadata.DataEntityAttribute)Attribute.GetCustomAttribute(typeof(TEntity), typeof(Metadata.DataEntityAttribute));
					_name = attribute == null ? typeof(TEntity).Name : attribute.Name;
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

		[Zongsoft.Services.ServiceDependency]
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
		public IEnumerable<T> Execute<T>(IDictionary<string, object> inParameters)
		{
			IDictionary<string, object> outParameters;
			return this.Execute<T>(inParameters, out outParameters);
		}

		public virtual IEnumerable<T> Execute<T>(IDictionary<string, object> inParameters, out IDictionary<string, object> outParameters)
		{
			//激发“Executing”事件
			var args = this.OnExecuting(typeof(T), inParameters, out outParameters);

			if(args.Cancel)
				return args.Result as IEnumerable<T>;

			//执行数据操作方法
			args.Result = this.DataAccess.Execute<T>(this.Name, args.InParameters, out outParameters);

			//激发“Executed”事件
			return this.OnExecuted(typeof(T), args.InParameters, ref outParameters, args.Result) as IEnumerable<T>;
		}

		public object ExecuteScalar(IDictionary<string, object> inParameters)
		{
			IDictionary<string, object> outParameters;
			return this.ExecuteScalar(inParameters, out outParameters);
		}

		public virtual object ExecuteScalar(IDictionary<string, object> inParameters, out IDictionary<string, object> outParameters)
		{
			//激发“Executing”事件
			var args = this.OnExecuting(typeof(object), inParameters, out outParameters);

			if(args.Cancel)
				return args.Result;

			//执行数据操作方法
			args.Result = this.DataAccess.ExecuteScalar(this.Name, args.InParameters, out outParameters);

			//激发“Executed”事件
			return this.OnExecuted(typeof(object), args.InParameters, ref outParameters, args.Result);
		}
		#endregion

		#region 存在方法
		public virtual bool Exists(ICondition condition)
		{
			return this.DataAccess.Exists(this.Name, condition);
		}

		public virtual bool Exists<TKey>(TKey key)
		{
			return this.Exists(this.ConvertKey(key));
		}

		public virtual bool Exists<TKey1, TKey2>(TKey1 key1, TKey2 key2)
		{
			return this.Exists(this.ConvertKey(key1, key2));
		}

		public virtual bool Exists<TKey1, TKey2, TKey3>(TKey1 key1, TKey2 key2, TKey3 key3)
		{
			return this.Exists(this.ConvertKey(key1, key2, key3));
		}
		#endregion

		#region 计数方法
		public virtual int Count(ICondition condition, string includes = null)
		{
			//激发“Counting”事件
			var args = this.OnCounting(condition, includes);

			if(args.Cancel)
				return args.Result;

			//执行数据计数操作方法
			args.Result = this.DataAccess.Count(this.Name, args.Condition, args.Includes);

			//激发“Counted”事件
			return this.OnCounted(args.Condition, args.Includes, args.Result);
		}
		#endregion

		#region 递增方法
		public virtual long Increment(string name, ICondition condition, int interval = 1)
		{
			if(string.IsNullOrWhiteSpace(name))
				throw new ArgumentNullException("name");

			return this.DataAccess.Increment(this.Name, name, condition, interval);
		}

		public long Decrement(string name, ICondition condition, int interval = 1)
		{
			return this.Increment(name, condition, -interval);
		}
		#endregion

		#region 查询方法
		public object Get<TKey>(TKey key, params Sorting[] sortings)
		{
			return this.Get<TKey>(key, string.Empty, null, sortings);
		}

		public virtual object Get<TKey>(TKey key, string scope, Paging paging = null, params Sorting[] sortings)
		{
			return this.Get(this.ConvertKey(key), scope, paging, sortings, items => this.GetResult(items, new object[] { key }));
		}

		public object Get<TKey>(TKey key, Paging paging, string scope = null, params Sorting[] sortings)
		{
			return this.Get<TKey>(key, scope, paging, sortings);
		}

		public object Get<TKey1, TKey2>(TKey1 key1, TKey2 key2, params Sorting[] sortings)
		{
			return this.Get<TKey1, TKey2>(key1, key2, string.Empty, null, sortings);
		}

		public virtual object Get<TKey1, TKey2>(TKey1 key1, TKey2 key2, string scope, Paging paging = null, params Sorting[] sortings)
		{
			return this.Get(this.ConvertKey(key1, key2), scope, paging, sortings, items => this.GetResult(items, new object[] { key1, key2 }));
		}

		public object Get<TKey1, TKey2>(TKey1 key1, TKey2 key2, Paging paging, string scope = null, params Sorting[] sortings)
		{
			return this.Get<TKey1, TKey2>(key1, key2, scope, paging, sortings);
		}

		public object Get<TKey1, TKey2, TKey3>(TKey1 key1, TKey2 key2, TKey3 key3, params Sorting[] sortings)
		{
			return this.Get<TKey1, TKey2, TKey3>(key1, key2, key3, string.Empty, null, sortings);
		}

		public virtual object Get<TKey1, TKey2, TKey3>(TKey1 key1, TKey2 key2, TKey3 key3, string scope, Paging paging = null, params Sorting[] sortings)
		{
			return this.Get(this.ConvertKey(key1, key2, key3), scope, paging, sortings, items => this.GetResult(items, new object[] { key1, key2, key3 }));
		}

		public object Get<TKey1, TKey2, TKey3>(TKey1 key1, TKey2 key2, TKey3 key3, Paging paging, string scope = null, params Sorting[] sortings)
		{
			return this.Get<TKey1, TKey2, TKey3>(key1, key2, key3, scope, paging, sortings);
		}

		private object Get(ICondition condition, string scope, Paging paging, Sorting[] sortings, Func<IEnumerable<TEntity>, object> resultThunk)
		{
			//激发“Getting”事件
			var args = this.OnGetting(condition, scope, paging, sortings);

			if(args.Cancel)
				return args.Result;

			//执行数据获取操作方法
			var items = this.GetCore(args.Condition, args.Scope, args.Paging, args.Sortings);

			//进一步处理数据结果
			args.Result = resultThunk != null ? resultThunk(items) : items;

			//激发“Getted”事件
			return this.OnGetted(args.Condition, args.Scope, args.Paging, args.Sortings, args.Result);
		}

		protected virtual IEnumerable<TEntity> GetCore(ICondition condition, string scope, Paging paging, params Sorting[] sortings)
		{
			return this.DataAccess.Select<TEntity>(this.Name, condition, scope, paging, sortings);
		}

		public IEnumerable<TEntity> Select(ICondition condition = null, params Sorting[] sortings)
		{
			return this.Select(condition, null, string.Empty, null, sortings);
		}

		public IEnumerable<TEntity> Select(ICondition condition, string scope, params Sorting[] sortings)
		{
			return this.Select(condition, null, scope, null, sortings);
		}

		public IEnumerable<TEntity> Select(ICondition condition, string scope, Paging paging, params Sorting[] sortings)
		{
			return this.Select(condition, null, scope, paging, sortings);
		}

		public IEnumerable<TEntity> Select(ICondition condition, Paging paging, params Sorting[] sortings)
		{
			return this.Select(condition, null, null, paging, sortings);
		}

		public IEnumerable<TEntity> Select(ICondition condition, Paging paging, string scope, params Sorting[] sortings)
		{
			return this.Select(condition, null, scope, paging, sortings);
		}

		public IEnumerable<TEntity> Select(ICondition condition, Grouping grouping, params Sorting[] sortings)
		{
			return this.Select(condition, grouping, string.Empty, null, sortings);
		}

		public IEnumerable<TEntity> Select(ICondition condition, Grouping grouping, string scope, params Sorting[] sortings)
		{
			return this.Select(condition, grouping, scope, null, sortings);
		}

		public IEnumerable<TEntity> Select(ICondition condition, Grouping grouping, string scope, Paging paging, params Sorting[] sortings)
		{
			//激发“Selecting”事件
			var args = this.OnSelecting(typeof(TEntity), condition, grouping, scope, paging, sortings);

			if(args.Cancel)
				return args.Result as IEnumerable<TEntity>;

			//执行数据查询操作
			args.Result = this.SelectCore(args.Condition, args.Grouping, args.Scope, args.Paging, args.Sortings);

			//激发“Selected”事件
			return this.OnSelected(typeof(TEntity), args.Condition, args.Grouping, args.Scope, args.Paging, args.Sortings, (IEnumerable<TEntity>)args.Result);
		}

		public IEnumerable<TEntity> Select(ICondition condition, Grouping grouping, Paging paging, params Sorting[] sortings)
		{
			return this.Select(condition, grouping, null, paging, sortings);
		}

		public IEnumerable<TEntity> Select(ICondition condition, Grouping grouping, Paging paging, string scope, params Sorting[] sortings)
		{
			return this.Select(condition, grouping, scope, paging, sortings);
		}

		protected virtual IEnumerable<TEntity> SelectCore(ICondition condition, Grouping grouping, string scope, Paging paging, params Sorting[] sortings)
		{
			return this.DataAccess.Select<TEntity>(this.Name, condition, grouping, scope, paging, sortings);
		}
		#endregion

		#region 删除方法
		public virtual int Delete<TKey>(TKey key, params string[] cascades)
		{
			return this.Delete(this.ConvertKey(key), cascades);
		}

		public virtual int Delete<TKey1, TKey2>(TKey1 key1, TKey2 key2, params string[] cascades)
		{
			return this.Delete(this.ConvertKey(key1, key2), cascades);
		}

		public virtual int Delete<TKey1, TKey2, TKey3>(TKey1 key1, TKey2 key2, TKey3 key3, params string[] cascades)
		{
			return this.Delete(this.ConvertKey(key1, key2, key3), cascades);
		}

		public int Delete(ICondition condition, params string[] cascades)
		{
			//激发“Deleting”事件
			var args = this.OnDeleting(condition, cascades);

			if(args.Cancel)
				return args.Result;

			//执行数据删除操作
			args.Result = this.DeleteCore(args.Condition, args.Cascades);

			//激发“Deleted”事件
			return this.OnDeleted(args.Condition, args.Cascades, args.Result);
		}

		protected virtual int DeleteCore(ICondition condition, string[] cascades)
		{
			if(condition == null)
				throw new NotSupportedException("The condition cann't is null on delete operation.");

			return this.DataAccess.Delete(this.Name, condition, cascades);
		}
		#endregion

		#region 插入方法
		public int Insert(object data, string scope = null)
		{
			//激发“Inserting”事件
			var args = this.OnInserting(data, scope);

			if(args.Cancel)
				return args.Result;

			//执行数据插入操作
			args.Result = this.InsertCore(new DataDictionary<TEntity>(args.Data), args.Scope);

			//激发“Inserted”事件
			return this.OnInserted(args.Data, args.Scope, args.Result);
		}

		public int InsertMany(IEnumerable data, string scope = null)
		{
			//激发“Inserting”事件
			var args = this.OnInserting(data, scope);

			if(args.Cancel)
				return args.Result;

			//创建数据包裹器列表
			var items = new List<DataDictionary<TEntity>>();

			//设置数据包裹器列表的内容
			if(args.Data is IEnumerable)
			{
				foreach(var item in (IEnumerable)args.Data)
				{
					items.Add(new DataDictionary<TEntity>(item));
				}
			}
			else
			{
				items.Add(new DataDictionary<TEntity>(data));
			}

			//执行数据插入操作
			args.Result = this.InsertManyCore(items.ToArray(), args.Scope);

			//激发“Inserted”事件
			return this.OnInserted(args.Data, args.Scope, args.Result);
		}

		protected virtual int InsertCore(DataDictionary<TEntity> data, string scope)
		{
			if(data == null || data.Data == null)
				return 0;

			return this.DataAccess.Insert(this.Name, data.Data, scope);
		}

		protected virtual int InsertManyCore(IEnumerable<DataDictionary<TEntity>> items, string scope)
		{
			if(items == null)
				return 0;

			return this.DataAccess.InsertMany(this.Name, items.Select(p => p.Data), scope);
		}
		#endregion

		#region 更新方法
		public virtual int Update<TKey>(object data, TKey key, string scope = null)
		{
			return this.Update(data, this.ConvertKey(key), scope);
		}

		public virtual int Update<TKey1, TKey2>(object data, TKey1 key1, TKey2 key2, string scope = null)
		{
			return this.Update(data, this.ConvertKey(key1, key2), scope);
		}

		public virtual int Update<TKey1, TKey2, TKey3>(object data, TKey1 key1, TKey2 key2, TKey3 key3, string scope = null)
		{
			return this.Update(data, this.ConvertKey(key1, key2, key3), scope);
		}

		public virtual int UpdateMany<TKey>(IEnumerable data, TKey key, string scope = null)
		{
			return this.UpdateMany(data, this.ConvertKey(key), scope);
		}

		public virtual int UpdateMany<TKey1, TKey2>(IEnumerable data, TKey1 key1, TKey2 key2, string scope = null)
		{
			return this.UpdateMany(data, this.ConvertKey(key1, key2), scope);
		}

		public virtual int UpdateMany<TKey1, TKey2, TKey3>(IEnumerable data, TKey1 key1, TKey2 key2, TKey3 key3, string scope = null)
		{
			return this.UpdateMany(data, this.ConvertKey(key1, key2, key3), scope);
		}

		public int Update(object data, ICondition condition = null, string scope = null)
		{
			//激发“Updating”事件
			var args = this.OnUpdating(data, condition, scope);

			if(args.Cancel)
				return args.Result;

			//执行数据更新操作
			args.Result = this.UpdateCore(new DataDictionary<TEntity>(args.Data), args.Condition, args.Scope);

			//激发“Updated”事件
			return this.OnUpdated(args.Data, args.Condition, args.Scope, args.Result);
		}

		public int Update(object data, string scope, ICondition condition = null)
		{
			return this.Update(data, condition, scope);
		}

		public int UpdateMany(IEnumerable data, ICondition condition = null, string scope = null)
		{
			//激发“Updating”事件
			var args = this.OnUpdating(data, condition, scope);

			if(args.Cancel)
				return args.Result;

			//创建数据包裹器列表
			var items = new List<DataDictionary<TEntity>>();

			//设置数据包裹器列表的内容
			if(args.Data is IEnumerable)
			{
				foreach(var item in (IEnumerable)args.Data)
				{
					items.Add(new DataDictionary<TEntity>(item));
				}
			}
			else
			{
				items.Add(new DataDictionary<TEntity>(data));
			}

			//执行数据更新操作
			args.Result = this.UpdateManyCore(items.ToArray(), args.Condition, args.Scope);

			//激发“Updated”事件
			return this.OnUpdated(args.Data, args.Condition, args.Scope, args.Result);
		}

		public int UpdateMany(IEnumerable data, string scope, ICondition condition = null)
		{
			return this.UpdateMany(data, condition, scope);
		}

		protected virtual int UpdateCore(DataDictionary<TEntity> data, ICondition condition, string scope)
		{
			if(data == null || data.Data == null)
				return 0;

			return this.DataAccess.Update(this.Name, data.Data, condition, scope);
		}

		protected virtual int UpdateManyCore(IEnumerable<DataDictionary<TEntity>> items, ICondition condition, string scope)
		{
			if(items == null)
				return 0;

			return this.DataAccess.UpdateMany(this.Name, items.Select(p => p.Data), condition, scope);
		}
		#endregion

		#region 键值操作
		/// <summary>
		/// 根据指定的查询参数值获取对应的查询键值对数组。
		/// </summary>
		/// <param name="values">传入的查询值数组。</param>
		/// <returns>返回对应的键值对数组。</returns>
		/// <remarks>
		///		<para>基类的实现始终返回当前数据服务对应的主键的键值对数组。</para>
		///		<para>对于重载者的提示：如果<paramref name="values"/>参数值为空(null)或空数组(零长度)，则应返回当前实体的主键的键值对数组（调用基类的<see cref="GetKey(object[])"/>即可）。</para>
		/// </remarks>
		protected virtual Zongsoft.Collections.KeyValuePair[] GetKey(object[] values)
		{
			if(values == null || values.Length == 0)
				return null;

			var primaryKey = this.DataAccess.GetKey(this.Name);

			if(primaryKey == null || primaryKey.Length == 0)
				return null;

			var result = new Zongsoft.Collections.KeyValuePair[Math.Min(primaryKey.Length, values.Length)];

			for(int i = 0; i < result.Length; i++)
			{
				result[i] = new Collections.KeyValuePair(primaryKey[i], values[i]);
			}

			return result;
		}

		private ICondition ConvertKey<TKey>(TKey key)
		{
			return this.EnsureInquiryKey(new object[] { key });
		}

		private ICondition ConvertKey<TKey1, TKey2>(TKey1 key1, TKey2 key2)
		{
			return this.EnsureInquiryKey(new object[] { key1, key2 });
		}

		private ICondition ConvertKey<TKey1, TKey2, TKey3>(TKey1 key1, TKey2 key2, TKey3 key3)
		{
			return this.EnsureInquiryKey(new object[] { key1, key2, key3 });
		}
		#endregion

		#region 激发事件
		protected int OnCounted(ICondition condition, string includes, int result)
		{
			var args = new DataCountedEventArgs(this.Name, condition, includes, result);
			this.OnCounted(args);
			return args.Result;
		}

		protected DataCountingEventArgs OnCounting(ICondition condition, string includes)
		{
			var args = new DataCountingEventArgs(this.Name, condition, includes);
			this.OnCounting(args);
			return args;
		}

		protected object OnExecuted(Type resultType, IDictionary<string, object> inParameters, ref IDictionary<string, object> outParameters, object result)
		{
			var args = new DataExecutedEventArgs(this.Name, resultType, inParameters, null, result);
			this.OnExecuted(args);
			outParameters = args.OutParameters;
			return args.Result;
		}

		protected DataExecutingEventArgs OnExecuting(Type resultType, IDictionary<string, object> inParameters, out IDictionary<string, object> outParameters)
		{
			var args = new DataExecutingEventArgs(this.Name, resultType, inParameters);
			this.OnExecuting(args);
			outParameters = args.OutParameters;
			return args;
		}

		protected object OnGetted(ICondition condition, string scope, Paging paging, Sorting[] sortings, object result)
		{
			var args = new DataGettedEventArgs(this.Name, condition, scope, paging, sortings, result);
			this.OnGetted(args);
			return args.Result;
		}

		protected DataGettingEventArgs OnGetting(ICondition condition, string scope, Paging paging, Sorting[] sortings)
		{
			var args = new DataGettingEventArgs(this.Name, condition, scope, paging, sortings);
			this.OnGetting(args);
			return args;
		}

		protected IEnumerable<TEntity> OnSelected(Type entityType, ICondition condition, Grouping grouping, string scope, Paging paging, Sorting[] sortings, IEnumerable<TEntity> result)
		{
			var args = new DataSelectedEventArgs(this.Name, entityType, condition, grouping, scope, paging, sortings, result);
			this.OnSelected(args);
			return args.Result as IEnumerable<TEntity>;
		}

		protected DataSelectingEventArgs OnSelecting(Type entityType, ICondition condition, Grouping grouping, string scope, Paging paging, Sorting[] sortings)
		{
			var args = new DataSelectingEventArgs(this.Name, entityType, condition, grouping, scope, paging, sortings);
			this.OnSelecting(args);
			return args;
		}

		protected int OnDeleted(ICondition condition, string[] cascades, int result)
		{
			var args = new DataDeletedEventArgs(this.Name, condition, cascades, result);
			this.OnDeleted(args);
			return args.Result;
		}

		protected DataDeletingEventArgs OnDeleting(ICondition condition, string[] cascades)
		{
			var args = new DataDeletingEventArgs(this.Name, condition, cascades);
			this.OnDeleting(args);
			return args;
		}

		protected int OnInserted(object data, string scope, int result)
		{
			var args = new DataInsertedEventArgs(this.Name, data, scope, result);
			this.OnInserted(args);
			return args.Result;
		}

		protected DataInsertingEventArgs OnInserting(object data, string scope)
		{
			var args = new DataInsertingEventArgs(this.Name, data, scope);
			this.OnInserting(args);
			return args;
		}

		protected int OnUpdated(object data, ICondition condition, string scope, int result)
		{
			var args = new DataUpdatedEventArgs(this.Name, data, condition, scope, result);
			this.OnUpdated(args);
			return args.Result;
		}

		protected DataUpdatingEventArgs OnUpdating(object data, ICondition condition, string scope)
		{
			var args = new DataUpdatingEventArgs(this.Name, data, condition, scope);
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

		protected virtual void OnGetted(DataGettedEventArgs args)
		{
			var e = this.Getted;

			if(e != null)
				e(this, args);
		}

		protected virtual void OnGetting(DataGettingEventArgs args)
		{
			var e = this.Getting;

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
		private static bool IsNothing(object value)
		{
			if(value == null || System.Convert.IsDBNull(value))
				return true;

			if(value is string)
				return string.IsNullOrWhiteSpace((string)value);

			return false;
		}

		private ICondition EnsureInquiryKey(object[] values)
		{
			if(values != null && values.Length > 3)
				throw new NotSupportedException("Too many the keys.");

			//获取查询键值对数组
			var items = this.GetKey(values ?? new object[0]);

			if(items == null || items.Length == 0)
				return null;

			if(items.Length == 1)
			{
				if(IsNothing(items[0].Value))
					return null;

				return items[0].Value is ICondition ? (ICondition)items[0].Value : Condition.Equal(items[0].Key, items[0].Value);
			}

			var conditions = new ConditionCollection(ConditionCombination.And);

			foreach(var item in items)
			{
				if(!IsNothing(item.Value))
					conditions.Add(item.Value is ICondition ? (ICondition)item.Value : Condition.Equal(item.Key, item.Value));
			}

			if(conditions.Count > 1)
				return conditions;

			return conditions.FirstOrDefault();
		}

		private object GetResult(IEnumerable<TEntity> result, object[] values)
		{
			//获取当前数据服务对应的主键
			var primaryKey = this.DataAccess.GetKey(this.Name);

			//获取当前查询对应的查询键名称数组
			var inquiryKey = this.GetKey(values);

			//如果查询键与主键完全一致，则返回单数据（主键查询）
			if(this.CompareStringArray(primaryKey, inquiryKey.Select(p => p.Key).ToArray()))
				return result.FirstOrDefault();

			return result;
		}

		private bool CompareStringArray(string[] a, string[] b)
		{
			if((a == null || a.Length == 0) && (b == null || b.Length == 0))
				return true;

			if((a == null && b != null) || (a != null && b == null) || a.Length != b.Length)
				return false;

			for(int i = 0; i < a.Length; i++)
			{
				if(!Array.Exists(b, item => string.Equals(a[i], item, StringComparison.OrdinalIgnoreCase)))
					return false;
			}

			return true;
		}
		#endregion
	}
}

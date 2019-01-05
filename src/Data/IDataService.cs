/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2015-2017 Zongsoft Corporation <http://www.zongsoft.com>
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
	/// 表示数据服务的接口。
	/// </summary>
	public interface IDataService
	{
		#region 事件定义
		event EventHandler<DataCountedEventArgs> Counted;
		event EventHandler<DataCountingEventArgs> Counting;
		event EventHandler<DataExecutedEventArgs> Executed;
		event EventHandler<DataExecutingEventArgs> Executing;
		event EventHandler<DataExistedEventArgs> Existed;
		event EventHandler<DataExistingEventArgs> Existing;
		event EventHandler<DataIncrementedEventArgs> Incremented;
		event EventHandler<DataIncrementingEventArgs> Incrementing;
		event EventHandler<DataDeletedEventArgs> Deleted;
		event EventHandler<DataDeletingEventArgs> Deleting;
		event EventHandler<DataInsertedEventArgs> Inserted;
		event EventHandler<DataInsertingEventArgs> Inserting;
		event EventHandler<DataUpdatedEventArgs> Updated;
		event EventHandler<DataUpdatingEventArgs> Updating;
		event EventHandler<DataSelectedEventArgs> Selected;
		event EventHandler<DataSelectingEventArgs> Selecting;
		#endregion

		#region 属性定义
		/// <summary>
		/// 获取数据服务的名称，该名称亦为数据访问接口的调用名。
		/// </summary>
		string Name
		{
			get;
		}

		/// <summary>
		/// 获取数据访问接口。
		/// </summary>
		IDataAccess DataAccess
		{
			get;
		}
		#endregion

		#region 执行方法
		IEnumerable<T> Execute<T>(string name, IDictionary<string, object> inParameters, object state = null);
		IEnumerable<T> Execute<T>(string name, IDictionary<string, object> inParameters, out IDictionary<string, object> outParameters, object state = null);

		object ExecuteScalar(string name, IDictionary<string, object> inParameters, object state = null);
		object ExecuteScalar(string name, IDictionary<string, object> inParameters, out IDictionary<string, object> outParameters, object state = null);
		#endregion

		#region 存在方法
		bool Exists(ICondition condition, object state = null);

		bool Exists<TKey>(TKey key, object state = null);
		bool Exists<TKey1, TKey2>(TKey1 key1, TKey2 key2, object state = null);
		bool Exists<TKey1, TKey2, TKey3>(TKey1 key1, TKey2 key2, TKey3 key3, object state = null);
		#endregion

		#region 计数方法
		int Count(ICondition condition, object state);
		int Count(ICondition condition, string member);
		int Count(ICondition condition, string member = null, object state = null);
		#endregion

		#region 递增方法
		long Increment(string member, ICondition condition, object state);
		long Increment(string member, ICondition condition, int interval);
		long Increment(string member, ICondition condition, int interval = 1, object state = null);

		long Decrement(string member, ICondition condition, object state);
		long Decrement(string member, ICondition condition, int interval);
		long Decrement(string member, ICondition condition, int interval = 1, object state = null);
		#endregion

		#region 删除方法
		int Delete<TKey>(TKey key, string schema = null);
		int Delete<TKey>(TKey key, string schema, object state);
		int Delete<TKey1, TKey2>(TKey1 key1, TKey2 key2, string schema = null);
		int Delete<TKey1, TKey2>(TKey1 key1, TKey2 key2, string schema, object state);
		int Delete<TKey1, TKey2, TKey3>(TKey1 key1, TKey2 key2, TKey3 key3, string schema = null);
		int Delete<TKey1, TKey2, TKey3>(TKey1 key1, TKey2 key2, TKey3 key3, string schema, object state);

		int Delete(ICondition condition, string schema = null);
		int Delete(ICondition condition, string schema, object state);
		#endregion

		#region 插入方法
		int Insert(object data);
		int Insert(object data, object state);
		int Insert(object data, string schema);
		int Insert(object data, string schema, object state);

		int InsertMany(IEnumerable items);
		int InsertMany(IEnumerable items, object state);
		int InsertMany(IEnumerable items, string schema);
		int InsertMany(IEnumerable items, string schema, object state);
		#endregion

		#region 更新方法
		int Update<TKey>(object data, TKey key, object state = null);
		int Update<TKey>(object data, TKey key, string schema, object state = null);
		int Update<TKey1, TKey2>(object data, TKey1 key1, TKey2 key2, object state = null);
		int Update<TKey1, TKey2>(object data, TKey1 key1, TKey2 key2, string schema, object state = null);
		int Update<TKey1, TKey2, TKey3>(object data, TKey1 key1, TKey2 key2, TKey3 key3, object state = null);
		int Update<TKey1, TKey2, TKey3>(object data, TKey1 key1, TKey2 key2, TKey3 key3, string schema, object state = null);

		int Update(object data, object state = null);
		int Update(object data, string schema, object state = null);
		int Update(object data, ICondition condition, object state = null);
		int Update(object data, ICondition condition, string schema, object state = null);

		int UpdateMany(IEnumerable items, object state = null);
		int UpdateMany(IEnumerable items, string schema, object state = null);
		#endregion

		#region 查询方法
		object Search(string keyword, params Sorting[] sortings);
		object Search(string keyword, object state, params Sorting[] sortings);
		object Search(string keyword, Paging paging, params Sorting[] sortings);
		object Search(string keyword, Paging paging, object state, params Sorting[] sortings);
		object Search(string keyword, Paging paging, string schema, params Sorting[] sortings);
		object Search(string keyword, Paging paging, string schema, object state, params Sorting[] sortings);
		object Search(string keyword, string schema, params Sorting[] sortings);
		object Search(string keyword, string schema, object state, params Sorting[] sortings);
		object Search(string keyword, string schema, Paging paging, params Sorting[] sortings);
		object Search(string keyword, string schema, Paging paging, object state, params Sorting[] sortings);

		object Get<TKey>(TKey key, params Sorting[] sortings);
		object Get<TKey>(TKey key, object state, params Sorting[] sortings);
		object Get<TKey>(TKey key, Paging paging, params Sorting[] sortings);
		object Get<TKey>(TKey key, Paging paging, object state, params Sorting[] sortings);
		object Get<TKey>(TKey key, Paging paging, string schema, params Sorting[] sortings);
		object Get<TKey>(TKey key, Paging paging, string schema, object state, params Sorting[] sortings);
		object Get<TKey>(TKey key, string schema, params Sorting[] sortings);
		object Get<TKey>(TKey key, string schema, object state, params Sorting[] sortings);
		object Get<TKey>(TKey key, string schema, Paging paging, params Sorting[] sortings);
		object Get<TKey>(TKey key, string schema, Paging paging, object state, params Sorting[] sortings);

		object Get<TKey1, TKey2>(TKey1 key1, TKey2 key2, params Sorting[] sortings);
		object Get<TKey1, TKey2>(TKey1 key1, TKey2 key2, object state, params Sorting[] sortings);
		object Get<TKey1, TKey2>(TKey1 key1, TKey2 key2, Paging paging, params Sorting[] sortings);
		object Get<TKey1, TKey2>(TKey1 key1, TKey2 key2, Paging paging, object state, params Sorting[] sortings);
		object Get<TKey1, TKey2>(TKey1 key1, TKey2 key2, Paging paging, string schema, params Sorting[] sortings);
		object Get<TKey1, TKey2>(TKey1 key1, TKey2 key2, Paging paging, string schema, object state, params Sorting[] sortings);
		object Get<TKey1, TKey2>(TKey1 key1, TKey2 key2, string schema, params Sorting[] sortings);
		object Get<TKey1, TKey2>(TKey1 key1, TKey2 key2, string schema, object state, params Sorting[] sortings);
		object Get<TKey1, TKey2>(TKey1 key1, TKey2 key2, string schema, Paging paging, params Sorting[] sortings);
		object Get<TKey1, TKey2>(TKey1 key1, TKey2 key2, string schema, Paging paging, object state, params Sorting[] sortings);

		object Get<TKey1, TKey2, TKey3>(TKey1 key1, TKey2 key2, TKey3 key3, params Sorting[] sortings);
		object Get<TKey1, TKey2, TKey3>(TKey1 key1, TKey2 key2, TKey3 key3, object state, params Sorting[] sortings);
		object Get<TKey1, TKey2, TKey3>(TKey1 key1, TKey2 key2, TKey3 key3, Paging paging, params Sorting[] sortings);
		object Get<TKey1, TKey2, TKey3>(TKey1 key1, TKey2 key2, TKey3 key3, Paging paging, object state, params Sorting[] sortings);
		object Get<TKey1, TKey2, TKey3>(TKey1 key1, TKey2 key2, TKey3 key3, Paging paging, string schema, params Sorting[] sortings);
		object Get<TKey1, TKey2, TKey3>(TKey1 key1, TKey2 key2, TKey3 key3, Paging paging, string schema, object state, params Sorting[] sortings);
		object Get<TKey1, TKey2, TKey3>(TKey1 key1, TKey2 key2, TKey3 key3, string schema, params Sorting[] sortings);
		object Get<TKey1, TKey2, TKey3>(TKey1 key1, TKey2 key2, TKey3 key3, string schema, object state, params Sorting[] sortings);
		object Get<TKey1, TKey2, TKey3>(TKey1 key1, TKey2 key2, TKey3 key3, string schema, Paging paging, params Sorting[] sortings);
		object Get<TKey1, TKey2, TKey3>(TKey1 key1, TKey2 key2, TKey3 key3, string schema, Paging paging, object state, params Sorting[] sortings);

		IEnumerable Select(object state = null, params Sorting[] sortings);
		IEnumerable Select(ICondition condition, params Sorting[] sortings);
		IEnumerable Select(ICondition condition, object state, params Sorting[] sortings);
		IEnumerable Select(ICondition condition, Paging paging, params Sorting[] sortings);
		IEnumerable Select(ICondition condition, Paging paging, object state, params Sorting[] sortings);
		IEnumerable Select(ICondition condition, Paging paging, string schema, params Sorting[] sortings);
		IEnumerable Select(ICondition condition, Paging paging, string schema, object state, params Sorting[] sortings);
		IEnumerable Select(ICondition condition, string schema, params Sorting[] sortings);
		IEnumerable Select(ICondition condition, string schema, object state, params Sorting[] sortings);
		IEnumerable Select(ICondition condition, string schema, Paging paging, params Sorting[] sortings);
		IEnumerable Select(ICondition condition, string schema, Paging paging, object state, params Sorting[] sortings);

		//IEnumerable<T> Select<T>(Grouping grouping, object state = null, params Sorting[] sortings);
		//IEnumerable<T> Select<T>(Grouping grouping, ICondition condition, params Sorting[] sortings);
		//IEnumerable<T> Select<T>(Grouping grouping, ICondition condition, object state, params Sorting[] sortings);
		//IEnumerable<T> Select<T>(Grouping grouping, ICondition condition, Paging paging, params Sorting[] sortings);
		//IEnumerable<T> Select<T>(Grouping grouping, ICondition condition, Paging paging, object state, params Sorting[] sortings);

		IEnumerable<T> Select<T>(Grouping grouping, params Sorting[] sortings);
		IEnumerable<T> Select<T>(Grouping grouping, object state, params Sorting[] sortings);
		IEnumerable<T> Select<T>(Grouping grouping, string schema, object state = null, params Sorting[] sortings);
		IEnumerable<T> Select<T>(Grouping grouping, Paging paging, object state = null, params Sorting[] sortings);
		IEnumerable<T> Select<T>(Grouping grouping, string schema, Paging paging, object state = null, params Sorting[] sortings);
		IEnumerable<T> Select<T>(Grouping grouping, ICondition condition, params Sorting[] sortings);
		IEnumerable<T> Select<T>(Grouping grouping, ICondition condition, object state, params Sorting[] sortings);
		IEnumerable<T> Select<T>(Grouping grouping, ICondition condition, string schema, object state = null, params Sorting[] sortings);
		IEnumerable<T> Select<T>(Grouping grouping, ICondition condition, Paging paging, object state = null, params Sorting[] sortings);
		IEnumerable<T> Select<T>(Grouping grouping, ICondition condition, string schema, Paging paging, object state = null, params Sorting[] sortings);
		#endregion
	}
}

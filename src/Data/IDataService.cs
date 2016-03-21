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

namespace Zongsoft.Data
{
	/// <summary>
	/// 表示数据服务的接口。
	/// </summary>
	/// <typeparam name="TEntity">关于本服务对应的数据实体类型。</typeparam>
	public interface IDataService<TEntity>
	{
		#region 事件定义
		event EventHandler<DataCountedEventArgs> Counted;
		event EventHandler<DataCountingEventArgs> Counting;
		event EventHandler<DataExecutedEventArgs> Executed;
		event EventHandler<DataExecutingEventArgs> Executing;
		event EventHandler<DataGettedEventArgs> Getted;
		event EventHandler<DataGettingEventArgs> Getting;
		event EventHandler<DataSelectedEventArgs> Selected;
		event EventHandler<DataSelectingEventArgs> Selecting;
		event EventHandler<DataDeletedEventArgs> Deleted;
		event EventHandler<DataDeletingEventArgs> Deleting;
		event EventHandler<DataInsertedEventArgs> Inserted;
		event EventHandler<DataInsertingEventArgs> Inserting;
		event EventHandler<DataUpdatedEventArgs> Updated;
		event EventHandler<DataUpdatingEventArgs> Updating;
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
		object Execute(IDictionary<string, object> inParameters);
		object Execute(IDictionary<string, object> inParameters, out IDictionary<string, object> outParameters);
		#endregion

		#region 存在方法
		bool Exists(ICondition condition);
		#endregion

		#region 计数方法
		int Count(ICondition condition, string includes = null);
		#endregion

		#region 递增方法
		long Increment(string name, ICondition condition, int interval = 1);
		long Decrement(string name, ICondition condition, int interval = 1);
		#endregion

		#region 查询方法
		object Get<TKey>(TKey key, params Sorting[] sortings);
		object Get<TKey>(TKey key, string scope, Paging paging = null, params Sorting[] sortings);
		object Get<TKey>(TKey key, Paging paging, string scope = null, params Sorting[] sortings);

		object Get<TKey1, TKey2>(TKey1 key1, TKey2 key2, params Sorting[] sortings);
		object Get<TKey1, TKey2>(TKey1 key1, TKey2 key2, string scope, Paging paging = null, params Sorting[] sortings);
		object Get<TKey1, TKey2>(TKey1 key1, TKey2 key2, Paging paging, string scope = null, params Sorting[] sortings);

		object Get<TKey1, TKey2, TKey3>(TKey1 key1, TKey2 key2, TKey3 key3, params Sorting[] sortings);
		object Get<TKey1, TKey2, TKey3>(TKey1 key1, TKey2 key2, TKey3 key3, string scope, Paging paging = null, params Sorting[] sortings);
		object Get<TKey1, TKey2, TKey3>(TKey1 key1, TKey2 key2, TKey3 key3, Paging paging, string scope = null, params Sorting[] sortings);

		IEnumerable<TEntity> Select(ICondition condition = null, params Sorting[] sortings);
		IEnumerable<TEntity> Select(ICondition condition, string scope, params Sorting[] sortings);
		IEnumerable<TEntity> Select(ICondition condition, string scope, Paging paging, params Sorting[] sortings);
		IEnumerable<TEntity> Select(ICondition condition, Paging paging, params Sorting[] sortings);
		IEnumerable<TEntity> Select(ICondition condition, Paging paging, string scope, params Sorting[] sortings);
		IEnumerable<TEntity> Select(ICondition condition, Grouping grouping, params Sorting[] sortings);
		IEnumerable<TEntity> Select(ICondition condition, Grouping grouping, string scope, params Sorting[] sortings);
		IEnumerable<TEntity> Select(ICondition condition, Grouping grouping, string scope, Paging paging, params Sorting[] sortings);
		IEnumerable<TEntity> Select(ICondition condition, Grouping grouping, Paging paging, params Sorting[] sortings);
		IEnumerable<TEntity> Select(ICondition condition, Grouping grouping, Paging paging, string scope, params Sorting[] sortings);
		#endregion

		#region 删除方法
		int Delete<TKey>(TKey key, string cascades = null);
		int Delete<TKey1, TKey2>(TKey1 key1, TKey2 key2, string cascades = null);
		int Delete<TKey1, TKey2, TKey3>(TKey1 key1, TKey2 key2, TKey3 key3, string cascades = null);

		int Delete(ICondition condition, string cascades = null);
		#endregion

		#region 插入方法
		int Insert(object data, string scope = null);
		int InsertMany(IEnumerable<TEntity> data, string scope = null);
		#endregion

		#region 更新方法
		int Update<TKey>(object data, TKey key, string scope = null);
		int Update<TKey1, TKey2>(object data, TKey1 key1, TKey2 key2, string scope = null);
		int Update<TKey1, TKey2, TKey3>(object data, TKey1 key1, TKey2 key2, TKey3 key3, string scope = null);

		int UpdateMany<TKey>(IEnumerable<TEntity> data, TKey key, string scope = null);
		int UpdateMany<TKey1, TKey2>(IEnumerable<TEntity> data, TKey1 key1, TKey2 key2, string scope = null);
		int UpdateMany<TKey1, TKey2, TKey3>(IEnumerable<TEntity> data, TKey1 key1, TKey2 key2, TKey3 key3, string scope = null);

		int Update(object data, ICondition condition = null, string scope = null);
		int Update(object data, string scope, ICondition condition = null);

		int UpdateMany(IEnumerable<TEntity> data, ICondition condition = null, string scope = null);
		int UpdateMany(IEnumerable<TEntity> data, string scope, ICondition condition = null);
		#endregion
	}
}

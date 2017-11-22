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
	/// 表示数据访问的公共接口。
	/// </summary>
	/// <remarks>
	///		<para>关于“scope”查询参数的说明：</para>
	///		<para>表示要包含和排除的属性名列表，如果指定的是多个属性则属性名之间使用逗号(,)分隔；要排除的属性以感叹号(!)打头，单独一个星号(*)表示所有属性，单独一个感叹号(!)表示排除所有属性；如果未指定该参数则默认只会获取所有单值属性而不会获取导航属性。</para>
	/// </remarks>
	public interface IDataAccess
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

		#region 属性声明
		IDataAccessNaming Naming
		{
			get;
		}

		ICollection<IDataAccessFilter> Filters
		{
			get;
		}
		#endregion

		#region 获取主键
		string[] GetKey<T>();

		/// <summary>
		/// 获取指定名称的主键名数组。
		/// </summary>
		/// <param name="name">指定的数据映射名。</param>
		/// <returns>返回的主键名数组。</returns>
		string[] GetKey(string name);
		#endregion

		#region 执行方法
		IEnumerable<T> Execute<T>(string name, IDictionary<string, object> inParameters, IDictionary<string, object> states = null, Func<DataExecutionContext, bool> executing = null, Action<DataExecutionContext> executed = null);
		IEnumerable<T> Execute<T>(string name, IDictionary<string, object> inParameters, out IDictionary<string, object> outParameters, IDictionary<string, object> states = null, Func<DataExecutionContext, bool> executing = null, Action<DataExecutionContext> executed = null);

		object ExecuteScalar(string name, IDictionary<string, object> inParameters, IDictionary<string, object> states = null, Func<DataExecutionContext, bool> executing = null, Action<DataExecutionContext> executed = null);
		object ExecuteScalar(string name, IDictionary<string, object> inParameters, out IDictionary<string, object> outParameters, IDictionary<string, object> states = null, Func<DataExecutionContext, bool> executing = null, Action<DataExecutionContext> executed = null);
		#endregion

		#region 存在方法
		bool Exists<T>(ICondition condition, IDictionary<string, object> states = null, Func<DataExistenceContext, bool> existing = null, Action<DataExistenceContext> existed = null);
		bool Exists(string name, ICondition condition, IDictionary<string, object> states = null, Func<DataExistenceContext, bool> existing = null, Action<DataExistenceContext> existed = null);
		#endregion

		#region 计数方法
		int Count<T>(ICondition condition, string includes = null, IDictionary<string, object> states = null, Func<DataCountContext, bool> counting = null, Action<DataCountContext> counted = null);
		int Count(string name, ICondition condition, string includes = null, IDictionary<string, object> states = null, Func<DataCountContext, bool> counting = null, Action<DataCountContext> counted = null);
		#endregion

		#region 递增方法
		long Increment<T>(string member, ICondition condition, int interval = 1, IDictionary<string, object> states = null, Func<DataIncrementContext, bool> incrementing = null, Action<DataIncrementContext> incremented = null);
		long Increment(string name, string member, ICondition condition, int interval = 1, IDictionary<string, object> states = null, Func<DataIncrementContext, bool> incrementing = null, Action<DataIncrementContext> incremented = null);

		long Decrement<T>(string member, ICondition condition, int interval = 1, IDictionary<string, object> states = null, Func<DataIncrementContext, bool> decrementing = null, Action<DataIncrementContext> decremented = null);
		long Decrement(string name, string member, ICondition condition, int interval = 1, IDictionary<string, object> states = null, Func<DataIncrementContext, bool> decrementing = null, Action<DataIncrementContext> decremented = null);
		#endregion

		#region 删除方法
		int Delete<T>(ICondition condition, params string[] cascades);
		int Delete<T>(ICondition condition, IDictionary<string, object> states, params string[] cascades);
		int Delete<T>(ICondition condition, IDictionary<string, object> states, string[] cascades, Func<DataDeletionContext, bool> deleting, Action<DataDeletionContext> deleted);

		int Delete(string name, ICondition condition, params string[] cascades);
		int Delete(string name, ICondition condition, IDictionary<string, object> states, params string[] cascades);
		int Delete(string name, ICondition condition, IDictionary<string, object> states, string[] cascades, Func<DataDeletionContext, bool> deleting, Action<DataDeletionContext> deleted);
		#endregion

		#region 插入方法
		int Insert<T>(T data);
		int Insert<T>(T data, string scope);
		int Insert<T>(T data, IDictionary<string, object> states);
		int Insert<T>(T data, string scope, IDictionary<string, object> states, Func<DataInsertionContext, bool> inserting, Action<DataInsertionContext> inserted);

		int Insert(string name, object data);
		int Insert(string name, object data, string scope);
		int Insert(string name, object data, IDictionary<string, object> states);
		int Insert(string name, object data, string scope, IDictionary<string, object> states, Func<DataInsertionContext, bool> inserting, Action<DataInsertionContext> inserted);

		int InsertMany<T>(IEnumerable<T> items);
		int InsertMany<T>(IEnumerable<T> items, string scope);
		int InsertMany<T>(IEnumerable<T> items, IDictionary<string, object> states);
		int InsertMany<T>(IEnumerable<T> items, string scope, IDictionary<string, object> states, Func<DataInsertionContext, bool> inserting, Action<DataInsertionContext> inserted);

		int InsertMany(string name, IEnumerable items);
		int InsertMany(string name, IEnumerable items, string scope);
		int InsertMany(string name, IEnumerable items, IDictionary<string, object> states);
		int InsertMany(string name, IEnumerable items, string scope, IDictionary<string, object> states, Func<DataInsertionContext, bool> inserting, Action<DataInsertionContext> inserted);
		#endregion

		#region 更新方法
		int Update<T>(T data, IDictionary<string, object> states = null, Func<DataUpdationContext, bool> updating = null, Action<DataUpdationContext> updated = null);
		int Update<T>(T data, string scope, IDictionary<string, object> states = null, Func<DataUpdationContext, bool> updating = null, Action<DataUpdationContext> updated = null);
		int Update<T>(T data, ICondition condition, IDictionary<string, object> states = null, Func<DataUpdationContext, bool> updating = null, Action<DataUpdationContext> updated = null);
		int Update<T>(T data, ICondition condition, string scope, IDictionary<string, object> states = null, Func<DataUpdationContext, bool> updating = null, Action<DataUpdationContext> updated = null);

		int Update(string name, object data, IDictionary<string, object> states = null, Func<DataUpdationContext, bool> updating = null, Action<DataUpdationContext> updated = null);
		int Update(string name, object data, string scope, IDictionary<string, object> states = null, Func<DataUpdationContext, bool> updating = null, Action<DataUpdationContext> updated = null);
		int Update(string name, object data, ICondition condition, IDictionary<string, object> states = null, Func<DataUpdationContext, bool> updating = null, Action<DataUpdationContext> updated = null);
		int Update(string name, object data, ICondition condition, string scope, IDictionary<string, object> states = null, Func<DataUpdationContext, bool> updating = null, Action<DataUpdationContext> updated = null);

		int UpdateMany<T>(IEnumerable<T> items, IDictionary<string, object> states = null, Func<DataUpdationContext, bool> updating = null, Action<DataUpdationContext> updated = null);
		int UpdateMany<T>(IEnumerable<T> items, string scope, IDictionary<string, object> states = null, Func<DataUpdationContext, bool> updating = null, Action<DataUpdationContext> updated = null);

		int UpdateMany(string name, IEnumerable items, IDictionary<string, object> states = null, Func<DataUpdationContext, bool> updating = null, Action<DataUpdationContext> updated = null);
		int UpdateMany(string name, IEnumerable items, string scope, IDictionary<string, object> states = null, Func<DataUpdationContext, bool> updating = null, Action<DataUpdationContext> updated = null);
		#endregion

		#region 查询方法
		IEnumerable<T> Select<T>(IDictionary<string, object> states = null, params Sorting[] sortings);
		IEnumerable<T> Select<T>(ICondition condition, params Sorting[] sortings);
		IEnumerable<T> Select<T>(ICondition condition, IDictionary<string, object> states, params Sorting[] sortings);
		IEnumerable<T> Select<T>(ICondition condition, Paging paging, params Sorting[] sortings);
		IEnumerable<T> Select<T>(ICondition condition, Paging paging, IDictionary<string, object> states, params Sorting[] sortings);
		IEnumerable<T> Select<T>(ICondition condition, Paging paging, string scope, params Sorting[] sortings);
		IEnumerable<T> Select<T>(ICondition condition, Paging paging, string scope, IDictionary<string, object> states, params Sorting[] sortings);
		IEnumerable<T> Select<T>(ICondition condition, string scope, params Sorting[] sortings);
		IEnumerable<T> Select<T>(ICondition condition, string scope, IDictionary<string, object> states, params Sorting[] sortings);
		IEnumerable<T> Select<T>(ICondition condition, string scope, Paging paging, params Sorting[] sortings);
		IEnumerable<T> Select<T>(ICondition condition, string scope, Paging paging, IDictionary<string, object> states, params Sorting[] sortings);
		IEnumerable<T> Select<T>(ICondition condition, string scope, Paging paging, IDictionary<string, object> states, Sorting[] sortings, Func<DataSelectionContext, bool> selecting, Action<DataSelectionContext> selected);

		IEnumerable<T> Select<T>(string name, IDictionary<string, object> states = null, params Sorting[] sortings);
		IEnumerable<T> Select<T>(string name, ICondition condition, params Sorting[] sortings);
		IEnumerable<T> Select<T>(string name, ICondition condition, IDictionary<string, object> states, params Sorting[] sortings);
		IEnumerable<T> Select<T>(string name, ICondition condition, Paging paging, params Sorting[] sortings);
		IEnumerable<T> Select<T>(string name, ICondition condition, Paging paging, IDictionary<string, object> states, params Sorting[] sortings);
		IEnumerable<T> Select<T>(string name, ICondition condition, Paging paging, string scope, params Sorting[] sortings);
		IEnumerable<T> Select<T>(string name, ICondition condition, Paging paging, string scope, IDictionary<string, object> states, params Sorting[] sortings);
		IEnumerable<T> Select<T>(string name, ICondition condition, string scope, params Sorting[] sortings);
		IEnumerable<T> Select<T>(string name, ICondition condition, string scope, IDictionary<string, object> states, params Sorting[] sortings);
		IEnumerable<T> Select<T>(string name, ICondition condition, string scope, Paging paging, params Sorting[] sortings);
		IEnumerable<T> Select<T>(string name, ICondition condition, string scope, Paging paging, IDictionary<string, object> states, params Sorting[] sortings);
		IEnumerable<T> Select<T>(string name, ICondition condition, string scope, Paging paging, IDictionary<string, object> states, Sorting[] sortings, Func<DataSelectionContext, bool> selecting, Action<DataSelectionContext> selected);

		//IEnumerable<T> Select<T, TEntity>(Grouping<TEntity> grouping, ICondition condition = null, params Sorting[] sortings);
		//IEnumerable<T> Select<T, TEntity>(Grouping<TEntity> grouping, ICondition condition = null, Paging paging = null, params Sorting[] sortings);
		//IEnumerable<T> Select<T, TEntity>(Grouping<TEntity> grouping, ICondition condition, Paging paging, Sorting[] sortings, Func<DataSelectionContext, bool> selecting, Action<DataSelectionContext> selected);

		IEnumerable<T> Select<T>(string name, Grouping grouping, IDictionary<string, object> states = null, params Sorting[] sortings);
		IEnumerable<T> Select<T>(string name, Grouping grouping, ICondition condition, params Sorting[] sortings);
		IEnumerable<T> Select<T>(string name, Grouping grouping, ICondition condition, IDictionary<string, object> states, params Sorting[] sortings);
		IEnumerable<T> Select<T>(string name, Grouping grouping, ICondition condition, Paging paging, params Sorting[] sortings);
		IEnumerable<T> Select<T>(string name, Grouping grouping, ICondition condition, Paging paging, IDictionary<string, object> states, params Sorting[] sortings);
		IEnumerable<T> Select<T>(string name, Grouping grouping, ICondition condition, Paging paging, IDictionary<string, object> states, Sorting[] sortings, Func<DataSelectionContext, bool> selecting, Action<DataSelectionContext> selected);
		#endregion
	}
}

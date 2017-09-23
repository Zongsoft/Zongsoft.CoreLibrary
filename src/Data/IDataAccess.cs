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
	///		<para>表示要包含和排除的属性名列表，如果指定的是多个属性则属性名之间使用逗号(,)分隔；要排除的属性以减号(!)打头，单独一个星号(*)表示所有属性，单独一个感叹号(!)表示排除所有属性；如果未指定该参数则默认只会获取所有单值属性而不会获取导航属性。</para>
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
		event EventHandler<DataDecrementedEventArgs> Decremented;
		event EventHandler<DataDecrementingEventArgs> Decrementing;
		event EventHandler<DataSelectedEventArgs> Selected;
		event EventHandler<DataSelectingEventArgs> Selecting;
		event EventHandler<DataDeletedEventArgs> Deleted;
		event EventHandler<DataDeletingEventArgs> Deleting;
		event EventHandler<DataInsertedEventArgs> Inserted;
		event EventHandler<DataInsertingEventArgs> Inserting;
		event EventHandler<DataManyInsertedEventArgs> ManyInserted;
		event EventHandler<DataManyInsertingEventArgs> ManyInserting;
		event EventHandler<DataUpdatedEventArgs> Updated;
		event EventHandler<DataUpdatingEventArgs> Updating;
		event EventHandler<DataManyUpdatedEventArgs> ManyUpdated;
		event EventHandler<DataManyUpdatingEventArgs> ManyUpdating;
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
		IEnumerable<T> Execute<T>(string name, IDictionary<string, object> inParameters);
		IEnumerable<T> Execute<T>(string name, IDictionary<string, object> inParameters, out IDictionary<string, object> outParameters);

		object ExecuteScalar(string name, IDictionary<string, object> inParameters);
		object ExecuteScalar(string name, IDictionary<string, object> inParameters, out IDictionary<string, object> outParameters);
		#endregion

		#region 存在方法
		bool Exists<T>(ICondition condition);
		bool Exists(string name, ICondition condition);
		#endregion

		#region 计数方法
		int Count<T>(ICondition condition, string includes = null);
		int Count(string name, ICondition condition, string includes = null);
		#endregion

		#region 递增方法
		long Increment<T>(string member, ICondition condition, int interval = 1);
		long Increment(string name, string member, ICondition condition, int interval = 1);

		long Decrement<T>(string member, ICondition condition, int interval = 1);
		long Decrement(string name, string member, ICondition condition, int interval = 1);
		#endregion

		#region 查询方法
		IEnumerable<T> Select<T>(ICondition condition = null, params Sorting[] sortings);
		IEnumerable<T> Select<T>(ICondition condition, string scope, params Sorting[] sortings);
		IEnumerable<T> Select<T>(ICondition condition, string scope, Paging paging, params Sorting[] sortings);
		IEnumerable<T> Select<T>(ICondition condition, Paging paging, params Sorting[] sortings);
		IEnumerable<T> Select<T>(ICondition condition, Paging paging, string scope, params Sorting[] sortings);
		IEnumerable<T> Select<T>(ICondition condition, Grouping grouping, params Sorting[] sortings);
		IEnumerable<T> Select<T>(ICondition condition, Grouping grouping, string scope, params Sorting[] sortings);
		IEnumerable<T> Select<T>(ICondition condition, Grouping grouping, string scope, Paging paging, params Sorting[] sortings);
		IEnumerable<T> Select<T>(ICondition condition, Grouping grouping, Paging paging, params Sorting[] sortings);
		IEnumerable<T> Select<T>(ICondition condition, Grouping grouping, Paging paging, string scope, params Sorting[] sortings);

		IEnumerable<T> Select<T>(string name, ICondition condition = null, params Sorting[] sortings);
		IEnumerable<T> Select<T>(string name, ICondition condition, string scope, params Sorting[] sortings);
		IEnumerable<T> Select<T>(string name, ICondition condition, string scope, Paging paging, params Sorting[] sortings);
		IEnumerable<T> Select<T>(string name, ICondition condition, Paging paging, params Sorting[] sortings);
		IEnumerable<T> Select<T>(string name, ICondition condition, Paging paging, string scope, params Sorting[] sortings);
		IEnumerable<T> Select<T>(string name, ICondition condition, Grouping grouping, params Sorting[] sortings);
		IEnumerable<T> Select<T>(string name, ICondition condition, Grouping grouping, string scope, params Sorting[] sortings);
		IEnumerable<T> Select<T>(string name, ICondition condition, Grouping grouping, string scope, Paging paging, params Sorting[] sortings);
		IEnumerable<T> Select<T>(string name, ICondition condition, Grouping grouping, Paging paging, params Sorting[] sortings);
		IEnumerable<T> Select<T>(string name, ICondition condition, Grouping grouping, Paging paging, string scope, params Sorting[] sortings);
		#endregion

		#region 删除方法
		int Delete<T>(ICondition condition, params string[] cascades);

		int Delete(string name, ICondition condition, params string[] cascades);
		#endregion

		#region 插入方法
		int Insert<T>(T data, string scope = null);
		int InsertMany<T>(IEnumerable<T> items, string scope = null);

		int Insert(string name, object data, string scope = null);
		int InsertMany(string name, IEnumerable items, string scope = null);
		#endregion

		#region 更新方法
		int Update<T>(T data, ICondition condition = null, string scope = null);
		int Update<T>(T data, string scope, ICondition condition = null);

		/// <summary>
		/// 根据指定的条件将指定的实体更新到数据源。
		/// </summary>
		/// <param name="name">指定的实体映射名。</param>
		/// <param name="data">要更新的数据实体。</param>
		/// <param name="condition">要更新的条件子句，如果为空(null)则根据实体的主键进行更新。</param>
		/// <param name="scope">指定的要更新的和排除更新的属性名列表，如果指定的是多个属性则属性名之间使用逗号(,)分隔；要排除的属性以减号(-)打头，星号(*)表示所有属性，感叹号(!)表示排除所有属性；如果未指定该参数则默认只会更新所有单值属性而不会更新导航属性。</param>
		/// <returns>返回受影响的记录行数，执行成功返回大于零的整数，失败则返回负数。</returns>
		int Update(string name, object data, ICondition condition = null, string scope = null);
		int Update(string name, object data, string scope, ICondition condition = null);

		int UpdateMany<T>(IEnumerable<T> items, ICondition condition = null, string scope = null);
		int UpdateMany<T>(IEnumerable<T> items, string scope, ICondition condition = null);

		/// <summary>
		/// 根据指定的条件将指定的实体集更新到数据源。
		/// </summary>
		/// <param name="name">指定的实体映射名。</param>
		/// <param name="items">要更新的数据集。</param>
		/// <param name="condition">要更新的条件子句，如果为空(null)则根据实体的主键进行更新。</param>
		/// <param name="scope">指定的要更新的和排除更新的属性名列表，如果指定的是多个属性则属性名之间使用逗号(,)分隔；要排除的属性以减号(-)打头，星号(*)表示所有属性，感叹号(!)表示排除所有属性；如果未指定该参数则默认只会更新所有单值属性而不会更新导航属性。</param>
		/// <returns>返回受影响的记录行数，执行成功返回大于零的整数，失败则返回负数。</returns>
		int UpdateMany(string name, IEnumerable items, ICondition condition = null, string scope = null);
		int UpdateMany(string name, IEnumerable items, string scope, ICondition condition = null);
		#endregion
	}
}

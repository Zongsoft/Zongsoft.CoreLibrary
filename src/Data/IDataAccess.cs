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
using System.Linq.Expressions;

namespace Zongsoft.Data
{
	/// <summary>
	/// 表示数据访问的公共接口。
	/// </summary>
	/// <remarks>
	///		<para>关于“scope”查询参数的说明：</para>
	///		<para>表示要包含和排除的属性名列表，如果指定的是多个属性则属性名之间使用逗号(,)分隔；要排除的属性以减号(-)打头，星号(*)表示所有属性，感叹号(!)表示排除所有属性；如果未指定该参数则默认只会获取所有单值属性而不会获取导航属性。</para>
	/// </remarks>
	public interface IDataAccess
	{
		#region 事件定义
		event EventHandler<DataCountedEventArgs> Counted;
		event EventHandler<DataCountingEventArgs> Counting;
		event EventHandler<DataExecutedEventArgs> Executed;
		event EventHandler<DataExecutingEventArgs> Executing;
		event EventHandler<DataSelectedEventArgs> Selected;
		event EventHandler<DataSelectingEventArgs> Selecting;
		event EventHandler<DataDeletedEventArgs> Deleted;
		event EventHandler<DataDeletingEventArgs> Deleting;
		event EventHandler<DataInsertedEventArgs> Inserted;
		event EventHandler<DataInsertingEventArgs> Inserting;
		event EventHandler<DataUpdatedEventArgs> Updated;
		event EventHandler<DataUpdatingEventArgs> Updating;
		#endregion

		#region 执行方法
		object Execute(string name, IDictionary<string, object> inParameters);
		object Execute(string name, IDictionary<string, object> inParameters, out IDictionary<string, object> outParameters);
		#endregion

		#region 存在方法
		bool Exists(string name, ICondition condition);
		#endregion

		#region 计数方法
		int Count(string name, ICondition condition, string includes = null);
		#endregion

		#region 查询方法
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
		int Delete(string name, ICondition condition, string cascades = null);
		#endregion

		#region 插入方法
		int Insert(string name, object data, string scope = null);

		int InsertMany<T>(string name, IEnumerable<T> data, string scope = null);
		#endregion

		#region 更新方法
		/// <summary>
		/// 根据指定的条件将指定的实体更新到数据源。
		/// </summary>
		/// <param name="name">指定的实体映射名。</param>
		/// <param name="data">要更新的数据实体。</param>
		/// <param name="condition">要更新的条件子句，如果为空(null)则根据实体的主键进行更新。</param>
		/// <param name="scope">指定的要更新的和排除更新的属性名列表，如果指定的是多个属性则属性名之间使用逗号(,)分隔；要排除的属性以减号(-)打头，星号(*)表示所有属性，感叹号(!)表示排除所有属性；如果未指定该参数则默认只会更新所有单值属性而不会更新导航属性。</param>
		/// <returns>返回受影响的记录行数，执行成功返回大于零的整数，失败则返回负数。</returns>
		int Update(string name, object data, ICondition condition = null, string scope = null);

		/// <summary>
		/// 根据指定的条件将指定的实体集更新到数据源。
		/// </summary>
		/// <typeparam name="T">指定的实体集中的实体的类型。</typeparam>
		/// <param name="name">指定的实体映射名。</param>
		/// <param name="data">要更新的数据实体集。</param>
		/// <param name="condition">要更新的条件子句，如果为空(null)则根据实体的主键进行更新。</param>
		/// <param name="scope">指定的要更新的和排除更新的属性名列表，如果指定的是多个属性则属性名之间使用逗号(,)分隔；要排除的属性以减号(-)打头，星号(*)表示所有属性，感叹号(!)表示排除所有属性；如果未指定该参数则默认只会更新所有单值属性而不会更新导航属性。</param>
		/// <returns>返回受影响的记录行数，执行成功返回大于零的整数，失败则返回负数。</returns>
		int UpdateMany<T>(string name, IEnumerable<T> data, ICondition condition = null, string scope = null);
		#endregion
	}
}

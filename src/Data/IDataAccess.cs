/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2010-2014 Zongsoft Corporation <http://www.zongsoft.com>
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
	public interface IDataAccess
	{
		#region 执行方法
		object Execute(string name, IDictionary<string, object> inParameters);
		object Execute(string name, IDictionary<string, object> inParameters, out IDictionary<string, object> outParameters);
		#endregion

		#region 计数方法
		int Count(string name, ICondition condition);
		int Count(string name, ICondition condition, string includes);
		int Count<T>(string name, ICondition condition, Expression<Func<T, object>> includes);
		#endregion

		#region 查询方法
		IEnumerable<T> Select<T>(string name, ICondition condition = null);

		/// <summary>
		/// 执行指定名称的数据查询操作。
		/// </summary>
		/// <param name="name">指定的查询名称，对应数据映射的名称。</param>
		/// <param name="condition">指定的查询条件。</param>
		/// <param name="scope">指定的要获取的和排除获取的属性名列表，如果指定的是多个属性则属性名之间使用逗号(,)分隔；要排除的属性以减号(-)打头，星号(*)表示所有属性，感叹号(!)表示排除所有属性；如果未指定该参数则默认只会获取所有单值属性而不会获取导航属性。</param>
		/// <param name="paging">指定的分页设置。</param>
		/// <param name="grouping">指定的分组设置。</param>
		/// <param name="sorting">指定的排序设置(包括排序的方式和字段)。</param>
		/// <returns>返回的结果集。</returns>
		/// <remarks>
		///		<example>
		///		<code>
		///		Select("VehiclePassing",
		///		       new ClauseCollection(ClauseCombine.And)
		///		       {
		///		           new Clause("PlateNo", "粤B12345"),
		///		           new Clause("Timestamp", ClauseOperator.Between, DateTime.Parse("2014-1-1"), DateTime.Parse("2014-1-31")),
		///		           new ClauseCollection(ClauseCombine.Or)
		///		           {
		///		               new Clause("Speed", ClauseOperator.GreaterThanEqual, 80),
		///		               new Clause("PlateColor", PlateColor.Blue),
		///		           }
		///		       },
		///		       "Creator.HomeAddress, Corssing, -Owner.PhoneNumber",
		///		       new Paging(1, 20),
		///		       null,
		///		       Sorting.Ascending("Timestamp"));
		///		</code>
		///		</example>
		/// </remarks>
		IEnumerable Select(string name,
		                   ICondition condition,
		                   string scope = null,
		                   Paging paging = null,
		                   Grouping grouping = null,
		                   params Sorting[] sorting);

		IEnumerable<T> Select<T>(string name,
		                         ICondition condition,
		                         string scope = null,
		                         Paging paging = null,
		                         Grouping grouping = null,
		                         params Sorting[] sorting);

		IEnumerable<T> Select<T>(string name,
		                         ICondition condition,
		                         Expression<Func<T, object>> includes = null,
		                         Expression<Func<T, object>> excludes = null,
		                         Paging paging = null,
		                         Grouping grouping = null,
		                         params Sorting[] sorting);
		#endregion

		#region 删除方法
		int Delete(string name, ICondition condition);
		int Delete(string name, ICondition condition, string cascades);
		int Delete<T>(string name, ICondition condition, Expression<Func<T, object>> cascades);
		#endregion

		#region 插入方法
		int Insert(string name, object entity, string scope = null);
		int Insert<T>(string name, T entity, Expression<Func<T, object>> includes, Expression<Func<T, object>> excludes);

		int Insert<T>(string name, IEnumerable<T> entities, string scope = null);
		int Insert<T>(string name, IEnumerable<T> entities, Expression<Func<T, object>> includes, Expression<Func<T, object>> excludes = null);
		#endregion

		#region 更新方法
		int Update(string name, object entity, ICondition condition = null);

		/// <summary>
		/// 根据指定的条件将指定的实体更新到数据源。
		/// </summary>
		/// <param name="name">指定的实体映射名。</param>
		/// <param name="entity">要更新的实体对象。</param>
		/// <param name="condition">要更新的条件子句，如果为空(null)则根据实体的主键进行更新。</param>
		/// <param name="scope">指定的要更新的和排除更新的属性名列表，如果指定的是多个属性则属性名之间使用逗号(,)分隔；要排除的属性以减号(-)打头，星号(*)表示所有属性，感叹号(!)表示排除所有属性；如果未指定该参数则默认只会更新所有单值属性而不会更新导航属性。</param>
		/// <returns>返回受影响的记录行数，执行成功返回大于零的整数，失败则返回负数。</returns>
		int Update(string name, object entity, ICondition condition, string scope = null);

		int Update<T>(string name, T entity, ICondition condition, Expression<Func<T, object>> includes = null, Expression<Func<T, object>> excludes = null);

		int Update<T>(string name, IEnumerable<T> entities, ICondition condition = null);

		/// <summary>
		/// 根据指定的条件将指定的实体集更新到数据源。
		/// </summary>
		/// <typeparam name="T">指定的实体集中的实体的类型。</typeparam>
		/// <param name="name">指定的实体映射名。</param>
		/// <param name="entities">要更新的实体集。</param>
		/// <param name="condition">要更新的条件子句，如果为空(null)则根据实体的主键进行更新。</param>
		/// <param name="scope">指定的要更新的和排除更新的属性名列表，如果指定的是多个属性则属性名之间使用逗号(,)分隔；要排除的属性以减号(-)打头，星号(*)表示所有属性，感叹号(!)表示排除所有属性；如果未指定该参数则默认只会更新所有单值属性而不会更新导航属性。</param>
		/// <returns>返回受影响的记录行数，执行成功返回大于零的整数，失败则返回负数。</returns>
		int Update<T>(string name, IEnumerable<T> entities, ICondition condition, string scope = null);

		int Update<T>(string name, IEnumerable<T> entities, ICondition condition, Expression<Func<T, object>> includes = null, Expression<Func<T, object>> excludes = null);
		#endregion
	}
}

/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2010-2013 Zongsoft Corporation <http://www.zongsoft.com>
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

namespace Zongsoft.Data.Entities
{
	/// <summary>
	/// 表示实体数据访问的公共接口。
	/// </summary>
	public interface IObjectAccess
	{
		#region 执行方法
		object Execute(string name, IDictionary<string, object> inParameters);
		object Execute(string name, IDictionary<string, object> inParameters, out IDictionary<string, object> outParameters);
		#endregion

		#region 查询方法
		/// <summary>
		/// 执行指定名称的数据查询操作。
		/// </summary>
		/// <param name="name">指定的查询名称，对应数据映射的名称。</param>
		/// <param name="where">指定的查询条件。</param>
		/// <param name="paging">指定的分页设置。</param>
		/// <param name="sorting">指定的排序字段和排序方式。</param>
		/// <param name="includes">指定要包含的结果实体中的导航属性名。</param>
		/// <returns>返回的结果集。</returns>
		/// <remarks>
		///		<code>
		///		Select("VehiclePassing",
		///		       new ClauseCollection(ClauseCombine.And)
		///		       {
		///		           new Clause("PlateNo", "鄂A12345"),
		///		           new Clause("Timestamp", ClauseOperator.Between, DateTime.Parse("2014-1-1"), DateTime.Parse("2014-1-31")),
		///		           new ClauseCollection(ClauseCombine.Or)
		///		           {
		///		               new Clause("Speed", ClauseOperator.GreaterThanEqual, 80),
		///		               new Clause("PlateColor", PlateColor.Blue),
		///		           }
		///		       },
		///		       new Paging(10, 20),
		///		       new Sorting[]{new Sorting(SortingMode.Ascending, "Timestamp")},
		///		       new string[]{"Creator.HomeAddress", "Corssing"},
		///		       null);
		///		</code>
		/// </remarks>
		IEnumerable Select(string name,
		                   IClause where = null,
		                   Paging paging = null,
		                   Sorting[] sorting = null,
		                   string[] includes = null);

		IEnumerable<TEntity> Select<TEntity>(string name,
						   IClause where = null,
						   Paging paging = null,
						   Sorting[] sorting = null,
						   string[] includes = null);
		#endregion

		#region 删除方法
		int Delete(string name, IClause where);
		int Delete(string name, IDictionary<string, object> where);
		int Delete(string name, object where);
		#endregion

		#region 插入方法
		int Insert(string name, object entity);
		int Insert(string name, IEnumerable entities);
		#endregion

		#region 更新方法
		/// <summary>
		/// 更新指定实体到数据源，更新的依据为指定<paramref name="name"/>映射的主键值。
		/// </summary>
		/// <param name="name">指定的实体映射名。</param>
		/// <param name="entity">要更新的实体对象。</param>
		/// <returns>返回受影响的记录行数，本方法执行成功返回1，失败则返回零。</returns>
		int Update(string name, object entity);

		/// <summary>
		/// 根据指定的条件将指定的实体更新到数据源。
		/// </summary>
		/// <param name="name">指定的实体映射名。</param>
		/// <param name="entity">要更新的实体对象。</param>
		/// <param name="where">要更新的条件子句。</param>
		/// <returns>返回受影响的记录行数，执行成功返回大于零的整数，失败则返回零。</returns>
		int Update(string name, object entity, IClause where);

		int Update(string name, object entity, IDictionary<string, object> where);
		int Update(string name, object entity, object where);
		#endregion
	}
}

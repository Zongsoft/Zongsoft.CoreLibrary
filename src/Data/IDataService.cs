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
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Zongsoft.Data
{
	public interface IDataService<TEntity>
	{
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
		int Count(ICondition condition, Expression<Func<TEntity, object>> includes);
		#endregion

		#region 查询方法
		object Get<TKey>(TKey key, params Sorting[] sorting);
		object Get<TKey>(TKey key, string scope, Paging paging = null, params Sorting[] sorting);
		object Get<TKey>(TKey key, Paging paging, string scope = null, params Sorting[] sorting);

		object Get<TKey1, TKey2>(TKey1 key1, TKey2 key2, params Sorting[] sorting);
		object Get<TKey1, TKey2>(TKey1 key1, TKey2 key2, string scope, Paging paging = null, params Sorting[] sorting);
		object Get<TKey1, TKey2>(TKey1 key1, TKey2 key2, Paging paging, string scope = null, params Sorting[] sorting);

		object Get<TKey1, TKey2, TKey3>(TKey1 key1, TKey2 key2, TKey3 key3, params Sorting[] sorting);
		object Get<TKey1, TKey2, TKey3>(TKey1 key1, TKey2 key2, TKey3 key3, string scope, Paging paging = null, params Sorting[] sorting);
		object Get<TKey1, TKey2, TKey3>(TKey1 key1, TKey2 key2, TKey3 key3, Paging paging, string scope = null, params Sorting[] sorting);

		IEnumerable<TEntity> Select(ICondition condition = null, params Sorting[] sorting);
		IEnumerable<TEntity> Select(ICondition condition, string scope, params Sorting[] sorting);
		IEnumerable<TEntity> Select(ICondition condition, string scope, Paging paging, params Sorting[] sorting);
		IEnumerable<TEntity> Select(ICondition condition, Paging paging, params Sorting[] sorting);
		IEnumerable<TEntity> Select(ICondition condition, Paging paging, string scope, params Sorting[] sorting);
		IEnumerable<TEntity> Select(ICondition condition, Grouping grouping, params Sorting[] sorting);
		IEnumerable<TEntity> Select(ICondition condition, Grouping grouping, string scope, params Sorting[] sorting);
		IEnumerable<TEntity> Select(ICondition condition, Grouping grouping, string scope, Paging paging, params Sorting[] sorting);
		IEnumerable<TEntity> Select(ICondition condition, Grouping grouping, Paging paging, params Sorting[] sorting);
		IEnumerable<TEntity> Select(ICondition condition, Grouping grouping, Paging paging, string scope, params Sorting[] sorting);
		#endregion

		#region 删除方法
		int Delete<TKey>(TKey key);
		int Delete<TKey1, TKey2>(TKey1 key1, TKey2 key2);
		int Delete<TKey1, TKey2, TKey3>(TKey1 key1, TKey2 key2, TKey3 key3);

		int Delete(ICondition condition);
		int Delete(ICondition condition, string cascades);
		int Delete(ICondition condition, Expression<Func<TEntity, object>> cascades);
		#endregion

		#region 插入方法
		int Insert(TEntity entity, string scope = null);
		int Insert(TEntity entity, Expression<Func<TEntity, object>> includes, Expression<Func<TEntity, object>> excludes);

		int Insert(IEnumerable<TEntity> entities, string scope = null);
		int Insert(IEnumerable<TEntity> entities, Expression<Func<TEntity, object>> includes, Expression<Func<TEntity, object>> excludes = null);
		#endregion

		#region 更新方法
		int Update(TEntity entity, ICondition condition = null, string scope = null);
		int Update(TEntity entity, ICondition condition, Expression<Func<TEntity, object>> includes, Expression<Func<TEntity, object>> excludes = null);

		int Update(IEnumerable<TEntity> entities, ICondition condition = null, string scope = null);
		int Update(IEnumerable<TEntity> entities, ICondition condition, Expression<Func<TEntity, object>> includes, Expression<Func<TEntity, object>> excludes = null);
		#endregion
	}
}

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
	/// <typeparam name="TEntity">关于本服务对应的数据实体类型。</typeparam>
	public interface IDataService<TEntity> : IDataService
	{
		#region 事件定义
		event EventHandler<DataGettedEventArgs<TEntity>> Getted;
		event EventHandler<DataGettingEventArgs<TEntity>> Getting;
		#endregion

		#region 查询方法
		IEnumerable<TEntity> Select(IDictionary<string, object> states = null, params Sorting[] sortings);
		IEnumerable<TEntity> Select(ICondition condition, params Sorting[] sortings);
		IEnumerable<TEntity> Select(ICondition condition, IDictionary<string, object> states, params Sorting[] sortings);
		IEnumerable<TEntity> Select(ICondition condition, Paging paging, params Sorting[] sortings);
		IEnumerable<TEntity> Select(ICondition condition, Paging paging, IDictionary<string, object> states, params Sorting[] sortings);
		IEnumerable<TEntity> Select(ICondition condition, Paging paging, string scope, params Sorting[] sortings);
		IEnumerable<TEntity> Select(ICondition condition, Paging paging, string scope, IDictionary<string, object> states, params Sorting[] sortings);
		IEnumerable<TEntity> Select(ICondition condition, string scope, params Sorting[] sortings);
		IEnumerable<TEntity> Select(ICondition condition, string scope, IDictionary<string, object> states, params Sorting[] sortings);
		IEnumerable<TEntity> Select(ICondition condition, string scope, Paging paging, params Sorting[] sortings);
		IEnumerable<TEntity> Select(ICondition condition, string scope, Paging paging, IDictionary<string, object> states, params Sorting[] sortings);
		#endregion
	}
}

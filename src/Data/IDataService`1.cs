/*
 *   _____                                ______
 *  /_   /  ____  ____  ____  _________  / __/ /_
 *    / /  / __ \/ __ \/ __ \/ ___/ __ \/ /_/ __/
 *   / /__/ /_/ / / / / /_/ /\_ \/ /_/ / __/ /_
 *  /____/\____/_/ /_/\__  /____/\____/_/  \__/
 *                   /____/
 *
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@qq.com>
 *
 * Copyright (C) 2016-2019 Zongsoft Corporation <http://www.zongsoft.com>
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

namespace Zongsoft.Data
{
	/// <summary>
	/// 表示数据服务的泛型接口。
	/// </summary>
	/// <typeparam name="TEntity">关于数据服务对应的数据实体类型。</typeparam>
	public interface IDataService<TEntity> : IDataService
	{
		#region 事件定义
		event EventHandler<DataGettedEventArgs<TEntity>> Getted;
		event EventHandler<DataGettingEventArgs<TEntity>> Getting;
		#endregion

		#region 属性定义
		/// <summary>
		/// 获取数据搜索器对象。
		/// </summary>
		IDataSearcher<TEntity> Searcher
		{
			get;
		}
		#endregion

		#region 查询方法
		new IEnumerable<TEntity> Select(IDictionary<string, object> states = null, params Sorting[] sortings);
		new IEnumerable<TEntity> Select(ICondition condition, params Sorting[] sortings);
		new IEnumerable<TEntity> Select(ICondition condition, IDictionary<string, object> states, params Sorting[] sortings);
		new IEnumerable<TEntity> Select(ICondition condition, Paging paging, params Sorting[] sortings);
		new IEnumerable<TEntity> Select(ICondition condition, Paging paging, IDictionary<string, object> states, params Sorting[] sortings);
		new IEnumerable<TEntity> Select(ICondition condition, Paging paging, string schema, params Sorting[] sortings);
		new IEnumerable<TEntity> Select(ICondition condition, Paging paging, string schema, IDictionary<string, object> states, params Sorting[] sortings);
		new IEnumerable<TEntity> Select(ICondition condition, string schema, params Sorting[] sortings);
		new IEnumerable<TEntity> Select(ICondition condition, string schema, IDictionary<string, object> states, params Sorting[] sortings);
		new IEnumerable<TEntity> Select(ICondition condition, string schema, Paging paging, params Sorting[] sortings);
		new IEnumerable<TEntity> Select(ICondition condition, string schema, Paging paging, IDictionary<string, object> states, params Sorting[] sortings);
		#endregion
	}
}

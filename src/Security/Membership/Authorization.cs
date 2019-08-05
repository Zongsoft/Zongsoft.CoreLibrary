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
 * Copyright (C) 2018-2019 Zongsoft Corporation <http://www.zongsoft.com>
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
using System.Collections.ObjectModel;
using System.Collections.Specialized;

using Zongsoft.Common;

namespace Zongsoft.Security.Membership
{
	/// <summary>
	/// 提供授权管理的平台类。
	/// </summary>
	[System.Reflection.DefaultMember(nameof(Authorizers))]
	public class Authorization
	{
		#region 单例字段
		public static readonly Authorization Instance = new Authorization();
		#endregion

		#region 构造函数
		private Authorization()
		{
			var authorizers = new ObservableCollection<IAuthorizer>();
			authorizers.CollectionChanged += OnCollectionChanged;

			this.Authorizers = authorizers;
			this.Filters = new List<IExecutionFilter>();
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取授权器的集合。
		/// </summary>
		public ICollection<IAuthorizer> Authorizers
		{
			get;
		}

		/// <summary>
		/// 获取一个授权的过滤器集合，该过滤器包含对授权的响应处理。
		/// </summary>
		public ICollection<IExecutionFilter> Filters
		{
			get;
		}
		#endregion

		#region 事件响应
		private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			switch(e.Action)
			{
				case NotifyCollectionChangedAction.Add:
					for(int i = e.NewStartingIndex; i < e.NewItems.Count; i++)
					{
						((IAuthorizer)e.NewItems[i]).Authorized += OnAuthorized;
						((IAuthorizer)e.NewItems[i]).Authorizing += OnAuthorizing;
					}

					break;
				case NotifyCollectionChangedAction.Reset:
				case NotifyCollectionChangedAction.Remove:
					for(int i = e.OldStartingIndex; i < e.OldItems.Count; i++)
					{
						((IAuthorizer)e.OldItems[i]).Authorized -= OnAuthorized;
						((IAuthorizer)e.OldItems[i]).Authorizing -= OnAuthorizing;
					}

					break;
				case NotifyCollectionChangedAction.Replace:
					for(int i = e.OldStartingIndex; i < e.OldItems.Count; i++)
					{
						((IAuthorizer)e.OldItems[i]).Authorized -= OnAuthorized;
						((IAuthorizer)e.OldItems[i]).Authorizing -= OnAuthorizing;
					}

					for(int i = e.NewStartingIndex; i < e.NewItems.Count; i++)
					{
						((IAuthorizer)e.NewItems[i]).Authorized += OnAuthorized;
						((IAuthorizer)e.NewItems[i]).Authorizing += OnAuthorizing;
					}

					break;
			}
		}

		private void OnAuthorizing(object sender, AuthorizationContext context)
		{
			foreach(var filter in this.Filters)
			{
				filter.OnFiltering(context);
			}
		}

		private void OnAuthorized(object sender, AuthorizationContext context)
		{
			foreach(var filter in this.Filters)
			{
				filter.OnFiltered(context);
			}
		}
		#endregion
	}
}

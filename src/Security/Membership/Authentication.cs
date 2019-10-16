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
using Zongsoft.Collections;

namespace Zongsoft.Security.Membership
{
	/// <summary>
	/// 提供身份验证的平台类。
	/// </summary>
	[System.Reflection.DefaultMember(nameof(Authenticators))]
	public class Authentication
	{
		#region 单例字段
		public static readonly Authentication Instance = new Authentication();
		#endregion

		#region 构造函数
		private Authentication()
		{
			var authenticators = new ObservableCollection<IAuthenticator>();
			authenticators.CollectionChanged += OnCollectionChanged;

			this.Authenticators = authenticators;
			this.Filters = new List<IExecutionFilter>();
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取身份验证器的集合。
		/// </summary>
		public ICollection<IAuthenticator> Authenticators
		{
			get;
		}

		/// <summary>
		/// 获取一个身份验证的过滤器集合，该过滤器包含对身份验证的响应处理。
		/// </summary>
		public ICollection<IExecutionFilter> Filters
		{
			get;
		}

		/// <summary>
		/// 获取或设置命名空间提供程序。
		/// </summary>
		public INamespaceProvider Namespaces
		{
			get; set;
		}
		#endregion

		#region 事件响应
		private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			switch(e.Action)
			{
				case NotifyCollectionChangedAction.Add:
					for(int i=e.NewStartingIndex; i< e.NewItems.Count; i++)
					{
						((IAuthenticator)e.NewItems[i]).Authenticated += OnAuthenticated;
						((IAuthenticator)e.NewItems[i]).Authenticating += OnAuthenticating;
					}

					break;
				case NotifyCollectionChangedAction.Reset:
				case NotifyCollectionChangedAction.Remove:
					for(int i = e.OldStartingIndex; i < e.OldItems.Count; i++)
					{
						((IAuthenticator)e.OldItems[i]).Authenticated -= OnAuthenticated;
						((IAuthenticator)e.OldItems[i]).Authenticating -= OnAuthenticating;
					}

					break;
				case NotifyCollectionChangedAction.Replace:
					for(int i = e.OldStartingIndex; i < e.OldItems.Count; i++)
					{
						((IAuthenticator)e.OldItems[i]).Authenticated -= OnAuthenticated;
						((IAuthenticator)e.OldItems[i]).Authenticating -= OnAuthenticating;
					}

					for(int i = e.NewStartingIndex; i < e.NewItems.Count; i++)
					{
						((IAuthenticator)e.NewItems[i]).Authenticated += OnAuthenticated;
						((IAuthenticator)e.NewItems[i]).Authenticating += OnAuthenticating;
					}

					break;
			}
		}

		private void OnAuthenticating(object sender, AuthenticationContext context)
		{
			foreach(var filter in this.Filters)
			{
				filter.OnFiltering(context);
			}
		}

		private void OnAuthenticated(object sender, AuthenticationContext context)
		{
			foreach(var filter in this.Filters)
			{
				filter.OnFiltered(context);
			}
		}
		#endregion
	}
}

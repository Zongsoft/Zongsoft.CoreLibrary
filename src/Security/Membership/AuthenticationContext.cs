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

namespace Zongsoft.Security.Membership
{
	public class AuthenticationContext
	{
		#region 成员字段
		private IUserIdentity _user;
		private IDictionary<string, object> _parameters;
		#endregion

		#region 构造函数
		public AuthenticationContext(IAuthenticator authenticator, string identity, string @namespace, IUserIdentity user, string scene, IDictionary<string, object> parameters = null)
		{
			this.Authenticator = authenticator ?? throw new ArgumentNullException(nameof(authenticator));

			this.Identity = identity;
			this.Namespace = @namespace;
			this.Scene = scene;
			this.User = user;

			if(parameters != null && parameters.Count > 0)
				_parameters = new Dictionary<string, object>(parameters, StringComparer.OrdinalIgnoreCase);
		}

		public AuthenticationContext(IAuthenticator authenticator, string identity, string @namespace, string scene, IDictionary<string, object> parameters = null)
		{
			this.Authenticator = authenticator ?? throw new ArgumentNullException(nameof(authenticator));

			this.Identity = identity;
			this.Namespace = @namespace;
			this.Scene = scene;
			_user = null;

			if(parameters != null && parameters.Count > 0)
				_parameters = new Dictionary<string, object>(parameters, StringComparer.OrdinalIgnoreCase);
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取激发的验证器对象。
		/// </summary>
		public IAuthenticator Authenticator
		{
			get;
		}

		/// <summary>
		/// 获取身份验证是否通过。
		/// </summary>
		public bool IsAuthenticated
		{
			get => _user != null && this.Exception == null;
		}

		/// <summary>
		/// 获取身份验证使用的身份标识。
		/// </summary>
		public string Identity
		{
			get;
		}

		/// <summary>
		/// 获取身份验证的命名空间。
		/// </summary>
		public string Namespace
		{
			get;
		}

		/// <summary>
		/// 获取身份验证的应用场景。
		/// </summary>
		public string Scene
		{
			get;
		}

		/// <summary>
		/// 获取或设置身份验证对应的用户对象。
		/// </summary>
		public IUserIdentity User
		{
			get
			{
				return _user;
			}
			set
			{
				_user = value ?? throw new ArgumentNullException();
			}
		}

		/// <summary>
		/// 获取或设置发生的错误异常。
		/// </summary>
		public Exception Exception
		{
			get; set;
		}

		/// <summary>
		/// 获取一个值，指示扩展参数集是否有内容。
		/// </summary>
		public bool HasParameters
		{
			get
			{
				return _parameters != null && _parameters.Count > 0;
			}
		}

		/// <summary>
		/// 获取验证结果的扩展参数集。
		/// </summary>
		public IDictionary<string, object> Parameters
		{
			get
			{
				if(_parameters == null)
					System.Threading.Interlocked.CompareExchange(ref _parameters, new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase), null);

				return _parameters;
			}
		}
		#endregion
	}
}

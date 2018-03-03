/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2003-2018 Zongsoft Corporation <http://www.zongsoft.com>
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

namespace Zongsoft.Security
{
	[Serializable]
	public class CredentialRegisterEventArgs : EventArgs
	{
		#region 成员字段
		private Membership.User _user;
		private string _scene;
		private Credential _credential;
		private IDictionary<string, object> _extendedProperties;
		#endregion

		#region 构造函数
		public CredentialRegisterEventArgs(Membership.User user, string scene, IDictionary<string, object> extendedProperties = null)
		{
			_user = user ?? throw new ArgumentNullException(nameof(user));
			_scene = scene;
			_extendedProperties = extendedProperties;
		}

		public CredentialRegisterEventArgs(Credential credential)
		{
			_credential = credential ?? throw new ArgumentNullException(nameof(credential));
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取注册的用户对象。
		/// </summary>
		public Membership.User User
		{
			get
			{
				return _user;
			}
		}

		/// <summary>
		/// 获取注册的应用场景。
		/// </summary>
		public string Scene
		{
			get
			{
				return _scene;
			}
		}

		/// <summary>
		/// 获取或设置注册成功的凭证对象。
		/// </summary>
		public Credential Credential
		{
			get
			{
				return _credential;
			}
			set
			{
				_credential = value;
			}
		}

		/// <summary>
		/// 获取一个值，指示扩展属性集是否存在并且有值。
		/// </summary>
		public bool HasExtendedProperties
		{
			get
			{
				return _extendedProperties != null && _extendedProperties.Count > 0;
			}
		}

		/// <summary>
		/// 获取注册操作传入的扩展属性集。
		/// </summary>
		public IDictionary<string, object> ExtendedProperties
		{
			get
			{
				return _extendedProperties;
			}
		}
		#endregion
	}
}

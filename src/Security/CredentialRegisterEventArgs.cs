/*
 *   _____                                ______
 *  /_   /  ____  ____  ____  _________  / __/ /_
 *    / /  / __ \/ __ \/ __ \/ ___/ __ \/ /_/ __/
 *   / /__/ /_/ / / / / /_/ /\_ \/ /_/ / __/ /_
 *  /____/\____/_/ /_/\__  /____/\____/_/  \__/
 *                   /____/
 *
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
		#region 构造函数
		public CredentialRegisterEventArgs(Credential credential, bool renewal = false)
		{
			this.IsRenewal = renewal;
			this.Credential = credential ?? throw new ArgumentNullException(nameof(credential));
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取注册的应用场景。
		/// </summary>
		public string Scene
		{
			get => this.Credential.Scene;
		}

		/// <summary>
		/// 获取凭证的有效时长。
		/// </summary>
		public TimeSpan Duration
		{
			get => this.Credential.Duration;
		}

		/// <summary>
		/// 获取或设置注册成功的凭证对象。
		/// </summary>
		public Credential Credential
		{
			get; set;
		}

		/// <summary>
		/// 获取一个值，指示当前注册是否为续约引发。
		/// </summary>
		public bool IsRenewal
		{
			get;
		}
		#endregion
	}
}

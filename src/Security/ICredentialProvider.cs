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
 * Copyright (C) 2015-2019 Zongsoft Corporation <http://www.zongsoft.com>
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
	/// <summary>
	/// 提供安全凭证相关操作的功能。
	/// </summary>
	public interface ICredentialProvider
	{
		/// <summary>表示注册完成事件。</summary>
		event EventHandler<CredentialRegisterEventArgs> Registered;

		/// <summary>表示准备注册事件。</summary>
		event EventHandler<CredentialRegisterEventArgs> Registering;

		/// <summary>表示注销完成事件。</summary>
		event EventHandler<CredentialUnregisterEventArgs> Unregistered;

		/// <summary>表示准备注销事件。</summary>
		event EventHandler<CredentialUnregisterEventArgs> Unregistering;

		/// <summary>
		/// 将指定的凭证对象注册到凭证容器中。
		/// </summary>
		/// <param name="credential">指定要注册的凭证对象。</param>
		void Register(Credential credential);

		/// <summary>
		/// 从安全凭证容器中注销指定的凭证。
		/// </summary>
		/// <param name="credentialId">指定的要注销的安全凭证编号。</param>
		void Unregister(string credentialId);

		/// <summary>
		/// 续约指定的安全凭证。
		/// </summary>
		/// <param name="credentialId">指定要续约的安全凭证编号。</param>
		Credential Renew(string credentialId);

		/// <summary>
		/// 获取指定安全凭证编号对应的<see cref="Credential"/>安全凭证对象。
		/// </summary>
		/// <param name="credentialId">指定要获取的安全凭证编号。</param>
		/// <returns>返回的对应的安全凭证对象，如果指定的安全凭证编号不存在则返回空(null)。</returns>
		Credential GetCredential(string credentialId);

		/// <summary>
		/// 获取指定用户及应用场景对应的<see cref="Credential"/>安全凭证对象。
		/// </summary>
		/// <param name="identity">指定要获取的安全凭证对应的用户唯一标识。</param>
		/// <param name="scene">指定要获取的安全凭证对应的应用场景。</param>
		/// <returns>返回成功的安全凭证对象，如果指定的用户及应用场景不存在对应的安全凭证则返回空(null)。</returns>
		Credential GetCredential(string identity, string scene);
	}
}

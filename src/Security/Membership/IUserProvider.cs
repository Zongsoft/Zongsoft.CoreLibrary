/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2003-2014 Zongsoft Corporation <http://www.zongsoft.com>
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
	/// <summary>
	/// 提供关于用户管理的接口。
	/// </summary>
	public interface IUserProvider
	{
		#region 用户管理
		/// <summary>
		/// 获取指定名称对应的用户对象。
		/// </summary>
		/// <param name="certificationId">调用者的安全凭证号。</param>
		/// <param name="name">要查找的用户名。</param>
		/// <returns>返回由<paramref name="name"/>参数指定的用户对象，如果没有找到指定名称的用户则返回空。</returns>
		/// <exception cref="System.ArgumentNullException">当<paramref name="name"/>参数为空(null)或者全空格字符。</exception>
		User GetUser(string certificationId, string name);

		/// <summary>
		/// 获取指定电子邮箱地址(EMail)的用户。
		/// </summary>
		/// <param name="certificationId">调用者的安全凭证号。</param>
		/// <param name="email">要查找的用户电子邮箱地址。</param>
		/// <returns>返回匹配查找条件的用户对象，如果没有找到则返回空(null)。</returns>
		User GetUserByEmail(string certificationId, string email);

		/// <summary>
		/// 获取当前系统中的所有用户，包括被禁用的。
		/// </summary>
		/// <param name="certificationId">调用者的安全凭证号。</param>
		/// <returns>返回当前系统中的所有用户对象集。</returns>
		IEnumerable<User> GetAllUsers(string certificationId);

		/// <summary>
		/// 删除指定名称集的多个用户。
		/// </summary>
		/// <param name="certificationId">调用者的安全凭证号。</param>
		/// <param name="names">要删除的用户名数组。</param>
		/// <returns>如果删除成功则返回删除的数量，否则返回零。</returns>
		int DeleteUsers(string certificationId, params string[] names);

		/// <summary>
		/// 创建单个或者多个用户。
		/// </summary>
		/// <param name="certificationId">调用者的安全凭证号。</param>
		/// <param name="users">要创建的用户对象集。</param>
		/// <remarks>如果创建失败则抛出异常，并且整个事务会被回滚。</remarks>
		void CreateUsers(string certificationId, IEnumerable<User> users);

		/// <summary>
		/// 更新单个或多个用户信息。
		/// </summary>
		/// <param name="certificationId">调用者的安全凭证号。</param>
		/// <param name="users">要更新的用户对象集。</param>
		/// <remarks>如果在批量更新中，如果待更新的用户对象在数据源中不存在则该项操作将被忽略，而不影响本次操作中的其他对象的更新。</remarks>
		void UpdateUsers(string certificationId, IEnumerable<User> users);
		#endregion

		#region 密码管理
		/// <summary>
		/// 修改指定用户的密码。
		/// </summary>
		/// <param name="certificationId">调用者的安全凭证号。</param>
		/// <param name="oldPassword">指定的用户的当前密码。</param>
		/// <param name="newPassword">指定的用户的新密码。</param>
		void ChangePassword(string certificationId, string oldPassword, string newPassword);

		/// <summary>
		/// 将用户密码重置为一个自动生成的新密码。
		/// </summary>
		/// <param name="certificationId">调用者的安全凭证号。</param>
		/// <param name="passwordAnswer">要重置密码的用户的密码问题的答案。</param>
		/// <returns>返回重置后的新密码。</returns>
		string ResetPassword(string certificationId, string passwordAnswer);

		/// <summary>
		/// 获取指定用户的密码问题的提示。
		/// </summary>
		/// <param name="certificationId">调用者的安全凭证号。</param>
		/// <returns>返回指定用户的密码问题的提示，即密码问答的问题部分。</returns>
		string GetPasswordQuestion(string certificationId);

		/// <summary>
		/// 更新指定用户的密码提示问题和答案。
		/// </summary>
		/// <param name="certificationId">调用者的安全凭证号。</param>
		/// <param name="password">指定的用户的密码。</param>
		/// <param name="passwordQuestion">指定用户的密码提示问题。</param>
		/// <param name="passwordAnswer">指定用户的密码提示问题答案。</param>
		void SetPasswordQuestionAndAnswer(string certificationId, string password, string passwordQuestion, string passwordAnswer);
		#endregion
	}
}

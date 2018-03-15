/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2003-2017 Zongsoft Corporation <http://www.zongsoft.com>
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
		/// 获取指定编号对应的用户对象。
		/// </summary>
		/// <param name="userId">要查找的用户编号。</param>
		/// <returns>返回由<paramref name="userId"/>参数指定的用户对象，如果没有找到指定编号的用户则返回空。</returns>
		User GetUser(uint userId);

		/// <summary>
		/// 获取指定标识对应的用户对象。
		/// </summary>
		/// <param name="identity">要查找的用户标识，可以是 <seealso cref="User.Name"/>、<seealso cref="User.Email"/>或<seealso cref="User.PhoneNumber"/>。</param>
		/// <param name="namespace">要查找的用户标识所属的命名空间。</param>
		/// <returns>返回找到的用户对象；如果在指定的命名空间内没有找到指定标识的用户则返回空(null)。</returns>
		/// <exception cref="System.ArgumentNullException">当<paramref name="identity"/>参数为空(null)或者全空格字符。</exception>
		User GetUser(string identity, string @namespace);

		/// <summary>
		/// 获取指定命名空间中的用户集。
		/// </summary>
		/// <param name="namespace">要获取的用户集所属的命名空间。如果为星号(*)则忽略命名空间即系统中的所有用户，如果为空(null)或空字符串("")则查找未设置命名空间的用户集。</param>
		/// <param name="paging">查询的分页设置，默认为第一页。</param>
		/// <returns>返回当前命名空间中的所有用户对象集。</returns>
		IEnumerable<User> GetUsers(string @namespace, Zongsoft.Data.Paging paging = null);

		/// <summary>
		/// 确定指定编号的用户是否存在。
		/// </summary>
		/// <param name="userId">指定要查找的用户编号。</param>
		/// <returns>如果指定编号的用户是存在的则返回真(True)，否则返回假(False)。</returns>
		bool Exists(uint userId);

		/// <summary>
		/// 确定指定的用户标识在指定的命名空间内是否已经存在。
		/// </summary>
		/// <param name="identity">要确定的用户标识，可以是“用户名”或“邮箱地址”或“手机号码”。</param>
		/// <param name="namespace">要确定的用户标识所属的命名空间。</param>
		/// <returns>如果指定的用户标识在命名空间内已经存在则返回真(True)，否则返回假(False)。</returns>
		bool Exists(string identity, string @namespace);

		/// <summary>
		/// 设置指定编号的用户头像标识。
		/// </summary>
		/// <param name="userId">要设置的用户编号。</param>
		/// <param name="avatar">要设置的用户头像标识(头像代码或头像图片的URL)。</param>
		/// <returns>如果设置成功则返回真(True)，否则返回假(False)。</returns>
		bool SetAvatar(uint userId, string avatar);

		/// <summary>
		/// 设置指定编号的用户邮箱地址。
		/// </summary>
		/// <param name="userId">要设置的用户编号。</param>
		/// <param name="email">要设置的邮箱地址。</param>
		/// <returns>如果设置成功则返回真(True)，否则返回假(False)。</returns>
		bool SetEmail(uint userId, string email);

		/// <summary>
		/// 设置指定编号的用户手机号码。
		/// </summary>
		/// <param name="userId">要设置的用户编号。</param>
		/// <param name="phoneNumber">要设置的手机号码。</param>
		/// <returns>如果设置成功则返回真(True)，否则返回假(False)。</returns>
		bool SetPhoneNumber(uint userId, string phoneNumber);

		/// <summary>
		/// 设置指定编号的用户所属命名空间。
		/// </summary>
		/// <param name="userId">要设置的用户编号。</param>
		/// <param name="namespace">要设置的命名空间。</param>
		/// <returns>如果设置成功则返回真(True)，否则返回假。</returns>
		bool SetNamespace(uint userId, string @namespace);

		/// <summary>
		/// 更新指定命名空间下所有用户到新的命名空间。
		/// </summary>
		/// <param name="oldNamespace">指定的旧命名空间。</param>
		/// <param name="newNamespace">指定的新命名空间。</param>
		/// <returns>返回更新成功的用户数。</returns>
		int SetNamespaces(string oldNamespace, string newNamespace);

		/// <summary>
		/// 设置指定编号的用户名称。
		/// </summary>
		/// <param name="userId">要设置的用户编号。</param>
		/// <param name="name">要设置的用户名称。</param>
		/// <returns>如果设置成功则返回真(True)，否则返回假(False)。</returns>
		bool SetName(uint userId, string name);

		/// <summary>
		/// 设置指定编号的用户全称(昵称)。
		/// </summary>
		/// <param name="userId">要设置的用户编号。</param>
		/// <param name="fullName">要设置的用户全称(昵称)。</param>
		/// <returns>如果设置成功则返回真(True)，否则返回假(False)。</returns>
		bool SetFullName(uint userId, string fullName);

		/// <summary>
		/// 设置指定编号的用户描述信息。
		/// </summary>
		/// <param name="userId">要设置的用户编号。</param>
		/// <param name="description">要设置的用户描述信息。</param>
		/// <returns>如果设置成功则返回真(True)，否则返回假(False)。</returns>
		bool SetDescription(uint userId, string description);

		/// <summary>
		/// 设置指定编号的用户所对应的主体标识。
		/// </summary>
		/// <param name="userId">要设置的用户编号。</param>
		/// <param name="principalId">要设置的用户主体标识。</param>
		/// <returns>如果设置成功则返回真(True)，否则返回假(False)。</returns>
		bool SetPrincipalId(uint userId, string principalId);

		/// <summary>
		/// 设置指定编号的用户状态。
		/// </summary>
		/// <param name="userId">要设置的用户编号。</param>
		/// <param name="status">指定的用户状态。</param>
		/// <returns>如果设置成功则返回真(True)，否则返回假(False)。</returns>
		bool SetStatus(uint userId, UserStatus status);

		/// <summary>
		/// 删除指定编号集的多个用户。
		/// </summary>
		/// <param name="userIds">要删除的用户编号数组。</param>
		/// <returns>如果删除成功则返回删除的数量，否则返回零。</returns>
		int DeleteUsers(params uint[] userIds);

		/// <summary>
		/// 创建一个用户，并为其设置密码。
		/// </summary>
		/// <param name="user">要创建的<seealso cref="User"/>用户对象。</param>
		/// <param name="password">为新创建用户的设置的密码。</param>
		/// <returns>如果创建成功则返回真(true)，否则返回假(false)。</returns>
		bool CreateUser(User user, string password);

		/// <summary>
		/// 创建单个或者多个用户。
		/// </summary>
		/// <param name="users">要创建的用户对象集。</param>
		/// <returns>返回创建成功的用户数量。</returns>
		int CreateUsers(IEnumerable<User> users);

		/// <summary>
		/// 更新单个或多个用户信息。
		/// </summary>
		/// <param name="users">要更新的用户对象数组。</param>
		/// <returns>返回更新成功的用户数量。</returns>
		int UpdateUsers(params User[] users);

		/// <summary>
		/// 更新单个或多个用户信息。
		/// </summary>
		/// <param name="users">要更新的用户对象集。</param>
		/// <param name="scope">指定需要更新的字段集，字段名之间使用逗号分隔。</param>
		/// <returns>返回更新成功的用户数量。</returns>
		int UpdateUsers(IEnumerable<User> users, string scope = null);
		#endregion

		#region 密码管理
		/// <summary>
		/// 判断指定编号的用户是否设置了密码。
		/// </summary>
		/// <param name="userId">指定的用户编号。</param>
		/// <returns>如果返回真(True)表示指定编号的用户已经设置了密码，否则未设置密码。</returns>
		bool HasPassword(uint userId);

		/// <summary>
		/// 判断指定标识的用户是否设置了密码。
		/// </summary>
		/// <param name="identity">指定的用户标识。</param>
		/// <param name="namespace">指定的用户标识所属的命名空间。</param>
		/// <returns>如果返回真(True)表示指定标识的用户已经设置了密码，否则未设置密码。</returns>
		bool HasPassword(string identity, string @namespace);

		/// <summary>
		/// 修改指定用户的密码。
		/// </summary>
		/// <param name="userId">要修改密码的用户编号。</param>
		/// <param name="oldPassword">指定的用户的当前密码。</param>
		/// <param name="newPassword">指定的用户的新密码。</param>
		/// <returns>如果修改成功返回真(True)，否则返回假(False)。</returns>
		bool ChangePassword(uint userId, string oldPassword, string newPassword);

		/// <summary>
		/// 准备重置指定用户的密码。
		/// </summary>
		/// <param name="identity">要重置密码的用户标识，仅限用户的“邮箱地址”或“手机号码”。</param>
		/// <param name="namespace">指定的用户标识所属的命名空间。</param>
		/// <returns>返回<paramref name="identity"/>参数对应的用户编号，如果指定用户标识不存在则返回零。</returns>
		uint ForgetPassword(string identity, string @namespace);

		/// <summary>
		/// 重置指定用户的密码，以验证码摘要的方式进行密码重置。
		/// </summary>
		/// <param name="userId">要重置的用户编号。</param>
		/// <param name="secret">重置密码的验证码。</param>
		/// <param name="newPassword">重置后的新密码。</param>
		/// <returns>如果密码重置成功则返回真(True)，否则返回假(False)。</returns>
		/// <remarks>
		///		<para>本重置方法通常由Web请求的方式进行，请求的URL大致如下：<c><![CDATA[http://zongsoft.com/security/user/resetpassword?userId=xxx&secret=xxxxxx]]></c></para>
		/// </remarks>
		bool ResetPassword(uint userId, string secret, string newPassword = null);

		/// <summary>
		/// 重置指定用户的密码，以密码问答的方式进行密码重置。
		/// </summary>
		/// <param name="identity">要重置的用户标识，可以是“用户名”、“邮箱地址”或“手机号码”。</param>
		/// <param name="namespace">指定的用户标识所属的命名空间。</param>
		/// <param name="passwordAnswers">指定用户的密码问答的答案集。</param>
		/// <param name="newPassword">重置后的新密码。</param>
		/// <returns>如果密码重置成功则返回真(True)，否则返回假(False)。</returns>
		/// <exception cref="SecurityException">如果指定的用户没有设置密码问答或者密码问答验证失败。</exception>
		bool ResetPassword(string identity, string @namespace, string[] passwordAnswers, string newPassword = null);

		/// <summary>
		/// 获取指定用户的密码问答的题面集。
		/// </summary>
		/// <param name="userId">指定的用户编号。</param>
		/// <returns>返回指定用户的密码问答的题面，即密码问答的提示部分。</returns>
		string[] GetPasswordQuestions(uint userId);

		/// <summary>
		/// 获取指定用户的密码问答的题面集。
		/// </summary>
		/// <param name="identity">指定的用户标识，可以是“用户名”、“邮箱地址”或“手机号码”。</param>
		/// <param name="namespace">指定的用户标识所属的命名空间。</param>
		/// <returns>返回指定用户的密码问答的题面，即密码问答的提示部分。</returns>
		string[] GetPasswordQuestions(string identity, string @namespace);

		/// <summary>
		/// 设置指定用户的密码问答集。
		/// </summary>
		/// <param name="userId">要设置密码问答集的用户编号。</param>
		/// <param name="password">当前用户的密码，如果密码错误则无法更新密码问答。</param>
		/// <param name="passwordQuestions">当前用户的密码问答的题面集。</param>
		/// <param name="passwordAnswers">当前用户的密码问答的答案集。</param>
		/// <returns>如果设置成则返回真(True)，否则返回假(False)。</returns>
		bool SetPasswordQuestionsAndAnswers(uint userId, string password, string[] passwordQuestions, string[] passwordAnswers);
		#endregion

		#region 秘密校验
		/// <summary>
		/// 校验指定的秘钥是否正确。
		/// </summary>
		/// <param name="userId">指定的用户编号。</param>
		/// <param name="type">指定的待校验的类型名。</param>
		/// <param name="secret">指定的待校验的秘钥。</param>
		/// <returns>如果校验成功则返回真(True)，否则返回假(False)。</returns>
		bool Verify(uint userId, string type, string secret);
		#endregion
	}
}

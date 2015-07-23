/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2003-2015 Zongsoft Corporation <http://www.zongsoft.com>
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
		User GetUser(int userId);

		/// <summary>
		/// 获取指定标识对应的用户对象。
		/// </summary>
		/// <param name="identity">要查找的用户标识，可以是“用户名”或“邮箱地址”或“手机号码”。</param>
		/// <param name="namespace">要查找的用户标识所属的命名空间。</param>
		/// <returns>返回找到的用户对象；如果在指定的命名空间内没有找到指定标识的用户则返回空(null)。</returns>
		/// <exception cref="System.ArgumentNullException">当<paramref name="identity"/>参数为空(null)或者全空格字符。</exception>
		User GetUser(string identity, string @namespace);

		/// <summary>
		/// 设置指定编号的用户主体标识。
		/// </summary>
		/// <param name="userId">要设置的用户编号。</param>
		/// <param name="principal">要设置的用户主体标识。</param>
		/// <returns>如果设置成功则返回真(True)，否则返回假(False)。</returns>
		bool SetPrincipal(int userId, string principal);

		/// <summary>
		/// 获取当前命名空间中的所有用户。
		/// </summary>
		/// <param name="namespace">要获取的用户集所属的命名空间。</param>
		/// <param name="paging">查询的分页设置，默认为第一页。</param>
		/// <returns>返回当前命名空间中的所有用户对象集。</returns>
		IEnumerable<User> GetAllUsers(string @namespace, Zongsoft.Data.Paging paging = null);

		/// <summary>
		/// 删除指定编号集的多个用户。
		/// </summary>
		/// <param name="userIds">要删除的用户编号数组。</param>
		/// <returns>如果删除成功则返回删除的数量，否则返回零。</returns>
		int DeleteUsers(params int[] userIds);

		/// <summary>
		/// 创建单个或者多个用户。
		/// </summary>
		/// <param name="users">要创建的用户对象数组。</param>
		/// <returns>返回创建成功的用户数量。</returns>
		int CreateUsers(params User[] users);

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
		/// <returns>返回更新成功的用户数量。</returns>
		int UpdateUsers(IEnumerable<User> users);
		#endregion

		#region 密码管理
		/// <summary>
		/// 修改指定用户的密码。
		/// </summary>
		/// <param name="userId">要修改密码的用户编号。</param>
		/// <param name="oldPassword">指定的用户的当前密码。</param>
		/// <param name="newPassword">指定的用户的新密码。</param>
		bool ChangePassword(int userId, string oldPassword, string newPassword);

		/// <summary>
		/// 准备重置指定用户的密码。
		/// </summary>
		/// <param name="identity">要重置密码的用户标识，仅限用户的“邮箱地址”或“手机号码”。</param>
		/// <param name="namespace">指定的用户标识所属的命名空间。</param>
		/// <param name="userId">输出参数，对应的用户编号。</param>
		/// <param name="secret">输出参数，生成的验证码。</param>
		/// <param name="token">输出参数，生成的验证码摘要。</param>
		/// <returns>如果成功则返回真(True)，否则返回假(False)这通常表示指定的用户标识不存在。</returns>
		/// <remarks>
		///		<para><paramref name="token"/>的计算公式：HEX(MD5(<paramref name="userId"/>+<paramref name="secret"/>))</para>
		/// </remarks>
		bool ForgetPassword(string identity, string @namespace, out int userId, out string secret, out string token);

		/// <summary>
		/// 重置指定用户的密码，以验证码摘要的方式进行密码重置。
		/// </summary>
		/// <param name="userId">要重置的用户编号。</param>
		/// <param name="token">重置密码的验证码摘要。</param>
		/// <param name="newPassword">重置后的新密码，如果为空(null)或空字符串("")则不进行密码设置，只进行验证码摘要的校验（即判断验证码摘要是否正确）。</param>
		/// <returns>如果重置或者验证码摘要校验成功则返回真(True)，否则返回假(False)。</returns>
		/// <remarks>
		///		<para>本重置方法通常由Web请求的方式进行，请求的URL大致如下：<c><![CDATA[http://zongsoft.com/security/user/resetpassword?userId=xxx&token=xxxxxx]]></c></para>
		/// </remarks>
		bool ResetPassword(int userId, string token, string newPassword = null);

		/// <summary>
		/// 重置指定用户的密码，以验证码的方式进行密码重置。
		/// </summary>
		/// <param name="identity">要重置的用户标识，可以是“用户名”、“邮箱地址”或“手机号码”。</param>
		/// <param name="namespace">指定的用户标识所属的命名空间。</param>
		/// <param name="secret">重置密码的验证码。</param>
		/// <param name="newPassword">重置后的新密码，如果为空(null)或空字符串("")则不进行密码设置，只进行验证码的校验（即判断验证码是否正确）。</param>
		/// <returns>如果重置或者验证码校验成功则返回真(True)，否则返回假(False)。</returns>
		bool ResetPassword(string identity, string @namespace, string secret, string newPassword = null);

		/// <summary>
		/// 重置指定用户的密码，以密码问答的方式进行密码重置。
		/// </summary>
		/// <param name="identity">要重置的用户标识，可以是“用户名”、“邮箱地址”或“手机号码”。</param>
		/// <param name="namespace">指定的用户标识所属的命名空间。</param>
		/// <param name="passwordAnswers">指定用户的密码问答的答案集。</param>
		/// <param name="newPassword">重置后的新密码，如果为空(null)或空字符串("")则不进行密码设置，只进行密码问答的校验（即判断密码问答的答案集是否全部正确）。</param>
		/// <returns>如果重置或者密码问答校验成功则返回真(True)，否则返回假(False)。</returns>
		bool ResetPassword(string identity, string @namespace, string[] passwordAnswers, string newPassword = null);

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
		void SetPasswordQuestionsAndAnswers(int userId, string password, string[] passwordQuestions, string[] passwordAnswers);
		#endregion
	}
}

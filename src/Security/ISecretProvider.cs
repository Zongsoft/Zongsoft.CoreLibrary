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

namespace Zongsoft.Security
{
	/// <summary>
	/// 提供秘密（验证码）生成和校验功能的接口。
	/// </summary>
	public interface ISecretProvider
	{
		/// <summary>
		/// 获取或设置秘密内容的默认过期时长（默认为10分钟），不能设置为零。
		/// </summary>
		TimeSpan Expiry
		{
			get; set;
		}

		/// <summary>
		/// 获取或设置重新生成秘密（验证码）的最小间隔时长，如果为零则表示不做限制。
		/// </summary>
		TimeSpan Period
		{
			get;
			set;
		}

		/// <summary>
		/// 生成一个指定名称的秘密（验证码）。
		/// </summary>
		/// <param name="name">指定的验证码名称，该名称通常包含对应目标标识（譬如：user.forget:100、user.email:100，其中数字100表示用户的唯一编号）。</param>
		/// <param name="extra">指定的附加文本，该附加文本可通过<see cref="Verify(string, string, out string)"/>方法验证通过后获取到。</param>
		/// <returns>返回生成成功的验证码，关于验证码的具体生成规则请参考特定实现版本。</returns>
		string Generate(string name, string extra = null);

		/// <summary>
		/// 生成一个指定名称的秘密（验证码）。
		/// </summary>
		/// <param name="name">指定的验证码名称，该名称通常包含对应的目标标识，譬如：user.forget:100(数字100表示用户的唯一编号)、user.phone:13812345678。</param>
		/// <param name="pattern">指定的验证码生成模式，基本定义参考备注说明。</param>
		/// <param name="extra">指定的附加文本，该附加文本可通过<see cref="Verify(string, string, out string)"/>方法验证通过后获取到。</param>
		/// <returns>返回生成成功的验证码，关于验证码的生成规则由<paramref name="pattern"/>参数定义。</returns>
		/// <remarks>
		/// 	<para>参数<paramref name="pattern"/>用来定义生成验证码的模式，如果为空(null)或空字符串则由特定实现版本自行定义（建议生成6位数字的验证码）；也可以表示生成验证码的规则，基本模式定义如下：</para>
		/// 	<list type="bullet">
		/// 		<item>guid|uuid，表示生成一个GUID值</item>
		/// 		<item>#{number}，表示生成{number}个的数字字符，譬如：#4</item>
		/// 		<item>?{number}，表示生成{number}个的含有字母或数字的字符，譬如：?8</item>
		/// 		<item>*{number}，完全等同于?{number}。</item>
		/// 	</list>
		/// 	<para>注：如果<paramref name="pattern"/>参数不匹配模式定义，则表示其即为要生成的秘密（验证码）值，这样的固定秘密（验证码）应只由字母和数字组成，不要包含其他符号。</para>
		/// </remarks>
		string Generate(string name, string pattern, string extra);

		/// <summary>
		/// 验证指定名称的秘密（验证码）是否正确。
		/// </summary>
		/// <param name="name">指定的验证码名称，该名称通常包含对应的目标标识（譬如：user.forget:100、user.email:100，其中数字100表示用户的唯一编号）。</param>
		/// <param name="secret">指定待确认的验证码。</param>
		/// <returns>如果验证成功则返回真(True)，否则返回假(False)。</returns>
		bool Verify(string name, string secret);

		/// <summary>
		/// 验证指定名称的秘密（验证码）是否正确。
		/// </summary>
		/// <param name="name">指定的验证码名称，该名称通常包含对应的目标标识（譬如：user.forget:100、user.email:100，其中数字100表示用户的唯一编号）。</param>
		/// <param name="secret">指定待确认的验证码。</param>
		/// <param name="extra">输出参数，表示验证通过后该验证码生成时绑定的附加文本。</param>
		/// <returns>如果验证成功则返回真(True)，否则返回假(False)。</returns>
		bool Verify(string name, string secret, out string extra);
	}
}

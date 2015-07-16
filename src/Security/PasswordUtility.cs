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
using System.Security;
using System.Security.Cryptography;

namespace Zongsoft.Security
{
	/// <summary>
	/// 提供密码操作的工具类。
	/// </summary>
	public static class PasswordUtility
	{
		#region 私有变量
		private static readonly char[] PasswordSymbols = new char[] { '-', '+', '#', '@', '~', '$', '&', '.' };
		#endregion

		/// <summary>
		/// 生成随机口令字符串。
		/// </summary>
		/// <returns>返回生成的随机口令字符串。</returns>
		public static string GeneratePassword()
		{
			return GeneratePassword(8);
		}

		/// <summary>
		/// 生成随机口令字符串。
		/// </summary>
		/// <param name="length">指定要生成的口令字符串的长度，如果长度小于8则为8。</param>
		/// <returns>返回生成的随机口令字符串。</returns>
		public static string GeneratePassword(int length)
		{
			byte[] buffer = Zongsoft.Common.RandomGenerator.Generate(Math.Max(length, 8));
			char[] password = new char[buffer.Length];

			for(int i = 0; i < buffer.Length; i++)
			{
				var index = buffer[i] % 70;

				if(index < 10)
					password[i] = (char)((byte)'0' + index);
				else if(index >= 10 && index < 36)
					password[i] = (char)((byte)'A' + (index - 10));
				else if(index >= 36 && index < 62)
					password[i] = (char)((byte)'a' + (index - 36));
				else
					password[i] = PasswordSymbols[index - 62];
			}

			return new string(password);
		}

		/// <summary>
		/// 使用系统默认的散列算法对口令明文进行散列。
		/// </summary>
		/// <param name="password">待散列(哈希)的口令明文。</param>
		/// <returns>散列后的口令值。</returns>
		/// <remarks>注意：该方法未对<paramref name="password"/>进行Salting干扰处理，建议使用带口令Salting功能的同名方法。</remarks>
		public static byte[] HashPassword(string password)
		{
			return HashPassword(password, null);
		}

		/// <summary>
		/// 使用指定的散列算法对口令明文进行散列。
		/// </summary>
		/// <param name="password">待散列(哈希)的口令明文。</param>
		/// <param name="passwordSalt">对密码进行散列操作的随机整型数。</param>
		/// <param name="hashAlgorithm">进行散列算法的名称，默认为SHA1。</param>
		/// <returns>散列后的口令值。</returns>
		public static byte[] HashPassword(string password, int passwordSalt, string hashAlgorithm = "SHA1")
		{
			return HashPassword(password, BitConverter.GetBytes(passwordSalt), hashAlgorithm);
		}

		/// <summary>
		/// 使用指定的散列算法对口令明文进行散列。
		/// </summary>
		/// <param name="password">待散列(哈希)的口令明文。</param>
		/// <param name="passwordSalt">对密码进行散列操作的随机值。</param>
		/// <param name="hashAlgorithm">进行散列算法的名称，默认为SHA1。</param>
		/// <returns>散列后的口令值。</returns>
		public static byte[] HashPassword(string password, byte[] passwordSalt, string hashAlgorithm = "SHA1")
		{
			if(string.IsNullOrWhiteSpace(hashAlgorithm))
				hashAlgorithm = "SHA1";

			using(HashAlgorithm hash = HashAlgorithm.Create(hashAlgorithm))
			{
				var passwordBuffer = System.Text.Encoding.UTF8.GetBytes(password);

				if(passwordSalt != null && passwordSalt.Length > 0)
				{
					var buffer = new byte[passwordBuffer.Length + passwordSalt.Length];

					Buffer.BlockCopy(passwordBuffer, 0, buffer, 0, passwordBuffer.Length);
					Buffer.BlockCopy(passwordSalt, 0, buffer, passwordBuffer.Length, passwordSalt.Length);

					return hash.ComputeHash(buffer);
				}

				return hash.ComputeHash(passwordBuffer);
			}
		}

		/// <summary>
		/// 验证口令是否与存储在数据库中的口令是否匹配。
		/// </summary>
		/// <param name="password">待验证的口令明文。</param>
		/// <param name="storedPassword">存储在数据库中的已Salt后的口令散列值。</param>
		/// <param name="storedPasswordSalt">存储在数据库中的与之匹对的整型随机Salt数。</param>
		/// <param name="hashAlgorithm">进行散列算法的名称，默认为SHA1。</param>
		/// <returns>验证成功则返回真，否则返回假。</returns>
		public static bool VerifyPassword(string password, byte[] storedPassword, int storedPasswordSalt, string hashAlgorithm = "SHA1")
		{
			return VerifyPassword(password, storedPassword, BitConverter.GetBytes(storedPasswordSalt), hashAlgorithm);
		}

		/// <summary>
		/// 验证口令是否与存储在数据库中的口令是否匹配。
		/// </summary>
		/// <param name="password">待验证的口令明文。</param>
		/// <param name="storedPassword">存储在数据库中的已Salt后的口令散列值。</param>
		/// <param name="storedPasswordSalt">存储在数据库中的与之匹对的随机Salt值。</param>
		/// <param name="hashAlgorithm">进行散列算法的名称，默认为SHA1。</param>
		/// <returns>验证成功则返回真，否则返回假。</returns>
		public static bool VerifyPassword(string password, byte[] storedPassword, byte[] storedPasswordSalt, string hashAlgorithm = "SHA1")
		{
			if(password == null || password.Length == 0)
			{
				return (storedPassword == null || storedPassword.Length == 0) &&
					   (storedPasswordSalt == null || storedPasswordSalt.Length == 0);
			}

			//计算密码与对应的随机值的哈希值
			byte[] hashedPassword = HashPassword(password, storedPasswordSalt, hashAlgorithm);

			if(storedPassword == null || storedPassword.Length != hashedPassword.Length)
				return false;

			//逐字节检测口令的散列值，如果有任一差异则整个匹对失败并退出。
			for(int i = 0; i < hashedPassword.Length; i++)
			{
				if(storedPassword[i] != hashedPassword[i])
					return false;
			}

			//匹对成功，返回。
			return true;
		}
	}
}

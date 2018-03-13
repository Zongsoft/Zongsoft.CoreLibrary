/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2015-2018 Zongsoft Corporation <http://www.zongsoft.com>
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

using Zongsoft.Runtime.Caching;

namespace Zongsoft.Security
{
	public class SecretProvider : ISecretProvider
	{
		#region 成员字段
		private ICache _cache;
		private TimeSpan _expiry;
		#endregion

		#region 构造函数
		public SecretProvider()
		{
			//设置默认过期时长
			_expiry = TimeSpan.FromMinutes(10);
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取或设置秘密内容的缓存容器。
		/// </summary>
		public ICache Cache
		{
			get
			{
				return _cache;
			}
			set
			{
				_cache = value ?? throw new ArgumentNullException();
			}
		}

		/// <summary>
		/// 获取或设置秘密内容的过期时长，默认为10分钟。
		/// </summary>
		public TimeSpan Expiry
		{
			get
			{
				return _expiry;
			}
			set
			{
				if(value > TimeSpan.Zero)
					_expiry = value;
			}
		}
		#endregion

		#region 生成方法
		public string Generate(string name, string extra = null)
		{
			return this.Generate(name, extra, _expiry);
		}

		public string Generate(string name, TimeSpan expiry)
		{
			return this.Generate(name, null, expiry);
		}

		public string Generate(string name, string extra, TimeSpan expiry)
		{
			//生成一个默认规则的随机数作为密文
			var secret = this.GenerateSecret(name);

			//生成指定密文到缓存容器中
			this.Generate(name, secret, extra, expiry);

			//返回生成的密文
			return secret;
		}

		public void Generate(string name, string secret, string extra, TimeSpan expiry)
		{
			if(string.IsNullOrEmpty(name))
				throw new ArgumentNullException(nameof(name));

			if(string.IsNullOrEmpty(secret))
				throw new ArgumentNullException(nameof(secret));

			var cache = this.Cache;

			if(cache == null)
				throw new InvalidOperationException("Missing cache for the secret notify operation.");

			//将秘密内容保存到缓存容器中（如果指定的过期时长为零则采用默认过期时长）
			if(!cache.SetValue(name, this.GetValue(name, secret, extra), expiry > TimeSpan.Zero ? expiry : _expiry))
				throw new InvalidOperationException("The secret caching failed.");
		}
		#endregion

		#region 校验方法
		public bool Verify(string name, string secret)
		{
			return this.Verify(name, secret, out var extra);
		}

		public bool Verify(string name, string secret, out string extra)
		{
			extra = null;

			if(string.IsNullOrEmpty(name))
				throw new ArgumentNullException(nameof(name));

			if(string.IsNullOrEmpty(secret))
				return false;

			var cache = this.Cache;

			if(cache == null)
				throw new InvalidOperationException("Missing cache for the secret verify operation.");

			//从缓存容器中获取对应的内容
			var cacheValue = cache.GetValue<string>(name);

			//从缓存内容解析出对应的秘密值
			var cacheSecret = this.GetSecret(name, cacheValue, out extra);

			//比对秘密内容
			if(string.Equals(secret, cacheSecret, StringComparison.OrdinalIgnoreCase))
			{
				//删除缓存项
				cache.Remove(name);

				//返回校验成功
				return true;
			}

			//返回校验失败
			return false;
		}
		#endregion

		#region 虚拟方法
		protected virtual string GenerateSecret(string name)
		{
			return Zongsoft.Common.RandomGenerator.GenerateString(6, true);
		}

		protected virtual string GetValue(string name, string secret, string extra)
		{
			if(string.IsNullOrEmpty(extra))
				return secret;

			return secret + "|" + extra;
		}

		protected virtual string GetSecret(string name, string value, out string extra)
		{
			extra = null;

			if(string.IsNullOrEmpty(value))
				return null;

			var index = value.IndexOf('|');

			if(index > 0)
			{
				extra = value.Substring(index + 1);
				return value.Substring(0, index);
			}

			return value;
		}
		#endregion
	}
}

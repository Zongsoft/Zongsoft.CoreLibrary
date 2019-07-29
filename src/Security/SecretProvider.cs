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
		#region 常量定义
		private const string NULL_VALUE = "*";
		private static readonly DateTime EPOCH = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);
		#endregion

		#region 成员字段
		private ICache _cache;
		private TimeSpan _expiry;
		private TimeSpan _period;
		#endregion

		#region 构造函数
		public SecretProvider()
		{
			//设置属性的默认值
			_expiry = TimeSpan.FromMinutes(30);
			_period = TimeSpan.FromSeconds(60);
		}

		public SecretProvider(ICache cache)
		{
			//设置缓存容器
			_cache = cache ?? throw new ArgumentNullException(nameof(cache));

			//设置属性的默认值
			_expiry = TimeSpan.FromMinutes(30);
			_period = TimeSpan.FromSeconds(60);
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
		/// 获取或设置秘密内容的默认过期时长（默认为10分钟）。
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

		/// <summary>
		/// 获取或设置重新生成秘密（验证码）的最小间隔时长（默认为60秒），如果为零则表示不做限制。
		/// </summary>
		public TimeSpan Period
		{
			get
			{
				return _period;
			}
			set
			{
				_period = value;
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
			return this.Generate(name, null, extra, expiry);
		}

		public string Generate(string name, string pattern, string extra, TimeSpan expiry)
		{
			if(string.IsNullOrEmpty(name))
				throw new ArgumentNullException(nameof(name));

			var cache = this.Cache;

			if(cache == null)
				throw new InvalidOperationException("Missing a required cache for the secret generate operation.");

			//修复秘密名（转换成小写并剔除收尾空格）
			name = name.ToLowerInvariant().Trim();

			//从缓存容器中获取对应的内容
			var text = cache.GetValue<string>(name);

			if(text != null)
			{
				//验证成功：则必须等待有效期过后才能重新生成
				if(text.Length == 0 || text == NULL_VALUE)
					throw new InvalidOperationException(Properties.Resources.Text_SecretGenerateTooFrequently_Message);

				//尚未验证：则必须确保在最小时间间隔之后才能重新生成
				if(_period > TimeSpan.Zero &&
				   this.Unpack(text, out var cachedSecret, out var timestamp, out var cachedExtra) &&
				   DateTime.UtcNow - timestamp < _period)
					throw new InvalidOperationException(Properties.Resources.Text_SecretGenerateTooFrequently_Message);
			}

			//根据指定的模式生成或获取秘密（验证码）
			var secret = this.GenerateSecret(name, pattern);

			//将秘密内容保存到缓存容器中（如果指定的过期时长为零则采用默认过期时长）
			if(!cache.SetValue(name, this.Pack(secret, extra), expiry > TimeSpan.Zero ? expiry : _expiry))
				throw new InvalidOperationException("The secret caching failed.");

			return secret;
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
				throw new InvalidOperationException("Missing a required cache for the secret verify operation.");

			//修复秘密名（转换成小写并剔除收尾空格）
			name = name.ToLowerInvariant().Trim();

			//从缓存容器中获取对应的内容
			var cacheValue = cache.GetValue<string>(name);

			//从缓存内容解析出对应的秘密值并且比对秘密内容成功
			if(this.Unpack(cacheValue, out var cachedSecret, out var timestamp, out extra) &&
			   string.Equals(secret, cachedSecret, StringComparison.OrdinalIgnoreCase))
			{
				/*
				 * 注意：不删除缓存项，只设置其内容为空或特定文本！ 
				 * 因为需要利用缓存项的过期时间来判定频繁的生成验证码。
				 */
				cache.SetValue(name, NULL_VALUE);

				//返回校验成功
				return true;
			}

			//返回校验失败
			return false;
		}
		#endregion

		#region 虚拟方法
		protected virtual string GenerateSecret(string name, string pattern = null)
		{
			if(string.IsNullOrWhiteSpace(pattern))
				return Common.RandomGenerator.GenerateString(6, true);

			if(string.Equals(pattern, "guid", StringComparison.OrdinalIgnoreCase) || string.Equals(pattern, "uuid", StringComparison.OrdinalIgnoreCase))
				return Guid.NewGuid().ToString("N");

			if(pattern.Length > 1 && (pattern[0] == '?' || pattern[0] == '*' || pattern[0] == '#'))
			{
				if(int.TryParse(pattern.Substring(1), out var count))
					return Common.RandomGenerator.GenerateString(count, pattern[0] == '#');

				throw new ArgumentException("Invalid secret pattern.");
			}
			else
			{
				if(pattern.Contains(":") | pattern.Contains("|"))
					throw new ArgumentException("The secret argument contains illegal characters.");
			}

			return pattern.Trim();
		}

		protected virtual string Pack(string secret, string extra)
		{
			var timestamp = (ulong)(DateTime.UtcNow - EPOCH).TotalSeconds;

			if(string.IsNullOrEmpty(extra))
				return secret + "|" + timestamp.ToString();
			else
				return secret + "|" + timestamp.ToString() + "|" + extra;
		}

		protected virtual bool Unpack(string text, out string secret, out DateTime timestamp, out string extra)
		{
			secret = null;
			extra = null;
			timestamp = EPOCH;

			if(string.IsNullOrEmpty(text))
				return false;

			var index = 0;
			var last = 0;
			ulong number;

			for(int i = 0; i < text.Length; i++)
			{
				if(text[i] == '|')
				{
					switch(index++)
					{
						case 0:
							secret = text.Substring(0, i);
							break;
						case 1:
							if(ulong.TryParse(text.Substring(last, i - last), out number))
								timestamp = EPOCH.AddSeconds(number);

							if(i < text.Length - 1)
								extra = text.Substring(i + 1);

							return true;
					}

					last = i + 1;
				}
			}

			if(last < text.Length && ulong.TryParse(text.Substring(last), out number))
				timestamp = EPOCH.AddSeconds(number);

			return !string.IsNullOrEmpty(secret);
		}
		#endregion
	}
}

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

using Zongsoft.Common;
using Zongsoft.Runtime.Caching;

namespace Zongsoft.Security
{
	public abstract class SecretVerifierBase : ISecretVerifier
	{
		#region 成员字段
		private string _prefix;
		private ICache _cache;
		private bool _ignoreCase;
		#endregion

		#region 构造函数
		protected SecretVerifierBase(string prefix)
		{
			if(prefix != null)
				_prefix = prefix.Trim();

			_ignoreCase = false;
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
		/// 获取或设置一个值，指示比对秘密内容是否忽略大小写。
		/// </summary>
		public bool IgnoreCase
		{
			get
			{
				return _ignoreCase;
			}
			set
			{
				_ignoreCase = value;
			}
		}
		#endregion

		#region 保护属性
		protected string Prefix
		{
			get
			{
				return _prefix;
			}
		}
		#endregion

		#region 公共方法
		public bool Verify(string name, string secret, object state = null)
		{
			if(string.IsNullOrEmpty(name))
				throw new ArgumentNullException(nameof(name));

			var cache = this.Cache;

			if(cache == null)
				throw new InvalidOperationException("Missing cache for the secret verify operation.");

			//获取当前校验请求对应的缓存键
			var key = this.GetCacheKey(name, state);

			//从缓存容器中获取对应的内容
			var value = cache.GetValue<string>(key);

			//从缓存内容解析出对应的秘密值
			var cachedSecret = this.GetSecret(value);

			//确定秘密内容的比对方式
			var comparison = _ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;

			//比对秘密内容
			if(string.Equals(secret, cachedSecret, comparison))
			{
				//激发比对成功
				this.OnVerified(name, state, value);

				//删除缓存项
				cache.Remove(key);

				//返回校验成功
				return true;
			}

			//返回校验失败
			return false;
		}
		#endregion

		#region 抽象方法
		protected abstract void OnVerified(string name, object state, string value);
		#endregion

		#region 虚拟方法
		protected virtual string GetCacheKey(string name, object state)
		{
			if(string.IsNullOrEmpty(_prefix))
				return name;
			else
				return _prefix +
					   (char.IsLetterOrDigit(_prefix[_prefix.Length - 1]) ? ":" : string.Empty) +
					   name;
		}

		protected virtual string GetSecret(string cachedValue)
		{
			if(string.IsNullOrEmpty(cachedValue))
				return null;

			var index = cachedValue.IndexOf('|');

			if(index > 0)
				return cachedValue.Substring(0, index);

			return cachedValue;
		}
		#endregion
	}
}

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
using System.Threading.Tasks;

using Zongsoft.Common;
using Zongsoft.Runtime.Caching;

namespace Zongsoft.Security
{
	public abstract class SceretNotifierBase : ISecretNotifier
	{
		#region 成员字段
		private string _prefix;
		private ICache _cache;
		private TimeSpan _timeout;
		#endregion

		#region 构造函数
		protected SceretNotifierBase(string prefix)
		{
			if(prefix != null)
				_prefix = prefix.Trim();

			//设置过期时长的默认值
			_timeout = TimeSpan.FromHours(30);
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
		/// 获取或设置秘密内容的过期时长。
		/// </summary>
		public TimeSpan Timeout
		{
			get
			{
				return _timeout;
			}
			set
			{
				_timeout = value;
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
		public IExecutionResult Notify(string name, object parameter, object state = null)
		{
			if(string.IsNullOrEmpty(name))
				throw new ArgumentNullException(nameof(name));

			var cache = this.Cache;

			if(cache == null)
				throw new InvalidOperationException("Missing cache for the secret notify operation.");

			var key = this.GetCacheKey(name, parameter, state);
			var value = this.GetCacheValue(name, parameter, state, out var secret);

			//将秘密内容保存到缓存容器中
			if(!cache.SetValue(key, value, _timeout))
				return ExecutionResult.Failure("The secret caching failed.");

			//激发消息通知
			var result = this.OnNotify(name, parameter, state, secret);

			//如果通知发送失败则删除缓存项
			if(result != null && !result.Succeed)
				cache.Remove(key);

			return result;
		}

		public Task<IExecutionResult> NotifyAsync(string name, object parameter, object state = null)
		{
			if(string.IsNullOrEmpty(name))
				throw new ArgumentNullException(nameof(name));

			return Task.Run(() => this.Notify(name, parameter, state));
		}
		#endregion

		#region 抽象方法
		protected abstract IExecutionResult OnNotify(string name, object parameter, object state, string secret);
		#endregion

		#region 虚拟方法
		protected virtual string GetCacheKey(string name, object parameter, object state)
		{
			if(string.IsNullOrEmpty(_prefix))
				return name;
			else
				return _prefix +
				       (char.IsLetterOrDigit(_prefix[_prefix.Length - 1]) ? ":" : string.Empty) +
				       name;
		}

		protected virtual string GetCacheValue(string name, object parameter, object state, out string secret)
		{
			return secret = Zongsoft.Common.RandomGenerator.GenerateString(6, true);
		}
		#endregion
	}
}

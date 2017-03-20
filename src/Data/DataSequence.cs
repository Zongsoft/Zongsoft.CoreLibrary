/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2017 Zongsoft Corporation <http://www.zongsoft.com>
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
using System.Linq;
using System.Collections.Concurrent;

namespace Zongsoft.Data
{
	public class DataSequence<T>
	{
		#region 静态缓存
		private static readonly ConcurrentDictionary<string, DataSequenceToken> _cache = new ConcurrentDictionary<string, DataSequenceToken>();
		#endregion

		#region 成员字段
		private Zongsoft.Common.ISequence _sequence;
		private IDataService<T> _dataService;
		#endregion

		#region 构造函数
		public DataSequence(IDataService<T> dataService, Zongsoft.Common.ISequence sequence)
		{
			if(dataService == null)
				throw new ArgumentNullException(nameof(dataService));
			if(sequence == null)
				throw new ArgumentNullException(nameof(sequence));

			_dataService = dataService;
			_sequence = sequence;
		}
		#endregion

		#region 公共属性
		private Zongsoft.Common.ISequence Sequence
		{
			get
			{
				return _sequence;
			}
			set
			{
				if(value == null)
					throw new ArgumentNullException();

				_sequence = value;
			}
		}
		#endregion

		#region 注册方法
		public bool Register(string keys, int seed = 0, bool required = true)
		{
			if(string.IsNullOrWhiteSpace(keys))
				throw new ArgumentNullException(nameof(keys));

			return this.Register(keys.Split(','), seed, required);
		}

		public bool Register(string[] keys, int seed = 0, bool required = true)
		{
			if(keys == null || keys.Length == 0)
				throw new ArgumentNullException(nameof(keys));

			return _cache.TryAdd(this.GetCacheKey(keys), new DataSequenceToken(keys, seed, required));
		}

		public bool Unregister(string[] keys)
		{
			if(keys == null || keys.Length == 0)
				throw new ArgumentNullException(nameof(keys));

			DataSequenceToken token;
			return _cache.TryRemove(this.GetCacheKey(keys), out token);
		}
		#endregion

		#region 公共方法
		public long Increment(params string[] keys)
		{
			DataSequenceToken token;
			return this.Sequence.Increment(this.GetSequenceKey(keys, out token), 1, token.Seed);
		}

		public long Decrement(params string[] keys)
		{
			DataSequenceToken token;
			return this.Sequence.Decrement(this.GetSequenceKey(keys, out token), 1, token.Seed);
		}
		#endregion

		#region 私有方法
		private string GetCacheKey(string[] keys)
		{
			if(keys == null || keys.Length == 0)
				throw new ArgumentNullException(nameof(keys));

			var result = _dataService.Name.ToLowerInvariant();

			foreach(var key in keys)
			{
				if(string.IsNullOrWhiteSpace(key))
					throw new ArgumentException("The 'keys' parameter contains null or empty string.");

				result += ":" + key.Trim().ToLowerInvariant();
			}

			return result;
		}

		private string GetSequenceKey(string[] keys, out DataSequenceToken token)
		{
			if(keys == null || keys.Length == 0)
				throw new ArgumentNullException(nameof(keys));

			var cacheKey = this.GetCacheKey(keys);

			if(!_cache.TryGetValue(cacheKey, out token))
				throw new InvalidOperationException("The sequence have not yet registered.");

			return "Zongsoft.Data.Sequence:" + cacheKey;
		}
		#endregion

		#region 嵌套子类
		private class DataSequenceToken
		{
			public int Seed;
			public bool Required;
			public string[] Keys;

			public DataSequenceToken(string[] keys, int seed, bool required)
			{
				this.Keys = keys.Select(p => p.Trim()).ToArray();
				this.Seed = seed;
				this.Required = required;
			}
		}
		#endregion
	}
}

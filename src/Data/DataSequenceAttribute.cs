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

namespace Zongsoft.Data
{
	/// <summary>
	/// 表示数据序号生成规则的特性类。
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
	public class DataSequenceAttribute : Attribute
	{
		#region 成员字段
		private int _seed;
		private string _prefix;
		private string[] _keys;
		private string _sequenceName;
		#endregion

		#region 构造函数
		/// <summary>
		/// 构造一个数据序号生成规则的特性实例。
		/// </summary>
		/// <param name="keys">指定要生成的序号键名，如果是复合序号键，则序号键成员名之间使用逗号分隔。支持序号键前缀，请参考备注说明。</param>
		/// <param name="seed">指定要生成的序号种子数。</param>
		/// <param name="sequenceName">指定的序号生成器名，默认为空。</param>
		/// <remarks>
		///		<para>参数<paramref name="keys"/>支持序号键前缀，其内容大致如此：</para>
		///		<code>
		///			[DataSequence("Community:FeedbackId", 100000)]
		///			[DataSearchKey("Key:Subject")]
		///			public class FeedbackService : ServiceBase&lt;Feedback&gt;
		///			{
		///			}
		///		</code>
		/// </remarks>
		public DataSequenceAttribute(string keys, int seed = 1, string sequenceName = null)
		{
			if(string.IsNullOrWhiteSpace(keys))
				throw new ArgumentNullException(nameof(keys));

			var index = keys.LastIndexOf(':');

			if(index >= 0)
			{
				_prefix = keys.Substring(0, index).Trim().ToLowerInvariant();
				keys = keys.Substring(index + 1);
			}

			_keys = keys.Split(',').Where(p => !string.IsNullOrWhiteSpace(p)).Select(p => p.Trim()).ToArray();
			_seed = seed;
			_sequenceName = sequenceName;
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取序号生成器中当前序号键的前缀，该值可用来区隔不同业务系统中实体和序号键属性均相同的情况。
		/// </summary>
		public string Prefix
		{
			get
			{
				return _prefix;
			}
		}

		/// <summary>
		/// 获取或设置要生成序号的数据属性名数组。
		/// </summary>
		public string[] Keys
		{
			get
			{
				return _keys;
			}
		}

		/// <summary>
		/// 获取或设置序号的种子值，即当新建序号时的初始值。
		/// </summary>
		public int Seed
		{
			get
			{
				return _seed;
			}
			set
			{
				_seed = value;
			}
		}

		/// <summary>
		/// 获取或设置序号生成器的名称，默认为空。
		/// </summary>
		public string SequenceName
		{
			get
			{
				return _sequenceName;
			}
			set
			{
				_sequenceName = string.IsNullOrWhiteSpace(value) ? null : value.Trim();
			}
		}
		#endregion
	}
}

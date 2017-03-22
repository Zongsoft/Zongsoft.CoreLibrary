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
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
	public class DataSequenceAttribute : Attribute
	{
		#region 成员字段
		private int _seed;
		private string[] _keys;
		private string _sequenceName;
		#endregion

		#region 构造函数
		public DataSequenceAttribute(string keys, int seed = 1, string sequenceName = null)
		{
			if(string.IsNullOrWhiteSpace(keys))
				throw new ArgumentNullException(nameof(keys));

			_keys = keys.Split(',').Where(p => !string.IsNullOrWhiteSpace(p)).Select(p => p.Trim()).ToArray();
			_seed = seed;
			_sequenceName = sequenceName;
		}
		#endregion

		#region 公共属性
		public string[] Keys
		{
			get
			{
				return _keys;
			}
		}

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

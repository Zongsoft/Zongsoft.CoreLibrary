/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2010-2013 Zongsoft Corporation <http://www.zongsoft.com>
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

namespace Zongsoft.Data
{
	public class Sorting
	{
		#region 成员字段
		private SortingMode _mode;
		private string[] _fields;
		#endregion

		#region 构造函数
		public Sorting()
		{
		}

		public Sorting(params string[] fields) : this(SortingMode.Ascending, fields)
		{
		}

		public Sorting(SortingMode mode, params string[] fields)
		{
			if(fields == null || fields.Length == 0)
				throw new ArgumentNullException("fields");

			_mode = mode;
			_fields = fields;
		}
		#endregion

		#region 公共属性
		public SortingMode Mode
		{
			get
			{
				return _mode;
			}
			set
			{
				_mode = value;
			}
		}

		public string FieldsText
		{
			get
			{
				if(_fields == null || _fields.Length < 1)
					return string.Empty;

				return string.Join(",", _fields);
			}
			set
			{
				if(string.IsNullOrWhiteSpace(value))
					throw new ArgumentNullException();

				_fields = value.Split(',');
			}
		}

		public string[] Fields
		{
			get
			{
				return _fields;
			}
			set
			{
				if(value == null || value.Length == 0)
					throw new ArgumentNullException();

				_fields = value;
			}
		}
		#endregion
	}

	public enum SortingMode
	{
		/// <summary>正序</summary>
		Ascending,
		/// <summary>倒序</summary>
		Descending,
	}
}

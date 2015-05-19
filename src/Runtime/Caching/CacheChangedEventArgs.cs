/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2013-2015 Zongsoft Corporation <http://www.zongsoft.com>
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
using System.ComponentModel;

namespace Zongsoft.Runtime.Caching
{
	[Serializable]
	public class CacheChangedEventArgs : EventArgs
	{
		#region 成员字段
		private CacheChangedReason _reason;
		private string _oldKey;
		private string _newKey;
		private object _oldValue;
		private object _newValue;
		#endregion

		#region 构造函数
		public CacheChangedEventArgs(CacheChangedReason reason, string oldKey, object oldValue, string newKey = null, object newValue = null)
		{
			_reason = reason;
			_oldKey = oldKey;
			_oldValue = oldValue;
			_newKey = newKey;
			_newValue = newValue;
		}
		#endregion

		#region 公共属性
		public CacheChangedReason Reason
		{
			get
			{
				return _reason;
			}
		}

		public string OldKey
		{
			get
			{
				return _oldKey;
			}
		}

		public object OldValue
		{
			get
			{
				return _oldValue;
			}
		}

		public string NewKey
		{
			get
			{
				return _newKey;
			}
		}

		public object NewValue
		{
			get
			{
				return _newValue;
			}
		}
		#endregion
	}
}

/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2015 Zongsoft Corporation <http://www.zongsoft.com>
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
using System.Threading.Tasks;

namespace Zongsoft.Messaging
{
	public abstract class MessageBase : MarshalByRefObject
	{
		#region 常量定义
		private static readonly DateTime MINIMUM_DATETIME = new DateTime(1900, 1, 1);
		#endregion

		#region 成员字段
		private string _id;
		private string _acknowledgementId;
		private byte[] _data;
		private byte[] _checksum;
		private DateTime _expires;
		private DateTime _enqueuedTime;
		private DateTime _dequeuedTime;
		private int _dequeuedCount;
		private byte _priority;
		#endregion

		#region 构造函数
		protected MessageBase(string id, byte[] data, byte[] checksum = null, DateTime? expires = null, DateTime? enqueuedTime = null, DateTime? dequeuedTime = null, int dequeuedCount = 0)
		{
			if(string.IsNullOrWhiteSpace(id))
				throw new ArgumentNullException("id");

			_id = id.Trim();
			_data = data;
			_checksum = checksum;
			_expires = (expires.HasValue ? expires.Value : DateTime.Today.AddYears(50));
			_enqueuedTime = (enqueuedTime.HasValue ? enqueuedTime.Value : MINIMUM_DATETIME);
			_dequeuedTime = (dequeuedTime.HasValue ? dequeuedTime.Value : MINIMUM_DATETIME);
			_dequeuedCount = dequeuedCount;
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取消息的编号。
		/// </summary>
		public string Id
		{
			get
			{
				return _id;
			}
		}

		public string AcknowledgementId
		{
			get
			{
				return _acknowledgementId;
			}
			protected set
			{
				_acknowledgementId = value == null ? null : value.Trim();
			}
		}

		public byte[] Data
		{
			get
			{
				return _data;
			}
			set
			{
				_data = value;
			}
		}

		public byte[] Checksum
		{
			get
			{
				return _checksum;
			}
			set
			{
				_checksum = value;
			}
		}

		public DateTime Expires
		{
			get
			{
				return _expires;
			}
			protected set
			{
				_expires = value;
			}
		}

		public DateTime EnqueuedTime
		{
			get
			{
				return _enqueuedTime;
			}
			protected set
			{
				_enqueuedTime = value;
			}
		}

		public DateTime DequeuedTime
		{
			get
			{
				return _dequeuedTime;
			}
			protected set
			{
				_dequeuedTime = value;
			}
		}

		public int DequeuedCount
		{
			get
			{
				return _dequeuedCount;
			}
			protected set
			{
				_dequeuedCount = value;
			}
		}

		public byte Priority
		{
			get
			{
				return _priority;
			}
			protected set
			{
				_priority = value;
			}
		}
		#endregion

		#region 公共方法
		public virtual DateTime Delay(TimeSpan duration)
		{
			return TaskUtility.ExecuteTask(() => this.DelayAsync(duration));
		}

		public abstract Task<DateTime> DelayAsync(TimeSpan duration);

		public virtual object Acknowledge(object parameter = null)
		{
			return TaskUtility.ExecuteTask(() => this.AcknowledgeAsync(parameter));
		}

		public abstract Task<object> AcknowledgeAsync(object parameter = null);
		#endregion
	}
}

/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2015-2017 Zongsoft Corporation <http://www.zongsoft.com>
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

namespace Zongsoft.Messaging
{
	/// <summary>
	/// 表示主题回调消息的实体类。
	/// </summary>
	public class TopicMessage
	{
		#region 成员字段
		private string _identity;
		private string _messageId;
		private string _tags;
		private byte[] _data;
		private byte[] _checksum;
		private DateTime _timestamp;
		private string _description;
		#endregion

		#region 构造函数
		public TopicMessage()
		{
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取或设置消息的标识。
		/// </summary>
		public string Identity
		{
			get
			{
				return _identity;
			}
			set
			{
				_identity = value;
			}
		}

		/// <summary>
		/// 获取或设置消息的编号。
		/// </summary>
		public string MessageId
		{
			get
			{
				return _messageId;
			}
			set
			{
				_messageId = value;
			}
		}

		/// <summary>
		/// 获取或设置关联的标签。
		/// </summary>
		public string Tags
		{
			get
			{
				return _tags;
			}
			set
			{
				_tags = value;
			}
		}

		/// <summary>
		/// 获取或设置消息内容。
		/// </summary>
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

		/// <summary>
		/// 获取或设置消息校验码。
		/// </summary>
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

		/// <summary>
		/// 获取或设置消息时间戳。
		/// </summary>
		public DateTime Timestamp
		{
			get
			{
				return _timestamp;
			}
			set
			{
				_timestamp = value;
			}
		}

		/// <summary>
		/// 获取或设置消息描述信息。
		/// </summary>
		public string Description
		{
			get
			{
				return _description;
			}
			set
			{
				_description = value;
			}
		}
		#endregion
	}
}

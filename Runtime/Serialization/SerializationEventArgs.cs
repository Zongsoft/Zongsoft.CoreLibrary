/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2013 Zongsoft Corporation <http://www.zongsoft.com>
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
using System.IO;
using System.Collections.Generic;

namespace Zongsoft.Runtime.Serialization
{
	[Serializable]
	public class SerializationEventArgs : EventArgs
	{
		#region 成员字段
		private SerializationDirection _direction;
		private Stream _serializationStream;
		private object _serializationObject;
		#endregion

		#region 构造函数
		public SerializationEventArgs(SerializationDirection direction, Stream serializationStream, object serializationObject)
		{
			if(serializationStream == null)
				throw new ArgumentNullException("serializationStream");

			_direction = direction;
			_serializationStream = serializationStream;
			_serializationObject = serializationObject;
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取当前序列化的调用方向。
		/// </summary>
		public SerializationDirection Direction
		{
			get
			{
				return _direction;
			}
		}

		/// <summary>
		/// 获取序列化流对象。
		/// </summary>
		public Stream SerializationStream
		{
			get
			{
				return _serializationStream;
			}
		}

		/// <summary>
		/// 获取序列化对象。
		/// </summary>
		public object SerializationObject
		{
			get
			{
				return _serializationObject;
			}
		}
		#endregion
	}
}

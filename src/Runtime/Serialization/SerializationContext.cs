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
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace Zongsoft.Runtime.Serialization
{
	public class SerializationContext : MarshalByRefObject
	{
		#region 成员字段
		private ISerializer _serializer;
		private Stream _serializationStream;
		private object _serializationObject;
		private SerializationSettings _settings;
		private IDictionary<string, object> _properties;
		#endregion

		#region 构造函数
		public SerializationContext(ISerializer serializer, Stream serializationStream, object serializationObject, SerializationSettings settings)
		{
			if(serializer == null)
				throw new ArgumentNullException("serializer");

			if(serializationObject == null)
				throw new ArgumentNullException("serializationObject");

			if(serializationStream == null)
				throw new ArgumentNullException("serializationStream");

			_settings = settings;
			_serializer = serializer;
			_serializationStream = serializationStream;
			_serializationObject = serializationObject;
		}
		#endregion

		#region 公共属性
		public ISerializer Serializer
		{
			get
			{
				return _serializer;
			}
		}

		public Stream SerializationStream
		{
			get
			{
				return _serializationStream;
			}
		}

		public object SerializationObject
		{
			get
			{
				return _serializationObject;
			}
		}

		public SerializationSettings Settings
		{
			get
			{
				return _settings;
			}
		}

		public bool HasProperties
		{
			get
			{
				return _properties != null && _properties.Count > 0;
			}
		}

		public IDictionary<string, object> Properties
		{
			get
			{
				if(_properties == null)
					System.Threading.Interlocked.CompareExchange(ref _properties, new ConcurrentDictionary<string, object>(StringComparer.OrdinalIgnoreCase), null);

				return _properties;
			}
		}
		#endregion
	}
}

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
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Zongsoft.Runtime.Caching
{
	[Serializable]
	public class Cache : MarshalByRefObject, System.Runtime.Serialization.ISerializable
	{
		#region 成员字段
		private string _name;
		private object _data;
		private object _parameter;
		private DateTime _timestamp;
		private IDictionary<string, object> _properties;
		#endregion

		#region 构造函数
		public Cache(string name, object data)
		{
			if(string.IsNullOrWhiteSpace(name))
				throw new ArgumentNullException("name");

			if(data == null)
				throw new ArgumentNullException("data");

			_name = name;
			_data = data;
			_timestamp = DateTime.Now;
			_properties = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
		}

		public Cache(SerializationInfo info, StreamingContext context)
		{
			_name = info.GetString("Name");
			_data = info.GetValue("Data", typeof(object));
			_timestamp = info.GetDateTime("Timestamp");
			_parameter = info.GetValue("Parameter", typeof(object));
		}
		#endregion

		#region 公共属性
		public string Name
		{
			get
			{
				return _name;
			}
		}

		public object Data
		{
			get
			{
				return _data;
			}
			set
			{
				if(value == null)
					throw new ArgumentNullException();

				_data = value;
			}
		}

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

		public object Parameter
		{
			get
			{
				return _parameter;
			}
			set
			{
				_parameter = value;
			}
		}

		public IDictionary<string, object> Properties
		{
			get
			{
				return _properties;
			}
		}
		#endregion

		#region 公共方法
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("Name", _name);
			info.AddValue("Data", _data);
			info.AddValue("Timestamp", _timestamp);
			info.AddValue("Parameter", _parameter);
			info.AddValue("Properties", _properties);
		}
		#endregion
	}
}

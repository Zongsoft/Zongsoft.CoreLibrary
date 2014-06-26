/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2011-2013 Zongsoft Corporation <http://www.zongsoft.com>
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
using System.Linq;
using System.Text;

namespace Zongsoft.Communication
{
	public class PackageHeader : Zongsoft.Runtime.Serialization.ISerializable
	{
		#region 成员变量
		private string _name;
		private string _value;
		#endregion

		#region 构造函数
		public PackageHeader(string name, string value)
		{
			if(string.IsNullOrWhiteSpace(name))
				throw new ArgumentNullException("name");

			_name = name.Trim();
			_value = value;
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

		public virtual string Value
		{
			get
			{
				return _value;
			}
		}
		#endregion

		#region 序列方法
		public virtual void Serialize(Stream serializationStream)
		{
			var bufferName = Encoding.UTF8.GetBytes(_name);
			var bufferValue = Encoding.UTF8.GetBytes(_value);

			serializationStream.WriteByte((byte)bufferName.Length);
			serializationStream.WriteByte((byte)bufferValue.Length);

			serializationStream.Write(bufferName, 0, (byte)bufferName.Length);
			serializationStream.Write(bufferValue, 0, (byte)bufferValue.Length);
		}
		#endregion
	}
}

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
using System.Runtime.Serialization;

namespace Zongsoft.Options.Configuration
{
	[Serializable]
	public class OptionConfigurationException : Exception
	{
		#region 成员字段
		private string _fileName;
		#endregion

		#region 构造函数
		internal OptionConfigurationException()
		{
		}

		public OptionConfigurationException(string message) : base(message)
		{
		}

		public OptionConfigurationException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected OptionConfigurationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			_fileName = info.GetString("FileName");
		}
		#endregion

		#region 公共属性
		public string FileName
		{
			get
			{
				return _fileName;
			}
			internal set
			{
				_fileName = value;
			}
		}
		#endregion

		#region 重写方法
		public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			base.GetObjectData(info, context);

			info.AddValue("FileName", _fileName);
		}
		#endregion
	}
}

/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2010-2017 Zongsoft Corporation <http://www.zongsoft.com>
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

namespace Zongsoft.Data
{
	/// <summary>
	/// 表示数据访问的异常类。
	/// </summary>
	public class DataAccessException : DataException
	{
		#region 成员字段
		private int _code;
		private string _driverName;
		#endregion

		#region 构造函数
		public DataAccessException(string driverName, int code)
		{
			_code = code;
			_driverName = driverName;
		}

		public DataAccessException(string driverName, int code, string message) : base(message)
		{
			_code = code;
			_driverName = driverName;
		}

		public DataAccessException(string driverName, int code, Exception innerException) : base(null, innerException)
		{
			_code = code;
			_driverName = driverName;
		}

		public DataAccessException(string driverName, int code, string message, Exception innerException) : base(message, innerException)
		{
			_code = code;
			_driverName = driverName;
		}

		protected DataAccessException(SerializationInfo info, StreamingContext context)
		{
			_code = info.GetInt32("Code");
			_driverName = info.GetString("DriverName");
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取数据访问的错误代码。
		/// </summary>
		public int Code
		{
			get
			{
				return _code;
			}
		}

		/// <summary>
		/// 获取数据访问驱动程序的名称。
		/// </summary>
		public string DriverName
		{
			get
			{
				return _driverName;
			}
		}

		/// <summary>
		/// 获取异常消息文本。
		/// </summary>
		public override string Message
		{
			get
			{
				if(string.IsNullOrWhiteSpace(base.Message))
					return Resources.ResourceUtility.GetString("Text.DataAccessException.Message", this.GetType().Assembly);

				return base.Message;
			}
		}
		#endregion

		#region 重写方法
		public override string ToString()
		{
			return $"[{_driverName}]{_code.ToString()}" + Environment.NewLine + this.Message;
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("Code", _code);
			info.AddValue("DriverName", _driverName);
		}
		#endregion
	}
}

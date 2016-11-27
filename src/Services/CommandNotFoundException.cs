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
using System.Runtime.Serialization;

namespace Zongsoft.Services
{
	public class CommandNotFoundException : CommandException
	{
		#region 成员字段
		private string _path;
		#endregion

		#region 构造函数
		public CommandNotFoundException(string path)
		{
			_path = path ?? string.Empty;
		}

		protected CommandNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			_path = info.GetString("Path");
		}
		#endregion

		#region 公共属性
		public string Path
		{
			get
			{
				return _path;
			}
		}
		#endregion

		#region 重写属性
		public override string Message
		{
			get
			{
				return Resources.ResourceUtility.GetString("CommandNotFound", _path);
			}
		}
		#endregion

		#region 重写方法
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			//调用基类同名方法
			base.GetObjectData(info, context);

			//将定义的属性值加入持久化信息集中
			info.AddValue("Path", _path);
		}
		#endregion
	}
}

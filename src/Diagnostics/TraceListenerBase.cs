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
using System.Collections.Generic;
using System.Text;

namespace Zongsoft.Diagnostics
{
	public abstract class TraceListenerBase : MarshalByRefObject, ITraceListener
	{
		#region 成员变量
		private string _name;
		private ITraceFilter _filter;
		#endregion

		#region 构造函数
		protected TraceListenerBase(string name)
		{
			if(string.IsNullOrEmpty(name))
				throw new ArgumentNullException("name");

			_name = name;
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

		public ITraceFilter Filter
		{
			get
			{
				return _filter;
			}
			set
			{
				_filter = value;
			}
		}
		#endregion

		#region 抽象方法
		public abstract void OnTrace(TraceEntry entry);
		#endregion
	}
}

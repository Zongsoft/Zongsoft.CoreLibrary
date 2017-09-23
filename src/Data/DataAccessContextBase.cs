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
using System.Collections.Generic;

namespace Zongsoft.Data
{
	/// <summary>
	/// 表示数据访问的上下文基类。
	/// </summary>
	public abstract class DataAccessContextBase : MarshalByRefObject
	{
		#region 成员字段
		private string _name;
		private DataAccessMethod _method;
		private IDataAccess _dataAccess;
		private IDictionary<string, object> _states;
		#endregion

		#region 构造函数
		protected DataAccessContextBase(IDataAccess dataAccess, string name, DataAccessMethod method)
		{
			if(dataAccess == null)
				throw new ArgumentNullException(nameof(dataAccess));
			if(string.IsNullOrWhiteSpace(name))
				throw new ArgumentNullException(nameof(name));

			_name = name;
			_method = method;
			_dataAccess = dataAccess;
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

		public DataAccessMethod Method
		{
			get
			{
				return _method;
			}
		}

		public IDataAccess DataAccess
		{
			get
			{
				return _dataAccess;
			}
		}

		public bool HasStates
		{
			get
			{
				return _states != null && _states.Count > 0;
			}
		}

		public IDictionary<string, object> States
		{
			get
			{
				if(_states == null)
					System.Threading.Interlocked.CompareExchange(ref _states, new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase), null);

				return _states;
			}
		}
		#endregion
	}
}

/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2016 Zongsoft Corporation <http://www.zongsoft.com>
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
	/// 为数据访问的执行事件提供数据。
	/// </summary>
	public class DataExecutedEventArgs : EventArgs
	{
		#region 成员字段
		private string _name;
		private object _result;
		private Type _resultType;
		private IDictionary<string, object> _inParameters;
		private IDictionary<string, object> _outParameters;
		#endregion

		#region 构造函数
		public DataExecutedEventArgs(string name, Type resultType, IDictionary<string, object> inParameters, IDictionary<string, object> outParameters, object result)
		{
			if(string.IsNullOrWhiteSpace(name))
				throw new ArgumentNullException("name");

			if(resultType == null)
				throw new ArgumentNullException("resultType");

			_name = name.Trim();
			_result = result;
			_resultType = resultType;
			_inParameters = inParameters;
			_outParameters = outParameters;
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取数据访问的名称。
		/// </summary>
		public string Name
		{
			get
			{
				return _name;
			}
		}

		/// <summary>
		/// 获取一个值，指示当前是否为单值执行方法。
		/// </summary>
		public bool IsScalar
		{
			get
			{
				return !(_resultType != null && Zongsoft.Common.TypeExtension.IsAssignableFrom(typeof(IEnumerable<>), _resultType));
			}
		}

		/// <summary>
		/// 获取查询结果集的实体类型，如果是单值执行方法则返回空(null)。
		/// </summary>
		public Type EntityType
		{
			get
			{
				if(_resultType != null && Zongsoft.Common.TypeExtension.IsAssignableFrom(typeof(IEnumerable<>), _resultType))
					return _resultType.GetGenericArguments()[0];

				return null;
			}
		}

		/// <summary>
		/// 获取或设置执行结果的类型。
		/// </summary>
		public Type ResultType
		{
			get
			{
				return _resultType;
			}
			set
			{
				if(value == null)
					throw new ArgumentNullException();

				_resultType = value;
			}
		}

		/// <summary>
		/// 获取或设置执行操作的结果。
		/// </summary>
		public object Result
		{
			get
			{
				return _result;
			}
			set
			{
				_result = value;
			}
		}

		/// <summary>
		/// 获取执行操作的输入参数。
		/// </summary>
		public IDictionary<string, object> InParameters
		{
			get
			{
				return _inParameters;
			}
		}

		/// <summary>
		/// 获取执行操作的输出参数。
		/// </summary>
		public IDictionary<string, object> OutParameters
		{
			get
			{
				return _outParameters;
			}
		}
		#endregion
	}
}

/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2014-2015 Zongsoft Corporation <http://www.zongsoft.com>
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

namespace Zongsoft.Services.Composition
{
	public class ExecutionContext : MarshalByRefObject, IExecutionContext
	{
		#region 成员字段
		private IExecutor _executor;
		private object _parameter;
		private object _result;
		private Exception _exception;
		private IDictionary<string, object> _extendedProperties;
		#endregion

		#region 构造函数
		public ExecutionContext(IExecutor executor, object parameter = null, IDictionary<string, object> extendedProperties = null)
		{
			if(executor == null)
				throw new ArgumentNullException("executor");

			_executor = executor;
			_parameter = parameter;

			if(extendedProperties != null && extendedProperties.Count > 0)
				_extendedProperties = new Dictionary<string, object>(extendedProperties);
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取处理本次执行请求的执行器。
		/// </summary>
		public virtual IExecutor Executor
		{
			get
			{
				return _executor;
			}
		}

		/// <summary>
		/// 获取处理本次执行请求的输入参数。
		/// </summary>
		public object Parameter
		{
			get
			{
				return _parameter;
			}
		}

		/// <summary>
		/// 获取本次执行中发生的异常。
		/// </summary>
		public virtual Exception Exception
		{
			get
			{
				return _exception;
			}
			internal protected set
			{
				_exception = value;
			}
		}

		/// <summary>
		/// 获取扩展属性集是否有内容。
		/// </summary>
		/// <remarks>
		///		<para>在不确定扩展属性集是否含有内容之前，建议先使用该属性来检测。</para>
		/// </remarks>
		public virtual bool HasExtendedProperties
		{
			get
			{
				return (_extendedProperties != null);
			}
		}

		/// <summary>
		/// 获取扩展属性集。
		/// </summary>
		public virtual IDictionary<string, object> ExtendedProperties
		{
			get
			{
				if(_extendedProperties == null)
					System.Threading.Interlocked.CompareExchange(ref _extendedProperties, new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase), null);

				return _extendedProperties;
			}
		}

		/// <summary>
		/// 获取或设置本次执行的返回结果。
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
		#endregion
	}
}

/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2010-2014 Zongsoft Corporation <http://www.zongsoft.com>
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
	public class ExecutorContext : MarshalByRefObject, IExecutorContext
	{
		#region 成员字段
		private IExecutor _executor;
		private object _parameter;
		private Dictionary<string, object> _extendedProperties;
		private object _result;
		#endregion

		#region 构造函数
		public ExecutorContext(IExecutor executor, object parameter = null, IDictionary<string, object> extendedProperties = null)
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
		public IExecutor Executor
		{
			get
			{
				return _executor;
			}
		}

		/// <summary>
		/// 获取或设置本次执行请求的调用参数。
		/// </summary>
		public virtual object Parameter
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
		/// 获取或设置当前执行器的返回结果。
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

		#region 公共方法
		/// <summary>
		/// 将本次执行请求的调用参数对象转换为指定类型。
		/// </summary>
		/// <typeparam name="T">泛型参数，表示转换后的参数类型。</typeparam>
		/// <returns>返回类型转换后的参数对象。</returns>
		/// <remarks>
		///		<para>该方法使用<seealso cref="Zongsoft.Common.Convert"/>类的ConvertValue<typeparamref name="T"/>方法进行类型转换，详细的转换逻辑请参考其说明。</para>
		/// </remarks>
		public T GetParameter<T>()
		{
			object parameter = this.Parameter;

			return Zongsoft.Common.Convert.ConvertValue<T>(parameter);
		}

		/// <summary>
		/// 通过指定的转换委托将本次执行请求的调用参数对象转换为指定类型。
		/// </summary>
		/// <typeparam name="T">泛型参数，表示转换后的参数类型。</typeparam>
		/// <param name="convert">指定的参数类型转换的委托。</param>
		/// <returns>返回类型转换后的参数对象。</returns>
		/// <remarks>
		///		<para>如果类型转换委托<paramref name="convert"/>参数值为空，则使用<seealso cref="Zongsoft.Common.Convert"/>类的ConvertValue<typeparamref name="T"/>方法进行类型转换，详细的转换逻辑请参考其说明。</para>
		/// </remarks>
		public T GetParameter<T>(Func<object, T> convert)
		{
			object parameter = this.Parameter;

			if(convert == null)
				return Zongsoft.Common.Convert.ConvertValue<T>(parameter);
			else
				return convert(parameter);
		}
		#endregion
	}
}

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
using System.Linq;
using System.Text;

namespace Zongsoft.Services.Composition
{
	[Serializable]
	public class ExecutionContext : MarshalByRefObject
	{
		#region 成员字段
		private object _parameter;
		private Executor _executor;
		private ExecutionPipeline _pipeline;
		private Dictionary<string, object> _extendedProperties;
		private object _result;
		#endregion

		#region 构造函数
		internal protected ExecutionContext(Executor executor) : this(executor, null, null)
		{
		}

		internal protected ExecutionContext(Executor executor, object parameter) : this(executor, parameter, null)
		{
		}

		internal ExecutionContext(Executor executor, object parameter, ExecutionPipeline pipeline)
		{
			if(executor == null)
				throw new ArgumentNullException("executor");

			_executor = executor;
			_parameter = parameter;
			_pipeline = pipeline;
			_extendedProperties = null;
		}

		protected ExecutionContext(ExecutionContext context)
		{
			if(context == null)
				throw new ArgumentNullException("context");

			_executor = context.Executor;
			_parameter = context.Parameter;
			_pipeline = context.Pipeline;
			_extendedProperties = context._extendedProperties;
		}
		#endregion

		#region 公共属性
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
		/// 获取本次执行请求的执行器。
		/// </summary>
		public Executor Executor
		{
			get
			{
				return _executor;
			}
		}

		/// <summary>
		/// 获取本次执行请求的处理器。
		/// </summary>
		public IExecutionHandler Handler
		{
			get
			{
				if(_pipeline == null)
					return null;

				return _pipeline.Handler;
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
					System.Threading.Interlocked.CompareExchange(ref _extendedProperties, new Dictionary<string, object>(), null);

				return _extendedProperties;
			}
		}

		/// <summary>
		/// 获取或设置当前执行的返回结果。
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

		#region 保护属性
		protected internal ExecutionPipeline Pipeline
		{
			get
			{
				return _pipeline;
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

		#region 内部方法
		internal void SetPipeline(ExecutionPipeline pipeline)
		{
			if(_pipeline != null)
				throw new InvalidOperationException();

			_pipeline = pipeline;
		}
		#endregion
	}
}

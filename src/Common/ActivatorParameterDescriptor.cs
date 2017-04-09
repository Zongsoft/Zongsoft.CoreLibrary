/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2017 Zongsoft Corporation <http://www.zongsoft.com>
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

namespace Zongsoft.Common
{
	public class ActivatorParameterDescriptor
	{
		#region 成员字段
		private object _argument;
		private string _parameterName;
		private Type _parameterType;
		private object _parameterValue;
		#endregion

		#region 构造函数
		public ActivatorParameterDescriptor(string parameterName, Type parameterType, object argument)
		{
			if(string.IsNullOrWhiteSpace(parameterName))
				throw new ArgumentNullException(nameof(parameterName));
			if(parameterType == null)
				throw new ArgumentNullException(nameof(parameterType));

			_parameterName = parameterName;
			_parameterType = parameterType;
			_argument = argument;
		}
		#endregion

		#region 公共属性
		public string ParameterName
		{
			get
			{
				return _parameterName;
			}
		}

		public Type ParameterType
		{
			get
			{
				return _parameterType;
			}
		}

		public object Argument
		{
			get
			{
				return _argument;
			}
		}

		public object ParameterValue
		{
			get
			{
				return _parameterValue;
			}
			set
			{
				_parameterValue = value;
			}
		}
		#endregion
	}
}

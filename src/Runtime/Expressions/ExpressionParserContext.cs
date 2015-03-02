/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2014 Zongsoft Corporation <http://www.zongsoft.com>
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

namespace Zongsoft.Runtime.Expressions
{
	public class ExpressionParserContext : MarshalByRefObject
	{
		#region 成员字段
		private IExpressionAnalyzer _analyzer;
		private object _parameter;
		private object _value;
		#endregion

		#region 构造函数
		public ExpressionParserContext(IExpressionAnalyzer analyzer, object parameter, object value)
		{
			if(analyzer == null)
				throw new ArgumentNullException("analyzer");

			_analyzer = analyzer;
			_parameter = parameter;
			_value = value;
		}
		#endregion

		#region 公共属性
		public IExpressionAnalyzer Analyzer
		{
			get
			{
				return _analyzer;
			}
		}

		public object Parameter
		{
			get
			{
				return _parameter;
			}
		}

		public object Value
		{
			get
			{
				return _value;
			}
		}
		#endregion
	}
}

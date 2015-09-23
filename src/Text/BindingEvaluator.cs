/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2015 Zongsoft Corporation <http://www.zongsoft.com>
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

namespace Zongsoft.Text
{
	public class BindingEvaluator : TemplateEvaluatorBase
	{
		#region 构造函数
		public BindingEvaluator() : base("binding")
		{
		}

		public BindingEvaluator(string schema) : base(schema)
		{
		}
		#endregion

		#region 评估方法
		public override object Evaluate(TemplateEvaluatorContext context)
		{
			if(context.Data == null)
				return null;

			if(string.IsNullOrWhiteSpace(context.Text))
				return context.Data;

			var index = context.Text.IndexOf('#');
			var result = Zongsoft.Common.Convert.GetValue(context.Data, (index > 0 ? context.Text.Substring(0, index) : context.Text));

			if(index > 0 && index < context.Text.Length - 1)
				return string.Format("{0:" + context.Text.Substring(index + 1) + "}", result);

			return result;
		}
		#endregion
	}
}

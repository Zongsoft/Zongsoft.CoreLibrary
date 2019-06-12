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

namespace Zongsoft.Text.Evaluation
{
	public class RandomEvaluator : TemplateEvaluatorBase
	{
		#region 构造函数
		public RandomEvaluator() : base("random")
		{
		}

		public RandomEvaluator(string scheme) : base(scheme)
		{
		}
		#endregion

		#region 重写方法
		public override object Evaluate(TemplateEvaluatorContext context)
		{
			if(string.IsNullOrWhiteSpace(context.Text))
				return this.GetDefaultRandom();

			switch(context.Text.ToLowerInvariant())
			{
				case "byte":
					return Zongsoft.Common.RandomGenerator.Generate(1)[0].ToString();
				case "short":
				case "int16":
					return ((ushort)Zongsoft.Common.RandomGenerator.GenerateInt32()).ToString();
				case "int":
				case "int32":
					return ((uint)Zongsoft.Common.RandomGenerator.GenerateInt32()).ToString();
				case "long":
				case "int64":
					return ((ulong)Zongsoft.Common.RandomGenerator.GenerateInt64()).ToString();
				case "guid":
				case "uuid":
					return Guid.NewGuid().ToString("n");
			}

			int length;

			if(Zongsoft.Common.Convert.TryConvertValue<int>(context.Text, out length))
				return Zongsoft.Common.RandomGenerator.GenerateString(Math.Max(length, 1));

			return this.GetDefaultRandom();
		}
		#endregion

		#region 私有方法
		private string GetDefaultRandom()
		{
			return Zongsoft.Common.RandomGenerator.GenerateString(6);
		}
		#endregion
	}
}

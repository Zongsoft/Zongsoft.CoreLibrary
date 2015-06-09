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

namespace Zongsoft.Services
{
	public class Matcher : IMatcher
	{
		#region 单例字段
		public static readonly Matcher Default = new Matcher();
		#endregion

		#region 匹配方法
		public virtual bool Match(object target, object parameter)
		{
			if(target == null)
				return false;

			var matchable = target as IMatchable;

			if(matchable != null)
				return matchable.IsMatch(parameter);

			var attribute = (MatcherAttribute)Attribute.GetCustomAttribute(target.GetType(), typeof(MatcherAttribute), true);

			if(attribute != null && attribute.Matcher != null)
				return attribute.Matcher.Match(target, parameter);

			//注意：默认返回必须是真
			return true;
		}
		#endregion
	}
}

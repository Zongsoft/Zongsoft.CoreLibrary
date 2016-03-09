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

namespace Zongsoft.Services
{
	public class PredicationCollection : Zongsoft.Collections.Collection<IPredication>, IPredication
	{
		#region 成员字段
		private PredicationCombination _combine;
		#endregion

		#region 构造函数
		public PredicationCollection() : this(PredicationCombination.Or)
		{
		}

		public PredicationCollection(PredicationCombination combine)
		{
			_combine = combine;
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取或设置断言集合内各断言的逻辑组合方式。
		/// </summary>
		public PredicationCombination Combination
		{
			get
			{
				return _combine;
			}
			set
			{
				_combine = value;
			}
		}
		#endregion

		#region 断言方法
		/// <summary>
		/// 对断言集合内的所有断言进行遍历断言调用，并根据<see cref="Combination"/>属性值进行组合判断。
		/// </summary>
		/// <param name="parameter">对断言集合内所有断言调用时的传入参数。</param>
		/// <returns>集合内所有断言的组合结果，如果集合为空则始终返回真(true)。</returns>
		/// <remarks>
		///		<para>在调用过程中如果是“或”组合则会发生“真”短路；如果是“与”组合则会发生“假”短路。</para>
		/// </remarks>
		public bool Predicate(object parameter)
		{
			var predications = base.Items;

			if(predications == null || predications.Count < 1)
				return true;

			foreach(var predication in predications)
			{
				if(predication == null)
					continue;

				if(predication.Predicate(parameter))
				{
					if(_combine == PredicationCombination.Or)
						return true;
				}
				else
				{
					if(_combine == PredicationCombination.And)
						return false;
				}
			}

			return _combine == PredicationCombination.Or ? false : true;
		}
		#endregion
	}
}

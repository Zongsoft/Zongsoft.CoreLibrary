/*
 * Authors:
 *   钟峰(Popeye Zhong) <9555843@qq.com>
 *
 * Copyright (C) 2010-2017 Zongsoft Corporation <http://www.zongsoft.com>
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

namespace Zongsoft.Reflection
{
	public class MemberSettingContext<T> : MemberGettingContext
	{
		#region 成员字段
		private T _value;
		private int _valueEvaluated;
		private Func<T> _valueFactory;
		#endregion

		#region 构造函数
		internal MemberSettingContext(object owner, MemberToken memberToken, T value, MemberGettingContext parent = null) : base(owner, memberToken, parent)
		{
			_value = value;
			_valueEvaluated = 1;
		}

		internal MemberSettingContext(object owner, MemberToken memberToken, Func<T> valueFactory, MemberGettingContext parent = null) : base(owner, memberToken, parent)
		{
			if(valueFactory == null)
				throw new ArgumentNullException(nameof(valueFactory));

			_valueFactory = valueFactory;
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取设置的值。
		/// </summary>
		/// <remarks>
		///		<para>如果指定的是一个取值工厂，即使多次获取该属性亦可确保取值计算只会被调用一次。</para>
		/// </remarks>
		public T Value
		{
			get
			{
				if(_valueFactory != null && _valueEvaluated == 0 && System.Threading.Interlocked.CompareExchange(ref _valueEvaluated, 1, 0) == 0)
					_value = _valueFactory();

				return _value;
			}
		}
		#endregion

		#region 公共方法
		public void Setup(bool throwsOnError = true)
		{
			MemberAccess.SetMemberValueCore(this.Owner, this.MemberToken, () => this.Value, throwsOnError);
		}
		#endregion
	}
}

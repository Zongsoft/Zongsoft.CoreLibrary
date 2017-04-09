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

namespace Zongsoft.Common
{
	/// <summary>
	/// 表示对象实例的创建器的接口。
	/// </summary>
	public interface IActivator
	{
		/// <summary>
		/// 返回一个值指示是否能创建指定类型的实例。
		/// </summary>
		/// <param name="type">指定是否可创建的类型。</param>
		/// <param name="argument">指定的判断参数。</param>
		/// <returns>返回是否能创建的值，如果为真(True)表示可以创建，否则不能创建。</returns>
		bool CanCreate(Type type, object argument);

		/// <summary>
		/// 创建指定类型的实例。
		/// </summary>
		/// <param name="type">指定要创建的对象类型。</param>
		/// <param name="argument">指定的创建参数。</param>
		/// <param name="binder">指定的构造函数参数绑定委托。</param>
		/// <returns>返回创建的对象实例。</returns>
		object Create(Type type, object argument, Func<ActivatorParameterDescriptor, bool> binder = null);
	}
}

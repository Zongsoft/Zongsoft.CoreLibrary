/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2005-2008 Zongsoft Corporation <http://www.zongsoft.com>
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
using System.ComponentModel;
using System.Collections.Generic;

namespace Zongsoft.Options
{
	/// <summary>
	/// 表示选项的基本功能定义，由 <seealso cref="Zongsoft.Options.Option"/> 类实现。
	/// </summary>
	/// <remarks>建议选项的实现者从 <see cref="Zongsoft.Options.Option"/> 基类继承。</remarks>
	public interface IOption
	{
		#region 事件定义
		event EventHandler Changed;
		event EventHandler Applied;
		event EventHandler Resetted;
		event CancelEventHandler Applying;
		event CancelEventHandler Resetting;
		#endregion

		#region 属性定义
		object OptionObject
		{
			get;
		}

		IOptionView View
		{
			get;
			set;
		}

		IOptionViewBuilder ViewBuilder
		{
			get;
			set;
		}

		ICollection<IOptionProvider> Providers
		{
			get;
		}

		bool IsDirty
		{
			get;
		}
		#endregion

		#region 方法定义
		void Reset();
		void Apply();
		#endregion
	}
}

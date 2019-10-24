/*
 *   _____                                ______
 *  /_   /  ____  ____  ____  _________  / __/ /_
 *    / /  / __ \/ __ \/ __ \/ ___/ __ \/ /_/ __/
 *   / /__/ /_/ / / / / /_/ /\_ \/ /_/ / __/ /_
 *  /____/\____/_/ /_/\__  /____/\____/_/  \__/
 *                   /____/
 *
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@qq.com>
 *
 * Copyright (C) 2016-2019 Zongsoft Corporation <http://www.zongsoft.com>
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

namespace Zongsoft.Data
{
	/// <summary>
	/// 表示数据查询条件的实体基类。
	/// </summary>
	public abstract class ConditionalBase : IModel
	{
		#region 构造函数
		protected ConditionalBase()
		{
		}
		#endregion

		#region 抽象方法
		protected abstract int GetCount();
		protected abstract bool HasChanges(string[] names);
		protected abstract IDictionary<string, object> GetChanges();
		protected abstract void Reset(string[] names);
		protected abstract bool Reset(string name, out object value);
		protected abstract bool TryGetValue(string name, out object value);
		protected abstract bool TrySetValue(string name, object value);
		#endregion

		#region 显式实现
		int IModel.GetCount()
		{
			return this.GetCount();
		}

		bool IModel.HasChanges(params string[] names)
		{
			return this.HasChanges(names);
		}

		IDictionary<string, object> IModel.GetChanges()
		{
			return this.GetChanges();
		}

		void IModel.Reset(params string[] names)
		{
			this.Reset(names);
		}

		bool IModel.Reset(string name, out object value)
		{
			return this.Reset(name, out value);
		}

		bool IModel.TryGetValue(string name, out object value)
		{
			return this.TryGetValue(name, out value);
		}

		bool IModel.TrySetValue(string name, object value)
		{
			return this.TrySetValue(name, value);
		}
		#endregion
	}
}

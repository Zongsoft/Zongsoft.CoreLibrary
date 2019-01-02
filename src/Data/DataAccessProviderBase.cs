/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2010-2018 Zongsoft Corporation <http://www.zongsoft.com>
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
using System.Collections;
using System.Collections.Generic;

namespace Zongsoft.Data
{
	public abstract class DataAccessProviderBase<TDataAccess> : IDataAccessProvider, ICollection<TDataAccess> where TDataAccess : IDataAccess
	{
		#region 成员字段
		private readonly Collections.INamedCollection<TDataAccess> _accesses;
		#endregion

		#region 构造函数
		protected DataAccessProviderBase()
		{
			_accesses = new Collections.NamedCollection<TDataAccess>(p => p.Name, StringComparer.OrdinalIgnoreCase);
		}
		#endregion

		#region 公共属性
		public int Count
		{
			get
			{
				return _accesses.Count;
			}
		}
		#endregion

		#region 公共方法
		public IDataAccess GetAccessor(string name)
		{
			if(string.IsNullOrEmpty(name))
				throw new ArgumentNullException(nameof(name));

			if(_accesses.TryGet(name, out var accessor))
				return accessor;

			lock(_accesses)
			{
				if(_accesses.TryGet(name, out accessor))
					return accessor;

				_accesses.Add(accessor = this.CreateAccessor(name));
			}

			return accessor;
		}
		#endregion

		#region 抽象方法
		protected abstract TDataAccess CreateAccessor(string name);
		#endregion

		#region 集合接口
		bool ICollection<TDataAccess>.IsReadOnly
		{
			get
			{
				return false;
			}
		}

		void ICollection<TDataAccess>.Add(TDataAccess item)
		{
			if(item == null)
				throw new ArgumentNullException(nameof(item));

			_accesses.Add(item);
		}

		void ICollection<TDataAccess>.Clear()
		{
			_accesses.Clear();
		}

		bool ICollection<TDataAccess>.Contains(TDataAccess item)
		{
			if(item == null)
				return false;

			return _accesses.Contains(item);
		}

		void ICollection<TDataAccess>.CopyTo(TDataAccess[] array, int arrayIndex)
		{
			_accesses.CopyTo(array, arrayIndex);
		}

		bool ICollection<TDataAccess>.Remove(TDataAccess item)
		{
			return _accesses.Remove(item);
		}
		#endregion

		#region 枚举遍历
		public IEnumerator<TDataAccess> GetEnumerator()
		{
			return _accesses.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _accesses.GetEnumerator();
		}
		#endregion
	}
}

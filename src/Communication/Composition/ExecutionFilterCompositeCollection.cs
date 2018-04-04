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
using System.Linq;

namespace Zongsoft.Communication.Composition
{
	public class ExecutionFilterCompositeCollection : Zongsoft.Collections.NamedCollectionBase<ExecutionFilterComposite>, ICollection<IExecutionFilter>
	{
		#region 构造函数
		public ExecutionFilterCompositeCollection() : base(StringComparer.OrdinalIgnoreCase)
		{
		}
		#endregion

		#region 公共方法
		public ExecutionFilterComposite Add(IExecutionFilter filter)
		{
			return this.Add(filter, null);
		}

		public ExecutionFilterComposite Add(IExecutionFilter filter, Services.IPredication predication)
		{
			if(filter == null)
				throw new ArgumentNullException("filter");

			var result = new ExecutionFilterComposite(filter, predication);
			base.Add(result);

			return result;
		}

		public bool Contains(IExecutionFilter item)
		{
			if(item == null)
				return false;

			return this.ContainsName(item.Name);
		}

		public void CopyTo(IExecutionFilter[] array, int arrayIndex)
		{
			if(array == null)
				throw new ArgumentNullException(nameof(array));

			if(arrayIndex < 0 || arrayIndex >= array.Length)
				throw new ArgumentOutOfRangeException(nameof(arrayIndex));

			var iterator = base.GetEnumerator();

			for(int i = arrayIndex; i < array.Length; i++)
			{
				if(iterator.MoveNext())
					array[i] = iterator.Current?.Filter;
			}
		}

		public bool Remove(IExecutionFilter item)
		{
			if(item == null)
				return false;

			return base.RemoveItem(item.Name);
		}
		#endregion

		#region 重写方法
		protected override string GetKeyForItem(ExecutionFilterComposite item)
		{
			return item.Filter.Name;
		}

		//protected override bool TryConvertItem(object value, out ExecutionFilterComposite item)
		//{
		//	if(value is IExecutionFilter)
		//	{
		//		item = new ExecutionFilterComposite((IExecutionFilter)value);
		//		return true;
		//	}

		//	return base.TryConvertItem(value, out item);
		//}
		#endregion

		#region 接口实现
		bool ICollection<IExecutionFilter>.IsReadOnly
		{
			get
			{
				return false;
			}
		}

		void ICollection<IExecutionFilter>.Add(IExecutionFilter item)
		{
			this.Add(item);
		}

		IEnumerator<IExecutionFilter> IEnumerable<IExecutionFilter>.GetEnumerator()
		{
			var iterator = base.GetEnumerator();

			while(iterator.MoveNext())
			{
				yield return iterator.Current?.Filter;
			}
		}
		#endregion
	}
}
